using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class InputInputConnection : MonoBehaviour {

    public CircuitInput Point1;
    public CircuitInput Point2;

    public void SetPoint1(CircuitInput input)
    {
        Point1 = input;
        input.IIConnections.Add(this);
    }
    public void SetPoint2(CircuitInput input)
    {
        Point2 = input;
        input.IIConnections.Add(this);
    }

    public Renderer Renderer;
    public MeshFilter Mesh;

    private void Awake()
    {
        Mesh = GetComponent<MeshFilter>();
        Renderer = GetComponent<Renderer>();
        Renderer.material.color = MiscellaneousSettings.CircuitOffColor; // so that when not connected to a cluster, it's off
    }

    public bool unbreakable; // currently only used by through pegs

    // sets the transforms of the gameobject, and by extension the plane, to look like it's a wire
    public void DrawWire()
    {
        // TODO: make wire have rotation relative to the first point
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
            if (ForwardHit.collider == null) { Destroy(gameObject); }
            else if (ForwardHit.collider.tag == "Input")
            {
                Point1 = ForwardHit.collider.GetComponent<CircuitInput>();
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
            if (BackwardHit.collider == null) { Destroy(gameObject); }
            if (BackwardHit.collider.tag == "Input" 
                && BackwardHit.collider.gameObject 
                != ForwardHit.collider.gameObject)
            {
                Point2 = BackwardHit.collider.GetComponent<CircuitInput>();
            }
        }

        if (Point1 == null || Point2 == null) { Debug.Log("destroying wire"); Destroy(gameObject); return; }

        if (!Point1.IIConnections.Contains(this))
        {
            Point1.IIConnections.Add(this);
        }
        if (!Point2.IIConnections.Contains(this))
        {
            Point2.IIConnections.Add(this);
        }
    }
}
