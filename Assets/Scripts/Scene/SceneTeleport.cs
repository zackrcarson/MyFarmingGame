using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class SceneTeleport : MonoBehaviour
{
    [SerializeField] private SceneName sceneNameGoto = SceneName.Scene1_Farm;
    [SerializeField] private Vector3 scenePositionGoto = new Vector3(); 

    public bool needsClick = false;

    private void OnTriggerStay2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();

        if (player != null)
        {
            // Calculate the players new position
            // This is a ternary operator, so if the goto x/y position is ~ 0, set the x/y Position as the players x/y position.
            // If it's not, return the goto position
            // Basically, if you didn't specify a goto x/y position in particular (the goto vector3 has default values of 0), go to the players current x/y position.
            // If you DID specify one, go to that instead 
            float xPosition = Mathf.Approximately(scenePositionGoto.x, 0f) ? player.transform.position.x : scenePositionGoto.x;
            float yPosition = Mathf.Approximately(scenePositionGoto.y, 0f) ? player.transform.position.y : scenePositionGoto.y;

            float zPosition = 0f;

            // Teleport to the new scene
            if (needsClick) // I added this check, so that if you want it to be a click-to-teleport (like entering house doors!), in addition to being in the collider trigger, you also need to click 'E' to enter
            {
                if (Input.GetKey(KeyCode.E))
                {
                    SceneControllerManager.Instance.FadeAndLoadScene(sceneNameGoto.ToString(), new Vector3(xPosition, yPosition, zPosition));
                }
            }
            else
            {
                SceneControllerManager.Instance.FadeAndLoadScene(sceneNameGoto.ToString(), new Vector3(xPosition, yPosition, zPosition));
            }
        }
    }
}
