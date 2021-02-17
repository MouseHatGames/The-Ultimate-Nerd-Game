using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class PaintBoardMenu : MonoBehaviour {

    [ReorderableList]
    public List<Color> Colors;

    // the rectangular object that gets colored to form the selection menu
    public GameObject ColorRectangle;

    public Canvas PaintCanvas;

    public RectTransform Selection;

	// Use this for initialization
	void Start () {
        GenerateMenu();
	}

    // turn the menu on, prevent other menus from opening
    private void OnEnable()
    {
        PaintCanvas.enabled = true;
        UIManager.SomeOtherMenuIsOpen = true;
    }

    public void GenerateMenu()
    {
        List<Color> menucolors = Colors;
        if(!ES3.KeyExists("PaintColors", "settings.txt"))
        {
            ES3.Save<List<Color>>("PaintColors", Colors, "settings.txt");
        }
        else
        {
            menucolors = ES3.Load<List<Color>>("PaintColors", "settings.txt", Colors);
        }

        for (int i = 0; i < menucolors.Count; i++) // we use this instead of foreach because a number is used for position
        {
            // create it and make it a child of the menu
            GameObject boob = Instantiate(ColorRectangle, PaintCanvas.transform);

            // set its position
            RectTransform RectTransform = boob.GetComponent<RectTransform>();
            RectTransform.anchoredPosition = new Vector3(47.5f + i * 110, 47.5f, 0);

            // set its color
            Image image = boob.GetComponent<Image>();
            image.color = menucolors[i];
        }
    }

    int SelectedThing = 0;
    // Update is called once per frame
    void Update () {

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
            if (SelectedThing < Colors.Count - 1) // so you can't scroll beyond the end
            {
                SelectedThing++; // go to the next thing
                UpdateSelectedThing();
            }
        }

        // create the menu if space or enter are pressed
        if (Input.GetButtonDown("Jump") || Input.GetButtonDown("Submit") || Input.GetButtonDown("BoardMenu")) // the last check is so you can place the same new board by double tapping v
        {
            ColorBoard();
        }

        // cancel if esc is pressed
        if (Input.GetButtonDown("Cancel"))
        {
            Cancel();
        }
    }

    void UpdateSelectedThing()
    {
        Selection.anchoredPosition = new Vector2(40 + 110 * SelectedThing, 40);
    }

    void ColorBoard()
    {
        RaycastHit hit;
        Transform cam = FirstPersonInteraction.FirstPersonCamera.transform;

        if (Physics.Raycast(cam.position, cam.forward, out hit, MiscellaneousSettings.ReachDistance))
        {
            if (hit.collider.tag == "CircuitBoard")
            {
                hit.collider.GetComponent<CircuitBoard>().SetBoardColor(Colors[SelectedThing]);
            }
        }

        Cancel();
    }

    void Cancel()
    {
        PaintCanvas.enabled = false;
        UIManager.SomeOtherMenuIsOpen = false;
        enabled = false;

        Input.ResetInputAxes(); // so that if V is used to place the board (a double tap), the board menu is not opened again

        HelpMenu.Instance.ShowDefault();
    }
}
