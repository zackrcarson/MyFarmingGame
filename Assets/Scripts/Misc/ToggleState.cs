using UnityEngine.UI;
using UnityEngine;
using System.Diagnostics;

// This script is in charge of toggles for the character customization screen. When a toggle is selected,
// This will unselect the other toggle in the same category and apply those character customization changes
// in the ApplyCharacterCustomization script
public class ToggleState : MonoBehaviour
{
    [HideInInspector]
    public Toggle toggle;

    public Toggle otherToggle;

    public ApplyCharacterCustomization playerCusomize = null;

    public void Start()
    {
        toggle = GetComponent<Toggle>();
    }


    // For changing the gender
    public void toggleOffGender(int sexNum)
    {
        // We need to check both so that when we change the other toggle to off, it's On Click functionality won't turn off the current toggle as well..
        if (toggle.isOn && otherToggle.isOn)
        {
            // Set the other toggle to off
            toggle.isOn = false;

            // Process the gender change
            playerCusomize.ChangeGender(sexNum);
        }
    }


    // For changing the shirt
    public void toggleOffShirt(int shirtNum)
    {
        // We need to check both so that when we change the other toggle to off, it's On Click functionality won't turn off the current toggle as well..
        if (toggle.isOn && otherToggle.isOn)
        {
            // Set the other toggle to off
            toggle.isOn = false;

            // Process the gender change
            playerCusomize.ChangeShirt(shirtNum);
        }
    }
}
