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
    private float             sm_ElapsedTime;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Do these first-----
        sm_AttachedObject      = animator.gameObject;
        sm_CombatScript        = sm_AttachedObject.GetComponent<CombatBehaviour>();
        sm_Movementscript      = sm_AttachedObject.GetComponent<MovementBehaviour>();
        sm_ParryWindowDuration = 0.3f;
        //-------------------

        sm_CombatScript.m_StateElapesedTime = 0.0f;
        sm_CombatScript.m_IsIdle               = false;
        sm_CombatScript.m_PreventAttacktInputs = true;
        sm_Movementscript.m_DisableMovement    = true;
        sm_CombatScript.m_IsParrying           = true;
        sm_CombatScript.m_IsParryingFull       = true;
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        sm_ParryWindowDuration -= Time.deltaTime;
        if(sm_ParryWindowDuration >= 0)
            sm_CombatScript.m_IsParrying = true;
        else
            sm_CombatScript.m_IsParrying = false;

        sm_CombatScript.m_StateElapesedTime += Time.deltaTime;
        sm_CombatScript.m_IsIdle               = false;
        sm_CombatScript.m_IsParryingFull       = true;
        sm_CombatScript.m_PreventAttacktInputs = true;
        sm_Movementscript.m_DisableMovement    = true;
        sm_Movementscript.transform.LookAt(sm_Movementscript.m_LockTarget.transform);
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        sm_CombatScript.m_StateElapesedTime = 0.0f;
        sm_CombatScript.m_IsIdle               = true;
        sm_CombatScript.m_IsParryingFull       = false;
        sm_CombatScript.m_IsParrying           = false;
        sm_CombatScript.m_PreventAttacktInputs = false;
        sm_Movementscript.m_DisableMovement    = false;
    }
}
