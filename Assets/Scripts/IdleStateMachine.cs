using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleStateMachine : StateMachineBehaviour
{
    private GameObject sm_AttachedObject;
    private CombatBehaviour sm_CombatScript;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        sm_AttachedObject = animator.gameObject;
        sm_CombatScript = sm_AttachedObject.GetComponent<CombatBehaviour>();

        sm_CombatScript.m_IsIdle = true;
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        sm_CombatScript.m_IsIdle = true;
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        sm_CombatScript.m_IsIdle = false;
    }
}
