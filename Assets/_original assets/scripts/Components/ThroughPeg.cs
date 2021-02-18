// if it's stupid but it works, it ain't stupid

using UnityEngine;

public class ThroughPeg : MonoBehaviour
{
    CircuitInput[] ChildInputs;

    void Awake()
    {
        if (transform.parent != null && transform.parent.gameObject.layer == 5) { return; } // don't do this in the UI cuz it won't be visible

        ChildInputs = GetComponentsInChildren<CircuitInput>();

        if (GetComponentInChildren<Wire>()) { return; } // if it's on a cloned board it'll already have a wire

        InputInputConnection connection = Instantiate(References.Prefabs.Wire, transform).AddComponent<InputInputConnection>();
        connection.Input1 = ChildInputs[0];
        connection.Input2 = ChildInputs[1];

        connection.DrawWire();
        connection.unbreakable = true;
        StuffConnector.LinkConnection(connection);

        DestroyImmediate(connection.GetComponent<BoxCollider>());
    }
}