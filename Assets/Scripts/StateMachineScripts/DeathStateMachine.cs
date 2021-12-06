using UnityEngine;

public class DeathStateMachine : StateMachineBehaviour
{
    private float sm_DisappearTimer = 2.0f;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("GetHitRight");
        animator.ResetTrigger("GetHitTopLeft");
        animator.ResetTrigger("GetHitMiddleLeft");
        animator.ResetTrigger("GetHitTopStraight");
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //After some time fires the corresponding events depending on the dying gameObject tag.
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
