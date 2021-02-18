// for rotating stuff. why is this comment even necessary

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using References;

public static class StuffRotater
{
    public static bool AllowedToDoRotation = true; // for when you're placing wires
    public static void RunGameplayRotation()
    {
        if (!AllowedToDoRotation) { return; }
        if (StuffPlacer.OkayToPlace) { return; }

        if (Input.GetButtonDown("Rotate"))
        {
            RaycastHit hit;
            if(Physics.Raycast(FirstPersonInteraction.Ray(), out hit, Settings.ReachDistance))
            {
                RotateThing(hit.collider.gameObject);
            }
            else { SoundPlayer.PlaySoundGlobal(Sounds.FailDoSomething); }
        }

        // kind of awkward that both this class and StuffPlacer are fucking with RotationLocked. TODO do it better
        if (Input.GetButtonDown("RotationLock"))
        {
            RaycastHit hit;
            if (Physics.Raycast(FirstPersonInteraction.Ray(), out hit, Settings.ReachDistance, Wire.IgnoreWiresLayermask))
            {
                if (StuffPlacer.GetThingBeingPlaced == null || !StuffPlacer.OkayToPlace) // so that if there's something we're placing we can lock rotation to that and not what's under it
                {
                    if (hit.collider.tag == "World" || hit.collider.tag == "CircuitBoard") { SetRotationLockToFalseIfThingBeingPlacedIsNull(); return; }
                    StuffPlacer.SetRotationLockAngles(ComponentPlacer.FullComponent(hit.collider));
                    StuffPlacer.RotationLocked = true;
                    SelectionMenu.Instance.SetRotationLockText();
                }
            }
            else { StuffPlacer.RotationLocked = false; }
        }

        if (Input.GetButtonDown("RotateThroughBoard"))
        {
            RotateThroughBoard();
        }
    }

    public static void RotateThing(GameObject RotateThis)
    {
        // determines which direction to rotate
        // TODO: use camera.main.transform to rotate stuff left and right rather than clockwise & ccw

        if (RotateThis.tag == "World")
        {
            SoundPlayer.PlaySoundGlobal(Sounds.FailDoSomething);
            return;
        }

        if (RotateThis.tag == "CircuitBoard" || RotateThis.tag == "PlaceOnlyCircuitBoard")
        {
            SoundPlayer.PlaySoundGlobal(Sounds.FailDoSomething);
            return;
        }

        // rotate the full object, not part of it
        // the last check is so that rotating child boards doesn't rotate their parent
        RotateThis = ComponentPlacer.FullComponent(RotateThis);

        Quaternion BeforeRotation = Quaternion.identity;
        Vector3 AxisToRotateAround = RotateThis.transform.up;

        // everything but wires should rotate around transform.up
        if (RotateThis.tag == "Wire")
        {
            if (RotateThis.GetComponent<SnappedConnection>()) // you cannot rotate snapped connections
            {
                SoundPlayer.PlaySoundGlobal(Sounds.FailDoSomething);
                return;
            }

            AxisToRotateAround = RotateThis.transform.forward;
            StuffConnector.QueueWireMeshRecalculation(RotateThis);

            SoundPlayer.PlaySoundAt(Sounds.RotateSomething, RotateThis);
        }
        else
        {
            BeforeRotation = RotateThis.transform.rotation; // non-wires might be rotated into invalid positions, in which case they should be reverted to their original rotations
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
            RotateThis.transform.RotateAround(RotateThis.transform.position, AxisToRotateAround, direction * 22.5f);
        }
        else // ...but normally rotate by 90 degrees
        {
            RotateThis.transform.RotateAround(RotateThis.transform.position, AxisToRotateAround, direction * 90f);
        }

        // the validity of wire placement is never affected when rotating them, but if it's an object, check if it can actually be placed there. If not, revert to original rotation
        if (RotateThis.tag != "Wire")
        {
            BoxCollider[] colliders = RotateThis.GetComponentsInChildren<BoxCollider>();
            StuffPlacer.SetStateOfBoxColliders(colliders, false);

            if (StuffPlacer.GameObjectIntersectingStuffOrWouldDestroyWires(RotateThis, true, true)) // ignore wires
            {
                RotateThis.transform.rotation = BeforeRotation;
                StuffPlacer.SetStateOfBoxColliders(colliders, true);

                SoundPlayer.PlaySoundGlobal(Sounds.FailDoSomething);
                return;
            }

            SoundPlayer.PlaySoundAt(Sounds.RotateSomething, RotateThis);
            StuffPlacer.SetStateOfBoxColliders(colliders, true);
        }

        FloatingPointRounder.RoundIn(RotateThis);

        RedrawCircuitGeometryOf(RotateThis);
        DestroyIntersectingConnections(RotateThis);

        SnappingPeg.TryToSnapIn(RotateThis);

        MegaMeshManager.RecalculateGroupsOf(RotateThis);
    }

