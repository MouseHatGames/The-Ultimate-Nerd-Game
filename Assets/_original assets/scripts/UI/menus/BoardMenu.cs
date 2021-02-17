using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardMenu : MonoBehaviour {

    public RectTransform Selection;

    public int SelectedThing;
    public int MaxSelectedThing;

    public Canvas BoardMenuCanvas;

    public void RunBoardMenu()
    {
        // move selected thing
        if (Input.GetAxis("Mouse ScrollWheel") > 0 || BuildMenu.KeyboardScrollUp()) // scroll up
        {
            if (SelectedThing > 0) // so you can't scroll past None
            {
                SelectedThing--; // go to the previous thing
                UpdateSelectedThing();
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0 || BuildMenu.KeyboardScrollDown()) // scroll down
        {
            if (SelectedThing < MaxSelectedThing) // so you can't scroll beyond the end
            {
                SelectedThing++; // go to the next thing
                UpdateSelectedThing();
            }
        }

        // checks for number key selection
        // I hope this method isn't super laggy but something tells me that it is. In the short term, however, fuck it, because I hate copy-paste coding.
        for (int i = 0; i < 10; i++)
        {
            if (Input.GetButtonDown("Selection" + (i + 1).ToString()))
            {
                if (i < MaxSelectedThing + 1)
                {
                    SelectedThing = i;
                    UpdateSelectedThing();
                }
            }
        }
    }

    public void UpdateSelectedThing()
    {
        float PositionX = 40;
        PositionX = 40 + 160 * SelectedThing;
        Selection.anchoredPosition = new Vector2(PositionX, 40);
    }

    public NewBoardMenu newboardmenu;
    public BoardPlacer boardplacer;
    public PaintBoardMenu paintboardmenu;
    // triggered when the board menu key is released by UImanager
    public void ExecuteSelectedAction()
    {
        if(SelectedThing == 0) // cancel
        {
            HelpMenu.Instance.ShowDefault();

            return;
        }
        else if(SelectedThing == 1) // new board
        {
            newboardmenu.enabled = true;

            HelpMenu.Instance.ShowNewBoard();
            HelpMenu.LockOpenMenu = true; // so that BoardPlacer.NewBoardBeingPlaced doesn't fuck the help menu up
        }
        else if(SelectedThing == 2) // move board
        {
            boardplacer.MoveExistingBoard();
        }
        else if(SelectedThing == 3) // clone board
        {
            boardplacer.CloneBoard();
        }
        else if(SelectedThing == 4) // paint board
        {
            paintboardmenu.enabled = true;

            HelpMenu.Instance.ShowColorBoard();
        }
        else
        {
            Debug.Log("tried to execute an invalid action ID");
        }
    }
}
