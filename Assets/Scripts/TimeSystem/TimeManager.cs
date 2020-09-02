using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is a singleton, so only one of them can exist!! Or else, our SingletonMonobehavious script will destroy the other one.
public class TimeManager : SingletonMonobehaviour<TimeManager>
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

            Debug.Log("Game Year: " + gameYear + ",  Game Season: " + gameSeason + ",  Game Day: " + gameDay + ",  Game Day of Week: " + gameDayOfWeek + ",  Game Hour: " + gameHour + ",  Game Minute: " + gameMinute + ",  Game Second: " + gameSecond + ".");
        }
    }


    // This method will calculate the day of the week from the game season and game day
    private string GetDayOfWeek()
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
}
