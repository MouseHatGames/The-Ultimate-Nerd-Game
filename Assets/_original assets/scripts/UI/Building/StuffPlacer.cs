// generic functions used by placing classes
// holy SHIT this class is a FUCKING MESS. TODO: CLEAN IT THE FUCK UP

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;
using References;

public static class StuffPlacer
{
    // some settings

    private static int MaxComplexityToCalculateOutlinesFor = Settings.Get("MaxComplexityToCalculateOutlinesFor", 1000);

    // recalculate whether something being placed can have green/red outlines every this number of frames
    private static int PlacementValidityRecalculationInterval = Settings.Get("PlacementValidityRecalculationInterval", 4);


    private static GameObject ThingBeingPlaced = null;

    public static GameObject GetThingBeingPlaced { get { return ThingBeingPlaced; } }

    public static void NewThingBeingPlaced(GameObject NewThingBeingPlaced)
    {
        if (NewThingBeingPlaced != ThingBeingPlaced) { DeleteThingBeingPlaced(); }

        ThingBeingPlaced = NewThingBeingPlaced;
        if (NewThingBeingPlaced == null) { return; }

        // the following stuff is only done when you set a new object as the ThingBeingPlaced
        OutlinesOfThingBeingPlaced = OutlineObjectAndReturnTheNewOutlines(ThingBeingPlaced);

        BoxCollidersOfThingBeingPlaced = ThingBeingPlaced.GetComponentsInChildren<BoxCollider>();
        SetStateOfAllBoxCollidersFromThingBeingPlaced(false);

        // mega mesh stuff
        MegaMeshManager.RemoveComponentsImmediatelyIn(NewThingBeingPlaced);

        VisualUpdaterWithMeshCombining[] visualbois = NewThingBeingPlaced.GetComponentsInChildren<VisualUpdaterWithMeshCombining>();
        foreach (VisualUpdaterWithMeshCombining visualboi in visualbois)
        {
            visualboi.AllowedToCombineOnStable = false;
        }
    }

    public static bool OkayToPlace
    {
        get
        {
            if (ThingBeingPlacedIntersectingStuffOrWouldDestroyWires()) { return false; } // you can't place if it's intersecting stuff
            if (ThingBeingPlaced == null) { return false; } // there is so much nullreferenceexception protection in this class...
            if (!CurrentPlacementIsValid) { return false; }
            return true;
        }
    }

    public static void PlaceThingBeingPlaced()
    {
        if (!OkayToPlace)
        {
            // shitty hack to prevent the sound playing in specific circumstances that are undesirable
            RaycastHit hit;
            Physics.Raycast(FirstPersonInteraction.Ray(), out hit, Settings.ReachDistance, Wire.IgnoreWiresLayermask);
            if (hit.collider == null || hit.collider.tag != "Interactable") { SoundPlayer.PlaySoundGlobal(Sounds.FailDoSomething); }

            return;
        }

        RemoveOutlineFromObject(ThingBeingPlaced, true); // destoryimmediate used so that if what you place has you looking at a peg, you'll be able to get it highlighted
        SetStateOfAllBoxCollidersFromThingBeingPlaced(true);
        OutlinesOfThingBeingPlaced = null;
        BoxCollidersOfThingBeingPlaced = null;

        FloatingPointRounder.RoundIn(ThingBeingPlaced, true);

        // mega mesh stuff
        MegaMeshManager.AddComponentsIn(ThingBeingPlaced);
        foreach (VisualUpdaterWithMeshCombining visualboi in ThingBeingPlaced.GetComponentsInChildren<VisualUpdaterWithMeshCombining>())
        {
            visualboi.AllowedToCombineOnStable = true;
        }
        SnappingPeg.TryToSnapIn(ThingBeingPlaced);

        if (MostRecentPlacementWasOnBoard) { SoundPlayer.PlaySoundAt(Sounds.PlaceOnBoard, ThingBeingPlaced); }
        else { SoundPlayer.PlaySoundAt(Sounds.PlaceOnTerrain, ThingBeingPlaced); }

        ThingBeingPlaced = null;
    }

    public static void DeleteThingBeingPlaced()
    {
        if (ThingBeingPlaced == null) { return; }

        Object.Destroy(ThingBeingPlaced);
        ThingBeingPlaced = null;
        ResetReferences();
    }

    public static void ResetReferences()
    {
        OutlinesOfThingBeingPlaced = null;
        BoxCollidersOfThingBeingPlaced = null;
    }

