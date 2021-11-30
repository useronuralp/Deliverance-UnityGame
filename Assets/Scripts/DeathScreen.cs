using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathScreen : MonoBehaviour
{
    private void Start()
    {
        EventManager.GetInstance().OnPlayerDeath += OnPlayerDies;
    }
    public void OnRestartCheckPointButtonPressed()
    {
        MenuManager.RestartLevel(1);
    }
    public void OnMainMenuButtonPressed()
    {
        MenuManager.RestartLevel(0);
    }
    void OnPlayerDies()
    {
        transform.Find("DeathScreenPanel").gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        Time.timeScale = 0;
    }
}
