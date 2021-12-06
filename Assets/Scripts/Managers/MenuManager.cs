using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//This class is unnecessary. TODO: Remove this later.
public class MenuManager : MonoBehaviour
{
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public static void RestartLevel(int level)
    {
        SceneManager.LoadScene(level);
        Time.timeScale = 1;
    }
}