    public static BoxCollider[] BoxCollidersOfThingBeingPlaced = new BoxCollider[0]; // this is cached because every frame we need to check for intersections
    public static Outline[] OutlinesOfThingBeingPlaced = new Outline[0]; // this is cached because changing outline color happens often

    private static RaycastHit MostRecentNonNullHit; // used so we can do rotation without looking at the thing
    public static void RunStuffPlacing(bool AllowFineRotation = true, bool HideWhenInvalidPlacement = false, bool AllowEdgePlacement = false)
    {
        if (ThingBeingPlaced == null) { return; }

        PollRotationInput(AllowFineRotation);

        RaycastHit hit;
        if (Physics.Raycast(FirstPersonInteraction.Ray(), out hit, Settings.ReachDistance))
        {
            DisableRotation = false;
            MoveThingBeingPlaced(hit, HideWhenInvalidPlacement, AllowEdgePlacement);
        }
        else if (HideWhenInvalidPlacement) { ThingBeingPlaced.transform.position = new Vector3(0, -10000, 0); CurrentPlacementIsValid = false; }

        if(hit.collider != null)
        {
            MostRecentNonNullHit = hit;
        }

        if (Input.GetButtonDown("Place"))
        {
            PlaceThingBeingPlaced();
        }
    }

    // rotation stuff
    private static float TrueRotationAboutUpVector;
    public static float RotationAboutUpVector
    {
        get { return TrueRotationAboutUpVector; }
        set
        {
            TrueRotationAboutUpVector = value;

            // keep it within the range of 0-360
            while (RotationAboutUpVector > 360) { RotationAboutUpVector -= 360; }
            while (RotationAboutUpVector < 0) { RotationAboutUpVector += 360; }
        }
    }
    public static bool RotationLocked;
    private static float TerrainRotationLockAngle = 0; // in degrees
    private static float BoardRotationLockAngle = 0; // in degrees

    public static void SetRotationLockAngles (float RotationValueBeingLockedTo)
    {
        TerrainRotationLockAngle = RotationValueBeingLockedTo;
        BoardRotationLockAngle = Mathf.RoundToInt(RotationValueBeingLockedTo / 22.5f) * 22.5f; // rounded to the nearest 22.5 degrees
        RotationAboutUpVector = 0;
    }

    public static void SetRotationLockAngles (GameObject ThingToLockTo)
    {
        SetRotationLockAngles(ThingToLockTo.transform.localEulerAngles.y);
    }

    public static bool DisableRotation;
    // should be run every frame while placing
    public static void PollRotationInput(bool AllowFineRotation = true)
    {
        if (ThingBeingPlaced == null || !CurrentPlacementIsValid || DisableRotation) { return; } // don't allow any rotating or anything while you can't see the thingbeingplaced

        if (Input.GetButtonDown("Rotate"))
        {
            float degrees = 90;
            if (Input.GetButton("Mod") && AllowFineRotation) { degrees = 22.5f; }

            if (Input.GetAxis("Rotate") > 0) { RotationAboutUpVector += degrees; }
            else { RotationAboutUpVector -= degrees; }

            MoveThingBeingPlaced(MostRecentNonNullHit, false, true); // ee... hopefully this doesn't break component placing...
        }

        if (Input.GetButtonDown("RotationLock"))
        {
            RotationAboutUpVector = 0;
            // the following chunk of code was 28 lines in 0.1. I am good at refactoring 😎
            RotationLocked = !RotationLocked;
            if (RotationLocked) { SetRotationLockAngles(ThingBeingPlaced); }
            SelectionMenu.Instance.SetRotationLockText();
        }
    }

    // moving methods
    private static bool CurrentPlacementIsValid;
    public static bool MostRecentPlacementWasOnBoard;
    public static void MoveThingBeingPlaced(RaycastHit hit, bool HideWhenInvalidPlacement = false, bool AllowEdgeBoardMovement = false)
    {
        CurrentPlacementIsValid = true;
        // when the code lines up just right 👌
        // the above comment is from before I added the AllowEdgeBoardMovement argument. It was so perfect 😢
        if (hit.collider.tag == "CircuitBoard" || hit.collider.tag == "PlaceOnlyCircuitBoard") { MoveOnBoard(hit, AllowEdgeBoardMovement); MostRecentPlacementWasOnBoard = true; }
        else if (hit.collider.tag == "World") { MoveOnWorld(hit); MostRecentPlacementWasOnBoard = false; }
        else
        {
            CurrentPlacementIsValid = false;
            if (HideWhenInvalidPlacement) { ThingBeingPlaced.transform.position = new Vector3(0, -10000, 0); }
        }

        OutlineThingBeingPlacedAppropriately();
    }

