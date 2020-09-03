using UnityEngine;
using Cinemachine;

public class SwitchConfineBoundingShape : MonoBehaviour
{
    // Subscribe to the event that is called every time a scene is fully loaded
    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += SwitchBoundingShape;
    }


    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= SwitchBoundingShape;
    }

    /// <summary>
    /// Switch in the collider that cinemachine uses to define the edges of the screen. 
    /// This is called whenever a new scene is fully loaded.
    /// Here we find the gameObject with the correct tag, to get the bounds confiner polygon collider
    /// We do this here instead of in Start() because the scene load coroutine is additive and this will not be able to find it until the scene is fully loaded
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
