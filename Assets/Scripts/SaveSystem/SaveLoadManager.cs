using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadManager : SingletonMonobehaviour<SaveLoadManager>
{
    // This class contains a Dictionary of GameObjectsSave's keyed by GUID
    public GameSave gameSave;

    // Hold a variety of objects in this list, which all use the ISaveable interface
    // This list is populated from EVERY object that uses the ISaveable interface (SceneItemsManager, GridPropertiesManager, Player, TimeManager, etc)
    // in the ISaveableRegister() methods. Here, we will loop through this list of objects that are registered, and save/load them
    public List<ISaveable> iSaveableObjectList;

    protected override void Awake()
    {
        base.Awake();

        iSaveableObjectList = new List<ISaveable>();
    }


    // This method will get called from our Pause menu GUI button for "Load Game", which will be linked here through the 'On click {}' field in the editor.
    // It will find the save data file, loop through all of the ISaveable objects, and call ISaveableLoad on all of them to restore the Save data
    public void LoadDataFromFile()
    {
        // This will deserialize our data
        BinaryFormatter bf = new BinaryFormatter();

        // Check if a save file exists
        if (File.Exists(Application.persistentDataPath + "/WildHopeCreek.dat"))
        {
            gameSave = new GameSave();

            // Open the data file for reading
            FileStream file = File.Open(Application.persistentDataPath + "/WildHopeCreek.dat", FileMode.Open);

            // Deserialize the data file into a GameSave object, which contains all of the save data in a dictionary, keyed by GUID
            gameSave = (GameSave)bf.Deserialize(file);

            // Loop through all of the ISaveable objects registered in iSaveableObjectList (all classes that use the ISaveable interface will register in this list. So it
            // contains SceneItemsManager.cs to save/load the scene items, the GridPropertiesManager to save/load gridProperties, etc. Each of those will then have a dictionary
            // keyed by the scene name for things to save/load), and then apply load data
            for (int i = iSaveableObjectList.Count - 1; i > -1; i--)
            {
                // gameSave.gameObjectData is the dictionary keyed by GUID
                if (gameSave.gameObjectData.ContainsKey(iSaveableObjectList[i].ISaveableUniqueID))
                {
                    // Execute the ISaveableLoad for the current ISaveable Object, which will restore the scene with that save type
                    iSaveableObjectList[i].ISaveableLoad(gameSave);
                }
                else
                {
                    // If we can't find the matching GUID, it isn't saved so just destroy it!
                    Component component = (Component)iSaveableObjectList[i];
                    Destroy(component.gameObject);
                }
            }

            // Close the file
            file.Close();
        }

        // Disable the pause screen so it just goes back into the loaded scene!
        UIManager.Instance.DisablePauseMenu();
    }


    // This method will get called from our Pause menu GUI button for "Save Game", which will be linked here through the 'On click {}' field in the editor.
    // It will
    public void SaveDataToFile()
    {
        gameSave = new GameSave();

        // Loop through all of the ISaveable objects registered in iSaveableObjectList (all classes that use the ISaveable interface will register in this list. So it
        // contains SceneItemsManager.cs to save/load the scene items, the GridPropertiesManager to save/load gridProperties, etc. Each of those will then have a dictionary
        // keyed by the scene name for things to save/load), and then generate the save data
        foreach (ISaveable iSaveableObject in iSaveableObjectList)
        {
            // For each iSaveableObject in the iSaveableObjectList, trigger the ISaveableSave method, which saves the dictionary (i.e. GridProperties, SceneItems, etc)
            // Those ISaveableSave methods return a GameObjectSave, which here we add it to the gameObjectdata, keyed by the GUID for the current iSaveableObject
            gameSave.gameObjectData.Add(iSaveableObject.ISaveableUniqueID, iSaveableObject.ISaveableSave());
        }

        // This will serialize our data
        BinaryFormatter bf = new BinaryFormatter();

        // create the data file for saving (overwrites if it exists)
        FileStream file = File.Open(Application.persistentDataPath + "/WildHopeCreek.dat", FileMode.Create);

        // Serialize the data file into the file, which contains all of the save data in a dictionary, keyed by GUID
        bf.Serialize(file, gameSave);

        // Close the file
        file.Close();

        // Disable the pause screen so it just goes back into the loaded scene!
        UIManager.Instance.DisablePauseMenu();
    }


    public void StoreCurrentSceneData()
    {
        // Loop through all of the ISaveable objects in the list and trigger the ISaveableStoreScene on THAT object (we know it'll have one because this list is only for objects
        // with the ISaveable interface), for each one
        foreach (ISaveable iSaveableObject in iSaveableObjectList)
        {
            iSaveableObject.ISaveableStoreScene(SceneManager.GetActiveScene().name);
        }
    }

    public void RestoreCurrentSceneData()
    {
        // Loop through all of the ISaveable objects in the list and trigger the ISaveableRestoreScene on THAT object (we know it'll have one because this list is only for objects
        // with the ISaveable interface), for each one
        foreach (ISaveable iSaveableObject in iSaveableObjectList)
        {
            iSaveableObject.ISaveableRestoreScene(SceneManager.GetActiveScene().name);
        }
    }
}
