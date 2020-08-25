using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInventoryBar : MonoBehaviour
{
    private RectTransform rectTransform;

    private bool _isInventoryBarPositionBottom = true;

    public bool IsInventoryBarPositionBottom {get => _isInventoryBarPositionBottom; set => _isInventoryBarPositionBottom = value;}

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        // Switch the inventory bar position depending on the players position
        SwitchInventoryBarPosition();
    }

    private void SwitchInventoryBarPosition()
    {
        // This vector is the players position on the camera field of view (not world coords), as computed in the Player class
        Vector3 playerViewportPosition = Player.Instance.GetPlayerViewportPosition();

        // Check if the player's viewport position is at the bottom of the screen or not. If it is, move the UI bar to the top of the viewport. 
        // Else, move it (keep it) to the bottom
        if (playerViewportPosition.y > 0.3f && IsInventoryBarPositionBottom == false)
        {
            // These rectTransform values are just the default ones we set up in the editor
            rectTransform.pivot = new Vector2(0.5f, 0f);
            rectTransform.anchorMin = new Vector2(0.5f, 0f);
            rectTransform.anchorMax = new Vector2(0.5f, 0f);
            rectTransform.anchoredPosition = new Vector2(0f, 2.5f);

            IsInventoryBarPositionBottom = true;
        }
        else if (playerViewportPosition.y <= 0.3f && IsInventoryBarPositionBottom == true)
        {
            // These rectTransform values are for the top of the screen instead
            rectTransform.pivot = new Vector2(0.5f, 1f);
            rectTransform.anchorMin = new Vector2(0.5f, 1f);
            rectTransform.anchorMax = new Vector2(0.5f, 1f);
            rectTransform.anchoredPosition = new Vector2(0f, -2.5f);

            IsInventoryBarPositionBottom = false;
        }
    }
}
