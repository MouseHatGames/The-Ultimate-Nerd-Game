// manages the two gameplay menus, the object placement menu and the board manager menu

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class UIManager : MonoBehaviour {

    // references to the instances of the scripts that run the respective menus
    public BuildMenu buildmenu;
    public BoardMenu boardmenu;

    public static bool SomeOtherMenuIsOpen = false; // miscellaneous menus like the label menu, the load board menu, the new board menu

	// Use this for initialization
	void Start () {
        SomeOtherMenuIsOpen = false;
	}

	// Update is called once per frame
	void Update () {

        if (SomeOtherMenuIsOpen)
        {
            buildmenu.MenuLockedOpen = false;
            if (buildmenu.MenuCanvas.gameObject.activeInHierarchy) { buildmenu.MenuCanvas.gameObject.SetActive(false); } // the check, while not strictly necessary, reduces garbage collection
            buildmenu.MenuOpenTime = MiscellaneousSettings.PlaceMenuOpenTime + 100;

            boardmenu.BoardMenuCanvas.enabled = false;
            return;
        }

        if (BoardPlacer.BoardBeingPlaced != null) { return; } // can't do the board menu or the build menu if a board placing is in progress

        if (Input.GetButtonDown("BoardMenu"))
        {
            boardmenu.BoardMenuCanvas.enabled = true;

            buildmenu.MenuLockedOpen = false;
            if (buildmenu.MenuCanvas.gameObject.activeInHierarchy) { buildmenu.MenuCanvas.gameObject.SetActive(false); }
            buildmenu.MenuOpenTime = MiscellaneousSettings.PlaceMenuOpenTime + 100;

            HelpMenu.Instance.ShowBoardMenu();
        }

        if (Input.GetButtonUp("BoardMenu"))
        {
            boardmenu.BoardMenuCanvas.enabled = false;
            boardmenu.ExecuteSelectedAction();
        }

        if (Input.GetButton("BoardMenu"))
        {
            boardmenu.RunBoardMenu();
        }
        else
        {
            buildmenu.RunBuildMenu();
        }
	}

    public static void UnlockMouseAndDisableFirstPersonLooking()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        FirstPersonController.Instance.enabled = false;
    }

    public static void LockMouseAndEnableFirstPersonLooking()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        FirstPersonController.Instance.enabled = true;
    }
}