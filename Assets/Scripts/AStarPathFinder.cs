// ================================================================================================================================
// File:        AStarPathFinder.cs
// Description:	Uses A* to find a path between two nodes, based on pseudocode https://en.wikipedia.org/wiki/A*_search_algorithm
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using System.Collections.Generic;
using UnityEngine;

public class AStarPathFinder : MonoBehaviour
{
    //Singleton instance
    public static AStarPathFinder Instance = null;
    private void Awake() { Instance = this; }

    //Uses A* to find a path between the two nodes
    public List<Node> FindPathWikipedia(Node StartNode, Node EndNode)
    {
        List<Node> OpenSet = new List<Node>();  //Set of discovered nodes that may still need to be evaluated for possible shorter pathways

        //Reset values of all nodes in the grid
        GridManager.Instance.ResetAStarValues();

        //Set the starting nodes initial values add add it into the OpenSet
        StartNode.GScore = 0;
        StartNode.FScore = GetHeuristicCost(StartNode, EndNode);
        OpenSet.Add(StartNode);

        int OpenSetIterations = 0;

        //Iterate over the OpenSet until a path is found, or all possible pathways have been exhausted
        while(OpenSet.Count > 0)
        {
            OpenSetIterations++;

            //Find the CurrentNode as the node from the OpenSet with the lowest FScore value
            Node CurrentNode = FindCurrentNode(OpenSet);

            //Return the final pathway if its been found
            if (CurrentNode == EndNode)
            {
                Debug.Log("Path found after " + OpenSetIterations + " open set iterations.");
                return ReconstructPathway(StartNode, EndNode);
            }

            //Remove the CurrentNode from the OpenSet, then find and iterate over all its neighbours
            OpenSet.Remove(CurrentNode);
            List<Node> Neighbours = GridManager.Instance.GetTraversableNeighbours(CurrentNode);
            foreach(Node Neighbour in Neighbours)
            {
                //Compare the cost of travelling from the StartNode to this Neighbour through the CurrentNode
                int TentativeGScore = CurrentNode.GScore + GetHeuristicCost(CurrentNode, Neighbour);
                if(TentativeGScore < Neighbour.GScore)
                {
                    //Update this as the proper way to travel if its got a cheaper cost
                    Neighbour.CameFrom = CurrentNode;
                    Neighbour.GScore = TentativeGScore;
                    Neighbour.FScore = Neighbour.GScore + GetHeuristicCost(Neighbour, EndNode);
                    //Add this neighbour into the OpenSet if its not already
                    if (!OpenSet.Contains(Neighbour))
                        OpenSet.Add(Neighbour);
                }
            }
        }

        Debug.Log("No pathway available from " + StartNode.NodePos.x + ", " + StartNode.NodePos.y + " to " + EndNode.NodePos.x + ", " + EndNode.NodePos.y + ".");
        return null;
    }

    //Calculates the heuristic cost value to travel between two nodes on the grid
    private int GetHeuristicCost(Node A, Node B)
    {
        int XDistance = (int)Mathf.Abs(A.NodePos.x - B.NodePos.x);
        int YDistance = (int)Mathf.Abs(A.NodePos.y - B.NodePos.y);
        return XDistance + YDistance;
    }

    //Reconstructs and returns the final pathway between the start and end nodes
    private List<Node> ReconstructPathway(Node StartNode, Node EndNode)
    {
        List<Node> Pathway = new List<Node>();

        Node CurrentNode = EndNode;
        while(CurrentNode != StartNode)
        {
            Pathway.Add(CurrentNode);
            CurrentNode = CurrentNode.CameFrom;
        }

        Pathway.Reverse();
        return Pathway;
    }

    //Returns the node from OpenSet with the lowest FScore value
    private Node FindCurrentNode(List<Node> OpenSet)
    {
        Node CurrentNode = OpenSet[0];
        float CurrentNodeCost = CurrentNode.FScore;

        for(int i = 1; i < OpenSet.Count; i++)
        {
            Node NodeCompare = OpenSet[i];
            float CostCompare = NodeCompare.FScore;
            if (CostCompare < CurrentNodeCost)
            {
                CurrentNode = NodeCompare;
                CurrentNodeCost = CostCompare;
            }
        }

        return CurrentNode;
    }
}
