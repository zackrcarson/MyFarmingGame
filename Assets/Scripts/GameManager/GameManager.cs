using UnityEngine;

public class GameManager : SingletonMonobehaviour<GameManager>
{
    // This will allow us to control the weather for testing NPC Schedule events. We can change the weather in the inspector!
    public Weather currentWeather;

    protected override void Awake()
    {
        base.Awake();

        // TODO: Need a resolution settings options screen
        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow, 0);

        // Set the starting weather as Dry
        currentWeather = Weather.dry;
    }
}
