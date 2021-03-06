using UnityEngine;

//This script is called when a character gets stunned.
public class GetStunnedStateMachine : StateMachineBehaviour
{
    private GameObject sm_AttachedObject;
    private MovementBehaviour sm_MovementScript;
    private CombatBehaviour sm_CombatScript;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Do these first-----
        sm_AttachedObject = animator.gameObject;
        sm_CombatScript = sm_AttachedObject.GetComponent<CombatBehaviour>();
        sm_MovementScript = sm_AttachedObject.GetComponent<MovementBehaviour>();
        //-------------------

        sm_CombatScript.m_PreventAttacktInputs = true;
        sm_MovementScript.m_DisableMovement    = true;
        sm_CombatScript.m_IsGettingHit         = false;
        sm_CombatScript.m_IsBlocking           = false;
        sm_CombatScript.m_IsParrying           = false;
        sm_CombatScript.m_IsInParryingAnimation       = false;
        sm_CombatScript.m_IsIdle               = true;
        sm_CombatScript.m_IsAttacking          = false;
        sm_CombatScript.m_IsGuarding           = false;
        sm_CombatScript.m_ComboCount           = 0;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        sm_CombatScript.m_IsStunned            = true;
        sm_CombatScript.m_IsGuarding           = false;
        sm_CombatScript.m_IsIdle               = true; //Keep this true. AI gets confused otherwise.
        sm_CombatScript.m_IsBlocking           = false;
        sm_CombatScript.m_PreventAttacktInputs = true;
        sm_MovementScript.m_DisableMovement    = true;
        sm_CombatScript.m_IsGettingHit         = false;
        sm_CombatScript.m_IsAttacking          = false;
        sm_CombatScript.m_IsParrying           = false;
        sm_CombatScript.m_IsInParryingAnimation       = false;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        sm_CombatScript.m_IsStunned            = false;
        sm_CombatScript.m_IsGuarding           = false;
        sm_CombatScript.m_IsIdle               = true;
        sm_CombatScript.m_IsBlocking           = false;
        sm_CombatScript.m_PreventAttacktInputs = false;
        sm_MovementScript.m_DisableMovement    = false;
        sm_CombatScript.m_IsGettingHit         = false;
        sm_CombatScript.m_IsAttacking          = false;
        sm_CombatScript.m_IsParrying           = false;
        sm_CombatScript.m_IsInParryingAnimation       = false;
    }
}
