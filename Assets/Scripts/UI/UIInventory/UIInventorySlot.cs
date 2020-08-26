using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIInventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Camera mainCamera;
    private Transform parentItem;
    private GameObject draggedItem;

    public Image inventorySlotHighlight;
    public Image inventorySlotImage;
    public TextMeshProUGUI textMeshProUGUI;

    [SerializeField] private UIInventoryBar inventoryBar = null;
    [SerializeField] private GameObject itemPrefab = null;

    [HideInInspector] public ItemDetails itemDetails;
    [HideInInspector] public int itemQuantity;

    private void Start()
    {
        mainCamera = Camera.main;
        parentItem = GameObject.FindGameObjectWithTag(Tags.ItemsParentTransform).transform;
    }


    /// <summary>
    /// Drops the item (if selected) at the current mouse position, called by the DropItem event 
    /// </summary>
    private void DropSelectedItemAtMousePosition()
    {
        if (itemDetails != null)
        {
            // Vector to mouse position in the world coordinates, as converted from the screen viewport coordinates 
            // (the cameras position is at "-10" z position. We want the item to be created at the opposite!)
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));

            // Create the item from prefab at the mouse position (itemPrefab was populated in the editor with the draggedItem Prefab)
            GameObject itemGameObject = Instantiate(itemPrefab, worldPosition, Quaternion.identity, parentItem);
            // Get the item details from the item in question, so we can set the item code
            Item item = itemGameObject.GetComponent<Item>();
            item.ItemCode = itemDetails.itemCode;

            // Remove the item from the players inventory
            InventoryManager.Instance.RemoveItem(InventoryLocation.player, item.ItemCode);
        }
    }


    // This will start as the player begins to drag an item
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemDetails != null)
        {
            // Disable keyboard input while dragging
            Player.Instance.DisablePlayerInputAndResetMovement();

            // Instantiate gameObject as a dragged item. inventoryBar is populated as the prefab for dragged items
            draggedItem = Instantiate(inventoryBar.inventoryBarDraggedItem, inventoryBar.transform);

            // Set the currently null image to the image for the dragged item we want
            Image draggedItemImage = draggedItem.GetComponentInChildren<Image>();
            draggedItemImage.sprite = inventorySlotImage.sprite;
        }
    }


    public void OnDrag(PointerEventData eventData)
    {
        // Move the dragged object with the mouse!
        if (draggedItem != null)
        {
            draggedItem.transform.position = Input.mousePosition;
        }
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggedItem != null)
        {
            // Destroy the gameObject as a dragged item
            Destroy(draggedItem);
        
            // If the drag ends over the inventory bar, get the item that was dragged over, and swap them
            if (eventData.pointerCurrentRaycast.gameObject != null && eventData.pointerCurrentRaycast.gameObject.GetComponent<UIInventorySlot>() != null)
            {
                // Do nothing now. Later add functionality to swap them
            } 
            // else attempt the item if it can be dropped
            else
            {
                if (itemDetails.canBeDropped)
                {
                    DropSelectedItemAtMousePosition();
                }
            }

            // Re-enable the player input
            Player.Instance.EnablePlayerInput();
        }
    }
}
