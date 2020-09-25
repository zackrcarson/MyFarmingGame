using System;
using System.Collections.Generic;
using UnityEngine;

// This class will take care of the scheduling and doling out of NPC Schedule Events for a given NPC. It will load up the SO storing a list of the scheduled movements we want the NPC
// to do at given times into a sorted set, and then whenever the gameMinute advances in the game, it will check the Set for a matching event, and then initiate the path building and NPC movement
[RequireComponent(typeof(NPCPath))]
public class NPCSchedule : MonoBehaviour
{
    // This is the SO storing our NPCScheduleEvents that we have setup for this NPC, the next one is the SortedSet that we will store all of the NPCScheduleEvent into, sorted by time and priority
    [SerializeField] private SO_NPCScheduleEventList so_NPCScheduleEventList = null;
    private SortedSet<NPCScheduleEvent> npcScheduleEventSet;

    private NPCPath npcPath;


    // Build our sorted NPCScheduleEvent set by time and priority from the so_NPCScheduleEventList containing all of the schedules we've added for this NPC
    private void Awake()
    {
        // Load the NPC schedule event list into a sorted set with our new NPCScheduleEventSort() custom sorter.
        // Putting that custom sorter in the SortetSet constructor will make use of the IComparer interface and the Compare method which sorts
        // all of the NPCScheduleEvent's in our SO by the time that they occur, and tie-broken by priorities if the times match
        npcScheduleEventSet = new SortedSet<NPCScheduleEvent>(new NPCScheduleEventSort());

        // Loop through all of the NPCScheduleEvent's in our SO's npcScheduleEventList, adding each one to our sortedSet. This will automatically sort
        // them how we specified in NPCScheduleEventSort.cs as we add them
        foreach (NPCScheduleEvent npcScheduleEvent in so_NPCScheduleEventList.npcScheduleEventList)
        {
            npcScheduleEventSet.Add(npcScheduleEvent);
        }

        // Get the NPC Path component for path building
        npcPath = GetComponent<NPCPath>();
    }


    // Subscribe the GameTimeSystem_AdvanceMinute method to the AdvanceGameMinute Event, which is called whenever a minute passes in gameTime
    // This method will then check if any scheduled events are supposed to occur at this minute (given the hour/day/season/weather), 
    // and then initiate the path building and NPC building. It will check the SortedSet of NPCScheduleEvents in order, so
    // it will stop when it gets past the current gameTime (tie-breakers are sorted by priority, so they will also be in the correct order
    private void OnEnable()
    {
        EventHandler.AdvanceGameMinuteEvent += GameTimeSystem_AdvanceMinute;
    }


    private void OnDisable()
    {
        EventHandler.AdvanceGameMinuteEvent -= GameTimeSystem_AdvanceMinute;
    }


    // This method is called every time the game Minute advances. It will loop through all of the npcScheduleEvents in the npcScheduleEventList
    // And see if we have a match (in time, day, season, weather, and priority if times match). If we do, this will call NPCPath
    // to build the path with AStar, and move the NPC accordingly with NPCMovement
    private void GameTimeSystem_AdvanceMinute(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        // Now that we know a minute has just advanced, get the current game time in the form of HHMM
        int time = (gameHour * 100) + gameMinute;

        // Attempt to get a matching schedule for this particular gameTime that just advanced to
        NPCScheduleEvent matchingNPCScheduleEvent = null;

        // Loop through all of the NPCScheduleEvents in the sorted set, which is already in time and priority order
        foreach (NPCScheduleEvent npcScheduleEvent in npcScheduleEventSet)
        {
            // First check if the current npcScheduleEvent occurs at the same time as the current game time
            if (npcScheduleEvent.Time == time)
            {
                // Now that the time matches, check if the parameters (day/season/weather) also match. If they don't, continue on to the next NPCScheduleEvent in the Set
                if (npcScheduleEvent.day != 0 && npcScheduleEvent.day != gameDay)
                {
                    continue;
                }
                if (npcScheduleEvent.season != Season.none && npcScheduleEvent.season != gameSeason)
                {
                    continue;
                }
                if (npcScheduleEvent.weather != Weather.none && npcScheduleEvent.weather != GameManager.Instance.currentWeather)
                {
                    continue;
                }

                // If all of the above checks pass (so we are on the correct time, day, season, and weather), we have a match! Play the Scheduled event! (build path, move NPC)
                // Note these are already sorted by priority, so the first match is automatically the correctly prioritized match for several schedules at the same time.
                // This will set matchingNPCScheduleEvent to the current NPCScheduleEvent, break out of the for loop, and then build the path down below
                matchingNPCScheduleEvent = npcScheduleEvent;
                break;
            }
            // If the time hasn't occured yet, dont do anything at all (The set is sorted by time, so if the current elements time hasn't occured yet, none of them will have!)
            else if (npcScheduleEvent.Time > time)
            {
                break;
            }
        }

        // Now, as long as we have found a matching event above (matchingNPCScheduleEvent isn't null anymore), Build the path with npcPath which takes
        // care of building the path with AStar, and moving the NPC with NPCMovement along the path, just by passing in this NPCScheduleEvent that matches the current time
        if (matchingNPCScheduleEvent != null)
        {
            npcPath.BuildPath(matchingNPCScheduleEvent);
        }
    }
}
