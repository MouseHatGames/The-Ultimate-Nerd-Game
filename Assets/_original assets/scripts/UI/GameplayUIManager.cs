// manages all gameplay UI

using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public enum UIState
{
    // all of these are mutually exclusive.
    None, // also includes selection menu
    BoardMenu,
    NewBoardMenu,
    PaintBoardMenu,
    TextEditMenu,
    PauseMenuOrSubMenu, // could be pause menu, options menu, or about menu. In the future there will be a LOT of pause sub-menus
    BoardBeingPlaced,
    ChooseDisplayColor,
    NoisemakerMenu,
    SaveBoard,
    LoadBoard,
    StackBoardMenu,
    MainMenu // temporary
}

public static class GameplayUIManager
{
    private static UIState TrueUIState = UIState.None;
    public static UIState UIState
    {
        get { return TrueUIState; }
        set
        {
            TrueUIState = value;
            DeselectAllUIElements(); // With the current system, this should almost always be done when the state changes

            HelpMenu.ShowAppropriate(value);
            SetMouseLockAsAppropriate(value);
        }
    }

    // run every frame by BehaviorManager
    public static void RunGameplayUI()
    {
        if (Input.GetButtonDown("ToggleHelp")) { HelpMenu.ToggleHelp(); }

        switch (UIState)
        {
            case UIState.None:
                FirstPersonInteraction.DoInteraction();
                break;

            case UIState.BoardMenu:
                BoardMenu.Instance.RunBoardMenu();
                break;

            case UIState.NewBoardMenu:
                NewBoardMenu.Instance.RunNewBoardMenu();
                break;

            case UIState.PaintBoardMenu:
                PaintBoardMenu.Instance.RunPaintMenu();
                break;

            case UIState.TextEditMenu:
                TextEditMenu.Instance.RunTextMenu();
                break;

            case UIState.PauseMenuOrSubMenu:
                PauseMenu.Instance.RunPauseMenu();
                break;

            case UIState.BoardBeingPlaced:
                BoardPlacer.RunBoardPlacing();
                break;

            case UIState.ChooseDisplayColor:
                EditDisplayColorMenu.Instance.RunDisplayColorMenu();
                break;

            case UIState.NoisemakerMenu:
                NoisemakerMenu.Instance.RunNoisemakerMenu();
                break;

            case UIState.SaveBoard:
                SaveBoardMenu.Instance.RunSaveBoardMenu();
                break;

            case UIState.LoadBoard:
                LoadBoardMenu.Instance.RunLoadBoardMenu();
                break;

            case UIState.StackBoardMenu:
                StackBoardMenu.Instance.RunStackBoardMenu();
                break;

            // states without anything to do are not listed here
        }
    }

    // a couple of functions to consolidate input detection
    public static bool ScrollUp(bool Inverse = true)
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0) { return true; } // scrolling with the mouse
        if (Input.GetButtonDown("Scroll") && (Inverse ? Input.GetAxis("Scroll") > 0 : Input.GetAxis("Scroll") < 0)) { return true; } // using a keyboard key to scroll. These are seperate axes because you can't bind the mousewheel and buttons to the same axis :(
        return false;
    }

    public static bool ScrollDown(bool Inverse = true)
    {
        if (Input.GetAxis("Mouse ScrollWheel") < 0) { return true; }
        if (Input.GetButtonDown("Scroll") && (Inverse ? Input.GetAxis("Scroll") < 0 : Input.GetAxis("Scroll") > 0)) { return true; }
        return false;
    }

    /// <summary> -1 if no keys are down</summary>
    public static int NumberKey()
    {
        int num = -1;
        for (int i = 0; i < 10; i++)
        {
            if (Input.GetButtonDown(NumberKeyButtonNames[i]))
            {
                if (Input.GetButton("Mod")) { i += 10; } // so we effectively have up to 20 hotkeys

                num = i;
            }
        }

        return num;
    }

    // because creating new strings every frame actually is a LOT of garbage
    private static string[] NumberKeyButtonNames =
    {
        "Selection1", "Selection2", "Selection3", "Selection4", "Selection5", "Selection6", "Selection7", "Selection8", "Selection9", "Selection10"
    };

    public static void DeselectAllUIElements()
    {
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
    }

    // these do exactly what they say, for use when there is UI that you have to move your mouse around and click on stuff
    // TODO update when we do the FPS overhaul
    // TODO set these back to privaste
    private static void UnlockMouseAndDisableFirstPersonLooking()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (UIState == UIState.MainMenu) { return; }
        FirstPersonController.Instance.enabled = false;
    }

    private static void LockMouseAndEnableFirstPersonLooking()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        FirstPersonController.Instance.enabled = true;
    }

    private static void SetMouseLockAsAppropriate(UIState state)
    {
        if (state == UIState.NewBoardMenu ||
            state == UIState.PauseMenuOrSubMenu ||
            state == UIState.TextEditMenu ||
            state == UIState.ChooseDisplayColor ||
            state == UIState.NoisemakerMenu ||
            state == UIState.SaveBoard ||
            state == UIState.LoadBoard ||
            state == UIState.MainMenu)
        {
            UnlockMouseAndDisableFirstPersonLooking();
        }
        else
        {
            LockMouseAndEnableFirstPersonLooking();
        }
    }
}