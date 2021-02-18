// creates and runs the paint board menu

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class PaintBoardMenu : HorizontalScrollMenuWithSelection
{
    public static PaintBoardMenu Instance;
    private void Awake() { Instance = this; }

    [ReorderableList]
    public List<Color> Colors;

    // this is a prefab, the rectangular object that gets colored to form the selection menu
    public GameObject ColorRectangle;

    void Start()
    {
        GenerateMenu();
    }

    public void GenerateMenu()
    {
        Colors = Settings.Get("PaintColors", Colors);
        MaxSelectedThing = Colors.Count - 1;

        for (int i = 0; i < Colors.Count; i++)
        {
            // create it and make it a child of the menu
            GameObject boob = Instantiate(ColorRectangle, Canvas.transform);

            // set its position
            RectTransform RectTransform = boob.GetComponent<RectTransform>();
            RectTransform.anchoredPosition = new Vector3(45f + i * DistanceBetweenPositions, 45f, 0);

            // set its color
            Image image = boob.GetComponent<Image>();
            image.color = Colors[i];
        }
    }

    // run every frame that the paint menu is active
    public void RunPaintMenu()
    {
        ScrollThroughMenu();
        if (SelectedThingJustChanged) { RemoveOutlineFromLookedAtBoard(); }
        HighlightLookedAtBoard();

        if (Input.GetButtonDown("Jump") || Input.GetButtonDown("Submit") || Input.GetButtonDown("Place") || Input.GetButtonDown("BoardMenu")) // the last check is so you can place the same new board by double tapping v
        {
            ColorBoard();
        }

        // cancel if esc is pressed
        if (Input.GetButtonDown("Cancel"))
        {
            Done();
        }
    }

    void ColorBoard()
    {
        RemoveOutlineFromLookedAtBoard();

        RaycastHit hit;
        if (Physics.Raycast(FirstPersonInteraction.Ray(), out hit, Settings.ReachDistance, Wire.IgnoreWiresLayermask))
        {
            if (hit.collider.tag == "CircuitBoard")
            {
                hit.collider.GetComponent<CircuitBoard>().SetBoardColor(Colors[SelectedThing]);
            }
        }

        Done();
    }

    private GameObject lookingatboard;
    private Color previouscoloroflookedatboard;

    private void HighlightLookedAtBoard()
    {
        RaycastHit hit;
        if (Physics.Raycast(FirstPersonInteraction.Ray(), out hit, Settings.ReachDistance, Wire.IgnoreWiresLayermask))
        {
            if (hit.collider.tag == "CircuitBoard")
            {
                if (hit.collider.gameObject != lookingatboard)
                {
                    RemoveOutlineFromLookedAtBoard();
                    previouscoloroflookedatboard = hit.collider.GetComponent<CircuitBoard>().GetBoardColor;
                    lookingatboard = hit.collider.gameObject;
                    lookingatboard.AddComponent<cakeslice.Outline>().color = 1;
                    lookingatboard.GetComponent<CircuitBoard>().SetBoardColor(Colors[SelectedThing]);
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
        if (lookingatboard == null) { return; }
        lookingatboard.GetComponent<CircuitBoard>().SetBoardColor(previouscoloroflookedatboard);
        Destroy(lookingatboard.GetComponent<cakeslice.Outline>());
        lookingatboard = null;
    }

    public void Initialize()
    {
        Canvas.enabled = true;
        GameplayUIManager.UIState = UIState.PaintBoardMenu;
    }

    void Done()
    {
        RemoveOutlineFromLookedAtBoard();
        Canvas.enabled = false;
        GameplayUIManager.UIState = UIState.None;
    }
}
