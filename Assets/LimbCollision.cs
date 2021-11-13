using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is attached to limb colliders (hitboxes) and handles collision.
/// </summary>
public class LimbCollision : MonoBehaviour
{
    private bool m_IsColliding = false;
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
        if (collidedObject.transform.root.CompareTag(target))   //Check if the collided object is actually who we want to deal damage to. (If the character is EnemyAI, deal damage to "Player", if the character is Player, deal damage to "EnemeyAI".)
        {
            if (collidedObject.GetType() == typeof(BoxCollider)) //Apply hit, only if you collide with the Hurt Box, not with hit boxes or move boxes (Capsule Collider). //TODO : Do not collide with limb colliders.
            {
                //Debug.Log(collidedObject.tag);   
                //Combat scripts
                CombatBehaviour selfCombatScript = transform.root.GetComponent<CombatBehaviour>();
                CombatBehaviour targetCombatScript = GameObject.FindWithTag(target).GetComponent<CombatBehaviour>();

                //Debug.Log(targetCombatScript.m_IsGettingHit);

                string curAttack = selfCombatScript.m_CurrentAttack; //Name of the attack that the character is currently throwing.

                if (!targetCombatScript.m_IsGettingHit)  //
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
    private void Update()
    {
        m_IsColliding = false;
    }
    void PlayGettingHitSound(string attack)
    {
        switch (attack) //Play the hit sound here.
        {
            case "TopKickLeft": SoundManager.PlaySound("GetHitBody"); break;
            case "TopPunchLeft": SoundManager.PlaySound("GetHitPunch"); break;
            case "TopPunchRight": SoundManager.PlaySound("GetHitPunch"); break;
            case "TopKickRight": SoundManager.PlaySound("GetHit"); break;
            case "LegSweepKick": SoundManager.PlaySound("GetHitBody"); break;
        }
    }
    void DealDamage(CombatBehaviour targetCombatScript, string attackToDealDamageWith, Collider other)
    {
        targetCombatScript.m_IsGettingHit = true;
        PlayGettingHitSound(attackToDealDamageWith);
        other.transform.root.GetComponent<HealthStamina>().GetHit(targetCombatScript.m_DamageNumbers[attackToDealDamageWith]);
        targetCombatScript.m_Animator.SetTrigger(targetCombatScript.m_HitLocations[attackToDealDamageWith]);
    }
    void ReduceStamina(CombatBehaviour targetCombatScript, string attackName, Collider other)
    {
        targetCombatScript.m_IsGettingHit = true;
        SoundManager.PlaySound("Block1"); //Play block hit sound.
        if(attackName.Contains("Kick") || attackName.Contains("kick"))
        {
            other.transform.root.GetComponent<HealthStamina>().ReduceStamina(20.0f);
        }
        else
        {
            other.transform.root.GetComponent<HealthStamina>().ReduceStamina(10.0f);
        }
        targetCombatScript.m_Animator.SetBool("isGuarding", true); // Added to fix a bug.
        targetCombatScript.m_Animator.SetTrigger(targetCombatScript.m_BlockLocations[attackName]); //Set block animation trigger.
    }
}
