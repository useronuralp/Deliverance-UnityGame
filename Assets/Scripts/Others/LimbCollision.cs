using UnityEngine;

/// <summary>
/// This script is attached to limb colliders (hitboxes) and handles collision
/// Every character in the game has four hitbox colliders on their four limbs. LeftFoot, LeftHand, RightFoot, RightHand.
/// </summary>
//Takes care of all the collisions in the game.
public class LimbCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetType() == typeof(BoxCollider))
        {
            if (transform.root.CompareTag("EnemyAI"))
            {
                ApplyCollisionTo("Player", other);
            }
            else if(transform.root.CompareTag("Player"))
            {
                ApplyCollisionTo("EnemyAI", other);
            }
        }
    }
    void ApplyCollisionTo(string target, Collider collidedObject)
    {
        if (collidedObject.transform.CompareTag(target))   //Check if the collided object is actually who we want to deal damage to. (If the character is EnemyAI, deal damage to "Player", if the character is Player, deal damage to "EnemeyAI".)
        {
            if (collidedObject.GetType() == typeof(BoxCollider)) //Apply hit, only if you collide with the Hurt Box, not with hit boxes or move boxes (Capsule Collider). //TODO : Do not collide with limb colliders.
            {
                //Combat scripts
                CombatBehaviour selfCombatScript = transform.root.GetComponent<CombatBehaviour>();
                CombatBehaviour targetCombatScript;
                if (target == "Player")
                {
                    targetCombatScript = GameObject.FindWithTag(target).GetComponent<CombatBehaviour>();
                }
                else
                {
                    targetCombatScript = GameObject.Find(collidedObject.transform.root.name).GetComponent<CombatBehaviour>();
                }

                string curAttack = selfCombatScript.m_CurrentAttack; //Name of the attack that the character is currently throwing.
                {
                    if (targetCombatScript.m_IsParrying) //Check if the opponent is parrying.
                    {
                        if(selfCombatScript.m_CurrentAttack.Contains("Punch") || selfCombatScript.m_CurrentAttack.Contains("punch")) //Check if you're punching the target. If yes, the collider parries itself.
                        {
                            //TODO: Add getting parried sound.
                            selfCombatScript.m_Animator.SetBool("isStunned", true);
                            selfCombatScript.m_Animator.SetTrigger("GetStunned");
                            selfCombatScript.m_IsStunned = true;
                        }
                        else //If the character is not punching, then it means it is kickin, and all the kicks will go through parries.
                        {
                            DealDamage(targetCombatScript, curAttack, collidedObject);
                        }
                    }
                    else if (targetCombatScript.m_IsGuarding) //Check for guarding.
                    {
                        ReduceStamina(targetCombatScript, curAttack, collidedObject);
                    }
                    else 
                    {
                        DealDamage(targetCombatScript, curAttack, collidedObject);
                    }
                }
            }
        }
    }
    void PlayGettingHitSound()
    {
        switch (Random.Range(0,4)) //Play the hit sound here.
        {
            case 0: SoundManager.PlaySound("GetHit1");break;
            case 1: SoundManager.PlaySound("GetHit2");break;
            case 2: SoundManager.PlaySound("GetHit3"); break;
            case 3: SoundManager.PlaySound("GetHit4"); break;
        }
    }
    void DealDamage(CombatBehaviour targetCombatScript, string attackToDealDamageWith, Collider other)
    {
        PlayGettingHitSound();
        if(attackToDealDamageWith != "None")
        {
            targetCombatScript.m_RecievedAttack = attackToDealDamageWith;
            targetCombatScript.m_IsGettingHit = true;
            other.transform.root.GetComponent<HealthStamina>().GetHit(targetCombatScript.m_DamageNumbers[attackToDealDamageWith]);
            targetCombatScript.m_Animator.SetTrigger(targetCombatScript.m_HitLocations[attackToDealDamageWith]);
        }
    }
    void ReduceStamina(CombatBehaviour targetCombatScript, string attackName, Collider other)
    {
        switch(Random.Range(0, 8))
        {
            case 0: SoundManager.PlaySound("Block1"); break;
            case 1: SoundManager.PlaySound("Block2"); break;
            case 2: SoundManager.PlaySound("Block3"); break;
            case 3: SoundManager.PlaySound("Block4"); break;
            case 4: SoundManager.PlaySound("Block5"); break;
            case 5: SoundManager.PlaySound("Block6"); break;
            case 6: SoundManager.PlaySound("Block7"); break;
            case 7: SoundManager.PlaySound("Block8"); break;
        }
        if(attackName != "None")
        {
            if(attackName.Contains("Kick") || attackName.Contains("kick"))
            {
                targetCombatScript.m_RecievedAttack = attackName;
                targetCombatScript.m_Animator.SetBool("isGuarding", true); // Added to fix a bug.
                targetCombatScript.m_IsGettingHit = true;
                other.transform.root.GetComponent<HealthStamina>().ReduceStamina(20.0f);
            }
            else
            {
                targetCombatScript.m_RecievedAttack = attackName;
                targetCombatScript.m_Animator.SetBool("isGuarding", true); // Added to fix a bug.
                targetCombatScript.m_IsGettingHit = true;
                other.transform.root.GetComponent<HealthStamina>().ReduceStamina(10.0f);
            }
        }
        targetCombatScript.m_Animator.SetBool("isGuarding", true); // Added to fix a bug.
        targetCombatScript.m_Animator.SetTrigger(targetCombatScript.m_BlockLocations[attackName]); //Set block animation trigger.
    }
}
