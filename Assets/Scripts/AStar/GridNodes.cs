using UnityEngine;

// This class stores a 2D array of nodes of size width, height (calculated from the scene Name), one Node for each tile in our tilemap
public class GridNodes
{
    // The overall width and height of our tilemap in question
    private int width;
    private int height;

    // 2D array of Nodes to be populated, one for each tile in our gridMap
    private Node[,] gridNode;


    // This constructor initializes the dimensions of the Node array from the tilemap size, and constructs a Node
    // at each position (Node initialization only requires the x, y coordinates)
    public GridNodes(int width, int height)
    {
        this.width = width;
        this.height = height;

        gridNode = new Node[width, height];

        // For each tile in the map, initialize a new Node object (Node constructor takes in the x, y location) at that location
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                gridNode[x, y] = new Node(new Vector2Int(x, y));
            }
        }
    }


    // This method will return a Node at the given x, y position, as long as they are within the scenes range
    public Node GetGridNode(int xPosition, int yPosition)
    {
        if (xPosition < width && yPosition < height)
        {
            return gridNode[xPosition, yPosition];
        }
        else
        {
            Debug.Log("Requested grid node at (" + xPosition + ", " + yPosition + ") is out of range, with grid of dimensionality " + width + " x " + height + ".");
            return null;
        }
    }
}
