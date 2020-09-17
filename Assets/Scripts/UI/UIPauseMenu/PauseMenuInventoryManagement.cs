using System;
using System.Collections.Generic;
using System.Configuration;
using UnityEngine;

// This will sit on our Tab0InventoryManagement gameObject, which is the first panel of our pause screen. It will manage our 48 inventory slot that are created underneath it
public class PauseMenuInventoryManagement : MonoBehaviour
{
    // This is populated in the editor with an array of our PauseMenuInventoryManagementSlot components (for all 48 slots on this panel!)
    [SerializeField] private PauseMenuInventoryManagementSlot[] inventoryManagementSlot = null;

    // Prefab for dragged items
    public GameObject inventoryManagementDraggedItemPrefab;

    // Transparent sprite
    [SerializeField] private Sprite transparent16x16 = null;

    // Game Object that pops up when we hover over items in the inventory, displaying all of the item's details
    [HideInInspector] public GameObject inventoryTextBoxGameObject;


    // Subscribe the PopulatePlayerInventory method to the InventoryUpdatedEvent, which will populate the player's inventory with the updated items when called
    private void OnEnable()
    {
        // Populate the inventory slots if we get an event saying something was updated
        EventHandler.InventoryUpdatedEvent += PopulatePlayerInventory;

        // Or, if we just enabled the pause screen Inventory tab (or entered this tab), Populate the players inventory without the event
        if (InventoryManager.Instance != null)
        {
            PopulatePlayerInventory(InventoryLocation.player, InventoryManager.Instance.inventoryLists[(int)InventoryLocation.player]);
        }
    }


    // Subscribe the PopulatePlayerInventory method to the InventoryUpdatedEvent, which will populate the player's inventory with the updated items when called
    private void OnDisable()
    {
        EventHandler.InventoryUpdatedEvent -= PopulatePlayerInventory;

        // When this gameObject has been disabled (i.e. when we switch pause menu screens, or un-pause, this will destroy any text boxes that might be active showing item details
        DestroyInventoryTextBoxGameObject();
    }


    // This method simplay destroys the itemDescription text box object when we un-pause, switch pause menu panels, etc.
    public void DestroyInventoryTextBoxGameObject()
    {
        // Destroy the inventory text box if it's been created
        if (inventoryTextBoxGameObject != null)
        {
            Destroy(inventoryTextBoxGameObject);
        }
    }


    // This method will destroy all of the currently dragged items in the inventory panel (i.e. if we switch panels or un-pause the game)
    public void DestroyCurrentlyDraggedItems()
    {
        // Loop through all of the players inventory items
        for (int i = 0; i < InventoryManager.Instance.inventoryLists[(int)InventoryLocation.player].Count; i++)
        {
            // If the current item has a dragged item, just destroy it
            if (inventoryManagementSlot[i].draggedItem != null)
            {
                Destroy(inventoryManagementSlot[i].draggedItem);
            }
        }
    }


    // First this method will clear out all of the inventory slots, and then re-populate them all with the
    // player's current inventory items
    private void PopulatePlayerInventory(InventoryLocation inventoryLocation, List<InventoryItem> playerInventoryList)
    {
        // Make sure we're looking at the player's inventory, rather than a chest
        if (inventoryLocation == InventoryLocation.player)
        {
            // This clears out all of the slots in the inventory, so we can loop through them all and set new ones
            InitialiseInventoryManagementSlots();

            // Loop through all of the player's inventory items
            for (int i = 0; i < InventoryManager.Instance.inventoryLists[(int)InventoryLocation.player].Count; i++)
            {
                // Get the inventory item details and quantity for the current item
                inventoryManagementSlot[i].itemDetails = InventoryManager.Instance.GetItemDetails(playerInventoryList[i].itemCode);
                inventoryManagementSlot[i].itemQuantity = playerInventoryList[i].itemQuantity;

                if (inventoryManagementSlot[i].itemDetails != null)
                {
                    // Update the inventory management slot with the image sprite and quantity
                    inventoryManagementSlot[i].inventoryManagementSlotImage.sprite = inventoryManagementSlot[i].itemDetails.itemSprite;
                    inventoryManagementSlot[i].textMeshProUGUI.text = inventoryManagementSlot[i].itemQuantity.ToString();
                }
            }
        }
    }


    // This method will loop through all of the slots, clear out the greyedOut GO, item details, quantity, and sprites, and text. Then it will deactivate all of
    // the unusable inventory slots (by activating the greyedOut gameobject)
    private void InitialiseInventoryManagementSlots()
    {
        // Clear all of the current inventoryslots, up to the maximum inventory capacity (48 currently, ALL of the slots we have in the inventory page, not just the active ones
        for (int i = 0; i < Settings.playerMaximumInventoryCapacity; i++)
        {
            // For the current slot, deactivate the greyed out image, clear the item details and quantity, set the sprite to transparent, and the TMP text to blank
            inventoryManagementSlot[i].greyedOutImageGO.SetActive(false);
            inventoryManagementSlot[i].itemDetails = null;
            inventoryManagementSlot[i].itemQuantity = 0;
            inventoryManagementSlot[i].inventoryManagementSlotImage.sprite = transparent16x16;
            inventoryManagementSlot[i].textMeshProUGUI.text = "";
        }

        // Grey out all of the unavailable slots (until we upgrade the player's backpack!), loop from the current players capacity, until the maximum allowable capacity
        for (int i = InventoryManager.Instance.inventoryListCapacityIntArray[(int)InventoryLocation.player]; i < Settings.playerMaximumInventoryCapacity; i++)
        {
            // Set these last slots to greyed out because we can't use them yet
            inventoryManagementSlot[i].greyedOutImageGO.SetActive(true);
        }
    }
}   
