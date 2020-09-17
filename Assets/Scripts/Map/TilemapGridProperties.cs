using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

// This class will always be running, both in the editor and in game. Each method only runs if it is in the editor
[ExecuteAlways]
public class TilemapGridProperties : MonoBehaviour
{
#if UNITY_EDITOR

    // This tileMap that this script is attached to
    private Tilemap tileMap;

    // Populate the SO asset that has the bool properties saved
    [SerializeField] private SO_GridProperties gridProperties = null;
    // The property in question -> enum, populated in editor
    [SerializeField] private GridBoolProperty gridBoolProperty = GridBoolProperty.diggable;
    

    // When the user ENABLES the GridProperties GameObject in the editor, This will go through, and clear all of the bools from the list, so that the user can re-paint.
    // Once that's done, the OnDisable method will fill the new ones back in.
    private void OnEnable()
    {
        // Only populate in the editor! Not while game is playing
        if (!Application.IsPlaying(gameObject))
        {
            tileMap = GetComponent<Tilemap>();

            // Clear the gridPropertiesList
            if (gridProperties != null)
            {
                gridProperties.gridPropertyList.Clear();
            }
        }
    }


    // When the user DISABLES the GridProperties GameObject in the editor, This will go through, and collect all of the painted tiles, and writing them to the SO
    private void OnDisable()
    {
        // Only populate in the editor! Not while game is playing
        if (!Application.IsPlaying(gameObject))
        {
            // Write all of the painted tiles to bools in the SO!
            UpdateGridProperties();
            
            // This is required to ensure that the updated gridProperties gameObject gets saved when the game is saved. Otherwise,
            // They are not saved
            if (gridProperties != null)
            {
                EditorUtility.SetDirty(gridProperties);
            }
        }
    }


    // Take thie tilemap, loop through all of the cells and add an entry (a gridProperty with coordinate, property, bool) to the gridPropertiesList.
    private void UpdateGridProperties()
    {
        // Compress the tilemap bounds (compresses the bounds to JUSt the tiles painted on the tilemap)
        tileMap.CompressBounds();

        // Only populate in the editor! Not running at run time! Not while game is playing
        if (!Application.IsPlaying(gameObject))
        {
            if (gridProperties != null)
            {
                // Bottom left to top right
                Vector3Int startCell = tileMap.cellBounds.min;
                Vector3Int endCell = tileMap.cellBounds.max;

                // Loop through all of the cells in the tileMap grid. Each time, take the tile, and add an entry to the
                // gridPropertyList, as a new gridProperty with the coordinate, the property, and a bool (true if tile != null,
                // and false (already default) if tile == null)
                for (int x = startCell.x; x < endCell.x; x++)
                {
                    for (int y = startCell.y; y < endCell.y; y++)
                    {   
                        // Grab the tile at this location
                        TileBase tile = tileMap.GetTile(new Vector3Int(x, y, 0));
                        // If null, it is not painted, and we keep the value at default (false). If not, it is painted and we make it true.
                        if (tile != null)
                        {
                            gridProperties.gridPropertyList.Add(new GridProperty(new GridCoordinate(x, y), gridBoolProperty, true));
                        }
                    }
                }
            }
        }
    }


    private void Update()
    {
        // Only populate in the editor
        if (!Application.IsPlaying(gameObject))
        {
            Debug.Log("DISABLE PROPERTY TILEMAPS");
        }
    }
#endif
}
