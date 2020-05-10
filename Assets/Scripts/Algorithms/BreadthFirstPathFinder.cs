// ================================================================================================================================
// File:        BreadthFirstPathFinder.cs
// Description:	Uses breadth first searching to find a pathway between the two nodes
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using System.Collections.Generic;

public class BreadthFirstPathFinder : PathFinder
{
    public override void FindPath(Node Start, Node End)
    {
        //Initialize everything
        Log.Print("Starting breadth first pathfind...");
        OpenSetIterations = 0;
        PathStart = Start;
        PathEnd = End;
        GridManager.Instance.ResetAllPathValues();
        FindingPathway = true;
        OpenSet = new List<Node>();

        //Push the start pos into the open set
        OpenSet.Add(PathStart);
        PathStart.Opened = true;
    }

    public override void IterateOpenSet()
    {
        OpenSetIterations++;

        //Path doesnt exist if the openset runs out
        if(OpenSet.Count <= 0)
        {
            Log.Print("no path found");
            GridManager.Instance.HideAllParentIndicators();
            FindingPathway = false;
            return;
        }

        //Take the front node from the open set
        Node Current = OpenSet[0];
        OpenSet.Remove(Current);
        Current.Closed = true;

        //Pathway is completed once the end node has been reached
        if(Current == PathEnd)
        {
            Log.Print("Pathway found after " + OpenSetIterations + " iterations.");
            FindingPathway = false;
            GridManager.Instance.HideAllParentIndicators();
            foreach (Node Step in GridManager.Instance.GetCompletedPathway(PathStart, PathEnd))
                Step.SetType(NodeType.Pathway);
            return;
        }

        //Process all the nodes neighbours
        foreach(Node Neighbour in GridManager.Instance.GetTraversableNeighbours(Current))
        {
            //Skip neighbours already searched
            if (Neighbour.Closed || Neighbour.Opened)
                continue;

            //Move this neighbour onto the open list, open it and update its parent
            OpenSet.Add(Neighbour);
            Neighbour.Opened = true;
            Neighbour.Parent = Current;
            Neighbour.ToggleParentIndicator(true);
            Neighbour.PointIndicator(Neighbour.GetDirection(Current));
        }
    }
}
