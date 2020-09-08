using UnityEngine;

[System.Serializable]
public class GridCoordinate
{
    // x, y represent the grid coordinates on our tilemap
    public int x;
    public int y;


    public GridCoordinate(int p1, int p2)
    {
        x = p1;
        y = p2;
    }


    // These allow us to explicitly convert GridCoordinates into Vector2, Vector2Int, Vector3, or Vector3Int
    // Lets us automatically create Vector3/Vector2 etc. from a gridCorodinate, and Unity will explicitly do the conversion
    // GridCoordinates ARE system serializable (unlike Vectors)!!
    public static explicit operator Vector2(GridCoordinate gridCoordinate)
    {
        return new Vector2((float)gridCoordinate.x, (float)gridCoordinate.y);
    }


    public static explicit operator Vector2Int(GridCoordinate gridCoordinate)
    {
        return new Vector2Int(gridCoordinate.x, gridCoordinate.y);
    }


    public static explicit operator Vector3(GridCoordinate gridCoordinate)
    {
        return new Vector3((float)gridCoordinate.x, (float)gridCoordinate.y, 0f);
    }


    public static explicit operator Vector3Int(GridCoordinate gridCoordinate)
    {
        return new Vector3Int(gridCoordinate.x, gridCoordinate.y, 0);
    }
}
