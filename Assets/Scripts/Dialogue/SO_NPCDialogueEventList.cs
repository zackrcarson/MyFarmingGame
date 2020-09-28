using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "so_NPCDialogueEventList", menuName = "Scriptable Objects/NPC/NPC Dialogue Event List")]
public class SO_NPCDialogueEventList : ScriptableObject
{
    [SerializeField] public List<NPCDialogueEvent> npcDialogueEventList;
}
