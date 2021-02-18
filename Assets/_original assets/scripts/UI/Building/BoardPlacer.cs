using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using References;

public static class BoardPlacer
{
    // the board being placed right now
    public static GameObject BoardBeingPlaced;
    private static CircuitBoard CircuitBoardBeingPlaced;

    // the reference object for the board being placed, to simplify rotation/translation code. The reference object is what is moved by StuffPlacer.
    private static GameObject TrueReferenceObject;
    public static GameObject ReferenceObject
    {
        get
        {
            if(TrueReferenceObject == null)
            {
                TrueReferenceObject = new GameObject("ReferenceObject");
            }
            return TrueReferenceObject;
        }
    }

    private static bool BoardIsFlat;
    private static Vector2Int PlacingOffset;
    private static int RotationState = 1; // for rotating about the awkward axis

    // a flat board is considered to be upside down if the rotation state is 0 or 2. 
    // The qualifying states are chosen so that the board being upside down can be toggled by pressing Z or X from any situation.
    private static bool RotationStateEven
    { get { return RotationState == 0 || RotationState == 2; } }

    public static void NewBoardBeingPlaced(GameObject NewBoard)
    {
        if (NewBoard == null) { return; }

        DestroyBoardBeingPlaced();

        BoardBeingPlaced = NewBoard;
        NewBoard.transform.parent = ReferenceObject.transform;
        CircuitBoardBeingPlaced = NewBoard.GetComponent<CircuitBoard>();

        SetRotationState();
        CapPlacingOffset();

        StuffPlacer.BoardRotationLockAngle = Mathf.Round(StuffPlacer.BoardRotationLockAngle / 90f) * 90; // fixes being able to place boards at non-right angles by locking rotation beforehand
        StuffPlacer.RotationAboutUpVector = Mathf.RoundToInt(StuffPlacer.RotationAboutUpVector / 90) * 90f;

        if (!BoardBeingPlaced.GetComponent<ObjectInfo>())
        {
            BoardBeingPlaced.AddComponent<ObjectInfo>().ComponentType = ComponentType.CircuitBoard;
        }

        BoardFunctions.DestroyAllWiresConnectedToBoardButNotPartOfIt(NewBoard);

        StuffPlacer.NewThingBeingPlaced(ReferenceObject);
    }

    // handles the first person placing of boards. Run every frame while there is a board to be placed
    public static void RunBoardPlacing()
    {
        if (BoardBeingPlaced == null)
        {
            return;
        }

        if (Input.GetButtonDown("Delete") || Input.GetButtonDown("Cancel"))
        {
            CancelPlacement();
        }

        PollForInput();

        SetRotationState();
        ApplyPlacingOffset();

        if (Input.GetButtonDown("Place")) { PlaceBoard(); return; }
        StuffPlacer.RunStuffPlacing(false, false, true); // those bools are AllowFineRotation, HideWhenInvalidPlacement, and AllowEdgePlacement, respectively
    }

    public static void PollForBoardFlatness()
    {
        if (Input.GetButtonDown("ToggleBoardFlatness"))
        {
            BoardIsFlat = !BoardIsFlat;

            SetRotationState();
        }
    }

    private static void PollForInput()
    {
        if (StuffPlacer.DisableRotation) { return; }
        PollForBoardFlatness();
        PollForBoardRotation();
        PollForOffsetInput();
        PollForMirrorInput();
    }

    public static void PollForBoardRotation()
    {
        if (Input.GetButtonDown("BoardRotation"))
        {
            if (Input.GetAxis("BoardRotation") > 0)
            {
                RotationState += 1;
            }
            else if (Input.GetAxis("BoardRotation") < 0)
            {
                RotationState -= 1;
            }

            SetRotationState();
        }
    }

    private static void PollForOffsetInput()
    {
        if (GameplayUIManager.ScrollUp()) // scroll up
        {
            if (Input.GetButton("Mod")) { PlacingOffset.y++; }
            else { PlacingOffset.x++; }
        }

        if (GameplayUIManager.ScrollDown())
        {
            if (Input.GetButton("Mod")) { PlacingOffset.y--; }
            else { PlacingOffset.x--; }
        }

        if (Input.GetButtonDown("ResetBoardOffset")) // middle click
        {
            PlacingOffset = Vector2Int.zero;
        }

        CapPlacingOffset();
    }

