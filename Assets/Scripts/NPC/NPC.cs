using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

// This class will manage the save game components for the NPC, so we can save and load their positions and/or target positions when we save/load the game
[RequireComponent(typeof(NPCMovement))]
[RequireComponent(typeof(GenerateGUID))]
public class NPC : MonoBehaviour, ISaveable
{
    [SerializeField] private AnimationClip[] targetAnimations = null; // I added these to populate with a list of all of the NPCs animations, so we can save the target animation as a string, and then load the proper animation clip from that string
    private Dictionary<string, AnimationClip> targetAnimationsDictionary;

    // Unique ID required by the ISaveable interface, will store the GUID attached to the NPC gameObject
    private string _iSaveableUniqueID;
    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }

    // GameObjectSave required by the ISaveable interface, storesd the save data that is built up for every object that has the ISaveable interface attached
    private GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }

    // NPCMovement component of the NPC so we can access the NPCs current position and target position for saving/loading
    private NPCMovement npcMovement;

    // On enable, this will just register this gameObject as an ISaveable, so that the SaveLoadManager can save/load the methods set up here
    private void OnEnable()
    {
        // Registers this game object within the iSaveableObjectList, which is looped through in the SaveLoadManager for all objects to save/load the saved items
        ISaveableRegister();
    }


    // Deregister from the iSaveableObjectList
    private void OnDisable()
    {
        // Deregisters this game object within the iSaveableObjectList, which is looped through in the SaveLoadManager for all objects to save/load the saved items
        ISaveableDeregister();
    }


    // When the NPC GameObject is Awake (at the beginning of the game), we will cache the GUID attached to the NPC, and initialize the GameObjectSave to save aspects of the NPC
    private void Awake()
    {
        // I added this to populate our dictionary with <string, AnimationClip>, so when we load the saved string describing the targetAnimation, we can find the corresponding clip and play it!!
        targetAnimationsDictionary = new Dictionary<string, AnimationClip>();
        foreach (AnimationClip animationClip in targetAnimations)
        {
            targetAnimationsDictionary.Add(animationClip.ToString(), animationClip);
        }

        // Get the unique ID for the GameObject
        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;

        // Initialize the GameObjectSave variable
        GameObjectSave = new GameObjectSave();
    }


    // Populate the npcMovement component to access the target positions
    private void Start()
    {
        // Get the NPCMovement component so we can save/load the NPCs position or target position
        npcMovement = GetComponent<NPCMovement>();
    }


    // Required method by the ISaveable interface, which will be called OnEnable() of the NPC GameObject, and it will 
    // Add an entry (of this gameObject) to the iSaveableObjectList in SaveLoadManager, which will then manage
    // Looping through all such items in this list to save/load their data
    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }


    // Required method by the ISaveable interface, which will be called OnDisable() of the NPC GameObject, and it will
    // Remove this item from the saveable objects list, as described above
    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }


    // Required method by the ISaveable interface. This will get called from the SaveLoadManager, for each scene to save the dictionaries (GameObjectSave has a dict keyed by scene name)
    // This method will store the sceneData for the persistent scene which the NPC lives on. It will then return a GameObjectSave, which just has a Dict of SceneSave data for each scene, keyed by scene name
    public GameObjectSave ISaveableSave()
    {
        // Delete the sceneData (dict of data to save in that scene, keyed by scene name) for the GameObject if it already exists in the persistent scene
        // which is where this data is going to be saved, so we can create a new one with updated dictionaries
        GameObjectSave.sceneData.Remove(Settings.PersistentScene);

        // Create the SaveScene for this gameObject (keyed by the scene name, storing multiple dicts for bools, the scene the player ended in, the players location, the gridPropertyDetails,
        // the SceneItems, and the inventory items and quantities, and the gameYear, day, hour, minute, second, season, day of week)
        SceneSave sceneSave = new SceneSave();

        // Create a new serializable Vector3 dictionary to store the NPCs target position
        sceneSave.vector3Dictionary = new Dictionary<string, Vector3Serializable>();

        // Create a new string dictionary to store the NPCs target scene
        sceneSave.stringDictionary = new Dictionary<string, string>();
        
        // Add values to the vector3 dictionary for the NPCs target grid and world positions, keyed so we can easily retrieve them on load
        sceneSave.vector3Dictionary.Add("npcTargetGridPosition", new Vector3Serializable(npcMovement.npcTargetGridPosition.x, npcMovement.npcTargetGridPosition.y, npcMovement.npcTargetGridPosition.z));
        sceneSave.vector3Dictionary.Add("npcTargetWorldPosition", new Vector3Serializable(npcMovement.npcTargetWorldPosition.x, npcMovement.npcTargetWorldPosition.y, npcMovement.npcTargetWorldPosition.z));

        // Add values to the string dictionary for the NPCs target scene, keyed so we can easily retrieve it on load
        sceneSave.stringDictionary.Add("npcTargetScene", npcMovement.npcTargetScene.ToString());
        sceneSave.stringDictionary.Add("npcTargetAnimation", npcMovement.npcTargetAnimationClip.ToString()); // I added this so we can save a string describing the target animatino clip, so we can play it when the npc is transported to his target

        // Add the SceneSave data for the NPC game object to the GameObjectSave, which is a dict storing all the dicts in a scene to be loaded/saved, keyed by the scene name
        // The NPC will get stored in the Persistent Scene
        GameObjectSave.sceneData.Add(Settings.PersistentScene, sceneSave);

        // Return the GameObjectSave, which has a dict of the Saved stuff for the CharacterCustomization GameObject
        return GameObjectSave;
    }


    // This is a required method for the ISaveable interface, which passes in a GameObjectSave dictionary, and restores the current scene from it
    // The SaveLoadManager script will loop through all of the ISaveableRegister GameObjects (all registered with their ISaveableRegister methods), and trigger this 
    // ISaveableLoad, which will load that Save data (here for the persistent scene NPC information, which includes the all of the customization parameters),
    // for each scene (GameObjectSave is a Dict keyed by scene name).
    public void ISaveableLoad(GameSave gameSave)
    {
        // gameSave stores a Dictionary of items to save keyed by GUID, see if there's one for this GUID (generated on the InventoryManager GameObject)
        if (gameSave.gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;

            // Get the save data for the scene, if one exists for the PersistentScene (what the NPC info is saved under)
            if (gameObjectSave.sceneData.TryGetValue(Settings.PersistentScene, out SceneSave sceneSave))
            {
                // If both the stringDictionary (storing NPC target scene) and the Vector3Dictionary (storing the NPCs target grid and world positions)
                // exist, populate the saved values!
                if (sceneSave.vector3Dictionary != null && sceneSave.stringDictionary != null)
                {
                    // Check if the npcTargetGridPosition is stored in the Vector3Dictionary, if so, repopulate it's values into NPCMovement so it'll know where the NPC currently is
                    if (sceneSave.vector3Dictionary.TryGetValue("npcTargetGridPosition", out Vector3Serializable savedNPCTargetGridPosition))
                    {
                        npcMovement.npcTargetGridPosition = new Vector3Int((int)savedNPCTargetGridPosition.x, (int)savedNPCTargetGridPosition.y, (int)savedNPCTargetGridPosition.z);
                        npcMovement.npcCurrentGridPosition = npcMovement.npcTargetGridPosition;
                    }

                    // Check if the npcTargetWorldPosition is stored in the Vector3Dictionary, if so, repopulate it's values into NPCMovement so we can transport the NPC over there
                    if (sceneSave.vector3Dictionary.TryGetValue("npcTargetWorldPosition", out Vector3Serializable savedNPCTargetWorldPosition))
                    {
                        npcMovement.npcTargetWorldPosition = new Vector3Int((int)savedNPCTargetGridPosition.x, (int)savedNPCTargetGridPosition.y, (int)savedNPCTargetGridPosition.z);
                        transform.position = npcMovement.npcTargetGridPosition;
                    }

                    // Check if the npcTargetScene is stored in the stringDictionary, if so, repopulate it's value into NPCMovement so we can set the NPC to the correct scene
                    if (sceneSave.stringDictionary.TryGetValue("npcTargetScene", out string savedNPCTargetScene))
                    {
                        // Check to make sure the targetSceneName string is an Enum, if so convert it to a SceneName enum
                        if (Enum.TryParse<SceneName>(savedNPCTargetScene, out SceneName sceneName))
                        {
                            // Pass the SceneName into the NPCMovements currentScene
                            npcMovement.npcTargetScene = sceneName;
                            npcMovement.npcCurrentScene = npcMovement.npcTargetScene;
                        }
                    }

                    // I added this to load the target animatino clip, so we can play it when the npc is transported to his target. This checks the animation dictionary for the <string, Clip> value,
                    // And uses the corresponding clip to play the npc animaition!
                    if (sceneSave.stringDictionary.TryGetValue("npcTargetAnimation", out string savedNPCTargetAnimation))
                    {
                        if (targetAnimationsDictionary.TryGetValue(savedNPCTargetAnimation, out AnimationClip savedNPCTargetAnimationClip))
                        {
                            // Clear out the movement and then set the NPCs animation to the loaded targetAnimation
                            npcMovement.CancelNPCMovement();
                            npcMovement.npcTargetAnimationClip = savedNPCTargetAnimationClip;
                            npcMovement.SetNPCEventAnimation();
                        }
                    }
                    else
                    {
                        // Clear out any current NPC movement on the current game before we load the saved game, so our npc doesn't start animnating or anything
                        npcMovement.CancelNPCMovement();
                    }
                }
            }
        }
    }


    // Required method by the ISaveable interface, which will store all of the scene data, executed for every item in the iSaveableObjectList. This let's us walk between
    // scenes and keep the stored stuff active with ISaveableRestoreScene 
    public void ISaveableStoreScene(string sceneName)
    {
        // Nothing to store here since the NPC is on a persistent scene - it won't get reset ever because we always stay on that scene
    }


    // Required method by the ISaveable interface, which will restore all of the scene data, executed for every item in the iSaveableObjectList. This let's us walk between
    // scenes and keep the stored stuff active with ISaveableRestoreScene 
    public void ISaveableRestoreScene(string sceneName)
    {
        // Nothing to restore here since the NPC is on a persistent scene - it won't get reset ever because we always stay on that scene
    }
}
