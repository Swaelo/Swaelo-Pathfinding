// ================================================================================================================================
// File:        Node.cs
// Description:	Stores all the nessacery info for a single node in the grid
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class Node : MonoBehaviour
{
    public NodeType Type = NodeType.Open;   //This nodes type
    public Vector2 NodePos; //This nodes position in the grid

    //Pathfinding values
    public Node Parent; //Node preceding this on the cheapest path from the start
    public float GScore;    //Cost to travel here from the start node along the shortest path
    public float FScore;    //Current best guess as to how short a path from start to finish can be if it goes through this node
    public bool Opened;     //Tracks if this node has been looked at yet
    public bool Closed;     //Tracks if the node is finished being looked at

    //Materials and renderer components
    public NodeMaterial[] Materials;
    public MeshRenderer Renderer;

    //Arrow pointing to this nodes parent
    public GameObject ParentIndicator;  //The arrow object
    public void ToggleParentIndicator(bool Show) { ParentIndicator.SetActive(Show); }    //Sets the indicators visibility
    public void PointIndicator(Direction At) { ParentIndicator.transform.rotation = Quaternion.Euler(0f, (float)At, 0f); }  //Points the indicator to face a certain way
    public Direction GetDirection(Node Other)   //Returns what direction the Other node is in relation to this one
    {
        if (Other.NodePos.y > NodePos.y)
            return Direction.North;
        if (Other.NodePos.x > NodePos.x)
            return Direction.East;
        if (Other.NodePos.y < NodePos.y)
            return Direction.South;
        if (Other.NodePos.x < NodePos.x)
            return Direction.West;
        return Direction.North;
    }

    //Resets all the pathfinding values in preperation for a new pathway search
    public void ResetPathValues()
    {
        Parent = null;
        GScore = Mathf.Infinity;
        FScore = Mathf.Infinity;
        Opened = false;
        Closed = false;
    }

    //Sets the node to a new type
    public void SetType(NodeType NewType)
    {
        //Store the new type thats being set
        Type = NewType;
        //Update the material to match the new type given
        string MaterialName = Type.ToString() + "Mat";
        //Debug.Log("Setting material: " + MaterialName);
        Renderer.material = GetMaterial(MaterialName);
        //Tell the GridManager when settings pathway start/end nodes
        if(NewType == NodeType.Start)
        {
            //Ignore if trying to reset the same start node
            if (GridManager.Instance.PathStart == this)
                return;
            //If theres already a previous start node, set that one back to an open node before setting this one to the start node
            if (GridManager.Instance.PathStart != null)
                GridManager.Instance.PathStart.SetType(NodeType.Open);
            GridManager.Instance.PathStart = this;
        }
        else if(NewType == NodeType.End)
        {
            if (GridManager.Instance.PathEnd == this)
                return;
            if (GridManager.Instance.PathEnd != null)
                GridManager.Instance.PathEnd.SetType(NodeType.Open);
            GridManager.Instance.PathEnd = this;
        }
    }

    //Gets one of the nodes materials
    private Material GetMaterial(string Name)
    {
        //Search through all the materials in the nodes material list
        for(int i = 0; i < Materials.Length; i++)
        {
            //Check the names until we find the requested material
            NodeMaterial NodeMat = Materials[i];
            if (NodeMat.Name == Name)
                return NodeMat.Material;
        }

        Debug.Log("Couldnt find node material: " + Name);
        return null;
    }

    //Checks if the node is traversable
    public bool IsTraversable()
    {
        return Type != NodeType.Wall;
    }

    //Returns the grid coordinates of where one of this nodes neighbours would be
    public Vector2 GetNeighbourPos(Direction NeighbourDirection)
    {
        //Start with current position
        Vector2 NeighbourPos = NodePos;

        //Offset in the specified direction
        switch(NeighbourDirection)
        {
            case (Direction.North):
                NeighbourPos.y += 1;
                break;
            case (Direction.East):
                NeighbourPos.x += 1;
                break;
            case (Direction.South):
                NeighbourPos.y -= 1;
                break;
            case (Direction.West):
                NeighbourPos.x -= 1;
                break;
        }

        //Return the position of the neighbour
        return NeighbourPos;
    }

    //Checks if the node has an existing neighbour in the given direction
    public bool HasNeighbour(Direction NeighbourDirection)
    {
        bool HasNeighbour = GridManager.Instance.NodeExists(GetNeighbourPos(NeighbourDirection));
        return HasNeighbour;
    }

    //Checks if the node has a traversable neighbour in the given direction
    public bool HasTraversableNeighbour(Direction NeighbourDirection)
    {
        if (!HasNeighbour(NeighbourDirection))
            return false;
        return GridManager.Instance.GetNode(GetNeighbourPos(NeighbourDirection)).IsTraversable();
    }

    //Returns the nodes neighbour in the given direction
    public Node GetNeighbour(Direction NeighbourDirection)
    {
        //Make sure the neighbour exists in that direction first
        if(!HasNeighbour(NeighbourDirection))
            return null;
        return GridManager.Instance.GetNode(GetNeighbourPos(NeighbourDirection));
    }
}
