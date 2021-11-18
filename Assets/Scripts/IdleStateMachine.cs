using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleStateMachine : StateMachineBehaviour
{
    private GameObject sm_AttachedObject;
    private CombatBehaviour sm_CombatScript;
    private MovementBehaviour sm_MovementScript;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        sm_AttachedObject = animator.gameObject;
        sm_MovementScript = sm_AttachedObject.GetComponent<MovementBehaviour>();
        sm_CombatScript = sm_AttachedObject.GetComponent<CombatBehaviour>();

        sm_CombatScript.m_IsIdle = true;
        sm_CombatScript.m_IsStunned = false;
        sm_CombatScript.m_IsGuarding = false;
        sm_CombatScript.m_IsBlocking = false;
        sm_CombatScript.m_PreventAttacktInputs = false;
        sm_MovementScript.m_DisableMovement = false;
        sm_CombatScript.m_IsGettingHit = false;
        sm_CombatScript.m_IsAttacking = false;
        sm_CombatScript.m_IsParrying = false;
        sm_CombatScript.m_IsParryingFull = false;
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        sm_CombatScript.m_IsIdle = true;
        sm_CombatScript.m_IsStunned = false;
        sm_CombatScript.m_IsGuarding = false;
        sm_CombatScript.m_IsBlocking = false;
        sm_CombatScript.m_PreventAttacktInputs = false;
        sm_MovementScript.m_DisableMovement = false;
        sm_CombatScript.m_IsGettingHit = false;
        sm_CombatScript.m_IsAttacking = false;
        sm_CombatScript.m_IsParrying = false;
        sm_CombatScript.m_IsParryingFull = false;
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        sm_CombatScript.m_IsIdle = false;
    }
}
