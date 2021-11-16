using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class EnemyMovementBehaviour : MovementBehaviour
{
    private GameObject m_Player;
    private Vector3 m_WayPoint;
    public  bool  m_WantToWander;
    private CombatBehaviour m_CombatScript; //TODO: High Coupling.
    private NavMeshAgent m_NavMeshAgent;
    private readonly float m_WanderRadius = 5.0f;
    protected override void Awake()
    {
        base.Awake();
        m_CombatScript = GetComponent<CombatBehaviour>();
        m_Player = GameObject.FindWithTag("Player");
    }
    private void Start()
    {
        m_WantToWander = false;
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
    }
    void Update()
    {
        if (m_WantToWander)
        {
            if ((transform.position - m_WayPoint).magnitude >= m_WanderRadius)
            {
                PickWanderWaypoint(m_WanderRadius);
            }
        }
        //Debug.Log(m_WantToWander);
        if (m_Animator.GetBool("isDead"))
        {
            enabled = false;
        }

        if (m_MovementDirection != Vector3.zero)
            m_TurnRotation = Quaternion.LookRotation(m_MovementDirection, Vector3.up);

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
                var q = Quaternion.LookRotation(m_LockTarget.transform.position - transform.position);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, q, 1000 * Time.deltaTime);
                if (m_WantToWander) //AI chills and wanders around.
                {
                    m_NavMeshAgent.isStopped = false;
                    if ((transform.position - m_WayPoint).magnitude < 2.0f)
                    {
                        PickWanderWaypoint(m_WanderRadius);
                    }
                    if(!m_CombatScript.m_IsAttacking)
                    {
                        m_Animator.SetBool("isMoving", true); 
                        m_NavMeshAgent.speed = m_LockedOnMovementSpeed;
                        m_NavMeshAgent.destination = m_WayPoint;
                        float degree = Vector3.SignedAngle(transform.forward, (m_WayPoint - transform.position).normalized, Vector3.up); //Angle between the waypoint and facing direction.
                        float rad = degree * Mathf.Deg2Rad; //Convert the degree to radians.

                        m_Animator.SetFloat("PosX", Mathf.Sin(rad) , 1.0f, Time.deltaTime * 20.0f); //Sinus of the degree gives me the local x axis direction.
                        m_Animator.SetFloat("PosY", Mathf.Cos(rad), 1.0f, Time.deltaTime * 20.0f);  //Cosine of the degree gives me the local y axis direction.
                    }
                    else
                    {
                        m_NavMeshAgent.isStopped = true;
                    }
                }
                else //AI becomes the aggressor.
                {
                    if (Vector3.Distance(transform.position, m_Player.transform.position) > 3.3f)
                    {
                        m_NavMeshAgent.speed = m_LockedOnMovementSpeed;
                        m_NavMeshAgent.isStopped = false;
                        m_Animator.SetBool("isMoving", true);
                        m_Animator.SetFloat("PosX", 0.0f, 1.0f, Time.deltaTime * 20.0f);
                        m_Animator.SetFloat("PosY", 1.0f, 1.0f, Time.deltaTime * 20.0f);

                        m_NavMeshAgent.destination = m_Player.transform.position;
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
                        PickWanderWaypoint(m_WanderRadius);
                    
                    transform.position += m_MovementSpeed * Time.deltaTime * new Vector3(transform.forward.x, 0, transform.forward.z);
                }
                else //Standing still
                {
                    m_Animator.SetBool("isRunning", false);
                }
                transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0); //Zero out x and z.
            }
        }
        else
        {
            m_NavMeshAgent.isStopped = true;
        }
    }

    //TODO: Improve wandering algortihm.
    public void PickWanderWaypoint(float wanderRadius) //Get a random point in a radius of 180 degrees between the bot.
    {
        int angle = Random.Range(-90, 91);
        float rad = angle * Mathf.Deg2Rad;
        Vector3 position = new Vector3(Mathf.Sin(rad), 0, Mathf.Cos(rad));
        position *= wanderRadius;

        m_WayPoint = transform.TransformPoint(-position);
        m_MovementDirection = (m_WayPoint - transform.position).normalized;        
    }
}
