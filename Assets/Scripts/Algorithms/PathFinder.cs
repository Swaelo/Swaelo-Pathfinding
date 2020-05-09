// ================================================================================================================================
// File:        PathFinder.cs
// Description:	Base class which all pathfinding algorithm classes derive from
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using System.Collections.Generic;
using UnityEngine;

public abstract class PathFinder : MonoBehaviour
{
    //Nodes to track during pathfinding
    public Node PathStart; //The node where the path begins from
    public Node PathEnd;   //The node where the pathway ends
    public List<Node> OpenSet; //Set of nodes the algorithm is currently iterating over
    public List<Node> ClosedSet; //Set of nodes finished iterating over

    //Pathfinding visualization
    public int OpenSetIterations = 0;  //Counts how many times this algorithm has iterated over the open set during the current pathfinding process
    public bool FindingPathway = false;    //Tracks when the pathfinding process is currently active or not
    private float IterationInterval = 0.005f;   //How often an algorithm iterates over the nodes during pathfinding
    private float NextIteration = 0.005f;   //Seconds left until its time for the algorithm to iterate over its nodes again

    private void Update()
    {
        //Update the iteration timer and perform an iteration over the OpenSet everytime it expires
        if(FindingPathway)
        {
            NextIteration -= Time.deltaTime;
            if(NextIteration <= 0.0f)
            {
                NextIteration = IterationInterval;
                IterateOpenSet();
            }
        }
    }

    //Begins the pathfinding process
    public abstract void FindPath(Node Start, Node End);

    //Performs 1 iteration over all the nodes in the open set
    public abstract void IterateOpenSet();
}
