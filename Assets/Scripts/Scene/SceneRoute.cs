using System.Collections.Generic;

// This class is just another data container for moving between scenes - containing the from/to scenes, and a list of scene paths for each of those scenes,
// which each just store that scene name, and the to/from grid cells
[System.Serializable]
public class SceneRoute
{
    public SceneName fromSceneName;
    public SceneName toSceneName;

    public List<ScenePath> scenePathList;
}
