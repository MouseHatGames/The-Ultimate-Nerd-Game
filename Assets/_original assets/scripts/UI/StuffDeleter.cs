// for deleting things

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using References;

public static class StuffDeleter
{
    public static bool AllowedToDoDeleting = true; // for wire placing
    public static void RunGameplayDeleting()
    {
        if (Input.GetButtonDown("Delete"))
        {
            RaycastHit hit;
            if(Physics.Raycast(FirstPersonInteraction.Ray(), out hit, Settings.ReachDistance) && AllowedToDoDeleting)
            {
                DeleteThing(hit.collider.gameObject);
            }
            else
            {
                SoundPlayer.PlaySoundGlobal(Sounds.FailDoSomething);
            }
        }
    }

	public static void DeleteThing(GameObject DestroyThis)
    {
        if (DestroyThis == null) { return; }

        // don't let players destroy the world lol
        if (DestroyThis.tag == "World")
        {
            SoundPlayer.PlaySoundGlobal(Sounds.FailDoSomething);
            return;
        }

        // destroy the full object, not part of it
        DestroyThis = ComponentPlacer.FullComponent(DestroyThis);

        // special cases of stuff to hit
        if(DestroyThis.GetComponent<ObjectInfo>().ComponentType == ComponentType.Mount)
        {
            // code copied from below - don't let players destroy a mount if it has children
            Transform TheBoardBit = DestroyThis.transform.GetChild(1); // the 0th child is the blocky bit
            if(TheBoardBit.childCount > 0)
            {
                if (TheBoardBit.childCount == 1)
                {
                    if (!TheBoardBit.GetChild(0).gameObject == StuffPlacer.GetThingBeingPlaced) { SoundPlayer.PlaySoundGlobal(Sounds.FailDoSomething); return; } // just a check so that you can destroy a board when looking at it with a component ghost
                }
                else
                {
                    SoundPlayer.PlaySoundGlobal(Sounds.FailDoSomething);
                    return;
                }
            }
        }

        if (DestroyThis.tag == "CircuitBoard") // only let players destroy a circuit board if it is empty, to prevent losing lots and lots of work
        {
            if (DestroyThis.transform.childCount > 0)
            {
                if(DestroyThis.transform.childCount == 1)
                {
                    if (!DestroyThis.transform.GetChild(0).gameObject == StuffPlacer.GetThingBeingPlaced) { SoundPlayer.PlaySoundGlobal(Sounds.FailDoSomething); return; } // just a check so that you can destroy a board when looking at it with a component ghost
                }
                else
                {
                    SoundPlayer.PlaySoundGlobal(Sounds.FailDoSomething);
                    return;
                }
            }
            // if no occupied slots are found, continue to destroy the board
        }

        // delink all inputs and outputs in the object if it's not a circuitboard
        else
        {
            CircuitInput[] inputs = DestroyThis.GetComponentsInChildren<CircuitInput>();
            CircuitOutput[] outputs = DestroyThis.GetComponentsInChildren<CircuitOutput>();
            foreach (CircuitInput input in inputs)
            {
                DestroyInput(input);
            }
            foreach (CircuitOutput output in outputs)
            {
                DestroyOutput(output);
            }
        }

        if (DestroyThis.tag == "Wire") // if it's a wire, determine which type of connection it is and destroy that connection
        {
            if (DestroyThis.GetComponent<SnappedConnection>()) // you cannot delete snapped connections
            {
                SoundPlayer.PlaySoundGlobal(Sounds.FailDoSomething);
                return;
            }

            DestroyWire(DestroyThis);
            SoundPlayer.PlaySoundAt(Sounds.DeleteSomething, DestroyThis);
        }
        else if(DestroyThis.tag == "CircuitBoard")
        {
            SoundPlayer.PlaySoundAt(Sounds.DeleteSomething, DestroyThis);
        }
        else
        {
            SoundPlayer.PlaySoundAt(Sounds.DeleteSomething, DestroyThis);
        }

        Object.Destroy(DestroyThis);
    }

