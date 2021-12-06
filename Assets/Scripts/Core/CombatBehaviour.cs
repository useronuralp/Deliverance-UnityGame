using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generic combat class for both player and enemyAI that contains attack specific static variables.
/// </summary>
public abstract class CombatBehaviour : MonoBehaviour
{
    //----------------------------Start Timers-------------------------------
    private const float s_NormalStance_LeftKick_1_SnapStart   = 0.17f;
    private const float s_NormalStance_UpKick_1_SnapStart     = 0.5f;
    private const float s_NormalStance_DownKick_1_SnapStart   = 0.25f;
    private const float s_NormalStance_UpPunch_1_SnapStart    = 0.1f;
    private const float s_NormalStance_UpPunch_2_SnapStart    = 0.2f;
    private const float s_NormalStance_RightKick_1_SnapStart  = 0.2f;
    private const float s_NormalStance_DownKick_2_SnapStart   = 0.2f;
    private const float s_NormalStance_RightPunch_1_SnapStart = 0.0f;
    private const float s_NormalStance_RightKick_2_SnapStart  = 0.15f;
    private const float s_NormalStance_UpKick_2_SnapStart     = 0.4f;
    private const float s_NormalStance_LeftPunch_1_SnapStart  = 0.1f;
    private const float s_NormalStance_LeftPunch_2_SnapStart  = 0.1f;
    private const float s_NormalStance_RightPunch_2_SnapStart = 0.1f;
    private const float s_NormalStance_LeftKick_2_SnapStart   = 0.1f;
    //----------------------------Slide Times--------------------------------
    private const float s_NormalStance_LeftKick_1_SlideTime   = 0.25f;
    private const float s_NormalStance_UpKick_1_SlideTime     = 0.3f;
    private const float s_NormalStance_DownKick_1_SlideTime   = 0.3f;
    private const float s_NormalStance_UpPunch_1_SlideTime    = 0.2f;
    private const float s_NormalStance_UpPunch_2_SlideTime    = 0.15f;
    private const float s_NormalStance_RightKick_1_SlideTime  = 0.3f;
    private const float s_NormalStance_DownKick_2_SlideTime   = 0.3f;
    private const float s_NormalStance_RightPunch_1_SlideTime = 0.2f;
    private const float s_NormalStance_RightKick_2_SlideTime  = 0.2f;
    private const float s_NormalStance_UpKick_2_SlideTime     = 0.3f;
    private const float s_NormalStance_LeftPunch_1_SlideTime  = 0.2f;
    private const float s_NormalStance_LeftPunch_2_SlideTime  = 0.2f;
    private const float s_NormalStance_RightPunch_2_SlideTime = 0.2f;
    private const float s_NormalStance_LeftKick_2_SlideTime   = 0.25f;
    //-------------------------Damage Numebers-------------------------------
    private const float s_NormalStance_LeftKick_1_Damage      = 10.0f;
    private const float s_NormalStance_UpKick_1_Damage        = 20.0f;
    private const float s_NormalStance_DownKick_1_Damage      = 10.0f;
    private const float s_NormalStance_UpPunch_1_Damage       = 5.0f;
    private const float s_NormalStance_UpPunch_2_Damage       = 10.0f;
    private const float s_NormalStance_RightKick_1_Damage     = 10.0f;
    private const float s_NormalStance_DownKick_2_Damage      = 5.0f;
    private const float s_NormalStance_RightPunch_1_Damage    = 5.0f;
    private const float s_NormalStance_RightKick_2_Damage     = 15.0f;
    private const float s_NormalStance_UpKick_2_Damage        = 30.0f;
    private const float s_NormalStance_LeftPunch_1_Damage     = 5.0f;
    private const float s_NormalStance_LeftPunch_2_Damage     = 10.0f;
    private const float s_NormalStance_RightPunch_2_Damage    = 10.0f;
    private const float s_NormalStance_LeftKick_2_Damage      = 15.0f;
    //-------------------------Stamina Costs---------------------------------
    private const float s_NormalStance_LeftKick_1_Stamina     = 10.0f;
    private const float s_NormalStance_UpKick_1_Stamina       = 10.0f;
    private const float s_NormalStance_DownKick_1_Stamina     = 10.0f;
    private const float s_NormalStance_UpPunch_1_Stamina      = 5.0f;
    private const float s_NormalStance_UpPunch_2_Stamina      = 5.0f;
    private const float s_NormalStance_RightKick_1_Stamina    = 10.0f;
    private const float s_NormalStance_DownKick_2_Stamina     = 10.0f;
    private const float s_NormalStance_RightPunch_1_Stamina   = 5.0f;
    private const float s_NormalStance_RightKick_2_Stamina    = 10.0f;
    private const float s_NormalStance_UpKick_2_Stamina       = 10.0f;
    private const float s_NormalStance_LeftPunch_1_Stamina    = 5.0f;
    private const float s_NormalStance_LeftPunch_2_Stamina    = 5.0f;
    private const float s_NormalStance_RightPunch_2_Stamina   = 5.0f;
    private const float s_NormalStance_LeftKick_2_Stamina     = 10.0f;
    //-------------------------Cancel Cooldowns------------------------------
    private const float s_NormalStance_LeftKick_1_Cancel      = 0.6f;
    private const float s_NormalStance_UpKick_1_Cancel        = 1.0f;
    private const float s_NormalStance_DownKick_1_Cancel      = 0.62f;
    private const float s_NormalStance_UpPunch_1_Cancel       = 0.35f;
    private const float s_NormalStance_UpPunch_2_Cancel       = 0.35f;
    private const float s_NormalStance_RightKick_1_Cancel     = 0.6f;
    private const float s_NormalStance_DownKick_2_Cancel      = 0.8f;
    private const float s_NormalStance_RightPunch_1_Cancel    = 0.45f;
    private const float s_NormalStance_RightKick_2_Cancel     = 0.6f;
    private const float s_NormalStance_UpKick_2_Cancel        = 0.95f;
    private const float s_NormalStance_LeftPunch_1_Cancel     = 0.55f;
    private const float s_NormalStance_LeftPunch_2_Cancel     = 0.5f;
    private const float s_NormalStance_RightPunch_2_Cancel    = 0.65f;
    private const float s_NormalStance_LeftKick_2_Cancel      = 0.75f;
    //-------------------------Landing Times---------------------------------
    private const float s_NormalStance_LeftKick_1_Landing     = 0.5f;
    private const float s_NormalStance_UpKick_1_Landing       = 0.8f;
    private const float s_NormalStance_DownKick_1_Landing     = 0.60f;
    private const float s_NormalStance_UpPunch_1_Landing      = 0.35f;
    private const float s_NormalStance_UpPunch_2_Landing      = 0.35f;
    private const float s_NormalStance_RightKick_1_Landing    = 0.55f;
    private const float s_NormalStance_DownKick_2_Landing     = 0.70f;
    private const float s_NormalStance_RightPunch_1_Landing   = 0.35f;
    private const float s_NormalStance_RightKick_2_Landing    = 0.45f;
    private const float s_NormalStance_UpKick_2_Landing       = 0.8f;
    private const float s_NormalStance_LeftPunch_1_Landing    = 0.45f;
    private const float s_NormalStance_LeftPunch_2_Landing    = 0.4f;
    private const float s_NormalStance_RightPunch_2_Landing   = 0.55f;
    private const float s_NormalStance_LeftKick_2_Landing     = 0.60f;
    //----------------------------------Maps---------------------------------
    public Dictionary<string, Collider>       m_OffensiveColliders;   //This dictionary maps all the collider names with their actual "Collider" objects. (Colliders: LeftHand, RightHand, LeftLeg, RightLeg)
    public Dictionary<string, string>         m_HitLocations;         //This dictionary maps all the attack names to their where-to-GET-HIT locations in mecanim.
    public Dictionary<string, string>         m_BlockLocations;       //This dictionary maps all the attack names to their where-to-BLOCK locations in mecanim.
    public Dictionary<string, float>          m_SnapStartTimers;      //Every attack has a SNAPPING start wait time before they can snap onto enemy targets. This dictionary maps the "<attackName, wait times>" pairs.
    public Dictionary<string, float>          m_SlideTimes;           //Every attack has a SLIDING duration. This dictionary maps the "<attackName, slideDuration>" pairs.
    public Dictionary<string, float>          m_DamageNumbers;        //This dictionary maps all the attack names to their damage numbers.
    public Dictionary<string, float>          m_StaminaCosts;         //This dictionary maps all the attack names to their stamina costs.
    public Dictionary<string, float>          m_LandingTimes;         //This will map the attacks to their landing times (Contact points / times).
    public Dictionary<string, float>          m_CancelCooldowns;      //This will map the attacks to their cancel cooldowns
    public Dictionary<string, string>         m_Kicks;                //Kicks are mapped to their used limbs. (Eg. Kick A is thrown is 'LeftLeg' while Kick B is thrown with 'RightLeg')
    public Dictionary<string, string>         m_Punches;              //Punches are mapped to their used limbs.
    public Dictionary<string, ParticleSystem> m_HitParticles;         //Dictionary that maps which limbs of the character will play which particle system during attacks. (Eg: A kick that uses LeftHand as a limb to attack will trigger
                                                                      // 'ParticleSystemLeftHand' to emit particles. Every limb has a particle system attached in this game to produce visual effects.)
    //----------------------------------Flags--------------------------------
    [HideInInspector]
    public bool                               m_PreventAttacktInputs; //This variable prevents user from inputting attacks during certain animations. (Eg: When getting hit, we don't want player to have the ability to keep attacking.)
    [HideInInspector]
    public bool                               m_IsGettingHit;         //This bool tells if the character is currently in a getting-hit animaiton or not.
    [HideInInspector]
    public bool                               m_IsStunned;            //This variable should stay true as long as the character is in a stun animation.
    [HideInInspector]
    public bool                               m_IsParrying;           //This variable should stay true as long as the character is in the parry WINDOW, this is not to be confused with the entire parry animation.
    [HideInInspector]
    public bool                               m_IsInParryingAnimation;//Contrary to the above variable, this variable will stay true during the entire parry animaiton.
    [HideInInspector]
    public bool                               m_IsIdle;               //This varialbe will stay true as long as the character is in idle state.
    [HideInInspector]
    public bool                               m_IsGuarding;           //This variable should stay true as long as the character is guarding.
    [HideInInspector]
    public bool                               m_IsAttacking;          //This variable should stay true as long as the character is in an attack animation.
    [HideInInspector]
    public bool                               m_CanCombo;             //This variable indicates whether player is in a combow window in one of the attacks.
    [HideInInspector]
    public bool                               m_LockAttacking;        //This variable will be set to false whenever player inputs an attack outside of the combo window during an attack animation. It is a flag to punish player for bad timing.
    [HideInInspector]
    public bool                               m_IsBlocking;           //This variable will be set to true when a character blocks a hit while in the guarding stance and a blocking animation plays..
    //----------------------------------Others--------------------------------
    [HideInInspector]
    public Animator                           m_Animator;
    [HideInInspector]
    public string                             m_CurrentAttack;          //Stores the name of the current attack that the character is performing as a string. If none, then stores "None".
    [HideInInspector]                                                   
    public string                             m_LimbName;               //Name of the collider of the limb (as string) that the current attack will use.
    [HideInInspector]                                                   
    public float                              m_StunTimer;              //This variable is the timer that the game starts counting back from when the character gets stunned.
    [HideInInspector]                                                   
    public float                              m_StateElapesedTime;      //The elapsed time while inside one of the state machines in the mecanim. (Eg: Attacking).
    [HideInInspector]                                                   
    public float                              m_StunDuration;           //The stun duration. Character stays stunned for the duration of this variable.
    [HideInInspector]                                                   
    public Vector3                            m_HurtBoxDimensions;      //X, Y and Z sizes of the hurtbox collider.
    [HideInInspector]                                                   
    public float                              m_DistanceToTarget;       //The distance between this character and its target (lock target).
    [HideInInspector]
    public int                                m_ConsecutiveAttackCount; //The number of the attacks the AI has thrown without wandering. (This is an AI specific varaible.)
    [HideInInspector]
    public Stance                             m_NormalStance;           //The game was originally designed to have lot more complex mechanics. This is currently the only stance in the game, I planned on having multiple stances, but I did not have enough time.
    [HideInInspector]
    public float                              m_ComboWindowDuration;    //The duration player is allowed to input a combo input during an attack. 
    [HideInInspector]
    public bool                               m_DidHitLand;             //This flag indicates whether the impact point of the attack was reached and the limb is currently being pulled back or not. Used to calculate precise collider enabling / disabling.
    [HideInInspector]
    public int                                m_ComboCount;             //Number of consecutive combo count without breaking it. Only used for the player character at the moment.
    [HideInInspector]
    public float                              m_ComboWindowStart;       //A float indicating when the the attack should be able to chain into the next one. This variable will have different values for each attack in the game set by the AttackStateMachine script.
    [HideInInspector]
    public string                             m_RecievedAttack = "None";//Name of the attack that the character is recieving if at all. 

