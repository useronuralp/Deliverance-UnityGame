using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine.AI;
public class EnemyCombatBehaviour : CombatBehaviour
{
    private GameObject             m_Player;
    private State                  m_PlayerState;
    private FileIO                 m_FileIO;
    private List<TrainingData>     m_TrainingData;
    private NeuralNetwork          m_NeuralNetwork;
    private bool                   m_Frenzy;
    private bool                   m_DidFrenzy;
    private float                  m_FrenzyTimer;
    private float                  m_FrenzyDuration = 5.0f;
    private HealthStamina          m_HealthStaminaScript;
    private int                    m_ConsecutiveAttackLimit;
    private bool                   m_IsInCoroutine = false;
    private CombatBehaviour        m_PlayerCombatScript; 
    private bool                   m_WantToWander;
    private EventManager           s_EventManager;
    protected override void Awake()
    {
        base.Awake();
        m_Player = GameObject.FindWithTag("Player");
    }
    private struct TrainingData
    {
        public int   EnemyAction;
        public float ReactionTime;
        public float Stamina;
        public int   Action;

        public void Print()
        {
            Debug.Log("Enemy Action: " + EnemyAction);
            Debug.Log("Reaction Time: " + ReactionTime);
            Debug.Log("Stamina: " + Stamina);
            Debug.Log("Action: " + Action);
        }
    }
    private void Start()
    {
        s_EventManager                     = EventManager.GetInstance();
        s_EventManager.OnAIStopsWandering += OnAIStopsWandering; //Subscribe to the corresponding event.

        m_WantToWander                     = false;
        m_IsInCoroutine                    = false;                                      //Check to see whether a coroutine is running.
        m_ConsecutiveAttackLimit           = Random.Range(3, 8);                         //Target integer that after which AI will stop attacking and recover stamina.
        m_DidFrenzy                        = false; 
        m_HealthStaminaScript              = GetComponent<HealthStamina>();
        m_FrenzyTimer                      = m_FrenzyDuration; 
        m_Frenzy                           = false;                                      //This will set to true when AI is allowed to chain / combo attacks.
        m_TrainingData                     = new List<TrainingData>(); 
        m_FileIO                           = new FileIO(); 
        List<string> data                  = m_FileIO.ReadFromFile("TrainingDataAggressive.txt");  //Get the data from the .txt line by line.
        m_NeuralNetwork                    = new NeuralNetwork(new int[]{5, 25, 25, 5}); //Create the correct layout for the NN.
        m_PlayerCombatScript               = m_Player.GetComponent<CombatBehaviour>();
#if true
        //TODO: Remove training at the start. Load an already trained NN.
        for (int i = 0; i < data.Count; i++)
        {
            if(data[i].Contains("Data"))
            {
                var enemyAction  = Regex.Match(data[i + 1], @"\d+").Groups[0].Value;
                var stamina = Regex.Match(data[i + 2], @"\d+").Groups[0].Value;
                var reactionTime = Regex.Match(data[i + 3], @"([-+]?[0-9]*\.?[0-9]+)").Groups[0].Value;
                var action = Regex.Match(data[i + 4], @"\d+").Groups[0].Value;

                m_FileIO.ParseFLOAT(stamina, out float staminaFLOAT);
                m_FileIO.ParseFLOAT(reactionTime, out float reactionTimeFLOAT);
                m_FileIO.ParseINT(enemyAction, out int enemyActionINT);
                m_FileIO.ParseINT(action, out int actionINT);

                m_TrainingData.Add(new TrainingData { EnemyAction = enemyActionINT, Stamina = staminaFLOAT, ReactionTime = reactionTimeFLOAT, Action = actionINT });
            }
        }
        foreach(TrainingData dt in m_TrainingData) //Prepare neurons with the data collected from the .txt file.
        {
            //Input Neurons
            float kick = 0.0f;
            float punch = 0.0f;
            float parry = 0.0f;
            float guard = 0.0f;
            float idle = 0.0f;
            //float stamina = 0.0f;
            //Reaction Ranges
            //float R1 = 0.0f;
            //float R2 = 0.0f;
            //float R3 = 0.0f;
            //float R4 = 0.0f;
            //float R5 = 0.0f;
            //float R6 = 0.0f;
        
            //Output Neurons
            float actionKick = 0.0f;
            float actionPunch = 0.0f;
            float actionParry = 0.0f;
            float actionIdle = 0.0f;
            float actionGuard = 0.0f;

            //Setup
            switch (dt.EnemyAction)
            {
                case 0: idle = 1.0f; break;
                case 1: kick = 1.0f; break;
                case 2: punch = 1.0f; break;
                case 3: parry = 1.0f; break;
                case 4: guard = 1.0f; break;
            }

            //if (dt.Stamina >= 10.0f)
            //    stamina = 1.0f;
            //if(dt.ReactionTime >= 0.0f && dt.ReactionTime < 0.1f)
            //{
            //    R1 = 1.0f;
            //}
            //else if(dt.ReactionTime >= 0.1f && dt.ReactionTime < 0.2f)
            //{
            //    R2 = 1.0f;
            //}
            //else if (dt.ReactionTime >= 0.2f && dt.ReactionTime < 0.3f)
            //{
            //    R3 = 1.0f;
            //}
            //else if (dt.ReactionTime >= 0.3f && dt.ReactionTime < 0.4f)
            //{
            //    R4 = 1.0f;
            //}
            //else if (dt.ReactionTime >= 0.4f && dt.ReactionTime < 0.5f)
            //{
            //    R5 = 1.0f;
            //}
            //else if (dt.ReactionTime >= 0.5)
            //{
            //    R6 = 1.0f;
            //}
        
            switch (dt.Action)
            {
                case 0: actionIdle = 1.0f; break;
                case 1: actionKick = 1.0f; break;
                case 2: actionPunch = 1.0f; break;
                case 3: actionParry = 1.0f; break;
                case 4: actionGuard = 1.0f; break;
            }
        
            //Train the network.
            m_NeuralNetwork.FeedForward(new float[] { idle, kick, punch, parry, guard});
            m_NeuralNetwork.BackProp(new float[] { actionIdle, actionKick, actionPunch, actionParry, actionGuard });
        }      
#endif
    }


