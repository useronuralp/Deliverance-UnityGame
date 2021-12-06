using UnityEngine;

//Attached to the trigger collider that triggers the door lock inside the prison cell.
public class DoorTrigger : MonoBehaviour
{
    private bool m_IsPlayerUnlockRange; 
    private void Awake()
    {
        if(GameState.WasTutorialAlreadyTriggered) //Check if the player has already completed the tutorial, if so there is no need to keep this locked.
        {
            transform.root.Find("DoorLock").GetComponent<BoxCollider>().enabled = false;
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        m_IsPlayerUnlockRange = false;
        EventManager.GetInstance().OnPlayerCompletingDialogue += OnPlayerCompletingDialogue;
    }
    private void Update()
    {
        if(m_IsPlayerUnlockRange)
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                transform.root.Find("DoorLock").GetComponent<BoxCollider>().enabled = false;
                EventManager.GetInstance().PlayerLeftDoorUnlockRange();
                Destroy(gameObject);
            }
        }
    }
    void OnPlayerCompletingDialogue() //This trigger gets enabled only when the player completes the dialogue with the rescuer.
    {
        GetComponent<BoxCollider>().enabled = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        m_IsPlayerUnlockRange = true;
        EventManager.GetInstance().PlayerEnteredDoorUnlockRange();
    }
    private void OnTriggerExit(Collider other)
    {
        m_IsPlayerUnlockRange = false;
        EventManager.GetInstance().PlayerLeftDoorUnlockRange();
    }
}
