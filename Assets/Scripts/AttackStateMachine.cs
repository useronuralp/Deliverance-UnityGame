using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is added to every distinct attack move state machine there is in the game.
/// sm_ prefix stands for State Machine and is used inside state machine behaviours.
/// </summary>
public class AttackStateMachine : StateMachineBehaviour
{
    private GameObject        sm_AttachedObject;                     //This script is attached to an animator and that animator is also attached to a GameObject. That is what is stored here. ( This Script --> Animator --> GameObject )
    private MovementBehaviour sm_Movementscript;                     //Movement script of the m_AttachedObject will be stored here.
    private CombatBehaviour   sm_CombatScript;                       //Combat script of the m_AttachedObject will be stored here.
    private float             sm_AnimationSnapCooldownTimer;         //Animation SNAP cooldown, taken from the combat script of the attached game object.
    private float             sm_AnimationSlideTimer;                //Animation SLIDE time, taken from the combat script of the attached game object.
    private string            sm_CurrentAttack;                      //Full name of the current attack that is being performed by the character. Taken from the combat script. (Eg: NormalStance_LeftKick_1, TopKickRight, etc...)
    private float             sm_CharacterSlideSpeed;                //The speed with which we move the character to its target during the sliding mechanic of the attacks.
    private BoxCollider       sm_HurtBox;                            //HurtBox of the character.
    private float             sm_ElapsedTime;                        //Counting the elapsed time when an attack animaiton starts. This is that timer.
    private float             sm_ComboWindowTimer;                   //For the duration of this variable, player will be able to input an attack that the previous attack chains into.

    private float             sm_DistanceToTarget;
    private float             sm_LandingTime;
    private GameObject        sm_LockTarget;
    private const float       sm_NormalStance_DownKick_1_WaitingConstant     = 0.4f; //HurtBox related waiting time constants. Used during the restoration of the HurtBoxes to their original sizes.
    private const float       sm_NormalStance_DownKick_2_WaitingConstant = 0.4f; //HurtBox related waiting time constants. Used during the restoration of the HurtBoxes to their original sizes.
    private const float       sm_NormalStance_UpKick_1_WaitingConstant = 0.25f; //HurtBox related waiting time constants. Used during the restoration of the HurtBoxes to their original sizes.
    private bool              sm_DidSlide;
    private float             sm_DistanceToTargetDelta;
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Do these first-----
        sm_AttachedObject = animator.gameObject;
        sm_CombatScript   = sm_AttachedObject.GetComponent<CombatBehaviour>();
        sm_Movementscript = sm_AttachedObject.GetComponent<MovementBehaviour>();
        sm_LockTarget     = sm_Movementscript.m_LockTarget;
        sm_HurtBox        = sm_AttachedObject.GetComponent<BoxCollider>();
        //-------------------

         
        sm_DistanceToTarget                    = sm_CombatScript.m_DistanceToTarget;
        sm_HurtBox.center                      = sm_CombatScript.m_HurtBoxDimensions;
        sm_ComboWindowTimer                    = sm_CombatScript.m_ComboWindowDuration;
        sm_ElapsedTime                         = 0.0f;
        sm_CombatScript.m_StateElapesedTime    = 0.0f;
        sm_CurrentAttack                       = sm_CombatScript.m_CurrentAttack;
        sm_DidSlide                            = false;
        sm_CombatScript.m_IsAttacking          = true;
        sm_CombatScript.m_PreventAttacktInputs = true;  
        sm_Movementscript.m_DisableMovement    = true;
        sm_CombatScript.m_IsIdle               = false;
        sm_CombatScript.m_CanCombo             = false;
        sm_CombatScript.m_IsGuarding           = false;
        sm_CombatScript.m_IsParrying = false;
        sm_CombatScript.m_IsParryingFull = false;
        sm_CombatScript.m_IsGettingHit = false;
        sm_CombatScript.m_IsBlocking = false;
        sm_CombatScript.m_IsStunned = false;
        sm_CharacterSlideSpeed                 = 7.0f;
        sm_AnimationSnapCooldownTimer          = sm_CombatScript.m_SnapStarTimers[sm_CombatScript.m_CurrentAttack];  //Get the current animations SNAP cooldown from the cache.
        sm_AnimationSlideTimer                 = sm_CombatScript.m_SlideTimes[sm_CombatScript.m_CurrentAttack];     //Get the current animations SLIDE duration from the cache.
        sm_LandingTime                         = sm_CombatScript.m_LandingTimes[sm_CurrentAttack];

