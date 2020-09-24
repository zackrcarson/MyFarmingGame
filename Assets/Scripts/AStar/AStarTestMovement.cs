using UnityEngine;

// This script is used to test the AStar algorthm in AStar.cs, and the NPCMovement and path following. It will create a scheduled movement event and 
// Build the path and make the NPC follow it
public class AStarTestMovement : MonoBehaviour
{
    // Populated in the editor
    [SerializeField] private NPCPath npcPath = null; //NPCPAth component for the NPC we want to control
    [SerializeField] private bool moveNPC = false; // Ticked in editor to say when to move the NPC
    [SerializeField] private SceneName sceneName = SceneName.Scene1_Farm; // The name of the scene we want the NPC to walk to
    [SerializeField] private Vector2Int finishPosition; // The position we want the NPC to move to
    [SerializeField] private AnimationClip idleDownAnimationClip = null; // The animation clip for idle down, so the NPC will idle down when they arrive
    [SerializeField] private AnimationClip eventAnimationClip = null; // the event animation clip to play when the NPC arrives
    private NPCMovement npcMovement;


    // Populate the npcMovement object, and set its facing direction and idledown animation
    private void Start()
    {
        npcMovement = npcPath.GetComponent<NPCMovement>();
        npcMovement.npcFacingDirectionAtDestination = Direction.down;
        npcMovement.npcTargetAnimationClip = idleDownAnimationClip;
    }

   
    // Check until the moveNPC bool is triggered in the editor. Then create an NPCSchedule event for this movement we want to make, and build the path and have the NPC follow it 
    private void Update()
    {
        if (moveNPC)
        {
            moveNPC = false;

            // Now scheduled event with the correct parameters
            NPCScheduleEvent npcScheduleEvent = new NPCScheduleEvent(0, 0, 0, 0, Weather.none, Season.none, sceneName, new GridCoordinate(finishPosition.x, finishPosition.y), eventAnimationClip);

            // Build the path from this npcScheduleEvent
            npcPath.BuildPath(npcScheduleEvent);
        }
    }
}

