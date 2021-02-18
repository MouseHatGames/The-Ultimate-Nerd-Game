using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditDisplayColorMenu : MonoBehaviour
{
    public static DisplayColor ColorOfNewDisplays = DisplayColor.Yellow;
    public Display DisplayBeingEdited = null;
    public Canvas MenuCanvas;

    public static EditDisplayColorMenu Instance;
    private void Awake()
    {
        MenuCanvas = GetComponent<Canvas>();
        Instance = this;
        ColorMenuAppropriately();
    }

    public void InitiateMenu()
    {
        MenuCanvas.enabled = true;
        GameplayUIManager.UIState = UIState.ChooseDisplayColor;
    }

    public void RunDisplayColorMenu()
    {
        if (Input.GetButtonDown("Cancel")) { DoneMenu(); }

        int numberkey = GameplayUIManager.NumberKey();
        if(numberkey != -1 && numberkey < 7)
        {
            SetNewDisplayColor((DisplayColor)numberkey + 1);
        }
    }

    public void DoneMenu()
    {
        MenuCanvas.enabled = false;
        GameplayUIManager.UIState = UIState.None;

        // this is so that if you change the display color and you're in the middle of placing a display, it gets set to the new color
        if (StuffPlacer.GetThingBeingPlaced == null) { return; }
        if (StuffPlacer.GetThingBeingPlaced.GetComponentInChildren<Display>()) { StuffPlacer.DeleteThingBeingPlaced(); }
    }

    public void SetNewDisplayColor(DisplayColor newcolor)
    {
        ColorOfNewDisplays = newcolor;
        DisplayBeingEdited.DisplayColor = newcolor;
        DisplayBeingEdited.ForceVisualRefresh();
        DoneMenu();
    }

    public void ButtonsClickThis(int ColorID)
    {
        SetNewDisplayColor((DisplayColor)ColorID);
    }

    public Selectable[] Buttons;
    public Image Background;
    private void ColorMenuAppropriately()
    {
        if (Background != null) { Background.color = Settings.DisplayOffColor; }

        ColorBlock Colors0 = Buttons[0].colors; // done like this because structs are passed as values, not references
        Colors0.normalColor = Settings.DisplayRedColor;
        Buttons[0].colors = Colors0;

        ColorBlock Colors1 = Buttons[1].colors;
        Colors1.normalColor = Settings.DisplayYellowColor;
        Buttons[1].colors = Colors1;

        ColorBlock Colors2 = Buttons[2].colors;
        Colors2.normalColor = Settings.DisplayBlueColor;
        Buttons[2].colors = Colors2;

        ColorBlock Colors3 = Buttons[3].colors;
        Colors3.normalColor = Settings.DisplayGreenColor;
        Buttons[3].colors = Colors3;

        ColorBlock Colors4 = Buttons[4].colors;
        Colors4.normalColor = Settings.DisplayOrangeColor;
        Buttons[4].colors = Colors4;

        ColorBlock Colors5 = Buttons[5].colors;
        Colors5.normalColor = Settings.DisplayPurpleColor;
        Buttons[5].colors = Colors5;

        ColorBlock Colors6 = Buttons[6].colors;
        Colors6.normalColor = Settings.DisplayWhiteColor;
        Buttons[6].colors = Colors6;

        for (int i = 0; i < 7; i++)
        {
            ColorBlock blockboi = Buttons[i].colors;
            blockboi.highlightedColor = blockboi.normalColor / 0.9f;
            blockboi.pressedColor = blockboi.normalColor / 0.8f;
            Buttons[i].colors = blockboi;
        }
    }
}

public enum DisplayColor
{
    Off,
    Red,
    Yellow,
    Blue,
    Green,
    Orange,
    Purple,
    White,
    Cyan,
}