    private static void CapPlacingOffset()
    {
        int MaxX, MaxY;

        MaxX = Mathf.CeilToInt(CircuitBoardBeingPlaced.x / 2);
        if (BoardIsFlat) // I don't understand... shouldn't this be !BoardIsFlat?? Fuck, this shouldn't work right, but it does
        {
            MaxY = Mathf.FloorToInt(CircuitBoardBeingPlaced.z / 2);

            // allow placing not on the grid for edge boards
            if (StuffPlacer.MostRecentPlacementWasOnBoard && StuffPlacer.MostRecentBoardPlacementWasOnEdge)
            {
                if(StuffPlacer.RotationAboutUpVector % 180 == 0)
                {
                    MaxX = MaxX * 4 + 2;
                    if (CircuitBoardBeingPlaced.x % 2 == 0 && PlacingOffset.x < -MaxX + 4) { PlacingOffset.x = -MaxX + 4; } // there are so many FUCKING edge cases in this class. Surely there's a better way!
                }
                else
                {
                    MaxY = MaxY * 4 + 2;
                    if (CircuitBoardBeingPlaced.z % 2 == 0 && PlacingOffset.y < -MaxY + 4) { PlacingOffset.y = -MaxY + 4; }
                }
            }
        }
        else
        {
            MaxY = 2;
            if (RotationStateEven)
            {
                MaxX = Mathf.CeilToInt(CircuitBoardBeingPlaced.z / 2);
            }
        }

        if (PlacingOffset.x > MaxX) { PlacingOffset.x = MaxX; }
        if (PlacingOffset.x <= -MaxX)
        {
            if (BoxesAlongDownFaceOfStandingBoard() % 2 == 0) { PlacingOffset.x = 1 - MaxX; }
            else { PlacingOffset.x = -MaxX; }
        }

        if (PlacingOffset.y > MaxY) { PlacingOffset.y = MaxY; }
        if (PlacingOffset.y <= -MaxY)
        {
            if (CircuitBoardBeingPlaced.z % 2 == 0 && BoardIsFlat) { PlacingOffset.y = 1 - MaxY; }
            else { PlacingOffset.y = -MaxY; }
        }
    }

    private static int BoxesAlongDownFaceOfStandingBoard()
    {
        if (RotationStateEven) { return CircuitBoardBeingPlaced.z; }
        else { return CircuitBoardBeingPlaced.x; }
    }
    
    private static void ApplyPlacingOffset()
    {
        if (BoardIsFlat)
        {
            int ForwardMultiplier = 1;
            if (!RotationStateEven)
            {
                ForwardMultiplier = -1;
            }

            float ForwardScale = 0.3f;
            float RightScale = 0.3f;

            if (StuffPlacer.MostRecentPlacementWasOnBoard && StuffPlacer.MostRecentBoardPlacementWasOnEdge)
            {
                if (StuffPlacer.RotationAboutUpVector % 180 == 0) { RightScale = 0.075f; }
                else { ForwardScale = 0.075f; }
            }

            BoardBeingPlaced.transform.Translate(BoardBeingPlaced.transform.forward * PlacingOffset.y * ForwardScale * ForwardMultiplier, Space.World);
            BoardBeingPlaced.transform.Translate(BoardBeingPlaced.transform.right * PlacingOffset.x * RightScale, Space.World);
        }
        else
        {
            if (RotationStateEven)
            {
                BoardBeingPlaced.transform.Translate(BoardBeingPlaced.transform.forward * PlacingOffset.x * 0.3f, Space.World);
                if (StuffPlacer.MostRecentPlacementWasOnBoard && !StuffPlacer.MostRecentBoardPlacementWasOnEdge)
                {
                    BoardBeingPlaced.transform.Translate(BoardBeingPlaced.transform.up * PlacingOffset.y * 0.075f, Space.World);
                }
            }
            else
            {
                BoardBeingPlaced.transform.Translate(BoardBeingPlaced.transform.right * PlacingOffset.x * 0.3f, Space.World);
                if (StuffPlacer.MostRecentPlacementWasOnBoard && !StuffPlacer.MostRecentBoardPlacementWasOnEdge)
                {
                    BoardBeingPlaced.transform.Translate(BoardBeingPlaced.transform.up * PlacingOffset.y * 0.075f, Space.World);
                }
            }
        }
    }

