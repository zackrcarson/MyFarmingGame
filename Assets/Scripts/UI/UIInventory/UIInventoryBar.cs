using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInventoryBar : MonoBehaviour
{
    [SerializeField] private Sprite blank16x16sprite = null;
    [SerializeField] private UIInventorySlot[] inventorySlot = null; // This is populated in-editor with the 12 inventory slot UI gameObjects
    public GameObject inventoryBarDraggedItem;

    private RectTransform rectTransform;

    private bool _isInventoryBarPositionBottom = true;

    public bool IsInventoryBarPositionBottom {get => _isInventoryBarPositionBottom; set => _isInventoryBarPositionBottom = value;}


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }


    private void OnDisable()
    {
        // When this item is disabled, we will unsubscribe to the InventoryUpdatedEvent
        EventHandler.InventoryUpdatedEvent -= InventoryUpdated;
    }


    private void OnEnable()
    {
        // When this item is enabled, we will subscribe to the InventoryUpdatedEvent so we will catch it every time it is triggered
        EventHandler.InventoryUpdatedEvent += InventoryUpdated;
    }


    private void ClearInventorySlots()
    {
        if (inventorySlot.Length > 0)
        {
            // Loop through the inventory slots and update with the blank sprite empty string, null item details, and 0 quantity
            for (int i = 0; i < inventorySlot.Length; i++)
            {
                inventorySlot[i].inventorySlotImage.sprite = blank16x16sprite;
                inventorySlot[i].textMeshProUGUI.text = "";
                inventorySlot[i].itemDetails = null;
                inventorySlot[i].itemQuantity = 0;
            }
        }
    }


    private void InventoryUpdated(InventoryLocation inventoryLocation, List<InventoryItem> inventoryList)
    {
        if (inventoryLocation == InventoryLocation.player)
        {
            ClearInventorySlots();

            // If the inventorySlot list (populated in the editor with the 12 UI inventory slot gameObjects) is greater than 0, and there are items in the inventory list,
            if (inventorySlot.Length > 0 && inventoryList.Count > 0)
            {
                // Loop through inventory slots and update with corresponding inventory list item, as long as the current slot is less than the total items in the inventory list
                for (int i = 0; i < inventorySlot.Length; i++)
                {
                    if (i < inventoryList.Count)
                    {
                        int itemCode = inventoryList[i].itemCode;

                        ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(itemCode);

                        if (itemDetails != null)
                        {
                            // Add the image, text, details, and quantity to the inventory item slot
                            inventorySlot[i].inventorySlotImage.sprite = itemDetails.itemSprite;
                            inventorySlot[i].textMeshProUGUI.text = inventoryList[i].itemQuantity.ToString();
                            inventorySlot[i].itemDetails = itemDetails;
                            inventorySlot[i].itemQuantity = inventoryList[i].itemQuantity;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
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
