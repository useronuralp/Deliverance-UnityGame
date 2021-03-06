using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
public class EnemyCombatBehaviour : CombatBehaviour
{
    private GameObject             m_Player;                 //Player gameObject.
    private State                  m_PlayerState;            //The state of the player. (Attack, guard, etc)
    private FileIO                 m_FileIO;                 //Used to read data from raw training data for the neural network.
    private List<TrainingData>     m_TrainingDataAgressive;  //Agressive data is stored here.
    private List<TrainingData>     m_TrainingDataDefensive;  //Defensive data is stored here.
    private NeuralNetwork          m_NeuralNetworkAgressive; //Aggressive NN.
    private NeuralNetwork          m_NeuralNetworkDefensive; //Defensive NN.
    private bool                   m_Frenzy;                 //Flag that determines if the AI should go into a frenzy (being able to combo) mode.
    private HealthStamina          m_HealthStaminaScript;
    private int                    m_ConsecutiveAttackLimit; //The limit at which the AI will take a break from attacking and let the player breathe.
    private bool                   m_IsInCoroutine = false;
    private CombatBehaviour        m_PlayerCombatScript; 
    private bool                   m_WantToWander;
    private EventManager           s_EventManager;           //Reference to the static event manager.

    //AI difficulty exposed to the editor.
    public float ParryLowerBound = 0.0f;        //Lowest reaction time that the AI can give.
    public float ParryUpperBound = 0.4f;        //Highest reaction time that the AI can give.
    public bool  BeAgressive = true;            //Flag that tells AI to switch to the aggressive neural network.


    public int ConsecutiveAttackLowerBound = 3; //The AI will take a break from throwing attack randomly between these two variables.
    public int ConsecutiveAttackUpperBound = 8;


    public bool CanFrenzy = false;              //Flag that determines the AI instance will have the ability to frenzy.
    public bool CanParry = false;               //Flag that determines the AI instance will have the ability to parry.

    public bool ShouldGoDefensive = false;      //Flag that determines the AI instance will start using the defensive neural network when its health drops below 50%.
    protected override void Awake()
    {
        base.Awake();
        m_Player = GameObject.FindWithTag("Player");
    }

