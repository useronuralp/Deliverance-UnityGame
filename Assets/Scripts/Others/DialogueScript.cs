using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DialogueScript : MonoBehaviour
{
    [TextArea(3,10)]
    public string[]         m_Sentences;                  //Sentences that the rescuer will say to player.
    private Queue<string>   m_DialogueQueue;              //The queue that I use to queue up sentences to be said by the rescuer.
    private TextMeshProUGUI m_TalkerName;                 // NPC name.
    private TextMeshProUGUI m_DialogueBorder;             // The dialogue border image.
    private SpriteRenderer  m_FloatingQuestionMark;       // Question mark icon that pops over the head of the NPC character when events occur.
    public Image            m_DialogueBox;                // The entire dialogue box object.
    private bool            m_IsPlayerInsideTalkingRange; // Flag to check if the player is in the range to talk.
    private void Start()
    {
        m_TalkerName                 = m_DialogueBox.transform.Find("NameFrame").Find("Name").GetComponent<TextMeshProUGUI>();
        m_DialogueBorder             = m_DialogueBox.transform.Find("Dialogue").GetComponent<TextMeshProUGUI>();
        m_FloatingQuestionMark       = transform.root.Find("FloatingQuestionMark").GetComponent<SpriteRenderer>();
        m_IsPlayerInsideTalkingRange = false;
        m_DialogueQueue              = new Queue<string>();
    }
    private void Update()
    {
        if(m_IsPlayerInsideTalkingRange) //Listen for input if the player is in talking range.
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                ProgressDialogue();
            }
        }
    }

    void StartDialogue() //Start the dialogue for the first time when the player presses 'E'.
    {
        m_DialogueQueue.Clear();
        m_TalkerName.text = "Old Man";
        m_FloatingQuestionMark.enabled = false;
        m_DialogueBox.gameObject.SetActive(true);
        foreach (string sentence in m_Sentences)
        {
            m_DialogueQueue.Enqueue(sentence);
        }
        DisplayNextSentence();
    }

    public void DisplayNextSentence() //Displays the next sentence in the queue onto the screen.
    {
        if(m_DialogueQueue.Count == 0)
        {
            EventManager.GetInstance().PlayerCompletedDialogue();
            Destroy(m_FloatingQuestionMark.gameObject);
            Destroy(gameObject);
            return;
        }
        string sentence = m_DialogueQueue.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }
    private void ProgressDialogue() //Wrapper fot the two functions above.
    {
        if (!m_DialogueBox.IsActive())
        {
            EventManager.GetInstance().PlayerStartsSpeakingWithRescuer();
            StartDialogue();
        }
        else
        {
            DisplayNextSentence();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            m_IsPlayerInsideTalkingRange = true;
            EventManager.GetInstance().PlayerStepsIntoTalkRange();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            m_IsPlayerInsideTalkingRange = false;
            m_FloatingQuestionMark.enabled = true;
            EventManager.GetInstance().PlayerLeftTalkRange();
        }
    }
    IEnumerator TypeSentence(string sentence) //Animating the letters with this.
    {
        m_DialogueBorder.text = "";
        foreach(char letter in sentence.ToCharArray())
        {
            m_DialogueBorder.text += letter;
            yield return null;
        }
    }
}