    private static void MoveOnWorld(RaycastHit hit)
    {
        ThingBeingPlaced.transform.position = hit.point;
        ThingBeingPlaced.transform.parent = null;

        ThingBeingPlaced.transform.up = hit.normal;
        if (Input.GetButton("Mod")) { ThingBeingPlaced.transform.up = Vector3.up; } // hold ctrl to make thing parallel/perpendicular to the ground when placing on non-flat terrain
        
        // handle rotation
        float ThisRotationAboutUpVector = RotationAboutUpVector;
        if (RotationLocked) { ThisRotationAboutUpVector += TerrainRotationLockAngle; }
        else { ThisRotationAboutUpVector += FirstPersonInteraction.FirstPersonCamera.transform.parent.localEulerAngles.y + 180; } // +180 is so that stuff faces away from you rather than towards you
        ThingBeingPlaced.transform.Rotate(ThingBeingPlaced.transform.up, ThisRotationAboutUpVector, Space.World);
    }

    // chirst, what a giant fucking method
    // I may be wrong, but I think this is one of the rare cases where refactoring to smaller methods won't really help...
    private static void MoveOnBoard(RaycastHit hit, bool AllowEdgePlacement = false)
    {
        // some important variables for later in the function
        Vector3 LocalPosition = hit.collider.transform.InverseTransformPoint(hit.point);
        CircuitBoard ParentBoard = hit.collider.gameObject.GetComponent<CircuitBoard>();
        Vector2Int BoardCoordinates = new Vector2Int(Mathf.RoundToInt((LocalPosition.x - 0.15f) / 0.3f), Mathf.RoundToInt((LocalPosition.z - 0.15f) / 0.3f)); // get the integer coordinates on the board by converting from the 0.3 scale

        // get the proper y position and direction to point 
        // this is a fairly messy logic chain that could definitely be cleaned up a lot...

        float YPosition = 0;
        bool edge = true;
        Vector3 LocalRotation = Vector3.zero;

        Transform parentboard = hit.collider.transform;
        if (AllowEdgePlacement && hit.normal == parentboard.right) { LocalRotation = MoveOnBoardData.Right; }
        else if (AllowEdgePlacement && hit.normal == -parentboard.right) { LocalRotation = MoveOnBoardData.Left; }
        else if (AllowEdgePlacement && hit.normal == parentboard.forward) { LocalRotation = MoveOnBoardData.Forward; }
        else if (AllowEdgePlacement && hit.normal == -parentboard.forward) { LocalRotation = MoveOnBoardData.Back; }
        else
        {
            edge = false;
            if (LocalPosition.y > 0)
            {
                LocalRotation = MoveOnBoardData.Top;
                YPosition = 0.075f;
            }
            else
            {
                LocalRotation = MoveOnBoardData.Bottom;
                YPosition = -0.075f;
            }
        }

        // cap the coordinates before determining the position
        if (BoardCoordinates.x >= ParentBoard.x) { BoardCoordinates.x = ParentBoard.x - 1; }
        if (BoardCoordinates.y >= ParentBoard.z) { BoardCoordinates.y = ParentBoard.z - 1; }
        if (BoardCoordinates.x < 0) { BoardCoordinates.x = 0; }
        if (BoardCoordinates.y < 0) { BoardCoordinates.y = 0; }

        ThingBeingPlaced.transform.parent = hit.collider.transform; // when placing on a board, we should parent it to that board
        ThingBeingPlaced.transform.localPosition = new Vector3(BoardCoordinates.x + 0.5f, YPosition / 0.3f, BoardCoordinates.y + 0.5f) * 0.3f; // the +0.5fs are to make it within the grid, not on the lines
        ThingBeingPlaced.transform.localEulerAngles = LocalRotation;

        // apply rotation about the up vector
        float ThisRotationAboutUpVector = RotationAboutUpVector + 180; // +180 is so that stuff faces away from you rather than towards you
        if (!edge) // placing angle has no effect when on an edge. By extension, neither does rotation lock.
        {
            if (RotationLocked) { ThisRotationAboutUpVector -= BoardRotationLockAngle + 180; } // look, I don't know why, okay? It just works.
            else
            {
                // angle is based on viewing angle if rotation lock is off
                ThingBeingPlaced.transform.forward = FirstPersonInteraction.FirstPersonCamera.transform.forward;
                ThisRotationAboutUpVector -= Mathf.RoundToInt(ThingBeingPlaced.transform.localEulerAngles.y / 90) * 90; // I've no idea why this needs to be minus and not plus, but it doesn't work properly with plus

                ThingBeingPlaced.transform.localEulerAngles = LocalRotation;
            }
        }
        ThingBeingPlaced.transform.Rotate(ThingBeingPlaced.transform.up, ThisRotationAboutUpVector, Space.World);

        // since the boardcoordinates are capped, edge boards will be placed by default half a unit inside the board. This fixes that
        if (edge) { ThingBeingPlaced.transform.Translate(hit.normal * 0.15f, Space.World); }

        MostRecentBoardPlacementWasOnEdge = edge;
    }

