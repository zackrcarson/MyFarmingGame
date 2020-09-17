using System.Collections.Specialized;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// This will be placed on every Pause menu Inventory slot, and it handles the dragging of items to and from this inventory slot (the first three
// interfaces below, and the mouse hover functionality to display item details (the last two interfaces)
public class PauseMenuInventoryManagementSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    // inventory slot image to be populated with the item sprite, the TMP to display the item quantity, and the greyed out image to control whether we can access
    // the slot or not
    public Image inventoryManagementSlotImage;
    public TextMeshProUGUI textMeshProUGUI;
    public GameObject greyedOutImageGO;

    // Populated in the editor with the PauseMenuInventoryManagementSlot, and the textbox prefab we created earlier to display item details as you hover over them
    [SerializeField] private PauseMenuInventoryManagement inventoryManagement = null;
    [SerializeField] private GameObject inventoryTextBoxPrefab = null;

    // item details and quantity for the item in the current slot
    [HideInInspector] public ItemDetails itemDetails;
    [HideInInspector] public int itemQuantity;

    // The slot number of the slot this script is placed on
    [SerializeField] private int slotNumber = 0;

    // GameObject placeholder for dragged items, and the parent canvas this exists on
    public GameObject draggedItem;
    private Canvas parentCanvas;


    private void Awake()
    {
        // For when we create our textbox gameObject, we will parent it under this parent canvas to the inventory slot (which is the UIInventoryBar gameObject)
        parentCanvas = GetComponentInParent<Canvas>();
    }


    // This will start as the player begins to drag an item
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemQuantity != 0)
        {
            // Instantiate gameObject as a dragged item. inventoryManagementDraggedItemPrefab is populated as the prefab for dragged items
            draggedItem = Instantiate(inventoryManagement.inventoryManagementDraggedItemPrefab, inventoryManagement.transform);

            // Set the currently null image to the sprite for the dragged item we want
            Image draggedItemImage = draggedItem.GetComponentInChildren<Image>();
            draggedItemImage.sprite = inventoryManagementSlotImage.sprite;
        }
    }


    // This method will control how the item is dragged
    public void OnDrag(PointerEventData eventData)
    {
        // Move the dragged object with the mouse!
        if (draggedItem != null)
        {
            draggedItem.transform.position = Input.mousePosition;
        }
    }


    // This method controls what happens when we release a dragged item to another inventory slot
    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggedItem != null)
        {
            // Destroy the dragged item gameObject
            Destroy(draggedItem);

            // If the drag ends over the inventory bar, and on top of another PauseMenuInventoryManagementSlot
            if (eventData.pointerCurrentRaycast.gameObject != null && eventData.pointerCurrentRaycast.gameObject.GetComponent<PauseMenuInventoryManagementSlot>() != null)
            {
                // Get the slot number corresponding to where the drag ended. 
                // PointerRayCast gets the object below your mouse, and we are getting the UIInventory slot component, which has a slotNumber variable (populated in editor)
                int toSlotNumber = eventData.pointerCurrentRaycast.gameObject.GetComponent<PauseMenuInventoryManagementSlot>().slotNumber;

                // Swap the inventory items in the inventory list from current slot number to final slot number
                InventoryManager.Instance.SwapInventoryItems(InventoryLocation.player, slotNumber, toSlotNumber);

                // Destroy the inventory text box!
                inventoryManagement.DestroyInventoryTextBoxGameObject();
            }
        }
    }


    // This method will be triggered when the users pointer enters the game object this script is on (inventorySlots 0...47)
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Populate the text box with the item details, as long as there is an actual item there (i.e. if itemQuantity is not 0)
        if (itemQuantity != 0)
        {
            // Instantiate the inventoryBar's inventoryTextBoxGameObject with the text box prefab, parented to the same parent all the slot UIs live under!
            inventoryManagement.inventoryTextBoxGameObject = Instantiate(inventoryTextBoxPrefab, transform.position, Quaternion.identity);
            inventoryManagement.inventoryTextBoxGameObject.transform.SetParent(parentCanvas.transform, false);

            UIInventoryTextBox inventoryTextBox = inventoryManagement.inventoryTextBoxGameObject.GetComponent<UIInventoryTextBox>();

            // Set the item type description
            string itemTypeDescription = InventoryManager.Instance.GetItemTypeDescription(itemDetails.itemType);

            // Populate the text box with the item descriptions, using the method in UIInventoryTextBox, taking in top1, top2, top3, bottom1, bottom2, bottom3
            inventoryTextBox.SetTextBoxText(itemDetails.itemDescription, itemTypeDescription, "", itemDetails.itemLongDescription, "", "");

            // Set text box position according the row of items we are accessing
            if (slotNumber > 23)
            {
                // If we are checking items in the bottom two rows, set the pivot to the bottom of the text box, and 50 pixels above the inventory bar
                inventoryManagement.inventoryTextBoxGameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0f);
                inventoryManagement.inventoryTextBoxGameObject.transform.position = new Vector3(transform.position.x, transform.position.y + 50f, transform.position.z);
            }
            else
            {
                // If we are checking items in the top two rows, set the pivot to the top of the text box, and 50 pixels below the inventory bar
                inventoryManagement.inventoryTextBoxGameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);
                inventoryManagement.inventoryTextBoxGameObject.transform.position = new Vector3(transform.position.x, transform.position.y - 50f, transform.position.z);
            }
        }
    }


    // This method will be triggered when the users pointer exits the game object this script is on (inventorySlots 0...11)
    public void OnPointerExit(PointerEventData eventData)
    {
        // Destroy the created text box once the mouse leaves!
        inventoryManagement.DestroyInventoryTextBoxGameObject();
    }
}
