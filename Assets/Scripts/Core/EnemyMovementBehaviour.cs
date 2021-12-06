using System.Collections;
using UnityEngine;
using UnityEngine.AI;
public class EnemyMovementBehaviour : MovementBehaviour
{
    private GameObject     m_Player;
    private Vector3        m_WayPoint;
    public bool            m_WantToWander; //Does AI want to wander or not.
    private NavMeshAgent   m_NavMeshAgent; //AI is moved with Unity's NavMeshAgent component.
    private bool           m_IsAttacking;
    private bool DoOnce; //A flag used to call a function once in Update().

    //AI difficulty settings---------
    public float m_WanderRadius = 5.0f;
    protected override void Awake()
    {
        base.Awake();
        m_Player = GameObject.FindWithTag("Player");
    }
    private void Start()
    {
        DoOnce = true;
        EventManager.GetInstance().OnAIStartsWandering += OnAIStartsWandering;
        EventManager.GetInstance().OnAIIsAttacking     += OnAIIsAttacking;
        EventManager.GetInstance().OnAIIsNotAttacking  += OnAIIsNotAttacking;
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
        m_WantToWander = false;
        m_IsAttacking = false;
    }
    void Update()
    {
        if (!PauseMenu.IsGamePaused) 
        {
            if (m_WantToWander)
            {
                if ((transform.position - m_WayPoint).magnitude >= m_WanderRadius)
                {
                    PickWanderWaypoint(m_WanderRadius);
                }
            }
            if (m_Animator.GetBool("isDead"))
            {
                EventManager.GetInstance().OnAIStartsWandering -= OnAIStartsWandering;
                EventManager.GetInstance().OnAIIsAttacking     -= OnAIIsAttacking;
                EventManager.GetInstance().OnAIIsNotAttacking  -= OnAIIsNotAttacking;
                enabled = false;
            }

            if (m_MovementDirection != Vector3.zero)
                m_TurnRotation = Quaternion.LookRotation(m_MovementDirection, Vector3.up);


            Movement(m_MovementDirection.normalized.x, m_MovementDirection.normalized.z);
        }
    }
    protected override void Movement(float horizontal = 0.0f, float vertical = 0.0f) //Movement is done with Unity's NavMeshAgent.
    {
        if (!m_DisableMovement)
        {
            if (Vector3.Distance(transform.position, m_Player.transform.position) <= 10.0f) //Acquire player if he gets into the range once and target him always after acquisition.
            {
                if(DoOnce)
                {
                    DoOnce = false;
                    EventManager.GetInstance().EnemySawPlayer();
                }
                m_LockTarget = m_Player;
            }
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

                        //Set the animator floats necessary to animate the legs of the character. The legs of AI rotate and twist according to the values of 'PosX' and 'PosY'.
                        m_Animator.SetFloat("PosX", Mathf.Sin(rad) , 1.0f, Time.deltaTime * 20.0f); //Sinus of the degree gives me the local x axis direction.
                        m_Animator.SetFloat("PosY", Mathf.Cos(rad), 1.0f, Time.deltaTime * 20.0f);  //Cosine of the degree gives me the local y axis direction.
                    }
                    else
                    {
                        m_NavMeshAgent.isStopped = true;
                        m_Animator.SetBool("isMoving", false);
                        m_Animator.SetFloat("PosX", 0.0f, 1.0f, Time.deltaTime * 20.0f);
                        m_Animator.SetFloat("PosY", 0.0f, 1.0f, Time.deltaTime * 20.0f);
                    }
                }
                else //AI become aggressive and moves towards the player.
                {
                    if (Vector3.Distance(transform.position, m_Player.transform.position) > 3.3f) //Not in attack range.
                    {
                        m_NavMeshAgent.speed = m_LockedOnMovementSpeed;
                        m_NavMeshAgent.isStopped = false;
                        m_Animator.SetBool("isMoving", true);
                        m_Animator.SetFloat("PosX", 0.0f, 1.0f, Time.deltaTime * 20.0f);
                        m_Animator.SetFloat("PosY", 1.0f, 1.0f, Time.deltaTime * 20.0f);

                        m_NavMeshAgent.destination = m_Player.transform.position;
                    }
                    else //In attack range so, stop the agent and let other classes take control of the movement like 'AttackStateMachine'.
                    {
                        m_NavMeshAgent.isStopped = true;
                        m_Animator.SetBool("isMoving", false);
                        m_Animator.SetFloat("PosX", 0.0f, 1.0f, Time.deltaTime * 20.0f);
                        m_Animator.SetFloat("PosY", 0.0f, 1.0f, Time.deltaTime * 20.0f);
                    }
                }
            }
            else 
            {
                m_Animator.SetBool("isLockedOn", false);
                m_Animator.SetBool("isRunning", false);
                transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0); //Zero out x and z.
            }
        }
        else
        {
            m_NavMeshAgent.isStopped = true;
            m_Animator.SetBool("isMoving", false);
            m_Animator.SetFloat("PosX", 0.0f, 1.0f, Time.deltaTime * 20.0f);
            m_Animator.SetFloat("PosY", 0.0f, 1.0f, Time.deltaTime * 20.0f);
        }
    }

    //TODO: Improve wandering algortihm.
    public void PickWanderWaypoint(float wanderRadius) //This function gets a random point in a 180 degree radiues behin the AI.
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
