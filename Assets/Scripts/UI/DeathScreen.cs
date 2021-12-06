using UnityEngine;

//Called when the player dies.
public class DeathScreen : MonoBehaviour
{
    private void Start()
    {
        EventManager.GetInstance().OnPlayerDeath += OnPlayerDies; //Subscribe to the event.
    }
    public void OnRestartCheckPointButtonPressed()
    {
        MenuManager.RestartLevel(1); //Load the game scene.
    }
    public void OnMainMenuButtonPressed()
    {
        //Load the main menu and set the game state accordingly.
        GameState.WasTutorialAlreadyTriggered = false;
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
