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

    public InputField WidthInput; //UI Element where users inputs the desired grid width
    public int MaxWidth = 128;  //Maximum grid width allowed
    public InputField HeightInput;    //Users inputs desired grid height
    public int MaxHeight = 128; //Maximum grid height allowed
    public GameObject NodePrefab;   //Prefab used to create the grid

    public float NodeSpacing = 1.085f;  //How much space to place between each grid node

    public GameObject SetupUI; //UI components used to setup the grid array, so they can be hidden once its been setup

    //Grid Nodes
    private int GridWidth;
    private int GridHeight;
    public List<List<GameObject>> GridNodes;   //Array of grid nodes

    //Current special nodes
    public GameObject PathStartNode = null;
    public GameObject PathEndNode = null;

    //Fast setup for rabid debugging
    public bool QuickSetup = false;
    public int QuickWidth = 5;
    public int QuickHeight = 5;
    private void Start()
    {
        if (QuickSetup)
        {
            SetupGrid(QuickWidth, QuickHeight);
            GetNode(new Vector2(0, 0)).SetNodeType(NodeType.PathStart);
            GetNode(new Vector2(QuickWidth - 1, QuickHeight - 1)).SetNodeType(NodeType.PathEnd);
            for (int i = 0; i < QuickWidth-1; i++)
                GetNode(new Vector2(i, 1)).SetNodeType(NodeType.WallNode);
            for (int i = QuickWidth-1; i > 0; i--)
                GetNode(new Vector2(i, QuickHeight-2)).SetNodeType(NodeType.WallNode);
        }
    }
    
    //Called by UI Input Field when user finishes entering a new grid width
    public void NewWidthEntered()
    {
        //Make sure current width is a positive value, and doesnt go above the max width
        int CurrentWidth = Convert.ToInt32(WidthInput.text);
        CurrentWidth = Mathf.Clamp(CurrentWidth, 0, MaxWidth);
        WidthInput.text = CurrentWidth.ToString();
    }

    //Called by UI Input Field when user finishes entering a new grid height
    public void NewHeightEntered()
    {
        //Make sure current height remains inside allowed value range
        int CurrentHeight = Convert.ToInt32(HeightInput.text);
        CurrentHeight = Mathf.Clamp(CurrentHeight, 0, MaxHeight);
        HeightInput.text = CurrentHeight.ToString();
    }

    //Called by UI button once user has entered the desired grid size
    public void ConfirmGridSize()
    {
        //Make sure both inputfields have some value entered into them
        if(WidthInput.text == "" || HeightInput.text == "")
        {
            Debug.Log("No grid size entered.");
            return;
        }

        //Get the desired grid size
        GridWidth = Convert.ToInt32(WidthInput.text);
        GridHeight = Convert.ToInt32(HeightInput.text);

        //Setup the grid
        SetupGrid(GridWidth, GridHeight);
    }

    //Sets up the grid with the specified size
    private void SetupGrid(int Width, int Height)
    {
        GridWidth = Width;
        GridHeight = Height;

        //Reposition the camera so the entire grid is in view
        int GridAbs = GridWidth > GridHeight ? GridWidth : GridHeight;
        Vector3 NewCameraPos = new Vector3(0f, GridAbs + 3, 0f);
        Camera.main.transform.position = NewCameraPos;

        //Initialize storage lists
        GridNodes = new List<List<GameObject>>();
        for (int i = 0; i < GridWidth; i++)
        {
            //Place each row into the list
            List<GameObject> RowList = new List<GameObject>();
            GridNodes.Add(RowList);
        }

        //Spawn in all the grid nodes
        Vector3 SpawnPos = new Vector3(-(GridWidth * 0.5f * NodeSpacing), 0f, -(GridHeight * 0.5f * NodeSpacing));
        for (int w = 0; w < GridWidth; w++)
        {
            for (int h = 0; h < GridHeight; h++)
            {
                //Spawn in the next node
                GameObject NodeSpawn = Instantiate(NodePrefab, SpawnPos, Quaternion.identity);

                //Place it in the storage list
                GridNodes[w].Add(NodeSpawn);

                //Tell it its position in the grid
                NodeSpawn.GetComponent<Node>().NodePos = new Vector2(w, h);

                //Offset z pos for next spawn
                SpawnPos.z += NodeSpacing;
            }

            //Offset x pos for next spawn and reset z pos
            SpawnPos.x += NodeSpacing;
            SpawnPos.z = -(GridHeight * 0.5f * NodeSpacing);
        }

        //Hide the grid setup UI
        SetupUI.SetActive(false);
    }

    //Called by node details script when it detects its been assigned as a new path start/end node
    public void AssignStartNode(GameObject NewStartNode)
    {
        if (PathStartNode != null)
            PathStartNode.GetComponent<Node>().SetNodeType(NodeType.PathNode);
        PathStartNode = NewStartNode;
    }
    public void AssignEndNode(GameObject NewEndNode)
    {
        if (PathEndNode != null)
            PathEndNode.GetComponent<Node>().SetNodeType(NodeType.PathNode);
        PathEndNode = NewEndNode;
    }

    //Finds path between the start and end nodes
    public void FindPathButtonFunction()
    {
        //Make sure a valid start and end node have been set
        if (PathStartNode == null || PathEndNode == null)
            return;

        //Find the pathway between the two nodes
        Node Start = PathStartNode.GetComponent<Node>();
        Node End = PathEndNode.GetComponent<Node>();

        List<Node> Pathway = AStarPathFinder.Instance.FindPathWikipedia(Start, End);

        //Highlight the pathway between the nodes
        Pathway.Remove(Start);
        Pathway.Remove(End);
        foreach (Node PathwayNode in Pathway)
            PathwayNode.SetNodeType(NodeType.PathWay);
    }

    //Resets the A* values of all nodes in the grid
    public void ResetAStarValues()
    {
        foreach(List<GameObject> Row in GridNodes)
        {
            foreach (GameObject Node in Row)
                Node.GetComponent<Node>().ResetAStarValues();
        }
    }

    //Returns a list of the nodes traversable neighbouring nodes
    public List<Node> GetTraversableNeighbours(Node Node)
    {
        //Create a new list to store the traversable neighbours
        List<Node> Neighbours = new List<Node>();

        //Get the coordinates of all the supposed neighbouring nodes
        Vector2 NorthPos = new Vector2(Node.NodePos.x, Node.NodePos.y + 1);
        Vector2 EastPos = new Vector2(Node.NodePos.x + 1, Node.NodePos.y);
        Vector2 SouthPos = new Vector2(Node.NodePos.x, Node.NodePos.y - 1);
        Vector2 WestPos = new Vector2(Node.NodePos.x - 1, Node.NodePos.y);

        //Any traversable nodes that exist at these locations get added to the list of neighbours
        if (NodeExists(NorthPos) && GetNode(NorthPos).Traversable())
            Neighbours.Add(GetNode(NorthPos));
        if (NodeExists(EastPos) && GetNode(EastPos).Traversable())
            Neighbours.Add(GetNode(EastPos));
        if (NodeExists(SouthPos) && GetNode(SouthPos).Traversable())
            Neighbours.Add(GetNode(SouthPos));
        if (NodeExists(WestPos) && GetNode(WestPos).Traversable())
            Neighbours.Add(GetNode(WestPos));

        //Return the final list of traversable neighbouring nodes
        return Neighbours;
    }

    //Checks if a node of the given coordinates exists within the grid
    private bool NodeExists(Vector2 NodePos)
    {
        if (NodePos.x < 0 || NodePos.x >= GridWidth ||
            NodePos.y < 0 || NodePos.y >= GridHeight)
            return false;
        return true;
    }

    //Returns the node with the given grid coordinates
    private Node GetNode(Vector2 NodePos)
    {
        return GridNodes[(int)NodePos.x][(int)NodePos.y].GetComponent<Node>();
    }
}
