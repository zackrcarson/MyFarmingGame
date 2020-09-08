[System.Serializable]
public class GridProperty
{
    // This class stores the gridCoordinate (x,y), the GridBoolProperty enum (i.e. isDiggable, etc..), and the bool value for each square found in
    // the SO. It will cycle through all itemsd in the SO, and add a GridProperty to the dict
    public GridCoordinate gridCoordinate;
    public GridBoolProperty gridBoolProperty;
    public bool gridBoolValue = false;

    public GridProperty(GridCoordinate gridCoordinate, GridBoolProperty gridBoolProperty, bool gridBoolValue)
    {
        this.gridCoordinate = gridCoordinate;
        this.gridBoolProperty = gridBoolProperty;
        this.gridBoolValue = gridBoolValue;
    }
}