        sm_CombatScript.m_ComboWindowStart     = sm_CombatScript.m_CancelCooldowns[sm_CurrentAttack];
        var q = Quaternion.LookRotation(sm_Movementscript.m_LockTarget.transform.position - sm_AttachedObject.transform.position);
        //sm_AttachedObject.transform.rotation = Quaternion.RotateTowards(sm_AttachedObject.transform.rotation, q, 500 * Time.deltaTime);
        //sm_AttachedObject.transform.LookAt(new Vector3(sm_LockTarget.transform.position.x, sm_AttachedObject.transform.position.y, sm_LockTarget.transform.position.z)); //Turn the character towards the target during attacks.
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        if (sm_ElapsedTime >= sm_CombatScript.m_LandingTimes[sm_CurrentAttack] - 0.15f)
        {
            sm_CombatScript.m_OffensiveColliders[sm_CombatScript.m_LimbName].enabled = true;  //ENABLE collider.
        }

        //TODO: Make particles glow.
        //sm_CombatScript.m_HitParticles[sm_CombatScript.m_LimbName].Emit(2); //Emit particles during the attack.
        //Debug.Log(sm_ComboWindow);
        sm_CombatScript.m_PreventAttacktInputs = true;
        sm_Movementscript.m_DisableMovement    = true;
        sm_CombatScript.m_IsAttacking          = true;
        sm_CombatScript.m_IsIdle               = false;
        sm_LandingTime                         -= Time.deltaTime;
        sm_AnimationSnapCooldownTimer          -= Time.deltaTime; //Snap cooldown timer for animation.
        sm_ElapsedTime                         += Time.deltaTime; //starts when the attack animation starts. 
        sm_CombatScript.m_StateElapesedTime    = sm_ElapsedTime; //Propogate the elapsed time info to the combat script.

        if (sm_AnimationSnapCooldownTimer <= 0)          //When snap cooldown ends.
        {
            sm_AnimationSlideTimer -= Time.deltaTime;                                                  //Another timer, starts only when the above "sm_AnimationSnapCooldownTimer" ends.
            //Two special attacks for which we have to adjust the HurtBoxes.
            if(sm_CurrentAttack == sm_CombatScript.m_CurrentAttack)
            {
                if (sm_CurrentAttack == "NormalStance_DownKick_1")
                {
                    sm_HurtBox.center = new Vector3(sm_HurtBox.center.x, 0.0f, sm_HurtBox.center.z);       //Decrease the height of the HurtBox during this attack.
                }
                else if (sm_CurrentAttack == "NormalStance_UpKick_1")
                {
                    sm_HurtBox.center = new Vector3(sm_HurtBox.center.x, 1.5f, sm_HurtBox.center.z);       //Increase the height of the HurtBox during this attack sine the character is jumping.
                }
                else if (sm_CurrentAttack == "NormalStance_DownKick_2")
                {
                    sm_HurtBox.center = new Vector3(sm_HurtBox.center.x, 0.0f, sm_HurtBox.center.z);       //Increase the height of the HurtBox during this attack sine the character is jumping.
                }
            }
        }
        else
        {
            if(sm_Movementscript.m_LockTarget)
            {
                var q = Quaternion.LookRotation(sm_Movementscript.m_LockTarget.transform.position - sm_AttachedObject.transform.position);
                sm_AttachedObject.transform.rotation = Quaternion.RotateTowards(sm_AttachedObject.transform.rotation, q, 500 * Time.deltaTime);
            }
        }

