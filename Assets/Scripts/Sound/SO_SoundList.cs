using System.Collections.Generic;
using UnityEngine;

// This allows us to create an SO filled with all of our SoundItem's (for playing player movement/effect sounds). Each
// SoundItem stores basic details about the sound in question - including the name, the audio clip to play, a description
// of the sound, the volume, and the min/max random pitch variation values
[CreateAssetMenu(fileName = "so_SoundList", menuName = "Scriptable Objects/Sounds/Sound List")]
public class SO_SoundList : ScriptableObject
{
    [SerializeField] public List<SoundItem> soundDetails;
}
