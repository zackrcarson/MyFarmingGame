using System;
using System.Collections.Generic;
using System.Configuration;
using UnityEngine;

// This class will define the NPC's movement path for a given scheduled movement event. It will build the path via NPCManager ->  AStar, populated into
// a NPC Movement Step Stack of steps to take, and populate the gameTimes for each step that the NPC needs to be on in time
[RequireComponent(typeof(NPCMovement))]
public class NPCPath : MonoBehaviour
{
    // This is the NPC Movement Step Stack created by the AStar.BuildPath, to give instructions to the NPC on where to walk
    public Stack<NPCMovementStep> npcMovementStepStack;

    // NPC Movement object that handles the movement of our NPC from the npcMovementStepStack instructions
    private NPCMovement npcMovement;


    // Populate the NPCMovement member, and the npcMovementStepStack with a new instance of it
    private void Awake()
    {
        npcMovement = GetComponent<NPCMovement>();
        npcMovementStepStack = new Stack<NPCMovementStep>();
    }


    // Clear out the npcMovementStepStack in case it still has movements on it from past paths
    public void ClearPath()
    {
        npcMovementStepStack.Clear();
    }


    // This method is called from AStarTest or NPCScheduler to build the path. This method then calls NPCManager.BuildPath(), which in turn calls AStar.BuildPath()
    // To ultimately find the path taken and populate npcMovementStepStack with steps to take.
    public void BuildPath(NPCScheduleEvent npcScheduleEvent)
    {
        // First clear out the npcMovementStepStack in case it has extra steps from earlier builds
        ClearPath();

        // If the scheduled event is for the same scene as the current NPC scene. For now, we are only moving the NPC within the current scene. If it's not, don't run the scheduled event
        if (npcScheduleEvent.toSceneName == npcMovement.npcCurrentScene)
        {
            // If we are in the same scene as the scheduled event, get the current (start) and target grid positions for the movement event, the former from the npcMovement, and
            // the latter from the scheduleEvent
            Vector2Int npcCurrentGridPosition = (Vector2Int)npcMovement.npcCurrentGridPosition;

            Vector2Int npcTargetGridPosition = (Vector2Int)npcScheduleEvent.toGridCoordinate;

            // Build the path with NPCManager -> AStar and add the corresponding movement steps to the movement step stack
            NPCManager.Instance.BuildPath(npcScheduleEvent.toSceneName, npcCurrentGridPosition, npcTargetGridPosition, npcMovementStepStack);

            // If stack count is > 1 (i.e. has steps beyond the starting one), update the times and the pop off the 1st item which is the starting position
            if (npcMovementStepStack.Count > 1)
            {
                // This method will loop through all of the steps in the npcMovementStepStack, and populate the gameTime that the NPC needs to 
                // be to that step by.
                UpdateTimesOnPath();

                // discard starting step (we're already on the starting step)
                //npcMovementStepStack.Pop();  // I removed this line so the NPC doesn't jump a square for the very first step...

                // Set the schedule event details in NPC movement, so it knows how to move the NPC, and what facing direction/animation to play when they get there
                npcMovement.SetScheduleEventDetails(npcScheduleEvent);
            }
        }
    }


    /// <summary>
    /// Update the path movement steps with the expected gameTimes we need to be on that step for
    /// All of the MovementSteps in the MovementStepStack for the NPC to follow, contains the sceneName/gridCoordinate to move to, and the hour/min/sec that we have to be on that step by
    /// This allows us to control the NPCs speed between steps
    /// </summary>
    public void UpdateTimesOnPath()
    {
        // Get the current game time from the timeManager
        TimeSpan currentGameTime = TimeManager.Instance.GetGameTime();

        // This is the previous step taken (so we can adjust the walking direction to go from previousStep -> currentStep)
        NPCMovementStep previousNPCMovementStep = null;

        // Loop through all of the npcMovementStep in the npcMovementStepStack and update the time we need to be at that step by, as member variables in the individual npcMovementSteps
        foreach (NPCMovementStep npcMovementStep in npcMovementStepStack)
        {
            // If we're on the first step, set the previousStep to the currentStep so the next step has it
            if (previousNPCMovementStep == null)
            {
                previousNPCMovementStep = npcMovementStep;
            }

            // set the current step as the current game time. At the end of the loop we will add the next step's worth of time to currentGameTime so the next step will be larger
            npcMovementStep.hour = currentGameTime.Hours;
            npcMovementStep.minute = currentGameTime.Minutes;
            npcMovementStep.second = currentGameTime.Seconds;

            // This is the amount of time the current step will take to make
            TimeSpan movementTimeStep;

            // Compute the time required for the current movement step, depending on if it's a diagonal or horizontal/vertical step
            // This is computed as the distance to travel / NPCs speed, and additionally modified by 1/secondsPerGameSecond to get it in real time
            // If the current step is diagonal from the previous step, calculate the step time distance from a diagonal cell size. If not, use a normal cell size
            if (MovementIsDiagonal(npcMovementStep, previousNPCMovementStep))
            {
                movementTimeStep = new TimeSpan(0, 0, (int)(Settings.gridCellDiagonalSize / Settings.secondsPerGameSecond / npcMovement.npcNormalSpeed));
            }
            else
            {
                movementTimeStep = new TimeSpan(0, 0, (int)(Settings.gridCellSize / Settings.secondsPerGameSecond / npcMovement.npcNormalSpeed));
            }

            // Add the time for the next movement step to the currentGameTime so the next step in the loop will have this steps time added to it
            currentGameTime = currentGameTime.Add(movementTimeStep);

            // Set the previous step to the current one so we can start over on the next step
            previousNPCMovementStep = npcMovementStep;
        }
    }


    /// <summary>
    ///  Determines if the movement step between a previous step and the current movement step is diagonal or not, so we know how long it takes to traverse between the two steps
    /// </summary>
    /// <param name="npcMovementStep"></param>
    /// <param name="previousNPCMovementStep"></param>
    /// <returns> Returns true if the step between them is diagonal, and false if they are horizontal or vertical </returns>
    private bool MovementIsDiagonal(NPCMovementStep npcMovementStep, NPCMovementStep previousNPCMovementStep)
    {
        // Return true if the npcMovementStep gridCoordinates are diagonal (x1 != x2 and y1 != y2), and false if they are horizontal/vertical
        if ((npcMovementStep.gridCoordinate.x != previousNPCMovementStep.gridCoordinate.x) && (npcMovementStep.gridCoordinate.y != previousNPCMovementStep.gridCoordinate.y))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
