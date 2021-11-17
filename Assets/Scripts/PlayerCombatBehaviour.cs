using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatBehaviour : CombatBehaviour
{
    private State m_AIState = State.NONE;
    private FileIO m_Recorder;
    private float m_GuardLockDuration;
    protected override void Awake()
    {
        base.Awake();
        //Child initializations go here...
    }
    private void Start()
    {
        m_GuardLockDuration = 0.5f;
        //m_Recorder         = new FileIO("TrainingData.txt");
    }
    void Update()
    {
        Debug.Log(m_StateElapesedTime);
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
            ProcessAttackInput();

            if (!m_IsAttacking && !m_IsGettingHit && !m_IsParryingFull && !m_IsStunned)
            {
                if (Input.GetKey(KeyCode.Space))
                {
                    GuardUp();
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
        m_Recorder.WriteToFile((int)m_AIState, GetComponent<HealthStamina>().m_CurrentStamina, m_MovementScript.m_LockTarget.GetComponent<CombatBehaviour>().m_StateElapesedTime, (int)move);
    }
    void ObserveAI()
    {
        if(GetComponent<MovementBehaviour>().m_LockTarget)
        {
            if (m_MovementScript.m_LockTarget.GetComponent<CombatBehaviour>().m_IsParryingFull)
            {
                m_AIState = State.PARRY;
            }
            else if (m_MovementScript.m_LockTarget.GetComponent<CombatBehaviour>().m_IsGuarding || m_MovementScript.m_LockTarget.GetComponent<CombatBehaviour>().m_IsBlocking)
            {
                m_AIState = State.GUARD;
            }
            else if (m_MovementScript.m_LockTarget.GetComponent<CombatBehaviour>().m_IsAttacking)
            {
                if (m_MovementScript.m_LockTarget.GetComponent<CombatBehaviour>().m_CurrentAttack.Contains("Kick"))
                {
                    m_AIState = State.KICK;
                }
                else if (m_MovementScript.m_LockTarget.GetComponent<CombatBehaviour>().m_CurrentAttack.Contains("Punch"))
                {
                    m_AIState = State.PUNCH;
                }
            }
            else if (m_MovementScript.m_LockTarget.GetComponent<CombatBehaviour>().m_IsIdle)
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
            //m_Recorder.WriteToFile((int)m_AIState, GetComponent<HealthStamina>().m_CurrentStamina, m_MovementScript.m_LockTarget.GetComponent<CombatBehaviour>().m_StateElapesedTime, (int)Moves.PARRY);

        }
        if (Input.GetKey(KeyCode.W) && Input.GetKeyDown(KeyCode.Mouse1))
        {
            ThrowAttack(m_NormalStance.UpKick, m_Kicks[m_NormalStance.UpKick.Head]);
            ObserveAI();
            //m_Recorder.WriteToFile((int)m_AIState, GetComponent<HealthStamina>().m_CurrentStamina, m_MovementScript.m_LockTarget.GetComponent<CombatBehaviour>().m_StateElapesedTime, (int)Moves.KICK);
        }
        else if (Input.GetKey(KeyCode.A) && Input.GetKeyDown(KeyCode.Mouse1))
        {
            ThrowAttack(m_NormalStance.LeftKick, m_Kicks[m_NormalStance.LeftKick.Head]);
            ObserveAI();
            //m_Recorder.WriteToFile((int)m_AIState, GetComponent<HealthStamina>().m_CurrentStamina, m_MovementScript.m_LockTarget.GetComponent<CombatBehaviour>().m_StateElapesedTime, (int)Moves.KICK);

        }
        else if (Input.GetKey(KeyCode.S) && Input.GetKeyDown(KeyCode.Mouse1))
        {
            ThrowAttack(m_NormalStance.DownKick, m_Kicks[m_NormalStance.DownKick.Head]);
            ObserveAI();
            //m_Recorder.WriteToFile((int)m_AIState, GetComponent<HealthStamina>().m_CurrentStamina, m_MovementScript.m_LockTarget.GetComponent<CombatBehaviour>().m_StateElapesedTime, (int)Moves.KICK);

        }
        else if(Input.GetKey(KeyCode.D) && Input.GetKeyDown(KeyCode.Mouse1))
        {
            ThrowAttack(m_NormalStance.RightKick, m_Kicks[m_NormalStance.RightKick.Head]);
            ObserveAI();
        }
        else if (Input.GetKey(KeyCode.W) && Input.GetKeyDown(KeyCode.Mouse0))
        {
            ThrowAttack(m_NormalStance.UpPunch, m_Punches[m_NormalStance.UpPunch.Head]);
            ObserveAI();
            //m_Recorder.WriteToFile((int)m_AIState, GetComponent<HealthStamina>().m_CurrentStamina, m_MovementScript.m_LockTarget.GetComponent<CombatBehaviour>().m_StateElapesedTime, (int)Moves.KICK);

        }
        else if (Input.GetKey(KeyCode.D) && Input.GetKeyDown(KeyCode.Mouse0))
        {
            ThrowAttack(m_NormalStance.RightPunch, m_Punches[m_NormalStance.RightPunch.Head]);
            ObserveAI();
            //m_Recorder.WriteToFile((int)m_AIState, GetComponent<HealthStamina>().m_CurrentStamina, m_MovementScript.m_LockTarget.GetComponent<CombatBehaviour>().m_StateElapesedTime, (int)Moves.KICK);

        }
        else if (Input.GetKey(KeyCode.A) && Input.GetKeyDown(KeyCode.Mouse0))
        {
            ThrowAttack(m_NormalStance.LeftPunch, m_Punches[m_NormalStance.LeftPunch.Head]);
            ObserveAI();
            //m_Recorder.WriteToFile((int)m_AIState, GetComponent<HealthStamina>().m_CurrentStamina, m_MovementScript.m_LockTarget.GetComponent<CombatBehaviour>().m_StateElapesedTime, (int)Moves.KICK);

        }
        //Debug.Log(m_NormalStance.UpPunch.Head);
    }
}
