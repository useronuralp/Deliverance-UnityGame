using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generic combat class for both player and enemyAI.
/// </summary>
public abstract class CombatBehaviour : MonoBehaviour
{
    //----------------------------Start Timers-------------------------------
    private const float s_NormalStance_LeftKick_1_SnapStart    = 0.17f;
    private const float s_NormalStance_UpKick_1_SnapStart   = 0.5f;
    private const float s_NormalStance_DownKick_1_SnapStart   = 0.25f;
    private const float s_NormalStance_UpPunch_1_SnapStart   = 0.1f;
    private const float s_NormalStance_UpPunch_2_SnapStart  = 0.2f;
    //----------------------------Slide Times--------------------------------
    private const float s_NormalStance_LeftKick_1_SlideTime         = 0.3f;
    private const float s_NormalStance_UpKick_1_SlideTime        = 0.3f;
    private const float s_NormalStance_DownKick_1_SlideTime        = 0.3f;
    private const float s_NormalStance_UpPunch_1_SlideTime        = 0.2f;
    private const float s_NormalStance_UpPunch_2_SlideTime       = 0.15f;
    //-------------------------Damage Numebers-------------------------------
    private const float s_NormalStance_LeftKick_1_Damage            = 10.0f;
    private const float s_NormalStance_UpKick_1_Damage           = 20.0f;
    private const float s_NormalStance_DownKick_1_Damage           = 5.0f;
    private const float s_NormalStance_UpPunch_1_Damage           = 5.0f;
    private const float s_NormalStance_UpPunch_2_Damage          = 5.0f;
    //-------------------------Stamina Costs---------------------------------
    private const float s_NormalStance_LeftKick_1_Stamina           = 5.0f;
    private const float s_NormalStance_UpKick_1_Stamina          = 5.0f;
    private const float s_NormalStance_DownKick_1_Stamina          = 5.0f;
    private const float s_NormalStance_UpPunch_1_Stamina          = 5.0f;
    private const float s_NormalStance_UpPunch_2_Stamina         = 5.0f;
    //-------------------------Cancel Cooldowns------------------------------
    private const float s_NormalStance_LeftKick_1_Cancel            = 0.6f;
    private const float s_NormalStance_UpKick_1_Cancel           = 1.0f;
    private const float s_NormalStance_DownKick_1_Cancel           = 0.62f;
    private const float s_NormalStance_UpPunch_1_Cancel           = 0.35f;
    private const float s_NormalStance_UpPunch_2_Cancel          = 0.35f;
    //-------------------------Landing Times---------------------------------
    private const float s_NormalStance_LeftKick_1_Landing           = 0.5f;
    private const float s_NormalStance_UpKick_1_Landing          = 0.8f;
    private const float s_NormalStance_DownKick_1_Landing          = 0.60f;
    private const float s_NormalStance_UpPunch_1_Landing          = 0.35f;
    private const float s_NormalStance_UpPunch_2_Landing         = 0.35f;
    //----------------------------------Maps---------------------------------
    public Dictionary<string, Collider>       m_OffensiveColliders;   //This dictionary maps all the collider names with their actual "Collider" objects. (Colliders: LeftHand, RightHand, LeftLeg, RightLeg)
    public Dictionary<string, string>         m_HitLocations;         //This dictionary maps all the attack names with their where-to-GET-HIT locations in mecanim.
    public Dictionary<string, string>         m_BlockLocations;       //This dictionary maps all the attack names with their where-to-BLOCK locations in mecanim.
    public Dictionary<string, float>          m_SnapStarTimers;       //Every attack has a SNAPPING start wait time before they can snap onto enemy targets. This dictionary maps the "<attackName, wait times>" pairs.
    public Dictionary<string, float>          m_SlideTimes;           //Every attack has a SLIDING duration the animations. This dictionary maps the "<attackName, slideDuration>" pairs.
    public Dictionary<string, float>          m_DamageNumbers;        //This dictionary maps all the attack names with their damage numbers.
    public Dictionary<string, float>          m_StaminaCosts;         //This dictionary maps all the attack names with their stamina costs.
    public Dictionary<string, float>          m_LandingTimes;         //This will map the attacks with their landing times (Contact times).
    public Dictionary<string, float>          m_CancelCooldowns;      //This will map the attacks with their cancel cooldowns
    public Dictionary<string, string>         m_Kicks;                //Kicks and their used limbs.
    public Dictionary<string, string>         m_Punches;              //Punches and their used limbs.
    public Dictionary<string, ParticleSystem> m_HitParticles;   
    //----------------------------------Flags--------------------------------
    public bool                               m_PreventAttacktInputs; //This variable prevents user from inputting during certain animations. (Eg: When gettin hit, we don't want player to have the ability to keep attacking.)
    public bool                               m_IsGettingHit;         //This bool tells if the character is currently in a getting-hit animaiton or not.
    public bool                               m_IsStunned;            //This variable should stay true as long as the character is in a getting hit animaiton.
    public bool                               m_IsParrying;           //This variable should stay true as long as the character is in the parry WINDOW, this is not to be confused with the entire parry animaiton.
    public bool                               m_IsParryingFull;       //This variable will stay true during the entire parry animation.
    public bool                               m_IsIdle;               //This varialbe will stay true as long as the character is idle state.
    public bool                               m_IsGuarding;           //This variable should stay true as long as the character is in a guarding.
    public bool                               m_IsAttacking;          //This variable should stay true as long as the character is in an attack animation.
    public bool                               m_CanCombo;             //This variable indicates whether player is in a combow window in one of the attacks.
    public bool                               m_LockAttacking;        //This variable will be set to false whenever player inputs an attack outside of the combo window during an attack animation. It is a flag to punish player for bad timing.
    public bool                               m_IsBlocking;           //This variable will be set to true while the character has blocked a hit while in the guarding stance.
    //----------------------------------Others--------------------------------
    public Animator                           m_Animator;
    public string                             m_CurrentAttack;        //Stores the current attack that the character is performing. If none, then stores "None".
    public string                             m_LimbName;             //Name of the collider of the limb (as string) that we will hit the enemy with.
    public float                              m_StunTimer;            //This variable is the timer that the game start counting back from when the character gets stunned.
    public float                              m_StateElapesedTime;    //The elapsed time while inside one of the state machines in the mecanim. (Eg: Attacking).
    public float                              m_StunDuration;         //The stun duration. Character stays stunned for the duration of this variable.
    public Vector3                            m_HurtBoxDimensions;    //X, Y and Z sizes of the hurtbox collider.
    public float                              m_DistanceToTarget;     //The distance between this character and its target (lock target).
    protected MovementBehaviour               m_MovementScript;       //TODO: This creates coupling. Consider removing this.
    private ActiveStance                      m_ActiveStance;
    protected int                             m_ConsecutiveAttackCount;
    public Stance                             m_NormalStance;
    public string                             m_RecievedAttack;
    public float                              m_ComboWindowDuration;

