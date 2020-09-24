using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

// AStar algorithm used to find the shortest path between a start node and a targetNode, given obstacles and penalties
public class AStar : MonoBehaviour
{
    [Header("Tiles & Tilemap References")]
    [Header("Options")]
    // Bool populated in the inspector switching on and off whether we will observe penalties (like paths, etc)
    [SerializeField] private bool observeMovementPenalties = true;

    // These are populated in the inspector, determining the penalty for a path, and a non-path (i.e. maybe we'll keep path as a 0 penalty, and give non-paths a larger penalty
    // So we'll encourage the algorithm to stay on the paths
    [Range(0, 20)]
    [SerializeField] private int pathMovementPenalty = 0;
    [Range(0, 20)]
    [SerializeField] private int defaultMovementPenalty = 0;

    // 2D array of Nodes stored in the gridNodes class, as well as the starting Node and ending Node we want to find a path in-between
    private GridNodes gridNodes;
    private Node startNode;
    private Node targetNode;

    // Grid dimensions and origin for the scene we want to find paths through (will be populated with GridPropertiesManager.GetGridDimensions())
    private int gridWidth;
    private int gridHeight;
    private int originX;
    private int originY;

    // The open and closed lists of nodes to be used in the algorithm, storing nodes to be checked, and nodes already checked. 
    // The closed list is a hash list, which uses a hash to lotate contents more quickly than a normal list, because the nodes in the list are
    // keyed by a mathematical hash key.
    // We keep the open list as a list because we need to sort it, which can't be done with a HashSet
    private List<Node> openNodeList;
    private HashSet<Node> closedNodeList;

    // Bool for if we've found a path or not. Set to true once we've found a path!!
    private bool pathFound = false;


