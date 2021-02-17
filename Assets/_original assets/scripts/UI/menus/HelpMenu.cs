using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class HelpMenu : MonoBehaviour {

    public Canvas HelpCanvas;

    public TMPro.TextMeshProUGUI Content;

    public static HelpMenu Instance;

	// Use this for initialization
	void Start () {
        if (ES3.KeyExists("ShowHelp", "settings.txt")) { HelpCanvas.enabled = ES3.Load<bool>("ShowHelp", "settings.txt"); }
        else { HelpCanvas.enabled = true; }
        ShowDefault();

        Instance = this;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("ToggleHelp")) { ToggleHelp(); }
	}

    public void ToggleHelp()
    {
        HelpCanvas.enabled = !HelpCanvas.enabled;
        ES3.Save<bool>("ShowHelp", HelpCanvas.enabled, "settings.txt");
    }

    public static bool LockOpenMenu; // not used here, but checked in BoardPlacer.NewBoardBeingPlaced
    public void ShowDefault() { Content.text = Defalut; }
    public void ShowBoardMenu() { Content.text = BoardMenu; }
    public void ShowBoardPlacing() { Content.text = BoardPlacing; }
    public void ShowColorBoard() { Content.text = ColorBoard; }
    public void ShowNewBoard() { Content.text = NewBoard; }

    [ResizableTextArea] public string Defalut;

    [ResizableTextArea] public string BoardMenu;

    [ResizableTextArea] public string BoardPlacing;

    [ResizableTextArea] public string ColorBoard;

    [ResizableTextArea] public string NewBoard;                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     
}