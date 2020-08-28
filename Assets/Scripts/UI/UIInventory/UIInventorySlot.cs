using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Each inventory slot has a UIInventorySlot object, populated in the editor with the slot highlight, image, text, blank item prefab, and its slot number
// IBeginDragHandler, IDragHandler, and IEndDragHandler interfaces are Unity classes that manage when dragging begins, during dragging, and when dragging ends,
// with the OnBeginDrag(), OnDrag(), and OnEndDrag() methods
// IPointerEnterHandler, and IPointerExitHandler are triggered when the pointer enters and exits a game object
// with the OnPointerEnter() and OnPointerExit() methods
// IPointerClickHandler is triggered when the user clicks on the game object (an inventory slot) with the OnPointerClic method
public class UIInventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Camera mainCamera;

    private Canvas parentCanvas;

    private Transform parentItem;
    private GameObject draggedItem;

    public Image inventorySlotHighlight;
    public Image inventorySlotImage;
    public TextMeshProUGUI textMeshProUGUI;

    [SerializeField] private UIInventoryBar inventoryBar = null;
    [SerializeField] private GameObject itemPrefab = null;

    [SerializeField] private GameObject inventoryTextBoxPrefab = null;

    [HideInInspector] public bool isSelected = false;

    [HideInInspector] public ItemDetails itemDetails;
    [HideInInspector] public int itemQuantity;

    [SerializeField] private int slotNumber = 0;


    private void Awake()
    {
        // For when we create our textbox gameObject, we will parent it under this parent canvas to the inventory slot (which is the UIInventoryBar gameObject)
        parentCanvas = GetComponentInParent<Canvas>();
    }


    private void Start()
    {
        mainCamera = Camera.main;
        parentItem = GameObject.FindGameObjectWithTag(Tags.ItemsParentTransform).transform;
    }


    /// <summary>
    /// Sets this inventorySlot item to be selected
    /// </summary>
    private void SetSelectedItem()
    {
        // clear the currently highlighted items
        inventoryBar.ClearHighlightOnInventorySlots();

        // Highlight the item on the inventory bar
        isSelected = true;

        // Set highlighted inventory slots
        inventoryBar.SetHighlightedInventorySlots();

        // Set item selected in inventory list
        InventoryManager.Instance.SetSelectedInventoryItem(InventoryLocation.player, itemDetails.itemCode);
    }


    private void ClearSelectedItem()
    {
        // Clear the currently highlighted items
        inventoryBar.ClearHighlightOnInventorySlots();

        isSelected = false;

        // Set no item selected in the inventory list
        InventoryManager.Instance.ClearSelectedInventoryItem(InventoryLocation.player);
    }


    /// <summary>
    /// Drops the item (if selected) at the current mouse position, called by the DropItem event 
    /// </summary>
    private void DropSelectedItemAtMousePosition()
    {
        // Only drop the item if it exists, and if it is selected!
        if (itemDetails != null && isSelected)
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

            // If we are dropping the last item in a stack, clear the selected highlight
            if (InventoryManager.Instance.FindItemInInventory(InventoryLocation.player, item.ItemCode) == -1)
            {
                ClearSelectedItem();
            }
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

            // Set the item to be selected
            SetSelectedItem();
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
                // Get the slot number corresponding to where the drag ended. 
                // PointerRayCast gets the object below your mouse, and we are getting the UIInventory slot component, which has a slotNumber variable (populated in editor)
                int toSlotNumber = eventData.pointerCurrentRaycast.gameObject.GetComponent<UIInventorySlot>().slotNumber;

                // Swap the inventory items in the inventory list from current slot number to final slot number
                InventoryManager.Instance.SwapInventoryItems(InventoryLocation.player, slotNumber, toSlotNumber);

                // Destroy the inventory text box!
                DestroyInventoryTextBox();

                // Clear the selected item if we've swapped!
                ClearSelectedItem();
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


    // This method is triggered then the player clicks on the GameObject (the inventory slot)
    public void OnPointerClick(PointerEventData eventData)
    {
        // If left click
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // If inventory slot currently selected, then deselect. If it isn't, then select it
            if (isSelected == true)
            {
                ClearSelectedItem();
            }
            else
            {
                // Only if there is an item in that inventory slot!
                if (itemQuantity > 0)
                {
                    SetSelectedItem();
                }
            }
        }
    }


    // This method will be triggered when the users pointer enters the game object this script is on (inventorySlots 0...11)
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Populate the text box with the item details, as long as there is an actual item there (i.e. if itemQuantity is not 0)
        if (itemQuantity != 0)
        {
            // Instantiate the inventoryBar's inventoryTextBoxGameObject with the text box prefab, parented to the same parent all the slot UIs live under!
            inventoryBar.inventoryTextBoxGameObject = Instantiate(inventoryTextBoxPrefab, transform.position, Quaternion.identity);
            inventoryBar.inventoryTextBoxGameObject.transform.SetParent(parentCanvas.transform, false);

            UIInventoryTextBox inventoryTextBox = inventoryBar.inventoryTextBoxGameObject.GetComponent<UIInventoryTextBox>();

            // Set the item type description
            string itemTypeDescription = InventoryManager.Instance.GetItemTypeDescription(itemDetails.itemType);

            // Populate the text box with the method in UIInventoryTextBox, taking in top1, top2, top3, bottom1, bottom2, bottom3
            inventoryTextBox.SetTextBoxText(itemDetails.itemDescription, itemTypeDescription, "", itemDetails.itemLongDescription, "", "");

            // Set text box position according to inventory bar position
            if (inventoryBar.IsInventoryBarPositionBottom)
            {
                // If the inventory bar is at the bottom, set the pivot to the bottom of the text box, and 50 pixels above the inventory bar
                inventoryBar.inventoryTextBoxGameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0f);
                inventoryBar.inventoryTextBoxGameObject.transform.position = new Vector3(transform.position.x, transform.position.y + 50f, transform.position.z);
            }
            else
            {
                // If the inventory bar is at the top, set the pivot to the top of the text box, and 50 pixels below the inventory bar
                inventoryBar.inventoryTextBoxGameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);
                inventoryBar.inventoryTextBoxGameObject.transform.position = new Vector3(transform.position.x, transform.position.y - 50f, transform.position.z);
            }
        }
    }


    // This method will be triggered when the users pointer exits the game object this script is on (inventorySlots 0...11)
    public void OnPointerExit(PointerEventData eventData)
    {
        // Destroy the created text box once the mouse leaves!
        DestroyInventoryTextBox();
    }


    public void DestroyInventoryTextBox()
    {
        if (inventoryBar.inventoryTextBoxGameObject != null)
        {
            Destroy(inventoryBar.inventoryTextBoxGameObject);
        }
    }
}
