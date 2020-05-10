// ================================================================================================================================
// File:        AStarPathFinder.cs
// Description:	Uses A* to find a path between two nodes, based on pseudocode from https://en.wikipedia.org/wiki/A*_search_algorithm
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using System.Collections.Generic;
using UnityEngine;

public class AStarPathFinder : PathFinder
{
    public override void FindPath(Node Start, Node End)
    {
        //Announce the start of the search
        Log.Print("Starting A* pathfind...");

        //Reset iteration counter
        OpenSetIterations = 0;

        //Store path targets
        PathStart = Start;
        PathEnd = End;

        //Reset all pathfinding values
        GridManager.Instance.ResetAllPathValues();

        //Initialize the tracking list
        OpenSet = new List<Node>();
        ClosedSet = new List<Node>();

        //Give the starting node some initial values and place it into the open set
        Start.GScore = 0;
        Start.FScore = 0;
        OpenSet.Add(Start);
        Start.Opened = true;

        //being the pathfinding process
        FindingPathway = true;
    }

    public override void IterateOpenSet()
    {
        //Track how many iterations have taken place to find this pathway so far
        OpenSetIterations++;

        //If the open set is empty, then no pathway was able to be found
        if (OpenSet.Count <= 0)
        {
            //Print a failure message and reset the grid
            Log.Print("Unable to find a valid pathway using A* algorithm.");
            FindingPathway = false;
            GridManager.Instance.HideAllParentIndicators();
            return;
        }

        //Get the cheapest node currently being stored in the open set
        Node Current = GridManager.Instance.FindCheapestNode(OpenSet);
        OpenSet.Remove(Current);

        //When Current matches the end node, the pathway is ready to be reconstructed
        if (Current == PathEnd)
        {
            //Announce the pathway has been found and how long it took to find
            Log.Print("A* pathfinding completed after " + OpenSetIterations + " iterations.");

            //Hide all parent indicators
            GridManager.Instance.HideAllParentIndicators();

            //Grab and display the final pathway
            List<Node> Pathway = GridManager.Instance.GetCompletedPathway(PathStart, PathEnd);
            foreach (Node PathStep in Pathway)
                PathStep.SetType(NodeType.Pathway);

            //Finalize the process
            FindingPathway = false;
            return;
        }

        //Remove the current node from the open set, then iterate over its neighbours
        OpenSet.Remove(Current);
        foreach (Node Neighbour in GridManager.Instance.GetTraversableNeighbours(Current))
        {
            if (Current.GScore < Neighbour.GScore)
            {
                //Update this as the preferred way to travel
                Neighbour.Parent = Current;
                Neighbour.ToggleParentIndicator(true);
                Neighbour.PointIndicator(Neighbour.GetDirection(Current));

                //Update neighbours score values
                Neighbour.GScore = Current.GScore;
                Neighbour.FScore = Neighbour.GScore + GridManager.FindHeuristic(Neighbour, PathEnd);

                //Add neighbour to open set if its not there already
                if (!OpenSet.Contains(Neighbour))
                    OpenSet.Add(Neighbour);
            }
        }
    }
}