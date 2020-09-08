[System.Serializable]
public class GridPropertyDetails
{
    // This is a class that holds all of the following information for a given square on the tilemap
    // We will have a dictionary in SceneSave.cs, gridPropertyDetailsDictionary, which stores a GridPropertyDetails
    // for every grid square, keyed be a string of the coordinates
    public int gridX;
    public int gridY;
    public bool isDiggable = false;
    public bool canDropItem = false;
    public bool canPlaceFurniture = false;
    public bool isPath = false;
    public bool isNPCObstacle = false;
    public int daysSinceDug = -1;
    public int daysSinceWatered = -1;
    public int seedItemCode = -1;
    public int growthDays = -1;
    public int daysSinceLastHarvest = -1;

    public GridPropertyDetails()
    {

    }
}
