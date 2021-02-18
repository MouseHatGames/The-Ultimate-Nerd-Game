using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnappingPeg : CircuitInput
{
    private void Awake()
    {
        Renderer = GetComponent<Renderer>();
        IsSnappingPeg = true;
        Renderer.material = References.Materials.SnappingPeg;
    }

    [NaughtyAttributes.Button]
    public void TryToConnect()
    {
        DestroySnappedConnection();
        SnappingPeg OtherSnappyPeg = GetPegToSnapTo(); // I quite like the prefix "snappy"

        if (OtherSnappyPeg != null && OtherSnappyPeg.GetPegToSnapTo() == this) // make sure the snapping can occur both ways
        {
            SnappedConnection SnappyConnection = Instantiate(References.Prefabs.Wire).AddComponent<SnappedConnection>();
            SnappyConnection.Input1 = this;
            SnappyConnection.Input2 = OtherSnappyPeg;
            SnappyConnection.DrawWire();
            SnappyConnection.Initialize();

            StuffConnector.LinkInputs(SnappyConnection);

            if (BehaviorManager.AllowedToUpdate) { SoundPlayer.PlaySoundAt(References.Sounds.ConnectionFinal, transform); } // the check is so it doesn't play on loaded pegs
        }
    }

    public SnappingPeg GetPegToSnapTo()
    {
        Vector3 origin = Wire.GetWireReference(gameObject).position;
        RaycastHit hit;
        if (Physics.Raycast(origin, -transform.forward, out hit, 0.20f, Wire.IgnoreWiresLayermask)) // snapped connections will be about 18cm long; we cast for 20, just to be safe
        {
            if (hit.collider.tag == "Input")
            {
                SnappingPeg OtherSnappyPeg = hit.collider.GetComponent<SnappingPeg>();
                if (OtherSnappyPeg != null)
                {
                    if (WirePlacer.CanConnect(gameObject, OtherSnappyPeg.gameObject) && !WirePlacer.ConnectionExists(gameObject, OtherSnappyPeg.gameObject)
                    && hit.transform.InverseTransformPoint(hit.point).z < -0.49f // make sure it hits the right face of the other peg
                    &&
                    // make sure it's rotated approximately correctly
                    ((hit.transform.eulerAngles.y + 180 > transform.eulerAngles.y - 2 && hit.transform.eulerAngles.y + 180 < transform.eulerAngles.y + 2)
                    || (hit.transform.eulerAngles.y - 180 > transform.eulerAngles.y - 2 && hit.transform.eulerAngles.y - 180 < transform.eulerAngles.y + 2)))
                    {
                        return OtherSnappyPeg;
                    }
                }
            }
        }

        return null;
    }

    // must be non-serialized to prevent bugs. Sorry, I'd describe them, but 0.2 comes out in 38 minutes HOLY SHIT
    [System.NonSerialized]
    public SnappedConnection SnappedConnection;

    private void DestroySnappedConnection()
    {
        if(SnappedConnection == null) { return; }
        StuffDeleter.DestroyWire(SnappedConnection);
    }


    public static void TryToSnapIn(GameObject bigboimanboi)
    {
        SnappingPeg[] AllSnappers = bigboimanboi.GetComponentsInChildren<SnappingPeg>();
        foreach (SnappingPeg snapper in AllSnappers)
        {
            snapper.TryToConnect();
        }
    }

    public static void SnapEverywhere()
    {
        SnappingPeg[] AllSnappers = Object.FindObjectsOfType<SnappingPeg>();
        foreach (SnappingPeg snapper in AllSnappers)
        {
            snapper.TryToConnect();
        }
    }
}