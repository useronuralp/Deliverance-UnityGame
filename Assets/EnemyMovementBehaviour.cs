using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementBehaviour : MovementBehaviour
{
    private GameObject m_Player;
    private Vector3 m_WayPoint;
    public  bool  m_WantToWander;
    private float m_ResetTime = 5.0f;
    private float m_TimeOutDuration;
    private CombatBehaviour m_CombatScript;
    protected override void Awake()
    {
        base.Awake();
        m_CombatScript = GetComponent<CombatBehaviour>();
        m_Player = GameObject.FindWithTag("Player");
        m_TimeOutDuration = m_ResetTime;
    }
    private void Start()
    {
        m_WantToWander = false;
        if(m_WantToWander)
            PickWanderWaypoint(5);
    }
    void Update()
    {
        //Debug.Log(m_WantToWander);
        if (m_Animator.GetBool("isDead"))
        {
            enabled = false;
        }
        //Debug.Log("Disable movement:" + m_DisableMovement);
        //Debug.Log("Is Getting hit:" + m_Animator.GetComponent<ICombatBehaviour>().m_IsGettingHit);
        //Debug.Log("Prevent attacks: " + m_Animator.GetComponent<ICombatBehaviour>().m_PreventAttacktInputs);

        if(m_WantToWander) 
        {
            m_TimeOutDuration -= Time.deltaTime; //This is a safeguard for wandering. If the AI hasn't reached the destination by the time this variable hits zero. Pick a new waypoint. 
            if (m_TimeOutDuration <= 0.0f)
            {
                PickWanderWaypoint(5);
            }
        }

        //m_MovementDirection.Normalize();

        if (m_MovementDirection != Vector3.zero)
            m_TurnRotation = Quaternion.LookRotation(m_MovementDirection, Vector3.up);

        //Debug.Log(m_MovementDirection);
        if(!m_CombatScript.m_IsStunned)
        {
            Movement(m_MovementDirection.normalized.x, m_MovementDirection.normalized.z);
        }
    }
    protected override void Movement(float horizontal = 0.0f, float vertical = 0.0f)
    {
        if (!m_DisableMovement)
        {
            if (Vector3.Distance(transform.position, m_Player.transform.position) <= 10.0f) //TODO: When AI leaves this radius, it bugs out.
                m_LockTarget = m_Player;
            else
                m_LockTarget = null;

            if(m_LockTarget)
            {
                m_Animator.SetBool("isLockedOn", true);
                transform.LookAt(new Vector3(m_LockTarget.transform.position.x, transform.position.y, m_LockTarget.transform.position.z));


                Vector3 direction = m_MovementDirection;
                if (m_WantToWander) //AI chills and wanders around.
                {
                    if ((transform.position - m_WayPoint).magnitude < 3)
                        PickWanderWaypoint(5);

                    //TODO: This part does not work correctly, 
                    LockedOnMovement(horizontal, vertical);
                }
                else //AI becomes the aggressor.
                {
                    if (Vector3.Distance(transform.position, m_Player.transform.position) > 2.6f)
                    {
                        //m_Player.transform.position - transform.position;
                        m_MovementDirection = m_Player.transform.position - transform.position;
                        direction = new Vector3(0,0,1); //Straight forward walking animation.
                        direction.Normalize();
                        LockedOnMovement(direction.x, direction.z);
                    }
                }
            }
            else 
            {
                m_Animator.SetBool("isLockedOn", false);
                if (m_WantToWander)
                {
                    m_Animator.SetBool("isRunning", true);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, m_TurnRotation, 1000 * Time.deltaTime);

                    if ((transform.position - m_WayPoint).magnitude < 3)
                        PickWanderWaypoint(5);
                    
                    transform.position += m_MovementSpeed * Time.deltaTime * new Vector3(transform.forward.x, 0, transform.forward.z);
                }
                else //Standing still
                {
                    m_Animator.SetBool("isRunning", false);
                }
                transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0); //Zero out x and z.
            }
        }
    }

    //TODO: Improve wandering algortihm.
    void PickWanderWaypoint(float wanderRadius)
    { 
        m_TimeOutDuration = m_ResetTime;
        m_WayPoint = new Vector3(Random.Range(transform.position.x - wanderRadius, transform.position.x + wanderRadius), transform.position.y, Random.Range(transform.position.z - wanderRadius, transform.position.z + wanderRadius))
        {
            y = 1
        };
        m_MovementDirection = (m_WayPoint - transform.position).normalized;        
    }
}
