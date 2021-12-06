using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//Called when the main menu is pulled up.
public class MainMenu : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(transform.root.gameObject);
    }
    public GameObject MainMenuPanel;
    public void ShowUpMainMenu()
    {
        MainMenuPanel.SetActive(true);
    }
    public void LoadGameScene()
    {
        SceneManager.LoadScene(1);
        GetComponent<Animator>().SetTrigger("FadeInShort");
    }
    public void OnPlayButtonPressed()
    {
        MainMenuPanel.SetActive(false);
        GetComponent<Animator>().SetTrigger("FadeOut");
        EventManager.GetInstance().PlayButtonPressed();
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
