using UnityEngine;
using TMPro;
public class PlayerCombatBehaviour : CombatBehaviour
{
    private bool m_DisableAllInput;
    private GameObject m_LockTarget;
    private TextMeshProUGUI m_ComboText;
    protected override void Awake()
    {
        base.Awake();
    }
    private void Start()
    {
        m_ComboCount = 0;
        m_LockTarget = null;
        m_DisableAllInput = false;
        EventManager.GetInstance().OnPlayerLockedOnToTarget += OnLockOnTarget; //Subscribe to event.
        EventManager.GetInstance().OnPlayerReleasedLockOnTarget += OnReleaseLockOnTarget; //Subscribe to event.
        m_ComboText = transform.Find("PlayerHUD").Find("ComboUI").Find("ComboText").Find("ComboNumber").GetComponent<TextMeshProUGUI>();
    }
    void Update()
    {
        m_ComboText.text = m_ComboCount.ToString();
        if(!PauseMenu.IsGamePaused)
        {
            if (m_Animator.GetBool("isDead")) //If player dies disable the script right away.
            {
                enabled = false;
            }
            if (m_IsStunned)
            {
                m_StunTimer -= Time.deltaTime;
                if(m_StunTimer <= 0)
                {
                    m_Animator.SetBool("isStunned", false);
                    m_StunTimer = m_StunDuration;
                    m_IsStunned = false;
                }
            }
            else if(!m_IsStunned)
            {
                if(!m_DisableAllInput)
                {
                    ProcessAttackInput();
                    if (!m_IsAttacking && !m_IsGettingHit && !m_IsInParryingAnimation && !m_IsStunned)
                    {
                        if (Input.GetKey(KeyCode.Space))
                        {
                            GuardUp(); //Parent class function.
                        }
                    }
                }
            }
            if(Input.GetKeyUp(KeyCode.Space))
            {
                GuardReleased(); //Parent class function.
            }
        }
    }
    private void ProcessAttackInput() //Standart input processing for player.
    {
        if (Input.GetKey(KeyCode.LeftShift) && m_LockTarget != null)
        {
            Parry();
        }
        if ((Input.GetKey(KeyCode.W) || Input.GetKeyDown(KeyCode.W)) && Input.GetKeyDown(KeyCode.Mouse1))
        {
            ThrowAttack(m_NormalStance.UpKick, m_Kicks[m_NormalStance.UpKick.Head], m_LockTarget);
        }
        else if ((Input.GetKey(KeyCode.A) || Input.GetKeyDown(KeyCode.A)) && Input.GetKeyDown(KeyCode.Mouse1))
        {
            ThrowAttack(m_NormalStance.LeftKick, m_Kicks[m_NormalStance.LeftKick.Head], m_LockTarget);
        }
        else if ((Input.GetKey(KeyCode.S) || Input.GetKeyDown(KeyCode.S)) && Input.GetKeyDown(KeyCode.Mouse1))
        {
            ThrowAttack(m_NormalStance.DownKick, m_Kicks[m_NormalStance.DownKick.Head], m_LockTarget);
        }
        else if((Input.GetKey(KeyCode.D) || Input.GetKeyDown(KeyCode.D)) && Input.GetKeyDown(KeyCode.Mouse1))
        {
            ThrowAttack(m_NormalStance.RightKick, m_Kicks[m_NormalStance.RightKick.Head], m_LockTarget);
        }
        else if ((Input.GetKey(KeyCode.W) || Input.GetKeyDown(KeyCode.W)) && Input.GetKeyDown(KeyCode.Mouse0))
        {
            ThrowAttack(m_NormalStance.UpPunch, m_Punches[m_NormalStance.UpPunch.Head], m_LockTarget);
        }
        else if ((Input.GetKey(KeyCode.D) || Input.GetKeyDown(KeyCode.D)) && Input.GetKeyDown(KeyCode.Mouse0))
        {
            ThrowAttack(m_NormalStance.RightPunch, m_Punches[m_NormalStance.RightPunch.Head], m_LockTarget);
        }
        else if ((Input.GetKey(KeyCode.A) || Input.GetKeyDown(KeyCode.A)) && Input.GetKeyDown(KeyCode.Mouse0))
        {
            ThrowAttack(m_NormalStance.LeftPunch, m_Punches[m_NormalStance.LeftPunch.Head], m_LockTarget);
        }
    }
    private void OnLockOnTarget(GameObject lockTarget)
    {
        m_LockTarget = lockTarget;
    }
    private void OnReleaseLockOnTarget()
    {
        m_LockTarget = null;
    }
}
