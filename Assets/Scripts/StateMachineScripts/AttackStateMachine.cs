using UnityEngine;

/// <summary>
/// This is added to every distinct attack move state machine there is in the game.
/// sm_ prefix stands for State Machine and is used inside state machine behaviours.
/// </summary>
public class AttackStateMachine : StateMachineBehaviour
{
    private GameObject        sm_AttachedObject;             //This script is attached to an animator and that animator is also attached to a GameObject. ( This Script --> Animator --> GameObject )
    private MovementBehaviour sm_Movementscript;             //Movement script of the m_AttachedObject will be stored here.
    private CombatBehaviour   sm_CombatScript;               //Combat script of the m_AttachedObject will be stored here.
    private float             sm_AnimationSnapStartTimer;    //Animation SNAP cooldown, taken from the combat script of the attached game object.
    private float             sm_AnimationSlideTimer;        //Animation SLIDE time, taken from the combat script of the attached game object.
    private string            sm_CurrentAttack;              //Full name of the current attack that is being performed by the character. Taken from the combat script. (Eg: NormalStance_LeftKick_1, TopKickRight, etc...)
    private float             sm_CharacterSlideSpeed;        //The speed with which we move the character to its target during the sliding mechanic of the attacks.
    private BoxCollider       sm_HurtBox;                    //HurtBox of the character.
    private float             sm_ElapsedTime;                //Counting the elapsed time when an attack animaiton starts. This is that timer.
    private float             sm_ComboWindowTimer;           //For the duration of this variable, player will be able to input an attack that the previous attack chains into.

    private float             sm_LandingTime;
    private const float       sm_NormalStance_DownKick_1_WaitingConstant = 0.4f;  //HurtBox related waiting time constants. Used during the restoration of the HurtBoxe dimensions back to their original sizes.
    private const float       sm_NormalStance_DownKick_2_WaitingConstant = 0.4f;  //HurtBox related waiting time constants. Used during the restoration of the HurtBoxe dimensions back to their original sizes.
    private const float       sm_NormalStance_UpKick_1_WaitingConstant   = 0.25f; //HurtBox related waiting time constants. Used during the restoration of the HurtBoxe dimensions back to their original sizes.
    private const float       sm_NormalStance_UpKick_2_WaitingConstant   = 0.25f; //HurtBox related waiting time constants. Used during the restoration of the HurtBoxe dimensions back to their original sizes.
    private float             sm_DistanceToTargetDelta;

    private GameObject        sm_HitIndicator = null;
    private Rigidbody         sm_AttachedObjectRigidBody;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Do these first-----
        sm_AttachedObject = animator.gameObject;
        sm_CombatScript   = sm_AttachedObject.GetComponent<CombatBehaviour>();
        sm_Movementscript = sm_AttachedObject.GetComponent<MovementBehaviour>();
        sm_HurtBox        = sm_AttachedObject.GetComponent<BoxCollider>();
        //-------------------


        //Defaulting variables before starting.
        sm_HurtBox.center                      = sm_CombatScript.m_HurtBoxDimensions;
        sm_ComboWindowTimer                    = sm_CombatScript.m_ComboWindowDuration;
        sm_ElapsedTime                         = 0.0f;
        sm_CombatScript.m_StateElapesedTime    = 0.0f;
        sm_CurrentAttack                       = sm_CombatScript.m_CurrentAttack;
        sm_CombatScript.m_IsAttacking          = true;
        sm_CombatScript.m_PreventAttacktInputs = true;  
        sm_Movementscript.m_DisableMovement    = true;
        sm_CombatScript.m_IsIdle               = false;
        sm_CombatScript.m_CanCombo             = false;
        sm_CombatScript.m_IsGuarding           = false;
        sm_CombatScript.m_IsParrying           = false;
        sm_CombatScript.m_IsInParryingAnimation       = false;
        sm_CombatScript.m_IsGettingHit         = false;
        sm_CombatScript.m_IsBlocking           = false;
        sm_CombatScript.m_IsStunned            = false;
        sm_CombatScript.m_DidHitLand           = false;
        sm_CharacterSlideSpeed                 = 7.0f;
        sm_AnimationSnapStartTimer             = sm_CombatScript.m_SnapStartTimers[sm_CombatScript.m_CurrentAttack]; //Get the current animations SNAP start time from the cache.
        sm_AnimationSlideTimer                 = sm_CombatScript.m_SlideTimes[sm_CombatScript.m_CurrentAttack];     //Get the current animations SLIDE duration from the cache.
        sm_LandingTime                         = sm_CombatScript.m_LandingTimes[sm_CurrentAttack];
        sm_CombatScript.m_ConsecutiveAttackCount++;


