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
        sm_CombatScript.m_IsAttacking = false;
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        sm_CombatScript.m_IsIdle = true;
        sm_CombatScript.m_IsAttacking = false;
        //sm_CombatScript.m_PreventAttacktInputs = false;
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        sm_CombatScript.m_IsIdle = false;
    }
}
