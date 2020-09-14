using System.Collections.Generic;

[System.Serializable]
public class SceneSave
{
    // These Boolean values are keyed by a string that we choose to identify what the list we are saving is (i.e. first time the scene has been loaded or not - we
    // only want to instantiate first-time objects like trees once when we first load the game!)
    public Dictionary<string, bool> boolDictionary; 

    // Every item in our scene will have a SceneItem instance with code, name, and position,
    // And they will all get added to the list of sceneItems to save
    public List<SceneItem> listSceneItem;

    // Dictionary storing the bool values on the grid Property Bool maps, keyed by the coordinate, with a value of bool property and value
    public Dictionary<string, GridPropertyDetails> gridPropertyDetailsDictionary;
}
