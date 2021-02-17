// for rotating stuff. why is this comment even necessary

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StuffRotater : MonoBehaviour {

    public static void RotateThing(GameObject RotateThis)
    {
        // determines which direction to rotate
        // TODO: use camera.main.transform to rotate stuff left and right rather than clockwise & ccw

        // rotate the full object, not part of it
        // the last check is so that rotating child boards doesn't rotate their parent
        if (RotateThis.transform.parent != null && RotateThis.transform.parent.tag != "CircuitBoard" && RotateThis.tag != "CircuitBoard")
        {
            RotateThing(RotateThis.transform.parent.gameObject);
            return;
        }

        if (RotateThis.tag == "Wire" || RotateThis.tag == "CircuitBoard") // you cannot rotate wires, and circuitboards are rotated with a different script in a special way
        {
            return;
        }

        int direction = 1;
        if(Input.GetAxis("Rotate") < 0)
        {
            direction = -1;
        }
        else
        {
            direction = 1;
        }

        if (Input.GetButton("Mod")) // rotate by 22.5 degrees if the mod key is held down
        {
            RotateThis.transform.RotateAround(RotateThis.transform.position, RotateThis.transform.up, direction * 22.5f);
        }
        else // ...but normally rotate by 90 degrees
        {
            RotateThis.transform.RotateAround(RotateThis.transform.position, RotateThis.transform.up, direction * 90f);
        }

        DealWithConnectedWires(RotateThis);
        StuffPlacer.DestroyIntersectingConnections(RotateThis);

        QueueClusterMeshRecalculationOn(RotateThis);

        // since the thing we're rotating in all likelihood is part of the MegaMesh, we recalculate that mesh
        MegaMesh.RecalculateMegaMesh();
    }

    // takes an object and checks all its connections and redraws those wires
    // this function should probably be in a different class... not sure where to put it though...
    // this function has a lot of repeated code. Could be tidied up a bit. TODO: do that
    public static void DealWithConnectedWires(GameObject RotatedObject)
    {
        CircuitInput[] inputs = RotatedObject.GetComponentsInChildren<CircuitInput>();
        Output[] outputs = RotatedObject.GetComponentsInChildren<Output>();

        foreach (CircuitInput input in inputs)
        {
            foreach (InputOutputConnection connection in input.IOConnections)
            {
                connection.DrawWire();
                connection.Point2.QueueMeshRecalculation();
            }
            foreach(InputInputConnection connection in input.IIConnections)
            {
                connection.DrawWire();
            }
        }

        foreach (Output output in outputs)
        {
            foreach (InputOutputConnection connection in output.GetIOConnections())
            {
                connection.DrawWire();
            }

            output.QueueMeshRecalculation();
        }

        BoardPlacer.Instance.ValidateInputRendererStateAndStuff(inputs); // in rare cases the inputs become invisible. Fixes that
    }

    // quick shitty fix for a bug
    public static void QueueClusterMeshRecalculationOn(GameObject go)
    {
        CircuitInput[] inputs = go.GetComponentsInChildren<CircuitInput>();
        foreach(CircuitInput input in inputs)
        {
            if(input.Cluster != null)
            {
                input.Cluster.QueueMeshRecalculation();
            }
        }
    }
}