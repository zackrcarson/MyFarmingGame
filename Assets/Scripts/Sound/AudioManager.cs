using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

// This singleton class Manages all of our sounds in the game. It creates a sounds Dictionary of all the sounds we can play in the game from the SO containing a SoundItemList,
// and can Play a given sound (via Sound.cs on a Sound gameObject prefab from the PoolManager) given the SoundName enum of the sound we want to play
public class AudioManager : SingletonMonobehaviour<AudioManager>
{
    // The soundPrefab in our GameObject Pool used to play a sound, as set up in Sound.cs, given directions from here,
    // Given an entry in our soundDictionary
    [SerializeField] private GameObject soundPrefab = null;

    // Populated in the editor with the ambient sound and game music audio sources to be playing the music in the game
    [Header("Audio Sources")]
    [SerializeField] private AudioSource ambientSoundAudioSource = null;
    [SerializeField] private AudioSource gameMusicAudioSource = null;

    // Populated in the editor with the Audio mixer containing all of the audio mixer groups (i.e. master, ambientMaster, ambient, musicMaster, music, each with sliders)
    [Header("Audio Mixers")]
    [SerializeField] private AudioMixer gameAudioMixer = null;

    // Populated in the editor with the music and ambient snapshots created with pre-saved audio mixer group settings
    [Header("Audio Snapshots")]
    [SerializeField] private AudioMixerSnapshot gameMusicSnapshot = null;
    [SerializeField] private AudioMixerSnapshot gameAmbientSnapshot = null;

    [Header("Other")]
    // SO containing the list of all sound effects in the game to be played, each item is a SoundItem (storing sound name, clip, description, volume, and min/max pitch variation)
    // also, the SO containing the list of all scene sounds in the game to be played, each item is a SceneSoundItem (storing scene name to be played, ambient, and music in that scene)
    [SerializeField] private SO_SoundList so_SoundList = null;
    [SerializeField] private SO_SceneSoundList so_SceneSoundList = null;

    // Populated in the editor for the default length of time to play the music clip for before transitioning back to ambient sounds, the min/max seconds to 
    // randomly choose from for when to first play the sceneMusic (transitioned from ambient) after entering the scene, and finally the number of seconds to transition between ambient/music
    [SerializeField] private float defaultSceneMusicPlayTimeSeconds = 120f;
    [SerializeField] private float sceneMusicStartMinSecs = 20f;
    [SerializeField] private float sceneMusicStartMaxSecs = 40f;
    [SerializeField] private float musicTransitionSecs = 8f;

    // The dictionaries of SoundItems and SceneSoundItemscreated from the SO_SoundList and SO_SceneSoundList, so we can easily access the SoundItem's and SceneSoundItems given a soundName enum
    private Dictionary<SoundName, SoundItem> soundDictionary;
    private Dictionary<SceneName, SceneSoundItem> sceneSoundDictionary;

    // The coroutine we will use to play the sceneSounds - this way we can easily cancel it when we transition between scenes
    private Coroutine playSceneSoundsCoroutine;

    // Populate the soundDictionary with SoundItems keyed by their SoundName enum
    protected override void Awake()
    {
        base.Awake();

        // Initialize the sound dictionary and sceneSound dictionary to store all of our SoundItems for each sound effect, and each of our SceneSoundItems for the music/ambient playing in the scene
        soundDictionary = new Dictionary<SoundName, SoundItem>();
        sceneSoundDictionary = new Dictionary<SceneName, SceneSoundItem>();

        // Populate the soundDictionary with SoundItems from our SO_SoundList, each keyed by their corresponding SoundName enum so we can easily access them
        foreach (SoundItem soundItem in so_SoundList.soundDetails)
        {
            soundDictionary.Add(soundItem.soundName, soundItem);
        }

        // Populate the sceneSoundDictionary with SceneSoundItems from our SO_SceneSoundList, each keyed by their corresponding sceneName so we can easily access them for a givin scene
        foreach (SceneSoundItem sceneSoundItem in so_SceneSoundList.sceneSoundDetails)
        {
            sceneSoundDictionary.Add(sceneSoundItem.sceneName, sceneSoundItem);
        }
    }


