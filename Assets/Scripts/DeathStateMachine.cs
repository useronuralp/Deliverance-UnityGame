using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathStateMachine : StateMachineBehaviour
{
    private float sm_DisappearTimer = 2.0f;
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        sm_DisappearTimer -= Time.deltaTime;
        if(sm_DisappearTimer <= 0.0f)
        {
            if(animator.gameObject.CompareTag("EnemyAI"))
            {
                EventManager.GetInstance().LockTargetDied();
                Destroy(animator.gameObject);   
            }
            else
            {
                EventManager.GetInstance().PlayerDied();
            }
        }
    }
}
