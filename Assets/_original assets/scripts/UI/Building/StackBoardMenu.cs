using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityStandardAssets.Characters.FirstPerson;
using References;

public class StackBoardMenu : MonoBehaviour
{
    private int TrueIterations = 1;
    private int Iterations
    {
        get { return TrueIterations; }
        set
        {
            if (value == TrueIterations) { return; }

            TrueIterations = value;
            IterationsInput.text = value.ToString();
            if (value <= IterationsSlider.maxValue) { IterationsSlider.value = value; } // check is so you can manually enter values beyond the slider's max value

            IterationsChanged();
        }
    }

    public Slider IterationsSlider;
    public TMP_InputField IterationsInput;

    public Canvas Canvas;

    public void OnSliderChange()
    {
        Iterations = (int)IterationsSlider.value;
    }

    public void OnInputChange()
    {
        if (IterationsInput.text == "") { return; }
        Iterations = TextToValidIterationsNumber(IterationsInput.text);
    }

    private static int TextToValidIterationsNumber(string text)
    {
        int iterations = 1;
        if (text == "" || text == "-") { return 1; } else { iterations = int.Parse(text); }
        iterations = Mathf.Abs(iterations); // negative value boards are terrible don't let them exist
        if (iterations > 1000) { iterations = 1000; } // bigger boards crash the game and are glitchy in general
        if (iterations < 1) { iterations = 1; }
        return iterations;
    }

    public void Initialize()
    {
        RaycastHit hit;
        if (Physics.Raycast(FirstPersonInteraction.Ray(), out hit, Settings.ReachDistance, Wire.IgnoreWiresLayermask))
        {
            if (hit.collider.tag == "CircuitBoard")
            {
                GameObject StackThis = hit.collider.gameObject;
                if (Input.GetButton("Mod"))
                {
                    GameObject RootBoard = hit.collider.transform.root.gameObject;
                    if (RootBoard.tag == "CircuitBoard") { StackThis = RootBoard; } // make sure to check for circuitboard in case of mounts
                }

                BoardBeingStacked = StackThis;
                BoardBeingStackedCircuitBoard = StackThis.GetComponent<CircuitBoard>();
                AllSubBoardsInvolvedWithStacking.Add(BoardBeingStacked);
                BoardBeingStacked.AddComponent<cakeslice.Outline>(); // added before we get the copy so clones will have it too
                BoardBeingStackedCopy = Instantiate(BoardBeingStacked, new Vector3(-1000, -1000, -1000), BoardBeingStacked.transform.rotation);

                GameplayUIManager.UIState = UIState.StackBoardMenu;
                Canvas.enabled = true;
                IterationsInput.ActivateInputField();

                StuffPlacer.SetObjectOutlineColor(BoardBeingStacked, OutlineColor.blue);

                // so that you don't accidentally crash your PC when you first stacked on a simple board then you go to stack on a complex board. Don't set iterations directly for
                // the reason outlined below
                TrueIterations = 1;

                // Using Invoke because for some reason this MUST be done a frame later. If it is not, outputs have duplicated geometry
                Invoke("ShittyHackThatShouldntExist", 0.01f);
                return;
            }
        }

        SoundPlayer.PlaySoundGlobal(Sounds.FailDoSomething);
        GameplayUIManager.UIState = UIState.None;
    }

    private void ShittyHackThatShouldntExist()
    {
        IterationsSlider.value = 1;
        IterationsInput.text = "1";
        IterationsChanged();
    }

    private GameObject BoardBeingStacked; // the original
    private CircuitBoard BoardBeingStackedCircuitBoard; // of the original

    private GameObject BoardBeingStackedCopy;

    // the clone
    private GameObject StackedBoard
    {
        get
        {
            if (AllSubBoardsInvolvedWithStacking.Count > 1) { return AllSubBoardsInvolvedWithStacking[1]; }
            else { return null; }
        }
    }

    private bool CurrentPlacementIsValid;

