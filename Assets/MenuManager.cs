using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static void RestartLevel(int level)
    {
        SceneManager.LoadScene(level);
    }
}
