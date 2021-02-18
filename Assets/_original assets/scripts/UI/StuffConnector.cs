using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using References;

public static class StuffConnector
{
    public static void LinkConnection(Wire wire)
    {
        if (wire is InputInputConnection) { LinkInputs((InputInputConnection)wire); }
        else if (wire is InputOutputConnection) { LinkInputOutput((InputOutputConnection)wire); }
        else { Debug.Log("o shit, unrecognized wire type!"); }
    }

    public static void LinkConnection(GameObject wire)
    {
        if (wire == null || wire.tag != "Wire") { return; }
        LinkConnection(wire.GetComponent<Wire>());
    }

    // Triggers WireCluster.ConnectInput in the appropriate cases for a connection. Covers possible cases.
    public static void LinkInputs(InputInputConnection connection)
    {
        CircuitInput input1 = connection.Input1;
        CircuitInput input2 = connection.Input2;

        if(input1 == null || input2 == null) { Debug.LogError("This wire doesn't have its inputs set!"); return; }

        if (input1.Cluster == null && input2.Cluster == null) // if neither input has a cluster, create a new one and add them to it
        {
            WireCluster NewCluster = Object.Instantiate(Prefabs.Cluster).GetComponent<WireCluster>();
            NewCluster.ConnectInput(input1);
            NewCluster.ConnectInput(input2);

            NewCluster.transform.parent = ProperClusterParent(NewCluster); // this is also done the next frame when the cluster recalculates its mesh. The reason it is being done here is because when a board is cloned, it gets new clusters, and those need to be children of the board during the same frame they start existing so that autocombineonstable can be set to false for them by StuffPlacer.NewThingBeingPlaced. My hope is that it doesn't affect performance, but I'm pretty nervous about it...
        }

        else if (input1.Cluster == input2.Cluster) // if they're both part of the same cluster already, mesh recalculation needs to happen to incorporate the new wire
        {
            input1.Cluster.QueueMeshRecalculation();
        }

        else if (input1.Cluster != null && input2.Cluster != null && input1.Cluster != input2.Cluster) // if both inputs are part of a cluster, merge those clusters
        {
            JoinClusters(input1.Cluster, input2.Cluster);
        }

        else if (input1.Cluster != null && input2.Cluster == null) // if one input is part of a cluster, connect the other to that cluster
        {
            input1.Cluster.ConnectInput(input2);
        }

        else if (input1.Cluster == null && input2.Cluster != null) // ditto
        {
            input2.Cluster.ConnectInput(input1);
        }

        else
        {
            Debug.Log("no input connection case found");
        }
    }

    // joins an output with an input by sticking the output in the cluster of the input
    public static void LinkInputOutput(InputOutputConnection connection)
    {
        CircuitInput input = connection.Input;
        CircuitOutput output = connection.Output;

        if(input == null || output == null) { Debug.LogError("this wire didn't have its input/output set!"); return; }

        if (input.Cluster != null)
        {
            input.Cluster.ConnectOutput(output);
        }

        else
        {
            WireCluster NewCluster = Object.Instantiate(Prefabs.Cluster).GetComponent<WireCluster>();
            NewCluster.ConnectInput(input);
            NewCluster.ConnectOutput(output);
            NewCluster.transform.parent = ProperClusterParent(NewCluster); // this is also done the next frame when the cluster recalculates its mesh. The reason it is being done here is because when a board is cloned, it gets new clusters, and those need to be children of the board during the same frame they start existing so that autocombineonstable can be set to false for them by StuffPlacer.NewThingBeingPlaced. My hope is that it doesn't affect performance, but I'm pretty nervous about it...
        }
    }

    // merge two clusters
    public static void JoinClusters(WireCluster cluster1, WireCluster cluster2)
    {
        // stick inputs & outputs of cluster2 into cluster1
        foreach (CircuitInput ConnectedInput in cluster2.GetConnectedInputs())
        {
            cluster1.ConnectInput(ConnectedInput);
        }

        foreach (CircuitOutput ConnectedOutput in cluster2.GetConnectedOutputs())
        {
            cluster1.ConnectOutput(ConnectedOutput);
        }

        Object.Destroy(cluster2.gameObject);
    }

    // some functions that deal with the circuitboard parent

    public static void SetAppropriateConnectionParent(Wire wire)
    {
        wire.transform.parent = AppropriateConnectionParent(wire);
    }

    public static void SetAppropriateConnectionParent(GameObject wire)
    {
        SetAppropriateConnectionParent(wire.GetComponent<Wire>());
    }

    // returns the highest-level common board of a connection
    public static Transform AppropriateConnectionParent(Wire connection)
    {
        Transform Board1 = ParentBoard(connection.Peg1);
        Transform Board2 = ParentBoard(connection.Peg2);

        if (Board1 == Board2)
        {
            return Board1;
        }

        if (Board1 == null || Board2 == null)
        {
            return null;
        }

        if (IsChildOf(Board1, Board2))
        {
            return Board2;
        }
        if(IsChildOf(Board2, Board1))
        {
            return Board1;
        }

        return HighestCommonParent(connection.Peg1.transform, connection.Peg2.transform);
    }


    // todo: rethink this...
    public static Transform ProperClusterParent(WireCluster cluster)
    {
        int ShallowestDepthInHeirarchy = 1000000000;
        Transform ProperParent = cluster.transform;
        foreach (CircuitInput input in cluster.GetConnectedInputs())
        {
            int ThisDepthInHeirarchy = DepthInHeirarchy(input.transform);
            if (ShallowestDepthInHeirarchy > ThisDepthInHeirarchy)
            {
                ProperParent = input.transform.parent;
                ShallowestDepthInHeirarchy = ThisDepthInHeirarchy;
            }
        }

        return ProperParent;
    }

    // returned as a transform to save lines of code; this function is only used for determining parentage, so no need to return an actual gameobject
    public static Transform ParentBoard(GameObject boardobject)
    {
        Transform t = boardobject.transform;

        while(t.parent != null)
        {
            if(t.parent.tag == "CircuitBoard")
            {
                return t.parent;
            }
            t = t.parent;
        }
        return null;
    }

    // this really isn't the appropriate class for this function, but currently it's the only one that uses it and I don't have anywhere better...
    public static bool IsChildOf(Transform child, Transform parent)
    {
        if(child == null || parent == null)
        {
            return false;
        }

        while(child.parent != null)
        {
            if(child.parent == parent)
            {
                return true;
            }
            child = child.parent;
        }

        return false;
    }

    public static Transform HighestCommonParent(Transform thing1, Transform thing2)
    {
        while(thing1.parent != null)
        {
            if(IsChildOf(thing2, thing1.parent))
            {
                return thing1.parent;
            }

            thing1 = thing1.parent;
        }

        return null;
    }

    public static int DepthInHeirarchy(Transform thing)
    {
        int i = 0;
        while (thing.parent != null) { thing = thing.parent; i++; }
        return i;
    }

    // if the wire is an IIConnection, queue mesh recalculation in the cluster. If it's IO, queue mesh recalculation in the output.
    // currently only used by stuffrotater for rotating wires
    public static void QueueWireMeshRecalculation(GameObject wire)
    {
        if (wire.tag != "Wire") { return; }

        InputInputConnection IIConnection = wire.GetComponent<InputInputConnection>();
        if(IIConnection != null)
        {
            IIConnection.Input1.Cluster.QueueMeshRecalculation();
            return;
        }

        wire.GetComponent<InputOutputConnection>().Output.QueueMeshRecalculation();
    }
}
