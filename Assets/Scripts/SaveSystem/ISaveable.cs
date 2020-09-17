// Any class that utilizes this interface MUST have these properties and methods!
public interface ISaveable
{
    // Unique ID with a getter/setter on the item with this interface
    string ISaveableUniqueID { get; set; }

    // Dictionary with sceneName keys, and other dictionary with GUID as key, and a list of all the items for the value
    GameObjectSave GameObjectSave { get; set; }
    
    // Registers the saveable objects with the SaveLoadManager
    void ISaveableRegister();

    // Registers the saveable objects with the SaveLoadManager
    void ISaveableDeregister();

    // Returns a GameObjectSave object, storing the items in the scene to save
    GameObjectSave ISaveableSave();
    
    // Takes in a gameSave object (dictionary of GameObjectSave dictionaries), and loads the saved data
    void ISaveableLoad(GameSave gameSave);

    // Store sceneData for a scene
    void ISaveableStoreScene(string sceneName);

    // Store sceneData in a scene
    void ISaveableRestoreScene(string sceneName);
}
