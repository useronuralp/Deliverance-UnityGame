using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class EnemyMovementBehaviour : MovementBehaviour
{
    private GameObject     m_Player;
    private Vector3        m_WayPoint;
    public bool            m_WantToWander;
    private NavMeshAgent   m_NavMeshAgent;
    private readonly float m_WanderRadius = 5.0f;
    private bool           m_IsAttacking;
    protected override void Awake()
    {
        base.Awake();
        m_Player = GameObject.FindWithTag("Player");
    }
    private void Start()
    {
        EventManager.GetInstance().OnAIStartsWandering += OnAIStartsWandering;
        EventManager.GetInstance().OnAIIsAttacking     += OnAIIsAttacking;
        EventManager.GetInstance().OnAIIsNotAttacking  += OnAIIsNotAttacking;
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
        m_WantToWander = false;
        m_IsAttacking = false;
    }
    void Update()
    {
        //Debug.Log(m_NavMeshAgent.isStopped);
        if (m_WantToWander)
        {
            if ((transform.position - m_WayPoint).magnitude >= m_WanderRadius)
            {
                PickWanderWaypoint(m_WanderRadius);
            }
        }
        //Debug.Log(m_IsAttacking);
        if (m_Animator.GetBool("isDead"))
        {
            enabled = false;
        }

        if (m_MovementDirection != Vector3.zero)
            m_TurnRotation = Quaternion.LookRotation(m_MovementDirection, Vector3.up);


        Movement(m_MovementDirection.normalized.x, m_MovementDirection.normalized.z);

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
                TurnCharacterTowards(m_LockTarget, 500.0f);

                if (m_WantToWander) //AI chills and wanders around.
                {
                    m_NavMeshAgent.isStopped = false;
                    if ((transform.position - m_WayPoint).magnitude < 2.0f)
                    {
                        PickWanderWaypoint(m_WanderRadius);
                    }
                    if(!m_IsAttacking)
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
                    else
                    {
                        m_NavMeshAgent.isStopped = true;
                    }
                }
            }
            else 
            {
                m_Animator.SetBool("isLockedOn", false);
                if (m_WantToWander)
                {
                    m_Animator.SetBool("isRunning", true);
                    //Debug.Log(m_MovementDirection);
                    TurnCharacterTowards(m_MovementDirection.normalized, 1000.0f);

                    if ((transform.position - m_WayPoint).magnitude < 3)
                        PickWanderWaypoint(m_WanderRadius);

                    m_RigidBody.MovePosition(transform.position + m_MovementSpeed * Time.deltaTime * transform.forward);
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
    IEnumerator Wander(float wanderTime)
    {
        m_WantToWander = true;
        yield return new WaitForSeconds(wanderTime);
        m_NavMeshAgent.destination = m_Player.transform.position;
        m_WantToWander = false;
        EventManager.GetInstance().AIWantsToAttackPlayer();
    }
    private void OnAIStartsWandering()
    {
        StartCoroutine(Wander(Random.Range(1,4)));
    }
    private void OnAIIsAttacking()
    {
        m_IsAttacking = true;
    }
    private void OnAIIsNotAttacking()
    {
        m_IsAttacking = false;
    }
}
