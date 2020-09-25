using System.Collections.Generic;

// This class will be attached to the constructor for creating a SortedSet of NPCScheduleEvent's. This class inherits from ICompare, which uses the Compare() method
// Within to compare individual elements in the list, so when we call Sort() on the list, it will use this to sort them.
// Here we are sorting the events by time, and then tie-broken by priority
public class NPCScheduleEventSort : IComparer<NPCScheduleEvent>
{
    // This IComparer compares two NPCScheduleEvent values, first checking the time they occur, and if the times match, compare their priorities.
    // NPCScheduleEvent's with earlier times, or equal times and higher priority  will go sooner in the list, so they will be caught first when we check which sheduled event to play.
    // This method returns -1 if event1 < event2, +1 if event1 > event 2, and 0 if event1 = event2 (in this case, in both time and priority)
    public int Compare(NPCScheduleEvent npcScheduleEvent1, NPCScheduleEvent npcScheduleEvent2)
    {
        // First check if they occur at the same time. If they do, return -1 if event2 has a higher priority, and +1 if event2 has a lower priority
        // ?. is a null reference exception, so if e.g. npcScheduleEvent1?.Time = null,, it just drops out of that if statement automatic, with no null reference errors
        if (npcScheduleEvent1?.Time == npcScheduleEvent2?.Time)
        {
            if (npcScheduleEvent1?.priority < npcScheduleEvent2?.priority)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
        // If they aren't the same time (and same priority), return 1 if event1's time is sooner than event2's, and -1 if event2 is sooner than event1
        else if (npcScheduleEvent1?.Time > npcScheduleEvent2?.Time)
        {
            return 1;
        }
        else if (npcScheduleEvent1?.Time < npcScheduleEvent2?.Time)
        {
            return -1;
        }
        // If it passes by all of the above statements, return 0 (this means the events are at the same time AND priority)
        else
        {
            return 0;
        }
    }
}
