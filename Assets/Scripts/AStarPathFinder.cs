// ================================================================================================================================
// File:        AStarPathFinder.cs
// Description:	Uses A* to find a path between two nodes, based on pseudocode from https://en.wikipedia.org/wiki/A*_search_algorithm
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using System.Collections.Generic;
using UnityEngine;

public class AStarPathFinder : MonoBehaviour
{
    //Singleton instance
    public static AStarPathFinder Instance = null;
    private void Awake() { Instance = this; }

    //Pathway start/end nodes, and everything in between
    private Node Start; //Where the pathway begins
    private Node End;   //Where the pathway ends
    private List<Node> OpenSet; //Current set of nodes that still need to be evaluated

    //Pathfinding visualization
    private bool FindingPathway = false;    //Tracks when pathway is currently being searched
    private float StepInterval = 0.005f;    //Time between each openset iteration
    private float NextStep = 0.005f;    //Time until next openset iteration
    private int OpenSetIterations = 0;  //Iterations taken so far to generate the complete pathway

    private void Update()
    {
        if (FindingPathway)
        {
            NextStep -= Time.deltaTime;
            if (NextStep <= 0.0f)
            {
                NextStep = StepInterval;
                IterateOpenSet();
            }
        }
    }

    //Begins the process of finding a pathway between the two given nodes
    public void FindPath(Node Start, Node End)
    {
        //Store the pathway targets
        this.Start = Start;
        this.End = End;

        //Reset the values of all nodes, and reset the openset list
        GridManager.Instance.ResetAllPathValues();
        OpenSet = new List<Node>();

        //Calculate starting nodes initial values and add it into the open set
        Start.GScore = 0;
        Start.FScore = GridManager.FindHeuristic(Start, End);
        OpenSet.Add(Start);

        //Allow the update function to handle the rest of the pathfinding process
        FindingPathway = true;
    }

    //Completes 1 iteration over all the nodes currently in the open set
    private void IterateOpenSet()
    {
        OpenSetIterations++;

        //If the open set is empty, then no pathway was able to be found
        if (OpenSet.Count <= 0)
        {
            Log.Print("no pathway available.");
            FindingPathway = false;
            return;
        }

        //Get the cheapest node currently being stored in the open set
        Node Current = FindCurrent();

        //When Current matches the end node, the pathway is ready to be reconstructed
        if(Current == End)
        {
            //Announce the pathway has been found and how long it took to find
            Log.Print("A* pathfinding complete after " + OpenSetIterations + " iterations.");

            //Hide all nodes came from parent node indicators
            GridManager.Instance.HideAllParentIndicators();

            //Reconstruct the completed pathway
            List<Node> FinalPathway = ReconstructPathway();
            FinalPathway.Remove(Start);
            FinalPathway.Remove(End);

            //Change the types of all the nodes to display the pathway
            foreach (Node PathStep in FinalPathway)
                PathStep.SetType(NodeType.Pathway);

            //Finalize the pathfinding process
            FindingPathway = false;
            return;
        }

        //Remove the current node from the open set, then iterate over all of its neighbours
        OpenSet.Remove(Current);
        List<Node> TraversableNeighbours = GridManager.Instance.GetTraversableNeighbours(Current);
        foreach (Node Neighbour in TraversableNeighbours)
        {
            //Check if its cheaper to travel across this neighbour
            float TentativeGScore = Current.GScore;
            if(TentativeGScore < Neighbour.GScore)
            {
                //Update this as the preffered way to travel
                Neighbour.Parent = Current;
                Neighbour.ToggleParentIndicator(true);
                Neighbour.PointIndicator(Neighbour.GetDirection(Current));
                Neighbour.GScore = TentativeGScore;
                Neighbour.FScore = Neighbour.GScore + GridManager.FindHeuristic(Neighbour, End);
                //Add this to the openset if its not already
                if (!OpenSet.Contains(Neighbour))
                    OpenSet.Add(Neighbour);
            }
        }
    }

    //Follows parent chain from the end node all the way back to the start to reconstruct the final pathway
    private List<Node> ReconstructPathway()
    {
        //Create a new list to store the nodes traversed
        List<Node> Pathway = new List<Node>();

        //Follow the parents from the End all the way back to the Start, adding each into the Pathway list
        Node Current = End;
        while (Current != Start)
        {
            Pathway.Add(Current);
            Current = Current.Parent;
        }

        //Reverse the pathway to complete it before returning it
        Pathway.Reverse();
        return Pathway;
    }

    //Returns the node from the openset which has the lowest FScore value
    private Node FindCurrent()
    {
        //Start with the first node in the set
        Node Current = OpenSet[0];

        //Compare it with all others in the set
        for(int i = 1; i < OpenSet.Count; i++)
        {
            //Update Current whenever another node is found with a lowest FScore
            if (OpenSet[i].FScore < Current.FScore)
                Current = OpenSet[i];
        }

        //Return the node that was found to have the lowest FScore
        return Current;
    }
}