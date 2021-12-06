using UnityEngine;

//Attached to the end game trigger collider after the final enemy. When player enters the collider game ends.
public class EndGameTrigger : MonoBehaviour
{
    private bool      m_HasPlayerFinishedGame = false;
    public GameObject EndGamePanel;
    void Update()
    {
        if(m_HasPlayerFinishedGame)
        {
            if(Input.GetKeyDown(KeyCode.E)) //Listen for inputs after player enters the trigger. When he continues to the next screen, reset everything to a fresh game and load the title screen.
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
        GameState.DidPlayerFinishTheGame = true; //Global variable.
        m_HasPlayerFinishedGame = true;          //Local variable.
        GameState.WasTutorialAlreadyTriggered = false; 
        Time.timeScale = 0;
        EndGamePanel.SetActive(true);
    }
}
