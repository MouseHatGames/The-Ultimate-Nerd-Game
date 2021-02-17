using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StuffConnecter : MonoBehaviour {

    // the currently selected peg
    public static GameObject SelectedPeg;

    // a wire prefab to instantiate when a connection is created
    public static GameObject WirePrefab;

    // summoned when creating clusters
    public static GameObject ClusterPrefab;

    // triggers when the connection button is pressed
    // selects the peg being looked at
    public static void ConnectionInitial(RaycastHit hit)
    {
        if(hit.collider.tag == "Input" || hit.collider.tag == "Output") // if it's an input or output...
        {
            SelectedPeg = hit.collider.gameObject; // ..make it the selected peg...

            cakeslice.Outline outline = SelectedPeg.GetComponent<cakeslice.Outline>();
            outline.eraseRenderer = false; // ...and apply the outline effect
            outline.enabled = true; // the script is toggled on and off to reduce garbage collection. The third party asset for outlines has a LOT of GC -_-
        }
        else
        {
            ConnectionFinal();
        }
    }

    // triggers when the connection button is let up
    // if it's looking at a peg, attempt a connection to the selectedpeg via raycast
    // deselect the peg
    public static void ConnectionFinal(RaycastHit hit)
    {
        if((hit.collider.tag == "Input" || hit.collider.tag == "Output") && hit.collider.gameObject != SelectedPeg && SelectedPeg != null) // if the looked-at thing is a peg AND it's not the same peg we just looked at AND there actually was a peg that was just looked at
        {
            // only continue if there does not exist a connection between the two already and a connection can exist
            if (!ConnectionExists(SelectedPeg, hit.collider.gameObject) && CanConnect(SelectedPeg, hit.collider.gameObject))
            {
                if (hit.collider.tag == "Input" && SelectedPeg.tag == "Input")
                {
                    CreateIIConnection(SelectedPeg.GetComponent<CircuitInput>(), hit.collider.GetComponent<CircuitInput>());
                }

                else if (hit.collider.tag == "Input" && SelectedPeg.tag == "Output")
                {
                    CreateIOConnection(hit.collider.GetComponent<CircuitInput>(), SelectedPeg.GetComponent<Output>());
                }

                else if (hit.collider.tag == "Output" && SelectedPeg.tag == "Input")
                {
                    CreateIOConnection(SelectedPeg.GetComponent<CircuitInput>(), hit.collider.GetComponent<Output>());
                }

                else if(hit.collider.tag == "Output" && SelectedPeg.tag == "Output")
                {
                    // do nothing
                }

                else
                {
                    Debug.Log("no connection case found, though there should have been");
                }
            }
        }

        ConnectionFinal();
    }

    // non-overload method, so we can deselect stuff even if the raycast doesn't hit
    public static void ConnectionFinal()
    {
        // remove SelectedPeg's outline and set it to null
        if (SelectedPeg != null)
        {
            cakeslice.Outline outline = SelectedPeg.GetComponent<cakeslice.Outline>();
            outline.eraseRenderer = true;
            outline.enabled = false;
        }
        SelectedPeg = null;
    }

    // check if a connection already exists between the two pegs
    public static bool ConnectionExists(GameObject thing1, GameObject thing2)
    {
        // find the types that each thing is by trying to access its CircuitInput or Output
        CircuitInput thing1input = thing1.GetComponent<CircuitInput>();
        Output thing1output = thing1.GetComponent<Output>();
        CircuitInput thing2input = thing2.GetComponent<CircuitInput>();
        Output thing2output = thing2.GetComponent<Output>();

        if (thing1input != null && thing2input != null) // if this would be an InputInputConnection
        {
            foreach(InputInputConnection connection in thing1input.IIConnections)
            {
                // return true if any of the point's connections have the two things as points. Probably there's a terser way to do this but sadly I don't know that particular nuance of C#.
                if(connection.Point1 == thing1input && connection.Point2 == thing2input)
                {
                    return true;
                }
                if (connection.Point1 == thing2input && connection.Point2 == thing1input)
                {
                    return true;
                }
            }
        }

        else if(thing1input != null && thing2output != null) // if this would be an InputOutputConnection and thing1 is the input
        {
            foreach(InputOutputConnection connection in thing1input.IOConnections)
            {
                if(connection.Point1 == thing1input && connection.Point2 == thing2output)
                {
                    return true;
                }
            }
        }

        else if (thing1output != null && thing2input != null) // if this would be an InputOutputConnection and thing2 is the input
        {
            foreach (InputOutputConnection connection in thing2input.IOConnections)
            {
                if (connection.Point1 == thing2input && connection.Point2 == thing1output)
                {
                    return true;
                }
            }
        }

        // if none of the true cases were found, return false
        return false;
    }

    // determines, via raycast, if two pegs can connect
    public static bool CanConnect(GameObject peg1, GameObject peg2)
    {
        // the first child of each is the WireReference object, where the wire should come from and go to
        Vector3 WirePoint1 = peg1.transform.GetChild(0).transform.position;
        Vector3 WirePoint2 = peg2.transform.GetChild(0).transform.position;

        int layerMask = 1 << 0; // cast against only the default layer

        RaycastHit hit;
        if (Physics.Raycast(WirePoint1, WirePoint2 - WirePoint1, out hit, MiscellaneousSettings.WireDistance, layerMask)) // raycast from wirepoint1, in the direction of wirepoint2, for WireDistance, ignoring wires
        {
            if(hit.collider.gameObject == peg2) // if the raycast meets no obstacle on the way from peg1 to peg2, return true. All other paths return false.
            {
                return true;
            }
            return false;
        }
        return false;
    }

    // overload method that uses the connection object rather than the individual pegs. It is the only version that respects the unbreakable tag
    public static bool CanConnect(GameObject wire)
    {
        if(wire.tag != "Wire")
        {
            return false; // I'd rather this return null but bools can't be null :(
        }

        // get the objects for each point of the wire
        GameObject Point1 = null;
        GameObject Point2 = null;

        InputInputConnection IIConnection = wire.GetComponent<InputInputConnection>();
        if (IIConnection != null)
        {
            Point1 = IIConnection.Point1.gameObject;
            Point2 = IIConnection.Point2.gameObject;

            if (IIConnection.unbreakable)
            {
                return true;
            }
        }
        InputOutputConnection IOConnection = wire.GetComponent<InputOutputConnection>();
        if (IOConnection != null)
        {
            Point1 = IOConnection.Point1.gameObject;
            Point2 = IOConnection.Point2.gameObject;

            if (IOConnection.unbreakable)
            {
                return true;
            }
        }

        if(CanConnect(Point1, Point2))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // TODO: add overload methods for the connection classes

    // creates a physical connection between two inputs, then triggers the non-physical connection
    public static void CreateIIConnection(CircuitInput input1, CircuitInput input2)
    {
        GameObject wire = Instantiate(WirePrefab);
        wire.AddComponent<SaveThisObject>().ObjectType = "Wire";
        InputInputConnection connection = wire.AddComponent<InputInputConnection>();
        connection.SetPoint1(input1);
        connection.SetPoint2(input2);
        connection.DrawWire();

        wire.transform.parent = AppropriateConnectionParent(connection);
        LinkInputs(connection);
    }

    // creates a physical connection between an input and an output, then triggers the non-physical connection
    public static void CreateIOConnection(CircuitInput input, Output output)
    {
        GameObject wire = Instantiate(WirePrefab);
        wire.AddComponent<SaveThisObject>().ObjectType = "Wire";
        InputOutputConnection connection = wire.AddComponent<InputOutputConnection>();
        connection.SetInput(input);
        connection.SetOutput(output);
        connection.DrawWire();
        output.QueueMeshRecalculation();

        wire.transform.parent = AppropriateConnectionParent(connection);
        LinkInputOutput(connection);
    }

    // joins two inputs in the cluster code. Covers possible cases.
    public static void LinkInputs(InputInputConnection connection)
    {
        CircuitInput input1 = connection.Point1;
        CircuitInput input2 = connection.Point2;

        if (input1.Cluster == null && input2.Cluster == null) // if neither input has a cluster, create a new one and add them to it
        {
            GameObject CurrentlyManagedCluster = Instantiate(ClusterPrefab); // make the cluster's position nearby what it's controlling. This is for the future when we load things based on their distance from the player

            WireCluster ClusterData = CurrentlyManagedCluster.GetComponent<WireCluster>();
            ClusterData.ConnectInput(input1);
            ClusterData.ConnectInput(input2);
        }

        else if (input1.Cluster == input2.Cluster) // if they're both part of the same cluster already, stop the function, everything is good
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
            // CreateIIConnection(input2, input1); // note that the order is reversed here. Fuck you, future jimmy who doesn't understand the code, I'm too lazy to comment why.
            // oh shit. Now I don't know why this was there. I'm removing it and if there are problems I'll analyze it again...
            input2.Cluster.ConnectInput(input1);
        }

        else
        {
            Debug.Log("ClusterManager: no input connection case found");
        }
    }

    // joins an output with an input by sticking the output in the cluster of the input
    public static void LinkInputOutput(InputOutputConnection connection)
    {
        CircuitInput input = connection.Point1;
        Output output = connection.Point2;

        if (input.Cluster != null)
        {
            input.Cluster.ConnectOutput(output);
        }

        else
        {
            GameObject CurrentlyManagedCluster = Instantiate(ClusterPrefab);

            WireCluster ClusterData = CurrentlyManagedCluster.GetComponent<WireCluster>();
            ClusterData.ConnectInput(input);
            ClusterData.ConnectOutput(output);
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

        foreach (Output ConnectedOutput in cluster2.GetConnectedOutputs())
        {
            cluster1.ConnectOutput(ConnectedOutput);
        }

        Destroy(cluster2.gameObject);
    }

    // some functions that deal with the circuitboard parent

    // returns the highest-level common board of a connection
    public static Transform AppropriateConnectionParent(InputInputConnection connection)
    {
        Transform Board1 = ParentBoard(connection.Point1.gameObject);
        Transform Board2 = ParentBoard(connection.Point2.gameObject);

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

        return HighestCommonParent(connection.Point1.transform, connection.Point2.transform);
    }

    // overload for the other connection type
    public static Transform AppropriateConnectionParent(InputOutputConnection connection)
    {
        Transform Board1 = ParentBoard(connection.Point1.gameObject);
        Transform Board2 = ParentBoard(connection.Point2.gameObject);

        if(Board1 == Board2)
        {
            return Board1;
        }

        if (IsChildOf(Board1, Board2))
        {
            return Board2;
        }
        if (IsChildOf(Board2, Board1))
        {
            return Board1;
        }

        return HighestCommonParent(connection.Point1.transform, connection.Point2.transform);
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
}
