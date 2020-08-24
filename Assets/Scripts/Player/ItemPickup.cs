using UnityEngine;

public class ItemPickup : MonoBehaviour
{   
    // Gets called by unity every time the player collides with an item with a trigger collider
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Grab the item component of the object we collided with (may be null if it's not an item!)
        Item item = collision.GetComponent<Item>();

        // Check if it was indeed an item! 
        if (item != null)
        {
            // Then get it's item details from InventoryManager if it is an item
            // InventoryManager inherits from SingletonMonobehavious, so .Instance gets the vars/methods.
            // The GetItemDetails is a method that returns the item details object given an item code
            ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(item.ItemCode);

            // Add the item to the players inventory, if the item can be picked up (pass in the players inventory location, the item in question, and the game object for destroying)
            if (itemDetails.canBePickedUp == true)
            {
                InventoryManager.Instance.AddItem(InventoryLocation.player, item, collision.gameObject);
            }

        }

    }
}
