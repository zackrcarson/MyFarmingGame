using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : SingletonMonobehaviour<InventoryManager>
{
    // Create a dictionary for inventory, with itemCode : itemDetails. This will be fast to access
    private Dictionary<int, ItemDetails> itemDetailsDictionary;

    [SerializeField] private SO_ItemList itemList = null;

    private void Start()
    {
        // Create item details dictionary
        CreateItemDetailsDictionary();
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
}