    // determines which type of connection a wire is and passes it off to the appropriate destroyer
    public static void DestroyWire(GameObject wire)
    {
        InputInputConnection IIConnection = wire.GetComponent<InputInputConnection>();
        if (IIConnection != null)
        {
            DestroyIIConnection(IIConnection);
            return; // just for that teeny tiny completely insignificant amount of extra performance, stop the function here if it's an IIConnection
        }

        InputOutputConnection IOConnection = wire.GetComponent<InputOutputConnection>();
        if (IOConnection != null)
        {
            DestroyIOConnection(IOConnection);
        }
    }

    public static void DestroyWire(Wire wire)
    {
        if (wire is InputInputConnection) { DestroyIIConnection((InputInputConnection)wire); }
        else { DestroyIOConnection((InputOutputConnection)wire); }
    }

    // destroys a circuitinput
    public static void DestroyInput(CircuitInput input)
    {
        if(input.Cluster != null)
        {
            // get all IIconnections this input was a part of
            InputInputConnection[] DestroyTheseIIs = input.IIConnections.ToArray(); // use arrays to avoid annoying enumeration errors

            // get all IOconnections this input was a part of
            InputOutputConnection[] DestroyTheseIOs = input.IOConnections.ToArray();

            // destroy all the connections found to have the input part of them
            foreach(InputOutputConnection connection in DestroyTheseIOs) // annoying-ass InvalidOperationException prevention. God damn it, why can't lists be like arrays...
            {
                DestroyIOConnection(connection);
            }
            foreach (InputInputConnection connection in DestroyTheseIIs) // annoying-ass InvalidOperationException prevention. God damn it, why can't lists be like arrays...
            {
                DestroyIIConnection(connection);
            }
        }

        // destroy the physical object now that all the behind the scenes circuitry stuff is out of the way
        Object.Destroy(input.gameObject);
    }

    // destroys an output
    public static void DestroyOutput(CircuitOutput output)
    {
        InputOutputConnection[] DestroyTheseIOs = output.GetIOConnections().ToArray();
        foreach(InputOutputConnection connection in DestroyTheseIOs)
        {
            DestroyIOConnection(connection);
        }

        // destroy the physical object now that all the behind the scenes circuitry stuff is out of the way
        Object.Destroy(output.gameObject);
    }

    // destroys an input-input connection
    public static void DestroyIIConnection(InputInputConnection connection)
    {
        connection.Input1.IIConnections.Remove(connection);
        connection.Input2.IIConnections.Remove(connection);
        RecalculateCluster(connection.Input1.Cluster); // recalculate the clusters, since they might (probably, in fact) need changing now
        Object.Destroy(connection.gameObject); // destroy the physical connection
    }

    // destroys an input-output connection
    public static void DestroyIOConnection(InputOutputConnection connection)
    {
        connection.Input.IOConnections.Remove(connection);
        connection.Output.RemoveIOConnection(connection);
        RecalculateCluster(connection.Input.Cluster); // remove the output from the cluster, and destroy the cluster in a lot of cases - whatever, who cares, some other shitty function will deal with it
        Object.Destroy(connection.gameObject); // destroy the physical connection
    }

    // recalculates the clusters for each input and output in the cluster, based on all the connections in the cluster
    public static void RecalculateCluster(WireCluster cluster)
    {
        if (cluster == null) { return; }

        RecalculateClustersFromInputs(cluster.GetConnectedInputs());

        Object.Destroy(cluster.gameObject);
    }

    public static void RecalculateClustersFromInputs(CircuitInput[] inputs)
    {
        HashSet<InputInputConnection> IIConnections = new HashSet<InputInputConnection>();
        HashSet<InputOutputConnection> IOConnections = new HashSet<InputOutputConnection>();

        foreach (CircuitInput input in inputs) // the connections of the inputs of the cluster are the same as all the connections in the cluster
        {
            if (input.Cluster != null) { input.Cluster.RemoveInput(input); } // a lot of important stuff happens here!

            foreach (InputInputConnection connection in input.IIConnections)
            {
                connection.Renderer.enabled = true;

                IIConnections.Add(connection);
            }
            foreach (InputOutputConnection connection in input.IOConnections)
            {
                IOConnections.Add(connection);
            }
        }

        foreach (InputInputConnection connection in IIConnections)
        {
            StuffConnector.LinkInputs(connection); // LinkInputs is used because we already have the physical object, we just need the connection codeside
        }
        foreach (InputOutputConnection connection in IOConnections)
        {
            StuffConnector.LinkInputOutput(connection);
        }
    }
}
