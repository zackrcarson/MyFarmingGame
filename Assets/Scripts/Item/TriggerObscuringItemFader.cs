using UnityEngine;

public class TriggerObscuringItemFader : MonoBehaviour
{
    // This method is called when a 2D collider enters a collision, which passes in the collision object
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Get the gameObject we have collided with, and then get all the ObscuringItemFader components on it and its children, and then trigger the fade out

        // Gets a list of all of the ObscuringItemFader objects in both parents and children of the collided object
        ObscuringItemFader[] obscuringItemFader = collision.gameObject.GetComponentsInChildren<ObscuringItemFader>(); 

        if (obscuringItemFader.Length > 0)
        {
            for (int i = 0; i < obscuringItemFader.Length; i++)
            {
                // For each ObscuringItemFader found in the collided object, trigger the FadeOut Method
                obscuringItemFader[i].FadeOut();
            }
        }
    }

    // This method is called when a 2D collider leaves a collision, which passes in the collision object
    private void OnTriggerExit2D(Collider2D collision)
    {
        // Get the gameObject we have collided with, and then get all the ObscuringItemFader components on it and its children, and then trigger the fade in

        // Gets a list of all of the ObscuringItemFader objects in both parents and children of the collided object
        ObscuringItemFader[] obscuringItemFader = collision.gameObject.GetComponentsInChildren<ObscuringItemFader>(); 

        if (obscuringItemFader.Length > 0)
        {
            for (int i = 0; i < obscuringItemFader.Length; i++)
            {
                // For each ObscuringItemFader found in the collided object, trigger the FadeIn Method
                obscuringItemFader[i].FadeIn();
            }
        }
    }
}