    void Update()
    {
        //Debug.Log(m_ConsecutiveAttackCount);
        //Debug.Log(m_PreventAttacktInputs);
        if (m_Animator.GetBool("isDead")) //Check if the character is dead at the start.
        {
            enabled = false;
        }
        if(m_HealthStaminaScript.m_CurrentHealth <= 50) //AI goes into a frenzy mode (starts to chain attacks) if it's health drops below 50.
        {
            //m_DidFrenzy = true;
            m_Frenzy = true;
        }
        //if (m_Frenzy)
        //{
        //    m_FrenzyTimer -= Time.deltaTime;
        //    if (m_FrenzyTimer <= 0.0f)
        //        m_Frenzy = false;
        //}
        if(m_ConsecutiveAttackCount == m_ConsecutiveAttackLimit) //After throwing a random number of attacks in range 3-8, AI starts to wander a bit to let the player breathe.
        {
            m_WantToWander = true;
            s_EventManager.AIWantsToWander(); //Trigger the event.
            m_ConsecutiveAttackCount = 0; //Reset the counter for the next check.
            m_ConsecutiveAttackLimit = Random.Range(3, 8); //Get a new random attack limit.
        }
        if (m_IsStunned)
        {
            m_StunTimer -= Time.deltaTime;
            if (m_StunTimer <= 0)
            {
                m_Animator.SetBool("isStunned", false);
                m_StunTimer = m_StunDuration;
                m_IsStunned = false;
            }
        }
        else if(!m_IsStunned)
        {
           ObservePlayer(); 
           if (Vector3.Distance(m_Player.transform.position, transform.position) < 3.3f)
           {
                if (!m_WantToWander)
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
           }
        }
        if(m_IsAttacking)
        {
            s_EventManager.AIIsAttacking();
        }
        else
        {
            s_EventManager.AIIsNotAttacking();
        }
        //HardCodedAI();
    }
    void TakeAction()
    {
        //if (m_PlayerState == Moves.IDLE)
        //    return;


        //Prepare the neurons.
        float kick = 0.0f;
        float punch = 0.0f;
        float parry = 0.0f;
        float guard = 0.0f;
        float idle = 0.0f;
        //float stamina = 0.0f;
        //float R1 = 0.0f;
        //float R2 = 0.0f;
        //float R3 = 0.0f;
        //float R4 = 0.0f;
        //float R5 = 0.0f;
        //float R6 = 0.0f;


        switch ((int)m_PlayerState)
        {
            case 0: idle = 1.0f; break;
            case 1: kick = 1.0f; break;
            case 2: punch = 1.0f; break;
            case 3: parry = 1.0f; break;
            case 4: guard = 1.0f; break;
        }
        //if (GetComponent<HealthStamina>().m_CurrentStamina >= 10.0f)
        //    stamina = 1.0f;
        //
        //float rndReactionTime = Random.value;
        //if (rndReactionTime >= 0.0f && rndReactionTime < 0.1f)
        //{
        //    R1 = 1.0f;
        //}
        //else if (rndReactionTime >= 0.1f && rndReactionTime < 0.2f)
        //{
        //    R2 = 1.0f;
        //}
        //else if (rndReactionTime >= 0.2f && rndReactionTime < 0.3f)
        //{
        //    R3 = 1.0f;
        //}
        //else if (rndReactionTime >= 0.3f && rndReactionTime < 0.4f)
        //{
        //    R4 = 1.0f;
        //}
        //else if (rndReactionTime >= 0.4f && rndReactionTime < 0.5f)
        //{
        //    R5 = 1.0f;
        //}
        //else if (rndReactionTime >= 0.5)
        //{
        //    R6 = 1.0f;
        //}

        //Feed the data to the NN and get a result.
        float[] output = m_NeuralNetwork.FeedForward(new float[] { idle, kick, punch, parry, guard });


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

        
        foreach (var element in neurons.Reverse())
        {
            Debug.Log(element.Key);
        }
        
        foreach (int number in possibleChoices)
        {
            Debug.Log(number);
        }

        int choice = possibleChoices[0]; //Set the choice to the highest action decided by the AI first.

        for(int i = 1; i < possibleChoices.Count; i++) //With each iteration, it gets harder to land in the preferred percentage since iterations are dependent on previous ones.
        {
            if (Random.value < 0.3f) //Decide on whether to choose the next best option in the list randomly.
            {
                choice = possibleChoices[i]; 
                break; //If we hit this control path and changed the previous preffered attack. Break out the loop.
            }
        }
        //if(Random.Range(0.0f, 100.0f) <= 50.0f)
        //{
        //    choice = Random.Range(1, 5);
        //}
        if(!m_IsInCoroutine)
        {
            switch (choice)
            {
                case 0:
                    //if (!m_IsParryingFull)
                    //    Parry();
                    break;
                case 1:
                    switch (Random.Range(0,4))
                    {
                        case 0: ThrowAttack(m_NormalStance.UpKick, m_Kicks[m_NormalStance.UpKick.Head], m_Player); break;
                        case 1: ThrowAttack(m_NormalStance.LeftKick,  m_Kicks[m_NormalStance.LeftKick.Head], m_Player); break;
                        case 2: ThrowAttack(m_NormalStance.DownKick, m_Kicks[m_NormalStance.DownKick.Head], m_Player); break;
                        case 3: ThrowAttack(m_NormalStance.RightKick, m_Kicks[m_NormalStance.RightKick.Head], m_Player); break;
                    }
                    break;
                case 2:
                    switch (Random.Range(0, 3))
                    {
                        case 0: ThrowAttack(m_NormalStance.UpPunch, m_Punches[m_NormalStance.UpPunch.Head], m_Player); break;
                        case 1: ThrowAttack(m_NormalStance.RightPunch, m_Punches[m_NormalStance.RightPunch.Head], m_Player); break;
                        case 2: ThrowAttack(m_NormalStance.LeftPunch, m_Punches[m_NormalStance.LeftPunch.Head], m_Player); break;
                    }
                    break;
                case 3:
                    StartCoroutine(DelayedParry(Random.Range(0.0f, 0.4f))); //You can adjust the parry timing here to increase the difficulty of the AI.
                    break;
                case 4:
                    StartCoroutine(Guard());
                    break;
            }                
        }
    }
    void ObservePlayer() //Gets the current state of the player.
    {
        if (m_Player)
        {
            if (m_PlayerCombatScript.m_IsParryingFull)
            {
                m_PlayerState = State.PARRY;
            }
            else if (m_PlayerCombatScript.m_IsGuarding || m_PlayerCombatScript.m_IsBlocking)
            {
                m_PlayerState = State.GUARD;
            }
            else if (m_PlayerCombatScript.m_IsAttacking)
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
            else if (m_PlayerCombatScript.m_IsIdle)
            {
                m_PlayerState = State.IDLE;
            }
        }
    }
    IEnumerator Guard()
    {
        if(!m_IsInCoroutine)
        {
            if (!m_IsAttacking && !m_IsGettingHit && !m_IsParryingFull && !m_IsStunned)
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
    IEnumerator DelayedParry(float time)
    {
        if(!m_IsInCoroutine)
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
    void HardCodedAI()
    {
        if (!m_IsStunned)
        {
            //Parry();
            //ThrowAttack("TopPunchRight", "RightHand");
            //m_Animator.SetBool("isGuarding", true);
        }
    }
    private void OnAIStopsWandering()
    {
        m_WantToWander = false;
    }
}