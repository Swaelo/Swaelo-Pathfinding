// ================================================================================================================================
// File:        ThetaStarPathFinder.cs
// Description:	Uses Theta* to find a path between two nodes, based on pseudocode from https://en.wikipedia.org/wiki/Theta*
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using System.Collections.Generic;
using UnityEngine;

public class ThetaStarPathFinder : PathFinder
{
    public override void FindPath(Node Start, Node End)
    {
        Log.Print("Starting Theta* path find...");

        //Reset iteration count
        OpenSetIterations = 0;

        //Store path targets
        PathStart = Start;
        PathEnd = End;

        //Reset all nodes pathfinding values
        GridManager.Instance.ResetAllPathValues();

        //Initialize the open and closed sets
        OpenSet = new List<Node>();
        ClosedSet = new List<Node>();

        //Begin the pathfinding process
        FindingPathway = true;

        //Give the starting node some initial values and add it into the open set
        PathStart.GScore = GridManager.FindHeuristic(PathStart, PathEnd);
        PathStart.Parent = PathStart;
        OpenSet.Add(PathStart);
    }

    public override void IterateOpenSet()
    {
        //Track how many times the open set has been iterated over
        OpenSetIterations++;

        //If the open set is empty, then no pathway could be found
        if(OpenSet.Count <= 0)
        {
            Log.Print("Unable to find a valid pathway using Theta* algorithm, after a total " + OpenSetIterations + " iterations.");
            FindingPathway = false;
            GridManager.Instance.HideAllParentIndicators();
            return;
        }

        //Grab the node from open set with the lowest FScore value, move it from the OpenSet to the ClosedSet
        Node Current = GridManager.Instance.FindCheapestNode(OpenSet);
        OpenSet.Remove(Current);
        ClosedSet.Add(Current);
        
        //If the Current node is the EndNode, then the pathway has been found
        if(Current == PathEnd)
        {
            //Announce the pathway has been found
            Log.Print("Theta* pathfinding complete after " + OpenSetIterations + " iterations.");

            //Hide all nodes parent indicators
            GridManager.Instance.HideAllParentIndicators();

            //Get the completed pathway
            List<Node> FinalPathway = GridManager.Instance.GetCompletedPathway(PathStart, PathEnd);

            //Change all pathway node types so the completed pathway is displayed
            foreach (Node PathStep in FinalPathway)
                PathStep.SetType(NodeType.Pathway);

            //Finalize the pathfinding process
            FindingPathway = false;
            return;
        }

        //Go through all the current nodes neighbours
        foreach(Node Neighbour in GridManager.Instance.GetTraversableNeighbours(Current))
        {
            //Ignore neighbours in the closed list
            if (ClosedSet.Contains(Neighbour))
                continue;

            if(!OpenSet.Contains(Neighbour))
            {
                Neighbour.GScore = Mathf.Infinity;
                Neighbour.Parent = null;
            }

            UpdateNode(Current, Neighbour);
        }
    }

    //This function is the main difference between A* and Theta* algorithms
    private void UpdateNode(Node Current, Node Neighbour)
    {
        //If there is LoS, ignore current and use the path from its parent to the neighbour node
        if(GridManager.Instance.LineOfSight(Current.Parent, Neighbour))
        {
            //Make sure this pathway is cheaper before updating it
            if(Current.Parent.GScore + GridManager.FindHeuristic(Current.Parent, Neighbour) < Neighbour.GScore)
            {
                Neighbour.GScore = Current.Parent.GScore + GridManager.FindHeuristic(Current.Parent, Neighbour);
                Neighbour.Parent = Current;
                Neighbour.ToggleParentIndicator(true);
                Neighbour.PointIndicator(Neighbour.GetDirection(Current));
                if (OpenSet.Contains(Neighbour))
                    OpenSet.Remove(Neighbour);
                OpenSet.Add(Neighbour);
            }
        }
        else
        {
            if(Current.GScore + GridManager.FindHeuristic(Current, Neighbour) < Neighbour.GScore)
            {
                Neighbour.GScore = Current.GScore + GridManager.FindHeuristic(Current, Neighbour);
                Neighbour.Parent = Current;
                if (OpenSet.Contains(Neighbour))
                    OpenSet.Remove(Neighbour);
            }
        }
    }
}
