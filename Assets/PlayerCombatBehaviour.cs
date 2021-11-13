using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatBehaviour : CombatBehaviour
{

    private State m_AIState = State.NONE;
    private FileIO m_Recorder;
    private List<string> m_AvailablePunches;
    private List<string> m_AvailableKicks;
    private bool m_ReleaseGuard;
    protected override void Awake()
    {
        base.Awake();
        //Child initializations go here...
    }
    private void Start()
    {
        m_ReleaseGuard = false;
        m_AvailablePunches = new List<string>();
        m_AvailableKicks   = new List<string>();
        ResetKicks();
        ResetPunches();
        //m_Recorder         = new FileIO("TrainingData.txt");
    }
    void Update()
    {
        //Debug.Log(m_IsGettingHit);
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
            if (Input.GetKeyDown(KeyCode.X))
            {
                Parry();
                ObserveAI();
                //m_Recorder.WriteToFile((int)m_AIState, GetComponent<HealthStamina>().m_CurrentStamina, m_MovementScript.m_LockTarget.GetComponent<CombatBehaviour>().m_StateElapesedTime, (int)Moves.PARRY);

            }
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Punch();
                ObserveAI();
                //m_Recorder.WriteToFile((int)m_AIState, GetComponent<HealthStamina>().m_CurrentStamina, m_MovementScript.m_LockTarget.GetComponent<CombatBehaviour>().m_StateElapesedTime, (int)Moves.KICK);
            }
            else if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                Kick();
                ObserveAI();
                //m_Recorder.WriteToFile((int)m_AIState, GetComponent<HealthStamina>().m_CurrentStamina, m_MovementScript.m_LockTarget.GetComponent<CombatBehaviour>().m_StateElapesedTime, (int)Moves.KICK);

            }
            if(!m_IsAttacking && !m_IsGettingHit && !m_IsParryingFull && !m_IsStunned)
            {
                if(Input.GetKeyDown(KeyCode.Space))
                {
                    GuardUp();
                }
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
    public void Kick()
    {
        int attackIndex = Random.Range(0, m_AvailableKicks.Count);
        if(ThrowAttack(m_AvailableKicks[attackIndex], m_Kicks[m_AvailableKicks[attackIndex]]))
        {
            m_AvailableKicks.RemoveAt(attackIndex);
            if (m_AvailableKicks.Count == 0)
            {
                ResetKicks();
            }
        }
    }
    public void Punch()
    {
        Debug.Log(m_AvailablePunches.Count);
        int attackIndex = Random.Range(0, m_AvailablePunches.Count);
        if(ThrowAttack(m_AvailablePunches[attackIndex], m_Punches[m_AvailablePunches[attackIndex]]))
        {
            m_AvailablePunches.RemoveAt(attackIndex);
            if (m_AvailablePunches.Count == 0)
            {
                ResetPunches();
            }
        }
    }
    public override void ResetPunches()
    {
        m_AvailablePunches = new List<string>();
        foreach(var element in m_Punches)
        {
            m_AvailablePunches.Add(element.Key);
        }
    }
    public override void ResetKicks()
    {
        m_AvailableKicks = new List<string>();
        foreach (var element in m_Kicks)
        {
            m_AvailableKicks.Add(element.Key);
        }
    }
}
