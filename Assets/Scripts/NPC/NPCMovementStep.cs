using UnityEngine;

// This is basically a container to store a single step that an NPC will take on a path! AStar will populate a stack of these
// steps for every grid square on the path that it found, and then the NPC will follow it one step at a time
public class NPCMovementStep
{
    // These are the variables stored for each step. We have the sceneName the step corresponds to (so they can cross scenes), the gridCoordinate of that
    // step to move to, and the hour/minute/second that the NPC should be to that step by (determines the speed of the NPC!)
    public SceneName sceneName;
    public int hour;
    public int minute;
    public int second;
    public Vector2Int gridCoordinate;
}