        if(sm_AttachedObject.CompareTag("EnemyAI")) //If the caller of this state is EnemyAI, Set the attack lights to active.
        {
            DisableAllIndicators(sm_AttachedObject.transform);
            switch (sm_CombatScript.m_LimbName)
            {
                case "RightHand": sm_HitIndicator = sm_CombatScript.m_OffensiveColliders[sm_CombatScript.m_LimbName].transform.Find("IndicatorRightHand").gameObject; break;
                case "LeftHand": sm_HitIndicator =  sm_CombatScript.m_OffensiveColliders[sm_CombatScript.m_LimbName].transform.Find("IndicatorLeftHand").gameObject;   break;
                case "RightLeg": sm_HitIndicator =  sm_CombatScript.m_OffensiveColliders[sm_CombatScript.m_LimbName].transform.Find("IndicatorRightLeg").gameObject;   break;
                case "LeftLeg": sm_HitIndicator =   sm_CombatScript.m_OffensiveColliders[sm_CombatScript.m_LimbName].transform.Find("IndicatorLeftLeg").gameObject;     break;
            }

            sm_HitIndicator.SetActive(true);
        }
        sm_AttachedObjectRigidBody         = sm_AttachedObject.GetComponent<Rigidbody>();
        sm_CombatScript.m_ComboWindowStart = sm_CombatScript.m_CancelCooldowns[sm_CurrentAttack];
        sm_CombatScript.m_ComboCount++; //Counting the combo count each time an attack state machine is triggered. Reliable way of counting combo.
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        sm_CombatScript.m_DidHitLand = false;
        if (sm_ElapsedTime >= sm_CombatScript.m_LandingTimes[sm_CurrentAttack] - 0.15f)
        {
            sm_CombatScript.m_OffensiveColliders[sm_CombatScript.m_LimbName].enabled = true;  //ENABLE collider.
        }
        if(!sm_CombatScript.m_IsInParryingAnimation)
        {
            sm_CombatScript.m_HitParticles[sm_CombatScript.m_LimbName].Emit(2); //Emit particles during the attack.
            sm_CombatScript.m_PreventAttacktInputs = true;
            sm_Movementscript.m_DisableMovement    = true;
            sm_CombatScript.m_IsAttacking          = true;
            sm_CombatScript.m_IsIdle               = false;
            sm_LandingTime                         -= Time.deltaTime;
            sm_AnimationSnapStartTimer             -= Time.deltaTime; //Snap cooldown timer for animation.
            sm_ElapsedTime                         += Time.deltaTime; //Starts when the attack animation starts.
            sm_CombatScript.m_StateElapesedTime    = sm_ElapsedTime;  //Propogate the elapsed time info to the combat script.
        }

        if (sm_AnimationSnapStartTimer <= 0)  //When snap cooldown ends.
        {
            sm_AnimationSlideTimer -= Time.deltaTime;  //Another timer, starts only when the above "sm_AnimationSnapCooldownTimer" ends.
            if(sm_CurrentAttack == sm_CombatScript.m_CurrentAttack)
            {
                if (sm_CurrentAttack == "NormalStance_DownKick_1")
                {
                    sm_HurtBox.center = new Vector3(sm_HurtBox.center.x, 0.0f, sm_HurtBox.center.z); //Decrease the height of the HurtBox during this attack since the character crouches during this attack.
                }
                else if (sm_CurrentAttack == "NormalStance_UpKick_1")
                {
                    sm_HurtBox.center = new Vector3(sm_HurtBox.center.x, 1.5f, sm_HurtBox.center.z); //Increase the height of the HurtBox during this attack sine the character is jumping.
                }
                else if (sm_CurrentAttack == "NormalStance_DownKick_2")
                {
                    sm_HurtBox.center = new Vector3(sm_HurtBox.center.x, 0.0f, sm_HurtBox.center.z); //Decrease the height of the HurtBox during this attack since the character crouches during this attack.
                }
                else if (sm_CurrentAttack == "NormalStance_UpKick_2")
                {
                    sm_HurtBox.center = new Vector3(sm_HurtBox.center.x, 1.5f, sm_HurtBox.center.z); //Increase the height of the HurtBox during this attack sine the character is jumping.
                }
            }
        }
        else
        {
            if(sm_Movementscript.m_LockTarget)
            {
                RotateTowards(sm_Movementscript.m_LockTarget, 700.0f); //Look towards the attack target at a humanely possible rate. If the rate gets too high it causes unrealistic behaviour.
            }
        }

