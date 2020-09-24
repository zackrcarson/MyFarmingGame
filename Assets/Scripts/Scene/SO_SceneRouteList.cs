using System.Collections.Generic;
using UnityEngine;

// This SO will simply store a list of SceneRoutes (each storing to/from scenes, and a list of scenePaths in those scenes), to tell us how to traverse different sets of scenes
// This way scripts like NPCMovement can access the SO to transport between scenes correctly
[CreateAssetMenu(fileName = "so_SceneRouteList", menuName = "Scriptable Objects/Scene/Scene Route List")]
public class SO_SceneRouteList : ScriptableObject
{
    public List<SceneRoute> sceneRouteList;
}