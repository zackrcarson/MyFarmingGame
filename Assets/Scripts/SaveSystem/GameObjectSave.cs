using System.Collections.Generic;

[System.Serializable]
public class GameObjectSave 
{
    // String key is the scene name, and the value is a SceneSave type, which is 
    // a dictionary of lists of all of the items in the scene 
    // So for each object we want to be saveable, the SceneSave dictionary will have a
    // list of all of the currently instantiated items of that type
    public Dictionary<string, SceneSave> sceneData;

    // First constructor will instantiate the dictionary, the second one will allow us to 
    // Pass in sceneData to be created in the dict
    public GameObjectSave()
    {
        sceneData = new Dictionary<string, SceneSave>();
    }

    public GameObjectSave(Dictionary<string, SceneSave> sceneData)
    {
        this.sceneData = sceneData;
    }
}