        //---------------------------------------------------Sliding Start---------------------------------------------------------
        if (sm_Movementscript.m_LockTarget)  //Where we apply the snapping mechanic of the attacks..
        {
            sm_DistanceToTargetDelta = Vector3.Distance(sm_AttachedObject.transform.position, sm_Movementscript.m_LockTarget.transform.position);
            if ( sm_AnimationSnapStartTimer < 0 && sm_AnimationSlideTimer  > 0.0f && sm_DistanceToTargetDelta >= 1.1f) //Checking for the necessary conditions. I think the variable names are self explanatory.
            {
                sm_AttachedObjectRigidBody.MovePosition(sm_AttachedObject.transform.position + sm_CharacterSlideSpeed * Time.deltaTime * sm_AttachedObject.transform.forward);
            }
        }
        //----------------------------------------------------Sliding End----------------------------------------------------------

        if (sm_LandingTime <= 0.0f)  //Check if the landing time is is smaller than 0.
        {
            sm_CombatScript.m_OffensiveColliders[sm_CombatScript.m_LimbName].enabled = false; //Disable the correct collider(s) AS SOON AS THE IMPACT POINT IS REACHED.
            sm_CombatScript.m_DidHitLand = true; //Tell the combat script that the threating part of the animaiton has ended.


            if(sm_Movementscript.m_LockTarget)
            {
                RotateTowards(sm_Movementscript.m_LockTarget, 400.0f); 
            }
            //TODO: MISSING HIT SOUND LOGIC 

            if (sm_CurrentAttack == "NormalStance_DownKick_1" && sm_AnimationSlideTimer + sm_NormalStance_DownKick_1_WaitingConstant <= 0.0f) //Wait a little bit before restoring the hurtbox back to its original values. Constant at the end is the tested optimal wait time.
            {
                sm_HurtBox.center = sm_CombatScript.m_HurtBoxDimensions; 
            }
            else if (sm_CurrentAttack == "NormalStance_UpKick_1" && sm_AnimationSlideTimer + sm_NormalStance_UpKick_1_WaitingConstant <= 0.0f) //Wait a little bit before restoring the hurtbox back to its original values. Constant at the end is the tested optimal wait time.
            {
                sm_HurtBox.center = sm_CombatScript.m_HurtBoxDimensions; 
            }
            else if (sm_CurrentAttack == "NormalStance_DownKick_2" && sm_AnimationSlideTimer + sm_NormalStance_DownKick_2_WaitingConstant <= 0.0f) //Wait a little bit before restoring the hurtbox back to its original values. Constant at the end is the tested optimal wait time.
            {
                sm_HurtBox.center = sm_CombatScript.m_HurtBoxDimensions;
            }
            else if (sm_CurrentAttack == "NormalStance_UpKick_2" && sm_AnimationSlideTimer + sm_NormalStance_UpKick_2_WaitingConstant <= 0.0f) //Wait a little bit before restoring the hurtbox back to its original values. Constant at the end is the tested optimal wait time.
            {
                sm_HurtBox.center = sm_CombatScript.m_HurtBoxDimensions;
            }
        }
        //----------------------------------------------------Animation Cancel / Combo-----------------------------------------------
        if(sm_ElapsedTime >= sm_CombatScript.m_CancelCooldowns[sm_CurrentAttack]) //Check if the elapsed time until the start of the animation has reached the animation cancel window.
        {
            sm_ComboWindowTimer -= Time.deltaTime;  //Decrease cancel window timer every frame. 
            if (sm_ComboWindowTimer > 0.0f) //This is the duration of the window that where we give the player a chance to input a combo attack.
            {                                                                            
                if(sm_CombatScript.m_CurrentAttack == sm_CurrentAttack) //If the attack in this state and the one in the actual character script are same, then it means that user has not input an attack yet and thus we let him input during this window. 
                {                                                                 
                    if(!sm_CombatScript.m_LockAttacking) //Check if the user is spamming the attack before the combo window timer is reached. If so, just punish him and take his comboin abilty away by locking this state.
                    {
                        sm_CombatScript.m_PreventAttacktInputs = false;
                        sm_CombatScript.m_CanCombo = true;
                    }
                    else
                    {
                        sm_CombatScript.m_PreventAttacktInputs = true;
                        sm_CombatScript.m_CanCombo = false;
                    }
                }
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        sm_HurtBox.center  = sm_CombatScript.m_HurtBoxDimensions; //Restore the HurtBox back to its original values. Note : This should already be taken care of by the time the finite state reaches here, but still, I am setting it here again just in case.
        
        if(sm_CombatScript.m_IsGettingHit) //Check if the transitioned state is "Getting-Hit" state, if so set the flags / variables accordingly before leaving.
        {
            sm_CombatScript.m_IsAttacking   = false;
            sm_CombatScript.m_CanCombo      = false;
            sm_CombatScript.m_LockAttacking = false;
            sm_CombatScript.m_CurrentAttack = "None";
            sm_CombatScript.m_HitParticles[sm_CombatScript.m_LimbName].Stop();
            sm_CombatScript.m_NormalStance.ResetStance();
            DisableActiveHitIndicator();
        }
        else if(sm_CombatScript.m_IsInParryingAnimation) //Check if the transitioned state is parrying, if so set the flags accordingly before leaving.
        {
            sm_CombatScript.m_IsIdle               = false;
            sm_CombatScript.m_IsAttacking          = false;
            sm_CombatScript.m_PreventAttacktInputs = true;
            sm_Movementscript.m_DisableMovement    = true;
            sm_CombatScript.m_CanCombo             = false;
            sm_CombatScript.m_LockAttacking        = false;
            sm_CombatScript.m_CurrentAttack        = "None";
            DisableActiveHitIndicator();
        }
        else if(sm_CurrentAttack == sm_CombatScript.m_CurrentAttack) //The case where the character left this state without comboing.
        {
            sm_CombatScript.m_IsIdle               = true;
            sm_CombatScript.m_IsAttacking          = false;
            sm_CombatScript.m_PreventAttacktInputs = false;
            sm_Movementscript.m_DisableMovement    = false;
            sm_CombatScript.m_CanCombo             = false;
            sm_CombatScript.m_LockAttacking        = false;
            sm_CombatScript.m_CurrentAttack        = "None";
            sm_CombatScript.m_StateElapesedTime    = 0.0f;
            sm_CombatScript.m_HitParticles[sm_CombatScript.m_LimbName].Stop();
            sm_CombatScript.m_ComboCount = 0;
            sm_CombatScript.m_NormalStance.ResetStance();

            sm_CombatScript.m_IsParrying            = false;
            sm_CombatScript.m_IsInParryingAnimation = false;
            DisableActiveHitIndicator();
        }
        //Disable all the colliders upon exit.
        foreach (var nameColliderPair in sm_CombatScript.m_OffensiveColliders)
        {
            nameColliderPair.Value.enabled = false;
        }

    }
    private void RotateTowards(GameObject targetObject, float turnSpeed) //Helper fnc to rotate the character slowly.
    {
        Vector3 targetRotation = targetObject.transform.position - sm_AttachedObject.transform.position;
        targetRotation = new Vector3(targetRotation.x, 0, targetRotation.z); //Zero out Y axis.
        sm_AttachedObjectRigidBody.MoveRotation(Quaternion.RotateTowards(sm_AttachedObjectRigidBody.rotation, Quaternion.LookRotation(targetRotation), turnSpeed * Time.deltaTime));
    }


    //These functions are for AI only. Do an if check before you use them or you will crash the game.
    private void DisableAllIndicators(Transform root)
    {
        foreach (Transform child in root)
        {
            if(child.name.Contains("Indicator"))
            {
                child.gameObject.SetActive(false);
            }
            DisableAllIndicators(child);
        }
    }
    private void DisableActiveHitIndicator()
    {
        if (sm_AttachedObject.CompareTag("EnemyAI"))
        {
            sm_HitIndicator.SetActive(false);
        }
    }
}