    private static void SetRotationState()
    {
        // keep it within the range
        while (RotationState > 3)
        {
            RotationState -= 4;
        }
        while (RotationState < 0)
        {
            RotationState += 4;
        }

        // possible cases
        if (!BoardIsFlat)
        {
            if (RotationState == 0)
            {
                BoardBeingPlaced.transform.localEulerAngles = new Vector3(0, 0, 90); // makes the board stand up instead of laying down, so ReferenceObject.up points along the y axis of the board
                BoardBeingPlaced.transform.localPosition = new Vector3(0, 0, -(CircuitBoardBeingPlaced.z / 2) * 0.3f - 0.15f);
            }
            else if (RotationState == 1)
            {
                BoardBeingPlaced.transform.localEulerAngles = new Vector3(90, 0, 90);
                BoardBeingPlaced.transform.localPosition = new Vector3(0, CircuitBoardBeingPlaced.z * 0.3f, -(CircuitBoardBeingPlaced.x / 2) * 0.3f - 0.15f);
            }
            else if (RotationState == 2)
            {
                BoardBeingPlaced.transform.localEulerAngles = new Vector3(180, 0, 90);
                BoardBeingPlaced.transform.localPosition = new Vector3(0, CircuitBoardBeingPlaced.x * 0.3f, (CircuitBoardBeingPlaced.z / 2) * 0.3f + 0.15f);
            }
            else if (RotationState == 3)
            {
                BoardBeingPlaced.transform.localEulerAngles = new Vector3(270, 0, 90);
                BoardBeingPlaced.transform.localPosition = new Vector3(0, 0, (CircuitBoardBeingPlaced.x / 2) * 0.3f + 0.15f);
            }
            else
            {
                Debug.LogError("RotationState not found! State ID was " + RotationState);
            }
        }
        else
        {
            if (RotationStateEven)
            {
                BoardBeingPlaced.transform.localEulerAngles = new Vector3(0, 0, 0);
                BoardBeingPlaced.transform.localPosition = new Vector3((-CircuitBoardBeingPlaced.x / 2) * 0.3f - 0.15f, 0.075f, (-CircuitBoardBeingPlaced.z / 2) * 0.3f - 0.15f);
            }
            else
            {
                BoardBeingPlaced.transform.localEulerAngles = new Vector3(180, 0, 0);
                BoardBeingPlaced.transform.localPosition = new Vector3((-CircuitBoardBeingPlaced.x / 2) * 0.3f - 0.15f, 0.075f, (CircuitBoardBeingPlaced.z / 2) * 0.3f + 0.15f);

                // this is just to fix an edge case. I'm not really sure why it's necessary
                if (CircuitBoardBeingPlaced.z % 2 == 0)
                {
                    BoardBeingPlaced.transform.localPosition -= new Vector3(0, 0, 0.3f);
                }
            }
        }

        CapPlacingOffset(); // since the caps change when the rotationstate does
    }

    private static void PollForMirrorInput()
    {
        if (Input.GetButtonDown("MirrorBoard"))
        {
            if (Input.GetAxis("MirrorBoard") > 0) { MirrorBoard(MirrorAxis.x); }
            else { MirrorBoard(MirrorAxis.z); }
        }
    }

