using System.Collections.Generic;
using UnityEngine;

// This SO will be created to store a list of NPCScheduleEvent's for a given NPC (we'll create one per NPC). Each NPCScheduleEvent in the list
// Stores the time/weather/priority for the event to happen, and the facing direction/animation to play at destination
// This list will be sorted by time and priority in the NPCSchedule class, so the first elements will be the soonest times,
// while ties will be broken up by priority
[CreateAssetMenu(fileName = "so_NPCScheduleEventList", menuName = "Scriptable Objects/NPC/NPC Schedule Event List")]
public class SO_NPCScheduleEventList : ScriptableObject
{
    [SerializeField] public List<NPCScheduleEvent> npcScheduleEventList;
}
