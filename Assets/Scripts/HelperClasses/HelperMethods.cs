using System.Collections.Generic;
using UnityEngine;

public static class HelperMethods
{
<<<<<<< HEAD

    /// <summary>
    /// Gets the components of type T at positionToCheck. Returns true if at least one is found, and the found components are returned in componentAtPositionList
    /// </summary>
    public static bool GetComponentsAtCursorLocation<T>(out List<T> componentsAtPositionList, Vector3 positionCheck)
    { // This method has a generic type T, so it can be used for any type T that you pass in (say, T = Item)

        // Bool indicating if we've found any type Ts at the cursor location
        bool found = false;
        
        // List containing all objects of type T found at the location
        List<T> componentList = new List<T>();

        // Array of all 2Dcollider objects found that overlap at that point in space
        Collider2D[] collider2DArray = Physics2D.OverlapPointAll(positionCheck);

        // Loop through all of the colliders to get objects of type T

        T tComponent = default(T); // This is just the default type T

        for (int i = 0; i < collider2DArray.Length; i++)
        {   
            // First check to see if we have any components in the Parent gameObject
            tComponent = collider2DArray[i].gameObject.GetComponentInParent<T>();

            if (tComponent != null)
            {
                // If a component in parent gameObject of type T is found, set the bool to true and add the found component to the list
                found = true;
                componentList.Add(tComponent);
            }
            else
            {   
                // Now check in the children GameObjects!
                tComponent = collider2DArray[i].gameObject.GetComponentInChildren<T>();

                if (tComponent != null)
                {
                    found = true;
                    componentList.Add(tComponent);
                }
            }
        }

        componentsAtPositionList = componentList;

        return found;
    }


=======
>>>>>>> 06b270bebb0960c9d8506772aa1531ea81c70c95
    /// <summary>
    /// Gets the components of type T at the box with center point point, size size, and angle angle. Returns tru if at least one is found, and the found components are returned in the list
    /// </summary>
    public static bool GetComponentsAtBoxLocation<T>(out List<T> listComponentsAtBoxPosition, Vector2 point, Vector2 size, float angle)
    { // This method has a generic type T, so it can be used for any type T that you pass in (say, T = Item)

        // Bool indicating if we've found any type Ts in the box
        bool found = false;

        // List containing all objects of type T found in the box
        List<T> componentList = new List<T>();

        // Array of all 2Dcollider objects found in the box given
        Collider2D[] collider2DArray = Physics2D.OverlapBoxAll(point, size, angle);

        // Loop through all of the colliders to get all objects of type T
        for (int i = 0; i < collider2DArray.Length; i++)
        {
            // First check to see if we have any components in the Parent gameObject
            T tComponent = collider2DArray[i].gameObject.GetComponentInParent<T>();

            if (tComponent != null)
            {
                // If a component in parent gameObject of type T is found, set the bool to true and add the found component to the list
                found = true;
                componentList.Add(tComponent);
            }
            else
            {   
                // Now check in the children GameObjects!
                tComponent = collider2DArray[i].gameObject.GetComponentInChildren<T>();

                if (tComponent != null)
                {
                    found = true;
                    componentList.Add(tComponent);
                }
            }
        }
        
        // The out parameter is the list of all objects of type T found in the box
        listComponentsAtBoxPosition = componentList;

        return found;
    }
<<<<<<< HEAD


    /// <summary>
    /// Returns an array of components of type T at box with centerPoint, size, and angle. The numberOfCollidersToTest for is passed as a parameter.
    /// Found components are returned in the array. This method returns them in an array so there is less overhead if we're using it often
    /// </summary>
    public static T[] GetComponentsAtBoxLocationNonAlloc<T>(int numberOfCollidersToTest, Vector2 point, Vector2 size, float angle)
    { // This method has a generic type T, so it can be used for any type T that you pass in (say, T = Item)

        // Create a new Collider2D array, that will be used to be populated with the next method
        Collider2D[] collider2DArray = new Collider2D[numberOfCollidersToTest];
        
        // This method will return all colliders that overlap with the given box area. This is memory efficient (doesn't allocate it's own memory at it goes), 
        // and doesn't start garbage collection as frequently so it'll have less overhead, especially because we'll use it a lot
        Physics2D.OverlapBoxNonAlloc(point, size, angle, collider2DArray);

        // This is just the default type T
        T tComponent = default(T); 

        // Array to store the found objects of type T in
        T[] componentArray = new T[collider2DArray.Length];

        // Loop through all of the colliders to get objects of type T
        for (int i = collider2DArray.Length - 1; i >= 0; i--)
        {   
            if (collider2DArray[i] != null)
            {
               tComponent = collider2DArray[i].gameObject.GetComponent<T>();

                //Check if the found component of type T was found or not, if so, add it to our outpit array
                if (tComponent != null)
                {
                    componentArray[i] = tComponent;
                }
            }
        }

        return componentArray;
    }
=======
>>>>>>> 06b270bebb0960c9d8506772aa1531ea81c70c95
}
