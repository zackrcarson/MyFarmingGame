using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AStar))]
public class NPCManager : SingletonMonobehaviour<NPCManager>
{
    // Populated in the inspector with the so_SceneRouteList describing all the possible routes between scenes, and then a dictionary we'll create to store these routes,
    // so we can easily find if a route exists, keyed by a combination of the two scene names we want to traverse
    [SerializeField] private SO_SceneRouteList so_SceneRouteList = null;
    private Dictionary<string, SceneRoute> sceneRouteDictionary;

    // This will hold our array of NPCs int he game
    [HideInInspector]
    public NPC[] npcArray;

    // AStar class for path finding
    private AStar aStar;


    // Populate the AStar for path finding, and the array of NPC to be used
    protected override void Awake()
    {
        base.Awake();

        // Create the SceneRouteDictionary containing all of the possible scene routes the NPC can take (i.e. farm->field, cabin->field, etc, each describing the enter/exit points)
        sceneRouteDictionary = new Dictionary<string, SceneRoute>();

        if (so_SceneRouteList.sceneRouteList.Count > 0)
        {
            // As long as the SO_SceneRouteList has been populated, loop through all of the SceneRoutes in the SO's sceneRouteList, adding them to the dictionary keyed by the from and to scene names
            foreach (SceneRoute so_sceneRoute in so_SceneRouteList.sceneRouteList)
            {
                // Check for duplicate routes already in the dictionary - don't add it to the dictionary if so
                if (sceneRouteDictionary.ContainsKey(so_sceneRoute.fromSceneName.ToString() + so_sceneRoute.toSceneName.ToString()))
                {
                    Debug.Log("** Duplicate Scene Route Key Found ** Check for duplicate routes in the scriptable object scene route list");
                    continue;
                }

                // Add the current route to our dictionary, keyed by a combination of the fromSceneName and the toSceneName
                sceneRouteDictionary.Add(so_sceneRoute.fromSceneName.ToString() + so_sceneRoute.toSceneName.ToString(), so_sceneRoute);
            }
        }

        // AStar is our path builder algorithm
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


    // This method takes in a desired fromScene and toScene that we want our NPC to move between, and returns the corresponding SceneRoute from the sceneRouteDictionary, if that route exists.
    // Else, it returns null
    public SceneRoute GetSceneRoute(string fromSceneName, string toSceneName)
    {
        SceneRoute sceneRoute;

        // Get the scene route desired from the SceneRouteDictionary if it exists. Return the scene route if it exists, and null if not
        if (sceneRouteDictionary.TryGetValue(fromSceneName + toSceneName, out sceneRoute))
        {
            return sceneRoute;
        }
        else
        {
            return null;
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