    public void RunStackBoardMenu()
    {
        if (Input.GetButtonDown("Cancel") || Input.GetButtonDown("Delete")) { Done(); }
        if (Input.GetButtonDown("Confirm") || Input.GetButtonDown("BoardMenu") || Input.GetButtonDown("Place")) { Place(); }

        if (GameplayUIManager.ScrollUp(false)) { IterationsSlider.value++; }
        if (GameplayUIManager.ScrollDown(false)) { IterationsSlider.value--; }

        PollForStackingDirectionInput();
    }

    //private void TestForNewShit()
    //{
    //    Quaternion TestRotation = Quaternion.LookRotation(Camera.main.transform.forward, BoardBeingStacked.transform.up);
    //    float YRotation = TestRotation.eulerAngles.y - BoardBeingStacked.transform.eulerAngles.y;
    //    while (YRotation < 000) { YRotation += 360; }
    //    while (YRotation > 360) { YRotation -= 360; }

    //    int RoundedTo90 = Mathf.RoundToInt(YRotation / 90);
    //    Debug.Log(RoundedTo90);
    //}

    int StackingDirection = 1;
    private void PollForStackingDirectionInput()
    {
        if (Input.GetButtonDown("BoardRotation"))
        {
            if(Input.GetAxis("BoardRotation") > 0)
            {
                StackingDirection++;
            }
            else
            {
                StackingDirection--;
            }

            // keep it within range
            while (StackingDirection < 0) { StackingDirection += 4; }
            while (StackingDirection > 3) { StackingDirection -= 4; }

            SetAllBoardPositions();
            MegaMeshManager.RemoveComponentsImmediatelyIn(StackedBoard);
            MegaMeshManager.AddComponentsIn(StackedBoard);
        }
    }

    private Vector3 WorldspaceSingleIterationDisplacement()
    {
        Vector3 WorldspaceSingleIterationDisplacement = BoardBeingStacked.transform.forward * BoardBeingStackedCircuitBoard.z * 0.3f;
        if (StackingDirection == 0) { WorldspaceSingleIterationDisplacement = BoardBeingStacked.transform.forward * BoardBeingStackedCircuitBoard.z * 0.3f; }
        else if (StackingDirection == 1) { WorldspaceSingleIterationDisplacement = BoardBeingStacked.transform.right * BoardBeingStackedCircuitBoard.x * 0.3f; }
        else if (StackingDirection == 2) { WorldspaceSingleIterationDisplacement = -BoardBeingStacked.transform.forward * BoardBeingStackedCircuitBoard.z * 0.3f; }
        else if (StackingDirection == 3) { WorldspaceSingleIterationDisplacement = -BoardBeingStacked.transform.right * BoardBeingStackedCircuitBoard.x * 0.3f; }
        else Debug.LogError("Stacking direction ID not found - ID was " + StackingDirection.ToString());

        return WorldspaceSingleIterationDisplacement;
    }

    // this includes the original board. So at index 0 it'll be BoardBeingStacked, and everything after that is part of the stack
    [SerializeField] private List<GameObject> AllSubBoardsInvolvedWithStacking = new List<GameObject>();

    private void SetAllBoardPositions()
    {
        Vector3 DirectionAndDistance = WorldspaceSingleIterationDisplacement();

        for (int i = 0; i < AllSubBoardsInvolvedWithStacking.Count; i++)
        {
            AllSubBoardsInvolvedWithStacking[i].transform.position = BoardBeingStacked.transform.position + DirectionAndDistance * i;
        }

        UpdateHighlight();
    }

