using UnityEngine;
//Fires the event to signal that player entered the tutorial trigger.
public class TutorialTrigger : MonoBehaviour
{
    private void Awake()
    {
        if (GameState.WasTutorialAlreadyTriggered)
            Destroy(gameObject);
    }
    private void Start()
    {
        EventManager.GetInstance().OnPlayerClosesTutorial += OnPlayerCompletesTutorial;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            GameState.WasTutorialAlreadyTriggered = true;
            EventManager.GetInstance().PlayerEnteredTutorialTrigger(); //Trigger tutorial text.
            EventManager.GetInstance().TriggerFightMusicFirstTime();   //Trigger a tutorial reading slow / non-distracting music for the first time.
            GetComponent<BoxCollider>().enabled = false;
        }
    }
    void OnPlayerCompletesTutorial()
    {
        Destroy(gameObject);
    }
}
