// note to self: never, ever name your class with a class already used by Unity. ffs.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircuitInput : MonoBehaviour
{
    public bool On; // the very first line of code written for TUNG 😭

    public WireCluster Cluster = null;

    public Renderer Renderer;
    public MeshFilter MeshFilter;

    public CircuitLogicComponent CircuitLogicComponent = null; // set in editor, assigned for prefabs

    // all connections this input is part of
    public List<InputOutputConnection> IOConnections;
    public List<InputInputConnection> IIConnections;

    public bool IsSnappingPeg = false; // boolean comparisons are faster than type checking

    private void Awake()
    {
        MeshFilter = GetComponent<MeshFilter>();
        Renderer = GetComponent<Renderer>();
        Renderer.material.color = Settings.CircuitOffColor; // so that when not connected to a cluster and part of its mesh, it's off
    }
}