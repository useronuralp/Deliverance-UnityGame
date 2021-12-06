using UnityEngine;
using UnityEngine.UI;
public class PauseMenu : MonoBehaviour
{
    public static bool IsGamePaused = false;
    private GameObject m_PausePanel;
    private GameObject m_TutorialPanel;
    public GameObject  TutorialScreen;
    private bool       m_IsPlayerInTutorial;
    private bool       m_IsPlayerAtDeathScreen;
    private void Awake()
    {
        transform.Find("PausePanel").Find("Slider").GetComponent<Slider>().value = AudioListener.volume;
    }
    private void Start()
    {
        m_IsPlayerAtDeathScreen = false;
        m_IsPlayerInTutorial = false;
        m_PausePanel = transform.Find("PausePanel").gameObject;
        m_TutorialPanel = transform.Find("TutorialPanel").gameObject;
        EventManager.GetInstance().OnSeperateWindowClosed += OnSeperateWindowClosed;
        EventManager.GetInstance().OnPlayerClosesTutorial += OnPlayerClosesTutorial;
        EventManager.GetInstance().OnPlayerEnteringTutorialTrigger += OnPlayerEntersTutorial;
        EventManager.GetInstance().OnPlayerDeath += OnPlayerDeath;


        if(GameState.WasTutorialAlreadyTriggered)
            m_PausePanel.transform.Find("TutorialButton").gameObject.SetActive(true);
    }
    void Update()
    {
        if(!m_IsPlayerInTutorial && !m_IsPlayerAtDeathScreen && !GameState.DidPlayerFinishTheGame)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                if(IsGamePaused)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    m_PausePanel.SetActive(false);
                    m_TutorialPanel.SetActive(false);
                    Time.timeScale = 1;
                    IsGamePaused = false;
                    EventManager.GetInstance().GameResumed();
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Confined;
                    Cursor.visible = true;
                    m_PausePanel.SetActive(true);
                    Time.timeScale = 0;
                    IsGamePaused = true;
                }
            }
        }
    }
    public void OnTutorialButtonPressed()
    {
        m_TutorialPanel.SetActive(true);
        m_PausePanel.SetActive(false);
    }
    public void OnRestartButtonPressed()
    {
        Time.timeScale = 1;
        IsGamePaused = false;
        MenuManager.RestartLevel(1);
    }
    public void OnLockOnButtonPressed()
    {
        m_TutorialPanel.SetActive(false);
        EventManager.GetInstance().LockOnButtonPressed();
    }
    public void OnOffenseButtonPressed()
    {
        m_TutorialPanel.SetActive(false);
        EventManager.GetInstance().OffenseButtonPressed();
    }
    public void OnDefenseButtonPressed()
    {
        m_TutorialPanel.SetActive(false);
        EventManager.GetInstance().DefenseButtonPressed();
    }
    public void OnResourcesButtonPressed()
    {
        m_TutorialPanel.SetActive(false);
        EventManager.GetInstance().ResourcesButtonPressed();
    }
    public void OnComboButtonPressed()
    {
        m_TutorialPanel.SetActive(false);
        EventManager.GetInstance().ComboButtonPressed();
    }
    public void OnParryButtonPressed()
    {
        m_TutorialPanel.SetActive(false);
        EventManager.GetInstance().ParryButtonPressed();
    }
    public void OnBackButtonPressed()
    {
        m_TutorialPanel.SetActive(false);
        m_PausePanel.SetActive(true);
        Time.timeScale = 0;
    }
    public void OnSeperateWindowClosed()
    {
        m_TutorialPanel.SetActive(true);
    }
    public void OnPlayerClosesTutorial()
    {
        m_PausePanel.transform.Find("TutorialButton").gameObject.SetActive(true);
        m_IsPlayerInTutorial = false;
    }
    public void OnPlayerEntersTutorial()
    {
        m_IsPlayerInTutorial = true;
    }
    public void OnPlayerDeath()
    {
        m_IsPlayerAtDeathScreen = true;
    }
    public void OnQuitButtonPressed()
    {
        Application.Quit();
    }
    public void OnMasterVolumeChanged(float volume)
    {
        AudioListener.volume = volume;
    }
}
