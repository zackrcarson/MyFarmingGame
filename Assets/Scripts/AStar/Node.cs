using System;
using UnityEngine;

// Used to store details about nodes that will be used in the Astar algorithm (e.g. grid position, Gcost, Hcost, Fcost, isObstacle, movementPenalty, parentNode)
// The ICompareable interfact requires us to create a CompareTo method used to compare this node's fCost to another nodes fCost
// When we make a list of Nodes, we will sort them using a method that requires all elements to have an IComparable interface.
// We will have a node for every position on our gridMaps for each scene!
public class Node : IComparable<Node>
{
    public Vector2Int gridPosition; // Position in the grid tilemap this node exists on
    public int gCost = 0; // distance from the starting node, based on the route to the parentNode (defined below)
    public int hCost = 0; // distance from the finishing node
    public bool isObstacle = false;
    public int movementPenalty; // Penalties and bonuses (i.e. for being on a path, not being on a path)
    public Node parentNode; // Instance of the parent node, also storing all of the above members (it itself has a parent, all the way back to the start!)


    // Constructor for this class, initializes the grid position and the parentNode
    public Node(Vector2Int gridPosition)
    {
        this.gridPosition = gridPosition;

        parentNode = null;
    }


    /// <summary>
    /// This property returns the FCost as the sum of the gCost and hCost
    /// </summary>
    public int FCost
    {
        get
        {
            return gCost + hCost;
        }
    }


    /// <summary>
    /// This is a method required by the ICompareable interface, which compares this node's fCost, to another node's fCost passed into the method.
    /// It will return -1 if this node's fCost is smaller than the other, 0 if they're equal, and 1 if the other node is smaller.
    /// </summary>
    public int CompareTo(Node nodeToCompare)
    {
        // Compare is <0 if this instances fCost is less than nodeToCompare's fCost, >0 if it's greater, and =0 if they are the same

        // Compare the fCosts for each node
        int compare = FCost.CompareTo(nodeToCompare.FCost);
        
        // If the fCosts are equivalent, use the hCost as a tie breaker
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }

        return compare;
    }
}
