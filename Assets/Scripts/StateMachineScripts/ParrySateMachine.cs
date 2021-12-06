using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// sm_ prefix stands for State Machine and is used inside state machine behaviours.
/// </summary>
public class ParrySateMachine : StateMachineBehaviour
{
    private GameObject        sm_AttachedObject;
    private MovementBehaviour sm_Movementscript;
    private CombatBehaviour   sm_CombatScript;
    private float             sm_ParryWindowDuration;
    private float             sm_ComboWindowDuration;
    private float             sm_ComboWindowStartTime;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Do these first-----
        sm_AttachedObject      = animator.gameObject;
        sm_CombatScript        = sm_AttachedObject.GetComponent<CombatBehaviour>();
        sm_Movementscript      = sm_AttachedObject.GetComponent<MovementBehaviour>();
        sm_ParryWindowDuration = 0.3f;
        //-------------------

        sm_ComboWindowStartTime                 = 0.6f;
        sm_ComboWindowDuration                  = sm_CombatScript.m_ComboWindowDuration;
        sm_CombatScript.m_StateElapesedTime     = 0.0f;
        sm_CombatScript.m_IsIdle                = false;
        sm_CombatScript.m_PreventAttacktInputs  = true;
        sm_Movementscript.m_DisableMovement     = true;
        sm_CombatScript.m_IsParrying            = true;
        sm_CombatScript.m_IsInParryingAnimation = true;
        sm_CombatScript.m_ComboWindowStart      = sm_ComboWindowStartTime;
        sm_CombatScript.m_CurrentAttack         = "None";

        sm_CombatScript.m_ComboCount++;
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        sm_ComboWindowStartTime -= Time.deltaTime;
        sm_ParryWindowDuration -= Time.deltaTime;

        //This parry window is a sub-window in this parry animation. The first 0.3f seconds of the animation will have invincivilty frames and it will stun the target. This variable tracks that. 
        if(sm_ParryWindowDuration >= 0)
            sm_CombatScript.m_IsParrying = true;
        else
            sm_CombatScript.m_IsParrying = false;

        if(sm_ComboWindowStartTime <= 0.0f && sm_ComboWindowDuration >= 0.0f)
        {
            sm_ComboWindowDuration -= Time.deltaTime;
            if (sm_CombatScript.m_CurrentAttack == "None") //If the attack in this state and the one in the actual character script are same, then it means that user has not input an attack yet and thus we let him input during this window. 
            {
                if (!sm_CombatScript.m_LockAttacking) //Check if the user is spamming the attack before the combo window timer is reached. If so, just punish him and take his comboin abilty away by locking this state.
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
        else
        {
            sm_CombatScript.m_PreventAttacktInputs = true;
        }
        sm_CombatScript.m_StateElapesedTime     += Time.deltaTime;
        sm_CombatScript.m_IsIdle                = false;
        sm_CombatScript.m_IsInParryingAnimation = true;
        sm_Movementscript.m_DisableMovement     = true;
        sm_Movementscript.transform.LookAt(sm_Movementscript.m_LockTarget.transform);
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (sm_CombatScript.m_CurrentAttack == "None")
        {
            sm_CombatScript.m_StateElapesedTime    = 0.0f;
            sm_CombatScript.m_IsIdle               = true;
            sm_CombatScript.m_IsInParryingAnimation= false;
            sm_CombatScript.m_IsParrying           = false;
            sm_CombatScript.m_PreventAttacktInputs = false;
            sm_Movementscript.m_DisableMovement    = false;
            sm_CombatScript.m_CanCombo             = false;
            sm_CombatScript.m_LockAttacking        = false;
            sm_CombatScript.m_ComboCount           = 0;
            sm_CombatScript.m_NormalStance.ResetStance();
        }
        else
        {
            sm_CombatScript.m_IsIdle                = false;
            sm_CombatScript.m_IsAttacking           = true;
            sm_CombatScript.m_PreventAttacktInputs  = true;
            sm_Movementscript.m_DisableMovement     = true;
            sm_CombatScript.m_CanCombo              = false;
            sm_CombatScript.m_LockAttacking         = false;
            sm_CombatScript.m_StateElapesedTime     = 0.0f;
            sm_CombatScript.m_IsInParryingAnimation = false;
            sm_CombatScript.m_IsParrying            = false;
        }
    }
}
