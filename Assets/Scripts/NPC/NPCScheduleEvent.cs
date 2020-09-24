using UnityEngine;

// NPCScheduleEvent is used to store all of the details about an NPC's movement event - the time, weather, season, scene, where to go, which direction to face, and what animation to do 
// when there. This will be used to inform the NPCMovement what the NPC needs to do for a schedule. These NPCScheduleEvent's will be created by our NPCScheduler (or AStarTest)
[System.Serializable]
public class NPCScheduleEvent
{
    // Our NPC scheduler will check if all of the below members match a created schedule. If so, it will build a path for that NPC and enable the movement
    public int hour; // At what time during the day will the schedule take effect
    public int minute;
    public int priority; // Which order should conflicting events go in 
    public int day; // If something should happen only on a particular day/weather/season
    public Weather weather;
    public Season season;

    // The scene name and grid coordinate we want our NPC to move to, which direction we want them to face when they get there, and which animation to play when they get there (i.e. smoke, dig, etc)
    public SceneName toSceneName;
    public GridCoordinate toGridCoordinate;
    public Direction npcFacingDirectionAtDestination = Direction.none;
    public AnimationClip animationAtDestination;


    // Returns the time in the format e.g. 1152 for 11:52
    public int Time
    {
        get
        {
            return (hour * 100) + minute;
        }
    }


    // Constructor for creating NPCScheduleEvents. Populates all of the event details for a given schedule event
    public NPCScheduleEvent(int hour, int minute, int priority, int day, Weather weather, Season season, 
                            SceneName toSceneName, GridCoordinate toGridCoordinate, AnimationClip animationAtDestination)
    {
        this.hour = hour;
        this.minute = minute;
        this.priority = priority;
        this.day = day;
        this.weather = weather;
        this.season = season;
        this.toSceneName = toSceneName;
        this.toGridCoordinate = toGridCoordinate;
        this.animationAtDestination = animationAtDestination;
    }


    // Empty Constructor if we want to just instantiate an empty NPCScheduleEvent and populate the members afterwards
    public NPCScheduleEvent()
    {

    }


    // This overrides the ToString method, So we can use it for debugging so we can easily see the details of a scheduled event, in a nice formatted way
    public override string ToString()
    {
        return $"Time: {Time}, Priority: {priority}, Day: {day}, Weather: {weather}, Season: {season}";
    }
}