    private void IterationsChanged()
    {
        if (Iterations >= AllSubBoardsInvolvedWithStacking.Count)
        {
            int NumberOfNewBoards = Iterations + 1 - AllSubBoardsInvolvedWithStacking.Count;
            for (int i = 0; i < NumberOfNewBoards; i++)
            {
                GameObject NewBoard = Instantiate(BoardBeingStackedCopy);
                NewBoard.transform.position = new Vector3(1000, -1000, 1000); // so RecalculateCluster can work unhindered
                NewBoard.transform.parent = AllSubBoardsInvolvedWithStacking[AllSubBoardsInvolvedWithStacking.Count - 1].transform;

                StuffPlacer.SetStateOfAllBoxCollidersIn(NewBoard, false);

                AllSubBoardsInvolvedWithStacking.Add(NewBoard);

                BoardFunctions.RecalculateClustersOfBoard(NewBoard);

                MegaMeshManager.RemoveComponentsImmediatelyIn(NewBoard);
                MegaMeshManager.AddComponentsIn(NewBoard);
            }
        }
        else if (Iterations < AllSubBoardsInvolvedWithStacking.Count)
        {
            //int NumberOfExtraBoards = AllSubBoardsInvolvedWithStacking.Count + 1 - Iterations;
            for(int i = AllSubBoardsInvolvedWithStacking.Count - 1; i > Iterations; i--)
            {
                GameObject oldboard = AllSubBoardsInvolvedWithStacking[i];
                AllSubBoardsInvolvedWithStacking.Remove(oldboard);
                oldboard.transform.parent = null; // because the board is not destroyed immediately, this is done to prevent the old board from being used in the interesction test of UpdateHighlight
                Destroy(oldboard);
            }
        }

        SetAllBoardPositions();
    }

    private void UpdateHighlight()
    {
        if (StuffPlacer.GameObjectIntersectingStuffOrWouldDestroyWires(StackedBoard))
        {
            CurrentPlacementIsValid = false;
            StuffPlacer.SetObjectOutlineColor(StackedBoard, OutlineColor.red);
        }
        else
        {
            CurrentPlacementIsValid = true;
            StuffPlacer.SetObjectOutlineColor(StackedBoard, OutlineColor.green);
        }
    }

    public void Place()
    {
        if (!CurrentPlacementIsValid)
        {
            SoundPlayer.PlaySoundGlobal(Sounds.FailDoSomething);
            return;
        }

        StuffPlacer.RemoveOutlineFromObject(BoardBeingStacked);
        StuffPlacer.SetStateOfAllBoxCollidersIn(BoardBeingStacked, true);

        FloatingPointRounder.RoundIn(BoardBeingStacked);
        SnappingPeg.TryToSnapIn(BoardBeingStacked);

        MegaMeshManager.AddComponentsIn(StackedBoard);
        foreach (VisualUpdaterWithMeshCombining visualboi in StackedBoard.GetComponentsInChildren<VisualUpdaterWithMeshCombining>())
        {
            visualboi.AllowedToCombineOnStable = true;
        }

        SoundPlayer.PlaySoundAt(Sounds.PlaceOnBoard, BoardBeingStacked);

        BoardBeingStacked = null;
        BoardBeingStackedCircuitBoard = null;

        AllSubBoardsInvolvedWithStacking = new List<GameObject>();

        Done();
    }

    public void Done()
    {
        StuffPlacer.RemoveOutlineFromObject(BoardBeingStacked);
        StuffPlacer.SetStateOfAllBoxCollidersIn(BoardBeingStacked, true);

        if (StackedBoard != null)
        {
            SoundPlayer.PlaySoundAt(Sounds.DeleteSomething, StackedBoard);
            Destroy(StackedBoard);
        }

        BoardBeingStacked = null;
        Destroy(BoardBeingStackedCopy);

        AllSubBoardsInvolvedWithStacking = new List<GameObject>();
        Canvas.enabled = false;
        GameplayUIManager.UIState = UIState.None;
    }

    public static StackBoardMenu Instance;
    private void Awake()
    {
        Instance = this;
    }

    public void DestroyStuffIfAppropriate()
    {
        if (BoardBeingStackedCopy != null) { DestroyImmediate(BoardBeingStackedCopy); }
        if (StackedBoard != null) { DestroyImmediate(StackedBoard); }
    }
}