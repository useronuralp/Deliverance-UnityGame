using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameTrigger : MonoBehaviour
{
    private bool m_HasPlayerFinishedGame = false;
    public GameObject EndGamePanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(m_HasPlayerFinishedGame)
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                GameState.DidPlayerFinishTheGame = false;
                MenuManager.RestartLevel(0);
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
                EndGamePanel.SetActive(false);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        GameState.DidPlayerFinishTheGame = true;
        m_HasPlayerFinishedGame = true;
        GameState.WasTutorialAlreadyTriggered = false;
        Time.timeScale = 0;
        EndGamePanel.SetActive(true);
    }
}
