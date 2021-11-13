using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generic combat class for both player and enemyAI.
/// </summary>
public abstract class CombatBehaviour : MonoBehaviour
{
    //----------------------------Cooldowns----------------------------------
    //Kicks-----
    private const float  s_TopKickLeftSnapCooldown   = 0.17f;
    private const float  s_TopKickRightSnapCooldown  = 0.5f;
    private const float  s_LegSweepKickSnapCooldown  = 0.20f;
    //Punches---
    private const float  s_TopPunchLeftSnapCooldown  = 0.1f;
    private const float  s_TopPunchRightSnapCooldown = 0.2f;
    //----------------------------Slide Times--------------------------------
    //Kicks-----
    private const float  s_TopKickLeftSlideTime      = 0.2f;
    private const float  s_TopKickRightSlideTime     = 0.3f;
    private const float  s_LegSweepKickSlideTime     = 0.2f;
    //Punches---
    private const float  s_TopPunchLeftSlideTime     = 0.15f;
    private const float  s_TopPunchRightSlideTime    = 0.15f;

    //----------------------------------Maps----------------------------------
    public Dictionary<string, Collider>  m_OffensiveColliders;   //This dictionary maps all the collider names with their actual "Collider" objects. (Colliders: LeftHand, RightHand, LeftLeg, RightLeg)
    public Dictionary<string, string>    m_HitLocations;         //This dictionary maps all the attack names with their where-to-GET-HIT locations in mecanim.
    public Dictionary<string, string>    m_BlockLocations;       //This dictionary maps all the attack names with their where-to-BLOCK locations in mecanim.
    public Dictionary<string, float>     m_SnapCooldowns;        //Every attack has a SNAPPING cooldown before they can snap onto enemy targets. This dictionary maps the "<attackName, snapCooldown>" pairs.
    public Dictionary<string, float>     m_SlideTimes;           //Every attack has a SLIDING duration the animations. This dictionary maps the "<attackName, slideDuration>" pairs.
    public Dictionary<string, float>     m_DamageNumbers;        //This dictionary maps all the attack names with their damage numbers.
    public Dictionary<string, float>     m_StaminaCosts;         //This dictionary maps all the attack names with their stamina costs.
    public Dictionary<string, float>     m_LandingTimes;         //The amount of time that needs to pass after the snapping onto targes to land.
    public Dictionary<string, float>     m_CancelCooldowns;
    //----------------------------------Flags---------------------------------
    public bool                          m_PreventAttacktInputs; //This variable prevents user from inputting during certain animations. (Eg: When gettin hit, we don't want player to have the ability to keep attacking.)
    public bool                          m_IsGettingHit;         //This bool tells if the character is currently in a getting-hit animaiton or not.
    public bool                          m_IsStunned;            //This variable should stay true as long as the character is in a getting hit animaiton.
    public bool                          m_IsParrying;           //This variable should stay true as long as the character is in the parry WINDOW, this is not to be confused with the entire parry animaiton.
    public bool                          m_IsParryingFull;       //This variable will stay true during the entire parry animation.
    public bool                          m_IsIdle;               //This varialbe will stay true as long as the character is idle state.
    public bool                          m_IsGuarding;           //This variable should stay true as long as the character is in a guarding.
    public bool                          m_IsAttacking;          //This variable should stay true as long as the character is in an attack animation.
    public bool                          m_CanCombo;             //This variable indicates whether player is in a combow window in one of the attacks.
    public bool                          m_LockAttacking;        //This variable will be set to false whenever player inputs an attack outside of the combo window during an attack animation. It is a flag to punish player for bad timing.
    public bool                          m_IsBlocking;           //This variable will be set to true while the character has blocked a hit while in the guarding stance.
    //----------------------------------Others--------------------------------
    public Animator                      m_Animator;
    public string                        m_CurrentAttack;        //Stores the current attack that the character is performing. If none, then stores "None".
    public string                        m_LimbName;             //Name of the collider of the limb (as string) that we will hit the enemy with.
    public float                         m_StunTimer;            //This variable is the timer that the game start counting back from when the character gets stunned.
    public float                         m_StateElapesedTime;    //The elapsed time while inside one of the state machines in the mecanim. (Eg: Attacking).
    public float                         m_StunDuration;         //The stun duration. Character stays stunned for the duration of this variable.
    public Vector3                       m_HurtBoxDimensions;    //X, Y and Z sizes of the hurtbox collider.
    public float                         m_DistanceToTarget;     //The distance between this character and its target (lock target).
    protected MovementBehaviour          m_MovementScript;       //TODO: This creates coupling. Consider removing this.
    protected Dictionary<string, string> m_Kicks;                //Kicks and their used limbs.
    protected Dictionary<string, string> m_Punches;              //Punches and their used limbs.
    public Dictionary<string, ParticleSystem> m_HitParticles;

