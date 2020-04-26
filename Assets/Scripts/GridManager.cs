// ================================================================================================================================
// File:        GridManager.cs
// Description:	Sets up the grid array for testing pathfinding algorithms
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    //Singleton instance
    public static GridManager Instance = null;
    private void Awake() { Instance = this; }

    //UI Elements
    public GameObject GridSetupWindow;  //Set of UI elements used to initially setup the level grid
    public InputField WidthInput;   //Input fields to enter desired grid size
    public InputField HeightInput;
    private int MaxGridSize = 128;  //Grid size limitation

    //Level Grid
    private Vector2 GridSize = new Vector2(2, 2);   //Width/Height of the grid stored here once initialized through UI
    public GameObject NodePrefab;   //Grid node prefab used when setting up
    private float NodeSpacing = 1.085f; //Space between each node in the grid
    public List<List<GameObject>> Nodes; //All grid nodes stored in this 2d list once initialized

    //Pathway Nodes
    public Node PathStart = null;
    public Node PathEnd = null;

    //Quick setup for rapid testing
    public bool QuickSetup = false; //If true, immediatly setups up grid with specified size on scene load
    public Vector2 QuickSetupSize = new Vector2(15, 15);    //Size of grid for use with quick setup
    public Vector2 QuickStartPos = new Vector2(2, 2);   //Position of path start node for quick setup
    public Vector2 QuickEndPos = new Vector2(14, 14);   //Pos of path end for quick setup

    private void Start()
    {
        //Override with quick setup settings when quick setup is enabled
        if(QuickSetup)
        {
            GridSize = QuickSetupSize;
            InitializeGrid();
            PathStart = GetNode(QuickStartPos);
            PathStart.SetType(NodeType.Start);
            PathEnd = GetNode(QuickEndPos);
            PathEnd.SetType(NodeType.End);
        }
    }

    //UI specific functions to make sure grid dimensions entered in stay within max limits
    public void DimensionsUpdated()
    {
        if(WidthInput.text != "")
        {
            //Fetch and store new grid width entered into UI input field
            GridSize.x = Convert.ToInt32(WidthInput.text);
            //Make sure it remains within size limits
            GridSize.x = Mathf.Clamp(GridSize.x, 2, MaxGridSize);
            //Update UI with clamped size value
            WidthInput.text = GridSize.x.ToString();
        }
        //Do all the same thing if with height input too
        if(HeightInput.text != "")
        {
            GridSize.y = Convert.ToInt32(HeightInput.text);
            GridSize.y = Mathf.Clamp(GridSize.y, 2, MaxGridSize);
            HeightInput.text = GridSize.y.ToString();
        }
    }

    //UI button function to finish grid setup
    public void ClickSetupGrid()
    {
        InitializeGrid();
    }

    //UI button function to initiate pathfinding between the start and end nodes
    public void ClickFindPath()
    {
        //Make sure start and end nodes have been set
        if(PathStart == null || PathEnd == null)
        {
            Debug.Log("Path ends need to be set before a path can be found.");
            return;
        }

        AStarPathFinder.Instance.FindPath(PathStart, PathEnd);
    }

    //Sets up the level grid
    private void InitializeGrid()
    {
        //Reposition the camera so the entire grid will remain in view
        float MaxGridSize = Mathf.Max(GridSize.x, GridSize.y);
        Camera.main.transform.position = new Vector3(0f, MaxGridSize + 3, 0f);

        //Initialize the storage lists
        Nodes = new List<List<GameObject>>();
        for (int i = 0; i < GridSize.x; i++)
            Nodes.Add(new List<GameObject>());

        //Setup the grid nodes
        Vector3 SpawnPos = new Vector3(-(GridSize.x * 0.5f * NodeSpacing), 0f, -(GridSize.y * 0.5f * NodeSpacing));
        for (int Column = 0; Column < GridSize.x; Column++)
        {
            for (int Row = 0; Row < GridSize.y; Row++)
            {
                //Spawn in each new node, assign them their grid coordinates, then store them in the lists with the others
                GameObject Node = Instantiate(NodePrefab, SpawnPos, Quaternion.identity);
                Node.GetComponent<Node>().NodePos = new Vector2(Column, Row);
                Nodes[Column].Add(Node);

                //Offset Z position for next node spawn
                SpawnPos.z += NodeSpacing;
            }

            //Offset X position and reset Z position for next row of spawns
            SpawnPos.x += NodeSpacing;
            SpawnPos.z = -(GridSize.y * 0.5f * NodeSpacing);
        }

        //Hide setup UI elements now that the grid has been initialized
        GridSetupWindow.SetActive(false);
    }

    //Resets pathfinding values of all nodes in the grid
    public void ResetAllPathValues()
    {
        foreach (List<GameObject> Row in Nodes)
            foreach (GameObject Node in Row)
                Node.GetComponent<Node>().ResetPathValues();
    }

    //Returns a list of the nodes neighbours
    public List<Node> GetNeighbours(Node Node)
    {
        List<Node> Neighbours = new List<Node>();
        if (Node.HasNeighbour(Direction.North))
            Neighbours.Add(Node.GetNeighbour(Direction.North));
        if (Node.HasNeighbour(Direction.East))
            Neighbours.Add(Node.GetNeighbour(Direction.East));
        if (Node.HasNeighbour(Direction.South))
            Neighbours.Add(Node.GetNeighbour(Direction.South));
        if (Node.HasNeighbour(Direction.West))
            Neighbours.Add(Node.GetNeighbour(Direction.West));
        return Neighbours;
    }

    //Returns a list of the nodes traversable neighbours
    public List<Node> GetTraversableNeighbours(Node Node)
    {
        //Create a new list to store the neighbours
        List<Node> Neighbours = new List<Node>();

        //Add any existing (traversable) neighbours to the list
        if (Node.HasTraversableNeighbour(Direction.North))
            Neighbours.Add(Node.GetNeighbour(Direction.North));
        if (Node.HasTraversableNeighbour(Direction.East))
            Neighbours.Add(Node.GetNeighbour(Direction.East));
        if (Node.HasTraversableNeighbour(Direction.South))
            Neighbours.Add(Node.GetNeighbour(Direction.South));
        if (Node.HasTraversableNeighbour(Direction.West))
            Neighbours.Add(Node.GetNeighbour(Direction.West));

        //Return the list of traversable neighbours that could be found
        return Neighbours;
    }

    //Checks if there exists a node with the given grid coordinates
    public bool NodeExists(Vector2 NodePos)
    {
        bool NodeExists = true;
        if (NodePos.x < 0 || NodePos.x >= GridSize.x || NodePos.y < 0 || NodePos.y >= GridSize.y)
            NodeExists = false;
        //Debug.Log("Does node " + NodePos.x + ", " + NodePos.y + " exist in grid of size " + GridSize.x + " x " + GridSize.y + (NodeExists ? "YEP" : "NOPE"));
        return NodeExists;
    }

    //Returns a node from the grid which matches the given coordinates
    public Node GetNode(Vector2 NodePos)
    {
        return Nodes[(int)NodePos.x][(int)NodePos.y].GetComponent<Node>();
    }
}