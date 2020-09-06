using System.Collections.Generic;

[System.Serializable]
public class SceneSave
{
    // String key is an identifier name that we choose for the given list (value)
    // So, every item in our scene will have a SceneItem instance with code, name, and position,
    // And they will all get added to the list as a value, for some unique identifier key.
    public Dictionary<string, List<SceneItem>> listSceneItemDictionary;
}
