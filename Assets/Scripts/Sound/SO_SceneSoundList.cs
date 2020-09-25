using System.Collections.Generic;
using UnityEngine;

// This allows us to create an SO filled with all of our SceneSoundItem's (for the scene name, scene ambient, and scene music). Each
// SceneSoundItem stores basic details about the scene sound in question - including the scene name to play it in, the ambient sound name,
// and the music sound name
[CreateAssetMenu(fileName = "so_SceneSoundList", menuName = "Scriptable Objects/Sounds/Scene Sounds List")]
public class SO_SceneSoundList : ScriptableObject
{
    [SerializeField] public List<SceneSoundItem> sceneSoundDetails;
}
