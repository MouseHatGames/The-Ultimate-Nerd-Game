// for deleting things

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StuffDeleter : MonoBehaviour {

	public static void DeleteThing(GameObject DestroyThis)
    {
        // don't let players destroy the world lol
        if (DestroyThis.tag == "World")
        {
            return;
        }

        // destroy the full object, not part of it
        // the last check is so that destroying child boards doesn't destroy their parent
        // TODO: change to a while loop
        if (DestroyThis.transform.parent != null && DestroyThis.transform.parent.tag != "CircuitBoard" && DestroyThis.tag != "CircuitBoard")
        {
            DeleteThing(DestroyThis.transform.parent.gameObject);
            return;
        }

        // delink all inputs and outputs in the object - unless, of course, it's a circuitboard
        if (DestroyThis.transform.tag != "CircuitBoard")
        {
            CircuitInput[] inputs = DestroyThis.GetComponentsInChildren<CircuitInput>();
            Output[] outputs = DestroyThis.GetComponentsInChildren<Output>();
            foreach (CircuitInput input in inputs)
            {
                DestroyInput(input);
            }
            foreach (Output output in outputs)
            {
                DestroyOutput(output);
            }
        }

        // special cases of stuff to hit
        if (DestroyThis.tag == "CircuitBoard") // only let players destroy a circuit board if it is empty, to prevent losing lots and lots of work
        {
            if(DestroyThis.transform.childCount > 0)
            {
                return; // stop the function if the board has anything on it (i. e. boardobjects, wires)
            }

            MegaBoardMeshManager.RemoveBoardsFrom(DestroyThis);
            Destroy(DestroyThis); // if no occupied slots are found, destroy the board
        }

        else if(DestroyThis.tag == "Wire") // if it's a wire, determine which type of connection it is and destroy that connection
        {
            DestroyWire(DestroyThis);
        }

        else // if it's not one of the special cases just destroy it
        {
            Destroy(DestroyThis);
        }

        MegaMesh.RemoveMeshesFrom(DestroyThis);
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
        Destroy(input.gameObject);
    }

    // destroys an output
    public static void DestroyOutput(Output output)
    {
        InputOutputConnection[] DestroyTheseIOs = output.GetIOConnections().ToArray();
        foreach(InputOutputConnection connection in DestroyTheseIOs)
        {
            DestroyIOConnection(connection);
        }

        // destroy the physical object now that all the behind the scenes circuitry stuff is out of the way
        Destroy(output.gameObject);
    }

    // destroys an input-input connection
    public static void DestroyIIConnection(InputInputConnection connection)
    {
        connection.Point1.IIConnections.Remove(connection);
        connection.Point2.IIConnections.Remove(connection);
        RecalculateCluster(connection.Point1.Cluster, true); // recalculate the clusters, since they might (probably, in fact) need changing now
        Destroy(connection.gameObject); // destroy the physical connection
    }

    // destroys an input-output connection
    public static void DestroyIOConnection(InputOutputConnection connection)
    {
        connection.Point1.IOConnections.Remove(connection);
        connection.Point2.RemoveIOConnection(connection);
        RecalculateCluster(connection.Point1.Cluster, true); // remove the output from the cluster, and destroy the cluster in a lot of cases - whatever, who cares, some other shitty function will deal with it
        Destroy(connection.gameObject); // destroy the physical connection
    }

    // recalculates the clusters for each input and output in the cluster, based on all the connections in the cluster
    public static void RecalculateCluster(WireCluster cluster, bool CheckInputRendererState = false)
    {
        if (cluster == null) { return; }
        // grab the connections from the cluster before we FUCKING MURDER IT
        List<InputInputConnection> IIConnections = new List<InputInputConnection>(); // declare the new lists
        List<InputOutputConnection> IOConnections = new List<InputOutputConnection>();
        CircuitInput[] inputs = cluster.GetConnectedInputs();
        foreach (CircuitInput input in inputs) // the connections of the inputs of the cluster are the same as all the connections in the cluster
        {
            input.Renderer.enabled = true;

            foreach(InputInputConnection connection in input.IIConnections)
            {
                connection.Renderer.enabled = true;

                if (!IIConnections.Contains(connection)) // each IIconnection will appear twice, once in each of its inputs. This is to prevent duplicates
                {
                    IIConnections.Add(connection);
                }
            }
            foreach(InputOutputConnection connection in input.IOConnections)
            {
                if (!IOConnections.Contains(connection)) // probably not necessary but #YOLO
                {
                    IOConnections.Add(connection);
                }
            }
        }

        // assign every connected input and output to have no cluster so that StuffConnecter works properly
        foreach(CircuitInput input in inputs)
        {
            input.Cluster = null;
        }

        // FUCKING MURDER IT
        Destroy(cluster.gameObject);

        foreach (InputInputConnection connection in IIConnections)
        {
            StuffConnecter.LinkInputs(connection); // LinkInputs is used because we already have the physical object, we just need the connection codeside
        }
        foreach (InputOutputConnection connection in IOConnections)
        {
            StuffConnecter.LinkInputOutput(connection);
        }

        // this fixes a bug where if you hooked up an inverter to itself via an input and then destroyed the
        // inverter, the input would become invisible. God damn this code has become so MESSY and SHITTY and I can't wait to refractor it all!!!
        // the reason it's only done in specific circumstances and not all the time is because it breaks the main menu for some reason
        if (CheckInputRendererState)
        {
            BoardPlacer.Instance.ValidateInputRendererStateAndStuff(inputs);
        }
    }
}
