using System.Collections;
using UnityEngine;

// Attached to all crops in the scene. For now, this just stores the grid position of the crop
public class Crop : MonoBehaviour
{
    [HideInInspector]
    public Vector2Int cropGridPosition;
}
