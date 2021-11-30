using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
public class Rescuer : MonoBehaviour
{
    private Animator m_Animator;
    private Rigidbody m_RigidBody;
    private float m_TurnDuration;
    public SpriteRenderer m_QuestionMark;
    public BoxCollider m_Collider;
    private float m_WalkStartTimer;
    private bool m_DidStartWalking;
    private NavMeshAgent m_NavMeshAgent;

    public GameObject m_TalkPoint;
    public GameObject m_LeavePoint;


    private float m_ElapsedTime;
    private bool m_StartTimer;
    private void Awake()
    {
        if(GameState.WasTutorialAlreadyTriggered)
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        m_StartTimer = false;
        m_ElapsedTime = 0.0f;
        m_DidStartWalking = false;
        m_WalkStartTimer = 2.0f;
        m_TurnDuration = 1.0f;
        m_Animator = GetComponent<Animator>();
        m_RigidBody = GetComponent<Rigidbody>();
        EventManager.GetInstance().OnRescuerEnteredStopTrigger += OnEnteringStopTrigger;
        EventManager.GetInstance().OnPlayerCompletingDialogue += OnPlayerCompletesDialogue;
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
        m_NavMeshAgent.isStopped = true;

    }
    private void Update()
    {
        if(m_ElapsedTime >= 7.0f)
        {
            Destroy(gameObject);
        }
        if(m_StartTimer)
        {
            m_ElapsedTime += Time.deltaTime;
        }
        if (m_WalkStartTimer <= 0.0f && !m_DidStartWalking)
        {
            m_DidStartWalking = true;
            StartCoroutine(MoveToTalkingPosition());
        }
        m_WalkStartTimer -= Time.deltaTime;
    }
    IEnumerator MoveToTalkingPosition()
    {
        m_NavMeshAgent.isStopped = false;
        m_NavMeshAgent.speed = 0.4f;
        m_NavMeshAgent.destination = m_TalkPoint.transform.position;
        yield return null;
    }
    IEnumerator TurnBackAndRun()
    {
        m_NavMeshAgent.isStopped = false;
        m_NavMeshAgent.speed = 3.0f;
        m_NavMeshAgent.destination = m_LeavePoint.transform.position;
        m_NavMeshAgent.angularSpeed = 360.0f;
        while(true)
        {
            m_Animator.SetBool("isRunning", true);
            yield return null;
        }
    }
    IEnumerator FaceWindow()
    {
        var rotation = Quaternion.LookRotation(transform.forward);
        rotation *= Quaternion.Euler(0, 90, 0); // this adds a 90 degrees Y rotation
        while (m_TurnDuration >= 0.0f)
        {
            m_TurnDuration -= Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5.0f);
            yield return null;
        }
        m_QuestionMark.enabled = true;
        m_Collider.enabled = true;
        yield return null;
    }
    void OnEnteringStopTrigger()
    {
        StartCoroutine(FaceWindow());
    }
    void OnPlayerCompletesDialogue()
    {
        m_StartTimer = true;
        StartCoroutine(TurnBackAndRun());
    }

}
