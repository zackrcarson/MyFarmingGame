using UnityEngine;

// This Sounds will be attached to GameObjects in the ObjectPool, each containing an AudioSource to play the sound effects that the player triggers.
// This class will set up the pitch/sound clip, and volume to be played, and then play it once the GameObject this is on is enabled, to be stopped when
// it's disabled. The setup and enable/disabled is controlled from the AudioManager, which has a dictionary of the SoundItems, populated from the SO_SoundItemsList
[RequireComponent(typeof(AudioSource))]
public class Sound : MonoBehaviour
{
    private AudioSource audioSource;


    // Populate the audio source component on the GameObject, which is the thing that emits the sound
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }


    // When this gameObject is enabled by the AudioManager after setting up the details in SetSound (set the pitch, volume, and clip form the SO_SoundItems, populated into a 
    // dictionary in the AudioManager), it will play the clip as long as it exists 
    private void OnEnable()
    {
        // If the clip has bene populated, play it
        if (audioSource.clip != null)
        {
            audioSource.Play();
        }
    }


    // When this gameObject is disabled, stop playing the setup sound
    private void OnDisable()
    {
        audioSource.Stop();
    }


    // Called from the Audio manager, given a SoundItem from the SoundItem Dictionary (from the SO_SoundItemsList), this will set up the 
    // pitch, volume, and sound clip to be played once the gameObject is enabled
    public void SetSound(SoundItem soundItem)
    {
        // Set the pitch, volume, and audio clip to be played once the GameObject this is on is enabled.
        // We are randomly choosing a pitch between the min/max set up in the SO, so it's not monotonous when
        // we play it over and over again
        audioSource.pitch = Random.Range(soundItem.soundPitchRandomVariationMin, soundItem.soundPitchRandomVariationMax);
        audioSource.volume = soundItem.soundVolume;
        audioSource.clip = soundItem.soundClip;
    }
}
