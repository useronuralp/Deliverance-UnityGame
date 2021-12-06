using UnityEngine;

//Called when a character recieves a hit.
public class RecieveHitStateMachine : StateMachineBehaviour
{
    private GameObject        sm_AttachedObject;
    private MovementBehaviour sm_Movementscript;
    private CombatBehaviour   sm_CombatScript;
    private float             sm_KnockBackTimer;  //The object that gets hit will be staggered backwards FOR the duration of this variable.
    private string            sm_CurrentRecievedAttack;
    private Vector3           sm_KnockbackDirection;
    private Rigidbody         sm_RigidBody;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Do these first-----
        sm_AttachedObject = animator.gameObject;
        sm_CombatScript   = sm_AttachedObject.GetComponent<CombatBehaviour>();
        sm_Movementscript = sm_AttachedObject.GetComponent<MovementBehaviour>();
        sm_RigidBody      = sm_AttachedObject.GetComponent<Rigidbody>();
        //-------------------

        if (sm_Movementscript.m_LockTarget)
        {
            sm_CurrentRecievedAttack = sm_CombatScript.m_RecievedAttack;
            if(sm_CurrentRecievedAttack.Contains("Left"))
            {
                sm_KnockbackDirection = Quaternion.AngleAxis(45, sm_AttachedObject.transform.up) * sm_AttachedObject.transform.forward;
            }
            else if(sm_CurrentRecievedAttack.Contains("Right"))
            {
                sm_KnockbackDirection = Quaternion.AngleAxis(-45, sm_AttachedObject.transform.up) * sm_AttachedObject.transform.forward;
            }
            else
            {
                sm_KnockbackDirection = new Vector3(sm_AttachedObject.transform.forward.x, 0, sm_AttachedObject.transform.forward.z);
            }
        }
        else
        {
            sm_KnockbackDirection = Vector3.zero;
        }
        sm_KnockbackDirection = new Vector3(sm_KnockbackDirection.x, 0, sm_KnockbackDirection.z); //Zero out Y.

        sm_CombatScript.m_Animator.SetBool("isStunned", false); //Break the stun.
        sm_CombatScript.m_StunTimer = sm_CombatScript.m_StunDuration; //Set the stun timer back to its starting value in the combat script of tha attached object.

        sm_KnockBackTimer                      = 0.07f;
        sm_CombatScript.m_IsStunned            = false; //If the character was stunned and is getting hit during that state, break the stun.
        sm_CombatScript.m_PreventAttacktInputs = true;
        sm_Movementscript.m_DisableMovement    = true;
        sm_CombatScript.m_IsGettingHit         = true;
        sm_CombatScript.m_StateElapesedTime    = 0.0f;
        sm_CombatScript.m_IsIdle               = false;
        sm_CombatScript.m_IsAttacking          = false;
        sm_CombatScript.m_ComboCount           = 0;
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        sm_CombatScript.m_IsGettingHit         = true;
        sm_CombatScript.m_PreventAttacktInputs = true;
        sm_Movementscript.m_DisableMovement    = true;
        sm_CombatScript.m_IsIdle               = false;
        sm_CombatScript.m_IsAttacking          = false;

        sm_KnockBackTimer -= Time.deltaTime; //Decrease the timer each second.
        if (sm_KnockBackTimer >= 0.0f)
            sm_RigidBody.MovePosition(sm_AttachedObject.transform.position + 15.0f * Time.deltaTime * -sm_KnockbackDirection.normalized);

        if(sm_Movementscript.m_LockTarget)
        {
            sm_AttachedObject.transform.LookAt(new Vector3(sm_Movementscript.m_LockTarget.transform.position.x, sm_AttachedObject.transform.position.y, sm_Movementscript.m_LockTarget.transform.position.z));
        }
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        sm_CombatScript.m_IsStunned            = false;
        sm_CombatScript.m_PreventAttacktInputs = false;
        sm_Movementscript.m_DisableMovement    = false;
        sm_CombatScript.m_IsGettingHit         = false;
        sm_CombatScript.m_IsIdle               = true;
    }
}
