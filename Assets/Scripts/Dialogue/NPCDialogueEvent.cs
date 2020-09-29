using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]
public class NPCDialogueEvent
{
    public string dialogueDetail;

    public int hourMin;
    public int hourMax;

    public int priority; 

    public List<DayOfWeek> daysOfWeek;
    public List<Weather> weatherTypes;
    public List<Season> seasons;
    public List<int> years;

    public List<SceneName> sceneNames;

    public string[] dialogue;
    public NPCEmotions[] emotions;


    public NPCDialogueEvent(int hourMin, int hourMax, int priority, List<DayOfWeek> daysOfWeek, List<Weather> weatherTypes, List<Season> seasons, List<int> years,
                            List<SceneName> sceneNames, string[] dialogue, NPCEmotions[] emotions)
    {
        this.hourMin = hourMin;
        this.hourMax = hourMax;
        this.priority = priority;
        this.daysOfWeek = daysOfWeek;
        this.weatherTypes = weatherTypes;
        this.seasons = seasons;
        this.years = years;
        this.sceneNames = sceneNames;
        this.dialogue = dialogue;
        this.emotions = emotions;
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
