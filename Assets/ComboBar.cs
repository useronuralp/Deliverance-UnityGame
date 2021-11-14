using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ComboBar : MonoBehaviour
{
    private CombatBehaviour m_PlayerCombatScript;
    public Image m_ComboBarImage;
    private float m_ComboWindowDuration;
    // Start is called before the first frame update
    void Start()
    {
        m_PlayerCombatScript = transform.root.GetComponent<CombatBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(m_PlayerCombatScript.m_ComboWindowStart);
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
