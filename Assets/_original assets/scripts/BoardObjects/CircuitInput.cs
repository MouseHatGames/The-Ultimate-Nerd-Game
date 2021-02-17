// note to self: never, ever name your class with a class already used by Unity. ffs.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircuitInput : MonoBehaviour {

    public bool On;

    public WireCluster Cluster;

    public Renderer Renderer;

    public MeshFilter Mesh;

    public Transform WireReference; // for referencing the point wires should attach to on the input

    // all connections this input is part of
    public List<InputOutputConnection> IOConnections;
    public List<InputInputConnection> IIConnections;

    private void Awake()
    {
        Mesh = GetComponent<MeshFilter>();
        Renderer = GetComponent<Renderer>();
        Renderer.material.color = MiscellaneousSettings.CircuitOffColor; // so that when not connected to a cluster, it's off

        WireReference = transform.GetChild(0);

        BehaviorManager.UpdatedInputs.Add(this);
    }

    private void OnDestroy()
    {
        BehaviorManager.UpdatedInputs.Remove(this);
    }

    public void CircuitLogicUpdate()
    {
        if (Cluster != null)
        {
            On = Cluster.On;
        }
        else
        {
            On = false;
        }
    }
}