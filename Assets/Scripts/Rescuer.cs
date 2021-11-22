using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Rescuer : MonoBehaviour
{
    private float m_MovementSpeed = 0.4f;
    private Animator m_Animator;
    private Rigidbody m_RigidBody;
    private float m_Timer = 0.0f;
    private float m_TurnDuration;
    public SpriteRenderer m_QuestionMark;
    public BoxCollider m_Collider;
    private float m_WalkStartTimer;
    private bool m_DidStartWalking;
    private void Start()
    {
        m_DidStartWalking = false;
        m_WalkStartTimer = 2.0f;
        m_TurnDuration = 1.0f;
        m_Animator = GetComponent<Animator>();
        m_RigidBody = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        if(m_WalkStartTimer <= 0.0f && !m_DidStartWalking)
        {
            m_DidStartWalking = true;
            StartCoroutine(MoveToTalkingPosition());
        }
        m_WalkStartTimer -= Time.deltaTime;
        Debug.Log(m_Timer);
    }
    IEnumerator MoveToTalkingPosition()
    {
        while(m_Timer < 17.3f)
        {
            m_Animator.SetBool("isMoving", true);
            m_Animator.SetFloat("PosX", 0.0f, 1.0f, Time.deltaTime * 20.0f); 
            m_Animator.SetFloat("PosY", 1.0f, 1.0f, Time.deltaTime * 20.0f);
            m_RigidBody.MovePosition(transform.position + m_MovementSpeed * Time.deltaTime * transform.forward);
            m_Timer += Time.deltaTime;
            yield return null; 
        }
        var rotation = Quaternion.LookRotation(transform.forward);
        rotation *= Quaternion.Euler(0, 90, 0); // this adds a 90 degrees Y rotation
        while(m_TurnDuration >= 0.0f)
        {
            m_TurnDuration -= Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5.0f);
            yield return null;
        }
        m_QuestionMark.enabled = true;
        m_Collider.enabled = true;
    }
}