    private static bool AllowFlippingOneSidedComponents = Settings.Get("AllowFlippingOneSidedComponents", false);

    private static void RotateThroughBoard()
    {
        RaycastHit hit;
        if(Physics.Raycast(FirstPersonInteraction.Ray(), out hit, Settings.ReachDistance, Wire.IgnoreWiresLayermask))
        {
            if (hit.collider.tag == "World" || hit.collider.tag == "CircuitBoard") { SoundPlayer.PlaySoundGlobal(Sounds.FailDoSomething); return; }
            GameObject component = ComponentPlacer.FullComponent(hit.collider);
            if (component.transform.parent == null) { return; }
            if (!AllowFlippingOneSidedComponents)
            {
                if (!component.GetComponent<IsThroughComponent>()) { SoundPlayer.PlaySoundGlobal(Sounds.FailDoSomething); return; }
            }

            component.transform.localEulerAngles += new Vector3(0, 0, 180);
            component.transform.Translate(Vector3.up * 0.15f, Space.Self);

            BoxCollider[] colliders = component.GetComponentsInChildren<BoxCollider>();
            StuffPlacer.SetStateOfBoxColliders(colliders, false);

            if (StuffPlacer.GameObjectIntersectingStuffOrWouldDestroyWires(component, true))
            {
                component.transform.localEulerAngles += new Vector3(0, 0, 180);
                component.transform.Translate(Vector3.up * 0.15f, Space.Self);
                StuffPlacer.SetStateOfBoxColliders(colliders, true);
                SoundPlayer.PlaySoundGlobal(Sounds.FailDoSomething);
                return;
            }

            FloatingPointRounder.RoundIn(component);

            StuffPlacer.SetStateOfBoxColliders(colliders, true);

            RedrawCircuitGeometryOf(component);
            DestroyIntersectingConnections(component);
            MegaMeshManager.RecalculateGroupsOf(component);

            SoundPlayer.PlaySoundAt(Sounds.RotateSomething, component);
        }
        else
        {
            SoundPlayer.PlaySoundGlobal(Sounds.FailDoSomething);
        }
    }

    // takes an object and checks all its connections and redraws those wires, or destroys them if necessary
    // this function should probably be in a different class, especially since it's now being used by BoardPlacer... not sure where to put it though...
    public static void RedrawCircuitGeometryOf(GameObject RotatedObject)
    {
        CircuitInput[] inputs = RotatedObject.GetComponentsInChildren<CircuitInput>();
        CircuitOutput[] outputs = RotatedObject.GetComponentsInChildren<CircuitOutput>();

        foreach (CircuitInput input in inputs)
        {
            foreach (InputOutputConnection connection in input.IOConnections)
            {
                connection.DrawWire();
                connection.Output.QueueMeshRecalculation();
            }
            foreach (InputInputConnection connection in input.IIConnections)
            {
                connection.DrawWire();
            }

            if (input.Cluster != null) { input.Cluster.QueueMeshRecalculation(); }
        }

        foreach (CircuitOutput output in outputs)
        {
            foreach (InputOutputConnection connection in output.GetIOConnections())
            {
                connection.DrawWire();
            }

            output.QueueMeshRecalculation();
        }
    }

    public static void DestroyIntersectingConnections(GameObject PlacedObject)
    {
        BoxCollider[] boxes = PlacedObject.GetComponentsInChildren<BoxCollider>();
        List<GameObject> WiresDestroyed = new List<GameObject>();

        foreach (BoxCollider box in boxes)
        {
            Vector3 center = box.transform.TransformPoint(box.center);
            Vector3 halfextents = Vector3.Scale(box.size, box.transform.lossyScale) / 2;
            Vector3 direction = box.transform.up;
            Quaternion orientation = box.transform.rotation;

            RaycastHit[] hits = Physics.BoxCastAll(center, halfextents, direction, orientation, 1); // not sure why maxdistance needs to be 1 here but 0 doesn't register ANY wire collisions. Possibly something to do with how thin their colliders are?
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.tag == "Wire")
                {
                    // manually check connections, just in case the boxcast hits a false positive
                    if (!WirePlacer.CanConnect(hit.collider.gameObject) || !hit.collider.GetComponent<Wire>().CanFindPoints())
                    {
                        if (!WiresDestroyed.Contains(hit.collider.gameObject))
                        {
                            WiresDestroyed.Add(hit.collider.gameObject);
                            StuffDeleter.DestroyWire(hit.collider.gameObject);
                        }
                    }
                }
            }
        }
    }

    private static void SetRotationLockToFalseIfThingBeingPlacedIsNull()
    {
        if(StuffPlacer.GetThingBeingPlaced == null) { StuffPlacer.RotationLocked = false; }
    }
}