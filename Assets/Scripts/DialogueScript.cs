using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DialogueScript : MonoBehaviour
{
    [TextArea(3,10)]
    public string[] m_Sentences;
    private Queue<string> m_DialogueQueue;
    private TextMeshProUGUI m_TalkerName;
    private TextMeshProUGUI m_Dialogue;
    private SpriteRenderer m_FloatingQuestionMark;
    public Image m_DialogueBox;
    private bool m_IsPlayerInsideTalkingRange;
    private void Start()
    {
        m_TalkerName = m_DialogueBox.transform.Find("NameFrame").Find("Name").GetComponent<TextMeshProUGUI>();
        m_Dialogue = m_DialogueBox.transform.Find("Dialogue").GetComponent<TextMeshProUGUI>();
        m_FloatingQuestionMark = transform.root.Find("FloatingQuestionMark").GetComponent<SpriteRenderer>();
        m_IsPlayerInsideTalkingRange = false;
        m_DialogueQueue = new Queue<string>();
    }
    private void Update()
    {
        if(m_IsPlayerInsideTalkingRange)
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                ProgressDialogue();
            }
        }
    }

    void StartDialogue()
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

    public void DisplayNextSentence()
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
        //m_Dialogue.text = sentence;
    }
    private void ProgressDialogue()
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
    IEnumerator TypeSentence(string sentence)
    {
        m_Dialogue.text = "";
        foreach(char letter in sentence.ToCharArray())
        {
            m_Dialogue.text += letter;
            yield return null;
        }
    }
}
