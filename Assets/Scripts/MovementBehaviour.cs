using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generic movement class for both player and enemyAI. Inheriting classes should be the ones that implement Movement(float horizontal = 0.0f, float vertical = 0.0f).
/// </summary>
public abstract class MovementBehaviour : MonoBehaviour
{
    public Quaternion        m_TurnRotation;           //Target rotation for the character to turn to in the next frame.
    public GameObject        m_LockTarget;             //GameObject that the character will lock onto.
    public bool              m_DisableMovement;        //This is being set by the attack animaitons in the game. Prevents player from moving during certain actions like attacks.
    protected readonly float m_MovementSpeed;          //Movement speed when the character is free roaming.
    protected readonly float m_LockedOnMovementSpeed;  //When the character is locked on and fighting it's target, this speed is applied.
    protected Vector3        m_MovementDirection;      //The calculate movement direction of the character. Depending on whether the character is locked on and fighting or free roaming, it changes.
    protected float          m_RetreatMovementSpeed;   //When the character moves in general backward direction, this speed is applied.
    protected Animator       m_Animator;
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
        m_Animator = GetComponent<Animator>();
    }
    protected abstract void Movement(float horizontal = 0.0f, float vertical = 0.0f); //Must be implemented by children. This is where the movement code is handled.
}
