using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public void TriggerLoad()
    {
        MainMenuPanel.SetActive(false);
        GetComponent<Animator>().SetTrigger("FadeOut");
    }
    public void OnQuitButtonPressed()
    {
        Application.Quit();
    }

}