    public static bool MostRecentBoardPlacementWasOnEdge;

    public static bool ThingBeingPlacedIntersectingStuffOrWouldDestroyWires()
    {
        if (BoxCollidersOfThingBeingPlaced == null) { return true; } // best to just not allow stuff to be placed in this scenario

        return BoxCollidersIntersectingStuffOrWouldDestroyWires(BoxCollidersOfThingBeingPlaced, true); // ignore world colliders, otherwise it would be impossible to place stuff on non-flat terrain  
    }

    public static bool GameObjectIntersectingStuffOrWouldDestroyWires(GameObject thingamajig, bool IgnoreWorld = true, bool IgnoreWires = false)
    {
        if (thingamajig == null) { return true; }
        return BoxCollidersIntersectingStuffOrWouldDestroyWires(thingamajig.GetComponentsInChildren<BoxCollider>(), IgnoreWorld, IgnoreWires);
    }

    public static bool BoxCollidersIntersectingStuffOrWouldDestroyWires(BoxCollider[] colliders, bool IgnoreWorld = true, bool IgnoreWires = false)
    {
        foreach (BoxCollider box in colliders)
        {
            Vector3 center = box.transform.TransformPoint(box.center);
            Vector3 halfextents = Vector3.Scale(box.size, box.transform.lossyScale) / 2;

            // make it only go for 97%, so you can place directly next to things
            // 97% is the experimentally determined maximum value to avoid false positives. I suspect this is due to the inaccuracies in transform.localscale but I am not certain of that
            halfextents = Vector3.Scale(halfextents, new Vector3(0.97f, 0.97f, 0.97f));

            Vector3 direction = box.transform.up;
            Quaternion orientation = box.transform.rotation;

            RaycastHit[] hits = Physics.BoxCastAll(center, halfextents, direction, orientation, 0); // maxdistance is 0 so that the box doesn't move!
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.tag == "World")
                {
                    if (!IgnoreWorld) { return true; }
                }
                else if(hit.collider.tag == "Wire")
                {
                    if (!IgnoreWires)
                    {
                        box.enabled = true; // need to enable the collider for these methods to work
                                            // manually check connections, since wires are allowed to clip a little
                        if (!WirePlacer.CanConnect(hit.collider.gameObject) || !hit.collider.GetComponent<Wire>().CanFindPoints())
                        {
                            box.enabled = false;
                            return true;
                        }
                        box.enabled = false;
                    }
                }
                else
                {
                    if (hit.collider.gameObject != box.gameObject) // without this line, non-disabled box colliders will hit themselves in their cast
                    {
                        // Debug.Log("box: " + box.gameObject.name + " , hit: " + hit.collider.gameObject.name);
                        return true;
                    }
                }
            }
        }

        return false;
    }


    // disabling all colliders means that the player can't run into it and that the placement raycast will not hit it
    private static void SetStateOfAllBoxCollidersFromThingBeingPlaced(bool enabled)
    {
        SetStateOfBoxColliders(BoxCollidersOfThingBeingPlaced, enabled);
    }

    public static void SetStateOfBoxColliders(BoxCollider[] colliders, bool enabled)
    {
        if (colliders == null) { return; }
        foreach(BoxCollider collider in colliders)
        {
            collider.enabled = enabled;
        }
    }

    public static void SetStateOfAllBoxCollidersIn(GameObject bigfattiddies, bool enabled)
    {
        if (bigfattiddies == null) { return; }
        SetStateOfBoxColliders(bigfattiddies.GetComponentsInChildren<BoxCollider>(), enabled);
    }

    // methods to do with outlining
    private static int FramesSinceLastOutlineCheck;
    private static void OutlineThingBeingPlacedAppropriately()
    {
        if (OutlinesOfThingBeingPlaced == null) { return; }

        // don't run the performance-intensive ThingBeingPlacedIntersectingStuff if the object is too complex, and give the player visual feedback of that
        if (ThingBeingPlacedExceedsMaxOutliningComplexity)
        {
            SetOutlinesColor(OutlinesOfThingBeingPlaced, OutlineColor.blue);
            return;
        }

        // only do the expensive check every PVRI frames; default 10, configurable from settings.txt
        if (FramesSinceLastOutlineCheck < PlacementValidityRecalculationInterval)
        {
            FramesSinceLastOutlineCheck++;
            return;
        }
        else
        {
            FramesSinceLastOutlineCheck = 0;
        }

        // I'm pretty sure that setting the color ever frame does not have a significant performance impact, but if it does, the state of ThingBeingPlacedIntersectingStuff
        // can be cached, and we'll only set colors if the state has changed since the last check.
        // if you do this, be careful that the check/outline is reset when a new thing is set as the thingbeingplaced!
        if (ThingBeingPlacedIntersectingStuffOrWouldDestroyWires())
        {
            SetOutlinesColor(OutlinesOfThingBeingPlaced, OutlineColor.red);
        }
        else
        {
            SetOutlinesColor(OutlinesOfThingBeingPlaced, OutlineColor.green);
        }
    }

    // adds an outline component for every meshrenderer, so the whole object appears outlined
    public static void OutlineObject(GameObject OutlineThis, OutlineColor outlinecolor = OutlineColor.green)
    {
        if (OutlineThis == null) { return; }

        MeshRenderer[] renderers = OutlineThis.GetComponentsInChildren<MeshRenderer>();
        if(renderers.Length > MaxComplexityToCalculateOutlinesFor)
        {
            SetOutlineColor(renderers[0].gameObject.AddComponent<Outline>(), outlinecolor);
            return;
        }
        foreach (MeshRenderer renderer in renderers)
        {
            if (!renderer.GetComponent<Outline>()) // so we don't add duplicates
            {
                SetOutlineColor(renderer.gameObject.AddComponent<Outline>(), outlinecolor);
            }
        }
    }

    private static bool ThingBeingPlacedExceedsMaxOutliningComplexity;
    public static Outline[] OutlineObjectAndReturnTheNewOutlines(GameObject OutlineThis, OutlineColor outlinecolor = OutlineColor.green)
    {
        MeshRenderer[] renderers = OutlineThis.GetComponentsInChildren<MeshRenderer>();

        if(renderers.Length > MaxComplexityToCalculateOutlinesFor)
        {
            ThingBeingPlacedExceedsMaxOutliningComplexity = true;
            return new Outline[] { renderers[0].gameObject.AddComponent<Outline>() };
        }

        ThingBeingPlacedExceedsMaxOutliningComplexity = false;
        Outline[] outlines = new Outline[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            outlines[i] = renderers[i].gameObject.AddComponent<Outline>(); // note that a gameobject can only ever contain one mesh renderer, so there will not be duplicates
            SetOutlineColor(outlines[i], outlinecolor);
        }

        return outlines;
    }

    public static void RemoveOutlineFromObject(GameObject RemoveOutlineFromThis, bool DestroyImmediate = false)
    {
        if (RemoveOutlineFromThis == null) { return; }

        Outline[] outlines = RemoveOutlineFromThis.GetComponentsInChildren<Outline>();
        foreach (Outline outline in outlines)
        {
            if (DestroyImmediate) { Object.DestroyImmediate(outline); }
            else { Object.Destroy(outline); }
        }
    }

    public static void SetObjectOutlineColor(GameObject ObjectWithOutline, OutlineColor color)
    {
        if (ObjectWithOutline == null) { return; }
        Outline[] outlines = ObjectWithOutline.GetComponentsInChildren<Outline>(); // todo: caching this would increase performance
        SetOutlinesColor(outlines, color);
    }

    private static void SetOutlinesColor(Outline[] outlines, OutlineColor color)
    {
        foreach (Outline outline in outlines)
        {
            SetOutlineColor(outline, color);
        }
    }

    private static void SetOutlineColor(Outline outline, OutlineColor color)
    {
        // a little awkward that the integer color correlation is referenced in code but changeable in editor.
        // in the future, if you're looking to modify it from code, note that OutlineEffect is a singleton and has an Instance static variable
        if (color == OutlineColor.red)
        {
            outline.color = 0;
        }
        else if (color == OutlineColor.green)
        {
            outline.color = 1;
        }
        else if (color == OutlineColor.blue)
        {
            outline.color = 2;
        }
    }

    // for convenience, used by WirePlacer. Can be used like this: OutlineObjects(new GameObject[] { object1, object2 } );
    public static void OutlineObjects(GameObject[] OutlineThese)
    {
        foreach(GameObject nipple in OutlineThese)
        {
            OutlineObject(nipple);
        }
    }

    public static void RemoveOutlineFromObjects(GameObject[] OutlineThese, bool DestroyImmediate = false)
    {
        foreach (GameObject nipple in OutlineThese)
        {
            RemoveOutlineFromObject(nipple, DestroyImmediate);
        }
    }

    // reading my comments, are you now, mister pipe0481?
    // note that this method is much slower than SetOutlinesColor due to the GetComponent()s
    public static void SetObjectsOutlineColor(GameObject[] ReadingMyCodeAreYouNowMisterPipe0481, OutlineColor HeThrustsDeeperIntoMeThanHeEverHasBeforeIFeelItEverywhereMyBackArcsMyToesCurlMyEyesWidenIPullHimCloserToMeFeelTheTightMusclesUnderHisSkinBreatheInHisScentImGaspingForBreathHeLooksMeInTheEyesWithThatSlyGrinOfHisThatILoveSoMuchHeKnowsImCloseSuddenlyHePressesHisFaceIntoMyChestHisMouthOverMyLeftNipplePleasureCoursesThroughMeLikeElectricityEmanatingFromHisLipsHisTongueMyEyesRollBackInMyHeadAndIMoanInEcstasyMyBodyConvulsesWithAnEarthShatteringOrgasmItWasTooMuchMyHeartStopsTheParamedicsDontGetThereInTime)
    {
        foreach(GameObject ObjectWithOutline in ReadingMyCodeAreYouNowMisterPipe0481)
        {
            SetObjectOutlineColor(ObjectWithOutline, HeThrustsDeeperIntoMeThanHeEverHasBeforeIFeelItEverywhereMyBackArcsMyToesCurlMyEyesWidenIPullHimCloserToMeFeelTheTightMusclesUnderHisSkinBreatheInHisScentImGaspingForBreathHeLooksMeInTheEyesWithThatSlyGrinOfHisThatILoveSoMuchHeKnowsImCloseSuddenlyHePressesHisFaceIntoMyChestHisMouthOverMyLeftNipplePleasureCoursesThroughMeLikeElectricityEmanatingFromHisLipsHisTongueMyEyesRollBackInMyHeadAndIMoanInEcstasyMyBodyConvulsesWithAnEarthShatteringOrgasmItWasTooMuchMyHeartStopsTheParamedicsDontGetThereInTime);
        }
    }
}

