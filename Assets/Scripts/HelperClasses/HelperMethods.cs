using System.Collections.Generic;
using UnityEngine;

public static class HelperMethods
{
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
}