    private void Start()
    {
        s_EventManager                     = EventManager.GetInstance();
        s_EventManager.OnAIStopsWandering += OnAIStopsWandering;                                                     //Subscribe to the corresponding event.
        m_WantToWander                     = false;
        m_IsInCoroutine                    = false;                                                                  
        m_ConsecutiveAttackLimit           = Random.Range(ConsecutiveAttackLowerBound, ConsecutiveAttackUpperBound); 
        m_HealthStaminaScript              = GetComponent<HealthStamina>();
        m_Frenzy                           = false;
        m_TrainingDataAgressive            = new List<TrainingData>();
        m_TrainingDataDefensive            = new List<TrainingData>();
        m_FileIO                           = new FileIO(); 
        List<string> dataAgressive         = m_FileIO.ReadFromFile("TrainingDataAggressive.txt");                    //Get the data from the .txt line by line.
        List<string> dataDefensive         = m_FileIO.ReadFromFile("TrainingDataDefensive.txt");                     //Get the data from the .txt line by line.
        m_NeuralNetworkAgressive           = new NeuralNetwork(new int[]{5, 25, 25, 5});                             //Create the correct layout for the NN.
        m_NeuralNetworkDefensive           = new NeuralNetwork(new int[] { 5, 25, 25, 5 });                          //Create the correct layout for the NN.
        m_PlayerCombatScript               = m_Player.GetComponent<CombatBehaviour>();
#if true
        //Currently there is no loading happening from a file for the neural networks. I train AI every time the game starts running. This is bad and good at the same time.
        //Con: Obviously, not the way to go. Since the AI I have is very small, the performance hit is not apparent at all, but as the training times get bigger, this will start causing problems
        //Pros: The AI will have a different NN brain every time the game is launched. This introduces variety.
        TrainNeuralNetwork(dataAgressive, m_TrainingDataAgressive, m_NeuralNetworkAgressive);
        TrainNeuralNetwork(dataDefensive, m_TrainingDataDefensive, m_NeuralNetworkDefensive);
#endif
    }
    void Update()
    {
        if(!PauseMenu.IsGamePaused)
        {
            if (m_Animator.GetBool("isDead")) //Check if the character is dead at the start.
            {
                s_EventManager.OnAIStopsWandering -= OnAIStopsWandering;
                enabled = false;
            }
            if(m_HealthStaminaScript.m_CurrentHealth <= 50) //AI goes into a frenzy mode (starts to chain attacks) if it's health drops below 50.
            {
                if(ShouldGoDefensive)
                {
                    BeAgressive = false;
                }
                m_Frenzy = true;
            }
            if((m_ConsecutiveAttackCount == m_ConsecutiveAttackLimit) || (m_HealthStaminaScript.m_CurrentStamina < 5.0f)) //After throwing a random number of attacks in the pre-determined range, AI starts to wander a bit to let the player breathe.
            {
                m_WantToWander = true;
                s_EventManager.AIWantsToWander(); //Trigger the event.
                m_ConsecutiveAttackCount = 0; //Reset the counter for the next check.
                m_ConsecutiveAttackLimit = Random.Range(ConsecutiveAttackLowerBound, ConsecutiveAttackUpperBound); //Get a new random attack limit.
            }
            if (m_IsStunned) //Handle the stun case.
            {
                m_StunTimer -= Time.deltaTime;
                if (m_StunTimer <= 0)
                {
                    m_Animator.SetBool("isStunned", false);
                    m_StunTimer = m_StunDuration;
                    m_IsStunned = false;
                }
            }
            else if(!m_IsStunned) //AI is allowed to move and attack when it is not stunned.
            {
               ObservePlayer(); //Gets the player state.
               if (Vector3.Distance(m_Player.transform.position, transform.position) < 3.3f) //If in attack range.
               {
                    if (!m_WantToWander)
                    {
                        if(CanFrenzy)
                        {
                            if (m_Frenzy)
                            {

                                if (!m_PreventAttacktInputs) //AI is allowed to combo during frenzy.
                                    TakeAction();
                            }
                            else
                            {
                                TakeAction();
                            }
                        }
                        else
                        {
                            TakeAction();
                        }
                    }
               }
            }
            if(m_IsAttacking)
            {
                s_EventManager.AIIsAttacking(); //Fire the event for listeners.
            }
            else
            {
                s_EventManager.AIIsNotAttacking(); //Fire the event for listeners.
            }
        }
    }
    void TakeAction()
    {
        //Prepare the neurons.
        float kick = 0.0f;
        float punch = 0.0f;
        float parry = 0.0f;
        float guard = 0.0f;
        float idle = 0.0f;

        switch ((int)m_PlayerState)
        {
            case 0: idle = 1.0f; break;
            case 1: kick = 1.0f; break;
            case 2: punch = 1.0f; break;
            case 3: parry = 1.0f; break;
            case 4: guard = 1.0f; break;
        }
        //Feed the data to the NN and get a result.
        float[] output;
        if(BeAgressive) //Depending on this flag, use the corresponding neural network.
        {
            output = m_NeuralNetworkAgressive.FeedForward(new float[] { idle, kick, punch, parry, guard });
        }
        else
        {
            output = m_NeuralNetworkDefensive.FeedForward(new float[] { idle, kick, punch, parry, guard });
        }

        //Order of the nodes will always be:  0 - > IDLE, 1 - > KICK, 2 - > PUNCH, 3 - > PARRY, 4 - > GUARD.
        SortedDictionary<float, int> neurons = new SortedDictionary<float, int> {{ output[0], 0 },{ output[1], 1 },{ output[2], 2},{ output[3], 3 },{ output[4], 4 } }; //Sorting neurons depending on their values in ascending order.

        KeyValuePair<float, int> previousNeuron = neurons.Last(); // Initialize this neuron to the highest valued node that we got from the NN.
        List<int> possibleChoices = new List<int>(); //This list will hold the feasable choices that AI can switch between.

        foreach(var element in neurons.Reverse()) 
        {
            if (previousNeuron.Key - element.Key <= 0.3f) //Check the difference between the previous node and the current one.
            {
                previousNeuron = element;
                possibleChoices.Add(element.Value); //If the difference is smaller than a certain value, we say that AI's mind is not fully made between the two moves, therefore, we save it to the possibleChoices list.
                //Eg: Output A has: 0.8 magnitude, Output B has: 0.6 magnitude. Rest has 0.0. The difference between nodes A and B is 0.2. In this case, a normal human would choose the action A more frequently while still choosing the action B occasionaly.
                //Like, 2/3 of the choices would be output A while the remaining 1/3 would be output B. This is how a human would play the game. That is why I am calculating possible choices in here.
            }
            else
                break; //Break out of the loop as soon as we see a difference in neurons that is bigger than a certain value. This means that the remaining neurons are not good candidates for possible choices.
        }

        int choice = possibleChoices[0]; //Set the choice to the highest action decided by the AI first.

        for(int i = 1; i < possibleChoices.Count; i++) //With each iteration, it gets harder to land in the preferred percentage since iterations are dependent on previous ones.
        {
            if (Random.value < 0.3f) //Decide on whether to choose the next best option in the list randomly.
            {
                choice = possibleChoices[i]; 
                break; //If we hit this control path and changed the current preffered attack. Break out the loop. 
            }
        }
        if(!m_IsInCoroutine)
        {
            switch (choice)
            {
                case 0: //Does nothing.
                    break;
                case 1: //Throws a random kick. Instead of randomizing, a second neural network could be used here to decide which attack to throw.
                    switch (Random.Range(0, 4))
                    {
                        case 0: ThrowAttack(m_NormalStance.UpKick, m_Kicks[m_NormalStance.UpKick.Head], m_Player); break;
                        case 1: ThrowAttack(m_NormalStance.LeftKick,  m_Kicks[m_NormalStance.LeftKick.Head], m_Player); break;
                        case 2: ThrowAttack(m_NormalStance.DownKick, m_Kicks[m_NormalStance.DownKick.Head], m_Player); break;
                        case 3: ThrowAttack(m_NormalStance.RightKick, m_Kicks[m_NormalStance.RightKick.Head], m_Player); break;
                    }
                    break;
                case 2: //Throws a random punch. Instead of randomizing, a second neural network could be used here to decide which attack to throw.
                    switch (Random.Range(0, 3))
                    {
                        case 0: ThrowAttack(m_NormalStance.UpPunch, m_Punches[m_NormalStance.UpPunch.Head], m_Player); break;
                        case 1: ThrowAttack(m_NormalStance.RightPunch, m_Punches[m_NormalStance.RightPunch.Head], m_Player); break;
                        case 2: ThrowAttack(m_NormalStance.LeftPunch, m_Punches[m_NormalStance.LeftPunch.Head], m_Player); break;
                    }
                    break;
                case 3: //Parries.
                    if (CanParry && m_IsIdle) StartCoroutine(DelayedParry(Random.Range(ParryLowerBound, ParryUpperBound)));
                    break;
                case 4: //Guards.
                    if (m_IsIdle) StartCoroutine(Guard());
                    break;
            }                
        }
    }
    void ObservePlayer() //Gets the current state of the player.
    {
        if (m_Player)
        {
            if (m_PlayerCombatScript.m_IsInParryingAnimation)
            {
                m_PlayerState = State.PARRY;
            }
            else if (m_PlayerCombatScript.m_IsGuarding || m_PlayerCombatScript.m_IsBlocking)
            {
                m_PlayerState = State.GUARD;
            }
            else if (m_PlayerCombatScript.m_IsAttacking && !m_PlayerCombatScript.m_DidHitLand)
            {
                if (m_PlayerCombatScript.m_CurrentAttack.Contains("Kick"))
                {
                    m_PlayerState = State.KICK;
                }
                else if (m_PlayerCombatScript.m_CurrentAttack.Contains("Punch"))
                {
                    m_PlayerState = State.PUNCH;
                }
            }
            else if(m_PlayerCombatScript.m_IsAttacking && m_PlayerCombatScript.m_DidHitLand)
            {
                m_PlayerState = State.IDLE;
            }
            else if (m_PlayerCombatScript.m_IsIdle)
            {
                m_PlayerState = State.IDLE;
            }
        }
    }
    IEnumerator Guard() // AI guards for three seconds and then lowers its guard. TODO: The waiting time could determined with another neural network instead of hard-coding.
    {
        if(!m_IsInCoroutine)
        {
            if (!m_IsAttacking && !m_IsGettingHit && !m_IsInParryingAnimation && !m_IsStunned)
            {
                m_IsInCoroutine = true;
                GuardUp();
                yield return new WaitForSecondsRealtime(3.0f);
                GuardReleased();
                m_IsInCoroutine = false;
            }
            else
                yield return null;
        }
    }
    IEnumerator DelayedParry(float time) //To integrate the reaction time element to parries the AI throws, I use a corouitine here. It will wait for a random time before throwing a parry.                                  
    {                                    //For example, the reaction time of an average human being is in between 0.2f - 0.5f.
        if (!m_IsInCoroutine)
        {
            if (!m_PreventAttacktInputs)
            {
                m_IsInCoroutine = true;
                yield return new WaitForSeconds(time);
                Parry();
                m_IsInCoroutine = false;
            }
            else
                yield return null;
        }
    }
    private void OnAIStopsWandering()
    {
        m_WantToWander = false;
    }
    void TrainNeuralNetwork(List<string> rawData, List<TrainingData> trainingData, NeuralNetwork neuralNetwork)
    {
        FileIO.PopulateTrainingData(rawData, out trainingData);

        foreach (TrainingData data in trainingData) //Prepare neurons with the data collected from the .txt file.
        {
            //Input Neurons
            float isEnemyKicking = 0.0f;
            float isEnemyPunching = 0.0f;
            float isEnemyParrying = 0.0f;
            float isEnemyGuarding = 0.0f;
            float isEnemyIdling = 0.0f;
            //Output Neurons
            float doKicking = 0.0f;
            float doPunching = 0.0f;
            float doParrying = 0.0f;
            float doIdlling = 0.0f;
            float doGuarding = 0.0f;

            //Setup
            switch (data.EnemyAction) //This is the action the opponent of the player took while recording the data.
            {
                case 0: isEnemyIdling = 1.0f; break;
                case 1: isEnemyKicking = 1.0f; break;
                case 2: isEnemyPunching = 1.0f; break;
                case 3: isEnemyParrying = 1.0f; break;
                case 4: isEnemyGuarding = 1.0f; break;
            }
            switch (data.CorrectActionToTake) //This is the action the player took to counter the move above.
            {
                case 0: doIdlling = 1.0f; break;
                case 1: doKicking = 1.0f; break;
                case 2: doPunching = 1.0f; break;
                case 3: doParrying = 1.0f; break;
                case 4: doGuarding = 1.0f; break;
            }
            //Train the network.
            neuralNetwork.FeedForward(new float[] { isEnemyIdling, isEnemyKicking, isEnemyPunching, isEnemyParrying, isEnemyGuarding }); //Feed-forwards the enemy action.
            neuralNetwork.BackProp(new float[] { doIdlling, doKicking, doPunching, doParrying, doGuarding }); //Back prop the correct answer.
        }
    }
}