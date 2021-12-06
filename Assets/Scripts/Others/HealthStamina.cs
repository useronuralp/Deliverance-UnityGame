using System.Collections;
using UnityEngine;
using UnityEngine.UI;

//This script is responsible for the health and stamina of characters.
public class HealthStamina : MonoBehaviour
{
    public float     m_MaxHealth                   = 100;
    public float     m_MaxStamina                  = 100;
    public float     m_CurrentHealth               = 100;
    public float     m_CurrentStamina              = 100;
    [HideInInspector]
    public float     m_UpdateSpeedSeconds          = 0.2f; //Used in coroutine to slowly animate health drops.
    [HideInInspector]
    public float     m_StaminaRechargeCooldown     = 2.0f;
    [HideInInspector]
    public float     m_StaminaBarDisappearTimer    = 2.0f;
    [HideInInspector]
    public float     m_StaminaBarDisappearCooldown = 2.0f;
    [HideInInspector]
    public float     m_StaminaRechargeTimer;


    public Image     m_HealthImage;
    public Image     m_HealthBackground;
    public Image     m_StaminaImage;
    public Image     m_StaminaBackground;
    private Animator m_Animator;

    private void Awake()
    {
        m_StaminaBarDisappearTimer = m_StaminaBarDisappearCooldown;
        m_StaminaRechargeTimer     = m_StaminaRechargeCooldown;
        m_Animator                 = GetComponent<Animator>();
    }
    private void Start()
    {
        m_HealthBackground.enabled = false;
        m_HealthImage.enabled = false;
        m_StaminaBackground.enabled = false;
        m_StaminaImage.enabled = false;
    }
    private void Update()
    {
        m_CurrentStamina = Mathf.Clamp(m_CurrentStamina, 0, 100);
        if (m_CurrentStamina < 100) //Start displaying the stamina bar if it is being used. 
        {
            m_StaminaBarDisappearTimer  = m_StaminaBarDisappearCooldown; //Fix the disappear timer.
            m_StaminaImage.enabled      = true;
            m_StaminaBackground.enabled = true;
            m_StaminaRechargeTimer     -= Time.deltaTime; //Also start this timer.
        }
        else if(m_CurrentStamina == 100) //If the stamina bar is at full capacity and has not been used for a certain duration, make it disappear.
        {
            m_StaminaRechargeTimer = m_StaminaRechargeCooldown; //Fix the recharge timer.
            m_StaminaBarDisappearTimer -= Time.deltaTime;
            if(m_StaminaBarDisappearTimer <= 0.0f)
            {
                m_StaminaBackground.enabled = false;
                m_StaminaImage.enabled = false;
            }
        }

        if(m_CurrentHealth < 100)
        {
            m_HealthBackground.enabled = true;
            m_HealthImage.enabled = true;
        }
        if(m_StaminaRechargeTimer <= 0) //If the timer is down, recharge the stamina bar.
        {
            if(m_Animator.GetBool("isGuarding"))
                RechargeStamina(15.0f);
            else
                RechargeStamina(30.0f);
        }
        if(m_CurrentHealth <= 0) //Death
        {
            m_Animator.SetTrigger("DeathHighRight");
            m_Animator.SetBool("isDead", true);
            transform.root.GetComponent<BoxCollider>().enabled = false;
            enabled = false;
        }
    }
    void RechargeStamina(float chargeRate)
    {
        m_CurrentStamina += chargeRate * Time.deltaTime; 
        m_StaminaImage.fillAmount = m_CurrentStamina / m_MaxStamina; //This fillAmount field is clamped between [0, 1]. Therefore, I am passing the current percentage of the stamina to it.
    }
    public void ReduceStamina(float amount)                          //Change the current stamina first, then pass the percentage to the coroutine.
    {
        m_StaminaRechargeTimer = m_StaminaRechargeCooldown;         //Reset the recharge timer every time stamina is reduced, meaning whenever the character throws an attack.
        m_CurrentStamina -= amount;
        StartCoroutine(ChangeStaminaTo(m_CurrentStamina / m_MaxStamina));
    }
    public void ReduceStaminaSprint(float amount)                   
    {
        m_StaminaRechargeTimer = 0.5f;         
        m_CurrentStamina -= amount;
        m_StaminaImage.fillAmount = m_CurrentStamina / m_MaxStamina;
    }
    public void GetHit(float damage)                                 //Change the current health first, then pass the percentage to the coroutine.
    {
        m_CurrentHealth -= damage;
        StartCoroutine(ChangeHealthTo(m_CurrentHealth / m_MaxHealth));
    }
    private IEnumerator ChangeHealthTo(float percentage)             //Only reason this is a Coroutine is that changing the health amount looks smooth this way, rather than sharp and instantenous decrease / increase in visuals.
    {
        float preChangePercentage = m_HealthImage.fillAmount;
        float elapsed = 0f;

        while(elapsed <  m_UpdateSpeedSeconds)
        {
            elapsed += Time.deltaTime;
            m_HealthImage.fillAmount = Mathf.Lerp(preChangePercentage, percentage, elapsed / m_UpdateSpeedSeconds);
            yield return null;
        }
        m_HealthImage.fillAmount = percentage;
    }
    private IEnumerator ChangeStaminaTo(float percentage)              //Only reason this is a Coroutine is because changing the stamina amount looks smooth this way, rather than sharp and instantenous decrease / increase in visuals.
    {
        float preChangePercentage = m_StaminaImage.fillAmount;
        float elapsed = 0f;

        while (elapsed < m_UpdateSpeedSeconds)
        {
            elapsed += Time.deltaTime;
            m_StaminaImage.fillAmount = Mathf.Lerp(preChangePercentage, percentage, elapsed / m_UpdateSpeedSeconds);
            yield return null;
        }
        m_StaminaImage.fillAmount = percentage;
    }
}
