using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

// This is a singleton that storing, restoring, saving, and unloading of items. It 
// Inherits from ISaveable, and it must be on something that has a GenerateGUID.
// Isaveable is an interfacte that guarantees it has the proper methods and properties
[RequireComponent(typeof(GenerateGUID))]
public class SceneItemsManager : SingletonMonobehaviour<SceneItemsManager>, ISaveable
{
    private Transform parentItem;

    // Prefab populated in the editor for creating new gameObjects
    [SerializeField] private GameObject itemPrefab = null;

    // First requirement of ISaveable -> the unique ID for each item
    private string _iSaveableUniqueID;
    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }

    // Second requirement -> dictionary containing items to be saved
    private GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }


    // After the event handles publishes an event for the new scene as being loaded, this method finds the parent game object (with tax ItemsParentTransform)
    // This is the parent directory of all items, to add all of the new saved items into
    private void AfterSceneLoad()
    {
        parentItem = GameObject.FindGameObjectWithTag(Tags.ItemsParentTransform).transform;
    }


    protected override void Awake()
    {
        base.Awake();

        // Use the generate GUID component on the item to get a unique ID for the item
        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        // GameObjectSave is a class with a dict of all the items to be saved
        GameObjectSave = new GameObjectSave();
    }


    /// <summary>
    /// Destroy items currently in the scene
    /// </summary>
    private void DestroySceneItems()
    {
        // Get all of the items in the scene
        Item[] itemsInScene = GameObject.FindObjectsOfType<Item>();

        // Loop through all of the scene items and destroy them
        for (int i = itemsInScene.Length - 1; i > -1; i--)
        {
            Destroy(itemsInScene[i].gameObject);
        }
    }


    // This will Instantiate a single sceneItem given an itemCode and position, using the prefab added to this class in the editor, within the items parent item
    public void InstantiateSceneItem(int itemCode, Vector3 itemPosition)
    {
        // Instantiate the prefab added via the editor to the given position
        GameObject itemGameObject = Instantiate(itemPrefab, itemPosition, Quaternion.identity, parentItem);
        Item item  = itemGameObject.GetComponent<Item>();
        item.Init(itemCode);
    }


    // Given a list of scene items, loop through them and instantiate each of them!
    private void InstantiateSceneItems(List<SceneItem> sceneItemList)
    {   
        GameObject itemGameObject;

        // Loop through all the items in the given list
        foreach(SceneItem sceneItem in sceneItemList)
        {   
            // Instantiate the game object with the prefab populated via the editor, at the sceneItems position, within the parent item gameobject
            itemGameObject = Instantiate(itemPrefab, new Vector3(sceneItem.position.x, sceneItem.position.y, sceneItem.position.z), Quaternion.identity, parentItem);

            // Populate the item code and name
            Item item = itemGameObject.GetComponent<Item>();
            item.ItemCode = sceneItem.itemCode;
            item.name = sceneItem.itemName;
        }
    }


    // Call IsaveableDeregister which removes this item from the SaveableObject List, and then unsubscribes to the AfterSceneLoad event
    private void OnDisable()
    {
        ISaveableDeregister();
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoad;
    }


    // Call IsaveableRegister which adds this item to the SaveableObject List, and then subscribe to the AfterSceneLoad event, so AfterSceneLoad is ran
    private void OnEnable()
    {
        ISaveableRegister();
        EventHandler.AfterSceneLoadEvent += AfterSceneLoad;
    }


    // Remove this item from the saveable objects list
    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }


    // This is a required method for the ISaveable interface, which passes in a GameObjectSave dictionary, and restores the current scene from it
    // The SaveLoadManager script will loop through all of the ISaveableRegister GameObjects, and trigger this ISaveableLoad, which will load that
    // Save data (here for ScenItems, for each scene (GameObjectSave is a Dict keyed by scene name)
    public void ISaveableLoad(GameSave gameSave)
    {
        // gameSave stores a Dictionary of items to save, see if there's one for this GUID
        if (gameSave.gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            // If we've found the save data, set the GridPropertiesManager GameObjectSave property with that save dictionary
            GameObjectSave = gameObjectSave;

            // Restore data for the current scene, with the GameObjectSave that was just updated from the save file, for the active scene
            ISaveableRestoreScene(SceneManager.GetActiveScene().name);
        }
    }


    // Add this item to the saveable objects list
    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }


    // This method will store the scene data for the current scene, and return a GameObjectSave, which just has a Dict of SceneSave data for each scene, keyed by scene name
    // This will get called from the SaveLoadManager, for each scene to save the dictionaries (GameObjectSave has a dict keyed by scene name)
    public GameObjectSave ISaveableSave()
    {
        // Store the current scene data
        ISaveableStoreScene(SceneManager.GetActiveScene().name);

        // Return the GameObjectSave, which has a dict of Saved stuff
        return GameObjectSave;
    }


    // core interfact method to restore all of the stored scene items!
    public void ISaveableRestoreScene(string sceneName)
    {   
        // First see if the GameObjectSave dictionary has an entry for this scene, sceneName : (sceneSave dictionary)
        if (GameObjectSave.sceneData.TryGetValue(sceneName, out SceneSave sceneSave))
        {
            // Then see if the item is not null, and if the ListSceneItem has entries in it
            if (sceneSave.listSceneItem != null)
            {
                // Scene list items  have been found!! destroy the existing items in the scene
                DestroySceneItems();

                // Now instantiate the list of scene items in the list
                InstantiateSceneItems(sceneSave.listSceneItem);
            }
        }
    }

    // One of the core methods, which will store all of the scene data, executed for every item in the SaveableObject list
    public void ISaveableStoreScene(string sceneName)
    {
        // Remove old scene save (dictionary keyed by scene name) for the gameObject if it exists
        GameObjectSave.sceneData.Remove(sceneName);

        // Get all of the items currently in the scene
        List<SceneItem> sceneItemList = new List<SceneItem>();
        Item[] itemsInScene = FindObjectsOfType<Item>();

        // Loop through all of the scene items, populate them, and add them to the sceneItemList
        foreach (Item item in itemsInScene)
        {   
            // Populate each sceneItem with their proper variables: item code, position, name  
            SceneItem sceneItem = new SceneItem();
            sceneItem.itemCode = item.ItemCode;
            sceneItem.position = new Vector3Serializable(item.transform.position.x, item.transform.position.y, item.transform.position.z);
            sceneItem.itemName = item.name;

            // Add the new scene item to the sceneItemList
            sceneItemList.Add(sceneItem);
        }

        // Create the list of scene items in the scene save and add to it to the sceneItemsList
        SceneSave sceneSave = new SceneSave();

        // Add this sceneItemList with all saveable items in the scene to the Item list
        sceneSave.listSceneItem = sceneItemList;

        // Add scene save to gameObjectSave a dictionary of sceneName: dict"sceneItemList" : list of saveable items)
        GameObjectSave.sceneData.Add(sceneName, sceneSave);
    }
}
