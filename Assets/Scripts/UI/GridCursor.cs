using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridCursor : MonoBehaviour
{
    // Canvas will be used to store the UI canvas that stores the cursor, grid is the tilemaps grid
    private Canvas canvas;
    private Grid grid;
    private Camera mainCamera;

    // cursor image, rectTransform, and sprites are populated in the editor
    [SerializeField] private Image cursorImage = null;
    [SerializeField] private RectTransform cursorRectTransform = null;
    [SerializeField] private Sprite greenCursorSprite = null;
    [SerializeField] private Sprite redCursorSprite = null;

    // Bool describing if the current cursor location is valid or not
    private bool _cursorPositionIsValid = false;
    public bool CursorPositionIsValid { get => _cursorPositionIsValid; set => _cursorPositionIsValid = value; }

    // The current item radius for dropping
    private int _itemUseGridRadius = 0;
    public int ItemUseGridRadius { get => _itemUseGridRadius; set => _itemUseGridRadius = value; }

    // The ItemType (enum - commodity, seed, hoeing_tool, etc) of the selected item
    private ItemType _selectedItemType;
    public ItemType SelectedItemType { get => _selectedItemType; set => _selectedItemType = value; }

    // Whether or not the cursor is enabled
    private bool _cursorIsEnabled = false;
    public bool CursorIsEnabled { get => _cursorIsEnabled; set => _cursorIsEnabled = value; }


    // Unsubscribe the SceneLoaded method from the AfterSceneLoaded event
    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= SceneLoaded;
    }


    // Subscribe the SceneLoaded method to the AfterSceneLoad event, to populate the grid variable
    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += SceneLoaded;
    }


    // Start is called BEFORE the first frame update. Here we will populate the camera and canvas variables
    private void Start()
    {
        mainCamera = Camera.main;
        canvas = GetComponentInParent<Canvas>();
    }


    // Update is called once per frame - just displaying the cursor (as long as it's enabled!)
    private void Update()
    {
        if (CursorIsEnabled)
        {
            DisplayCursor();
        }
    }

    
    // This method gets the grid position for the player/cursor, tests whether it's a valid position or not and set's the cursor color accordingle, 
    // Findes the rect transform for the cursor to be added to the canvas, and then returns the gridPosition.
    // This method is called every frame from Update()
    private Vector3Int DisplayCursor()
    {
        if (grid != null)
        {
            // Get the grid position for cursor
            Vector3Int gridPosition = GetGridPositionForCursor();

            // Get the grid position for player
            Vector3Int playerGridPosition = GetGridPositionForPlayer();

            // Set cursor sprite
            SetCursorValidity(gridPosition, playerGridPosition);

            // Get the rect transform position for the cursor, in pixels for the UI canvas
            cursorRectTransform.position = GetRectTransformPositionForCursor(gridPosition);

            return gridPosition;
        }
        else
        {
            return Vector3Int.zero;
        }
    }


    // This method is subscribed to AfterSceneLoaded event, so once that happens we can populate the grid variable
    private void SceneLoaded()
    {
        grid = GameObject.FindObjectOfType<Grid>();
    }


    // This method tests whether the current cursor position is valid given the selected item, the cursor grid position, and the player grid position
    private void SetCursorValidity(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
    {
        // Start off with a valid cursor (green)
        SetCursorToValid();

        // Check if the item use radius is invalid (distance from player larger than the itemUseRadius), if so, set the radius to invalid (red)
        if (Mathf.Abs(cursorGridPosition.x - playerGridPosition.x) > ItemUseGridRadius || Mathf.Abs(cursorGridPosition.y - playerGridPosition.y) > ItemUseGridRadius)
        {
            SetCursorToInvalid();
            return;
        }

        // Get selected item details (code, sprite, use radius, can be dropped, etc) for the selected inventory item
        ItemDetails itemDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.player);

        // If the item details are null, nothing is selected! Deactivate the cursor, and quit
        if (itemDetails == null)
        {
            SetCursorToInvalid();
            return;
        }

        // Get the grid property details (coordinate, diggable, can place item, etc.) at the cursor position
        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cursorGridPosition.x, cursorGridPosition.y);

        // If there exists grid property details in this square, leave the cursor as valid depending on the itemType, and if it's valid at that location.
        // Else, set it to invalid.
        if (gridPropertyDetails != null)
        {
            // Determine the cursor validity based on inventory item selected and the grid property details
            // itemDetails.itemType is the enum with seed, commodity, hoeing_tool, etc
            switch (itemDetails.itemType)
            {   
                // If the item is a seed, test to see if it's a valid cursor for a seed. If false, set cursor to invalid. If true, leave it as valid
                case ItemType.Seed:
                    if (!IsCursorValidForSeed(gridPropertyDetails))
                    {
                        SetCursorToInvalid();
                        return;
                    }
                    break;

                // Same thing for commodities
                case ItemType.Commodity:
                    if (!IsCursorValidForCommodity(gridPropertyDetails))
                    {
                        SetCursorToInvalid();
                        return;
                    }
                    break;

                // Same thing for the all of the different tools
                case ItemType.Watering_tool:
                case ItemType.Breaking_tool:
                case ItemType.Chopping_tool:
                case ItemType.Hoeing_tool:
                case ItemType.Reaping_tool:
                case ItemType.Collecting_tool:
                    if (!IsCursorValidForTool(gridPropertyDetails, itemDetails))
                    {
                        SetCursorToInvalid();
                        return;
                    }
                    break;

                // If it's none, count, or other, just exit out, leaving it as valid!
                case ItemType.none:
                    break;
                
                case ItemType.count:
                    break;

                default:
                    break;
            }
        }
        else
        {
            SetCursorToInvalid();
            return;
        }
    }


    /// <summary>
    /// Set the cursor to be invalid! Set the image sprite to the red cursor sprite
    /// </summary>
    private void SetCursorToInvalid()
    {
        cursorImage.sprite = redCursorSprite;
        CursorPositionIsValid = false;
    }


    /// <summary>
    /// Set the cursor to be valid! Set the image sprite to the green cursor sprite
    /// </summary>
    private void SetCursorToValid()
    {
        cursorImage.sprite = greenCursorSprite;
        CursorPositionIsValid = true;
    }


    /// <summary>
    /// Test cursor validity for a commodity for the given gridPropertyDetails. Returns true if it's valid, and false if it's invalid
    /// </summary>
    private bool IsCursorValidForCommodity(GridPropertyDetails gridPropertyDetails)
    {
        return gridPropertyDetails.canDropItem;
    }


    /// <summary>
    /// Test cursor validity for a seed for the given gridPropertyDetails. Returns true if it's valid, and false if it's invalid
    /// </summary>
    private bool IsCursorValidForSeed(GridPropertyDetails gridPropertyDetails)
    {
        return gridPropertyDetails.canDropItem;
    }


    /// <summary>
    /// Set's the cursor as either valid or invalid for the tool for the target gridPropertyDetails. Returns true if valid, and false if invalid
    /// </summary>
    private bool IsCursorValidForTool(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails)
    {
        // Switch on tool type, to test for the cases for different tools
        switch (itemDetails.itemType)
        {   
            // First, check if it's a hoeing tool
            case ItemType.Hoeing_tool:

                // First, the gridSquare in question needs to be diggable, and hasn't been dug yet (daysSinceDug = -1), if not, return false so it's an invalid cursor
                if (gridPropertyDetails.isDiggable == true && gridPropertyDetails.daysSinceDug == -1)
                {
                    #region Need to get any items at location so we can check if they are reapable

                    // Get the world position for the cursor (add half a unity unit in each direction to get the center)
                    Vector3 cursorWorldPosition = new Vector3(GetWorldPositionForCursor().x + 0.5f, GetWorldPositionForCursor().y + 0.5f, 0f);

                    // Get a list of the items at the cursor location
                    List<Item> itemList = new List<Item>();

                    // This helper method will look through the box passed on at the cursor position, and return true if items of type Item are found, false if not.
                    // The out parameter itemList is a list of all Items found in that box
                    HelperMethods.GetComponentsAtBoxLocation<Item>(out itemList, cursorWorldPosition, Settings.cursorSize, 0f);

                    #endregion

                    // Loop through the items in the itemList found to see if any are of reapable type - we are not going to let the player dig where there are reapable scenary items! 
                    // Need to scythe them first to dig
                    bool foundReapable = false;

                    foreach (Item item in itemList)
                    {
                        // Check if the itemType in the itemDetails is of type ReapableScenary
                        if (InventoryManager.Instance.GetItemDetails(item.ItemCode).itemType == ItemType.Reapable_scenary)
                        {
                            // If we've found even one, we can just break the loop and say we can't dig here.
                            foundReapable = true;
                            break;
                        }
                    }

                    // If there is a reapable item, return false so we can't dig there (red cursor), else return true
                    if (foundReapable)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    // This is if the ground square isn't diggable or has been dug - return a false so we can't dig
                    return false;
                }

            // Now, check if it's a watering tool
            case ItemType.Watering_tool:
                // Check if it's been dug, and not watered
                if (gridPropertyDetails.daysSinceDug > -1 && gridPropertyDetails.daysSinceWatered == -1)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            // All other tools will default to false for now, so invalid cursor position
            default:
                return false;
        }
    }


    // This is called in the UIInventorySlot.ClearCursors(), when an inventory slot item is no longer selected.
    // disable the cursor enabled bool, and set it to clear so you can't see it
    public void DisableCursor()
    {
        cursorImage.color = Color.clear;

        CursorIsEnabled = false;
    }


    // This is called in the UIInventorySlot.SetSelectedItem(), when an inventory slot item is selected, if the ItemUseRadius is > 0.
    // enable the cursor enabled bool, and set it to it's original color so you can see it
    public void EnableCursor()
    {
        cursorImage.color = new Color(1f, 1f, 1f, 1f);

        CursorIsEnabled = true;
    }


    // Return the grid position at where the cursor is (mouse position)
    public Vector3Int GetGridPositionForCursor()
    {   
        // ScreenToWorldPoint from the main camera will convert the mouse's position (where the cursor is) into a worldpoint
        // z is how far the objects are in front of the camera. Camera is at -10, so objects are at (-)-10 = 10 in front!
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));

        // WorldToCell will return the cell location on the grid given the actual worldPosition
        return grid.WorldToCell(worldPosition);
    }


    // Simply returns the current cursors world position, using the grid position from the GetGridPositionForCursor method
    public Vector3 GetWorldPositionForCursor()
    {   
        // Getting this from the gridPosition instead of directly allows us to get it from the origin at the bottom-left corner of the map
        return grid.CellToWorld(GetGridPositionForCursor());
    }


    // Retun the players position in grid coordinates
    public Vector3Int GetGridPositionForPlayer()
    {   
        // This will convert the players current world position into a call location on the grid 
        return grid.WorldToCell(Player.Instance.transform.position);
    }


    // This method gets the RectTransform pixel position for the cursor in the canvas
    public Vector2 GetRectTransformPositionForCursor(Vector3Int gridPosition)
    {   
        // Get the world position of the grid cell, converted from the gridPosition
        Vector3 gridWorldPosition = grid.CellToWorld(gridPosition);
        
        // Convert the world position into a screen position
        Vector2 gridScreenPosition = mainCamera.WorldToScreenPoint(gridWorldPosition);

        // This returns the pixel location on the canvas for the current cursor RectTransform on the canvas at screen position as found above
        return RectTransformUtility.PixelAdjustPoint(gridScreenPosition, cursorRectTransform, canvas);
    }
}