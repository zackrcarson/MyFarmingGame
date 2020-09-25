// This class is just a container to store details about the scene sounds (ambient, music). Lists of these for each sound type 
// will be populated in the sound effects SO, and then stored into a dictionary in the AudioManager gameObject
[System.Serializable]
public class SceneSoundItem
{
    // The scene name these play in, the sound name (enum) of the ambient sound for that scene, and the sound name for the music for that scene
    public SceneName sceneName;
    public SoundName ambientSoundForScene;
    public SoundName musicForScene;
}
