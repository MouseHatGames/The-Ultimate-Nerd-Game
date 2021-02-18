using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Wire : MonoBehaviour
{
    // reference points for wire drawing 
    public Transform Point1;
    public Transform Point2;

    public GameObject Peg1 { get { return Point1.parent.gameObject; } }
    public GameObject Peg2 { get { return Point2.parent.gameObject; } }

    // stuff we need for mesh combining
    public Renderer Renderer;
    public MeshFilter MeshFilter;

    public virtual Material DefaultMaterial { get { return References.Materials.CircuitOff; } }

    private void Awake()
    {
        MeshFilter = GetComponent<MeshFilter>();
        Renderer = GetComponent<Renderer>();
        Renderer.material = DefaultMaterial;
    }

    public bool unbreakable = false; // this is how through pegs work

    // sets the transforms of the gameobject, and by extension the plane, to look like it's a wire
    public void DrawWire()
    {
        transform.position = (Point1.position + Point2.position) / 2; // set the position to the center between the two vectors
        transform.LookAt(Point1, Point1.up); // point towards Point1, rotated so that the wire has Point1's up
        transform.localScale = new Vector3(0.05f, 0.02f, Vector3.Distance(Point1.position, Point2.position)); // set the scale so it looks like a wire and not a square
    }

    public void FindPoints()
    {
        if (unbreakable) { SetPegsBasedOnPoints(); return; }

        Point1 = FindPoint(true);
        Point2 = FindPoint(false);

        if (Point1 == null || Point2 == null) { Debug.LogError("ERROR: failed to find a point - destroying wire"); Destroy(gameObject); return; }

        SetPegsBasedOnPoints();
    }

    public bool CanFindPoints()
    {
        if (unbreakable) { return true; }

        Transform FoundPoint1 = FindPoint(true);
        Transform FoundPoint2 = FindPoint(false);

        if (Point1 == null || Point2 == null) { return false; }
        if (Point1 == FoundPoint1 && Point2 == FoundPoint2) { return true; }
        return false;
    }

    Transform FindPoint(bool WhichPoint)
    {
        Vector3 CastDirection = WhichPoint ? transform.forward : -transform.forward; // as the transform.LookAt used in drawwire points the forward vector in the direction of the thing being looked at
        Vector3 DifferenceToPoint = -CastDirection * (transform.localScale.z / 2);
        Vector3 PointPosition = transform.position + DifferenceToPoint;

        RaycastHit hit;
        Physics.Raycast(
        PointPosition,
        CastDirection,
        out hit,
        transform.localScale.z + 0.045f, // the length of the wire plus half the width of an input, just in case
        IgnoreWiresLayermask);
        
        return GetWireReference(hit);
    }


    // inherited classes find their inputs/outputs based on what point1 & point2 are
    abstract public void SetPegsBasedOnPoints();

    // static properties
    public static readonly LayerMask IgnoreWiresLayermask = (1 << 0) | (1 << 9); // not actually a layermask that ignores wires, just one that only hits stuff on the default layer and the world layer

    public static Transform GetWireReference(Transform peg)
    {
        if (peg == null) { return null; }
        if (peg.childCount < 1) { return null; }
        return peg.GetChild(0);
    }

    public static Transform GetWireReference(GameObject peg)
    {
        return GetWireReference(peg.transform);
    }

    public static Transform GetWireReference(RaycastHit hit)
    {
        return GetWireReference(hit.transform);
    }
}