    /// <summary>
    /// Builds a path for the given sceneName, from the startGridPosition to the endGridPosition, and adds movement steps to the passed in npcMovementStack (a stack of movement steps for the NPC to follow).
    /// Also returns true if a path is found, or false if no path found
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="startGridPosition"></param>
    /// <param name="endGridPosition"></param>
    /// <param name="npcMovementStepStack"></param>
    /// <returns> Returns a bool true if a path is found, and false if not. The NPC movement steps are also passed into NPCMovementStepStack </returns>
    public bool BuildPath(SceneName sceneName, Vector2Int startGridPosition, Vector2Int endGridPosition, Stack<NPCMovementStep> npcMovementStepStack)
    {
        // Make sure pathFound always starts off as false, i.e. for when we call BuildPath a second time after it already found a path earlier!
        pathFound = false;

        // This method will populate all of the GridNodes array nodes with the isPath and isNPCObstacle properties from the GridPropertiesDictionary for this scene, and also set up
        // the starting/ending grid nodes, and the open/closed nodes lists. It will return true if this was successful.
        if (PopulateGridNodesFromGridPropertiesDictionary(sceneName, startGridPosition, endGridPosition))
        {
            // If we successfully populated the GridNodes properties, Now we run the FindShortestPath algorithm which runs the AStar algorithm (add the startNode to the OpenNodeList,
            // loop through the openList until it's empty, sort the openList by fCost tie-broken by hCost, set the best-case to the currentNode (remove it from open list and add to closedList)
            // Then evaluate the new h/g/f costs of the 8 neighbors to the currentNode. Repeat until currentNode = targetNode, or openNodeList is empty.
            if (FindShortestPath())
            {
                // If we've found a shortest path from startNode -> targetNode, we know that currentNode = targetNode. And currentNode's list of parent nodes
                // Will lead us back to the startNode. This method loops through all of the currentNodes parents back to the startNode, and adds each step
                // to the npcMovementStepStack, which NPCs will be able to follow
                UpdatePathOnNPCMovementStepStack(sceneName, npcMovementStepStack);

                // If we found a path, return true. Else, return false and no npcMovementStepStack gets updated
                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// This method loops through all of the parents to the currentNode which is now the targetNode because we found a path. Following the parents
    /// backwards will eventually get to the start node. for each node we traverse, create a new npcMovementStep populated with the sceneName and 
    /// Coordinate of the current node, and add it to the npcMovementStepStack that NPCs will follow
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="npcMovementStepStack"></param>
    private void UpdatePathOnNPCMovementStepStack(SceneName sceneName, Stack<NPCMovementStep> npcMovementStepStack)
    {
        // If we found a path, the currentNode = targetNode, so we just need to follow that path of parentNodes back to the startNode (every node has a list of parentNodes working back to the start Node)
        // Start out at the targetNode (= currentNode) and work backwards towards the startNode
        Node nextNode = targetNode;

        // Loop through the currentNode=targetNode's parent nodes all the way back to the startNode when it's parent = null
        while (nextNode != null)
        {
            // For each Node we traverse backwards, create a new movement step (which stores the sceneName, the goordinates of that step, and the time of that step)
            NPCMovementStep npcMovementStep = new NPCMovementStep();

            // Populate the sceneName and gridCoordinate (offset by the origon to convert grid location to tileMap location) for the current step in the loop
            npcMovementStep.sceneName = sceneName;
            npcMovementStep.gridCoordinate = new Vector2Int(nextNode.gridPosition.x + originX, nextNode.gridPosition.y + originY);

            // add the newly updated step to the NPC movement step stack (pushed onto the stack - so the FIRST thing we add to the stack, is the LAST thing we take off of the stack
            npcMovementStepStack.Push(npcMovementStep);

            // Set the nextNode to the currentNodes parent, and repeat, all the way to the startNode when the nextNode will = null!
            nextNode = nextNode.parentNode;
        }
    }


    /// <summary>
    /// This method is the main algorithm that determines the shortest path between a start and target node, using an AStar algorithm.
    /// </summary>
    /// <returns>Returns true if a path has been found.</returns>
    private bool FindShortestPath()
    {
        // Add start node to the open list to start the algorithm
        openNodeList.Add(startNode);

        // Loop through the open node list until it's empty - once that's the case we're done, even if we didn't find a path
        while (openNodeList.Count > 0)
        {
            // Sort the list by the gCost (tie break by hCost). The sort method uses the IComparable interface, which uses the CompareTo method 
            // in every Node in this list to sort them (by gCost, tie-broken by hCost)
            openNodeList.Sort();

            // Set the Current node to the node in the open list with the lowest fCost (i.e. the first entry of the sorted list!)
            Node currentNode = openNodeList[0];

            // Remove the current node from the open list
            openNodeList.RemoveAt(0);

            // Add the current node to the closed list
            closedNodeList.Add(currentNode);

            // If the current node is equal to the target node, finish! We're done - we found a path. Set the bool to true and break out of the loop
            if (currentNode == targetNode)
            {
                pathFound = true;
                break;
            }

            // Evaluate the fcost for each neighbor of the current node, and then restart the loop!
            EvaluateCurrentNodeNeighbors(currentNode);
        }

        if (pathFound)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    /// <summary>
    /// Thie method loops through all 8 neighboring nodes to the currentNode, checks if they are valid neighbor nodes, calculates it's new
    /// gCost and hCost, and adds it to the open list, barring exceptions as found in the algorithm.
    /// </summary>
    /// <param name="currentNode"></param>
    private void EvaluateCurrentNodeNeighbors(Node currentNode)
    {
        // Define the grid position for the current node
        Vector2Int currentNodeGridPosition = currentNode.gridPosition;

        // This will hold the valid neighbor node that we find in each of the 8 directions
        Node validNeighborNode;

        // Loop through all 8 of the directions from the current node (i.e. gridPosition.x +/- 1 and gridPosition.y +/- 1)
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                // Don't need to check the current Node - only neighbor nodes
                if (i == 0 && j == 0)
                {
                    continue;
                }

                // Populate the validNeighborNode with the neighbor node found at the + (i,j) position from the currentNode. This method returns null if the neighbor node
                // at that grid position is invalid (out of the grid, is an obstacle, or is already in the closed node list)
                validNeighborNode = GetValidNeighborNode(currentNodeGridPosition.x + i, currentNodeGridPosition.y + j);

                // If the current grid position has a valid node, continue the algorithm. If it's null, we will skip it and move directly to the next neighbor in the loop
                if (validNeighborNode != null)
                {
                    // Calculate the new gCost for the current valid neighbor node. The gCost is simply the gCost of the currentNode (already calculated) plus the distance
                    // between the current node and the valid neighbor node (this addition is either 10 or 14 if it's horizontal/vertical or diagonal to the currentNode)
                    int newCostToNeighbor;
                    if (observeMovementPenalties)
                    {
                        // If we are observing the movement penalties (i.e. paths), additionally add the movement penalty to the gCost (This has already been populated based on if it's a path or not)
                        newCostToNeighbor = currentNode.gCost + GetDistance(currentNode, validNeighborNode) + validNeighborNode.movementPenalty;
                    }
                    else
                    {
                        newCostToNeighbor = currentNode.gCost + GetDistance(currentNode, validNeighborNode);
                    }
                    
                    // Check if the current valid neighbor node is already in the openNodeList
                    bool isValidNeighborNodeInOpenList = openNodeList.Contains(validNeighborNode);

                    // If the current valid neighbor Node is already in the openNodeList, or if the new updated neighbor cost is > old neighbor cost, break out to the 
                    // next neighbor node in the loop. Otherwise, update the gCost, hCode, parentNode, and add the current valid neighbor node to the openNodeList
                    if (newCostToNeighbor < validNeighborNode.gCost || !isValidNeighborNodeInOpenList)
                    {
                        // Update the gCost and hCost (distance from neighbor node to target node) in the current valid neighbor node
                        validNeighborNode.gCost = newCostToNeighbor;
                        validNeighborNode.hCost = GetDistance(validNeighborNode, targetNode);

                        // Set the current valid neighbor node's parent to the current Node
                        validNeighborNode.parentNode = currentNode;

                        // If the current valid neighbor node isn't already in the openList, add it
                        if (!isValidNeighborNodeInOpenList)
                        {
                            openNodeList.Add(validNeighborNode);
                        }
                    }
                }
            }
        }
    }


    /// <summary>
    /// This method computes the distance between two nodes.
    /// </summary>
    /// <param name="nodeA"></param>
    /// <param name="nodeB"></param>
    /// <returns> Returns the distance between nodeA and nodeB - multiples of 10 (for horizontal/vertical steps), and 14 (for diagonal steps). </returns>
    private int GetDistance(Node nodeA, Node nodeB)
    {
        // X distance and Y distance between the two nodes
        int dstX = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
        int dstY = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

        // if dstX > dstY, we are looking at nodes to the right. Diagonals will get multiples of 14, and horizontals/verticals will get multiples of 10.
        if (dstX > dstY)
        {
            return 14 * dstY + 10 * (dstX - dstY);
        }

        // Same for the other 5 grid squares
        return 14 * dstX + 10 * (dstY - dstX);
    }


    /// <summary>
    /// This method returns the neighborNode from currentNode at the given neighbor node x/y position. If that node location is invalid (out of the grid, is an obstacle,
    /// or is already in the closed node list), this returns null so we won't have to check it
    /// </summary>
    /// <param name="neighborNodeXPosition"></param>
    /// <param name="neighborNodeYPosition"></param>
    /// <returns> Returns the neighbor Node at the specified position if it's valid, else returns null </returns>
    private Node GetValidNeighborNode(int neighborNodeXPosition, int neighborNodeYPosition)
    {
        // If the neighbor node's position is beyond the grid, then return null - they aren't valid!
        if (neighborNodeXPosition >= gridWidth || neighborNodeXPosition < 0 || neighborNodeYPosition >= gridHeight || neighborNodeYPosition < 0)
        {
            return null;
        }

        // Get the neighbor node from the GridNodes Node array at the neighbor grid position
        Node neighborNode = gridNodes.GetGridNode(neighborNodeXPosition, neighborNodeYPosition);

        // If the neighbor node is an obstacle, or if the neighbor node is in the closed list already, then skip it! It's not a valid neighbor node
        if (neighborNode.isObstacle || closedNodeList.Contains(neighborNode))
        {
            return null;
        }
        // Finally, if it's valid return the valid neighbor node!
        else
        {
            return neighborNode;
        }
    }


    /// <summary>
    /// This method will populate all of the nodes in a tileMap for a given scene, and store within them members from the GridPropertyDetailsDictionary
    /// including the bool isNPCObstacle, and isPath properties.
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="startGridPosition"></param>
    /// <param name="endGridPosition"></param>
    /// <returns> populates all of the nodes in the current scene tilemap GridNodes with properties from the GridPropertiesDictionary isPath and isNPCObstacle, and Returns a bool
    /// stating if it was successful or not. </returns>
    private bool PopulateGridNodesFromGridPropertiesDictionary(SceneName sceneName, Vector2Int startGridPosition, Vector2Int endGridPosition)
    {
        // Get the grid properties dictionary for the given scene in the SceneSave, where the GridPropertiesDictionary is stored (and saved/loaded)
        SceneSave sceneSave;

        // Get the SceneData (dictionary of all saved items in that scene) for this scene, if it exists
        if (GridPropertiesManager.Instance.GameObjectSave.sceneData.TryGetValue(sceneName.ToString(), out sceneSave))
        {
            // Get the dict of grid property details stored in the SceneSave, if it exists
            if (sceneSave.gridPropertyDetailsDictionary != null)
            {
                // Get the grid height, width, and origin of the current scene if the sceneName was found in the dictionary
                if (GridPropertiesManager.Instance.GetGridDimensions(sceneName, out Vector2Int gridDimensions, out Vector2Int gridOrigin))
                {
                    // Create a GridNodes (list of all nodes in a grid for the current scene) based on the grid properties dictionary just obtained,
                    // And populate the grid dimensions and origin for AStar
                    gridNodes = new GridNodes(gridDimensions.x, gridDimensions.y);
                    gridWidth = gridDimensions.x;
                    gridHeight = gridDimensions.y;
                    originX = gridOrigin.x;
                    originY = gridOrigin.y;

                    // Create the openNodeList to be used to store nodes to be checked
                    openNodeList = new List<Node>();

                    // create the closedNodeList to be used to store already checked nodes
                    closedNodeList = new HashSet<Node>();
                }
                // Just return false if we didn't find a dictionary for this scene
                else
                {
                    return false;
                }

                // Populate the start node, from gridNodes.GetGridNode which returns the node in the GridNode array at that position. Subtract the origins because the origin is different in every scene
                // Tile maps have an origin in the CENTER of the map, we subtract the origin so we start at the bottom left as usual
                startNode = gridNodes.GetGridNode(startGridPosition.x - gridOrigin.x, startGridPosition.y - gridOrigin.y);

                // Populate the target node in the same way
                targetNode = gridNodes.GetGridNode(endGridPosition.x - gridOrigin.x, endGridPosition.y - gridOrigin.y);

                // Loop through every square in the grid, and populate the obstacle and path info for the each grid square
                for (int x = 0; x < gridDimensions.x; x++)
                {
                    for (int y = 0; y < gridDimensions.y; y++)
                    {
                        // Get the gridPropertyDetails for the current square we are looking at (this will have members like isDiggable, isPath, isNPCObstacle, etc), adjusted by the origin as well
                        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(x + gridOrigin.x, y + gridOrigin.y, sceneSave.gridPropertyDetailsDictionary);
                        
                        // As long as we found the details for that square, populate the obstacle bool and penalty. If not, just quit out and return false from this method
                        if (gridPropertyDetails != null)
                        {
                            // If the current square is an obstacle, create a node at that grid location (obtained from the gridNodes array GetGridNode at this position), 
                            // and say so in the Node isObstacle member
                            if (gridPropertyDetails.isNPCObstacle == true)
                            {
                                Node node = gridNodes.GetGridNode(x, y);
                                node.isObstacle = true;
                            }
                            if (gridPropertyDetails.seedItemCode == 10000 || gridPropertyDetails.seedItemCode == 10009 || 
                                gridPropertyDetails.seedItemCode == 10010|| gridPropertyDetails.seedItemCode == 10011 || 
                                gridPropertyDetails.seedItemCode == 10014 || gridPropertyDetails.seedItemCode == 10016 || 
                                gridPropertyDetails.seedItemCode == 10022 || gridPropertyDetails.seedItemCode == 10023) // I added this if statement so that things like trees, rocks, etc are considered obstacles for the path finding algorithm to navigate around
                            {
                                Node node = gridNodes.GetGridNode(x, y);
                                node.isObstacle = true;
                            }
                            // Else, if the current square is a path, create a node at that grid location (obtained from the gridNodes array GetGridNode at this position), 
                            // and populate it's movement penalty with pathMovementPenalty
                            else if (gridPropertyDetails.isPath == true)
                            {
                                Node node = gridNodes.GetGridNode(x, y);
                                node.movementPenalty = pathMovementPenalty;
                            }
                            // Else, if the current square is not a path or obstacle, create a node at that grid location (obtained from the gridNodes array GetGridNode at this position), 
                            // and populate it's movement penalty with defaultMovementPenalty
                            else
                            {
                                Node node = gridNodes.GetGridNode(x, y);
                                node.movementPenalty = defaultMovementPenalty;
                            }
                        }
                    }
                }
            }
            else
            {
                return false;
            }
        }
        // Return false if we never found a sceneSave for this scen!
        else
        {
            return false;
        }

        // Return true at the end, so we know we've successfully populated the grid nodes
        return true;
    }
}
