// ================================================================================================================================
// File:        NodeMaterial.cs
// Description:	Defines a single material for the grid nodes
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

[System.Serializable]
public struct NodeMaterial
{
    public string Name; //The name of the material
    public Material Material;   //The mesh material
}