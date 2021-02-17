using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class InputOutputConnection : MonoBehaviour {

    public CircuitInput Point1;
    public Output Point2;

    public void SetInput(CircuitInput input)
    {
        Point1 = input;
        input.IOConnections.Add(this);
    }
    public void SetOutput(Output output)
    {
        Point2 = output;
        output.AddIOConnection(this);
    }

    public Renderer Renderer;
    public MeshFilter Mesh;

    private void Awake()
    {
        Mesh = GetComponent<MeshFilter>();
        Renderer = GetComponent<Renderer>();

        Renderer.material.color = MiscellaneousSettings.CircuitOffColor;
    }

    public bool unbreakable; // currently only used by through pegs


    // sets the transforms of the gameobject, and by extension the plane, to look like it's a wire
    public void DrawWire()
    {
        transform.position = (Point1.WireReference.position + Point2.WireReference.position) / 2; // set the position to the center between the two vectors
        transform.LookAt(Point1.WireReference.position, Point1.transform.up); // set the rotation to point from one pin to the other
        transform.localScale = new Vector3(0.05f, 1, Vector3.Distance(Point1.WireReference.position, Point2.WireReference.position)); // set the scale so it looks like a wire and not a square
    }

    // uses its transform (position, rotation, scale) as set in DrawWire to determine what its two points should be. Used when duplicating a board (as duplicating clusters gets messy) and loading a savegame.
    [Button]
    public void FindPoints()
    {
        RaycastHit ForwardHit;
        if (Physics.Raycast(
            transform.position,
            transform.forward, // as the transform.LookAt used in drawwire points the forward vector in the direction of the thing being looked at
            out ForwardHit,
            (transform.localScale.z / 2) + 0.045f, // half the length of the wire (transform.position is the center of the wire) plus half the width of an input, just in case
            1 << 0)) // cast against only the default layer
        {
            if (ForwardHit.collider.tag == "Input")
            {
                Point1 = ForwardHit.collider.GetComponent<CircuitInput>();
            }
            else if(ForwardHit.collider.tag == "Output")
            {
                Point2 = ForwardHit.collider.GetComponent<Output>();
            }
        }

        RaycastHit BackwardHit;
        if (Physics.Raycast(
            transform.position,
            -transform.forward, // as the transform.LookAt used in drawwire points the forward vector in the direction of the thing being looked at
            out BackwardHit,
            (transform.localScale.z / 2) + 0.045f, // half the length of the wire (transform.position is the center of the wire) plus half the width of an input, just in case
            1 << 0)) // cast against only the default layer
        {
            if (BackwardHit.collider.tag == "Input")
            {
                Point1 = ForwardHit.collider.GetComponent<CircuitInput>();
            }
            else if (BackwardHit.collider.tag == "Output")
            {
                Point2 = BackwardHit.collider.GetComponent<Output>();
            }
        }

        if (Point1 == null || Point2 == null) { Debug.Log("destroying wire"); Destroy(gameObject); return; }

        if (!Point1.IOConnections.Contains(this))
        {
            Point1.IOConnections.Add(this);
        }
        if (!Point2.GetIOConnections().Contains(this))
        {
            Point2.AddIOConnection(this);
        }
    }

    // used to make sure a wire won't destroy itself on load. TODO: merge code with above method
    public bool CanFindPoints()
    {
        CircuitInput FoundPoint1 = null;
        Output FoundPoint2 = null;

        RaycastHit ForwardHit;
        if (Physics.Raycast(
            transform.position,
            transform.forward, // as the transform.LookAt used in drawwire points the forward vector in the direction of the thing being looked at
            out ForwardHit,
            (transform.localScale.z / 2) + 0.045f, // half the length of the wire (transform.position is the center of the wire) plus half the width of an input, just in case
            1 << 0)) // cast against only the default layer
        {
            if (ForwardHit.collider.tag == "Input")
            {
                FoundPoint1 = ForwardHit.collider.GetComponent<CircuitInput>();
            }
            else if (ForwardHit.collider.tag == "Output")
            {
                FoundPoint2 = ForwardHit.collider.GetComponent<Output>();
            }
        }

        RaycastHit BackwardHit;
        if (Physics.Raycast(
            transform.position,
            -transform.forward, // as the transform.LookAt used in drawwire points the forward vector in the direction of the thing being looked at
            out BackwardHit,
            (transform.localScale.z / 2) + 0.045f, // half the length of the wire (transform.position is the center of the wire) plus half the width of an input, just in case
            1 << 0)) // cast against only the default layer
        {
            if (BackwardHit.collider.tag == "Input")
            {
                FoundPoint1 = ForwardHit.collider.GetComponent<CircuitInput>();
            }
            else if (BackwardHit.collider.tag == "Output")
            {
                FoundPoint2 = BackwardHit.collider.GetComponent<Output>();
            }
        }

        if (FoundPoint1 == Point1 && FoundPoint2 == Point2) { return true; }
        else { return false; }
    }
}
