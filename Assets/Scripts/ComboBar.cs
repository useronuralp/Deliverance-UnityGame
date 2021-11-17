using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ComboBar : MonoBehaviour
{
    private CombatBehaviour m_PlayerCombatScript;
    public Image m_ComboBarImage;
    public Image m_BackgroundImage;
    // Start is called before the first frame update
    void Start()
    {
        m_PlayerCombatScript = transform.root.GetComponent<CombatBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        if(m_PlayerCombatScript.m_StateElapesedTime >= m_PlayerCombatScript.m_ComboWindowStart + m_PlayerCombatScript.m_ComboWindowDuration)
        {
            m_ComboBarImage.color = new Color32(79, 79, 79, 179);
        }
        if(m_ComboBarImage.fillAmount == 0.0f)
        {
            m_ComboBarImage.color = new Color32(255, 255, 255, 179);
            m_BackgroundImage.enabled = false;
        }
        else
        {
            m_BackgroundImage.enabled = true;
        }
        if(m_PlayerCombatScript.m_IsAttacking && !m_PlayerCombatScript.m_LockAttacking)
        {
            m_ComboBarImage.fillAmount = m_PlayerCombatScript.m_StateElapesedTime / (m_PlayerCombatScript.m_ComboWindowStart);
        }
        else
        {
             m_ComboBarImage.fillAmount = 0.0f;
        }
        //if (m_PlayerCombatScript.m_StateElapesedTime >= m_PlayerCombatScript.m_ComboWindowStart + m_PlayerCombatScript.m_ComboWindowDuration || m_PlayerCombatScript.m_StateElapesedTime == 0.0f)
        //{
        //}
    }
}
