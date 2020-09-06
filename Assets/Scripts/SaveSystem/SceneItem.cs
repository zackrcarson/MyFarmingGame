[System.Serializable]
public class SceneItem
{
    public int itemCode;
    public Vector3Serializable position;
    public string itemName;

    // We will have a SceneItem Instance for every item in our scene, which will allow us to save a serializable itemCode, position, and itemName
    public SceneItem()
    {
        position = new Vector3Serializable();
    }
}
