using UnityEngine;

// This class is just a container to store details about player movement/action sounds. Lists of these for each sound type 
// will be populated in an SO, and then stored into a dictionary in the AudioManager cgameObject
[System.Serializable]
public class SoundItem
{
    // The name (enum) of the sound in question, the corrseponding audio clip to play, and a description of that sound
    public SoundName soundName;
    public AudioClip soundClip;
    public string soundDescription;

    // The min and max random variation pitches, to make the sounds less monotonous. Give each effect a little bit of uniqueness
    [Range(0.1f, 1.5f)]
    public float soundPitchRandomVariationMin = 0.8f;
    [Range(0.1f, 1.5f)]
    public float soundPitchRandomVariationMax = 1.2f;

    // This will allow us to equalize the sound clip volumes (they may have been recorded differently!)
    [Range(0f, 1f)]
    public float soundVolume = 1f;
}
