using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

// This class subscribes to the ISaveable interface, which means we must include several methods for saving/loading data (here, we will save the players inventory)
public class InventoryManager : SingletonMonobehaviour<InventoryManager>, ISaveable
{
    // Get a reference to the inventoryBar so we can deselect any selected items when we load the inventory
    private UIInventoryBar inventoryBar;

    // Create a dictionary for inventory, with itemCode : itemDetails. This will be fast to access
    private Dictionary<int, ItemDetails> itemDetailsDictionary;

    // The index of the array is the inventory list location (player, chest), and the value is the item code of the selected item (so they can still swap)
    private int[] selectedInventoryItem;

    // InventoryItem is a struct that stores the item code, and the number held
    public List<InventoryItem>[] inventoryLists;

    // The index of the array is the inventory list location (from the InventoryLocation enum), and the value is the capacity of that inventory list
    [HideInInspector] public int[] inventoryListCapacityIntArray;

    [SerializeField] private SO_ItemList itemList = null;

    // Unique ID required by the ISaveable interface, will store the GUID attached to the InventoryManager gameObject
    private string _iSaveableUniqueID;
    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }

    // GameObjectSave required by the ISaveable interface, storesd the save data that is built up for every object that has the ISaveable interface attached
    private GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }


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

        // Get the unique ID for the GameObject
        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;

        // Initialize the GameObjectSave variable
        GameObjectSave = new GameObjectSave();
    }


    // On enable, this will just register this gameObject as an ISaveable, so that the SaveLoadManager can save/load the methods set up here
    private void OnEnable()
    {
        // Registers this game object within the iSaveableObjectList, which is looped through in the SaveLoadManager for all objects to save/load the saved items
        ISaveableRegister();
    }


    // Deregister from the iSaveableObjectList
    private void OnDisable()
    {
        // Deregisters this game object within the iSaveableObjectList, which is looped through in the SaveLoadManager for all objects to save/load the saved items
        ISaveableDeregister();
    }


    // On Start, find the UIInventoryBar
    private void Start()
    {
        // Find the UIInventoryBar, and populate it so we can access it later (to deselect selected items on load)
        inventoryBar = FindObjectOfType<UIInventoryBar>();
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
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation]; // I added this and the next if statement so that we can't pickup items if we already have the current max allowed items (we can pick up if we currently have the item, and will be adding to the quantity)
        if (inventoryListCapacityIntArray[(int)InventoryLocation.player] > inventoryList.Count || FindItemInInventory(inventoryLocation, item.ItemCode) != -1) 
        {
            AddItem(inventoryLocation, item);

            Destroy(gameObjectToDelete);
        }
    }


    /// <summary>
    /// Add an item of type itemCode to the inventory list for the inventoryLocation
    /// </summary>
    public void AddItem(InventoryLocation inventoryLocation, int itemCode)
    {
        // Find the inventory list for this inventory location (i.e. player, chest, etc.)
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];

        // Check if the inventory at this inventoryLocation already contains the item with itemCode (-1 if not)
        int itemPosition = FindItemInInventory(inventoryLocation, itemCode);

        if (itemPosition != -1)
        {
            // If it does exist, add another one to that itemPosition
            AddItemAtPosition(inventoryList, itemCode, itemPosition);
        }
        else
        {
            // If it doesn't exist, add this item lone into the next available inventory slot
            AddItemAtPosition(inventoryList, itemCode);
        }

        // Send an event that the inventory has been updated - so subscribers to take the update into account
        EventHandler.CallInventoryUpdatedEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);
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
    /// Return the itemDetails from the SO_ItemList for the currently selected item in the inventoryLocation, or null if no item is selected
    /// </summary>
    public ItemDetails GetSelectedInventoryItemDetails(InventoryLocation inventoryLocation)
    {
        int itemCode = GetSelectedInventoryItem(inventoryLocation);

        if (itemCode == -1)
        {
            return null;
        }
        else
        {
            return GetItemDetails(itemCode);
        }
    }


    /// <summary>
    /// Get the selected item for inventoryLocation - return the selected itemCode or -1 if nothing is selected
    /// </summary>
    private int GetSelectedInventoryItem(InventoryLocation inventoryLocation)
    {
        return selectedInventoryItem[(int)inventoryLocation];
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


    // Required method by the ISaveable interface, which will be called OnEnable() of the InventoryManager GameObject, and it will 
    // Add an entry (of this gameObject) to the iSaveableObjectList in SaveLoadManager, which will then manage
    // Looping through all such items in this list to save/load their data
    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }


    // Required method by the ISaveable interface, which will be called OnDisable() of the InventoryManager GameObject, and it will
    // Remove this item from the saveable objects list, as described above
    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }


    // Required method by the ISaveable interface. This will get called from the SaveLoadManager, for each scene to save the dictionaries (GameObjectSave has a dict keyed by scene name)
    // This method will store the sceneData for the current scene (populating the listInvItemArray with the players inventory list, and the intArrayDictionary with the 
    // inventory list capacity arrays. It will then return a GameObjectSave, which just has a Dict of SceneSave data for each scene, keyed by scene name
    public GameObjectSave ISaveableSave()
    {
        // Create the SaveScene for this gameObject (keyed by the scene name, storing multiple dicts for bools, the scene the player ended in, the players location, the gridPropertyDetails,
        // the SceneItems, and the inventory items and quantities)
        SceneSave sceneSave = new SceneSave();

        // Delete the sceneData (dict of data to save in that scene, keyed by scene name) for the GameObject if it already exists in the persistent scene
        // which is where this data is going to be saved, so we can create a new one with updated dictionaries
        GameObjectSave.sceneData.Remove(Settings.PersistentScene);

        // Add inventory lists array to the persistent sceneSave
        sceneSave.listInvItemArray = inventoryLists;

        // Add inventory list capacity array to the persistent scene save, which stores how many items each inventory location can store (i.e. backpack, chest, etc)
        sceneSave.intArrayDictionary = new Dictionary<string, int[]>();
        sceneSave.intArrayDictionary.Add("inventoryListCapacityArray", inventoryListCapacityIntArray);

        // Add the SceneSave data for the InventoryManager game object to the GameObjectSave, which is a dict storing all the dicts in a scene to be loaded/saved, keyed by the scene name
        // The inventory manager will get stored in the Persistent Scene
        GameObjectSave.sceneData.Add(Settings.PersistentScene, sceneSave);

        // Return the GameObjectSave, which has a dict of the Saved stuff for the InventoryManager GameObject
        return GameObjectSave;
    }


    // This is a required method for the ISaveable interface, which passes in a GameObjectSave dictionary, and restores the current scene from it
    // The SaveLoadManager script will loop through all of the ISaveableRegister GameObjects (all registered with their ISaveableRegister methods), and trigger this 
    // ISaveableLoad, which will load that Save data (here for the persistent scene inventory information, which includes a list of inventory items, and
    // an int[] dict for the different inventory location capacities), for each scene (GameObjectSave is a Dict keyed by scene name)
    public void ISaveableLoad(GameSave gameSave)
    {
        // gameSave stores a Dictionary of items to save keyed by GUID, see if there's one for this GUID (generated on the InventoryManager GameObject)
        if (gameSave.gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            // Get the save data for the scene, if one exists for the PersistentScene (what the inventory info is saved under)
            if (gameObjectSave.sceneData.TryGetValue(Settings.PersistentScene, out SceneSave sceneSave))
            {
                // Get the inventory item array dictionary, if the SceneSave listInvItemArray exists in the persistent scene
                if (sceneSave.listInvItemArray != null)
                {
                    // Saved inventory list
                    inventoryLists = sceneSave.listInvItemArray;

                    // Send events that the inventory has been updated for each inventory location
                    for (int i = 0; i < (int)InventoryLocation.count; i++)
                    {
                        EventHandler.CallInventoryUpdatedEvent((InventoryLocation)i, inventoryLists[i]);
                    }

                    // Clear out any items the player migth have been carrying
                    Player.Instance.ClearCarriedItem();

                    // Clear out any highlights on the inventory bar
                    inventoryBar.ClearHighlightOnInventorySlots();
                }

                // Get the array of inventory capacities (i.e. for backpack, chest, etc) if it exists, and contains the "inventoryListCapacityArray" key.
                if (sceneSave.intArrayDictionary != null && sceneSave.intArrayDictionary.TryGetValue("inventoryListCapacityArray", out int[] inventoryCapacityArray))
                {
                    // Reset the current array of inventory capacities with the saved one
                    inventoryListCapacityIntArray = inventoryCapacityArray;   
                }
            }
        }
    }


    // Required method by the ISaveable interface, which will store all of the scene data, executed for every item in the iSaveableObjectList. This let's us walk between
    // scenes and keep the stored stuff active with ISaveableRestoreScene 
    public void ISaveableStoreScene(string sceneName)
    {
        // Nothing to store here since the InventoryManager is on a persistent scene - it won't get reset ever because we always stay on that scene
    }


    // Required method by the ISaveable interface, which will restore all of the scene data, executed for every item in the iSaveableObjectList. This let's us walk between
    // scenes and keep the stored stuff active with ISaveableRestoreScene 
    public void ISaveableRestoreScene(string sceneName)
    {   
        // Nothing to restore here since the InventoryManager is on a persistent scene - it won't get reset ever because we always stay on that scene
    }
}
