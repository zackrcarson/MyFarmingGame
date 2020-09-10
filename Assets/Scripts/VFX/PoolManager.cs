using System.Collections.Generic;
using UnityEngine;

// This singleton manages the pools we will set up for differen objects
public class PoolManager : SingletonMonobehaviour<PoolManager>
{   
    // The dictionary, keyed by the prefabs instance ID, valued by a queue of GameObjects of that same prefab, will store all of our pools, for each prefab type
    private Dictionary<int, Queue<GameObject>> poolDictionary = new Dictionary<int, Queue<GameObject>>();

    // Populated in inspector. The Pool struct contains the number of prefabs to create in the pool, and the prefab to use. In editor,
    // we will make an array of different pools (each with a prefab and size) we want to create. The transform is for the PoolManager,
    // and it's where we will store all of the pool objects under
    [SerializeField] private Pool[] pool = null;
    [SerializeField] private Transform objectPoolTransform = null;
     
    // This is the struct that defines a pool for an object, including the number of them to add to the queue, and the prefab it is 
    [System.Serializable]
    public struct Pool
    {
        public int poolSize;
        public GameObject prefab;
    }

    private void Start()
    {   
        // Create object pools on Start. Loop through all the pools we populated in editor, creating a Pool for each of them
        for (int i = 0; i < pool.Length; i++)
        {
            CreatePool(pool[i].prefab, pool[i].poolSize);
        }
    }

    // This method, called from Start() for each pool type populated in the editor, will create a pool of size poolSize of the prefab object, and
    // Add a queue of them to the poolDictionary, keyed by the given prefabs InstanceID, with it's value being the queue of prefab gameObjects
    private void CreatePool(GameObject prefab, int poolSize)
    {
        // The dictionary key for each pool is just the int InstanceID for the prefab we are going to populate the pool with
        int poolKey = prefab.GetInstanceID();
        string prefabName = prefab.name;

        // Create a parent gameObject for this pool, to parent the child pool objects to
        GameObject parentGameObject = new GameObject(prefabName + "Anchor"); 

        // Set this parent gameObject to be parented by our PoolManager gameObject
        parentGameObject.transform.SetParent(objectPoolTransform);

        // Only populate the pool if the dictionary doesn't already have a pool of this prefab type
        if (!poolDictionary.ContainsKey(poolKey))
        {
            // Add a Dict entry (keyed by the prefab InstanceID, and valued by the queue of objects)
            poolDictionary.Add(poolKey, new Queue<GameObject>());

            // Loop through the poolSize desired, each time Instantiating the prefab parented by the new parentGameObject
            for (int i = 0; i < poolSize; i++)
            {
                // Instantiate a new instance of this prefab, parented under parentGameObject, and set it to inactive (not in the scene atm)
                GameObject newObject = Instantiate(prefab, parentGameObject.transform) as GameObject;
                newObject.SetActive(false);

                // Add this created object to the qeueue at the current poolKey slot in our dictionary
                poolDictionary[poolKey].Enqueue(newObject);
            }
        }
    }


    // This method will reuse a gameObject from the poolDictionary, given a prefab, and a position/rotation to place it at. It returns the object to reuse!
    public GameObject ReuseObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {   
        // Find the pool key for this prefab
        int poolKey = prefab.GetInstanceID();

        // If the poolDictionary contains a pool of this prefab, 
        if (poolDictionary.ContainsKey(poolKey))
        {
            // Get the object from the front of the pool queue
            GameObject objectToReuse = GetObjectFromPool(poolKey);

            // reset the position, rotation, of the objectToReuse
            ResetObject(position, rotation, objectToReuse, prefab);

            return objectToReuse;
        }
        else
        {
            // If we don't have a pool for this object, return null and give a log out
            Debug.Log("No object pool for " + prefab);
            return null;
        }
    }


    // This method will simply take an object from the queue corresponding to the poolKey given, and add it back to the end of the queue, and return the object
    private GameObject GetObjectFromPool(int poolKey)
    {
        // Return the front object in the queue found in the poolDictionary at key poolKey
        GameObject objectToReuse = poolDictionary[poolKey].Dequeue();

        // Add another object of the same type back into the end of the queue
        poolDictionary[poolKey].Enqueue(objectToReuse);

        // Set the object to inactive if it's currently active in the scene
        if (objectToReuse.activeSelf == true)
        {
            objectToReuse.SetActive(false);
        }

        return objectToReuse;
    }


    // This method resets the objects position/rotation at the indicated position/rotation, and re-scales it back to the prefab scale
    private static void ResetObject(Vector3 position, Quaternion rotation, GameObject objectToReuse, GameObject prefab)
    {
        // Set the position/rotation to the desired places
        objectToReuse.transform.position = position;
        objectToReuse.transform.rotation = rotation;

        // rescale the size of the object to the prefab scale, if it had been resized for any reason
        objectToReuse.transform.localScale = prefab.transform.localScale;
    }
}
