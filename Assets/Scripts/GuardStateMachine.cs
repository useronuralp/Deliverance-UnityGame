using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// sm_ prefix stands for State Machine and is used inside state machine behaviours.
/// </summary>
public class GuardStateMachine : StateMachineBehaviour
{
    private GameObject        sm_AttachedObject;
    private MovementBehaviour sm_MovementScript;
    private CombatBehaviour   sm_CombatScript;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Do these first-----
        sm_AttachedObject = animator.gameObject;
        sm_MovementScript = sm_AttachedObject.GetComponent<MovementBehaviour>();
        sm_CombatScript   = sm_AttachedObject.GetComponent<CombatBehaviour>();
        //-------------------

        sm_CombatScript.m_StateElapesedTime    = 0.0f;
        sm_CombatScript.m_IsIdle               = false;
        sm_CombatScript.m_IsGuarding           = true;
        sm_CombatScript.m_PreventAttacktInputs = true;

        //Experimental
        sm_CombatScript.m_CurrentAttack        = "None";
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        sm_CombatScript.m_StateElapesedTime   += Time.deltaTime;
        sm_CombatScript.m_IsIdle               = false;
        sm_CombatScript.m_IsGuarding           = true;
        sm_MovementScript.m_DisableMovement    = true;
        sm_CombatScript.m_PreventAttacktInputs = true;

        sm_AttachedObject.transform.LookAt(new Vector3(sm_MovementScript.m_LockTarget.transform.position.x, sm_AttachedObject.transform.position.y, sm_MovementScript.m_LockTarget.transform.position.z));
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        sm_CombatScript.m_StateElapesedTime    = 0.0f;
        sm_CombatScript.m_IsIdle               = true;
        sm_CombatScript.m_IsGuarding           = false;
        sm_CombatScript.m_PreventAttacktInputs = false;
        sm_MovementScript.m_DisableMovement    = false;
        sm_CombatScript.m_IsGettingHit         = false;
    }
}
