using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

// This script is used to test the AStar algorthm in AStar.cs. It will run the algorithm between a start/stop point, and then display the tiles used as the NPC's steps on a tilemap
[RequireComponent(typeof(AStar))] // Must have an AStar component on the GameObject so we can use it here
public class AStarTest : MonoBehaviour
{
    private AStar aStar;

    // starting and finish position for the algorithm to find a path between, populated in the editor
    [SerializeField] private Vector2Int startPosition;
    [SerializeField] private Vector2Int finishPosition;

    // Temporary tilemap to display the path on, and also the tile used to draw on that tilemap (semi-transparent blue tiles), populated in the editor
    [SerializeField] private Tilemap tileMapToDisplayPathOn = null;
    [SerializeField] private TileBase tileToUseToDisplayPath = null;

    // Flags populated in the editor for whether we want the game to show the path found, and the start/finish tiles
    [SerializeField] private bool displayStartAndFinish = false;
    [SerializeField] private bool displayPath = false;

    // This Stack will hold all of the NPC movement steps as determined in the AStar algorithm
    private Stack<NPCMovementStep> npcMovementSteps;


    // Populate the AStar object and initialize the NPCMovementStepStack
    private void Awake()
    {
        aStar = GetComponent<AStar>();

        npcMovementSteps = new Stack<NPCMovementStep>();
    }


    // This method will run the AStar algorithm to find a shortest path between the given start and finish grid positions.
    // It will then draw tiles on a temporary tileMap corresponding to the NPCMovementSteps populated by AStar in the
    // NPCMovementStepsStack so we can see the corresponding path.
    private void Update()
    {
        // Only run if we've supplied a start/finish position, tilemap to display, and tiles to make path. Else, do nothing!
        if (startPosition != null && finishPosition != null & tileMapToDisplayPathOn != null && tileToUseToDisplayPath != null)
        {
            // Display the start and finish tiles if we set the flag to do so
            if (displayStartAndFinish)
            {
                // Display the start tile on the temporary tileMap
                tileMapToDisplayPathOn.SetTile(new Vector3Int(startPosition.x, startPosition.y, 0), tileToUseToDisplayPath);

                // Display the finish tile on the temporary tileMap
                tileMapToDisplayPathOn.SetTile(new Vector3Int(finishPosition.x, finishPosition.y, 0), tileToUseToDisplayPath);
            }
            // If we didn't set the flag, clear out any tiles that might be displayed from when we did have the flag set
            else
            {
                // Clear the start tile on the temporary tileMap if we didn't set the flag
                tileMapToDisplayPathOn.SetTile(new Vector3Int(startPosition.x, startPosition.y, 0), null);

                // Clear the finish tile on the temporary tileMap if we didn't set the flag
                tileMapToDisplayPathOn.SetTile(new Vector3Int(finishPosition.x, finishPosition.y, 0), null);
            }

            // Display the path found if we set the flag to do so
            if (displayPath)
            {
                // Get the current active scene name
                Enum.TryParse<SceneName>(SceneManager.GetActiveScene().name, out SceneName sceneName);

                // Build the path with the AStar algorithm
                aStar.BuildPath(sceneName, startPosition, finishPosition, npcMovementSteps);

                // Display the path on the tilemap. Loop through all of the movementSteps stored in the npcMovementStapsStack, as determined with AStar, and enable a tile for each of them
                foreach (NPCMovementStep npcMovementStep in npcMovementSteps)
                {
                    tileMapToDisplayPathOn.SetTile(new Vector3Int(npcMovementStep.gridCoordinate.x, npcMovementStep.gridCoordinate.y, 0), tileToUseToDisplayPath);
                }
            }
            // If we didn't set the flag, clear out any tiles that might be displayed, and npcMovementSteps in the stack from when we did have the flag set
            else
            {
                // Clear the path on the temporary tileMap if we didn't set the flag
                if (npcMovementSteps.Count > 0)
                {
                    // Clear the path on the tilemap
                    foreach (NPCMovementStep npcMovementStep in npcMovementSteps)
                    {
                        tileMapToDisplayPathOn.SetTile(new Vector3Int(npcMovementStep.gridCoordinate.x, npcMovementStep.gridCoordinate.y, 0), null);
                    }

                    // Clear out the NPC movement steps
                    npcMovementSteps.Clear();
                }
            }
        }
    }

}
