using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ObscuringItemFader : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutRoutine());
    }

    public void FadeIn()
    {
        StartCoroutine(FadeInRoutine());
    }

    private IEnumerator FadeInRoutine()
    {
        float currentAlpha = spriteRenderer.color.a; // Current alpha of the sprite
        float distance = 1 - currentAlpha; // The remaining amount of alpha left to fade

        while (1 - currentAlpha > 0.01f)
        {
            currentAlpha = currentAlpha + distance / Settings.fadeInSeconds * Time.deltaTime;
            spriteRenderer.color = new Color(1f, 1f, 1f, currentAlpha);

            yield return null; // This will now yield back to the main game to come back again next frame, until this while loop exits
        }

        spriteRenderer.color = new Color(1f, 1f, 1f, 1f); // If the while loop is completed, simply set it back to original alpha
    }

    private IEnumerator FadeOutRoutine()
    {
        float currentAlpha = spriteRenderer.color.a; // Current alpha of the sprite
        float distance = currentAlpha - Settings.targetAlpha; // The remaining amount of alpha left to fade

        while (currentAlpha - Settings.targetAlpha > 0.01f)
        {
            currentAlpha = currentAlpha - distance / Settings.fadeOutSeconds * Time.deltaTime;
            spriteRenderer.color = new Color(1f, 1f, 1f, currentAlpha);

            yield return null; // This will now yield back to the main game to come back again next frame, until this while loop exits
        }

        spriteRenderer.color = new Color(1f, 1f, 1f, Settings.targetAlpha); // If the while loop is completed, simply set it to the target alpha
    }
}
