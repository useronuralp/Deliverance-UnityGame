using UnityEngine;
//This class shows up the "Press CTRL to sprint" dialogue box.
public class SprintPrompt : MonoBehaviour
{
    private bool m_PlayerSawPrompt;
    private void Awake()
    {
        if(GameState.WasTutorialAlreadyTriggered)
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        m_PlayerSawPrompt = false;
        EventManager.GetInstance().OnPlayerEnteredSprintTrigger += OnPlayerEnteredSprintTrigger;
    }

    void Update()
    {
        if(m_PlayerSawPrompt)
        {
            if(Input.GetKeyDown(KeyCode.LeftControl))
            {
                Destroy(gameObject);
            }
        }
    }
    void OnPlayerEnteredSprintTrigger()
    {
        m_PlayerSawPrompt = true;
        transform.Find("Prompt").gameObject.SetActive(true);
    }
}
