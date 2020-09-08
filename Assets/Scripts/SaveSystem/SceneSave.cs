using System.Collections.Generic;

[System.Serializable]
public class SceneSave
{
    // Every item in our scene will have a SceneItem instance with code, name, and position,
    // And they will all get added to the list of sceneItems to save
    public List<SceneItem> listSceneItem;

    // Dictionary storing the bool values on the grid Property Bool maps, keyed by the coordinate, with a value of bool property and value
    public Dictionary<string, GridPropertyDetails> gridPropertyDetailsDictionary;
}
