using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// This class (singleton! and interfaces with ISaveable!) will manage the gridProtertyDetails
[RequireComponent(typeof(GenerateGUID))]
public class GridPropertiesManager : SingletonMonobehaviour<GridPropertiesManager>, ISaveable
{
    private Tilemap groundDecoration1; // Dug ground tiles
    private Tilemap groundDecoration2; // Watered ground tiles
    
    // dug ground tile set as populated in the editor, so we can place various dug ground tiles as we hoe
    [SerializeField] private Tile[] dugGround = null;

    private Grid grid;

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


    // Remove all of the ground decorations (i.e. dug tiles (GD1) and watered tiles (GD2))
    private void ClearDisplayGroundDecorations()
    {
        groundDecoration1.ClearAllTiles();
        groundDecoration2.ClearAllTiles();
    }


    // This will display all visual grid property details (dug tiles, watered tiles, crops, etc.)
    private void ClearDisplayGridPropertyDetails()
    {
        ClearDisplayGroundDecorations();
    }


    public void DisplayDugGround(GridPropertyDetails gridPropertyDetails)
    {
        // Check to see if the current square passed in has been dug (o or more days since dug)
        if (gridPropertyDetails.daysSinceDug > -1)
        {
            ConnectDugGround(gridPropertyDetails);
        }
    }


    // Based on the passed in gridPropertyDetails about the square we are about to dig, this will determine what tile to place 
    // on the currently dug tile, and the four surrounding tiles
    private void ConnectDugGround(GridPropertyDetails gridPropertyDetails)
    {
        // Select tile based on surrounding dug tiles

        // Begin by setting the currently dug tile to what it needs to be based on the 4 surrounding tiles, as determined
        // by SetDugTile, which takes in the coordinates of the tile you want to dig, and returns the tile to use
        Tile dugTile0 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
        groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0), dugTile0);

        // Set 4 tiles if dug surrounding the current tile - up, down, left, right now that this central tile has been dug
        // With the proper tile

        GridPropertyDetails adjacentGridPropertyDetails;

        // Check the up tile (y+1), by getting the GridPropertyDetails there,and if it isn't null and it has been dug, use
        // The SetDugTile() method to determine what THAT tile should be based on ITS four neighbors, and so on and so forth..
        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceDug > -1)
        {
            Tile dugTile1 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1, 0), dugTile1);
        }

        // Check the down tile (y-1) in the same way
        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceDug > -1)
        {
            Tile dugTile2 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1, 0), dugTile2);
        }

        // Check the left tile (x-1) in the same way
        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceDug > -1)
        {
            Tile dugTile3 = SetDugTile(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY, 0), dugTile3);
        }

        // Check the right tile (x+1) in the same way
        adjacentGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
        if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceDug > -1)
        {
            Tile dugTile4 = SetDugTile(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY, 0), dugTile4);
        }
    }

    // This method, given the coordinates of a tile, checks it's 4 neighbors and determines which tile to place in the center.
    private Tile SetDugTile(int xGrid, int yGrid)
    {
        // Get whether the surrounding tiles (up, down, left, and right) are dug or not
        // The IsGridSquareDug(x,y) method simply checks if the given grid square is dug or not
        bool upDug = IsGridSquareDug(xGrid, yGrid + 1);
        bool downDug = IsGridSquareDug(xGrid, yGrid - 1);
        bool leftDug = IsGridSquareDug(xGrid - 1, yGrid);
        bool rightDug = IsGridSquareDug(xGrid + 1, yGrid);

        #region Set the appropriate tile based on whether the surrounding tiles are dug or not

        if (!upDug && !downDug && !rightDug && !leftDug)
        {
            // If all of the surrounding tiles are not dug, return the 1st element of the tile list populated in editor, which is a single tile not connected to anything
            return dugGround[0];
        }

        else if (!upDug && downDug && rightDug && !leftDug)
        {
            // and so on and so forth for all 16 combinations...
            return dugGround[1];
        }

        else if (!upDug && downDug && rightDug && leftDug)
        {
            return dugGround[2];
        }

        else if (!upDug && downDug && !rightDug && leftDug)
        {
            return dugGround[3];
        }

        else if (!upDug && downDug && !rightDug && !leftDug)
        {
            return dugGround[4];
        }

        else if (upDug && downDug && rightDug && !leftDug)
        {
            return dugGround[5];
        }

        else if (upDug && downDug && rightDug && leftDug)
        {
            return dugGround[6];
        }

        else if (upDug && downDug && !rightDug && leftDug)
        {
            return dugGround[7];
        }

        else if (upDug && downDug && !rightDug && !leftDug)
        {
            return dugGround[8];
        }

        else if (upDug && !downDug && rightDug && !leftDug)
        {
            return dugGround[9];
        }

        else if (upDug && !downDug && rightDug && leftDug)
        {
            return dugGround[10];
        }

        else if (upDug && !downDug && !rightDug && leftDug)
        {
            return dugGround[11];
        }

        else if (upDug && !downDug && !rightDug && !leftDug)
        {
            return dugGround[12];
        }

        else if (!upDug && !downDug && rightDug && !leftDug)
        {
            return dugGround[13];
        }

        else if (!upDug && !downDug && rightDug && leftDug)
        {
            return dugGround[14];
        }

        else if (!upDug && !downDug && !rightDug && leftDug)
        {
            return dugGround[15];
        }

        // return null if none of the cases are reached..
        return null;

        #endregion Set the appropriate tile based on whether the surrounding tiles are dug or not
    }


    // This method just checks if the given grid square has been dug or not, returning a bool
    private bool IsGridSquareDug(int xGrid, int yGrid)
    {
        // Get the gridPropertyDetails for that grid square
        GridPropertyDetails gridPropertyDetails = GetGridPropertyDetails(xGrid, yGrid);

        // If theres nothing there, just return false
        if (gridPropertyDetails == null)
        {
            return false;
        }

        // If it has been dug, return true
        else if (gridPropertyDetails.daysSinceDug > -1)
        {
            return true;
        }

        // Any other situation, return false
        else
        {
            return false;
        }
    }


    // Called from the ISaveableRestoreScene method, to display all of the dug grid squares
    private void DisplayGridPropertyDetails()
    {
        // Loop through all of the grid items in the gridproperty dictionary, and displaying the dug ground tile as determined in DisplayDugGround()
        foreach (KeyValuePair<string, GridPropertyDetails> item in gridPropertyDictionary)
        {
            GridPropertyDetails gridPropertyDetails = item.Value;

            DisplayDugGround(gridPropertyDetails);
        }
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

        // Get the tilemaps for ground decoration (watered tiles, dug tiles), which can only be grabbed once the scene is loaded
        groundDecoration1 = GameObject.FindGameObjectWithTag(Tags.GroundDecoration1).GetComponent<Tilemap>(); // Dug ground
        groundDecoration2 = GameObject.FindGameObjectWithTag(Tags.GroundDecoration2).GetComponent<Tilemap>(); // Watered ground
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

            // Check if grid properties exist
            if (gridPropertyDictionary.Count > 0)
            {
                // GridPropertyDetails found for the current scene, destroy the existing ground decoration (dug tiles, watered tiles)
                ClearDisplayGridPropertyDetails();

                // Instantiate the grid property details for the current scene 
                DisplayGridPropertyDetails();
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