// ================================================================================================================================
// File:        NodeInteraction.cs
// Description:	Allows the user to interact with the node grid, changing the details of nodes
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class NodeInteraction : MonoBehaviour
{
    private void Update()
    {
        bool CtrlHeld = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

        //Left clicking nodes turns them into pathway nodes
        if(!CtrlHeld && Input.GetMouseButton(0))
        {
            Node Node = GetHoveredNode();
            if (Node != null)
                Node.SetType(NodeType.Open);
        }
        //Ctrl+Left clicking nodes turns them into pathway start nodes
        else if(CtrlHeld && Input.GetMouseButtonDown(0))
        {
            Node Node = GetHoveredNode();
            if (Node != null)
            {
                Node.SetType(NodeType.Start);
                GridManager.Instance.PathStart = Node;
            }
        }

        //Right clicking nodes turns them into wall nodes
        if(!CtrlHeld && Input.GetMouseButton(1))
        {
            Node Node = GetHoveredNode();
            if (Node != null)
                Node.SetType(NodeType.Wall);
        }
        //Ctrl+Right clicking nodes turns them into pathway end nodes
        else if(CtrlHeld && Input.GetMouseButtonDown(1))
        {
            Node Node = GetHoveredNode();
            if (Node != null)
                Node.SetType(NodeType.End);
        }
    }

    private Node GetHoveredNode()
    {
        RaycastHit RayHit;
        Ray CameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(CameraRay, out RayHit, Mathf.Infinity))
            if (RayHit.transform.CompareTag("Node"))
                return RayHit.transform.GetComponent<Node>();

        return null;
    }
}
