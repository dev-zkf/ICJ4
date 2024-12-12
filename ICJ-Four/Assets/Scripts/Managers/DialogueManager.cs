using NaughtyAttributes;
using NaughtyAttributes.Test;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;


public class DialogueManager : MonoBehaviour
{
    [Scene] public int scene;
    
    public GameObject dialogueBox;
    public static DialogueManager Instance;
    public Dialogue dialogue;

    public TMP_Text NameText;
    public TMP_Text DialogueText;

    private Queue<string> sentences;

    [Tag] public string npcTag;


    void Start()
    {
        sentences = new Queue<string>();
        TriggerDialogue();   
    }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void TriggerDialogue()
    {
        StartDialogue(dialogue);
    }

    public void StartDialogue (Dialogue dialogue)
    {
        dialogueBox.SetActive(true);
        NameText.text = dialogue.name;

        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            dialogueBox.SetActive(false);
            EndDialogue();
            return;
        }
        string sentence = sentences.Dequeue();
        DialogueText.text = sentence;
    }

    public void EndDialogue()
    {
        SceneManager.LoadScene(scene);
    }
}
