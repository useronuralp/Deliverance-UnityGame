using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Tutorial : MonoBehaviour
{
    private GameObject m_Box1;
    private GameObject m_Box2;
    private GameObject m_Box3;
    private GameObject m_Box4;
    private GameObject m_Box5;
    private Queue<GameObject> m_Screens;
    private bool m_PlayerReadingTutorial;
    private GameObject m_PreviousScreen;
    private GameObject m_PressEToContinue;
    private GameObject m_ContinueImage;
    private bool m_OnSeperateMenu;
    private GameObject m_ActiveWindow;
    void Awake()
    {

        m_Box1 = transform.Find("TutorialBox1").gameObject;
        m_Box2 = transform.Find("TutorialBox2").gameObject;
        m_Box3 = transform.Find("TutorialBox3").gameObject;
        m_Box4 = transform.Find("TutorialBox4").gameObject;
        m_Box5 = transform.Find("TutorialBox5").gameObject;
        m_PressEToContinue = transform.Find("PressEToContinue").gameObject;
        m_ContinueImage = transform.Find("ContinueImage").gameObject;
        m_Screens = new Queue<GameObject>();
        m_PreviousScreen = null;
        m_PlayerReadingTutorial = false;
        m_Screens.Enqueue(m_Box1); m_Screens.Enqueue(m_Box2); m_Screens.Enqueue(m_Box3); m_Screens.Enqueue(m_Box4); m_Screens.Enqueue(m_Box5);
        m_PressEToContinue.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        LeanTween.scale(m_PressEToContinue.gameObject, new Vector3(1, 1, 1), 0.00001f).setLoopType(LeanTweenType.pingPong);
    }
    private void Start()
    {
        m_ActiveWindow = null;
        m_OnSeperateMenu = false;
        EventManager.GetInstance().OnPlayerEnteringTutorialTrigger += OnPlayerEnteredTutorialTrigger;
        EventManager.GetInstance().OnLockOnButtonPressed += OnLockOnButtonPressed;
        EventManager.GetInstance().OnOffenseButtonPressed += OnOffenseButtonPressed;
        EventManager.GetInstance().OnDefenseButtonPressed += OnDefenseButtonPressed;
        EventManager.GetInstance().OnResourcesButtonPressed += OnResourcesButtonPressed;
        EventManager.GetInstance().OnComboButtonPressed += OnComboButtonPressed;

        EventManager.GetInstance().OnGameResumed += OnGameResumed;
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
        if(m_OnSeperateMenu)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                m_OnSeperateMenu = false;
                m_ActiveWindow.SetActive(false);
                m_ActiveWindow = null;
                m_PressEToContinue.SetActive(false);
                m_ContinueImage.SetActive(false);
                EventManager.GetInstance().SeperateWindowClosed();
                Time.timeScale = 0;
            }
        }
    }
    void DisplayNextScreen()
    {
        if(m_Screens.Count == 0)
        {
            m_PlayerReadingTutorial = false;
            EventManager.GetInstance().PlayerClosedTutorial();
            m_PressEToContinue.SetActive(false);
            m_ContinueImage.SetActive(false);
            Time.timeScale = 1;
            m_PreviousScreen.SetActive(false);
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
        m_ContinueImage.SetActive(true);
        Time.timeScale = 0.00001f;
    }
    public void OnLockOnButtonPressed()
    {
        m_OnSeperateMenu = true;
        m_Box1.SetActive(true);
        m_ActiveWindow = m_Box1;
        m_PressEToContinue.SetActive(true);
        m_ContinueImage.SetActive(true);
        Time.timeScale = 0.00001f;
    }
    public void OnOffenseButtonPressed()
    {
        m_OnSeperateMenu = true;
        m_Box2.SetActive(true);
        m_ActiveWindow = m_Box2;
        m_PressEToContinue.SetActive(true);
        m_ContinueImage.SetActive(true);
        Time.timeScale = 0.00001f;
    }
    public void OnDefenseButtonPressed()
    {
        m_OnSeperateMenu = true;
        m_Box3.SetActive(true);
        m_ActiveWindow = m_Box3;
        m_PressEToContinue.SetActive(true);
        m_ContinueImage.SetActive(true);
        Time.timeScale = 0.00001f;
    }
    public void OnResourcesButtonPressed()
    {
        m_OnSeperateMenu = true;
        m_Box4.SetActive(true);
        m_ActiveWindow = m_Box4;
        m_PressEToContinue.SetActive(true);
        m_ContinueImage.SetActive(true);
        Time.timeScale = 0.00001f;
    }
    public void OnComboButtonPressed()
    {
        m_OnSeperateMenu = true;
        m_Box5.SetActive(true);
        m_ActiveWindow = m_Box5;
        m_PressEToContinue.SetActive(true);

        m_ContinueImage.SetActive(true);
        Time.timeScale = 0.00001f;
    }
    public void OnGameResumed()
    {
        m_OnSeperateMenu = false;
        if(m_ActiveWindow)
            m_ActiveWindow.SetActive(false);
        if(m_PressEToContinue)
            m_PressEToContinue.SetActive(false);
        if(m_ContinueImage)
            m_ContinueImage.SetActive(false);
    }
}
