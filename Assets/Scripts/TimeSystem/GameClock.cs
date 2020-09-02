using TMPro;
using UnityEngine;

public class GameClock : MonoBehaviour
{
    // TMP elements for describing current time
    [SerializeField] private TextMeshProUGUI timeText = null;
    [SerializeField] private TextMeshProUGUI dateText = null;
    [SerializeField] private TextMeshProUGUI seasonText = null;
    [SerializeField] private TextMeshProUGUI yearText = null;


    private void OnEnable()
    {
        // Subscribe the UpdateGameTime method to the event that is published every time a minute passes
        EventHandler.AdvanceGameMinuteEvent += UpdateGameTime;
    }


    private void OnDisable()
    {
        // Unsubscribe the UpdateGameTime method to the event that is published every time a minute passes
        EventHandler.AdvanceGameMinuteEvent -= UpdateGameTime;
    }

    
    // This method is activated every time the EventHandler publishes an event for the game minute advancing! This will then update the clock UI
    private void UpdateGameTime(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        // Update the time

        // Only show for ten minute incremenets! Subtracting the mod 10 will always return the last multiple of ten that the minutes passed
        gameMinute = gameMinute - (gameMinute % 10);

        string ampm= "";
        string minute;

        // PM if hours is > 12, AM if it's before
        if (gameHour >= 12)
        {
            ampm = " pm";
        }
        else
        {
            ampm = " am";
        }

        // Using a standard 12-hr clock. So if it's greater than 12, subtract 12 to go to the next AM/PM
        if (gameHour >= 13)
        {
            gameHour -= 12;
        }

        // If the game minute is < 10, add a 0 before the final string so it still has two digits (at this point, anything < 10 will be 0! So this will return "00")
        // Else, just set the minute text to the actual game minutes 10 integer
        if (gameMinute < 10)
        {
            minute = "0" + gameMinute.ToString();
        }
        else
        {
            minute = gameMinute.ToString();
        }

        // The final time is hour : minute am/pm
        string time = gameHour.ToString() + " : " + minute + ampm;

        // Set the text in the the time, date, season, and year for the TMP GUI to display
        timeText.SetText(time);
        dateText.SetText(gameDayOfWeek + ". " + gameDay.ToString());
        seasonText.SetText(gameSeason.ToString());
        yearText.SetText("Year " + gameYear);
    }
}
