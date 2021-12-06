using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;
/// <summary>
/// Camera movements are also handled in this script along with player movement.
/// </summary>

//This class handles all the camera work and player movement. The camera follows the player movement and it seemed like a good idea to handle
//camera work here along with player movement because all the necessary variables for the camera are here.
//There are two cameras for player in the game. One is for the lock on fight while the other one is for free roaming.
//If player starts fighting, Cinemachine will transition to the fight camera and vice versa if player leaves the combat.
public class PlayerMovementBehaviour : MovementBehaviour
{
    private readonly float      m_PlayerRotationSpeedInEulerAngles = 500.0f; //Rotation speed of the player.
    private HealthStamina       m_HealthStaminaScript;
    public  CinemachineFreeLook m_FreeLookCamera; //Free look camera that is attached to player character.
    public  CinemachineFreeLook m_LockedOnCamera; //Free look camera that is attached to player character.
    protected override void Awake()
    {
        base.Awake();
        if(GameState.WasTutorialAlreadyTriggered && SceneManager.GetActiveScene().buildIndex == 1) //Check if the tutorial was triggered by the time this script is activated. (Meaning player restarted the game via menu)
        {
            OnEnableLeafParticles(); //Enable particles because player spawns outside.
            gameObject.transform.position = new Vector3(-18.5f, 6.03f, 21.2f); //Move the player to the outside, before the entrance.
        }
    }
    void Start()
    {
        CinemachineCore.GetInputAxis = GetAxisCustom;  //Pass Cinemachine delegate my own custom axis reading function.

        //Setting the default values for the free look camera.
        SetTrackedTargetOffset(m_FreeLookCamera, 1.2f);
        SetCenteringValues(m_FreeLookCamera, 3.0f, 5.0f, 0);
        SetCameraPosition(m_FreeLookCamera, new CinemachineRig(3.0f, 4.0f), new CinemachineRig(1.5f, 5.0f), new CinemachineRig(0.1f, 4.0f));
        IsCameraCenteringActive(m_FreeLookCamera, false);
        CameraBindingMode(m_FreeLookCamera, CinemachineTransposer.BindingMode.WorldSpace);
        m_FreeLookCamera.Follow = transform;
        m_FreeLookCamera.LookAt = transform;
        //Default values for the fight / locked on camera.
        SetTrackedTargetOffset(m_LockedOnCamera, 0.5f);
        SetCenteringValues(m_LockedOnCamera, 0.0f, 0.0f, -35);
        SetCameraPosition(m_LockedOnCamera, new CinemachineRig(2.0f, 4.0f), new CinemachineRig(2.0f, 4.0f), new CinemachineRig(2.0f, 4.0f));
        IsCameraCenteringActive(m_LockedOnCamera, true);
        CameraBindingMode(m_LockedOnCamera, CinemachineTransposer.BindingMode.WorldSpace);
        m_LockedOnCamera.Follow = transform;

        //Subscriptions.
        EventManager.GetInstance().OnLockTargetDeath += OnLockTargetDeath;
        EventManager.GetInstance().OnPlayerEnablesLeaves += OnEnableLeafParticles;
        EventManager.GetInstance().OnPlayerDisablesLeaves += OnDisableLeafParticles;

        m_HealthStaminaScript = GetComponent<HealthStamina>();
        m_RigidBody = GetComponent<Rigidbody>();
    }
    void Update()
    {
        if(!PauseMenu.IsGamePaused)
        {
            if (m_Animator.GetBool("isDead")) //Handling death on top before everything else.
            {
                m_FreeLookCamera.LookAt = transform;
                enabled = false;
            }
            if (Input.GetKeyDown(KeyCode.Mouse2)) //Handle locking on target.
            {
                LockOnEnemy();
            }
        }
    }
    private void FixedUpdate()
    {
        if(!PauseMenu.IsGamePaused)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            if (m_LockTarget) //If locked, movement is based on where the character is facing.
            {
                m_MovementDirection = transform.forward * vertical + transform.right * horizontal;
            }
            else //If free roaming, then the movement is based on the camera forward direction.
            {
                m_MovementDirection = m_FreeLookCamera.transform.forward * vertical + m_FreeLookCamera.transform.right * horizontal;
            }
            m_MovementDirection.Normalize();

            if (!m_DisableMovement) //Handle Movement.
            {
                Movement(horizontal, vertical);
            }
        }
    }

    protected override void Movement(float horizontal = 0.0f, float vertical = 0.0f)
    {
        if (m_LockTarget) //Locked onto a target and fighting.
        {
            IsCameraCenteringActive(m_FreeLookCamera, true);
            SetCenteringValues(m_FreeLookCamera, 0, 0, -35);
            m_Animator.SetBool("isLockedOn", true);


            TurnCharacterTowards(m_LockTarget, m_PlayerRotationSpeedInEulerAngles);

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
                m_RigidBody.MovePosition(transform.position + movementSpeed * Time.deltaTime * m_MovementDirection.normalized);
            }
            else // If the movement vector is zero, meaning the character is not moving.
            {
                m_Animator.SetBool("isMoving", false);
            }
        }
        else //Free roaming
        {
            IsCameraCenteringActive(m_FreeLookCamera, false);
            SetCenteringValues(m_FreeLookCamera, 3, 5, 0);
            m_Animator.SetBool("isLockedOn", false);
            if (m_MovementDirection != Vector3.zero)
            {
                m_Animator.SetBool("isRunning", true);
                if (new Vector3(horizontal, 0, vertical) == Vector3.forward) //Check if the player is moving straight forward.
                {
                    Vector3 turnRotationVector = m_FreeLookCamera.transform.forward; 
                    turnRotationVector = new Vector3(turnRotationVector.x, 0, turnRotationVector.z);
                    TurnCharacterTowards(turnRotationVector, 700.0f);
                }
                else
                {
                    Vector3 turnRotationVector = m_FreeLookCamera.transform.forward * vertical + m_FreeLookCamera.transform.right * horizontal;
                    turnRotationVector = new Vector3(turnRotationVector.x, 0, turnRotationVector.z);
                    TurnCharacterTowards(turnRotationVector, 700.0f);
                }
                if(Input.GetKey(KeyCode.LeftControl) && m_HealthStaminaScript.m_CurrentStamina >= 0.1f)
                {
                    m_Animator.SetBool("isSprinting", true);
                    m_HealthStaminaScript.ReduceStaminaSprint(0.04f);
                    m_RigidBody.MovePosition(transform.position + m_MovementSpeed * 1.7f * Time.deltaTime * transform.forward);
                }
                else
                {
                    m_Animator.SetBool("isSprinting", false);
                    m_RigidBody.MovePosition(transform.position + m_MovementSpeed * Time.deltaTime * transform.forward);
                }
            }
            else //Standing still
            {
                m_Animator.SetBool("isRunning", false);
                m_Animator.SetBool("isSprinting", false);
            }
        }
    }
    void LockOnEnemy()
    {
        if (m_LockTarget) //If there is already a lock target, release it.
        {
            m_FreeLookCamera.Priority = 1;
            m_LockedOnCamera.Priority = 0;
            m_LockTarget = null;
            m_LockedOnCamera.LookAt = transform;
            EventManager.GetInstance().PlayerReleasedLockOnTarget();
            return;
        }
        else
        {
            m_LockTarget = AcquireTarget();
        }
    }
    GameObject AcquireTarget() //If there is a target within 10.0f meter radius of the player, this function acquires that target like Dark Souls games.
    {
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("EnemyAI");
        float closestDistance = float.MaxValue;
        GameObject lockTarget = null;
        foreach (GameObject enemy in allEnemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < 10.0f)
            {
                if(distance < closestDistance)
                {
                    closestDistance = distance;
                    lockTarget = enemy;
                }
            }
        }
        if(lockTarget)
        {
            m_LockTarget = lockTarget;
            m_LockedOnCamera.LookAt = m_LockTarget.transform;
            //By changing the priorities of Cinemachine cameras like this, I can transition between them. Cinemachine handles the transition.
            m_FreeLookCamera.Priority = 0; 
            m_LockedOnCamera.Priority = 1;
            EventManager.GetInstance().PlayerLockedOnToTarget(lockTarget);
            return lockTarget;
        }
        else
        {
            return null;
        }
    }
    struct CinemachineRig  //Custom container for two floats that is used to pass to Cinemachine.
    {
        public CinemachineRig(float height, float radius)
        {
            Height = height;
            Radius = radius;
        }
        public float Height;
        public float Radius;
    }
    public float GetAxisCustom(string axisName) //Custom Axis reading function that is passed to the cinemachine delegate. This function does ---> If the player has locked onto a target then stop getting input from mouse / joystick.
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
    void SetCameraPosition(CinemachineFreeLook camera, CinemachineRig top, CinemachineRig middle, CinemachineRig bottom) //Helper to set variables.
    {
        camera.m_Orbits[0].m_Height = top.Height;
        camera.m_Orbits[0].m_Radius = top.Radius;
        camera.m_Orbits[1].m_Height = middle.Height;
        camera.m_Orbits[1].m_Radius = middle.Radius;
        camera.m_Orbits[2].m_Height = bottom.Height;
        camera.m_Orbits[2].m_Radius = bottom.Radius;
    }
    void SetCenteringValues(CinemachineFreeLook camera, float waitTime, float recenterTime, float bias) //Helper to set variables.
    {
        camera.m_Heading.m_Bias = bias;
        camera.m_RecenterToTargetHeading.m_WaitTime = waitTime;
        camera.m_RecenterToTargetHeading.m_RecenteringTime = recenterTime;
    }
    void SetTrackedTargetOffset(CinemachineFreeLook camera, float offset) //Helper to set variables.
    {
        camera.GetRig(0).GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset.y = offset;
        camera.GetRig(1).GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset.y = offset;
        camera.GetRig(2).GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset.y = offset;

        camera.GetRig(0).GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset.z = 0.0f;
        camera.GetRig(1).GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset.z = 0.0f;
        camera.GetRig(2).GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset.z = 0.0f;

        camera.GetRig(0).GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset.x = 0.0f;
        camera.GetRig(1).GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset.x = 0.0f;
        camera.GetRig(2).GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset.x = 0.0f;
    }
    void CameraBindingMode(CinemachineFreeLook camera, CinemachineTransposer.BindingMode bindingMode) //Helper to set variables.
    {
        camera.m_BindingMode = bindingMode;
    }
    void IsCameraCenteringActive(CinemachineFreeLook camera, bool isActive) //Helper to set variables.
    {
        camera.m_RecenterToTargetHeading.m_enabled = isActive;
    }
    void OnLockTargetDeath()
    {
        if(m_LockTarget)
        {
            m_FreeLookCamera.Priority = 1;
            m_LockedOnCamera.Priority = 0;
            m_LockTarget = null;
            m_LockedOnCamera.LookAt = transform;
            EventManager.GetInstance().PlayerReleasedLockOnTarget();
        }
    }
    void OnEnableLeafParticles()
    {
        transform.root.Find("LeafFall").gameObject.SetActive(true);
    }
    void OnDisableLeafParticles()
    {
        transform.root.Find("LeafFall").gameObject.SetActive(false);
    }
}
