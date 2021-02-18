// a base class from which side-scrolling menus - the selection menu, the board menu, and the paint board menu - derive from.
// fun fact, this (refactoring the UI code for 0.2) is my very first time using class inheritance. Apparently, it's an essential
// feature of OOP, but I literally haven't used it once in 6 months of making games.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalScrollMenuWithSelection : MonoBehaviour // monobehavior since the classes that inherit this one need to be attached to gameobjects
{
    public int SelectedThing;
    public int MaxSelectedThing;

    public RectTransform Selection; // the little square thing that moves when you scroll

    public Canvas Canvas;

    public bool SelectedThingJustChanged; // used for when action is needed when the player scrolls (i.e. opening a menu). True during a single frame when UpdateSelectedThing has been run
    // TODO change to an abstract void now that I know what those are :P

    public void ScrollThroughMenu()
    {
        SelectedThingJustChanged = false;

        // move selected thing
        if (GameplayUIManager.ScrollUp())
        {
            if (SelectedThing > 0)
            {
                SelectedThing--; // go to the previous thing
            }
            else
            {
                SelectedThing = MaxSelectedThing; // allows the scrolling to loop
            }
            UpdateSelectedThing();
        }

        if (GameplayUIManager.ScrollDown())
        {
            if (SelectedThing < MaxSelectedThing)
            {
                SelectedThing++; // go to the next thing
            }
            else
            {
                SelectedThing = 0; // allows the scrolling to loop
            }
            UpdateSelectedThing();
        }

        // number key selection
        int selectkey = GameplayUIManager.NumberKey();
        if(selectkey != -1 && selectkey <= MaxSelectedThing)
        {
            SelectedThing = selectkey;
            UpdateSelectedThing();
        }
    }

    public Vector2 StartingPosition;
    public float DistanceBetweenPositions;
    public void UpdateSelectedThing()
    {
        Selection.anchoredPosition = new Vector2(StartingPosition.x + DistanceBetweenPositions * SelectedThing, StartingPosition.y);
        SelectedThingJustChanged = true;
    }
}
