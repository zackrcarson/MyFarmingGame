using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneControllerManager : SingletonMonobehaviour<SceneControllerManager>
{
    private bool isFading;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private CanvasGroup faderCanvasGroup = null;
    [SerializeField] private Image faderImage = null;
    public SceneName startingSceneName;


    // Fade coroutine to slowly fade in our out the scene over a certain number of frames
    private IEnumerator Fade(float finalAlpha)
    {
        // Set the isFading flag to true so that the FadeAndSwitchScenes corouting won't be called again during the fade 
        isFading = true;

        // Make sure the Canvas group blocks raycasts into the scene so no more input can be accepted.
        faderCanvasGroup.blocksRaycasts = true;

        // Calculate how fast the CanvasGroup should fade based on it's current alpha, it's final alpha, and how long it has to change between the two
        float fadeSpeed = Mathf.Abs(faderCanvasGroup.alpha - finalAlpha) / fadeDuration;

        // While the CanvasGroup hasn't reached it's final alpha yet...
        // Yield return every frame to fade it a little bit more!
        // Approximately checks if they are approximately the same (floats are hard to be exactly the same)
        while (!Mathf.Approximately(faderCanvasGroup.alpha, finalAlpha))
        {
            // ... Move the alpha slightly towards its target alpha
            faderCanvasGroup.alpha = Mathf.MoveTowards(faderCanvasGroup.alpha, finalAlpha, fadeSpeed * Time.deltaTime);

            // Wait for a frame before continuing
            yield return null;
        }

        // Set the flag back to false once the scene has faded completely
        isFading = false;

        // Stop the CanvasGroup from blocking raycasts so input is no longer ignore
        faderCanvasGroup.blocksRaycasts = false;
    }


    // This is the coroutine where the building blocks of the script are put together, to slowly fade the scene and load the new one
    private IEnumerator FadeAndSwitchScenes(string sceneName, Vector3 spawnPosition)
    {
        // Call this event before the scene fade out event, so subscribers know
        EventHandler.CallBeforeSceneUnloadFadeOutEvent();

        // Start fading to black and wait for it to finish before continuing
        // This is calling another corouting so it will wait until the inner coroutine finishes
        yield return StartCoroutine(Fade(1f));

        // Set the player's position
        Player.Instance.gameObject.transform.position = spawnPosition;

        // Call this event before unloading the scene
        EventHandler.CallBeforeSceneUnloadEvent();

        // Unload the current active scene over a certain number of frames, waiting to continue until it's done
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);

        // Start loading the given new scene and wait for it to finish
        yield return StartCoroutine(LoadSceneAndSetActive(sceneName));

        // Call this event after loading the scene
        EventHandler.CallAfterSceneLoadEvent();

        // Start fading back in and wait for it to finish before exiting the function
        yield return StartCoroutine(Fade(0f));

        // Finally, Call this event after the fade in
        EventHandler.CallAfterSceneLoadFadeInEvent();
    }


    // This coroutine slowly loads in the new scene over a certain number of frames, and then marks it as active
    private IEnumerator LoadSceneAndSetActive(string sceneName)
    {
        // Alow the given scene to load over several frames and add it to the already loaded scenes (additive flag - just the persistent scene at this point!)
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        // Find the scene that was most recently loaded (the one at the last index of the loaded scenes)
        Scene newlyLoadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

        // Set the newly loaded scene as the active scene (This marks it as the one to be unloaded next)
        SceneManager.SetActiveScene(newlyLoadedScene);
    }


    // This will call start as a coroutine do do it over a certain number of frames while loading in the starting scene
    private IEnumerator Start()
    {
        // Set the initial alpha to start off with a black screen
        faderImage.color = new Color(0f, 0f, 0f, 1f);
        faderCanvasGroup.alpha = 1f;

        // Start the first scene loading (public field set up in editor) and wait for it to finish
        yield return StartCoroutine(LoadSceneAndSetActive(startingSceneName.ToString()));

        // If this event has any subscribers, call the event!
        EventHandler.CallAfterSceneLoadEvent();
        
        // Once the scene is finished loading, start fading in
        StartCoroutine(Fade(0f));
    }


    // This is the main external point of contact and influence from the rest of the project.
    // This will be called when the player wants to switch scenes
    public void FadeAndLoadScene(string sceneName, Vector3 spawnPosition)
    {
        // If a fade isn't already happening then start fading and switching scenes.
        if (!isFading)
        {
            // Start a coroutine to slowly fade in/out over the course of some frames
            StartCoroutine(FadeAndSwitchScenes(sceneName, spawnPosition));
        }
    }
}