    protected int m_ConsecutiveAttackCount;
    protected enum State
    {
        NONE = -1,
        IDLE,
        KICK,
        PUNCH,
        PARRY,
        GUARD
    }
    protected CombatBehaviour()
    {
        m_HitParticles       = new Dictionary<string, ParticleSystem>();
        m_OffensiveColliders = new Dictionary<string, Collider>();
        m_Kicks = new Dictionary<string, string>()
        {
            { "TopKickLeft",  "RightLeg" },
            { "TopKickRight", "RightLeg"},
            { "LegSweepKick", "RightLeg"}
        };
        m_Punches = new Dictionary<string, string>()
        {
            { "TopPunchLeft",  "LeftHand" },
            { "TopPunchRight", "RightHand" }
        };
        m_SnapCooldowns = new Dictionary<string, float>()
        {
            { "TopKickLeft",   s_TopKickLeftSnapCooldown   },
            { "TopPunchLeft",  s_TopPunchLeftSnapCooldown  },
            { "TopPunchRight", s_TopPunchRightSnapCooldown },
            { "TopKickRight",  s_TopKickRightSnapCooldown  },
            { "LegSweepKick",  s_LegSweepKickSnapCooldown  }
        };
        m_SlideTimes = new Dictionary<string, float>()
        {
            { "TopKickLeft",  s_TopKickLeftSlideTime   },
            { "TopPunchLeft", s_TopPunchLeftSlideTime  },
            { "TopPunchRight",s_TopPunchRightSlideTime },
            { "TopKickRight", s_TopKickRightSlideTime  },
            { "LegSweepKick", s_LegSweepKickSlideTime  }
        };
        m_BlockLocations = new Dictionary<string, string>()
        {
            { "TopKickRight",  "BlockTopLeft" },
            { "TopPunchLeft",  "BlockTopLeft" },
            { "TopPunchRight", "BlockTopLeft" },
            { "TopKickLeft",   "BlockTopLeft" },
            { "LegSweepKick",  "BlockTopLeft" }
        };
        m_HitLocations = new Dictionary<string, string>()
        {
            { "TopKickLeft",   "GetHitTopLeft"     },
            { "TopPunchLeft",  "GetHitTopStraight" },
            { "TopPunchRight", "GetHitTopStraight" },
            { "TopKickRight",  "GetHitTopLeft"     },
            { "LegSweepKick",  "GetHitMiddleLeft"  }
        };
        m_DamageNumbers = new Dictionary<string, float>()
        {
            { "TopKickLeft",   10.0f },
            { "TopPunchLeft",  5.0f  },
            { "TopPunchRight", 5.0f  },
            { "TopKickRight",  20.0f },
            { "LegSweepKick",  5.0f  }
        };
        m_StaminaCosts = new Dictionary<string, float>()
        {
            { "TopKickLeft",   5.0f },
            { "TopPunchLeft",  5.0f },
            { "TopPunchRight", 5.0f },
            { "TopKickRight",  5.0f },
            { "LegSweepKick",  5.0f }
        };
        m_CancelCooldowns = new Dictionary<string, float>()
        {
            { "TopKickLeft",   0.6f  },
            { "TopPunchLeft",  0.35f },
            { "TopPunchRight", 0.4f  },
            { "TopKickRight",  1.0f  },
            { "LegSweepKick",  0.62f }
        };
        m_LandingTimes = new Dictionary<string, float>()
        {
            { "TopKickLeft",   0.1f },
            { "TopPunchLeft",  0.1f },
            { "TopPunchRight", 0.1f },
            { "TopKickRight",  0.0f },
            { "LegSweepKick",  0.13f }
        };
   
    }
    protected virtual void Awake()
    {
        m_StateElapesedTime    = 0.0f;
        m_StunDuration         = 2.0f;
        m_IsGettingHit         = false;
        m_IsParrying           = false;
        m_IsIdle               = true;
        m_IsBlocking           = false;
        m_IsParryingFull       = false;
        m_IsGuarding           = false;
        m_PreventAttacktInputs = false;
        m_IsStunned            = false;
        m_IsAttacking          = false;
        m_CanCombo             = false;
        m_LockAttacking        = false;
        m_CurrentAttack        = "None";
        m_StunTimer            = m_StunDuration;
        m_Animator             = GetComponent<Animator>();
        m_HurtBoxDimensions    = GetComponent<BoxCollider>().center;
        m_MovementScript       = GetComponent<MovementBehaviour>();
        foreach (Collider collider in gameObject.GetComponentsInChildren<Collider>())
        {
            //Note: Currently depending on colliders to contain a key substring in them to find them. TODO : This part could use a more sophisticated search algorithm.
            if (collider.name.Contains("lowerarm_l"))
            {
                collider.enabled = false;
                m_OffensiveColliders["LeftHand"] = collider;
            }
            else if (collider.name.Contains("lowerarm_r"))
            {
                collider.enabled = false;
                m_OffensiveColliders["RightHand"] = collider;
            }
            else if (collider.name.Contains("calf_l"))
            {
                collider.enabled = false;
                m_OffensiveColliders["LeftLeg"] = collider;
            }
            else if (collider.name.Contains("calf_r"))
            {
                collider.enabled = false;
                m_OffensiveColliders["RightLeg"] = collider;
            }
        }
        foreach(ParticleSystem sys in gameObject.GetComponentsInChildren<ParticleSystem>())
        {
            if(sys.name.Equals("RightFootParticles"))
            {
                sys.Stop();
                m_HitParticles["RightLeg"] = sys;
            }
            else if (sys.name.Equals("LeftFootParticles"))
            {
                sys.Stop();
                m_HitParticles["LeftLeg"] = sys;
            }
            else if (sys.name.Equals("RightHandParticles"))
            {
                sys.Stop();
                m_HitParticles["RightHand"] = sys;
            }
            else if (sys.name.Equals("LeftHandParticles"))
            {
                sys.Stop();
                m_HitParticles["LeftHand"] = sys;
            }
        }
        //Debug.Log(m_HitParticles["RightLeg"]);
    }
    protected bool ThrowAttack(string nameOfTheAttack, string colliderName)
    {
        if (m_CanCombo && nameOfTheAttack == m_CurrentAttack)                                    //Eliminates the possibility of abusing combo with the same attack over and over again. (BUG FIX / WAS ADDED LATER IN DEVELOPMENT)
        {
            return false;
        }
        if (m_CurrentAttack != "None" && !m_CanCombo && m_StateElapesedTime > 0.2f)              //The case in which player tries to spam attakcs. Big no-no. Punish them by taking away their combo ability until the current attack animation ends. (BUG FIX / ADDED LATER)
        {
            m_LockAttacking = true;
        }
        if (!m_PreventAttacktInputs && m_Animator.GetBool("isLockedOn") && !m_IsGuarding)        //Conditions for a character to be able to throw an attack.
        {
            if(GetComponent<HealthStamina>().m_CurrentStamina > m_StaminaCosts[nameOfTheAttack]) //Current stamina amount must be larger than tha cost of the attack.
            {
                m_ConsecutiveAttackCount++;
                m_DistanceToTarget                 = Vector3.Distance(transform.position, m_MovementScript.m_LockTarget.transform.position);
                m_MovementScript.m_DisableMovement = true;
                m_IsAttacking                      = true;
                m_IsIdle                           = false;
                m_PreventAttacktInputs             = true;
                m_CanCombo                         = false;
                m_CurrentAttack                    = nameOfTheAttack;
                m_LimbName                         = colliderName;
                GetComponent<HealthStamina>().ReduceStamina(m_StaminaCosts[m_CurrentAttack]);
                m_Animator.SetTrigger("Throw" + nameOfTheAttack);
                return true;
            }
        }
        return false;
    }
    protected void Parry()
    {
        if(!m_PreventAttacktInputs && !m_CanCombo)
        {
            m_MovementScript.m_DisableMovement = true;
            m_IsIdle                           = false;
            m_PreventAttacktInputs             = true;
            m_IsParrying                       = true;
            m_IsParryingFull                   = true;
            m_Animator.SetTrigger("Parry");
        }
    }
    protected void GuardUp()
    {
        m_IsIdle = false;
        m_IsGuarding = true;
        m_PreventAttacktInputs = true;
        m_Animator.SetBool("isGuarding", true);
    }
    protected void GuardReleased()
    {
        m_Animator.SetBool("isGuarding", false);
        m_IsGuarding = false;
        m_PreventAttacktInputs = false;
        m_IsIdle = true;
    }
    public virtual void ResetKicks()
    {
        //Implemented in children...
    }
    public virtual void ResetPunches()
    {
        //Implemented in children...
    }
}
