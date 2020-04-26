// ================================================================================================================================
// File:        Node.cs
// Description:	Stores all the nessacery info for a single node in the grid
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public enum NodeType
{
    PathNode = 0,   //Path nodes can be travelled across
    WallNode = 1,    //Wall nodes cannot be travelled across
    PathStart = 2,
    PathEnd = 3,
    PathWay = 4
}

public class Node : MonoBehaviour
{
    //A* variables
    public Node CameFrom; //The node immediately preceding this one on the cheapest path from the starting node
    public int GScore;    //The cost of the cheapest path from the starting node to this node
    public int FScore;    //Current best guess as to how short a path from start to finish can be if it goes through this node
    public void ResetAStarValues()
    {
        CameFrom = null;
        GScore = 9999999;
        FScore = 9999999;
    }

    //Node coordinates
    public Vector2 NodePos;

    //Rendering
    public Material PathNodeMat;    //Material used for pathway nodes
    public Material WallNodeMat;    //Materials used for wall nodes
    public Material PathStartMat;
    public Material PathEndMat;
    public Material PathWayMat;

    //Type
    public NodeType Type = NodeType.PathNode;   //Tracks what type of node this is
    //Sets the node to a specific type
    public void SetNodeType(NodeType NewType)
    {
        Type = NewType;
        switch(Type)
        {
            case (NodeType.PathNode):
                GetComponent<MeshRenderer>().material = PathNodeMat;
                break;
            case (NodeType.WallNode):
                GetComponent<MeshRenderer>().material = WallNodeMat;
                break;
            case (NodeType.PathStart):
                GetComponent<MeshRenderer>().material = PathStartMat;
                GridManager.Instance.AssignStartNode(gameObject);
                break;
            case (NodeType.PathEnd):
                GetComponent<MeshRenderer>().material = PathEndMat;
                GridManager.Instance.AssignEndNode(gameObject);
                break;
            case (NodeType.PathWay):
                GetComponent<MeshRenderer>().material = PathWayMat;
                break;
        }
    }

    //Checks if this node is traversable
    public bool Traversable()
    {
        if (Type == NodeType.WallNode)
            return false;
        return true;
    }
}
