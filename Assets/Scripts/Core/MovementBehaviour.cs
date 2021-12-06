using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generic movement class for both player and enemyAI.
/// </summary>
public abstract class MovementBehaviour : MonoBehaviour
{
    [HideInInspector]
    public Quaternion        m_TurnRotation;           //Target rotation for the character to turn to in the next frame.
    [HideInInspector]
    public GameObject        m_LockTarget;             //GameObject that the character will lock onto.
    [HideInInspector]
    public bool              m_DisableMovement;        //This is being set by the attack animaitons in the game. Prevents player from moving during certain actions like attacks.
    protected readonly float m_MovementSpeed;          //Movement speed when the character is free roaming.
    protected readonly float m_LockedOnMovementSpeed;  //When the character is locked on and fighting it's target, this speed is applied.
    protected Vector3        m_MovementDirection;      //The calculate movement direction of the character. Depending on whether the character is locked on and fighting or free roaming, it changes.
    protected float          m_RetreatMovementSpeed;   //When the character moves in general backward direction, this speed is applied.
    protected Animator       m_Animator;
    protected Rigidbody      m_RigidBody;
    public bool              m_IsOnGrass = false;      //A flag the determines if the character is moving on grass or rock. Corresponding sounds are played depending on this variable.
    protected MovementBehaviour()
    {
        m_MovementSpeed         = 3.5f;
        m_RetreatMovementSpeed  = 1.0f;
        m_LockedOnMovementSpeed = 2f;
        m_LockTarget            = null;
        m_DisableMovement       = false;
    }
    protected virtual void Awake()
    {
        m_RigidBody = GetComponent<Rigidbody>();
        m_Animator = GetComponent<Animator>();
        m_IsOnGrass = false;
    }
    protected abstract void Movement(float horizontal = 0.0f, float vertical = 0.0f); //Must be implemented by children. This is where the movement code is handled.
    protected void TurnCharacterTowards(GameObject targetObject, float turnSpeed) //Slowly turns character towards a target gameObjects.
    {
        Vector3 targetRotation = targetObject.transform.position - transform.position;
        targetRotation = new Vector3(targetRotation.x, 0, targetRotation.z); //Zero out Y axis.
        m_RigidBody.MoveRotation(Quaternion.RotateTowards(m_RigidBody.rotation, Quaternion.LookRotation(targetRotation), turnSpeed * Time.deltaTime));
    }
    protected void TurnCharacterTowards(Vector3 direction, float turnSpeed) //Overloaded version that turns character towards a Vector3 direction.
    {
        Quaternion turnDirectionQuaternion = Quaternion.LookRotation(direction, Vector3.up);
        m_RigidBody.MoveRotation(Quaternion.RotateTowards(transform.rotation, turnDirectionQuaternion, turnSpeed * Time.deltaTime));
    }
    private void OnCollisionStay(Collision collision) //Checks which surface is the character stepping on.
    {
        if (collision.gameObject.CompareTag("EnemyAI") || collision.gameObject.CompareTag("Player") )
            return;
        if(collision.gameObject.CompareTag("Terrain"))
        {
            m_IsOnGrass = true;
        }
        else
        {
            m_IsOnGrass = false;
        }
    }
}
