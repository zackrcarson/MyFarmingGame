using UnityEngine;

[System.Serializable]
public class NPCDialogueEvent
{
    public string dialogueDetail;

    public int hourMin;
    public int hourMax;

    public int priority; 

    public DayOfWeek[] daysOfWeek;
    public Weather[] weatherTypes;
    public Season[] seasons;

    public SceneName[] sceneNames;

    public string[] dialogue;
    public NPCEmotions[] emotions;


    public NPCDialogueEvent(int hourMin, int hourMax, int priority, DayOfWeek[] daysOfWeek, Weather[] weatherTypes, Season[] seasons, 
                            SceneName[] sceneNames)
    {
        this.hourMin = hourMin;
        this.hourMax = hourMax;
        this.priority = priority;
        this.daysOfWeek = daysOfWeek;
        this.weatherTypes = weatherTypes;
        this.seasons = seasons;
        this.sceneNames = sceneNames;
    }


    // Empty Constructor if we want to just instantiate an empty NPCScheduleEvent and populate the members afterwards
    public NPCDialogueEvent()
    {

    }


    // This overrides the ToString method, So we can use it for debugging so we can easily see the details of a scheduled event, in a nice formatted way
    public override string ToString()
    {
        return $"Time range: {hourMin} - {hourMax}, Priority: {priority}, Days: {daysOfWeek}, Weather Types: {weatherTypes}, Seasons: {seasons}";
    }
}
