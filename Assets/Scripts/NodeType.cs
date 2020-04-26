// ================================================================================================================================
// File:        NodeType.cs
// Description:	Defines types of nodes that can exist
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

public enum NodeType
{
    Open = 0,   //Open nodes can be travelled across freely
    Wall = 1,    //Wall nodes cannot be travelled across
    Start = 2,  //Start of the pathway about to be calculated
    End = 3, //End of the pathway about to be calculated
    Pathway = 4    //Pathway nodes show the route found from pathfinding
}
