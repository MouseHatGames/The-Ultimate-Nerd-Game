using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LookThroughBoard {

    public static GameObject BoardBeingLookedThrough = null;

    public static void Run()
    {
        if (Input.GetButtonDown("LookThroughBoard"))
        {
            RaycastHit hit;
            if(Physics.Raycast(FirstPersonInteraction.Ray(), out hit, Settings.ReachDistance, Wire.IgnoreWiresLayermask))
            {
                Initial(hit);
            }
        }
        else if (Input.GetButtonUp("LookThroughBoard"))
        {
            Final();
        }
    }

    public static void Initial(RaycastHit hit)
    {
        if (hit.collider.tag != "CircuitBoard") { return; }

        BoardBeingLookedThrough = hit.collider.gameObject;

        MakeTransparent(BoardBeingLookedThrough.GetComponent<Renderer>(), 0.4f);
        MegaMeshManager.RemoveComponentImmediatelyOf(BoardBeingLookedThrough);
    }

    public static void Final()
    {
        if (BoardBeingLookedThrough == null) { return; } // to avoid nullreferenceexceptions. This method will still be called if f is tapped while not looking at a board

        MakeOpaque(BoardBeingLookedThrough.GetComponent<Renderer>());
        MegaMeshManager.AddComponentOf(BoardBeingLookedThrough);

        BoardBeingLookedThrough = null;
    }

    public static void MakeTransparent(Renderer renderer, float transparency)
    {
        renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, transparency);
        StandardShaderUtils.ChangeRenderMode(renderer.material, StandardShaderUtils.BlendMode.Transparent);
    }

    public static void MakeOpaque(Renderer renderer)
    {
        renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, 1f);
        StandardShaderUtils.ChangeRenderMode(renderer.material, StandardShaderUtils.BlendMode.Opaque);
    }

    public static void MakeTransparent(Renderer[] renderers, float transparency)
    {
        foreach(Renderer renderer in renderers)
        {
            MakeTransparent(renderer, transparency);
        }
    }

    public static void MakeOpaque(Renderer[] renderers)
    {
        foreach (Renderer renderer in renderers)
        {
            MakeOpaque(renderer);
        }
    }

    // gets the renderers of the board and all its children on a path that does NOT contain a circuit board
    // decided not to use this, at least for now
    //public static Renderer[] GetAppropriateRenderersFrom(GameObject board)
    //{
    //    List<Renderer> renderers = new List<Renderer>();
    //    renderers.Add(board.GetComponent<Renderer>());

    //    for(int i = 0; i < board.transform.childCount; i++)
    //    {
    //        Transform child = board.transform.GetChild(i);
    //        if(child.tag != "CircuitBoard" && child != StuffPlacer.GetThingBeingPlaced.transform)
    //        {
    //            renderers.AddRange(child.GetComponentsInChildren<Renderer>());
    //            if(child.tag == "Input")
    //            {
    //                WireCluster cluster = child.GetComponent<CircuitInput>().Cluster;
    //                if(cluster != null) { renderers.Add(cluster.GetRenderer); }
    //            }
    //        }
    //    }

    //    return renderers.ToArray();
    //}
}