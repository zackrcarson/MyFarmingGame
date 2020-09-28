using System.Collections;
using UnityEngine;

public class ItemNudge : MonoBehaviour
{
    private WaitForSeconds pause;
    private bool isAnimating = false;

    private void Awake()
    {
        pause = new WaitForSeconds(0.04f);
    }

    // This is called when the player enters a collision with an item with a trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        CircleCollider2D collider = collision.GetComponent<CircleCollider2D>();

        // Only start animating it if it's currently not animating - just in case we get duplicate instances before animation ends
        if (isAnimating == false)
        {
            
            // If the collision location (where the player hit the item) is to the right of the objects center, rotate it anti-clockwise. Else, do it clockwise
            if (gameObject.transform.position.x < collision.gameObject.transform.position.x)
            {
                StartCoroutine(RotateAntiClock());
            }
            else
            {
                StartCoroutine(RotateClock());
            }

            if (collision.gameObject.tag == "Player")
            {
                // If it's the player walking through the reapable scenary, play the rustle sound when we enter the collider
                AudioManager.Instance.PlaySound(SoundName.effectRustle);
            }
        }
    }

    // This is called when the player exits a collision with an item with a trigger
    private void OnTriggerExit2D(Collider2D collision)
    {
        // Only start animating it if it's currently not animating - just in case we get duplicate instances before animation ends
        if (isAnimating == false)
        {   
            // If the collision exit location (where the player hit the item) is to the right of the objects center, rotate it clockwise. Else, do it anti-clockwise
            if (gameObject.transform.position.x > collision.gameObject.transform.position.x)
            {
                StartCoroutine(RotateAntiClock());
            }
            else
            {
                StartCoroutine(RotateClock());
            }

            // If it's the player walking through the reapable scenary, play the rustle sound when we exit the collider
            if (collision.gameObject.tag == "Player")
            {
                AudioManager.Instance.PlaySound(SoundName.effectRustle);
            }
        }
    }

    // This coroutine will rotate the child object back and forth a little bit each frame 
    private IEnumerator RotateAntiClock()
    {
        // Set the animating bool to true so it can't start over again until it's over
        isAnimating = true;

        // Start rotating it anti-clockwise in 4 increments of 2 degrees each
        for (int i = 0; i < 4; i++)
        {
            gameObject.transform.GetChild(0).Rotate(0f, 0f, 2f);
            // on yield return, it will pause for 'pause' seconds and then come back to do the next step, and so on and so forth.
            yield return pause;
        }

        // Next rotate it clockwise in 5 increments of 2 degrees each, returning to the original position, and then 2 more degrees past it
        for (int i = 0; i < 5; i++)
        {
            gameObject.transform.GetChild(0).Rotate(0f, 0f, -2f);
            yield return pause;
        }
        
        // Finally, rotate it anti-clockwise again 1 degrees to return to the original position 
        gameObject.transform.GetChild(0).Rotate(0f, 0f, 2f);
        yield return pause;

        // Set the animating bool to false so now it can start over again if triggered
        isAnimating = false;
    }

    private IEnumerator RotateClock()
    {
        // Set the animating bool to true so it can't start over again until it's over
        isAnimating = true;

        // Start rotating it clockwise in 4 increments of 2 degrees each
        for (int i = 0; i < 4; i++)
        {
            gameObject.transform.GetChild(0).Rotate(0f, 0f, -2f);
            // on yield return, it will pause for 'pause' seconds and then come back to do the next step, and so on and so forth.
            yield return pause;
        }

        // Next rotate it anti-clockwise in 5 increments of 2 degrees each, returning to the original position, and then 2 more degrees past it
        for (int i = 0; i < 5; i++)
        {
            gameObject.transform.GetChild(0).Rotate(0f, 0f, 2f);
            yield return pause;
        }
        
        // Finally, rotate it clockwise again 1 degrees to return to the original position 
        gameObject.transform.GetChild(0).Rotate(0f, 0f, -2f);
        yield return pause;

        // Set the animating bool to false so now it can start over again if triggered
        isAnimating = false;
    }
}
