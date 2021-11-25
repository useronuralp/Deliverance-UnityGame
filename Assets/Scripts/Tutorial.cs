using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Tutorial : MonoBehaviour
{
    public GameObject m_Box1;
    public GameObject m_Box2;
    public GameObject m_Box3;
    private Queue<GameObject> m_Screens;
    private bool m_PlayerReadingTutorial;
    private GameObject m_PreviousScreen;
    private GameObject m_PressEToContinue;
    private GameObject m_ContinueImage;
    void Start()
    {
        m_PressEToContinue = transform.Find("PressEToContinue").gameObject;
        m_ContinueImage = transform.Find("ContinueImage").gameObject;
        m_Screens = new Queue<GameObject>();
        m_PreviousScreen = null;
        m_PlayerReadingTutorial = false;
        m_Screens.Enqueue(m_Box1); m_Screens.Enqueue(m_Box2); m_Screens.Enqueue(m_Box3);
        m_PressEToContinue.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        EventManager.GetInstance().OnPlayerEnteringTutorialTrigger += OnPlayerEnteredTutorialTrigger;
    }
    void Update()
    {
        if (m_PlayerReadingTutorial)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                DisplayNextScreen();
            }
        }
    }
    void DisplayNextScreen()
    {
        if(m_Screens.Count == 0)
        {
            EventManager.GetInstance().PlayerClosedTutorial();
            Time.timeScale = 1;
            Destroy(gameObject);
            return;
        }
        if(m_PreviousScreen)
        {
            m_PreviousScreen.SetActive(false);
        }
        GameObject screen = m_Screens.Dequeue();
        screen.gameObject.SetActive(true);
        m_PreviousScreen = screen;
    }
    void OnPlayerEnteredTutorialTrigger()
    {
        m_PlayerReadingTutorial = true;
        m_PreviousScreen = m_Screens.Dequeue();
        m_PreviousScreen.gameObject.SetActive(true);
        m_PressEToContinue.SetActive(true);
        LeanTween.scale(m_PressEToContinue.gameObject, new Vector3(1, 1, 1), 0.00001f).setLoopType(LeanTweenType.pingPong);
        m_ContinueImage.SetActive(true);
        Time.timeScale = 0.00001f;
    }
}
