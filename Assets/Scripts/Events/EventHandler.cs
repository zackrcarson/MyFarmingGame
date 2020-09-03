using System;
using System.Collections.Generic;

public delegate void MovementDelegate(float xInput, float yInput, bool isWalking, bool isRunning, bool isIdle, bool isCarrying, ToolEffect toolEffect, 
    bool isUsingToolRight, bool isUsingToolLeft, bool isUsingToolUp, bool isUsingToolDown, 
    bool isLiftingToolRight, bool isLiftingToolLeft, bool isLiftingToolUp, bool isLiftingToolDown, 
    bool isPickingLeft, bool isPickingRight, bool isPickingUp, bool isPickingDown, 
    bool isSwingingToolRight, bool isSwingingToolLeft, bool isSwingingToolUp, bool isSwingingToolDown, 
    bool idleUp, bool idleDown, bool idleLeft, bool idleRight);


public static class EventHandler
{
    // Inventory Updated event
    public static event Action<InventoryLocation, List<InventoryItem>> InventoryUpdatedEvent;

    // Inventory updated event call for publishers, to send out event details to any subscribers
    public static void CallInventoryUpdatedEvent(InventoryLocation inventoryLocation, List<InventoryItem> inventoryList)
    {
        // Check if there are any subscribers - or else do nothing!
        if (InventoryUpdatedEvent != null)
        {
            InventoryUpdatedEvent(inventoryLocation, inventoryList);
        }
    }


    // Movement Event
    public static event MovementDelegate MovementEvent;

    // Movement Event Call For Publishers
    public static void CallMovementEvent(float xInput, float yInput, bool isWalking, bool isRunning, bool isIdle, bool isCarrying, ToolEffect toolEffect, 
        bool isUsingToolRight, bool isUsingToolLeft, bool isUsingToolUp, bool isUsingToolDown, 
        bool isLiftingToolRight, bool isLiftingToolLeft, bool isLiftingToolUp, bool isLiftingToolDown, 
        bool isPickingLeft, bool isPickingRight, bool isPickingUp, bool isPickingDown, 
        bool isSwingingToolRight, bool isSwingingToolLeft, bool isSwingingToolUp, bool isSwingingToolDown, 
        bool idleUp, bool idleDown, bool idleLeft, bool idleRight)
    {
        if (MovementEvent != null)
        {
            MovementEvent(xInput, yInput, isWalking, isRunning, isIdle, isCarrying, toolEffect, 
                isUsingToolRight, isUsingToolLeft, isUsingToolUp, isUsingToolDown, 
                isLiftingToolRight, isLiftingToolLeft, isLiftingToolUp, isLiftingToolDown, 
                isPickingLeft, isPickingRight, isPickingUp, isPickingDown, 
                isSwingingToolRight, isSwingingToolLeft, isSwingingToolUp, isSwingingToolDown, 
                idleUp, idleDown, idleLeft, idleRight);
        }
    }



    // Time events. Every time the game minute, hour, day, year, or season is updated, we will send out an event to any subscribers to use as they will

    // Game minute advanced
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameMinuteEvent;

    public static void CallAdvanceGameMinuteEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        if (AdvanceGameMinuteEvent != null)
        {
            AdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
        }
    }


    // Game hour advanced
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameHourEvent;

    public static void CallAdvanceGameHourEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        if (AdvanceGameHourEvent != null)
        {
            AdvanceGameHourEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
        }
    }


    // Game day advanced
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameDayEvent;

    public static void CallAdvanceGameDayEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        if (AdvanceGameDayEvent != null)
        {
            AdvanceGameDayEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
        }
    }


    // Game season advanced
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameSeasonEvent;

    public static void CallAdvanceGameSeasonEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        if (AdvanceGameSeasonEvent != null)
        {
            AdvanceGameSeasonEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
        }
    }


    // Game year advanced
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameYearEvent;

    public static void CallAdvanceGameYearEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        if (AdvanceGameYearEvent != null)
        {
            AdvanceGameYearEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
        }
    }


    // Scene Load Events - in the order they happen!

    // Before Scene Unload FadeOut Event - called right before the fade out before unloading the current scene
    public static event Action BeforeSceneUnloadFadeOutEvent;

    public static void CallBeforeSceneUnloadFadeOutEvent()
    {
        if (BeforeSceneUnloadFadeOutEvent != null)
        {
            BeforeSceneUnloadFadeOutEvent();
        }
    }


    // Before Scene Unload Event - called right after the fade out before unloading the current scene
    public static event Action BeforeSceneUnloadEvent;

    public static void CallBeforeSceneUnloadEvent()
    {
        if (BeforeSceneUnloadEvent != null)
        {
            BeforeSceneUnloadEvent();
        }
    }


    // After Scene Loaded Event - called right after loading the new scene, before fading back in
    public static event Action AfterSceneLoadEvent;

    public static void CallAfterSceneLoadEvent()
    {
        if (AfterSceneLoadEvent != null)
        {
            AfterSceneLoadEvent();
        }
    }


    // After Scene Load Fade In Event - called right fading in with the new scene
    public static event Action AfterSceneLoadFadeInEvent;

    public static void CallAfterSceneLoadFadeInEvent()
    {
        if (AfterSceneLoadFadeInEvent != null)
        {
            AfterSceneLoadFadeInEvent();
        }
    }
}