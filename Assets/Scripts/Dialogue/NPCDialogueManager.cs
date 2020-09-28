using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NPCDialogueManager : SingletonMonobehaviour<NPCDialogueManager>
{
    [SerializeField] public GameObject dialogueBox = null;

    private bool _dialogueBoxOn = false;
    public bool DialogueBoxOn { get => _dialogueBoxOn; set => _dialogueBoxOn = value; }

    [SerializeField] private TextMeshProUGUI dialogueTextSlot = null;
    [SerializeField] private TextMeshProUGUI npcNameSlot = null;
    [SerializeField] private Image npcPortraitSlot = null;
    [SerializeField] private Sprite transparentPortrait = null;

    protected override void Awake()
    {
        base.Awake();

        dialogueBox.SetActive(false);
    }


    public void FillDialogueBox(string dialogueText, string npcName, Sprite npcPortrait)
    {
        npcPortraitSlot.sprite = npcPortrait;
        dialogueTextSlot.text = dialogueText;
        npcNameSlot.text = npcName;
    }


    public void ClearDialogueBox()
    {
        npcPortraitSlot.sprite = transparentPortrait;
        dialogueTextSlot.text = "";
        npcNameSlot.text = "";
    }
}
