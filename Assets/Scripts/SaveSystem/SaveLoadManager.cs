using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SaveLoadManager : SingletonMonobehaviour<SaveLoadManager>
{
    // Hold a variety of objects in this list, which all use the ISaveable interface
    public List<ISaveable> iSaveableObjectList;

    protected override void Awake()
    {
        base.Awake();

        iSaveableObjectList = new List<ISaveable>();
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
