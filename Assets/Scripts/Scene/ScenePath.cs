// This class is just a data container for the scene name, and the to/from grid cells within that scene name.
// These will be stored in a ScenePathList in SceneRoute
[System.Serializable]
public class ScenePath
{
    public SceneName sceneName;
    public GridCoordinate fromGridCell;
    public GridCoordinate toGridCell;
}
