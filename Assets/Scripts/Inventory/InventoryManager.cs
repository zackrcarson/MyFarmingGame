using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : SingletonMonobehaviour<InventoryManager>
{
    // Create a dictionary for inventory, with itemCode : itemDetails. This will be fast to access
    private Dictionary<int, ItemDetails> itemDetailsDictionary;

    // The index of the array is the inventory list location (player, chest), and the value is the item code of the selected item (so they can still swap)
    private int[] selectedInventoryItem;

    // InventoryItem is a struct that stores the item code, and the number held
    public List<InventoryItem>[] inventoryLists;

    // The index of the array is the inventory list location (from the InventoryLocation enum), and the value is the capacity of that inventory list
    [HideInInspector] public int[] inventoryListCapacityIntArray;

    [SerializeField] private SO_ItemList itemList = null;


    // Awake will run before start! This way this class and Item don't have to fight over accessing this dictionary. If this goes in Start() instead, 
    // we would get a null reference exception
    protected override void Awake()
    {
        base.Awake();

        // Create inventory lists
        CreateInventoryLists();

        // Create item details dictionary
        CreateItemDetailsDictionary();

        // Initialize the selected inventory item array, of size equal to the number of inventory lcations (player, chest), and each item at -1 (not selected)
        selectedInventoryItem = new int[(int)InventoryLocation.count];

        for (int i = 0; i < selectedInventoryItem.Length; i++)
        {
            selectedInventoryItem[i] = -1;
        }
    }


    private void CreateInventoryLists()
    {   
        // Array of inventory lists of length (number of inventory locations - equal to the enum.count). Currently only 2: player and chest
        inventoryLists = new List<InventoryItem>[(int)InventoryLocation.count];

        // Create an inventory list of at each index (player and count)
        for (int i = 0; i < (int)InventoryLocation.count; i++)
        {
            inventoryLists[i] = new List<InventoryItem>();
        }

        // Initialize inventory list capacity array
        inventoryListCapacityIntArray = new int[(int)InventoryLocation.count];

        // Initialize player inventory list capacity (each index is the capacity of that inventory location: player, chest)
        inventoryListCapacityIntArray[(int)InventoryLocation.player] = Settings.playerInitialInventoryCapacity;
    }


    /// <summary>
    /// Populates the itemDetailsDictionary from the scriptable object items list
    /// </summary>
    private void CreateItemDetailsDictionary()
    {
        itemDetailsDictionary = new Dictionary<int, ItemDetails>();

        // Loop through each of the item details options (code, name, sprite, description, etc) in each item in the SO item list
        foreach (ItemDetails itemDetails in itemList.itemDetails)
        {
            // For each item in the item list, populate the dictionary with the item code as a key, and the details as a value
            itemDetailsDictionary.Add(itemDetails.itemCode, itemDetails);
        }
    }


    /// <summary>
    /// Add an item to the inventory list for that inventory location, and then destroy the gameObject! This is an overloaded method!
    /// </summary>
    public void AddItem(InventoryLocation inventoryLocation, Item item, GameObject gameObjectToDelete)
    {
        AddItem(inventoryLocation, item);

        Destroy(gameObjectToDelete);
    }


    /// <summary>
    /// Add an item to the inventory list for that inventory location. This is an overloaded method!
    /// </summary>
    public void AddItem(InventoryLocation inventoryLocation, Item item)
    {
        int itemCode = item.ItemCode;
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];

        // Check if the inventory already contains the item (returns position of the item, or -1 if it doesn't exist)
        int itemPosition = FindItemInInventory(inventoryLocation, itemCode);

        // If it is in the inventory, add another one to the same position. If not, create a brand new one 
        if (itemPosition != -1)
        {
            AddItemAtPosition(inventoryList, itemCode, itemPosition);
        }
        else
        {
            AddItemAtPosition(inventoryList, itemCode);
        }

        // Send event that inventory has been updated
        EventHandler.CallInventoryUpdatedEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);
    }


    /// <summary>
    /// Add item to the end of the inventory list. This is an overloaded method!
    /// </summary>
    public void AddItemAtPosition(List<InventoryItem> inventoryList, int itemCode)
    {
        //InventoryItem type is a struct that stores the itemCode, and the quantity held
        InventoryItem inventoryItem = new InventoryItem();
        
        // Fill the struct with the item code, and set it to 1 held, then add the struct to the end of the inventory list
        inventoryItem.itemCode = itemCode;
        inventoryItem.itemQuantity = 1;
        inventoryList.Add(inventoryItem);

        // Print out the entire inventory
        // DebugPrintInventoryList(inventoryList);
    }


    /// <summary>
    /// Add item to a specific position in the inventory list. This is an overloaded method!
    /// </summary>
    public void AddItemAtPosition(List<InventoryItem> inventoryList, int itemCode, int position)
    {
        //InventoryItem type is a struct that stores the itemCode, and the quantity held
        InventoryItem inventoryItem = new InventoryItem();
        
        // Add 1 to the existing quantity of this item code at the given index
        int quantity = inventoryList[position].itemQuantity + 1;

        // Fill the struct with the item code, and the new quantity, then add the struct to the correct location in the inventory list
        inventoryItem.itemCode = itemCode;
        inventoryItem.itemQuantity = quantity;
        inventoryList[position] = inventoryItem;

        // Print out the entire inventory
        // DebugPrintInventoryList(inventoryList);
    }


    /// <summary>
    /// Swap item at fromItem index with item at toItem index in inventoryLocation inventory list
    /// </summary>
    public void SwapInventoryItems(InventoryLocation inventoryLocation, int fromItem, int toItem)
    {
        // If fromItem index and toItem index are within the bounds of the list (so if we have two items, we can't place it at slot > 3!), 
        // and are not the same, and are greater to or equal to zero
        if (fromItem < inventoryLists[(int)inventoryLocation].Count && toItem < inventoryLists[(int)inventoryLocation].Count && fromItem != toItem && fromItem >= 0 && toItem >= 0)
        {
            // InventoryItem is a struct containing the item code and quantity. Here we set the current from and to items to the structs at those positions
            InventoryItem fromInventoryItem = inventoryLists[(int)inventoryLocation][fromItem];
            InventoryItem toInventoryItem = inventoryLists[(int)inventoryLocation][toItem];

            // And then we set the from to the to, and the to to the from
            inventoryLists[(int)inventoryLocation][toItem] = fromInventoryItem;
            inventoryLists[(int)inventoryLocation][fromItem] = toInventoryItem;

            // Finally, send an event to the event handler that the inventory has been updated, which will alert the subscriber at UIInventoryBar to update the UI
            EventHandler.CallInventoryUpdatedEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);
        }
    }


    /// <summary>
    /// Clear the selected inventory item for inventoryLocation
    /// </summary>
    public void ClearSelectedInventoryItem(InventoryLocation inventoryLocation)
    {
        selectedInventoryItem[(int)inventoryLocation] = -1;
    }


    /// <summary>
    /// Find if an itemCode is already in the inventory.static Returns the item position
    /// in the inventory list, or -1 if the item is not in the inventory
    /// </summary>
    public int FindItemInInventory(InventoryLocation inventoryLocation, int itemCode)
    {
        // Inventory list is a list of inventory lists, 0 is the players inventory, 1 is the chest
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];

        // Loop through the inventory list to see if the item code is equal to any of them
        for (int i = 0; i < inventoryList.Count; i++)
        {
            if (inventoryList[i].itemCode == itemCode)
            {
                return i;
            }
        }

        return -1;
    }


    /// <summary>
    /// Returns the itemDetails from the SO_ItemList for the itemCode, or null if the item code doesn't exist
    /// </summary>
    public ItemDetails GetItemDetails(int itemCode)
    {
        ItemDetails itemDetails;

        // Check if the given item code is in the dictionary. If so, return the details. If not, return null.
        if (itemDetailsDictionary.TryGetValue(itemCode, out itemDetails))
        {
            return itemDetails;
        }
        else
        {
            return null;
        }
    }


    /// <summary>
    /// Get the item type description for a given itemType (enum with all item types) - returns the item type description as a string for a given itemType
    /// </summary>
    public string GetItemTypeDescription(ItemType itemType)
    {
        string itemTypeDescription;

        // Check which ItemType enum option the given item type is, and return the item type description. The tools have overridden descriptions
        // in the Settings class, while the other options (e.g. seed, commodity, ...) are explicitly described in the ItemType enum 
        switch (itemType)
        {
            case ItemType.Breaking_tool:
                itemTypeDescription = Settings.BreakingTool;
                break;
            
            case ItemType.Chopping_tool:
                itemTypeDescription = Settings.ChoppingTool;
                break;

            case ItemType.Hoeing_tool:
                itemTypeDescription = Settings.HoeingTool;
                break;

            case ItemType.Reaping_tool:
                itemTypeDescription = Settings.ReapingTool;
                break;

            case ItemType.Watering_tool:
                itemTypeDescription = Settings.WateringTool;
                break;

            case ItemType.Collecting_tool:
                itemTypeDescription = Settings.CollectingTool;
                break;

            default:
                itemTypeDescription = itemType.ToString();
                break;
        }

        return itemTypeDescription;
    }


    /// <summary>
    /// Remove an item from the inventory, and create a gameObject at the position it was dropped at
    /// </summary>
    public void RemoveItem(InventoryLocation inventoryLocation, int itemCode)
    {
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];

        // Check if the inventory already contains the item (returns -1 if it isn't there!)
        int itemPosition = FindItemInInventory(inventoryLocation, itemCode);

        // If it does exist, remove the item at that position
        if (itemPosition != -1)
        {
            RemoveItemAtPosition(inventoryList, itemCode, itemPosition);
        }

        // Send event that inventory has been updated for subscribers to update with
        EventHandler.CallInventoryUpdatedEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);
    }


    private void RemoveItemAtPosition(List<InventoryItem> inventoryList, int itemCode, int position)
    {
        // Inventory item is a struct containing the item code and quantity
        InventoryItem inventoryItem = new InventoryItem();

        // The new quantity after removing one of them
        int quantity = inventoryList[position].itemQuantity - 1;

        // If there still are some left, adjust the quantity. If not, remove it entirely from the inventory
        if (quantity > 0)
        {
            inventoryItem.itemQuantity = quantity;
            inventoryItem.itemCode = itemCode;
            inventoryList[position] = inventoryItem;
        }
        else
        {
            inventoryList.RemoveAt(position);
        }
    }


    /// <summary>
    /// Set the selected inventory item for inventoryLocation to itemCode
    /// </summary>
    public void SetSelectedInventoryItem(InventoryLocation inventoryLocation, int itemCode)
    {
        selectedInventoryItem[(int)inventoryLocation] = itemCode;
    }


    // private void DebugPrintInventoryList(List<InventoryItem> inventoryList)
    // {
    //     // Loop through the passed in inventoryList, and print out the item details from GetItemDetails method, with the inventoryItem struct item code, 
    //     // and print out the quantity in the struct 
    //     foreach (InventoryItem inventoryItem in inventoryList)
    //     {
    //         Debug.Log("Item Description: " + InventoryManager.Instance.GetItemDetails(inventoryItem.itemCode).itemDescription + "    Item Quantity: " + inventoryItem.itemQuantity);
    //     }
    //     Debug.Log("**********************************************************************************");
    // }
}
