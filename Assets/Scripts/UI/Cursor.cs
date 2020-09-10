using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cursor : MonoBehaviour
{
    private Canvas canvas;
    private Camera mainCamera;

    // Fields to be populated in the editor, for the cursor image, it's RectTransform, its sprites, and the gridCursor instance (to enable and disable it)
    [SerializeField] private Image cursorImage = null;
    [SerializeField] private RectTransform cursorRectTransform = null;
    [SerializeField] private Sprite greenCursorSprite = null;
    [SerializeField] private Sprite transparentCursorSprite = null;
    [SerializeField] private GridCursor gridCursor = null;

    // Whether or not the cursor is enabled
    private bool _cursorIsEnabled = false;
    public bool CursorIsEnabled { get => _cursorIsEnabled; set => _cursorIsEnabled = value; }

    // Bool describing if the current cursor location is valid or not
    private bool _cursorPositionIsValid = false;
    public bool CursorPositionIsValid { get => _cursorPositionIsValid; set => _cursorPositionIsValid = value; }

    // The ItemType (enum - commodity, seed, hoeing_tool, etc) of the selected item
    private ItemType _selectedItemType;
    public ItemType SelectedItemType { get => _selectedItemType; set => _selectedItemType = value; }

    // The current item radius (now in unity Units, rather than grid squares) for using
    private float _itemUseRadius = 0f;
    public float ItemUseRadius { get => _itemUseRadius; set => _itemUseRadius = value; }

    
    private void Start()
    {
        // Get the main camera, and the canvas which the cursor will sit in
        mainCamera = Camera.main;
        canvas = GetComponentInParent<Canvas>();
    }


    // Display the cursor if it is enabled (i.e. an item is selected in the inventory bar)
    private void Update()
    {
        if (CursorIsEnabled)
        {
            DisplayCursor();
        }
    }


    //
    private void DisplayCursor()
    {
        // Get the world position for the cursor
        Vector3 cursorWorldPosition = GetWorldPositionForCursor();

        // Set the cursor sprite (check it's validity! green or transparent crosshairs), given the cursors world position, and the CENTER of the the 
        // players body position (so an additional y-offset from the feet pivot point! calculated in GetPlayerCenterPosition from the Player script. 
        // This will allow the scythe to operate from the center of the player as it should)
        SetCursorValidity(cursorWorldPosition, Player.Instance.GetPlayerCenterPosition());

        // Get the rect transform position (in pixel points) for the cursor in the canvas where it will be displayed
        cursorRectTransform.position = GetRectTransformPositionForCursor();
    }


    // This is the base method to determine whether or not a given cursor location is valid or not, given it's distance from the player,
    // The used items type, etc. The result is either a Valid Cursor, or an Invalid cursor.
    private void SetCursorValidity(Vector3 cursorPosition, Vector3 playerPosition)
    {
        // Begin by setting the cursor to valid as default, then we'll check if it's invalid and set it so through several tests
        SetCursorToValid();

        // Check the use radius corners. If the item is in one of these corner areas, the cursor is invalid!
        if (
            cursorPosition.x > (playerPosition.x + ItemUseRadius / 2f) && cursorPosition.y > (playerPosition.y + ItemUseRadius / 2f) 
            ||
            cursorPosition.x < (playerPosition.x - ItemUseRadius / 2f) && cursorPosition.y > (playerPosition.y + ItemUseRadius / 2f) 
            ||
            cursorPosition.x < (playerPosition.x - ItemUseRadius / 2f) && cursorPosition.y < (playerPosition.y - ItemUseRadius / 2f) 
            ||
            cursorPosition.x > (playerPosition.x + ItemUseRadius / 2f) && cursorPosition.y < (playerPosition.y - ItemUseRadius / 2f) 
            )
        {
            SetCursorToInvalid();
            return;
        }

        // Check if the item use radius is valid (outside of the cross-shaped valid region, bounded by a box). If it's outside of this box, the cursor is also not valid!
        if (Mathf.Abs(cursorPosition.x - playerPosition.x) > ItemUseRadius || Mathf.Abs(cursorPosition.y - playerPosition.y) > ItemUseRadius)
        {
            SetCursorToInvalid();
            return;
        }

        // Get the selected items details
        ItemDetails itemDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.player);

        // If there's no valid item Details for the item selected, the cursor is invalid of course!
        if (itemDetails == null)
        {
            SetCursorToInvalid();
            return;
        }

        // Determines the cursor validity based on the inventoryItem itemType selected, and what object the cursor is over (i.e. if it's a tool, commodity, etc)
        switch (itemDetails.itemType)
        {
            // If the selected item is any of our tool types, check in SetCursorValidityTool to see if the square in question is valid or not based on the tool it is
            case ItemType.Watering_tool:
            case ItemType.Breaking_tool:
            case ItemType.Chopping_tool:
            case ItemType.Hoeing_tool:
            case ItemType.Reaping_tool:
            case ItemType.Collecting_tool:
                // Check if the given tool has a valid position here, if not set the cursor to invalid. If yes, then we break out and leave it as a valid position!
                if (!SetCursorValidityTool(cursorPosition, playerPosition, itemDetails))
                {
                    SetCursorToInvalid();
                    return;
                }
                break;

            // If it's anything else, just break out with a valid cursor 
            case ItemType.none:  
                break;

            case ItemType.count:
                break;

            default:
                break;  
        }
    }


    /// <summary>
    /// Set the cursor to be valid (green crosshair sprite), which is the default in SetCursorValidity() before checking all the tests which may set it to invalid.
    /// </summary>
    private void SetCursorToValid()
    {   
        // Set the sprite to the green cross cursor, and set the Valid bool to true
        cursorImage.sprite = greenCursorSprite;
        CursorPositionIsValid = true;
        
        // Disable the grid cursor because now we are using a valid normal cursor
        gridCursor.DisableCursor();
    }


    /// <summary>
    /// Set the cursor to be invalid (transparent sprite), which is set to invalid in SetCursorValidity() if any of the tests fail the validity checks
    /// </summary>
    private void SetCursorToInvalid()
    {   
        // Set the sprite to the transparent cross cursor, and set the Valid bool to false
        cursorImage.sprite = transparentCursorSprite;
        CursorPositionIsValid = false;
        
        // enable the grid cursor because we are not using a valid normal cursor
        gridCursor.EnableCursor();
    }


    /// <summary>
    /// Set's the cursor as either valid or invalid for the tool given the target cursor location. Returns true if it's a valid spot, and false if it's invalid.
    /// </summary>
    private bool SetCursorValidityTool(Vector3 cursorPosition, Vector3 playerPosition, ItemDetails itemDetails)
    {
        // Switch on tool type to test for different tools
        switch (itemDetails.itemType)
        {   
            // For now we only check for reaping tools. Everything else will be false, so invalid cursor location
            case ItemType.Reaping_tool:
                // This method checks the validity for ReapingTools in particular, returning true if valid, and false if not
                return SetCursorValidityReapingTool(cursorPosition, playerPosition, itemDetails);

            default:
                return false;
        }
    }


    // Check if the current cursor location is a valid place to use the reaping tool! Returns true for valid, and false for invalid
    private bool SetCursorValidityReapingTool(Vector3 cursorPosition, Vector3 playerPosition, ItemDetails itemDetails)
    {
        List<Item> itemList = new List<Item>();

        // This method returns a bool - true if an item IS found beneath the current cursor location. It's out parameter is a list of all 
        // items found underneath the cursor
        if (HelperMethods.GetComponentsAtCursorLocation<Item>(out itemList, cursorPosition))
        {   
            if (itemList.Count != 0)
            {
                // Loop through all of the returned items in the list, and return true (keep the cursor as VALID) if ANY of them
                // have an itemType of ReapableScenary
                foreach (Item item in itemList)
                {
                    if (InventoryManager.Instance.GetItemDetails(item.ItemCode).itemType == ItemType.Reapable_scenary)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }


    // Called from the UIInventorySlot script, to disable the Cursor depending on if an item is selected or not
    public void DisableCursor()
    {
        // Set the image color to invisible, and set the cursor enabled bool to be false
        cursorImage.color = new Color(1f, 1f, 1f, 0f);
        CursorIsEnabled = false;
    }


    // Called from the UIInventorySlot script, to enable the Cursor depending on if an item is selected or not
    public void EnableCursor()
    {
        // Set the image color to visible, and set the cursor enabled bool to be true
        cursorImage.color = new Color(1f, 1f, 1f, 1f);
        CursorIsEnabled = true;
    }


    // Returns the world position of the cursor
    public Vector3 GetWorldPositionForCursor()
    {   
        // First grab the screen position from the mouse
        Vector3 screenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);

        // Then convert the screenPosition to a worlPosition via the following mainCamera method
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);

        return worldPosition;
    }


    // Returns the RectTransform for the cursor, to be displayed in the UI canvas
    public Vector2 GetRectTransformPositionForCursor()
    {   
        // First get the screen position from the mouse
        Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        // Return a pixel point position for the UI to display the cursor
        return RectTransformUtility.PixelAdjustPoint(screenPosition, cursorRectTransform, canvas);
    }
    
}
