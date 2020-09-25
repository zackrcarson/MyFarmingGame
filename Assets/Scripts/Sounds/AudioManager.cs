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

    [Header("Other")]
    // SO containing the list of all sounds in the game to be played, each item is a SoundItem (storing sound name, clip, description, volume, and min/max pitch variation)
    [SerializeField] private SO_SoundList so_SoundList = null;

    // The dictionary of SoundItems created from the SO_SoundList, so we can easily access a SoundItem given a soundName enum
    private Dictionary<SoundName, SoundItem> soundDictionary;


    // Populate the soundDictionary with SoundItems keyed by their SoundName enum
    protected override void Awake()
    {
        base.Awake();

        // Initialize the sound dictionary to store all of our SoundItems for each sound effect
        soundDictionary = new Dictionary<SoundName, SoundItem>();

        // Populate the soundDictionary with SoundItems from our SO_SoundList, each keyed by their corresponding SoundName enum so we can easily access them
        foreach (SoundItem soundItem in so_SoundList.soundDetails)
        {
            soundDictionary.Add(soundItem.soundName, soundItem);
        }
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
