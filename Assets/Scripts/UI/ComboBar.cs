using UnityEngine;
using UnityEngine.UI;
public class ComboBar : MonoBehaviour
{
    private CombatBehaviour  m_PlayerCombatScript;
    public  Image            m_ComboBarImage; 
    public  Image            m_BackgroundImage;
    void Start()
    {
        m_PlayerCombatScript = transform.root.GetComponent<CombatBehaviour>();
    }
    void Update()
    {
        if(m_PlayerCombatScript.m_StateElapesedTime >= m_PlayerCombatScript.m_ComboWindowStart + m_PlayerCombatScript.m_ComboWindowDuration) //If the combo window is passed, grey out the image a little bit.
        {
            m_ComboBarImage.color = new Color32(79, 79, 79, 179);
        }
        else
        {
            m_ComboBarImage.color = new Color32(255, 255, 255, 179); //If the bar is filling or at the beginning, make its color original color.
        }
        if(m_ComboBarImage.fillAmount == 0.0f) //The case where the bar starts to fill again.
        {
            m_ComboBarImage.color = new Color32(255, 255, 255, 179);
            m_BackgroundImage.enabled = false;
        }
        else
        {
            m_BackgroundImage.enabled = true;
        }
        //Fill the bar if one of the following conditions is satisfied. (Character attacking or parrying.)
        if((m_PlayerCombatScript.m_IsAttacking && !m_PlayerCombatScript.m_LockAttacking) || (m_PlayerCombatScript.m_IsInParryingAnimation && !m_PlayerCombatScript.m_LockAttacking))
        {
            m_ComboBarImage.fillAmount = m_PlayerCombatScript.m_StateElapesedTime / (m_PlayerCombatScript.m_ComboWindowStart);
        }
        else
        {
             m_ComboBarImage.fillAmount = 0.0f;
        }
    }
}
