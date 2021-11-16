using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RepositionSprite : MonoBehaviour
{
    private MovementBehaviour m_MovementScript;
    private Image m_Icon;
    private RectTransform m_RecTransform;
    private Vector3 m_StartingPosition;
    private void Awake()
    {
        m_MovementScript = gameObject.transform.root.GetComponent<MovementBehaviour>();
        m_Icon = GetComponent<Image>();
        m_RecTransform = transform.parent.GetComponent<RectTransform>();
        m_StartingPosition = transform.position;
    }
    void Update()
    {
        if(m_MovementScript.m_LockTarget)
        {
            m_Icon.enabled = true;
            float distance = Vector3.Distance(gameObject.transform.root.position, m_MovementScript.m_LockTarget.transform.position);
            transform.position = new Vector3(transform.position.x, m_StartingPosition.y + (Mathf.Abs((distance - 10) * 10)), transform.position.z);
        }
        else
        {
            m_Icon.enabled = false;
        }
    }
}