    // Subscribe the PlaySceneSounds method to the AfterSceneLoadEvent, so whenever a new scene is loaded we can find the scene, and play the proper ambient/music found in the sceneSoundDictionary
    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += PlaySceneSounds;
    }


    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= PlaySceneSounds;
    }


    // This method is subscribed to the AfterSceneLoadEvent, and once it's triggered after a new scene is loaded, it takes care of playing scene sounds, 
    // including ambient sounds and background music. It finds the SceneSoundItem corresponding to the new scene, and starts a coroutine to 
    // first play ambient music, then transition to music, and then back to ambient, each of those sounds are SoundItems in the soundDictionary
    private void PlaySceneSounds()
    {
        // Once we find the SceneSoundsItem for the current scene, we will populate new SoundItem's (not SceneSoundItems!) with values from the soundDictionary (that dictionary contains the actual 
        // music/ambient clips themselves
        SoundItem musicSoundItem = null;
        SoundItem ambientSoundItem = null;

        // The amount of time to play a song for before transitioning back to ambient sounds
        float musicPlayTime = defaultSceneMusicPlayTimeSeconds;

        // Try to get the current scene Enum so we can find the ambient sounds and music for that scene in the sceneSoundDictionary
        if (Enum.TryParse<SceneName>(SceneManager.GetActiveScene().name, true, out SceneName currentSceneName))
        {
            // Check if that scene name is in the sceneSoundDictionary keyed by scene name, and then take the ambient and music sounds for that scene as a SoundItem, (not a sceneSoundItem! The soundItem will
            // play the actual clip itself from the soundDictionary
            if (sceneSoundDictionary.TryGetValue(currentSceneName, out SceneSoundItem sceneSoundItem))
            {
                // Look in the soundDictionary for the corresponding music and ambient sounds for the found SceneSoundsItem, and populate the SoundItems with them, so we can play them down below
                soundDictionary.TryGetValue(sceneSoundItem.musicForScene, out musicSoundItem);
                soundDictionary.TryGetValue(sceneSoundItem.ambientSoundForScene, out ambientSoundItem);
            }
            // If we can't find any matching sounds, just return and don't play it
            else
            {
                return;
            }

            // Stop any scene sounds already playing from the last scene, so we can start the new ones
            if (playSceneSoundsCoroutine != null)
            {
                StopCoroutine(playSceneSoundsCoroutine);
            }

            // Play the scene ambient Sounds and music using a coroutine that will control ambient->music->ambient over all of the frames the player is in that scene for.
            // It's cached in the playSceneSoundsCoroutine so we can easily stop it when wanted (i.e. leaving the scene as in the line above
            playSceneSoundsCoroutine = StartCoroutine(PlaySceneSoundsCoroutine(musicPlayTime, musicSoundItem, ambientSoundItem));
        }
    }


    // This is the coroutine that takes care of playing the ambient sounds in the musicSoundItem, for a random amount of time
    // before playing the music in musicSoundItem for a set period of time in musicPlayTime, and then back to ambient sounds
    private IEnumerator PlaySceneSoundsCoroutine(float musicPlayTime, SoundItem musicSoundItem, SoundItem ambientSoundItem)
    {
        // First make sure we actually have ambient sounds and music to play
        if (musicSoundItem != null && ambientSoundItem != null)
        {
            // Start playing the Ambient Sound clip found in the ambient SoundItem, play it immediately (no transition)
            PlayAmbientSoundClip(ambientSoundItem, 0f);

            // Wait for a random range of seconds (as a yield return in the coroutine) before we start playing the music
            yield return new WaitForSeconds(UnityEngine.Random.Range(sceneMusicStartMinSecs, sceneMusicStartMaxSecs));

            // Play the music sound clip found in the music SoundItem, with a transition period of musicTransitionSecs
            PlayMusicSoundClip(musicSoundItem, musicTransitionSecs);

            // Wait for the music to finish playing for the specified number of seconds before continuing on in the coroutine
            yield return new WaitForSeconds(musicPlayTime);

            // Start playing the Ambient Sound clip again, with a transition period of musicTransitionSecs. (play this one indefinitely now)
            PlayAmbientSoundClip(ambientSoundItem, musicTransitionSecs);
        }
    }


    // This method is called from the PlaySceneSoundsCoroutine, to transition into playing the ambient SoundItem in a transitionTimeSeconds number of seconds
    private void PlayAmbientSoundClip(SoundItem ambientSoundItem, float transitionTimeSeconds)
    {
        // Set the volume (we need to convert it from the sound volume set in the SoundItem (from the SO_SoundItemList) as a decimal, into decibels)
        // for the AmbientVolume in our gameAudio mixer (AmbientVolume is one of the exposed parameters that we can control, setting the volume of the ambient group)
        gameAudioMixer.SetFloat("AmbientVolume", ConvertSoundVolumeDecimalFractionToDecibels(ambientSoundItem.soundVolume));

        // Set the audio clip to the ambient sound source, and then play it from that source
        ambientSoundAudioSource.clip = ambientSoundItem.soundClip;
        ambientSoundAudioSource.Play();

        // Transition to the ambient snapshot of audio mixers, having high ambient volume and low music volume, over the given number of seconds in transitionTimeSeconds
        gameAmbientSnapshot.TransitionTo(transitionTimeSeconds);
    }


    // This method is called from the PlaySceneSoundsCoroutine, to transition into playing the music SoundItem in a transitionTimeSeconds number of seconds
    private void PlayMusicSoundClip(SoundItem musicSoundItem, float transitionTimeSeconds)
    {
        // Set the volume (we need to convert it from the sound volume set in the SoundItem (from the SO_SoundItemList) as a decimal, into decibels)
        // for the MusicVolume in our gameAudio mixer (MusicVolume is one of the exposed parameters that we can control, setting the volume of the music group)
        gameAudioMixer.SetFloat("MusicVolume", ConvertSoundVolumeDecimalFractionToDecibels(musicSoundItem.soundVolume));

        // Set the audio clip to the ambient sound source, and then play it from that source
        gameMusicAudioSource.clip = musicSoundItem.soundClip;
        gameMusicAudioSource.Play();

        // Transition to the music snapshot of audio mixers, having high music volume and low ambient volume, over the given number of seconds in transitionTimeSeconds
        gameMusicSnapshot.TransitionTo(transitionTimeSeconds);
    }


    /// <summary>
    /// Converts a given volume as a decimal fraction, into a decibel level that can be used by Unitys audio mixers
    /// </summary>
    /// <param name="volumeDecimalFraction"></param>
    /// <returns> Returns the decibel level for the given decimal </returns>
    private float ConvertSoundVolumeDecimalFractionToDecibels(float volumeDecimalFraction)
    {
        // Convert the volume from a decimal fraction, into the (-80, 20) decibel range
        return (volumeDecimalFraction * 100f - 80f);
    }


    // This public method allows us to play a given sound (i.e. trigger this from elsewhere in the game!), given it's enum soundName. It will sound the SoundItem from the soundName from the soundDictionary,
    // set it up in Sound.cs, and play it in Sound.cs (by activating the prefab taken from the Pool)
    public void PlaySound(SoundName soundName)
    {
        // First get the soundItem coresponding to it's soundName in the soundDictionary (if it exists), and make sure we've populated the soundPrefab gameObject so we can obtain it from the pool
        if (soundDictionary.TryGetValue(soundName, out SoundItem soundItem) && soundPrefab != null)
        {
            // Obtain 1 instance of the soundPrefab from the PoolManager, setting it at the origin (These are all 2D sounds, so the origin doesn't matter - same sound everywhere)
            GameObject soundGameObject = PoolManager.Instance.ReuseObject(soundPrefab, Vector3.zero, Quaternion.identity);

            // Find the Sound component on the soundGameObject from the Pool
            Sound sound = soundGameObject.GetComponent<Sound>();

            // This Sound method will set up the pitch, volume, and clip of the sound corresponding to the soundItem we're interested in
            sound.SetSound(soundItem);

            // Setting the Sound GameObject to active will begin playing the sound via the OnEnable method!
            soundGameObject.SetActive(true);

            // Start a coroutine to the disable the sound playing after the length of its sound clip, so it doesn't start over
            StartCoroutine(DisableSound(soundGameObject, soundItem.soundClip.length));
        }
    }


    // This coroutine disables a Sound GameObject from playing after a given length of time
    private IEnumerator DisableSound(GameObject soundGameObject, float soundDuration)
    {
        // Yield return this method until the duration of the sound clip has passed. Once that happens, set the sound GameObject to inactive so it'll stop playing
        yield return new WaitForSeconds(soundDuration);
        soundGameObject.SetActive(false);
    }
}