public enum OutlineColor
{
    red, // invalid placements
    green, // valid placements
    blue // object is too complex, validity of placement will be determined on place
}

public static class MoveOnBoardData
{
    // rotations for all six sides of a cube, to point away from the center of the cube
    // these are used instead of hit.normal because hit.normal will often have some bullshit additional rotation around it
    public static readonly Vector3 Top = new Vector3(0, 0, 0);
    public static readonly Vector3 Bottom = new Vector3(0, 0, 180);
    public static readonly Vector3 Forward = new Vector3(0, 270, 270);
    public static readonly Vector3 Back = new Vector3(0, 270, 90);
    public static readonly Vector3 Left = new Vector3(0, 0, 90);
    public static readonly Vector3 Right = new Vector3(0, 0, 270);
}

// this was a method in StuffPlacer before refactoring in 0.2, back from when you could place stuff inside wires.
// I figured it should be preserved in case it is ever needed again - obviously the method itself could be cleaned up, but there are some useful notes about doing this in the comments.

// checks all wires on the board and destroys any that can't connect
//public void DestroyInvalidWiresOnBoard()
//{
//    InputInputConnection[] IIConnections = BoardBeingPlaced.GetComponentsInChildren<InputInputConnection>();
//    InputOutputConnection[] IOConnections = BoardBeingPlaced.GetComponentsInChildren<InputOutputConnection>();

//    foreach (InputInputConnection connection in IIConnections)
//    {
//        if (!StuffConnector.CanConnect(connection.gameObject))
//        {
//            StuffDeleter.DestroyIIConnection(connection);
//        }
//    }
//    foreach (InputOutputConnection connection in IOConnections)
//    {
//        if (!StuffConnector.CanConnect(connection.gameObject))
//        {
//            StuffDeleter.DestroyIOConnection(connection);
//        }
//    }
//}