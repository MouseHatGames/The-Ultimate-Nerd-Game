// for placing stuff on boards and other stuff

// TODO: it would be nice if you could see a transparent preview of the object you're placing before you commit to placing it, like with how circuitboards work

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StuffPlacer : MonoBehaviour {

    // this prefab is placed during placing. If it is null, placing is disabled
    public static GameObject PrefabToPlace;

    // determine which place function to use
    // this function uses raycasthits because it must determine both the thing it was placed on and the point at which it was placed
    public static void PlaceOnSomething(RaycastHit hit)
    {
        // when the player has nothing selected just peace out
        if(PrefabToPlace == null)
        {
            return;
        }

        else if (hit.collider.tag == "Input" || hit.collider.tag == "Output" || hit.collider.tag == "BoardObject" || hit.collider.tag == "Wire") // if the cast hits an output, input, wire or boardobject, stop the function. You cannot place on these types of object.
        {
            return;
        }

        else if (hit.collider.tag == "CircuitBoard") // if the cast hits a board, place it on a board
        {
            PlaceOnBoard(hit);
        }
        else // if it's not a board, place it on the mystery material. This is most often terrain.
        {
            PlaceOnOther(hit);
        }
    }

    // places on a CircuitBoard locked to its grid
    public static void PlaceOnBoard(RaycastHit hit)
    {
        Vector3 LocalPosition = hit.collider.transform.InverseTransformPoint(hit.point);

        bool top = true; // whether the placement is on the top of the board
        if(LocalPosition.y < 0)
        {
            top = false; // set top to false if we're placing on the bottom
        }

        CircuitBoard Board = hit.transform.gameObject.GetComponent<CircuitBoard>();

        Vector2Int BoardCoordinates = new Vector2Int(Mathf.RoundToInt((LocalPosition.x - 0.15f) / 0.3f), Mathf.RoundToInt((LocalPosition.z - 0.15f) / 0.3f)); // get the integer coordinates on the board by converting from the 0.3 scale

        // cap the coordinates of the board so you can't place outside it
        if (BoardCoordinates.x >= Board.x) { BoardCoordinates.x = Board.x - 1; }
        if (BoardCoordinates.y >= Board.z) { BoardCoordinates.y = Board.z - 1; }
        if (BoardCoordinates.x < 0) { BoardCoordinates.x = 0; }
        if (BoardCoordinates.y < 0) { BoardCoordinates.y = 0; }

        if (Board.SpaceOccupied(BoardCoordinates, top)) { return; } // only place something here if the position is unoccupied. TODO: for objects that go through the board, also check the other side

        GameObject PlacedObject = Instantiate(PrefabToPlace, new Vector3(10000, 10000, 10000), Quaternion.identity, Board.transform);

        // get the y position that the object is placed at based on whether it's on top or not
        float YPosition = 0.25f;
        if (!top)
        {
            YPosition = -0.25f; // the initial value assumes it is top, correct that if it is not
        }

        PlacedObject.transform.localPosition = new Vector3(BoardCoordinates.x + 0.5f, YPosition, BoardCoordinates.y + 0.5f) * 0.3f; // the +0.5fs are to make it within the grid, not on the lines

        // rotation
        float Rotation;
        if (RotationLocked) { Rotation = LockedBoardRotation; } // if locked, set it to the locked rotation
        else
        {
            PlacedObject.transform.forward = -Camera.main.transform.forward;
            Rotation = Mathf.RoundToInt(PlacedObject.transform.localEulerAngles.y / 90) * 90;
        }

        if (!top) { Rotation += 180; }

        float XRotation = 0;
        if (!top)
        {
            XRotation = 180;
        }

        PlacedObject.transform.localEulerAngles = new Vector3(XRotation, Rotation, 0);

        PlacedObject.AddComponent<SaveThisObject>().ObjectType = PrefabToPlace.name;

        DestroyIntersectingConnections(PlacedObject);

        MegaMesh.AddMeshesFrom(PlacedObject);
    }

    public static void PlaceOnOther(RaycastHit hit)
    {
        GameObject PlacedObject = Instantiate(PrefabToPlace, hit.point, Quaternion.identity);
        PlacedObject.transform.up = hit.normal;

        if (RotationLocked) { PlacedObject.transform.Rotate(PlacedObject.transform.up, LockedOtherRotation, Space.World); } // if rotation is locked, apply the lock
        else { PlacedObject.transform.Rotate(PlacedObject.transform.up, FirstPersonInteraction.FirstPersonCamera.transform.parent.localEulerAngles.y + 180, Space.World); } // otherwise, make it dependant on which direction we're looking in

        PlacedObject.AddComponent<SaveThisObject>().ObjectType = PrefabToPlace.name;

        DestroyIntersectingConnections(PlacedObject);

        MegaMesh.AddMeshesFrom(PlacedObject);
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
                    if (!StuffConnecter.CanConnect(hit.collider.gameObject))
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

    // rotation locking stuff
    public static bool RotationLocked;
    public static float LockedBoardRotation;
    public static float LockedOtherRotation;

    public static BuildMenu buildmenu; // set in BuildMenu.awake

    public static void ToggleRotationLock()
    {
        // if we're looking at a board object, get the rotation of that object and lock it
        RaycastHit hit;
        Transform cam = FirstPersonInteraction.FirstPersonCamera.transform;
        if (Physics.Raycast(cam.position, cam.forward, out hit, MiscellaneousSettings.ReachDistance))
        {
            Transform GetRotationFromThis = hit.collider.transform;

            while(GetRotationFromThis.transform.parent != null && GetRotationFromThis.transform.parent.tag != "CircuitBoard") // get the full object if the hit hits e.g. an output
            {
                GetRotationFromThis = GetRotationFromThis.transform.parent;
            }

            if (GetRotationFromThis.tag != "CircuitBoard" && GetRotationFromThis.tag != "World" && GetRotationFromThis.tag != "Wire" && GetRotationFromThis.tag != "Untagged") // these types of object are invalid!
            {
                LockedOtherRotation = GetRotationFromThis.localEulerAngles.y;
                LockedBoardRotation = Mathf.RoundToInt(LockedOtherRotation / 22.5f) * 22.5f; // rounded to the nearest 22.5 degrees

                RotationLocked = true;
                buildmenu.ToggleRotationLockText();
                return;
            }
        }

        // otherwise, unlock rotation
        RotationLocked = false;
        buildmenu.ToggleRotationLockText();
    }
}