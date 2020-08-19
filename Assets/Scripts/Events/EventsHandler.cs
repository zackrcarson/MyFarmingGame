public delegate void MovementDelegate(float inputX, float inputY, bool isWalking, bool isRunning, bool isIdle, bool isCarrying, ToolEffect toolEffect, 
    bool isUsingToolRight, bool isUsingToolLeft, bool isUsingToolUp, bool isUsingToolDown, 
    bool isLiftingToolRight, bool isLiftingToolLeft, bool isLiftingToolUp, bool isLiftingToolDown, 
    bool isPickingLeft, bool isPickingright, bool isPickingUp, bool isPickingdown, 
    bool isSwingToolRight, bool isSwingToolLeft, bool isSwingToolUp, bool isSwingToolDown, 
    bool isIdleUp, bool isIdleDown, bool isIdleLeft, bool isIdleRight);

public static class EventsHandler
{
    // Movement Event

    public static event MovementDelegate MovementEvent;

    // Movement Event Call For Publishers

    public static void CallMovementEvent(float inputX, float inputY, bool isWalking, bool isRunning, bool isIdle, bool isCarrying, ToolEffect toolEffect, 
        bool isUsingToolRight, bool isUsingToolLeft, bool isUsingToolUp, bool isUsingToolDown, 
        bool isLiftingToolRight, bool isLiftingToolLeft, bool isLiftingToolUp, bool isLiftingToolDown, 
        bool isPickingLeft, bool isPickingright, bool isPickingUp, bool isPickingdown, 
        bool isSwingToolRight, bool isSwingToolLeft, bool isSwingToolUp, bool isSwingToolDown, 
        bool isIdleUp, bool isIdleDown, bool isIdleLeft, bool isIdleRight)
    {
        if (MovementEvent != null)
        {
            MovementEvent(inputX, inputY, isWalking, isRunning, isIdle, isCarrying, toolEffect, 
                isUsingToolRight, isUsingToolLeft, isUsingToolUp, isUsingToolDown, 
                isLiftingToolRight, isLiftingToolLeft, isLiftingToolUp, isLiftingToolDown, 
                isPickingLeft, isPickingright, isPickingUp, isPickingdown, 
                isSwingToolRight, isSwingToolLeft, isSwingToolUp, isSwingToolDown, 
                isIdleUp, isIdleDown, isIdleLeft, isIdleRight);
        }
    }
}