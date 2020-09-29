using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class NPCDialogue : MonoBehaviour
{
    [SerializeField] private SO_NPCDialogueEventList so_NPCDialogueEventList = null;
    //private Dictionary<string, NPCDialogueEvent> DialogueEventDictionary;
    private List<NPCDialogueEvent> DialogueEventList;
    private NPCDialogueEvent defaultDialogue;

    public string NPCName;

    private bool _isTalking = false;
    public bool IsTalking { get => _isTalking; set => _isTalking = value; }

    private NPCMovement npcMovement;

    [SerializeField] private Sprite NPCPortraitDefault = null;
    [SerializeField] private Sprite NPCPortraitHappy = null;
    [SerializeField] private Sprite NPCPortraitMad = null;
    [SerializeField] private Sprite transparentPortrait = null;

    private int dialogueNumber = 0;
    NPCDialogueEvent currentDialogue;

    private void Awake()
    {
        npcMovement = GetComponent<NPCMovement>();

        DialogueEventList = new List<NPCDialogueEvent>();
        if (so_NPCDialogueEventList.npcDialogueEventList.Count > 0)
        {
            foreach (NPCDialogueEvent npcDialogueEvent in so_NPCDialogueEventList.npcDialogueEventList)
            {
                if (npcDialogueEvent.dialogueDetail == "default")
                {
                    defaultDialogue = npcDialogueEvent;
                }
                else
                {
                    DialogueEventList.Add(npcDialogueEvent);
                }
            }
        }

    }


    private void Update()
    {
        if (npcMovement.npcCurrentScene.ToString() == SceneManager.GetActiveScene().name)
        {
            if (Vector2.Distance(gameObject.transform.position, Player.Instance.transform.position) <= Settings.NPCTalkDistance)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    if (NPCDialogueManager.Instance.DialogueBoxOn && IsTalking == true && dialogueNumber <= currentDialogue.dialogue.Length - 1)
                    {
                        DisplayNextText(currentDialogue, dialogueNumber);
                        dialogueNumber++;
                    }

                    else if (NPCDialogueManager.Instance.DialogueBoxOn)
                    {
                        IsTalking = false;
                        DisableDialogueBox();
                        NPCDialogueManager.Instance.DialogueBoxOn = false;
                        dialogueNumber = 0;
                        currentDialogue = new NPCDialogueEvent();

                        NPCDialogueManager.Instance.ClearDialogueBox();
                    }
                    else
                    {
                        IsTalking = true;
                        EnableDialogueBox();
                        NPCDialogueManager.Instance.DialogueBoxOn = true;

                        currentDialogue = FindDialogue();
                        
                        if (currentDialogue != null)
                        {
                            DisplayNextText(currentDialogue, dialogueNumber);
                            dialogueNumber++;
                        }
                        else
                        {
                            IsTalking = false;
                            DisableDialogueBox();
                            NPCDialogueManager.Instance.DialogueBoxOn = false;

                            NPCDialogueManager.Instance.ClearDialogueBox();
                        }
                    }
                }
            }
        }
    }


    public void EnableDialogueBox()
    {
        NPCDialogueManager.Instance.DialogueBoxOn = true;

        // disables the players movement input
        Player.Instance.PlayerInputIsDisabled = true;

        // Stops running all update methods! No more time is counted
        Time.timeScale = 0;

        NPCDialogueManager.Instance.dialogueBox.SetActive(true);

        // Trigger the garbage collector - might as well do it now while not much is happening
        System.GC.Collect();
    }


    public void DisableDialogueBox()
    {
        NPCDialogueManager.Instance.DialogueBoxOn = false;

        // Enables the players movement input
        Player.Instance.PlayerInputIsDisabled = false;

        // Starts running all update methods again! Time flows again
        Time.timeScale = 1;

        NPCDialogueManager.Instance.dialogueBox.SetActive(false);

        //ResetDialogueBox(0); 
    }


    private void DisplayNextText(NPCDialogueEvent currentDialogue, int dialogueNumber)
    {
        Sprite currentEmotionSprite;

        switch (currentDialogue.emotions[dialogueNumber])
        {
            case NPCEmotions.normal:
                currentEmotionSprite = NPCPortraitDefault;
                break;

            case NPCEmotions.happy:
                currentEmotionSprite = NPCPortraitHappy;
                break;

            case NPCEmotions.mad:
                currentEmotionSprite = NPCPortraitMad;
                break;

            default:
                currentEmotionSprite = transparentPortrait;
                break;
        }

        NPCDialogueManager.Instance.FillDialogueBox(currentDialogue.dialogue[dialogueNumber], NPCName, currentEmotionSprite);
    }


    private NPCDialogueEvent FindDialogue()
    {
        NPCDialogueEvent foundDialogue = new NPCDialogueEvent();
        int foundDialoguePriority = 999999;
        bool isDialogueFound = false;

        Enum.TryParse(TimeManager.Instance.GetDayOfWeek(), out DayOfWeek currentDay);
        Enum.TryParse(SceneManager.GetActiveScene().name, out SceneName currentScene);

        int currentHour = TimeManager.Instance.GetGameTime().Hours;
        Weather currentWeather = GameManager.Instance.currentWeather;
        Season currentSeason = TimeManager.Instance.GetGameSeason();
        int currentYear = TimeManager.Instance.GetGameYear();

        for (int i = 0; i < DialogueEventList.Count; i ++)
        {
            if (DialogueEventList[i].sceneNames.Contains(currentScene))
            {
                if (DialogueEventList[i].daysOfWeek.Contains(currentDay))
                {
                    if (DialogueEventList[i].weatherTypes.Contains(currentWeather))
                    {
                        if (DialogueEventList[i].seasons.Contains(currentSeason))
                        {
                            if (DialogueEventList[i].years.Count == 0 || DialogueEventList[i].years.Contains(currentYear))
                            {
                                if (currentHour >= DialogueEventList[i].hourMin && currentHour <= DialogueEventList[i].hourMax)
                                {
                                    if (DialogueEventList[i].priority < foundDialoguePriority)
                                    {
                                        foundDialogue = DialogueEventList[i];
                                        foundDialoguePriority = DialogueEventList[i].priority;
                                        isDialogueFound = true;
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }
            }
            else
            {
                continue;
            }
        }

        if (!isDialogueFound)
        {
            foundDialogue = defaultDialogue;
        }

        return foundDialogue;
    }
}
