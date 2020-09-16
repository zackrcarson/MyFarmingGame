using UnityEngine;
using UnityEngine.UI;

public class UIManager : SingletonMonobehaviour<UIManager>
{
    // This property is a boolean value that controls whether or not the pause menu is active
    private bool _pauseMenuOn = false;
    public bool PauseMenuOn { get => _pauseMenuOn; set => _pauseMenuOn = value; }

    // These are populated in the editor with the pause menu canvas (to activate and deactivate it when the player pauses/unpauses), a list of it's tabs (to activate 
    // different tabs within the pause menu), and a list of it's buttons (to control when the tabs are clicked with the Unity Buttons component!)
    [SerializeField] private GameObject pauseMenu = null;
    [SerializeField] private GameObject[] menuTabs = null;
    [SerializeField] private Button[] menuButtons = null;


    // Deactivate the pause menu when the game starts
    protected override void Awake()
    {
        base.Awake();

        pauseMenu.SetActive(false);
    }


    // Every frame, call the method that activates/deactivates the pause menu whenever the player controls it
    private void Update()
    {
        PauseMenu();
    }


    // Checks for player ESC input to pause or unpause the game (depending on if it's currently unpaused or paused)
    private void PauseMenu()
    {
        // If the player presses ESC, pause the game if it's unpaused. If it's already paused, unpause it
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PauseMenuOn)
            {
                DisablePauseMenu();
            }
            else
            {
                EnablePauseMenu();
            }
        }
    }

    
    // This method will disable the players input, turn off time counting, enable the pauseMenu UI, runs garbage collection, and highlight the selected tab's button
    private void EnablePauseMenu()
    {
        // Bool for us to know when the pause menu is on or not, so we know whether to pause or unpause on ESC
        PauseMenuOn = true;

        // Disables the players movement input
        Player.Instance.PlayerInputIsDisabled = true;

        // Stops running all update methods! No more time is counted
        Time.timeScale = 0; 

        // Set the pause menu UI to be active to cover the screen with it
        pauseMenu.SetActive(true);

        // Trigger the garbage collector - might as well do it now while not much is happening
        System.GC.Collect();

        // Highlight the selected button
        HighlightButtonForSelectedTab();
    }


    // This method will re-enable the players input, turn back on time counting, and disables the pauseMenu UI
    private void DisablePauseMenu()
    {
        // Bool for us to know when the pause menu is on or not, so we know whether to pause or unpause on ESC
        PauseMenuOn = false;

        // Enables the players movement input
        Player.Instance.PlayerInputIsDisabled = false;

        // Starts running all update methods again! Time flows again
        Time.timeScale = 1;

        // Set the pause menu UI to be inactivea to uncover the screen with it
        pauseMenu.SetActive(false);
    }


    // This method simply loops through the menu tabs, and highlights the selected tab, and unhighlights all of the unselected tabs
    private void HighlightButtonForSelectedTab()
    {
        // Loop through all of the menuTabs populated as a list in the editor
        for (int i = 0; i < menuTabs.Length; i++)
        {
            // Active self is a bool determining whether a gameObject is active or not, EVEN IF THE PARENT OBJECT IS INACTIVE!! Here, we are
            // Checking if the current menu button is active or not (only the selected one is active)
            // The active/inactive tabs are determined in SwitchPauseMenuTab, which then calls this method
            if (menuTabs[i].activeSelf)
            {
                // If the current tab IS active, set the button color to active for the current menuTab. Else, set the button color to inactive
                SetButtonColorToActive(menuButtons[i]);
            }
            else
            {
                SetButtonColorToInactive(menuButtons[i]);
            }
        }
    }


    // Given the found button that we want highlighted, set the color of that button to the pressedColor
    private void SetButtonColorToActive(Button button)
    {
        // Each button has normal color, highlighted color, pressed color, selected color, and disabled color, stored in a ColorBlock data type
        ColorBlock colors = button.colors;

        // Set the normalColor to be the pressedColor set up in the editor (pure white)
        colors.normalColor = colors.pressedColor;

        // Update the button colors with the above changed color
        button.colors = colors;
    }


    // Given the found buttons that we don't want highlighted, set the color of that button to the disabledColor
    private void SetButtonColorToInactive(Button button)
    {
        // Each button has normal color, highlighted color, pressed color, selected color, and disabled color, stored in a ColorBlock data type
        ColorBlock colors = button.colors;

        // Set the normalColor to be the disabledColor set up in the editor (pure white)
        colors.normalColor = colors.disabledColor;

        // Update the button colors with the above changed color
        button.colors = colors;
    }


    // This method will be triggered from each of the button objects 'On Click {}' functionality, which calls this method with the tabNumber of the button that was clicked!
    // Given the tab number that we have clicked, this will set that tab to active, and the other ones to inactive. It will then 
    // highlight the selected tab, and unhighlight all of the others via HighlightButtonForSelectedTab
    public void SwitchPauseMenuTab(int tabNum)
    {
        // Loop through all of the tabs populated in the editor
        for (int i = 0; i < menuTabs.Length; i++)
        {
            // If the current tab isn't the one we've selected, deactive the gameObject. If it is, activate it
            if (i != tabNum)
            {
                menuTabs[i].SetActive(false);
            }
            else
            {
                menuTabs[i].SetActive(true);
            }
        }
        // Highlight the active tab, and unhighlight all of the inactive tabs (as set up in the above for loop
        HighlightButtonForSelectedTab();
    }
}
