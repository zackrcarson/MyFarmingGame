using System.Collections.Generic;

// This class is serializable, so we can serialize the data into a file
[System.Serializable]
public class GameSave
{
    // This dictionary has a string key = GUID GameObject ID, and a value of GameObjectSave, which is the class that stores a Dict of scene items to save
    public Dictionary<string, GameObjectSave> gameObjectData;

    public GameSave()
    {
        gameObjectData = new Dictionary<string, GameObjectSave>();
    }
}
