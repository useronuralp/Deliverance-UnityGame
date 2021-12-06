using UnityEngine;
using TMPro;
public class ComboUI : MonoBehaviour
{
    private TextMeshProUGUI m_ComboText;
    private TextMeshProUGUI m_ComboNumber;
    private float           m_DisappearTimer; 
    private float           m_DisappearDuration = 0.5f; //The Combo UI will disappear after this time.
    void Start()
    {
        m_ComboText      = transform.Find("ComboText").GetComponent<TextMeshProUGUI>();
        m_DisappearTimer = 0;
        m_ComboNumber    = transform.Find("ComboText").Find("ComboNumber").GetComponent<TextMeshProUGUI>();
    }
    void Update()
    {
        if(m_ComboNumber.text == "0") //TODO: This string comparison is really bad. Change if you have time.
        {
            m_DisappearTimer -= Time.deltaTime;
        }
        else
        {
            m_DisappearTimer = m_DisappearDuration;
            m_ComboNumber.enabled = true;
            m_ComboText.enabled = true;
        }
        if(m_DisappearTimer <= 0.0f)
        {
            m_ComboNumber.enabled = false;
            m_ComboText.enabled = false;
        }
    }
}
