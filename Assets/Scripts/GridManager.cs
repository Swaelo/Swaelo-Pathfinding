﻿// ================================================================================================================================
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

    //UI component used to specify which pathfinding algorithm should be used
    public Dropdown AlgorithmSelector;

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
            Log.Print("Path ends need to be set before a path can be found.");
            return;
        }

        //Get the current algorithm selection
        int AlgorithmSelection = AlgorithmSelector.value;
        switch(AlgorithmSelection)
        {
            case (0):
                AStarPathFinder.Instance.FindPath(PathStart, PathEnd);
                break;
            case (1):
                DijkstrasPathFinder.Instance.FindPath(PathStart, PathEnd);
                break;
        }
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
        //Create a new list to store all the neighboring nodes
        List<Node> Neighbours = new List<Node>();

        //Start by adding the adjacent nodes in straight directions
        if (Node.HasNeighbour(Direction.North))
            Neighbours.Add(Node.GetNeighbour(Direction.North));
        if (Node.HasNeighbour(Direction.East))
            Neighbours.Add(Node.GetNeighbour(Direction.East));
        if (Node.HasNeighbour(Direction.South))
            Neighbours.Add(Node.GetNeighbour(Direction.South));
        if (Node.HasNeighbour(Direction.West))
            Neighbours.Add(Node.GetNeighbour(Direction.West));

        //Add diagonal neighbours
        if (Node.HasNeighbour(Direction.NorthEast))
            Neighbours.Add(Node.GetNeighbour(Direction.NorthEast));
        if (Node.HasNeighbour(Direction.SouthEast))
            Neighbours.Add(Node.GetNeighbour(Direction.SouthEast));
        if (Node.HasNeighbour(Direction.SouthWest))
            Neighbours.Add(Node.GetNeighbour(Direction.SouthWest));
        if (Node.HasNeighbour(Direction.NorthWest))
            Neighbours.Add(Node.GetNeighbour(Direction.NorthWest));

        //Return the final list of neighbouring nodes
        return Neighbours;
    }

    //Returns a list of the nodes traversable neighbours
    public List<Node> GetTraversableNeighbours(Node Node)
    {
        //Start by just grabbing all the neighbours, regardless of traversability
        List<Node> Neighbours = GetNeighbours(Node);

        //Now sort through this list, finding any which are traversable, adding those to a new 2nd list
        List<Node> TraversableNeighbours = new List<Node>();
        foreach (Node Neighbour in Neighbours)
            if (Neighbour.IsTraversable())
                TraversableNeighbours.Add(Neighbour);

        //Finally return the list of neighbours which were found to be traversable
        return TraversableNeighbours;
    }

    //Checks if there exists a node with the given grid coordinates
    public bool NodeExists(Vector2 NodePos)
    {
        bool NodeExists = true;
        if (NodePos.x < 0 || NodePos.x >= GridSize.x || NodePos.y < 0 || NodePos.y >= GridSize.y)
            NodeExists = false;
        return NodeExists;
    }

    //Returns a node from the grid which matches the given coordinates
    public Node GetNode(Vector2 NodePos)
    {
        return Nodes[(int)NodePos.x][(int)NodePos.y].GetComponent<Node>();
    }

    //Hides all nodes came from parent node indicators
    public void HideAllParentIndicators()
    {
        foreach(List<GameObject> Columns in Nodes)
        {
            foreach (GameObject Node in Columns)
                Node.GetComponent<Node>().ToggleParentIndicator(false);
        }
    }

    //Finds the sum of absolute values between two nodes
    public static float FindHeuristic(Node A, Node B)
    {
        //Use manhattan heuristic if the nodes are directly adjacent to one another in the N, E, S or W direction
        Direction NeighbourDirection = A.GetDirection(B);
        switch(NeighbourDirection)
        {
            case (Direction.North):
                return ManhattanHeuristic(A, B);
            case (Direction.NorthEast):
                return DiagonalHeuristic(A, B);
            case (Direction.East):
                return ManhattanHeuristic(A, B);
            case (Direction.SouthEast):
                return DiagonalHeuristic(A, B);
            case (Direction.South):
                return ManhattanHeuristic(A, B);
            case (Direction.SouthWest):
                return DiagonalHeuristic(A, B);
            case (Direction.West):
                return ManhattanHeuristic(A, B);
            case (Direction.NorthWest):
                return DiagonalHeuristic(A, B);
        }
        return Mathf.Infinity;
    }

    //Finds the sum of absolute values of differences in the two nodes X and Y coordinates
    private static float ManhattanHeuristic(Node A, Node B)
    {
        int XDistance = (int)Mathf.Abs(A.NodePos.x - B.NodePos.x);
        int YDistance = (int)Mathf.Abs(A.NodePos.y - B.NodePos.y);
        return XDistance + YDistance;
    }

    //Finds the maximum of absolute values of differences in the two nodes X and Y coordinates
    private static float DiagonalHeuristic(Node A, Node B)
    {
        return Mathf.Max(Mathf.Abs(A.NodePos.x - B.NodePos.x),
            Mathf.Abs(A.NodePos.y - B.NodePos.y));
    }

    //Returns a list containing every node in the entire grid
    public List<Node> GetCompleteNodeList()
    {
        List<Node> NodeList = new List<Node>();
        foreach(List<GameObject> Columns in Nodes)
        {
            foreach (GameObject Node in Columns)
                NodeList.Add(Node.GetComponent<Node>());
        }
        return NodeList;
    }
}