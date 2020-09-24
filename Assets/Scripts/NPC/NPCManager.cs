using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AStar))]
public class NPCManager : SingletonMonobehaviour<NPCManager>
{
    // This will hold our array of NPCs int he game
    [HideInInspector]
    public NPC[] npcArray;

    // AStar class for path finding
    private AStar aStar;


    // Populate the AStar for path finding, and the array of NPC to be used
    protected override void Awake()
    {
        base.Awake();

        aStar = GetComponent<AStar>();

        // Get an array of all NPC objects in the scene
        npcArray = FindObjectsOfType<NPC>();
    }


    // Subscribe the AfterSceneLoad method to the AfterSceneLoadedEvent, so when a scene is loaded we can set the NPCs as active
    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += AfterSceneLoad;
    }


    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoad;
    }


    // This method is subscribed to the AfterSceneLoadedEvent, so when the scene has been loaded, this will run and set the NPCs as active status
    private void AfterSceneLoad()
    {
        // Once the scene is loaded, set the NPCs to active or inactive depending on if their current scene is the current active scene or not
        SetNPCsActiveStatus();
    }


    // Loop through all of the NPCs, and set them to active (visible) if they are in the currently loaded scene the player is in, and inactive if they are not
    // Note NPCs are on the persistent scene so theyre always there. We just set them to active/inactive depending on what scene the player is in
    private void SetNPCsActiveStatus()
    {
        foreach (NPC npc in npcArray)
        {
            NPCMovement npcMovement = npc.GetComponent<NPCMovement>();

            if (npcMovement.npcCurrentScene.ToString() == SceneManager.GetActiveScene().name)
            {
                npcMovement.SetNPCActiveInScene();
            }
            else
            {
                npcMovement.SetNPCInactiveInScene();
            }
        }
    }


    // This method is calls AStar.BuildPath to build a path in the scene, from start to end positions, and populates the npcMovementStepStack for instructions for the NPCs.
    // This will return true is AStar was successful, and false if not.
    // NPCPath will be activated on the npcScheduleEvent, which will then call this method, NPCManager.BuildPath, which then in turn calls the 
    // AStar BuildPath method, which will actually populate the npcMovementStepStack
    public bool BuildPath(SceneName sceneName, Vector2Int startGridPosition, Vector2Int endGridPosition, Stack<NPCMovementStep> npcMovementStepStack)
    {
        if (aStar.BuildPath(sceneName, startGridPosition, endGridPosition, npcMovementStepStack))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
