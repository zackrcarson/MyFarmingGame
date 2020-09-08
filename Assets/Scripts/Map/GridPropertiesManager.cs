using System.Collections.Generic;
using UnityEngine;

// This class (singleton! and interfaces with ISaveable!) will manage the gridProtertyDetails
[RequireComponent(typeof(GenerateGUID))]
public class GridPropertiesManager : SingletonMonobehaviour<GridPropertiesManager>, ISaveable
{
    public Grid grid;

    // This is the dictionary storing our gridPropertyDetails for every square, keyed by a string of the coordinates, and value
    // GridPropertyDetails, which contains the coordinates, all the bool values, and planting-related ints
    private Dictionary<string, GridPropertyDetails> gridPropertyDictionary;

    // This field is populated in the editor, which is an array of the gridProperties SO's from each scene.
    [SerializeField] private SO_GridProperties[] so_gridPropertiesArray = null;

    // Required by the ISaveable interface, which is the unique GUID
    private string _iSaveableUniqueID;
    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }

    // Required by the ISaveable interface, which is the game object save class for storing a dictionary to be saved
    private GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }


    protected override void Awake()
    {
        base.Awake();

        // Populate the unique ID from the GenerateGUID script in editor
        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }


    // Calls ISaveableRegister to add the item to the SaveLoadManager iSaveable List, and subscribes the AfterSceneLoaded method to the AfterSceneLoadEvent
    // This method will grab the grid from the tilemap, because we must do this AFTER the scene has been loaded
    private void OnEnable()
    {
        ISaveableRegister();

        EventHandler.AfterSceneLoadEvent += AfterSceneLoaded;
    }


    // Calls ISaveableDeregister to remove the item from the SaveLoadManager iSaveable List, and unsubscribes the AfterSceneLoaded method to the AfterSceneLoadEvent
    private void OnDisable()
    {
        ISaveableDeregister();

        EventHandler.AfterSceneLoadEvent -= AfterSceneLoaded;
    }

    
    // On game start, this will initialise the grid properties! This will take all of the elements in the SO for each grid square, and 
    // Populate the grid Property dictionary for storing bool values
    private void Start()
    {
        InitialiseGridProperties();
    }


    /// <summary>
    /// This initialises the grid property dictionary with the values from the SO_GridProperties assets and stores the values for each scene in
    /// a GameObjectSave sceneData
    /// </summary>
    private void InitialiseGridProperties()
    {
        // Loop through all of the gridProperties in the array of scene SO's (one for each scene, containing all of the grid properties)
        foreach (SO_GridProperties so_GridProperties in so_gridPropertiesArray)
        {
            // Create a dictionary of grid property details. This is what we'll store the GridPropertyDetails in
            Dictionary<string, GridPropertyDetails> gridPropertyDictionary = new Dictionary<string, GridPropertyDetails>();

            // Populate the grid details dictionary - iterate through all of the grid properties in the SO_gridPropertiesList
            foreach (GridProperty gridProperty in so_GridProperties.gridPropertyList)
            {
                GridPropertyDetails gridPropertyDetails;
                
                // For each square in this list, this method will return the property details (coordinate, bools, planting ints, etc) (or null if nothing there)
                gridPropertyDetails = GetGridPropertyDetails(gridProperty.gridCoordinate.x, gridProperty.gridCoordinate.y, gridPropertyDictionary);
                
                // If we haven't gotten a gridPropertyDetails, make a new empty one with default values
                if (gridPropertyDetails == null)
                {
                    gridPropertyDetails = new GridPropertyDetails();
                }

                // Check which GridBoolProperty (enum) is stored in the gridProperty, and set the corresponding value in the gridPropertyDetails
                switch (gridProperty.gridBoolProperty)
                {
                    case GridBoolProperty.diggable:
                        gridPropertyDetails.isDiggable = gridProperty.gridBoolValue;
                        break;

                    case GridBoolProperty.canDropItem:
                        gridPropertyDetails.canDropItem = gridProperty.gridBoolValue;
                        break;

                    case GridBoolProperty.canPlaceFurniture:
                        gridPropertyDetails.canPlaceFurniture = gridProperty.gridBoolValue;
                        break;

                    case GridBoolProperty.isPath:
                        gridPropertyDetails.isPath = gridProperty.gridBoolValue;
                        break;

                    case GridBoolProperty.isNPCObstacle:
                        gridPropertyDetails.isNPCObstacle = gridProperty.gridBoolValue;
                        break;

                    default:
                        break;
                }

                // Pass the gridPropertyDetails and dictionary into this method, which sets up the current squares gridPropertyDetails in the dictionary
                SetGridPropertyDetails(gridProperty.gridCoordinate.x, gridProperty.gridCoordinate.y, gridPropertyDetails, gridPropertyDictionary);
            }

            // Now that we've looped through all of the values in the square, Create a sceneSave list for this GameObject, which will allow us to save it
            SceneSave sceneSave = new SceneSave();

            // Add grid property dictionary to scene save data gridPropertyDetailsDictionary
            sceneSave.gridPropertyDetailsDictionary = gridPropertyDictionary;

            // If the current scene is the starting scene, set the gridPropertyDictionary member variable to the current iteration
            if (so_GridProperties.sceneName.ToString() == SceneControllerManager.Instance.startingSceneName.ToString())
            {
                this.gridPropertyDictionary = gridPropertyDictionary;
            }

            // Add the sceneSave to the GameObjectSave dictionary, keyed by the scene name
            GameObjectSave.sceneData.Add(so_GridProperties.sceneName.ToString(), sceneSave);
        }
    }


    // This method will be automatically called when the EventHandler publishes an AfterSceneLoadEvent, and here we will populate grid with the Grid gameobject
    private void AfterSceneLoaded()
    {
        // Get the grid! Tilemaps always have a grid component
        grid = GameObject.FindObjectOfType<Grid>();
    }


    /// <summary>
    /// Returns the gridPropertyDetails at the gridLocation for the supplied dictionary, or null if no properties exist at that location.
    /// </summary>
    public GridPropertyDetails GetGridPropertyDetails(int gridX, int gridY, Dictionary<string, GridPropertyDetails> gridPropertyDictionary)
    {
        // Construct the key from the coordinates
        string key = "x" + gridX + "y" + gridY;

        GridPropertyDetails gridPropertyDetails;

        // Check if the grid property details exist for the given coordinates, and retrieve it and return it if not null
        if (!gridPropertyDictionary.TryGetValue(key, out gridPropertyDetails))
        {
            // If not found, return null
            return null;
        }
        else
        {
            // Else, return the property details
            return gridPropertyDetails;
        }
    }


    /// <summary>
    /// Get the grid property details for the tile at (gridX, gridY). If no grid property details exist,
    /// We return null and can assume that all the grid property details values are null or false
    /// </summary>
    public GridPropertyDetails GetGridPropertyDetails(int gridX, int gridY)
    {
        return GetGridPropertyDetails(gridX, gridY, gridPropertyDictionary);
    }


    // Required method in the ISaveable interface. This will deregister the current game object with the SaveLoadManager's iSaveableObject list!
    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }


    // Required method in the ISaveable interface. This will register the current game object with the SaveLoadManager's iSaveableObject list!
    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }


    // Required method for the ISaveable interface. This will get called as after loading the new scene. This will restore the 
    // gridPropertyDetailsDictionary with square bool values for the entered scene 
    public void ISaveableRestoreScene(string sceneName)
    {
        // Get sceneSave for the scene - it exists because we created it in initialise
        if (GameObjectSave.sceneData.TryGetValue(sceneName, out SceneSave sceneSave))
        {
            // Get the grid property details dictionary - it exists because we created it in initialise
            if (sceneSave.gridPropertyDetailsDictionary != null)
            {
                gridPropertyDictionary = sceneSave.gridPropertyDetailsDictionary;
            }
        }
    }


    // Required method for the ISaveable interface. This will get called as we're moving inbetween scenes. This will save all of the gridPropertyDetailsDictionary
    // To be used again when we enter the scene again
    public void ISaveableStoreScene(string sceneName)
    {
        // Remove sceneSave for the scene
        GameObjectSave.sceneData.Remove(sceneName);

        // Create sceneSave for scene
        SceneSave sceneSave = new SceneSave();

        // Create and add dict grid property details dictionary
        sceneSave.gridPropertyDetailsDictionary = gridPropertyDictionary;

        // Add scene save to the game object save scene data
        GameObjectSave.sceneData.Add(sceneName, sceneSave);
    }


    public void SetGridPropertyDetails(int gridX, int gridY, GridPropertyDetails gridPropertyDetails)
    {
        SetGridPropertyDetails(gridX, gridY, gridPropertyDetails, gridPropertyDictionary);
    }

    
    /// <summary>
    /// Set the grid property details to gridPropertyDetails for the tile at (gridX, gridY) in the gridPropertyDictionary
    /// </summary>
    public void SetGridPropertyDetails(int gridX, int gridY, GridPropertyDetails gridPropertyDetails, Dictionary<string, GridPropertyDetails> gridPropertyDictionary)
    {
        // Construct the key from the coordinates
        string key = "x" + gridX + "y" + gridY;

        gridPropertyDetails.gridX = gridX;
        gridPropertyDetails.gridY = gridY;

        // Set the value
        gridPropertyDictionary[key] = gridPropertyDetails;
    }

}