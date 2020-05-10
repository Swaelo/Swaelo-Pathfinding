// ================================================================================================================================
// File:        DijkstrasPathFinder.cs
// Description:	Uses Dijkstras algorithm to find a path between two nodes, based on pseudocode from https://en.wikipedia.org/wiki/Dijkstra%27s_algorithm#Pseudocode
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using System;
using System.Collections.Generic;
using UnityEngine;

public class DijkstrasPathFinder : PathFinder
{
    public override void FindPath(Node Start, Node End)
    {
        //Announce the start of the search
        Log.Print("Starting Dijkstra's path find...");

        //Reset iteration count
        OpenSetIterations = 0;

        //Store path targets
        PathStart = Start;
        PathEnd = End;

        //Reset all pathfinding values
        GridManager.Instance.ResetAllPathValues();

        //Initialize tracking list
        OpenSet = new List<Node>();

        //Add all nodes into the open set, give the starting node some initial values
        foreach (Node Node in GridManager.Instance.GetCompleteNodeList())
            OpenSet.Add(Node);
        PathStart.FScore = 0;

        //Begin pathfinding process
        FindingPathway = true;
    }

    public override void IterateOpenSet()
    {
        //Track how many iterations have taken place to find this pathway
        OpenSetIterations++;
        
        //Once the openset has been exhausted its time to complete the pathway
        if(OpenSet.Count <= 0)
        {
            //Get the completed pathway
            List<Node> Pathway = GridManager.Instance.GetCompletedPathway(PathStart, PathEnd);

            //If the path is empty, then no pathway was able to be found between the target nodes
            if(Pathway == null)
            {
                //Print a failure message and reset the grid
                Log.Print("Unable to find a valid pathway using Dijkstras algorithm.");
                FindingPathway = false;
                GridManager.Instance.HideAllParentIndicators();
                return;
            }

            //Announce the pathway has been found
            Log.Print("Dijkstras pathfinding completed after " + OpenSetIterations + " iterations.");

            //Hide all the neighbour indicators
            GridManager.Instance.HideAllParentIndicators();

            //Change the type of all nodes in the pathway to display it in the game
            foreach (Node PathStep in Pathway)
                PathStep.SetType(NodeType.Pathway);

            //Complete the process
            FindingPathway = false;
            return;
        }

        //Find the new current node, then iterate through all its neighbours
        Node Current = GridManager.Instance.FindCheapestNode(OpenSet);
        OpenSet.Remove(Current);
        foreach(Node Neighbour in GridManager.Instance.GetTraversableNeighbours(Current))
        {
            //Ignore nodes not listed in the open set
            if (!OpenSet.Contains(Neighbour))
                continue;

            //check if its cheaper to travel over this neighbour
            float NeighbourCost = Current.FScore + GridManager.FindHeuristic(Current, Neighbour);
            if(NeighbourCost < Neighbour.FScore)
            {
                //update this neighbour as the best way to travel
                Neighbour.FScore = NeighbourCost;
                Neighbour.Parent = Current;
                Neighbour.ToggleParentIndicator(true);
                Neighbour.PointIndicator(Neighbour.GetDirection(Current));
            }
        }
    }
}