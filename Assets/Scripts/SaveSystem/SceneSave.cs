using System.Collections.Generic;

[System.Serializable]
public class SceneSave
{
    // These Boolean values are keyed by a string that we choose to identify what the list we are saving is (i.e. first time the scene has been loaded or not - we
    // only want to instantiate first-time objects like trees once when we first load the game!)
    public Dictionary<string, bool> boolDictionary;

    // Dictionary storing which scene the player was in when the game was saved, and the direction they are facing. The key is just an identifier we pick to label it,
    // i.e. "currentScene" or "playerDirection".
    public Dictionary<string, string> stringDictionary;

    // Dictionary storing the players location (via a serializable Vector3) when the game was saved. The key is just an identifier we pick to label it (i.e. "playerPosition")
    public Dictionary<string, Vector3Serializable> vector3Dictionary;

    // Every item in our scene will have a SceneItem instance with code, name, and position,
    // And they will all get added to the list of sceneItems to save
    public List<SceneItem> listSceneItem;

    // Dictionary storing the bool values on the grid Property Bool maps, keyed by the coordinate, with a value of bool property and value
    public Dictionary<string, GridPropertyDetails> gridPropertyDetailsDictionary;

    // This will be a list of inventory items to save/load
    public List<InventoryItem>[] listInvItemArray;

    // This dictionary will store the inventory capacity arrays, showing the inventory location capacities (i.e. how many items the players inventory can hold, the chest can hold, etc). The 
    // key is just an identifier we pick to label it (i.e. "inventoryListCapacityArray")
    public Dictionary<string, int[]> intArrayDictionary;
}
