using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : SingletonMonobehaviour<Player>
{
    // The pause after using the tool/lifting tool animation before we can use the tool or walk again
    private WaitForSeconds afterUseToolAnimationPause;
    private WaitForSeconds afterLiftToolAnimationPause;

    // The pause while using the tool/ lifting tool animation before we can use the tool or walk again, which corresponds to the animation time of using the tools
    private WaitForSeconds useToolAnimationPause;
    private WaitForSeconds liftToolAnimationPause;

    // This will hold our animation overrides
    private AnimationOverrides animationOverrides;

    // This is the grid cursor for valid/invalid item drops
    private GridCursor gridCursor;

    // Movement Parameters
    public float xInput;
    public float yInput;
    public bool isWalking;
    public bool isRunning;
    public bool isIdle;
    public bool isCarrying = false;
    public bool isUsingToolRight;
    public bool isUsingToolLeft;
    public bool isUsingToolUp;
    public bool isUsingToolDown;
    public bool isLiftingToolRight;
    public bool isLiftingToolLeft;
    public bool isLiftingToolUp;
    public bool isLiftingToolDown;
    public bool isSwingingToolRight;
    public bool isSwingingToolLeft;
    public bool isSwingingToolUp;
    public bool isSwingingToolDown;
    public bool isPickingRight;
    public bool isPickingLeft;
    public bool isPickingUp;
    public bool isPickingDown;
    public bool idleUp;
    public bool idleDown;
    public bool idleLeft;
    public bool idleRight;
    private ToolEffect toolEffect = ToolEffect.none;

    private Camera mainCamera;

    // Bool is enabled while the tool is in motion, so we can't do other stuff
    private bool playerToolUseDisabled = false;

    private Rigidbody2D rigidBody2D;

#pragma warning disable 414
    private Direction playerDirection;
#pragma warning restore 414

    // List for characterAttribute Structs that we want to swap animations for. This is what we pass into the AnimationOverride
    private List<CharacterAttribute> characterAttributeCustomisationList;

    private float movementSpeed;

    // Serialized field which we will populate with a prefab to show equipped item above a players head.
    [Tooltip("Should be populated in the prefab with the equipped item sprite rendered")]
    [SerializeField] private SpriteRenderer equippedItemSpriteRenderer = null;

    // Player attributes that can be swapped
    private CharacterAttribute armsCharacterAttribute;
    private CharacterAttribute toolCharacterAttribute;

    private bool _playerInputIsDisabled = false;

    public bool PlayerInputIsDisabled {get => _playerInputIsDisabled; set => _playerInputIsDisabled = value;}

    protected override void Awake()
    {
        base.Awake();

        rigidBody2D = GetComponent<Rigidbody2D>();

        // This will get all of the AnimationOverrides found in the children of player (arm, leg, etc)
        animationOverrides = GetComponentInChildren<AnimationOverrides>();

        // initialize our swappable character attributes (a struct) from the enums for the body part, the color, and the type
        armsCharacterAttribute = new CharacterAttribute(CharacterPartAnimator.arms, PartVariantColor.none, PartVariantType.none);

        // Initialize the list of character attributes
        characterAttributeCustomisationList = new List<CharacterAttribute>();

        // Get reference to the main camera
        mainCamera = Camera.main;
    }


    // Populate the gridCursor variable with the game object found in fame!
    private void Start()
    {
        // Populates the gridcursor member variable
        gridCursor = FindObjectOfType<GridCursor>();

        // Populated the tool/ lifting tool animation pauses using the settings file members
        useToolAnimationPause = new WaitForSeconds(Settings.useToolAnimationPause);
        liftToolAnimationPause = new WaitForSeconds(Settings.liftToolAnimationPause);

        afterUseToolAnimationPause = new WaitForSeconds(Settings.afterUseToolAnimationPause);
        afterLiftToolAnimationPause = new WaitForSeconds(Settings.afterLiftToolAnimationPause);
    }


    private void Update()
    {
        #region Player Input 

        if (!PlayerInputIsDisabled)
        {
            // Reset all the animation triggers to default to make sure
            ResetAnimationTriggers();

            // Take the keyboard input and occupy (xInput, yInput) with it, and determine player direction 
            PlayerMovementInput();

            // Check whether the player is walking (shift) or running
            PlayerWalkInput();
            
            // Player click to drop items
            PlayerClickInput();

            // Check if the testing keys for advancing game time have been pressed!
            PlayerTestInput();


            // From above two calls, we have xInput, yInput, movementSpeed, and isRunning, isWalking, and isIdle.
            // Now, send event info to delegate so any listeners will recieve player movement input
            EventHandler.CallMovementEvent(xInput, yInput, isWalking, isRunning, isIdle, isCarrying, toolEffect, 
                isUsingToolRight, isUsingToolLeft, isUsingToolUp, isUsingToolDown, 
                isLiftingToolRight, isLiftingToolLeft, isLiftingToolUp, isLiftingToolDown, 
                isPickingLeft, isPickingRight, isPickingUp, isPickingDown, 
                isSwingingToolRight, isSwingingToolLeft, isSwingingToolUp, isSwingingToolDown, 
                false, false, false, false);
        }

        #endregion
    }


    // Fixed update is for physics systems
    private void FixedUpdate()
    {   
        // Physically move the player!
        PlayerMovement();
    }


    private void PlayerMovement()
    {   
        // Calculate the new position to update each frame (Time.deltaTime is the FixedUpdate cycle time)
        Vector2 move = new Vector2(xInput * movementSpeed * Time.deltaTime, yInput * movementSpeed * Time.deltaTime);

        rigidBody2D.MovePosition(rigidBody2D.position + move);
    }


    private void ResetAnimationTriggers()
    {
        isPickingRight = false;
        isPickingLeft = false;
        isPickingUp = false;
        isPickingDown = false;
        isUsingToolRight = false;
        isUsingToolLeft = false;
        isUsingToolUp = false;
        isUsingToolDown = false;
        isLiftingToolRight = false;
        isLiftingToolLeft = false;
        isLiftingToolUp = false;
        isLiftingToolDown = false;
        isSwingingToolRight = false;
        isSwingingToolLeft = false;
        isSwingingToolUp = false;
        isSwingingToolDown = false;
        toolEffect = ToolEffect.none;
    }


    private void PlayerMovementInput()
    {   
        // These return only -1, 0, 1
        yInput = Input.GetAxisRaw("Vertical");
        xInput = Input.GetAxisRaw("Horizontal");

        if (yInput != 0 && xInput != 0)
        {
            xInput = xInput * 0.71f;
            yInput = yInput * 0.71f;
        }

        if (xInput != 0 || yInput != 0)
        {
            isRunning = true;
            isWalking = false;
            isIdle = false;

            movementSpeed = Settings.runningSpeed;

            // Capture player direction for save game
            if (xInput < 0)
            {
                playerDirection = Direction.left;
            }
            else if (xInput > 0)
            {
                playerDirection = Direction.right;
            }
            else if (yInput < 0)
            {
                playerDirection = Direction.down;
            }
            else
            {
                playerDirection = Direction.up;
            }
        }
        else if (xInput == 0 && yInput == 0)
        {
            isRunning = false;
            isWalking = false;
            isIdle = true;
        }

    }


    private void PlayerWalkInput()
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            isRunning = false;
            isWalking = true;
            isIdle = false;
            movementSpeed = Settings.walkingSpeed;
        }
        else
        {
            isRunning = true;
            isWalking = false;
            isIdle = false;
            movementSpeed = Settings.runningSpeed;
        }
    }


    // See if the left mouse button is clicked. If so, if the gridCursor is currently enabled (i.e. an item is selected), we will process the input
    private void PlayerClickInput()
    {
        // Can't do click inputs (like using tool again) if the tool use is disabled! Wait for the enimation pause to be over before it's enabled again
        if (!playerToolUseDisabled)
        {
            if (Input.GetMouseButton(0))
            {
                if (gridCursor.CursorIsEnabled)
                {                 
                    // Get the cursor grid position
                    Vector3Int cursorGridPosition = gridCursor.GetGridPositionForCursor();

                    // Get the player's grid position
                    Vector3Int playerGridPosition = gridCursor.GetGridPositionForPlayer();

                    ProcessPlayerClickInput(cursorGridPosition, playerGridPosition);
                }
            }
        }
    }


    // Process the players click input - whether it be dropping a seed/colmmodity, using a tool, etc. For tools, we need to calculate the direction we want to use it in for the
    // proper animation
    private void ProcessPlayerClickInput(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
    {   
        // Reset the players movement
        ResetMovement();

        // Find the direction the player needs to face to click on the given cursor location
        Vector3Int playerDirection = GetPlayerClickDirection(cursorGridPosition, playerGridPosition);

        // Get the GridPropertyDetails at the cursor position (the GridCursor validation routine ensures that the grid property details are not null)
        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cursorGridPosition.x, cursorGridPosition.y);

        // Get the selected items ItemDetails
        ItemDetails itemDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.player);

        // If the itemDetails aren't null (i.e. nothing selected), check the itemType for Seed, Commodity, hoeing_tool, watering_tool, or none/count.
        if (itemDetails != null)
        {
            switch (itemDetails.itemType)
            {   
                // If it's a seed, check if it can be dropped, and if the current cursor position is valid. If so, publish an event so subscribers can see it
                case ItemType.Seed:
                    if (Input.GetMouseButtonDown(0))
                    {
                        ProcessPlayerClickInputSeed(itemDetails);
                    }
                    break;
                
                // Same for commodities
                case ItemType.Commodity:
                    if (Input.GetMouseButtonDown(0))
                    {
                        ProcessPlayerClickInputCommodity(itemDetails);
                    }
                    break;

                // If it's a hoeing/watering tool, we use the ProcessPlayerClickInputTool method, which checks which tool is being used. If it's a hoeing_tool/watering_tool and if 
                // the cursor position is valid, we will execute the player use hoe/water sequence - which runs the hoeing/watering animation in the correct player direction, marks 
                // the ground gridPropertyDetails as dug/watered, updates the soil sprite to dug/watered, etc.
                case ItemType.Watering_tool:
                case ItemType.Hoeing_tool:
                    if (Input.GetMouseButtonDown(0))
                    {
                        ProcessPlayerClickInputTool(gridPropertyDetails, itemDetails, playerDirection);
                    }
                    break;

                case ItemType.none:
                    break;

                case ItemType.count:
                    break;

                default:
                    break;
            }
        }
    }


    // Given the cursor grid position and the player's grid position, calculate which direction the player needs to use the tool in.
    private Vector3Int GetPlayerClickDirection(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
    {
        if (cursorGridPosition.x > playerGridPosition.x)
        {
            return Vector3Int.right;
        }

        else if (cursorGridPosition.x < playerGridPosition.x)
        {
            return Vector3Int.left;
        }

        else if (cursorGridPosition.y > playerGridPosition.y)
        {
            return Vector3Int.up;
        }

        else
        {
            return Vector3Int.down;
        }
    }


    // Check if the selected seed item can be dropped, and if the current cursor position is valid (from distance from player, bool tilemap, etc.)
    private void ProcessPlayerClickInputSeed(ItemDetails itemDetails)
    {
        if (itemDetails.canBeDropped && gridCursor.CursorPositionIsValid)
        {   
            // If it's a valid drop, publish this event so subscribers can see it. UIInventorySlot.DropSelectedItemAtMousePosition will subscribe to this, and drop the item
            EventHandler.CallDropSelectedItemEvent();
        }
    }


    // Check if the selected commodity item can be dropped, and if the current cursor position is valid (from distance from player, bool tilemap, etc.)
    private void ProcessPlayerClickInputCommodity(ItemDetails itemDetails)
    {
        if (itemDetails.canBeDropped && gridCursor.CursorPositionIsValid)
        {   
            // If it's a valid drop, publish this event so subscribers can see it. UIInventorySlot.DropSelectedItemAtMousePosition will subscribe to this, and drop the item
            EventHandler.CallDropSelectedItemEvent();
        }
    }


    // This will process the players click input for all tools. We check which kind of tool it is, and do the corresponding action for that tool
    private void ProcessPlayerClickInputTool(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails, Vector3Int playerDirection)
    {
        // Switch on tool to check for which tool is actually being used - only hoeing_tools for now
        switch (itemDetails.itemType)
        {
            case ItemType.Hoeing_tool:
                if (gridCursor.CursorPositionIsValid)
                {
                    // If it's a hoeing tool, and the cursor position is valid, initiate the HoeGround sequence (play the hoeing animation in the correct player direction, mark the
                    // soil as dug, update the ground sprite to dug, etc.)
                    HoeGroundAtCursor(gridPropertyDetails, playerDirection);
                }
                break;

            case ItemType.Watering_tool:
                if (gridCursor.CursorPositionIsValid)
                {
                    // If it's a watering tool, and the cursor position is valid, initiate the WaterGround sequence (play the watering animation in the correct player direction, mark the
                    // soil as watered, update the ground sprite to watered, etc.)
                    WaterGroundAtCursor(gridPropertyDetails, playerDirection);
                }
                break;
            
            default:
                break;
        }
    }


    // This method just initiates the coroutine that enables the hoeing coroutine to initiate the animation, and update the GridPropertyDetails to dug at the square
    private void HoeGroundAtCursor(GridPropertyDetails gridPropertyDetails, Vector3Int playerDirection)
    {
        // Trigger the animation as a coroutine to run over several frames
        StartCoroutine(HoeGroundAtCursorRoutine(playerDirection, gridPropertyDetails));
    }


    // This coroutine initiates the hoeing animation, and updates the GridPropertyDetails at the square in question to be dug
    private IEnumerator HoeGroundAtCursorRoutine(Vector3Int playerDirection, GridPropertyDetails gridPropertyDetails)
    {
        // Disable player input and tool use so we can't walk away or use a tool again during the animation
        PlayerInputIsDisabled = true;
        playerToolUseDisabled = true;

        // Set the tool animation to hoe in the override animations. First, apply the 'hoe' part variant type, for the attribute we want swapped
        toolCharacterAttribute.partVariantType = PartVariantType.hoe;
        // Next, clear the list and add the tool character attribute struct to it. This list is used as an override to the animations
        characterAttributeCustomisationList.Clear();
        characterAttributeCustomisationList.Add(toolCharacterAttribute);
        // This method builds an animation ovveride list, and applies it to the tool animator
        animationOverrides.ApplyCharacterCustomizationParameters(characterAttributeCustomisationList);

        // Set the proper isUsingToolDirection bool for the player animation parameter.
        // Now that the overrides are active, these will be picked up in the update loop (movement event publisher!) to hoe in the right direction
        if (playerDirection == Vector3Int.right)
        {
            isUsingToolRight = true;
        }

        else if (playerDirection == Vector3Int.left)
        {
            isUsingToolLeft = true;
        }

        else if (playerDirection == Vector3Int.up)
        {
            isUsingToolUp = true;
        }

        else if (playerDirection == Vector3Int.down)
        {
            isUsingToolDown = true;
        }

        // Wait for useToolAnimationPause seconds (while animation goes with the animators!) before starting the next phase of the coroutine
        yield return useToolAnimationPause;

        // Set the Grid property details for the time since the ground was dug here
        if (gridPropertyDetails.daysSinceDug == -1)
        {
            gridPropertyDetails.daysSinceDug = 0;
        }

        // Set the grid property to dug with the above modified details (now that the ground is dug, we won't be able to dig again - red cursor)
        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);

        // Display the dug grid tiles
        GridPropertiesManager.Instance.DisplayDugGround(gridPropertyDetails);


        // Wait again for the tool animation pause for enabling input again, so we don't have to rapid of animations occuring
        yield return afterUseToolAnimationPause;

        // Enable player input and tool use so we can walk away or use a tool again
        PlayerInputIsDisabled = false;
        playerToolUseDisabled = false;
    }


    // This method just initiates the coroutine that enables the watering coroutine to initiate the animation, and update the GridPropertyDetails to watered at the square
    private void WaterGroundAtCursor(GridPropertyDetails gridPropertyDetails, Vector3Int playerDirection)
    {
        // Trigger the animation as a coroutine to run over several frames
        StartCoroutine(WaterGroundAtCursorRoutine(playerDirection, gridPropertyDetails));
    }


    // This coroutine initiates the watering animation, and updates the GridPropertyDetails at the square in question to be watered
    private IEnumerator WaterGroundAtCursorRoutine(Vector3Int playerDirection, GridPropertyDetails gridPropertyDetails)
    {
        // Disable player input and tool use so we can't walk away or use a tool again during the animation
        PlayerInputIsDisabled = true;
        playerToolUseDisabled = true;

        // Set the tool animation to wateringcan in the override animations. First, apply the 'wateringcan' part variant type, for the attribute we want swapped
        toolCharacterAttribute.partVariantType = PartVariantType.wateringCan;
        // Next, clear the list and add the tool character attribute struct to it. This list is used as an override to the animations
        characterAttributeCustomisationList.Clear();
        characterAttributeCustomisationList.Add(toolCharacterAttribute);
        // This method builds an animation ovveride list, and applies it to the tool animator
        animationOverrides.ApplyCharacterCustomizationParameters(characterAttributeCustomisationList);

        // TODO: If there is water in the watering can!
        // Add the watering tool effect to the animation parameters, which shows water pouring out
        toolEffect = ToolEffect.watering;

        // Set the proper isLiftingToolDirection bool for the player animation parameter.
        // Now that the overrides are active, these will be picked up in the update loop (movement event publisher!) to water in the right direction
        if (playerDirection == Vector3Int.right)
        {
            isLiftingToolRight = true;
        }

        else if (playerDirection == Vector3Int.left)
        {
            isLiftingToolLeft = true;
        }

        else if (playerDirection == Vector3Int.up)
        {
            isLiftingToolUp = true;
        }

        else if (playerDirection == Vector3Int.down)
        {
            isLiftingToolDown = true;
        }

        // Wait for liftToolAnimationPause seconds (while animation goes with the animators!) before starting the next phase of the coroutine
        yield return liftToolAnimationPause;

        // Set the Grid property details for the time since the ground was watered here
        if (gridPropertyDetails.daysSinceWatered == -1)
        {
            gridPropertyDetails.daysSinceWatered = 0;
        }

        // Set the grid property to watered with the above modified details (now that the ground is watered, we won't be able to water again - red cursor)
        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);

        // Display the grid tiles to reflect they've been watered
        GridPropertiesManager.Instance.DisplayWateredGround(gridPropertyDetails);

        // Wait again for the tool animation pause for enabling input again, so we don't have to rapid of animations occuring
        yield return afterLiftToolAnimationPause;

        // Enable player input and tool use so we can walk away or use a tool again
        PlayerInputIsDisabled = false;
        playerToolUseDisabled = false;
    }


    // TODO: Remove
    /// <summary>
    /// Temporary routine for test input to advance the game clock quickly
    /// </summary>
    private void PlayerTestInput()
    {
        // Trigger advance minute (will continue to be called if held down)
        if (Input.GetKey(KeyCode.T))
        {
            TimeManager.Instance.TestAdvanceGameMinute();
        }

        // Trigger advance day (will only be called once every time held down)
        if (Input.GetKeyDown(KeyCode.G))
        {
            TimeManager.Instance.TestAdvanceGameDay();
        }

        // Test scene unload/loading
        if (Input.GetKeyDown(KeyCode.L))
        {
            SceneControllerManager.Instance.FadeAndLoadScene(SceneName.Scene1_Farm.ToString(), transform.position);
        }
    }


    private void ResetMovement()
    {
        // Reset the players movement
        xInput = 0f;
        yInput = 0f;
        isRunning = false;
        isWalking = false;
        isIdle = true;
    }


    public void DisablePlayerInputAndResetMovement()
    {
        DisablePlayerInput();
        ResetMovement();

        // Send event to any listeners for movement input, with reset, stationary values 
        EventHandler.CallMovementEvent(xInput, yInput, isWalking, isRunning, isIdle, isCarrying, toolEffect, 
            isUsingToolRight, isUsingToolLeft, isUsingToolUp, isUsingToolDown, 
            isLiftingToolRight, isLiftingToolLeft, isLiftingToolUp, isLiftingToolDown, 
            isPickingLeft, isPickingRight, isPickingUp, isPickingDown, 
            isSwingingToolRight, isSwingingToolLeft, isSwingingToolUp, isSwingingToolDown, 
            false, false, false, false);
    }


    public void DisablePlayerInput()
    {
        PlayerInputIsDisabled = true;
    }


    public void EnablePlayerInput()
    {
        PlayerInputIsDisabled = false;
    }


    public void ClearCarriedItem()
    {
        // Set the equipped item sprite to null, with a not visible color
        equippedItemSpriteRenderer.sprite = null;
        equippedItemSpriteRenderer.color = new Color(1f, 1f, 1f, 0f);

        // Apply the base character arms customization to none
        armsCharacterAttribute.partVariantType = PartVariantType.none;

        // Clear the list and add the none characterAttribute struct to it
        characterAttributeCustomisationList.Clear();
        characterAttributeCustomisationList.Add(armsCharacterAttribute);

        // This method builds an animation ovveride list, and applies it to the arms animator. Now the type is none so nothing will be overriden!
        animationOverrides.ApplyCharacterCustomizationParameters(characterAttributeCustomisationList);
        
        // Set the flag so the player is not carrying the item
        isCarrying = false;
    }


    public void ShowCarriedItem(int itemCode)
    {
        // Extract the item codes item detailsd
        ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(itemCode);

        if (itemDetails != null)
        {
            // Set the equipped item sprite to the one applied in the item details, with a visible color (default is not visible..)
            equippedItemSpriteRenderer.sprite = itemDetails.itemSprite;
            equippedItemSpriteRenderer.color = new Color(1f, 1f, 1f, 1f);

            // Apply the 'carry' character arms customization
            armsCharacterAttribute.partVariantType = PartVariantType.carry;

            // Clear the list and add the arms character attribute struct to it
            characterAttributeCustomisationList.Clear();
            characterAttributeCustomisationList.Add(armsCharacterAttribute);

            // This method builds an animation ovveride list, and applies it to the arms animator
            animationOverrides.ApplyCharacterCustomizationParameters(characterAttributeCustomisationList);
            
            // Set the flag so the player is carrying the item
            isCarrying = true;
        }
    }


    public Vector3 GetPlayerViewportPosition()
    {
        // Vector3 viewport position for player ((0, 0) is viewport bottom left, and (1,1) is viewport top right)
        // This is the position of the player in the cameras field of view, so the UI bar can tell if the player is near the bottom
        return mainCamera.WorldToViewportPoint(transform.position);
    }
<<<<<<< HEAD


    // This will return the world position for the CENTER of the player's GameObject, so we include a y-offset to bring this position up from 
    // The standard pivot point at the feet.
    public Vector3 GetPlayerCenterPosition()
    {
        return new Vector3(transform.position.x, transform.position.y + Settings.playerCenterYOffset, transform.position.z);
    }
=======
>>>>>>> 06b270bebb0960c9d8506772aa1531ea81c70c95
}
