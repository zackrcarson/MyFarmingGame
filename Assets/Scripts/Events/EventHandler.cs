using System;
using System.Collections.Generic;

public delegate void MovementDelegate(float xInput, float yInput, bool isWalking, bool isRunning, bool isIdle, bool isCarrying, ToolEffect toolEffect, 
    bool isUsingToolRight, bool isUsingToolLeft, bool isUsingToolUp, bool isUsingToolDown, 
    bool isLiftingToolRight, bool isLiftingToolLeft, bool isLiftingToolUp, bool isLiftingToolDown, 
    bool isPickingLeft, bool isPickingRight, bool isPickingUp, bool isPickingDown, 
    bool isSwingingToolRight, bool isSwingingToolLeft, bool isSwingingToolUp, bool isSwingingToolDown, 
    bool idleUp, bool idleDown, bool idleLeft, bool idleRight);

public static class EventHandler
{
    // Inventory Updated event
    public static event Action<InventoryLocation, List<InventoryItem>> InventoryUpdatedEvent;

    // Inventory updated event call for publishers, to send out event details to any subscribers
    public static void CallInventoryUpdatedEvent(InventoryLocation inventoryLocation, List<InventoryItem> inventoryList)
    {
        // Check if there are any subscribers - or else do nothing!
        if (InventoryUpdatedEvent != null)
        {
            InventoryUpdatedEvent(inventoryLocation, inventoryList);
        }
    }

    // Movement Event
    public static event MovementDelegate MovementEvent;

    // Movement Event Call For Publishers
    public static void CallMovementEvent(float xInput, float yInput, bool isWalking, bool isRunning, bool isIdle, bool isCarrying, ToolEffect toolEffect, 
        bool isUsingToolRight, bool isUsingToolLeft, bool isUsingToolUp, bool isUsingToolDown, 
        bool isLiftingToolRight, bool isLiftingToolLeft, bool isLiftingToolUp, bool isLiftingToolDown, 
        bool isPickingLeft, bool isPickingRight, bool isPickingUp, bool isPickingDown, 
        bool isSwingingToolRight, bool isSwingingToolLeft, bool isSwingingToolUp, bool isSwingingToolDown, 
        bool idleUp, bool idleDown, bool idleLeft, bool idleRight)
    {
        if (MovementEvent != null)
        {
            MovementEvent(xInput, yInput, isWalking, isRunning, isIdle, isCarrying, toolEffect, 
                isUsingToolRight, isUsingToolLeft, isUsingToolUp, isUsingToolDown, 
                isLiftingToolRight, isLiftingToolLeft, isLiftingToolUp, isLiftingToolDown, 
                isPickingLeft, isPickingRight, isPickingUp, isPickingDown, 
                isSwingingToolRight, isSwingingToolLeft, isSwingingToolUp, isSwingingToolDown, 
                idleUp, idleDown, idleLeft, idleRight);
        }
    }
}