using UnityEngine;

public class GameManager : SingletonMonobehaviour<GameManager>
{
    protected override void Awake()
    {
        base.Awake();

        // TODO: Need a resolution settings options screen
        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow, 0);
    }
}