        //---------------------------------------------------Sliding Start---------------------------------------------------------
        if (sm_Movementscript.m_LockTarget)                                                                                                           //Where we apply the snapping mechanic of the attacks..
        {
            sm_DistanceToTargetDelta = Vector3.Distance(sm_AttachedObject.transform.position, sm_Movementscript.m_LockTarget.transform.position);
            if ( sm_AnimationSnapCooldownTimer < 0 && sm_AnimationSlideTimer  > 0.0f && sm_DistanceToTargetDelta >= 1.1f) //Checking for the necessary conditions. I think the variable names are self explanatory.
            {
                sm_AttachedObject.transform.position += sm_CharacterSlideSpeed *  /*(sm_DistanceToTarget / sm_DistanceToTargetDelta) **/ Time.deltaTime * new Vector3(sm_AttachedObject.transform.forward.x, 0, sm_AttachedObject.transform.forward.z);  //Moving the character to target at a certain rate.
            }
            //Debug.Log(sm_DistanceToTargetDelta);
        }
        //----------------------------------------------------Sliding End----------------------------------------------------------

        if (sm_LandingTime <= 0.0f)  //Check if the landing time is is smaller than 0.
        {
            sm_CombatScript.m_OffensiveColliders[sm_CombatScript.m_LimbName].enabled = false;                     //Disable the correct collider(s) AS SOON AS THE IMPACT POINT IS REACHED.

            if(sm_Movementscript.m_LockTarget)
            {
                var q = Quaternion.LookRotation(sm_Movementscript.m_LockTarget.transform.position - sm_AttachedObject.transform.position);
                sm_AttachedObject.transform.rotation = Quaternion.RotateTowards(sm_AttachedObject.transform.rotation, q, 100 * Time.deltaTime);
            }
            //TODO: MISSING HIT SOUND LOGIC 

            if (sm_CurrentAttack == "NormalStance_DownKick_1" && sm_AnimationSlideTimer + sm_NormalStance_DownKick_1_WaitingConstant <= 0.0f)          //Wait a little bit before restoring the hurtbox back to its original values. Constant at the end is the tested optimal wait time.
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
        }

        //----------------------------------------------------Animation Cancel / Combo-----------------------------------------------
        if(sm_ElapsedTime >= sm_CombatScript.m_CancelCooldowns[sm_CurrentAttack]) //Check if the elapsed time until the start of the animation has reached the animation cancel window.
        {
            sm_ComboWindowTimer -= Time.deltaTime;                                //Decrease cancel window timer every frame. 
            if (sm_ComboWindowTimer > 0.0f)                                       //This is the duration of the window that where we give the player a chance to input a combo attack.
            {                                                                            
                if(sm_CombatScript.m_CurrentAttack == sm_CurrentAttack)           //If the attack in this state and the one in the actual character script are same, then it means that user has not input an attack yet and thus we let him input during this window. 
                {                                                                 
                    if(!sm_CombatScript.m_LockAttacking)                          //Check if the user is spamming the attack before the combo window timer is reached. If so, just punish him and take his comboin abilty away by locking this state.
                    {
                        sm_CombatScript.m_PreventAttacktInputs = false;
                        sm_CombatScript.m_CanCombo = true;
                    }
                }
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        sm_HurtBox.center  = sm_CombatScript.m_HurtBoxDimensions; //Restore the HurtBox back to its original values. Note : This should already be taken care of by the time the finite state reaches here, but still, I am setting it here again just in case.

        if(sm_CombatScript.m_IsGettingHit)
        {
            sm_CombatScript.m_IsAttacking   = false;
            sm_CombatScript.m_CanCombo      = false;
            sm_CombatScript.m_LockAttacking = false;
            sm_CombatScript.m_CurrentAttack = "None";
            //sm_CombatScript.m_HitParticles[sm_CombatScript.m_LimbName].Stop();
            sm_CombatScript.m_NormalStance.ResetStance();
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
            //sm_CombatScript.m_HitParticles[sm_CombatScript.m_LimbName].Stop();
            sm_CombatScript.m_NormalStance.ResetStance();
        }
        //Disable all the colliders upon exit.
        foreach (var nameColliderPair in sm_CombatScript.m_OffensiveColliders)
        {
            nameColliderPair.Value.enabled = false;
        }
    }
}
