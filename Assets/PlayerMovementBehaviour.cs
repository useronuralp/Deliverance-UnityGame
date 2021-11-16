using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/// <summary>
/// Camera movements are also handled in this script along with player movement.
/// </summary>
public class PlayerMovementBehaviour : MovementBehaviour
{
    private readonly float      m_PlayerRotationSpeedInEulerAngles = 1000.0f; //Rotation speed of the player.
    private Vector3             m_CameraDirection;                            //Forward vector of the main camera. 
    private Vector3             m_PlayerForwardDirection;                     //Forward vector of the player character. 
    public  CinemachineFreeLook m_FreeLookCamera;                             //Free look camera that is attached to player character.
    private CombatBehaviour    m_CombatScript;

    protected override void Awake()
    {
        base.Awake();
        m_CombatScript = GetComponent<CombatBehaviour>();
        //Continue with the child class initializations here...
    }
    void Start()
    {
        CinemachineCore.GetInputAxis = GetAxisCustom;  //Pass Cinemachine delegate my own custom axis reading function.

        //TODO: Changing the lock target DURING attacks cause an error.

        //Setting the default values for the camera launch.
        SetTrackedTargetOffset(1.2f);
        SetCenteringValues(3.0f, 5.0f, 0);
        SetCameraPosition(new CinemachineRig(3.0f, 4.0f), new CinemachineRig(1.5f, 5.0f), new CinemachineRig(0.1f, 4.0f));
        SetCameraCenteringTo(false);
        CameraBindingMode(CinemachineTransposer.BindingMode.WorldSpace);
    }
    void Update()
    {
        int generalAngleOfMovement = (int)Vector3.SignedAngle(transform.forward, m_MovementDirection, Vector3.up);
        if (m_Animator.GetBool("isDead")) //Handling death on top before everything else.
        {
            m_FreeLookCamera.LookAt = transform;
            SetCameraPosition(new CinemachineRig(3.0f, 4.0f), new CinemachineRig(1.5f, 5.0f), new CinemachineRig(0.1f, 4.0f));
            enabled = false;
        }

        //TODO: Check out FixedUpdata & LateUpdate.
        m_CameraDirection = m_FreeLookCamera.transform.forward; /*--*/ m_CameraDirection.y = 0; 
        m_PlayerForwardDirection = transform.forward;           /*--*/ m_PlayerForwardDirection.y = 0;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (m_LockTarget) //If locked, movement is based on where the character is facing.
        {
            m_MovementDirection = transform.forward * vertical + transform.right * horizontal;
        }
        else             //If free roaming, then the movement is based on the camera forward direction.
        {
            m_MovementDirection = m_FreeLookCamera.transform.forward * vertical + m_FreeLookCamera.transform.right * horizontal;
        }
        m_MovementDirection.Normalize();

        if (m_MovementDirection != Vector3.zero) //Handle turn direction                          
        {
            m_TurnRotation = Quaternion.LookRotation(m_MovementDirection, Vector3.up); //As long as there is movement, we want to set m_CharacterToTurnRotation to the direction of m_MovementDirection.
        }

        if (Input.GetKeyDown(KeyCode.Mouse2)) //Handle locking on target.
        {
            LockOnEnemy();
        }

        if (!m_CombatScript.m_IsStunned && !m_DisableMovement) //Handle Movement.
        {
            Movement(horizontal, vertical);
        }

    }
    struct CinemachineRig  //Custom container for two floats.
    {
        public CinemachineRig(float height, float radius)
        {
            Height = height;
            Radius = radius;
        }
        public float Height;
        public float Radius;
    }
    public float GetAxisCustom(string axisName) //Custom Axis reading function that is passed to the cinemachine delegate (function pointer). If the player has locked onto a target then stop getting input from mouse / joystick.
    {
        if (!m_LockTarget)
        {
            if (axisName == "Mouse X")
                return Input.GetAxis("Mouse X");
            else if (axisName == "Mouse Y")
                return Input.GetAxis("Mouse Y");
        }
        else
            return 0;
        return 0;
    }
    protected override void Movement(float horizontal = 0.0f, float vertical = 0.0f)
    {
        if (m_LockTarget) //Locked onto a target and fighting.
        {
            m_Animator.SetBool("isLockedOn", true);
            transform.LookAt(new Vector3(m_LockTarget.transform.position.x, transform.position.y, m_LockTarget.transform.position.z));

            //LockedOnMovement(horizontal, vertical);
            if (m_MovementDirection != Vector3.zero) //Characater is moving.
            {
                float movementSpeed = m_LockedOnMovementSpeed;
                int generalAngleOfMovement = (int)Vector3.SignedAngle(transform.forward, m_MovementDirection, Vector3.up);

                m_Animator.SetBool("isMoving", true);
                m_Animator.SetFloat("PosX", horizontal, 1.0f, Time.deltaTime * 20.0f);
                m_Animator.SetFloat("PosY", vertical, 1.0f, Time.deltaTime * 20.0f);

                if (generalAngleOfMovement >= 134 && generalAngleOfMovement <= 180 || generalAngleOfMovement <= -134 && generalAngleOfMovement > -180)
                {
                    movementSpeed = m_RetreatMovementSpeed; //Decrease speed to discourage running away while fighting.
                }
                m_MovementDirection = new Vector3(m_MovementDirection.x, 0, m_MovementDirection.z); //Zero out y.
                transform.position += movementSpeed * Time.deltaTime * m_MovementDirection.normalized;
            }
            else // If the movement vector is zero, meaning the character is not moving.
            {
                m_Animator.SetBool("isMoving", false);
            }
        }
        else //Free roaming
        {
            SetCameraCenteringTo(false);
            m_Animator.SetBool("isLockedOn", false);
            if (m_MovementDirection != Vector3.zero)
            {
                m_Animator.SetBool("isRunning", true);
                if (new Vector3(horizontal, 0, vertical) == Vector3.forward) //Check if the player is moving straight forward.
                {
                    RotateCharacterTowards(m_FreeLookCamera.transform.rotation);
                }
                else
                {
                    RotateCharacterTowards(m_TurnRotation);
                }
                transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0); // Zero out x, z rotations. So that the character doesn't start to float. We only want to rotate in Y axis. (THIS IS A BUG FIX)
                transform.position += m_MovementSpeed * Time.deltaTime * new Vector3(transform.forward.x, 0, transform.forward.z);
            }
            else //Standing still
            {
                m_Animator.SetBool("isRunning", false);
            }
        }
    }
    void LockOnEnemy()
    {
        if (m_LockTarget) //If there is already a lock target, release it.
        {
            SetCenteringValues(1.5f, 2.0f, 0);
            SetCameraPosition(new CinemachineRig(3.0f, 5.0f), new CinemachineRig(1.5f, 6.0f), new CinemachineRig(0.1f, 5.0f));
            SetTrackedTargetOffset(1.2f);
            m_LockTarget = null;
            m_FreeLookCamera.LookAt = m_FreeLookCamera.Follow;
            return;
        }
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("EnemyAI");
        foreach (GameObject enemy in allEnemies)
        {
            if (Vector3.Distance(transform.position, enemy.transform.position) < 10.0f)
            {
                m_LockTarget = enemy;
                m_FreeLookCamera.LookAt = m_LockTarget.transform;
                SetCameraCenteringTo(true);
                SetTrackedTargetOffset(0.5f);
                SetCenteringValues(0.0f, 0.0f, -35);
                SetCameraPosition(new CinemachineRig(2.0f, 4.0f), new CinemachineRig(2.0f, 4.0f), new CinemachineRig(2.0f, 4.0f));
            }
        }
    }
    void SetCameraPosition(CinemachineRig top, CinemachineRig middle, CinemachineRig bottom)
    {
        m_FreeLookCamera.m_Orbits[0].m_Height = top.Height;
        m_FreeLookCamera.m_Orbits[0].m_Radius = top.Radius;
        m_FreeLookCamera.m_Orbits[1].m_Height = middle.Height;
        m_FreeLookCamera.m_Orbits[1].m_Radius = middle.Radius;
        m_FreeLookCamera.m_Orbits[2].m_Height = bottom.Height;
        m_FreeLookCamera.m_Orbits[2].m_Radius = bottom.Radius;
    }
    void SetCenteringValues(float waitTime, float recenterTime, float bias)
    {
        m_FreeLookCamera.m_Heading.m_Bias = bias;
        m_FreeLookCamera.m_RecenterToTargetHeading.m_WaitTime = waitTime;
        m_FreeLookCamera.m_RecenterToTargetHeading.m_RecenteringTime = recenterTime;
    }
    void SetTrackedTargetOffset(float offset)
    {
        m_FreeLookCamera.GetRig(0).GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset.y = offset;
        m_FreeLookCamera.GetRig(1).GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset.y = offset;
        m_FreeLookCamera.GetRig(2).GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset.y = offset;
    }
    void CameraBindingMode(CinemachineTransposer.BindingMode bindingMode)
    {
        m_FreeLookCamera.m_BindingMode = bindingMode;
    }
    void SetCameraCenteringTo(bool isActive)
    {
        m_FreeLookCamera.m_RecenterToTargetHeading.m_enabled = isActive;
    }
    void RotateCharacterTowards(Quaternion targetRotation)
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, m_PlayerRotationSpeedInEulerAngles * Time.deltaTime);
    }
}
