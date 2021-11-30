using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// sm_ prefix stands for State Machine and is used inside state machine behaviours.
/// </summary>
public class BlockHitStateMachine : StateMachineBehaviour
{
    private GameObject        sm_AttachedObject;
    private MovementBehaviour sm_Movementscript;
    private CombatBehaviour   sm_CombatScript;
    private HealthStamina     sm_HealthStaminaScript;
    private float             sm_KnockBackTimer;  //The object that gets hit will be staggered backwards FOR the duration of this variable.
    private string            sm_CurrentRecievedAttack;
    private Vector3           sm_KnockbackDirection;
    private Rigidbody         sm_RigidBody;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Do these first-----
        sm_AttachedObject        = animator.gameObject;
        sm_CombatScript          = sm_AttachedObject.GetComponent<CombatBehaviour>();
        sm_Movementscript        = sm_AttachedObject.GetComponent<MovementBehaviour>();
        sm_HealthStaminaScript   = sm_AttachedObject.GetComponent<HealthStamina>();
        sm_RigidBody             = sm_AttachedObject.GetComponent<Rigidbody>();
        //-------------------

        sm_CurrentRecievedAttack = sm_Movementscript.m_LockTarget.GetComponent<CombatBehaviour>().m_CurrentAttack;

        if (sm_CurrentRecievedAttack.Contains("Left"))
        {
            sm_KnockbackDirection = Quaternion.AngleAxis(45, sm_AttachedObject.transform.up) * sm_AttachedObject.transform.forward;
        }
        else if (sm_CurrentRecievedAttack.Contains("Right"))
        {
            sm_KnockbackDirection = Quaternion.AngleAxis(-45, sm_AttachedObject.transform.up) * sm_AttachedObject.transform.forward;
        }
        else
        {
            sm_KnockbackDirection = new Vector3(sm_AttachedObject.transform.forward.x, 0, sm_AttachedObject.transform.forward.z);
        }
        sm_KnockbackDirection = new Vector3(sm_KnockbackDirection.x, 0, sm_KnockbackDirection.z); //Zero out Y.

        sm_KnockBackTimer                      = 0.04f;
        sm_CombatScript.m_PreventAttacktInputs = true;
        sm_Movementscript.m_DisableMovement    = true;
        sm_CombatScript.m_IsGettingHit         = true;
        sm_CombatScript.m_IsBlocking           = true;

        //Exp
        sm_CombatScript.m_IsIdle = false;
        sm_CombatScript.m_IsAttacking = false;
        sm_CombatScript.m_ComboCount = 0;

    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        sm_KnockBackTimer -= Time.deltaTime;                                                                     
        if (sm_KnockBackTimer >= 0.0f)
            sm_RigidBody.MovePosition(sm_AttachedObject.transform.position + 15.0f * Time.deltaTime * -sm_KnockbackDirection.normalized);

        if (sm_HealthStaminaScript.m_CurrentStamina <= 0) //During guarding, if the your stamina runs out, the character will get stunned. 
        {
            sm_CombatScript.m_Animator.SetBool("isGuarding", false);
            sm_HealthStaminaScript.m_StaminaRechargeTimer = 0.0f; //Immedietaly start recharging the stamina.
            sm_CombatScript.m_Animator.SetBool("isStunned", true);
            sm_CombatScript.m_Animator.SetTrigger("GetStunned");
            sm_CombatScript.m_IsStunned = true;
            sm_CombatScript.m_IsGuarding = false;

            sm_CombatScript.m_IsBlocking           = false;
            sm_CombatScript.m_PreventAttacktInputs = true;
            sm_Movementscript.m_DisableMovement    = true;
            sm_CombatScript.m_IsGettingHit         = false;
        }
        else
        {
            sm_CombatScript.m_IsBlocking           = true;
            sm_CombatScript.m_PreventAttacktInputs = true;
            sm_Movementscript.m_DisableMovement    = true;
            sm_CombatScript.m_IsGettingHit         = true;
            //Exp
            sm_CombatScript.m_IsIdle = false;
            sm_CombatScript.m_IsAttacking = false;
        }


        sm_AttachedObject.transform.LookAt(new Vector3(sm_Movementscript.m_LockTarget.transform.position.x, sm_AttachedObject.transform.position.y, sm_Movementscript.m_LockTarget.transform.position.z));
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        sm_CombatScript.m_IsBlocking           = false;
        sm_CombatScript.m_PreventAttacktInputs = false;
        sm_Movementscript.m_DisableMovement    = false;
        sm_CombatScript.m_IsGettingHit         = false;

        //Exp
        sm_CombatScript.m_IsIdle = true;
        sm_CombatScript.m_IsAttacking = false;
    }
}
