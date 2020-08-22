using UnityEngine;
using Cinemachine;

public class SwitchConfineBoundingShape : MonoBehaviour
{
    // Start is called before the first frame update. This will then switch the boundsConfiner component to the polygon collider
    void Start()
    {
        SwitchBoundingShape();
    }

    /// <summary>
    /// Switch in the collider that cinemachine uses to define the edges of the screen
    /// </summary>

    private void SwitchBoundingShape()
    {
        // Get the polygon collider on the 'BoundsConfiner' gameObject (in the Scene1_farm scene) which is used 
        // by Cinemachine (in the PersistentScene) to prevent the camera from going beyond the screen edges
        PolygonCollider2D polygoncollider2D = GameObject.FindGameObjectWithTag(Tags.BoundsConfiner).GetComponent<PolygonCollider2D>();

        CinemachineConfiner cinemachineConfiner = GetComponent<CinemachineConfiner>();

        cinemachineConfiner.m_BoundingShape2D = polygoncollider2D;

        // Since the confiner bounds have been changed, call this to clear the cache

        cinemachineConfiner.InvalidatePathCache();
    }
}