    //Experimental
    public float                              m_ComboWindowStart;
    public class AttackPair //Two attacks that each direction will have in a stance..
    {
        public string Head; //Pointer to the next attack that the character will throw in this direction.
        public AttackPair(string first, string second)
        {
            this.first = first;
            this.second = second;
            ResetHead();
        }
        public void ResetHead()
        {
            Head = first;
        }
        public void MoveHead()
        {
            if(Head == first)
            {
                if(second.Equals("None"))
                {
                    return;
                }
                Head = second;
            }
            else if (Head == second)
            {
                Head = first;
            }
        }
        private readonly string first; //First attack.
        private readonly string second;//Second attack.
    }
    public class Stance //A stance has four directions and in each direction it has a punch pair and a kick pair.
    {
        //Up
        public AttackPair UpPunch;
        public AttackPair UpKick;
        //Left
        public AttackPair LeftPunch;
        public AttackPair LeftKick;
        //Right
        public AttackPair RightPunch;
        public AttackPair RightKick;
        //Down
        public AttackPair DownPunch;
        public AttackPair DownKick;

        public void ResetStance() //Resets the stance by making the Head of each of the stances point to the first attacks. //TODO: Put this in a loop somehow.
        {
            UpPunch.ResetHead();
            UpKick.ResetHead();
            LeftPunch.ResetHead();
            LeftKick.ResetHead();
            RightPunch.ResetHead();
            RightKick.ResetHead();
            DownPunch.ResetHead();
            DownKick.ResetHead();
        }
    }
    protected enum ActiveStance
    {
        Normal,
        Alternative
    }
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
            { "NormalStance_LeftKick_1", "RightLeg" },
            { "NormalStance_UpKick_1",   "RightLeg"},
            { "NormalStance_DownKick_1", "RightLeg"}
        };
        m_Punches = new Dictionary<string, string>()
        {
            { "NormalStance_UpPunch_1",  "LeftHand" },
            { "NormalStance_UpPunch_2",  "RightHand" }
        };
        m_BlockLocations = new Dictionary<string, string>()
        {
            { "NormalStance_LeftKick_1", "BlockTopLeft" },
            { "NormalStance_UpKick_1",   "BlockTopLeft" },
            { "NormalStance_DownKick_1", "BlockTopLeft" },
            { "NormalStance_UpPunch_1",  "BlockTopLeft" },
            { "NormalStance_UpPunch_2",  "BlockTopLeft" }
        };
        m_HitLocations = new Dictionary<string, string>()
        {
            { "NormalStance_LeftKick_1", "GetHitTopLeft"     },
            { "NormalStance_UpKick_1",   "GetHitTopLeft"     },
            { "NormalStance_DownKick_1", "GetHitMiddleLeft"  },
            { "NormalStance_UpPunch_1",  "GetHitTopStraight" },
            { "NormalStance_UpPunch_2",  "GetHitTopStraight" }
        };
        m_SnapStarTimers = new Dictionary<string, float>()
        {
            { "NormalStance_LeftKick_1", s_NormalStance_LeftKick_1_SnapStart },
            { "NormalStance_UpKick_1",   s_NormalStance_UpKick_1_SnapStart   },
            { "NormalStance_DownKick_1", s_NormalStance_DownKick_1_SnapStart },
            { "NormalStance_UpPunch_1",  s_NormalStance_UpPunch_1_SnapStart  },
            { "NormalStance_UpPunch_2",  s_NormalStance_UpPunch_2_SnapStart  }
        };
        m_SlideTimes = new Dictionary<string, float>()
        {
            { "NormalStance_LeftKick_1", s_NormalStance_LeftKick_1_SlideTime },
            { "NormalStance_UpKick_1",   s_NormalStance_UpKick_1_SlideTime   },
            { "NormalStance_DownKick_1", s_NormalStance_DownKick_1_SlideTime },
            { "NormalStance_UpPunch_1",  s_NormalStance_UpPunch_1_SlideTime  },
            { "NormalStance_UpPunch_2",  s_NormalStance_UpPunch_2_SlideTime  }
        };
        m_DamageNumbers = new Dictionary<string, float>()
        {
            { "NormalStance_LeftKick_1", s_NormalStance_LeftKick_1_Damage    },
            { "NormalStance_UpKick_1",   s_NormalStance_UpKick_1_Damage      },
            { "NormalStance_DownKick_1", s_NormalStance_DownKick_1_Damage    },
            { "NormalStance_UpPunch_1",  s_NormalStance_UpPunch_1_Damage     },
            { "NormalStance_UpPunch_2",  s_NormalStance_UpPunch_2_Damage     }
        };
        m_StaminaCosts = new Dictionary<string, float>()
        {
            { "NormalStance_LeftKick_1", s_NormalStance_LeftKick_1_Stamina   },
            { "NormalStance_UpKick_1",   s_NormalStance_UpKick_1_Stamina     },
            { "NormalStance_DownKick_1", s_NormalStance_DownKick_1_Stamina   },
            { "NormalStance_UpPunch_1",  s_NormalStance_UpPunch_1_Stamina    },
            { "NormalStance_UpPunch_2",  s_NormalStance_UpPunch_2_Stamina    }
        };
        m_CancelCooldowns = new Dictionary<string, float>()
        {
            { "NormalStance_LeftKick_1", s_NormalStance_LeftKick_1_Cancel    },
            { "NormalStance_UpKick_1",   s_NormalStance_UpKick_1_Cancel      },
            { "NormalStance_DownKick_1", s_NormalStance_DownKick_1_Cancel    },
            { "NormalStance_UpPunch_1",  s_NormalStance_UpPunch_1_Cancel     },
            { "NormalStance_UpPunch_2",  s_NormalStance_UpPunch_2_Cancel     }
        };
        m_LandingTimes = new Dictionary<string, float>()
        {
            { "NormalStance_LeftKick_1", s_NormalStance_LeftKick_1_Landing   },
            { "NormalStance_UpKick_1",   s_NormalStance_UpKick_1_Landing     },
            { "NormalStance_DownKick_1", s_NormalStance_DownKick_1_Landing   },
            { "NormalStance_UpPunch_1",  s_NormalStance_UpPunch_1_Landing    },
            { "NormalStance_UpPunch_2",  s_NormalStance_UpPunch_2_Landing    }
        };
        //-----Stance initilizations----------------------------------------
        m_NormalStance = new Stance
        {
            UpPunch    = new AttackPair("NormalStance_UpPunch_1", "NormalStance_UpPunch_2"),
            UpKick     = new AttackPair("NormalStance_UpKick_1", "None"),
            LeftPunch  = new AttackPair("None",         "None"),
            LeftKick   = new AttackPair("NormalStance_LeftKick_1",  "None"),
            RightKick  = new AttackPair("None",         "None"),
            RightPunch = new AttackPair("None",         "None"),
            DownPunch  = new AttackPair("None",         "None"),
            DownKick   = new AttackPair("NormalStance_DownKick_1", "None")
        };
    }
    protected virtual void Awake()
    {
        m_ComboWindowDuration          = 0.35f;
        m_ActiveStance                 = ActiveStance.Normal;
        m_StateElapesedTime            = 0.0f;
        m_StunDuration                 = 2.0f;
        m_IsGettingHit                 = false;
        m_IsParrying                   = false;
        m_IsIdle                       = true;
        m_IsBlocking                   = false;
        m_IsParryingFull               = false;
        m_IsGuarding                   = false;
        m_PreventAttacktInputs         = false;
        m_IsStunned                    = false;
        m_IsAttacking                  = false;
        m_CanCombo                     = false;
        m_LockAttacking                = false;
        m_RecievedAttack               = "None";
        m_CurrentAttack                = "None";
        m_StunTimer                    = m_StunDuration;
        m_Animator                     = GetComponent<Animator>();
        m_HurtBoxDimensions            = GetComponent<BoxCollider>().center;
        m_MovementScript               = GetComponent<MovementBehaviour>();
        m_ComboWindowStart             = 0.0f;
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
        foreach(ParticleSystem sys in gameObject.GetComponentsInChildren<ParticleSystem>()) //Caching the particle systems on the limbs of the character.
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
    protected bool ThrowAttack(AttackPair duo, string colliderName)
    {
        if (m_CanCombo && duo.Head == m_CurrentAttack)                                    //Eliminates the possibility of abusing combo with the same attack over and over again. (BUG FIX / WAS ADDED LATER IN DEVELOPMENT)
        {
            return false;
        }
        if (m_CurrentAttack != "None" && !m_CanCombo && m_StateElapesedTime > 0.2f)              //The case in which player tries to spam attakcs. Big no-no. Punish them by taking away their combo ability until the current attack animation ends. (BUG FIX / ADDED LATER)
        {
            m_LockAttacking = true;
        }
        if (!m_PreventAttacktInputs && m_Animator.GetBool("isLockedOn") && !m_IsGuarding)        //Conditions for a character to be able to throw an attack.
        {
            if(duo.Head == "None")
            {
                return false;
            }
            if(GetComponent<HealthStamina>().m_CurrentStamina > m_StaminaCosts[duo.Head]) //Current stamina amount must be larger than tha cost of the attack.
            {
                m_ConsecutiveAttackCount++;
                m_DistanceToTarget                 = Vector3.Distance(transform.position, m_MovementScript.m_LockTarget.transform.position);
                m_MovementScript.m_DisableMovement = true;
                m_IsAttacking                      = true;
                m_IsIdle                           = false;
                m_PreventAttacktInputs             = true;
                m_CanCombo                         = false;
                m_CurrentAttack                    = duo.Head;
                m_LimbName                         = colliderName;
                GetComponent<HealthStamina>().ReduceStamina(m_StaminaCosts[m_CurrentAttack]);
                m_Animator.SetTrigger("Throw" + duo.Head);
                duo.MoveHead();
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
        m_Animator.SetBool("isGuarding", true);
        m_IsIdle               = false;
        m_IsGuarding           = true;
        m_PreventAttacktInputs = true;
    }
    protected void GuardReleased()
    {
        m_Animator.SetBool("isGuarding", false);
        m_IsIdle               = true;
        m_IsGuarding           = false;
        m_PreventAttacktInputs = false;
    }
}
