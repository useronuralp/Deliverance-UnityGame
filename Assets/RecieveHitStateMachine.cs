using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecieveHitStateMachine : StateMachineBehaviour
{
    private GameObject        sm_AttachedObject;
    private MovementBehaviour sm_Movementscript;
    private CombatBehaviour   sm_CombatScript;
    private float             sm_KnockBackTimer;  //The object that gets hit will be staggered backwards FOR the duration of this variable.

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Do these first-----
        sm_AttachedObject = animator.gameObject;
        sm_CombatScript   = sm_AttachedObject.GetComponent<CombatBehaviour>();
        sm_Movementscript = sm_AttachedObject.GetComponent<MovementBehaviour>();
        //-------------------

        sm_CombatScript.m_Animator.SetBool("isStunned", false);         //Break the stun.
        sm_CombatScript.m_StunTimer = sm_CombatScript.m_StunDuration;   //Set the stun timer back to its starting value in the combat script of tha attached object.

        sm_KnockBackTimer                      = 0.07f;
        sm_CombatScript.m_IsStunned            = false;                 //If the character was stunned and is getting hit during that state, break the stun.
        sm_CombatScript.m_PreventAttacktInputs = true;
        sm_Movementscript.m_DisableMovement    = true;
        sm_CombatScript.m_IsGettingHit         = true;

    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        sm_CombatScript.m_IsStunned = false;
        sm_CombatScript.m_IsGettingHit = true;
        sm_CombatScript.m_PreventAttacktInputs = true;
        sm_Movementscript.m_DisableMovement = true;

        sm_KnockBackTimer -= Time.deltaTime;                                                                     //Decrease the timer each second.
        if(sm_KnockBackTimer >= 0.0f)                                    
            sm_AttachedObject.transform.position += 15f * Time.deltaTime * -new Vector3(sm_AttachedObject.transform.forward.x , 0, sm_AttachedObject.transform.forward.z); //Slide back the character 
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        sm_CombatScript.m_PreventAttacktInputs = false;
        sm_Movementscript.m_DisableMovement    = false;
        sm_CombatScript.m_IsGettingHit         = false;
    }
}
