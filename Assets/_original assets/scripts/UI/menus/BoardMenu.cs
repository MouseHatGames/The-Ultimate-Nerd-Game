// runs the board menu

using UnityEngine;

public class BoardMenu : HorizontalScrollMenuWithSelection
{
    public static BoardMenu Instance;
    private void Awake() { Instance = this; }

    public static BoardMenuMode MenuMode = BoardMenuMode.HoldThenRelease;

    // run on the frame when V is pressed
    public void InitializeBoardMenu()
    {
        GameplayUIManager.UIState = UIState.BoardMenu;
        Canvas.enabled = true;
    }

    // run every frame that the board menu is active
    public void RunBoardMenu()
    {
        ScrollThroughMenu();
        OutlineLookedAtBoardIfAppropriate();

        if (MenuMode == BoardMenuMode.HoldThenRelease)
        {
            if (Input.GetButtonUp("BoardMenu")) // when you release V which was being held down
            {
                ExecuteSelectedAction();
            }
        }
        else if (MenuMode == BoardMenuMode.TapTwice)
        {
            if (Input.GetButtonDown("BoardMenu")) // when you tap V for the second time
            {
                ExecuteSelectedAction();
            }
        }

        if (Input.GetButtonDown("Place")) { ExecuteSelectedAction(); }
        if (Input.GetButtonDown("Cancel")) { Done(); GameplayUIManager.UIState = UIState.None; }
    }

    private void OutlineLookedAtBoardIfAppropriate()
    {
        if (SelectedThing == 0 || SelectedThing == 1 || SelectedThing == 5 || SelectedThing == 7) // canccel, new board, paint board, load board
        {
            RemoveOutlineFromLookedAtBoard();
            return;
        }

        HighlightLookedAtBoard();
    }

    private GameObject highlightedboard;
    private void HighlightLookedAtBoard()
    {
        RaycastHit hit;
        if(Physics.Raycast(FirstPersonInteraction.Ray(), out hit, Settings.ReachDistance, Wire.IgnoreWiresLayermask))
        {
            if (hit.collider.tag == "CircuitBoard")
            {
                GameObject highlightthis = hit.collider.gameObject;
                if (Input.GetButton("Mod"))
                {
                    GameObject RootObject = hit.collider.transform.root.gameObject;
                    if(RootObject.tag == "CircuitBoard") // protection from mounts
                    {
                        highlightthis = hit.collider.transform.root.gameObject;
                    }
                }
                if(highlightthis != highlightedboard)
                {
                    RemoveOutlineFromLookedAtBoard();
                    highlightedboard = highlightthis;
                    StuffPlacer.OutlineObject(highlightedboard, OutlineColor.blue);
                }
            }
            else
            {
                RemoveOutlineFromLookedAtBoard();
            }
        }
        else
        {
            RemoveOutlineFromLookedAtBoard();
        }
    }

    private void RemoveOutlineFromLookedAtBoard()
    {
        if (highlightedboard == null) { return; }
        StuffPlacer.RemoveOutlineFromObject(highlightedboard, true);
        highlightedboard = null;
    }

    // triggered when the board menu key is released by GameplayUIManager
    private void ExecuteSelectedAction()
    {
        Done();

        if (SelectedThing == 0) // cancel
        {
            GameplayUIManager.UIState = UIState.None;
        }
        else if (SelectedThing == 1) // new board
        {
            NewBoardMenu.Instance.Initialize();
        }
        else if (SelectedThing == 2) // move board
        {
            BoardFunctions.MoveExistingBoard();
        }
        else if (SelectedThing == 3) // clone board
        {
            BoardFunctions.CloneBoard();
        }
        else if (SelectedThing == 4) // stack board
        {
            StackBoardMenu.Instance.Initialize();
        }
        else if (SelectedThing == 5) // paint board
        {
            PaintBoardMenu.Instance.Initialize();
        }
        else if (SelectedThing == 6) // save board
        {
            SaveBoardMenu.SaveBoard();
        }
        else if (SelectedThing == 7) // load board
        {
            LoadBoardMenu.Instance.Initialize();
        }
        else
        {
            Debug.LogError("tried to execute invalid action ID: " + SelectedThing);
            GameplayUIManager.UIState = UIState.None;
        }
    }

    private void Done()
    {
        Canvas.enabled = false;
        RemoveOutlineFromLookedAtBoard();
    }
}

public enum BoardMenuMode
{
    HoldThenRelease,
    TapTwice
}