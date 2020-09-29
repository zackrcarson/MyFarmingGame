using System;
using System.Collections.Generic;
using UnityEngine;

// This is a singleton, so only one of them can exist!! Or else, our SingletonMonobehavious script will destroy the other one.
// This class subscribes to the ISaveable interface, which means we must include several methods for saving/loading data (here, we will save the current time - second, minute, day, season, year)
public class TimeManager : SingletonMonobehaviour<TimeManager>, ISaveable
{
    private int gameYear = 1;
    private Season gameSeason = Season.Spring;
    private int gameDay = 1;
    private int gameHour = 6;
    private int gameMinute = 30;
    private int gameSecond = 0;
    private string gameDayOfWeek = "Mon";

    private bool gameClockPaused = false;

    private float gameTick = 0f;

    // Unique ID required by the ISaveable interface, will store the GUID attached to the InventoryManager gameObject
    private string _iSaveableUniqueID;
    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }

    // GameObjectSave required by the ISaveable interface, storesd the save data that is built up for every object that has the ISaveable interface attached
    private GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }


    // on object awake, populate the GUID and GameObject saves for this object
    protected override void Awake()
    {
        base.Awake();

        // Get the unique ID for the GameObject
        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;

        // Initialize the GameObjectSave variable
        GameObjectSave = new GameObjectSave();
    }


    // On enable, this will just register this gameObject as an ISaveable, so that the SaveLoadManager can save/load the methods set up here
    // Also subscribe to scene loading/unloading events so we can pause/start the game clock while scenes are unloading/loading
    private void OnEnable()
    {
        // Registers this game object within the iSaveableObjectList, which is looped through in the SaveLoadManager for all objects to save/load the saved items
        ISaveableRegister();

        // Subscribe the BeforeSceneUnloadFadeOut and AfterSceneLoadFadeIn methods to the corresponding events. These methods will then pause and restart the game clock
        // when each one is triggered, respectively - so time doesn't pass as we load and reload scenes
        EventHandler.BeforeSceneUnloadEvent += BeforeSceneUnloadFadeOut;
        EventHandler.AfterSceneLoadEvent += AfterSceneLoadFadeIn;
    }


    // Deregister from the iSaveableObjectList
    private void OnDisable()
    {
        // Deregisters this game object within the iSaveableObjectList, which is looped through in the SaveLoadManager for all objects to save/load the saved items
        ISaveableDeregister();

        EventHandler.BeforeSceneUnloadEvent -= BeforeSceneUnloadFadeOut;
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoadFadeIn;
    }


    // Once the old scene begins to fade out, pause the game clock (gameClockPaused just stops the ticking if it's true)
    private void BeforeSceneUnloadFadeOut()
    {
        gameClockPaused = true;
    }


    // After the new scene finishes fading in, restart the game clock (gameClockPaused just starts the ticking if it's false)
    private void AfterSceneLoadFadeIn()
    {
        gameClockPaused = false;
    }


    private void Start()
    {
        // First notification to subscribers that a minute has passed (to initial time!)
        EventHandler.CallAdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
    }


    // If the game is not paused, call the game tick method to tick the clock forward every frame
    private void Update()
    {
        if (!gameClockPaused)
        {
            GameTick();
        }
    }


    // Every frame, this is called and the frame time is added to gameTick. 
    // If the gameTick is greater than the number of seconds per game seconds, subtract the seconds per game second out, and update the game second
    // This is because once the total amount of game time has exceeded the seconds per game second, we know one game second has passed!
    // So we update the game second (next method), and then subtract the secondsPerGameSecond so we keep the leftover game time that didn't go into the current second.
    private void GameTick()
    {
        gameTick +=Time.deltaTime;

        if (gameTick >= Settings.secondsPerGameSecond)
        {
            gameTick -= Settings.secondsPerGameSecond;

            UpdateGameSecond();
        }
    }


    // This is called every single time a game second has passed! We start by iterating gameSecond by one. Then, if the seconds is greater than
    // 59, we reset them to 0 and iterate the minutes. Then, once the seconds>59, and minutes>59, we set them to 0 and iterate the hour. Then,
    // once the seconds>59, minutes>59, and hours > 23, we set them to 0 and iterate the day, so on and so forth! Each time (after checking if the next biggest
    // increment is maximized), we will call the appropriate event for subscribers to see.
    private void UpdateGameSecond()
    {
        gameSecond++;

        if (gameSecond > 59)
        {
            gameSecond = 0;
            gameMinute++;

            if (gameMinute > 59)
            {
                gameMinute = 0;
                gameHour++;

                if (gameHour > 23)
                {
                    gameHour = 0;
                    gameDay++;

                    if (gameDay > 30)
                    {
                        gameDay = 1;

                        // Case Season enum as an int to iterate, and then cast the int back to a Season for proper updates.
                        int gs = (int)gameSeason;
                        gs++;
                        gameSeason = (Season)gs;

                        if (gs > 3)
                        {
                            gs = 0;
                            gameSeason = (Season)gs;

                            gameYear++;

                            // Reset the game year to 1 if it reaches 10,000 - we can't display 5 digits!
                            if (gameYear > 9999)
                            {
                                gameYear = 1;
                            }

                            EventHandler.CallAdvanceGameYearEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
                        }

                        EventHandler.CallAdvanceGameSeasonEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
                    }

                    // This will calculate the day of the week from the current day.
                    gameDayOfWeek = GetDayOfWeek();
                    EventHandler.CallAdvanceGameDayEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
                }

                EventHandler.CallAdvanceGameHourEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
            }

            EventHandler.CallAdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
        }

        // Call to update the game second here if desired!
    }


    // This method will calculate the day of the week from the game season and game day
    public string GetDayOfWeek()
    {
        // Multiply the current season (int) by 30 and add the current game day to get the total number of days since the Spring 1 in this game year
        int totalDays = (((int)gameSeason) * 30) + gameDay;

        // The day of the week is then the modulus of totalDays since Spring 1 with 7
        int dayOfWeek = totalDays % 7;

        // Return the proper day of the week string based on the remainder int
        switch (dayOfWeek)
        {
            case 1:
                return "Mon";
            
            case 2:
                return "Tue";
            
            case 3:
                return "Wed";
            
            case 4:
                return "Thu";
            
            case 5:
                return "Fri";
            
            case 6:
                return "Sat";
            
            case 0:
                return "Sun";
            
            default:
                return "";
        }
    }


    // This method returns a TimeSpan (unit of time in (hr, min, sec) for the current game Time, so the NPCs know the time, for scheduling purposes
    public TimeSpan GetGameTime()
    {
        // TimeSpan stores a time i.e. hr:min:sec in a tuple (hr, min, sec)
        TimeSpan gameTime = new TimeSpan(gameHour, gameMinute, gameSecond);

        return gameTime;
    }


    // This method returns the current game year, so the NPCs know the time, for dialogue purposes
    public int GetGameYear()
    {
        return gameYear;
    }


    // This method returns the current game season, so the NPCs know the time, for dialogue purposes
    public Season GetGameSeason()
    {
        return gameSeason;
    }


    //TODO:Remove
    /// <summary>
    /// Advance 1 game minute automatically
    /// </summary>
    public void TestAdvanceGameMinute()
    {
        // This just simply loops through 60 game seconds, and updates the game seconds each time so time passes much more quickly.
        // this will still allow all of the triggering higher-order time increments, etc to still work
        for (int i = 0; i < 60; i++)
        {
            UpdateGameSecond();
        }
    }


    //TODO:Remove
    /// <summary>
    /// Advance 1 game day automatically
    /// </summary>
    public void TestAdvanceGameDay()
    {
        // This just simply loops through 1 day of game seconds, and updates the game seconds each time so time passes much more quickly.
        // this will still allow all of the triggering higher-order time increments, etc to still work
        for (int i = 0; i < 86400; i++)
        {
            UpdateGameSecond();
        }
    }


    // Required method by the ISaveable interface, which will be called OnEnable() of the TimeManager GameObject, and it will 
    // Add an entry (of this gameObject) to the iSaveableObjectList in SaveLoadManager, which will then manage
    // Looping through all such items in this list to save/load their data
    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }


    // Required method by the ISaveable interface, which will be called OnDisable() of the TimeManager GameObject, and it will
    // Remove this item from the saveable objects list, as described above
    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }


    // Required method by the ISaveable interface. This will get called from the SaveLoadManager, for each scene to save the dictionaries (GameObjectSave has a dict keyed by scene name)
    // This method will store the sceneData for the current scene (). It will then return a GameObjectSave, which just has a Dict of SceneSave data for each scene, keyed by scene name
    public GameObjectSave ISaveableSave()
    {
        // Delete the sceneData (dict of data to save in that scene, keyed by scene name) for the GameObject if it already exists in the persistent scene
        // which is where this data is going to be saved, so we can create a new one with updated dictionaries
        GameObjectSave.sceneData.Remove(Settings.PersistentScene);

        // Create the SaveScene for this gameObject (keyed by the scene name, storing multiple dicts for bools, the scene the player ended in, the players location, the gridPropertyDetails,
        // the SceneItems, and the inventory items and quantities, and the gameYear, day, hour, minute, second, season, day of week)
        SceneSave sceneSave = new SceneSave();

        // Create a new int dictionary to store the times
        sceneSave.intDictionary = new Dictionary<string, int>();

        // Create a new string dictionary, to store the day of the week and the season
        sceneSave.stringDictionary = new Dictionary<string, string>();

        // Add values to the int dictionary for the different time increments, keyed so we can easily retrieve them in load
        sceneSave.intDictionary.Add("gameYear", gameYear);
        sceneSave.intDictionary.Add("gameDay", gameDay);
        sceneSave.intDictionary.Add("gameHour", gameHour);
        sceneSave.intDictionary.Add("gameMinute", gameMinute);
        sceneSave.intDictionary.Add("gameSecond", gameSecond);

        // Add values to the string dictionary for the day of week and season, keyed so we can easily retrieve them in load
        sceneSave.stringDictionary.Add("gameDayOfWeek", gameDayOfWeek);
        sceneSave.stringDictionary.Add("gameSeason", gameSeason.ToString());

        // Add the SceneSave data for the TimeManager game object to the GameObjectSave, which is a dict storing all the dicts in a scene to be loaded/saved, keyed by the scene name
        // The time manager will get stored in the Persistent Scene
        GameObjectSave.sceneData.Add(Settings.PersistentScene, sceneSave);

        // Return the GameObjectSave, which has a dict of the Saved stuff for the TimeManager GameObject
        return GameObjectSave;
    }


    // This is a required method for the ISaveable interface, which passes in a GameObjectSave dictionary, and restores the current scene from it
    // The SaveLoadManager script will loop through all of the ISaveableRegister GameObjects (all registered with their ISaveableRegister methods), and trigger this 
    // ISaveableLoad, which will load that Save data (here for the persistent scene time information, which includes the all of the time increments, day of week, and season),
    // for each scene (GameObjectSave is a Dict keyed by scene name).
    public void ISaveableLoad(GameSave gameSave)
    {
        // gameSave stores a Dictionary of items to save keyed by GUID, see if there's one for this GUID (generated on the InventoryManager GameObject)
        if (gameSave.gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;

            // Get the save data for the scene, if one exists for the PersistentScene (what the time info is saved under)
            if (gameObjectSave.sceneData.TryGetValue(Settings.PersistentScene, out SceneSave sceneSave))
            {
                // If both the intDictionary (storing time increments) and the stringDictionary (storiny day of week and season)
                // exist, populate the saved values!
                if (sceneSave.intDictionary != null && sceneSave.stringDictionary != null)
                {
                    // Check if the intDictionary contains entries for the year, day, hour, minute and second. If so, populate the gameClock with the saved values
                    if (sceneSave.intDictionary.TryGetValue("gameYear", out int savedGameYear))
                    {
                        gameYear = savedGameYear;
                    }
                    if (sceneSave.intDictionary.TryGetValue("gameDay", out int savedGameDay))
                    {
                        gameDay = savedGameDay;
                    }
                    if (sceneSave.intDictionary.TryGetValue("gameHour", out int savedGameHour))
                    {
                        gameHour = savedGameHour;
                    }
                    if (sceneSave.intDictionary.TryGetValue("gameMinute", out int savedGameMinute))
                    {
                        gameMinute = savedGameMinute;
                    }
                    if (sceneSave.intDictionary.TryGetValue("gameSecond", out int savedGameSecond))
                    {
                        gameSecond = savedGameSecond;
                    }

                    // Check if the stringDictionary contains entries for the DayOfWeek and Season. If so, populate the gameClock with the saved values
                    if (sceneSave.stringDictionary.TryGetValue("gameDayOfWeek", out string savedGameDayOfWeek))
                    {
                        gameDayOfWeek = savedGameDayOfWeek;
                    }
                    if (sceneSave.stringDictionary.TryGetValue("gameSeason", out string savedGameSeason))
                    {
                        // For the Season Enum, we have to check if it passed us a proper Season Enum value before setting it, which will return out a Season Enum
                        if (Enum.TryParse<Season>(savedGameSeason, out Season season))
                        {
                            gameSeason = season;
                        }
                    }

                    // Zero out the game tick so it can start counting seconds fresh from the beginning of the updated gameSecond
                    gameTick = 0f;

                    // Trigger the advance minute event so any subscribers will know that a "minute" has happened, so the game clock GUI is refreshed
                    EventHandler.CallAdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
                }
            }
        }
    }


    // Required method by the ISaveable interface, which will store all of the scene data, executed for every item in the iSaveableObjectList. This let's us walk between
    // scenes and keep the stored stuff active with ISaveableRestoreScene 
    public void ISaveableStoreScene(string sceneName)
    {
        // Nothing to store here since the TimeManager is on a persistent scene - it won't get reset ever because we always stay on that scene
    }


    // Required method by the ISaveable interface, which will restore all of the scene data, executed for every item in the iSaveableObjectList. This let's us walk between
    // scenes and keep the stored stuff active with ISaveableRestoreScene 
    public void ISaveableRestoreScene(string sceneName)
    {   
        // Nothing to restore here since the TimeManager is on a persistent scene - it won't get reset ever because we always stay on that scene
    }
}
