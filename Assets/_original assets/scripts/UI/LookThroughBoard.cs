using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookThroughBoard {

    public static CircuitBoard BoardBeingLookedThrough = null;

    public static void Initial(RaycastHit hit)
    {
        if (hit.collider.tag != "CircuitBoard") { return; }

        BoardBeingLookedThrough = hit.collider.gameObject.GetComponent<CircuitBoard>();

        // what a fucking mess. TODO clean this up
        BoardBeingLookedThrough.Renderer.material.color = new Color(BoardBeingLookedThrough.Renderer.material.color.r, BoardBeingLookedThrough.Renderer.material.color.g, BoardBeingLookedThrough.Renderer.material.color.b, 0.6f);
        StandardShaderUtils.ChangeRenderMode(BoardBeingLookedThrough.Renderer.material, StandardShaderUtils.BlendMode.Transparent);

        MegaBoardMeshManager.RemoveBoard(hit.collider.GetComponent<CircuitBoard>());
    }

    public static void Final()
    {
        if (BoardBeingLookedThrough != null) // to avoid nullreferenceexceptions. This method will be called if f is tapped while not looking at a board
        {
            BoardBeingLookedThrough.Renderer.material.color = new Color(BoardBeingLookedThrough.Renderer.material.color.r, BoardBeingLookedThrough.Renderer.material.color.g, BoardBeingLookedThrough.Renderer.material.color.b, 1f);
            StandardShaderUtils.ChangeRenderMode(BoardBeingLookedThrough.Renderer.material, StandardShaderUtils.BlendMode.Opaque);

            MegaBoardMeshManager.AddBoard(BoardBeingLookedThrough);
        }

        BoardBeingLookedThrough = null;
    }
}
