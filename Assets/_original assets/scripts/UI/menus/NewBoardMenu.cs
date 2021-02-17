using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewBoardMenu : MonoBehaviour {

    // all the components of the new board menu
    public Slider SizeXSlider;
    public Slider SizeYSlider;
    public TMP_InputField SizeXInput;
    public TMP_InputField SizeYInput;

    public Canvas NewBoardCanvas;

    public int SizeX;
    public int SizeY;

    public BoardPlacer boardplacer;

	// Use this for initialization
	void Start () {
        // to make sure the UI doesn't have any errors or whatever at startup
        SizeXSlider.value = SizeX;
        SizeYSlider.value = SizeY;

        SizeXInput.text = SizeX.ToString();
        SizeYInput.text = SizeY.ToString();
	}

    // turn the menu on, prevent other menus from opening
    private void OnEnable()
    {
        NewBoardCanvas.enabled = true;
        UIManager.SomeOtherMenuIsOpen = true;
        UIManager.UnlockMouseAndDisableFirstPersonLooking();

        // select the size x input field so that the user can immediately begin typing, if they so choose
        SizeXInput.ActivateInputField();
    }

    int CachedSizeX;
    int CachedSizeY;
    private void Update()
    {
        // caching system makes sure a new board is only generated on frames where it matters
        if(CachedSizeX != SizeX || CachedSizeY != SizeY)
        {
            boardplacer.CreateNewBoard(SizeX, SizeY);
        }
        CachedSizeX = SizeX;
        CachedSizeY = SizeY;

        // create the menu if space or enter are pressed
        if (Input.GetButtonDown("Jump") || Input.GetButtonDown("Submit") || Input.GetButtonDown("BoardMenu")) // the last check is so you can place the same new board by double tapping v
        {
            OnPlaceBoardButtonPress();
        }

        // cancel if esc is pressed
        if (Input.GetButtonDown("Cancel"))
        {
            BoardPlacer.CancelPlacement();
            OnPlaceBoardButtonPress();
        }

        // tab between input fields
        if (Input.GetButtonDown("tab"))
        {
            if (SizeXInput.isFocused)
            {
                SizeYInput.ActivateInputField();
            }
            else
            {
                SizeXInput.ActivateInputField();
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0 || BuildMenu.KeyboardScrollUp()) // scroll up
        {
            if (Input.GetButton("Mod"))
            {
                SizeXSlider.value += 1;
            }
            else
            {
                SizeYSlider.value += 1;
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0 || BuildMenu.KeyboardScrollDown()) // scroll down
        {
            if (Input.GetButton("Mod"))
            {
                SizeXSlider.value -= 1;
            }
            else
            {
                SizeYSlider.value -= 1;
            }
        }
    }

    public void OnXSliderUpdate()
    {
        SizeX = (int)SizeXSlider.value; // because the slider works in multiples of 5, but you can only assign it to snap to the nearest 1
        SizeXInput.text = SizeX.ToString();
    }

    public void OnYSliderUpdate()
    {
        SizeY = (int)SizeYSlider.value; // because the slider works in multiples of 5, but you can only assign it to snap to the nearest 1
        SizeYInput.text = SizeY.ToString();
    }

    // ok the enxt to methods are getting really messy. TODO: clean up
    public void OnXTextUpdate()
    {
        int NewSizeX;
        if (SizeXInput.text == "") { NewSizeX = 1; } else { NewSizeX = int.Parse(SizeXInput.text); }
        if (NewSizeX < 0) { NewSizeX = Mathf.Abs(NewSizeX); SizeXInput.text = NewSizeX.ToString(); } // negative value boards are terrible don't let them exist
        if (NewSizeX > 1000) { SizeXInput.text = "1000"; } // bigger boards crash the game and are glitchy in general
        if (NewSizeX < 1) { SizeXInput.text = "1"; NewSizeX = 1; }
        SizeX = NewSizeX;
        if (SizeX <= SizeXSlider.maxValue) { SizeXSlider.value = SizeX; } // this check is so that you can manually enter numbers bigger than the slider's max
    }

    public void OnYTextUpdate()
    {
        int NewSizeY;
        if (SizeYInput.text == "") { NewSizeY = 1; } else { NewSizeY = int.Parse(SizeYInput.text); }
        if (NewSizeY < 0) { NewSizeY = Mathf.Abs(NewSizeY); SizeXInput.text = NewSizeY.ToString(); } // negative value boards are terrible don't let them exist
        NewSizeY = Mathf.Abs(NewSizeY); // negative value boards are terrible don't let them exist
        if (NewSizeY > 1000) { SizeYInput.text = "1000"; } // bigger boards crash the game and are glitchy in general
        if (NewSizeY < 1) { SizeYInput.text = "1"; NewSizeY = 1; }
        SizeY = NewSizeY;
        if (SizeY <= SizeYSlider.maxValue) { SizeYSlider.value = SizeY; } // this check is so that you can manually enter numbers bigger than the slider's max
    }

    public void OnPlaceBoardButtonPress()
    {
        // shitty hack to make sure a new board is generated on the next new board menu open. Works by tricking the caching system into thinking SizeX has changed
        CachedSizeX = -69696969;

        // close the menu and disable the script
        NewBoardCanvas.enabled = false;
        UIManager.SomeOtherMenuIsOpen = false;
        UIManager.LockMouseAndEnableFirstPersonLooking();
        enabled = false;

        Input.ResetInputAxes(); // so that if V is used to place the board (a double tap), the board menu is not opened again

        HelpMenu.LockOpenMenu = false;
        HelpMenu.Instance.ShowBoardPlacing();
    }
}