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
        Log.Print("Starting Dijkstra's path find...");
        OpenSetIterations = 0;
        PathStart = Start;
        PathEnd = End;
        FindingPathway = true;
        GridManager.Instance.ResetAllPathValues();
        OpenSet = new List<Node>();
        foreach (Node Node in GridManager.Instance.GetCompleteNodeList())
            OpenSet.Add(Node);
        PathStart.FScore = 0;
    }

    public override void IterateOpenSet()
    {
        OpenSetIterations++;
        
        if(OpenSet.Count <= 0)
        {
            List<Node> Pathway = GridManager.Instance.GetCompletedPathway(PathStart, PathEnd);

            if(Pathway == null)
            {
                Log.Print("Unable to find a valid pathway using Dijkstras algorithm.");
                FindingPathway = false;
                GridManager.Instance.HideAllParentIndicators();
                return;
            }

            Log.Print("Dijkstras pathfinding completed after " + OpenSetIterations + " iterations.");
            FindingPathway = false;
            GridManager.Instance.HideAllParentIndicators();
            foreach (Node PathStep in Pathway)
                PathStep.SetType(NodeType.Pathway);
            return;
        }

        Node Current = GridManager.Instance.FindCheapestNode(OpenSet);
        OpenSet.Remove(Current);
        foreach(Node Neighbour in GridManager.Instance.GetTraversableNeighbours(Current))
        {
            if (!OpenSet.Contains(Neighbour))
                continue;
            float NeighbourCost = Current.FScore + GridManager.FindHeuristic(Current, Neighbour);
            if(NeighbourCost < Neighbour.FScore)
            {
                Neighbour.FScore = NeighbourCost;
                Neighbour.Parent = Current;
                Neighbour.ToggleParentIndicator(true);
                Neighbour.PointIndicator(Neighbour.GetDirection(Current));
            }
        }
    }
}