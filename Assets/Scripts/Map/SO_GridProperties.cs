using System.Collections.Generic;
using UnityEngine;

// Create a editor menu to create these SO's
[CreateAssetMenu(fileName = "so_GridProperties", menuName = "Scriptable Objects/Grid Properties")]
public class SO_GridProperties : ScriptableObject
{
    // EVERY scene will have their own SO with sceneName given below
    public SceneName sceneName;
    
    // To account for varying-size scenes
    public int gridWidth;
    public int gridHeight;
    public int originX;
    public int originY;

    // SO is just a list of GridProperties for every square in the scene
    [SerializeField]
    public List<GridProperty> gridPropertyList;
}