    public class AttackPair // In the game, each direction has two different attacks that the player can throw. This class represenst a single "ATTACK PAIR" that each direction will have for both PUNCHES and KICKS. 
    {                       //This means that there are 8 Attack pairs in the game as a total. I currently don't have animations for the down direction for punches.

        public string Head; //Pointer to attack that the player will throw in this attack pair when he uses this direction.
        public AttackPair(string first, string second)
        {
            this.first = first;
            this.second = second;
            ResetHead();
        }
        public void ResetHead() //Resets the Head to the first attack.
        {
            Head = first;
        }
        public void MoveHead() //Moves the between the first and the second attack whenever an attack is thrown.
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
        public AttackPair DownPunch; //Down punches are not currently present in the game because I could not find appropriate animations for them.
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
    protected enum State //The state which the character is in.
    {
        NONE = -1,
        IDLE,
        KICK,
        PUNCH,
        PARRY,
        GUARD
    }
    protected CombatBehaviour() //This class is the meat of the game.
    {
        m_HitParticles       = new Dictionary<string, ParticleSystem>();
        m_OffensiveColliders = new Dictionary<string, Collider>();

        //Remark: Explanations of each of these dictionaries were written at the beginning of this source file.
        //Each of these dictonaries contain the names of the attacks in the game with the naming convention -> [Stance Name]_[Attack Name]_[Number of the attack(first or second)]
        m_Kicks = new Dictionary<string, string>()
        {
            { "NormalStance_LeftKick_1", "RightLeg" },
            { "NormalStance_UpKick_1",   "RightLeg" },
            { "NormalStance_DownKick_1", "RightLeg" },
            { "NormalStance_RightKick_1","RightLeg" } ,
            { "NormalStance_DownKick_2", "LeftLeg"  },
            { "NormalStance_RightKick_2","RightLeg" },
            { "NormalStance_UpKick_2",   "LeftLeg"  },
            { "NormalStance_LeftKick_2", "LeftLeg"  },
        };
        m_Punches = new Dictionary<string, string>()
        {
            { "NormalStance_UpPunch_1",   "LeftHand" },
            { "NormalStance_UpPunch_2",   "RightHand"},
            { "NormalStance_RightPunch_1","RightHand"},
            { "NormalStance_LeftPunch_1", "RightHand"},
            { "NormalStance_LeftPunch_2", "RightHand"},
            { "NormalStance_RightPunch_2","RightHand"},
        };
        m_BlockLocations = new Dictionary<string, string>()
        {
            { "NormalStance_LeftKick_1",  "BlockTopRight" },
            { "NormalStance_UpKick_1",    "BlockTopLeft"  },
            { "NormalStance_RightKick_1", "BlockTopLeft"  },
            { "NormalStance_DownKick_1",  "BlockTopLeft"  },
            { "NormalStance_UpPunch_1",   "BlockStraight" },
            { "NormalStance_UpPunch_2",   "BlockStraight" },
            { "NormalStance_DownKick_2",  "BlockTopLeft"  },
            { "NormalStance_RightPunch_1","BlockTopLeft"  },
            { "NormalStance_RightKick_2", "BlockTopLeft"  },
            { "NormalStance_UpKick_2",    "BlockTopLeft"  },
            { "NormalStance_LeftPunch_1", "BlockTopRight" },
            { "NormalStance_LeftPunch_2", "BlockTopRight" },
            { "NormalStance_RightPunch_2","BlockTopLeft"  },
            { "NormalStance_LeftKick_2",  "BlockTopRight" },
        };
        m_HitLocations = new Dictionary<string, string>()
        {
            { "NormalStance_LeftKick_1",   "GetHitRight"      },
            { "NormalStance_UpKick_1",     "GetHitTopLeft"    },
            { "NormalStance_RightKick_1",  "GetHitTopLeft"    },
            { "NormalStance_DownKick_1",   "GetHitMiddleLeft" },
            { "NormalStance_UpPunch_1",    "GetHitTopStraight"},
            { "NormalStance_UpPunch_2",    "GetHitTopStraight"},
            { "NormalStance_DownKick_2",   "GetHitMiddleLeft" },
            { "NormalStance_RightPunch_1", "GetHitTopLeft"    },
            { "NormalStance_RightKick_2",  "GetHitTopLeft"    },
            { "NormalStance_UpKick_2",     "GetHitTopLeft"    },
            { "NormalStance_LeftPunch_1",  "GetHitRight"      },
            { "NormalStance_LeftPunch_2",  "GetHitRight"      },
            { "NormalStance_RightPunch_2", "GetHitTopLeft"    },
            { "NormalStance_LeftKick_2",   "GetHitRight"      },
        };
        m_SnapStartTimers = new Dictionary<string, float>()
        {
            { "NormalStance_LeftKick_1",   s_NormalStance_LeftKick_1_SnapStart   },
            { "NormalStance_UpKick_1",     s_NormalStance_UpKick_1_SnapStart     },
            { "NormalStance_RightKick_1",  s_NormalStance_RightKick_1_SnapStart  },
            { "NormalStance_DownKick_1",   s_NormalStance_DownKick_1_SnapStart   },
            { "NormalStance_UpPunch_2",    s_NormalStance_UpPunch_2_SnapStart    },
            { "NormalStance_UpPunch_1",    s_NormalStance_UpPunch_1_SnapStart    },
            { "NormalStance_DownKick_2",   s_NormalStance_DownKick_2_SnapStart   },
            { "NormalStance_RightPunch_1", s_NormalStance_RightPunch_1_SnapStart },
            { "NormalStance_RightKick_2",  s_NormalStance_RightKick_2_SnapStart  },
            { "NormalStance_UpKick_2",     s_NormalStance_UpKick_2_SnapStart     },
            { "NormalStance_LeftPunch_1",  s_NormalStance_LeftPunch_1_SnapStart  },
            { "NormalStance_LeftPunch_2",  s_NormalStance_LeftPunch_2_SnapStart  },
            { "NormalStance_RightPunch_2", s_NormalStance_RightPunch_2_SnapStart },
            { "NormalStance_LeftKick_2",   s_NormalStance_LeftKick_2_SnapStart   },
        };
        m_SlideTimes = new Dictionary<string, float>()
        {
            { "NormalStance_LeftKick_1",   s_NormalStance_LeftKick_1_SlideTime   },
            { "NormalStance_UpKick_1",     s_NormalStance_UpKick_1_SlideTime     },
            { "NormalStance_RightKick_1",  s_NormalStance_RightKick_1_SlideTime  },
            { "NormalStance_DownKick_1",   s_NormalStance_DownKick_1_SlideTime   },
            { "NormalStance_UpPunch_1",    s_NormalStance_UpPunch_1_SlideTime    },
            { "NormalStance_UpPunch_2",    s_NormalStance_UpPunch_2_SlideTime    },
            { "NormalStance_DownKick_2",   s_NormalStance_DownKick_2_SlideTime   },
            { "NormalStance_RightPunch_1", s_NormalStance_RightPunch_1_SlideTime },
            { "NormalStance_RightKick_2",  s_NormalStance_RightKick_2_SlideTime  },
            { "NormalStance_UpKick_2",     s_NormalStance_UpKick_2_SlideTime     },
            { "NormalStance_LeftPunch_1",  s_NormalStance_LeftPunch_1_SlideTime  },
            { "NormalStance_LeftPunch_2",  s_NormalStance_LeftPunch_2_SlideTime  },
            { "NormalStance_RightPunch_2", s_NormalStance_RightPunch_2_SlideTime },
            { "NormalStance_LeftKick_2",   s_NormalStance_LeftKick_2_SlideTime   },
        };
        m_DamageNumbers = new Dictionary<string, float>()
        {
            { "NormalStance_LeftKick_1",   s_NormalStance_LeftKick_1_Damage    },
            { "NormalStance_UpKick_1",     s_NormalStance_UpKick_1_Damage      },
            { "NormalStance_RightKick_1",  s_NormalStance_RightKick_1_Damage   },
            { "NormalStance_DownKick_1",   s_NormalStance_DownKick_1_Damage    },
            { "NormalStance_UpPunch_1",    s_NormalStance_UpPunch_1_Damage     },
            { "NormalStance_UpPunch_2",    s_NormalStance_UpPunch_2_Damage     },
            { "NormalStance_DownKick_2",   s_NormalStance_DownKick_2_Damage    },
            { "NormalStance_RightPunch_1", s_NormalStance_RightPunch_1_Damage  },
            { "NormalStance_RightKick_2",  s_NormalStance_RightKick_2_Damage   },
            { "NormalStance_UpKick_2",     s_NormalStance_UpKick_2_Damage      },
            { "NormalStance_LeftPunch_1",  s_NormalStance_LeftPunch_1_Damage   },
            { "NormalStance_LeftPunch_2",  s_NormalStance_LeftPunch_2_Damage   },
            { "NormalStance_RightPunch_2", s_NormalStance_RightPunch_2_Damage  },
            { "NormalStance_LeftKick_2",   s_NormalStance_LeftKick_2_Damage    },
        };
        m_StaminaCosts = new Dictionary<string, float>()
        {
            { "NormalStance_LeftKick_1",   s_NormalStance_LeftKick_1_Stamina   },
            { "NormalStance_UpKick_1",     s_NormalStance_UpKick_1_Stamina     },
            { "NormalStance_RightKick_1",  s_NormalStance_RightKick_1_Stamina  },
            { "NormalStance_DownKick_1",   s_NormalStance_DownKick_1_Stamina   },
            { "NormalStance_UpPunch_1",    s_NormalStance_UpPunch_1_Stamina    },
            { "NormalStance_UpPunch_2",    s_NormalStance_UpPunch_2_Stamina    },
            { "NormalStance_DownKick_2",   s_NormalStance_DownKick_2_Stamina   },
            { "NormalStance_RightPunch_1", s_NormalStance_RightPunch_1_Stamina },
            { "NormalStance_RightKick_2",  s_NormalStance_RightKick_2_Stamina  },
            { "NormalStance_UpKick_2",     s_NormalStance_UpKick_2_Stamina     },
            { "NormalStance_LeftPunch_1",  s_NormalStance_LeftPunch_1_Stamina  },
            { "NormalStance_LeftPunch_2",  s_NormalStance_LeftPunch_2_Stamina  },
            { "NormalStance_RightPunch_2", s_NormalStance_RightPunch_2_Stamina },
            { "NormalStance_LeftKick_2",   s_NormalStance_LeftKick_2_Stamina   },
        };
        m_CancelCooldowns = new Dictionary<string, float>()
        {
            { "NormalStance_LeftKick_1",   s_NormalStance_LeftKick_1_Cancel    },
            { "NormalStance_UpKick_1",     s_NormalStance_UpKick_1_Cancel      },
            { "NormalStance_RightKick_1",  s_NormalStance_RightKick_1_Cancel   },
            { "NormalStance_DownKick_1",   s_NormalStance_DownKick_1_Cancel    },
            { "NormalStance_UpPunch_1",    s_NormalStance_UpPunch_1_Cancel     },
            { "NormalStance_UpPunch_2",    s_NormalStance_UpPunch_2_Cancel     },
            { "NormalStance_DownKick_2",   s_NormalStance_DownKick_2_Cancel    },
            { "NormalStance_RightPunch_1", s_NormalStance_RightPunch_1_Cancel  },
            { "NormalStance_RightKick_2",  s_NormalStance_RightKick_2_Cancel   },
            { "NormalStance_UpKick_2",     s_NormalStance_UpKick_2_Cancel      },
            { "NormalStance_LeftPunch_1",  s_NormalStance_LeftPunch_1_Cancel   },
            { "NormalStance_LeftPunch_2",  s_NormalStance_LeftPunch_2_Cancel   },
            { "NormalStance_RightPunch_2", s_NormalStance_RightPunch_2_Cancel  },
            { "NormalStance_LeftKick_2",   s_NormalStance_LeftKick_2_Cancel    },
        };
        m_LandingTimes = new Dictionary<string, float>()
        {
            { "NormalStance_LeftKick_1",   s_NormalStance_LeftKick_1_Landing   },
            { "NormalStance_UpKick_1",     s_NormalStance_UpKick_1_Landing     },
            { "NormalStance_RightKick_1",  s_NormalStance_RightKick_1_Landing  },
            { "NormalStance_DownKick_1",   s_NormalStance_DownKick_1_Landing   },
            { "NormalStance_UpPunch_1",    s_NormalStance_UpPunch_1_Landing    },
            { "NormalStance_UpPunch_2",    s_NormalStance_UpPunch_2_Landing    },
            { "NormalStance_DownKick_2",   s_NormalStance_DownKick_2_Landing   },
            { "NormalStance_RightPunch_1", s_NormalStance_RightPunch_1_Landing },
            { "NormalStance_RightKick_2",  s_NormalStance_RightKick_2_Landing  },
            { "NormalStance_UpKick_2",     s_NormalStance_UpKick_2_Landing     },
            { "NormalStance_LeftPunch_1",  s_NormalStance_LeftPunch_1_Landing  },
            { "NormalStance_LeftPunch_2",  s_NormalStance_LeftPunch_2_Landing  },
            { "NormalStance_RightPunch_2", s_NormalStance_RightPunch_2_Landing },
            { "NormalStance_LeftKick_2",   s_NormalStance_LeftKick_2_Landing   },
        };
        //-----Stance initilizations----------------------------------------
        m_NormalStance = new Stance
        {
            UpPunch    = new AttackPair("NormalStance_UpPunch_1", "NormalStance_UpPunch_2"),
            UpKick     = new AttackPair("NormalStance_UpKick_1", "NormalStance_UpKick_2"),
            LeftPunch  = new AttackPair("NormalStance_LeftPunch_1", "NormalStance_LeftPunch_2"),
            LeftKick   = new AttackPair("NormalStance_LeftKick_1", "NormalStance_LeftKick_2"),
            RightKick  = new AttackPair("NormalStance_RightKick_1", "NormalStance_RightKick_2"),
            RightPunch = new AttackPair("NormalStance_RightPunch_1", "NormalStance_RightPunch_2"),
            DownPunch  = new AttackPair("None",         "None"),  //I currently don't have attacks for this direcion. Could not find appropriate animations :((
            DownKick   = new AttackPair("NormalStance_DownKick_1", "NormalStance_DownKick_2")
        };
    }
    protected virtual void Awake()
    {
        //Defaulting the varaibles on Awake.
        m_ComboWindowDuration          = 0.2f;
        m_StateElapesedTime            = 0.0f;
        m_StunDuration                 = 2.0f;
        m_IsGettingHit                 = false;
        m_IsParrying                   = false;
        m_IsIdle                       = true;
        m_IsBlocking                   = false;
        m_IsInParryingAnimation        = false;
        m_IsGuarding                   = false;
        m_PreventAttacktInputs         = false;
        m_IsStunned                    = false;
        m_IsAttacking                  = false;
        m_DidHitLand                   = true;
        m_CanCombo                     = false;
        m_LockAttacking                = false;
        m_CurrentAttack                = "None";
        m_StunTimer                    = m_StunDuration;
        m_Animator                     = GetComponent<Animator>();
        m_HurtBoxDimensions            = GetComponent<BoxCollider>().center;
        m_ComboWindowStart             = 0.0f;
        
        //Get the limb colliders.
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
        //Caching the particle systems on the limbs of the character.
        foreach (ParticleSystem particleSys in gameObject.GetComponentsInChildren<ParticleSystem>()) 
        {
            if(particleSys.name.Equals("RightFootParticles"))
            {
                particleSys.Stop();
                m_HitParticles["RightLeg"] = particleSys;
            }
            else if (particleSys.name.Equals("LeftFootParticles"))
            {
                particleSys.Stop();
                m_HitParticles["LeftLeg"] = particleSys;
            }
            else if (particleSys.name.Equals("RightHandParticles"))
            {
                particleSys.Stop();
                m_HitParticles["RightHand"] = particleSys;
            }
            else if (particleSys.name.Equals("LeftHandParticles"))
            {
                particleSys.Stop();
                m_HitParticles["LeftHand"] = particleSys;
            }
        }
    }
    protected bool ThrowAttack(AttackPair attackPair, string colliderName, GameObject target) //This function is used when attacking.
    {
        if(m_IsInParryingAnimation) 
        {
            m_LockAttacking = true;
        }
        if (m_CanCombo && attackPair.Head == m_CurrentAttack) //Eliminates the possibility of abusing combo with the same attack over and over again.
        {
            return false;
        }
        if (m_CurrentAttack != "None" && !m_CanCombo && m_StateElapesedTime > 0.2f) //The case in which player tries to spam attacks. Big no-no. Punish them by taking away their combo ability until the current attack animation ends. (BUG FIX / ADDED LATER)
        {
            m_LockAttacking = true;
        }
        if (!m_PreventAttacktInputs && m_Animator.GetBool("isLockedOn") && !m_IsGuarding) //Conditions for a character to be able to throw an attack.
        {
            if (m_CanCombo && gameObject.CompareTag("Player"))
                SoundManager.PlaySound("ComboSuccess");
            if(attackPair.Head == "None")
            {
                return false;
            }
            if(GetComponent<HealthStamina>().m_CurrentStamina > m_StaminaCosts[attackPair.Head]) //Current stamina amount must be larger than tha cost of the attack.
            {
                m_DistanceToTarget                 = Vector3.Distance(transform.position, target.transform.position);
                m_IsAttacking                      = true;
                m_IsIdle                           = false;
                m_DidHitLand                       = false;
                m_CanCombo                         = false;
                m_PreventAttacktInputs             = true;
                m_CurrentAttack                    = attackPair.Head;
                m_LimbName                         = colliderName;
                GetComponent<HealthStamina>().ReduceStamina(m_StaminaCosts[m_CurrentAttack]);
                m_Animator.SetTrigger("Throw" + attackPair.Head);
                attackPair.MoveHead();
                return true;
            }
        }
        return false;
    }
    protected void Parry()
    {
        if (m_CurrentAttack != "None" && !m_CanCombo && m_StateElapesedTime > 0.2f) 
        {
            m_LockAttacking = true;
            return;
        }
        if (m_IsInParryingAnimation) 
        {
            return;
        }
        if (!m_PreventAttacktInputs)
        {
            m_IsIdle                           = false;
            m_PreventAttacktInputs             = true;
            m_IsParrying                       = true;
            m_IsInParryingAnimation            = true;
            m_Animator.SetTrigger("Parry");
        }
    }
    protected void GuardUp() //Called when the character wants to guard.
    {
        m_Animator.SetBool("isGuarding", true);
        m_IsIdle               = false;
        m_IsGuarding           = true;
        m_PreventAttacktInputs = true;
    }
    protected void GuardReleased() //Called when the button is released.
    {
        m_Animator.SetBool("isGuarding", false);
        m_IsIdle               = true;
        m_IsGuarding           = false;
        m_PreventAttacktInputs = false;
    }
}
