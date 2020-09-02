using System.Collections.Generic;
using UnityEngine;

public class Player : SingletonMonobehaviour<Player>
{
    // This will hold our animation overrides
    private AnimationOverrides animationOverrides;

    // Movement Parameters
    public float xInput;
    public float yInput;
    public bool isWalking;
    public bool isRunning;
    public bool isIdle;
    public bool isCarrying = false;
    public ToolEffect toolEffect = ToolEffect.none;
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

    private Camera mainCamera;
    
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
}
