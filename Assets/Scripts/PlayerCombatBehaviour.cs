using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatBehaviour : CombatBehaviour
{
    private State m_AIState = State.NONE;
    private FileIO m_Recorder;
    private bool m_DisableAllInput;
    private GameObject m_LockTarget;
    protected override void Awake()
    {
        base.Awake();
        //Child initializations go here...
    }
    private void Start()
    {
        m_LockTarget = null;
        m_DisableAllInput = false;
        EventManager.GetInstance().OnPlayerLockedOnToTarget += OnLockOnTarget; //Subscribe to event.
        EventManager.GetInstance().OnPlayerReleasedLockOnTarget += OnReleaseLockOnTarget; //Subscribe to event.
        //m_Recorder = new FileIO("TrainingData.txt");
    }
    void Update()
    {
        //Debug.Log(m_PreventAttacktInputs);
        if(Input.GetKey(KeyCode.T))
        {
            MenuManager.RestartLevel(0);
        }
        //Debug.Log(m_CurrentAttack);
        ObserveAI();
        //Debug.Log(m_AIState);
        //if(!m_Animator.GetBool("isAttacking"))
        //{
        //    m_CurrentAttack = "None";
        //}
        if (Input.GetKey(KeyCode.T))
        {
            MenuManager.RestartLevel(0);
        }
        if (m_Animator.GetBool("isDead"))
        {
            enabled = false;
        }
        if (m_IsStunned)
        {
            m_StunTimer -= Time.deltaTime;
            if(m_StunTimer <= 0)
            {
                m_Animator.SetBool("isStunned", false);
                m_StunTimer = m_StunDuration;
                m_IsStunned = false;
            }
        }
        else if(!m_IsStunned)
        {
            if(!m_DisableAllInput)
            {
                ProcessAttackInput();

                if (!m_IsAttacking && !m_IsGettingHit && !m_IsParryingFull && !m_IsStunned)
                {
                    if (Input.GetKey(KeyCode.Space))
                    {
                        GuardUp();
                    }
                }
            }
        }
        if(Input.GetKeyUp(KeyCode.Space))
        {
            GuardReleased();
        }
    }
    void RecordState(State move)
    {
        ObserveAI();
        m_Recorder.WriteToFile((int)m_AIState, GetComponent<HealthStamina>().m_CurrentStamina, m_LockTarget.GetComponent<CombatBehaviour>().m_StateElapesedTime, (int)move);
    }
    void ObserveAI()
    {
        if(GetComponent<MovementBehaviour>().m_LockTarget)
        {
            if (m_LockTarget.GetComponent<CombatBehaviour>().m_IsParryingFull)
            {
                m_AIState = State.PARRY;
            }
            else if (m_LockTarget.GetComponent<CombatBehaviour>().m_IsGuarding || m_LockTarget.GetComponent<CombatBehaviour>().m_IsBlocking)
            {
                m_AIState = State.GUARD;
            }
            else if (m_LockTarget.GetComponent<CombatBehaviour>().m_IsAttacking)
            {
                if (m_LockTarget.GetComponent<CombatBehaviour>().m_CurrentAttack.Contains("Kick"))
                {
                    m_AIState = State.KICK;
                }
                else if (m_LockTarget.GetComponent<CombatBehaviour>().m_CurrentAttack.Contains("Punch"))
                {
                    m_AIState = State.PUNCH;
                }
            }
            else if (m_LockTarget.GetComponent<CombatBehaviour>().m_IsIdle)
            {
                m_AIState = State.IDLE;
            }
        }
    }
    private void ProcessAttackInput()
    {
        if (Input.GetKey(KeyCode.X))
        {
            Parry();
            ObserveAI();
            //m_Recorder.WriteToFile((int)m_AIState, GetComponent<HealthStamina>().m_CurrentStamina, m_LockTarget.GetComponent<CombatBehaviour>().m_StateElapesedTime, (int)Moves.PARRY);

        }
        if ((Input.GetKey(KeyCode.W) || Input.GetKeyDown(KeyCode.W)) && Input.GetKeyDown(KeyCode.Mouse1))
        {
            ThrowAttack(m_NormalStance.UpKick, m_Kicks[m_NormalStance.UpKick.Head], m_LockTarget);
            ObserveAI();
            //m_Recorder.WriteToFile((int)m_AIState, GetComponent<HealthStamina>().m_CurrentStamina, m_LockTarget.GetComponent<CombatBehaviour>().m_StateElapesedTime, (int)Moves.KICK);
        }
        else if ((Input.GetKey(KeyCode.A) || Input.GetKeyDown(KeyCode.A)) && Input.GetKeyDown(KeyCode.Mouse1))
        {
            ThrowAttack(m_NormalStance.LeftKick, m_Kicks[m_NormalStance.LeftKick.Head], m_LockTarget);
            ObserveAI();
            //m_Recorder.WriteToFile((int)m_AIState, GetComponent<HealthStamina>().m_CurrentStamina, m_LockTarget.GetComponent<CombatBehaviour>().m_StateElapesedTime, (int)Moves.KICK);

        }
        else if ((Input.GetKey(KeyCode.S) || Input.GetKeyDown(KeyCode.S)) && Input.GetKeyDown(KeyCode.Mouse1))
        {
            ThrowAttack(m_NormalStance.DownKick, m_Kicks[m_NormalStance.DownKick.Head], m_LockTarget);
            ObserveAI();
            //m_Recorder.WriteToFile((int)m_AIState, GetComponent<HealthStamina>().m_CurrentStamina, m_LockTarget.GetComponent<CombatBehaviour>().m_StateElapesedTime, (int)Moves.KICK);

        }
        else if((Input.GetKey(KeyCode.D) || Input.GetKeyDown(KeyCode.D)) && Input.GetKeyDown(KeyCode.Mouse1))
        {
            ThrowAttack(m_NormalStance.RightKick, m_Kicks[m_NormalStance.RightKick.Head], m_LockTarget);
            ObserveAI();
        }
        else if ((Input.GetKey(KeyCode.W) || Input.GetKeyDown(KeyCode.W)) && Input.GetKeyDown(KeyCode.Mouse0))
        {
            ThrowAttack(m_NormalStance.UpPunch, m_Punches[m_NormalStance.UpPunch.Head], m_LockTarget);
            ObserveAI();
            //m_Recorder.WriteToFile((int)m_AIState, GetComponent<HealthStamina>().m_CurrentStamina, m_LockTarget.GetComponent<CombatBehaviour>().m_StateElapesedTime, (int)Moves.KICK);

        }
        else if ((Input.GetKey(KeyCode.D) || Input.GetKeyDown(KeyCode.D)) && Input.GetKeyDown(KeyCode.Mouse0))
        {
            ThrowAttack(m_NormalStance.RightPunch, m_Punches[m_NormalStance.RightPunch.Head], m_LockTarget);
            ObserveAI();
            //m_Recorder.WriteToFile((int)m_AIState, GetComponent<HealthStamina>().m_CurrentStamina, m_LockTarget.GetComponent<CombatBehaviour>().m_StateElapesedTime, (int)Moves.KICK);

        }
        else if ((Input.GetKey(KeyCode.A) || Input.GetKeyDown(KeyCode.A)) && Input.GetKeyDown(KeyCode.Mouse0))
        {
            ThrowAttack(m_NormalStance.LeftPunch, m_Punches[m_NormalStance.LeftPunch.Head], m_LockTarget);
            ObserveAI();
            //m_Recorder.WriteToFile((int)m_AIState, GetComponent<HealthStamina>().m_CurrentStamina, m_LockTarget.GetComponent<CombatBehaviour>().m_StateElapesedTime, (int)Moves.KICK);

        }
        //Debug.Log(m_NormalStance.UpPunch.Head);
    }
    private void OnLockOnTarget(GameObject lockTarget)
    {
        m_LockTarget = lockTarget;
    }
    private void OnReleaseLockOnTarget()
    {
        m_LockTarget = null;
    }
}