    private static void MirrorBoard(MirrorAxis axis)
    {
        if (BoardBeingPlaced.GetComponentsInChildren<CircuitBoard>().Length > 1) { return; } // everything will be screwed up if we have a board with other boards attached to it

        List<Transform> childcomponents = new List<Transform>();
        for (int i = 0; i < BoardBeingPlaced.transform.childCount; i++)
        {
            childcomponents.Add(BoardBeingPlaced.transform.GetChild(i));
        }

        foreach (Transform child in childcomponents)
        {
            if(axis == MirrorAxis.x)
            {
                float Xcenter = (CircuitBoardBeingPlaced.x * 0.3f) / 2;
                float DistanceFromCenter = Mathf.Abs(Xcenter - child.localPosition.x);
                float NewX = child.localPosition.x;

                if (child.transform.localPosition.x > Xcenter) { NewX = Xcenter - DistanceFromCenter; }
                else if (child.transform.localPosition.x < Xcenter) { NewX = Xcenter + DistanceFromCenter; }

                child.transform.localPosition = new Vector3(NewX, child.transform.localPosition.y, child.transform.localPosition.z);
                Quaternion oldrot = child.transform.localRotation;
                child.transform.localRotation = new Quaternion(-oldrot.x, oldrot.y, oldrot.z, -oldrot.w); // this is my first time really using quaternions. I have absolutely no idea how this works, I found it on google
            }
            else
            {
                float Zcenter = (CircuitBoardBeingPlaced.z * 0.3f) / 2;
                float DistanceFromCenter = Mathf.Abs(Zcenter - child.localPosition.z);
                float NewZ = child.localPosition.z;

                if (child.transform.localPosition.z > Zcenter) { NewZ = Zcenter - DistanceFromCenter; }
                else if (child.transform.localPosition.z < Zcenter) { NewZ = Zcenter + DistanceFromCenter; }

                child.transform.localPosition = new Vector3(child.transform.localPosition.x, child.transform.localPosition.y, NewZ);
                Quaternion oldrot = child.transform.localRotation;
                child.transform.localRotation = new Quaternion(oldrot.x, oldrot.y, -oldrot.z, -oldrot.w);
                child.transform.localEulerAngles += new Vector3(180, 0, 180); // why is the previous effect always off by 180 on two axes? I don't fucking know! Quaternions!!!!!!!!
            }

            StuffRotater.RedrawCircuitGeometryOf(BoardBeingPlaced);
        }
    }

    // done after StuffPlacer has done its own thing in PlaceThingBeingPlaced
    private static void PlaceBoard()
    {
        if (!StuffPlacer.OkayToPlace) { return; }

        StuffPlacer.PlaceThingBeingPlaced();

        BoardBeingPlaced.transform.parent = ReferenceObject.transform.parent; // StuffPlacer set referenceobject to the correct parent, but that should actually be BoardBeingPlaced's parent
        BoardBeingPlaced = null;
        ReferenceObject.transform.parent = null; // this is important because a circuitboard having any children means it can't be deleted, and this can fuck that up
        GameplayUIManager.UIState = UIState.None;
    }

    public static void CancelPlacement()
    {
        SoundPlayer.PlaySoundAt(Sounds.DeleteSomething, BoardBeingPlaced);

        BoardFunctions.SetMostRecentlyDeletedBoard(BoardBeingPlaced);

        DestroyBoardBeingPlaced();

        GameplayUIManager.UIState = UIState.None;
    }

    public static void DestroyBoardBeingPlaced()
    {
        ReferenceObject.transform.parent = null; // without this line the referenceobject is left on the board and you are unable to delete it without move board
        if (BoardBeingPlaced == null) { return; }

        // so that we don't end up with empty clusters when deleting boards.
        // this code is from stuffdeleter.cs. TODO: merge in a nice way
        CircuitInput[] inputs = BoardBeingPlaced.GetComponentsInChildren<CircuitInput>();
        CircuitOutput[] outputs = BoardBeingPlaced.GetComponentsInChildren<CircuitOutput>();
        foreach (CircuitInput input in inputs)
        {
            StuffDeleter.DestroyInput(input);
        }
        foreach (CircuitOutput output in outputs)
        {
            StuffDeleter.DestroyOutput(output);
        }

        BoardBeingPlaced.transform.parent = null; // this is done so that when StuffPlacer sets the BoxCollidersOfThingBeingPlaced it doesn't get the box collider from the old board. Maybe a better way would be DestroyImmediate but the documentation is really insistant that you don't use that so IDK. Man... the extent to which stuff in this codebase is interdependant on other stuff in unintuitive ways really bothers me. I'll need to refactor the refactor at this rate...

        Object.Destroy(BoardBeingPlaced);
        StuffPlacer.ResetReferences();
    }
}

public enum MirrorAxis
{
    x,
    z
}