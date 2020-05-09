// ================================================================================================================================
// File:        DijkstrasPathFinder.cs
// Description:	Uses Dijkstras algorithm to find a path between two nodes, based on pseudocode from https://en.wikipedia.org/wiki/Dijkstra%27s_algorithm#Pseudocode
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using System;
using System.Collections.Generic;
using UnityEngine;

public class DijkstrasPathFinder : MonoBehaviour
{
    //Singleton instance
    public static DijkstrasPathFinder Instance = null;
    private void Awake() { Instance = this; }

    //Pathway start/end nodes and everything in between
    private Node Start; //Where the pathway begins
    private Node End;   //Where the pathway ends
    private List<Node> Q;   //Set of nodes that need to be searched

    //Pathfinding visualization
    private bool FindingPathway = false;
    private float StepInterval = 0.005f;
    private float NextStep = 0.005f;
    private int QSetIterations = 0;

    private void Update()
    {
        if (FindingPathway)
        {
            NextStep -= Time.deltaTime;
            if (NextStep <= 0.0f)
            {
                NextStep = StepInterval;
                IterateQSet();
            }
        }
    }

    //Begins pathfinding process
    public void FindPath(Node Start, Node End)
    {
        this.Start = Start;
        this.End = End;
        FindingPathway = true;

        GridManager.Instance.ResetAllPathValues();
        Q = new List<Node>();

        //Reset the values of all nodes and add them all to the Q set
        foreach (Node Node in GridManager.Instance.GetCompleteNodeList())
        {
            Node.Dist = Mathf.Infinity;
            Node.Prev = null;
            Q.Add(Node);
        }

        Start.Dist = 0;
    }

    private void IterateQSet()
    {
        QSetIterations++;

        //Once the Q set it emptied, we can reconstruct the final pathway
        if (Q.Count <= 0)
        {
            //Get the final reconstructed pathway
            List<Node> FinalPathway = ReconstructPathway();

            //Announce no pathway could be found if the path is null
            if (FinalPathway == null)
            {
                Log.Print("No pathway was able to be found.");
                return;
            }

            Log.Print("Dijkstras pathfinding complete after " + QSetIterations + " iterations.");

            //Complete the pathfinding process
            FindingPathway = false;
            FinalPathway.Remove(Start);
            FinalPathway.Remove(End);

            //Hide all nodes came from parent node indicators
            GridManager.Instance.HideAllParentIndicators();

            //Display the pathway nodes
            foreach (Node PathStep in FinalPathway)
                PathStep.SetType(NodeType.Pathway);

            return;
        }

        //Find the node in Q with the lowest distance value
        Node U = FindU();

        //Remove it from the Q set
        Q.Remove(U);

        //Iterate over all of U's neighbours
        foreach (Node V in GridManager.Instance.GetTraversableNeighbours(U))
        {
            //Ignore neighbours which are no longer part of the Q set
            if (!Q.Contains(V))
                continue;

            //Find the cost of travelling through this node
            float Alt = U.Dist + GridManager.FindHeuristic(U, V);
            //Update this as the better way to travel if its got a cheaper cost
            if (Alt < V.Dist)
            {
                V.Dist = Alt;
                V.Prev = U;
                V.ToggleParentIndicator(true);
                V.PointIndicator(V.GetDirection(U));
            }
        }
    }

    //Returns the Node from Q set with the lowest Dist value
    private Node FindU()
    {
        Node CheapestNode = Q[0];

        for (int i = 1; i < Q.Count; i++)
        {
            if (Q[i].Dist < CheapestNode.Dist)
                CheapestNode = Q[i];
        }

        return CheapestNode;
    }

    //Follows came from chain from the end node all the way back to the start node
    private List<Node> ReconstructPathway()
    {
        List<Node> Pathway = new List<Node>();

        Node Current = End;
        while (Current != Start)
        {
            Pathway.Add(Current);

            //Return null if any node links doesnt link to anything
            if (Current.Prev == null)
                return null;

            Current = Current.Prev;
        }

        Pathway.Reverse();
        return Pathway;
    }
}