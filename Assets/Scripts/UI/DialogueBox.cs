using UnityEngine;

//Attached to the dialogue box that pops when player speaks with the Rescuer.
public class DialogueBox : MonoBehaviour
{
    private void Start()
    {
        EventManager.GetInstance().OnPlayerLeavingTalkRange += OnPlayerLeavingRange;
        EventManager.GetInstance().OnPlayerCompletingDialogue += OnPlayerCompletingDialogue;
    }
    void OnPlayerLeavingRange()
    {
        gameObject.SetActive(false);
    }
    void OnPlayerCompletingDialogue()
    {
        Destroy(gameObject);
    }
}
