// runs the new board menu, though not the functions for actually creating a new board

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewBoardMenu : MonoBehaviour
{
    public static NewBoardMenu Instance;
    private void Awake() { Instance = this; }

    // all the components of the new board menu
    public Slider SizeXSlider;
    public Slider SizeYSlider;
    public TMP_InputField SizeXInput;
    public TMP_InputField SizeYInput;

    public Canvas NewBoardCanvas;

    public int TrueSizeX;
    public int TrueSizeY;
    public int SizeX
    {
        get { return TrueSizeX; }
        set
        {
            TrueSizeX = value;
            SizeXInput.text = value.ToString();
            if (value <= SizeXSlider.maxValue) { SizeXSlider.value = value; } // this check is so that you can manually enter numbers bigger than the slider's max
        }
    }
    public int SizeY
    {
        get { return TrueSizeY; }
        set
        {
            TrueSizeY = value;
            SizeYInput.text = value.ToString();
            if (value <= SizeYSlider.maxValue) { SizeYSlider.value = value; }
        }
    }

    public void Initialize()
    {
        GameplayUIManager.UIState = UIState.NewBoardMenu;

        // to make sure there's no disconnect between the UI and the what it represents
        SizeXSlider.value = SizeX;
        SizeYSlider.value = SizeY;
        SizeXInput.text = SizeX.ToString();
        SizeYInput.text = SizeY.ToString();

        NewBoardCanvas.enabled = true;

        // select the size x input field so that the user can immediately begin typing
        SizeXInput.ActivateInputField();
    }

    int PreviousSizeX;
    int CachedSizeY;
    public void RunNewBoardMenu()
    {
        // caching system makes sure a new board is only generated on frames where it matters
        if (PreviousSizeX != SizeX || CachedSizeY != SizeY)
        {
            BoardFunctions.CreateNewBoard(SizeX, SizeY);
        }
        PreviousSizeX = SizeX;
        CachedSizeY = SizeY;

        // create the menu if space or enter or V are pressed
        if (Input.GetButtonDown("Jump") || Input.GetButtonDown("Submit") || Input.GetButtonDown("BoardMenu")) // the last check is so you can place the same new board by double tapping v
        {
            Done();
        }

        // cancel if esc is pressed
        if (Input.GetButtonDown("Cancel"))
        {
            Done(true);
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

        if (GameplayUIManager.ScrollUp(false))
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

        if (GameplayUIManager.ScrollDown(false))
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

        // lets you rotate in the new board menu
        StuffPlacer.PollRotationInput();
        BoardPlacer.PollForBoardRotation();
        BoardPlacer.PollForBoardFlatness();

        RaycastHit hit;
        if(Physics.Raycast(FirstPersonInteraction.Ray(), out hit, Settings.ReachDistance))
        {
            StuffPlacer.MoveThingBeingPlaced(hit, false, true);
        }
    }

    public void OnXSliderUpdate()
    {
        SizeX = (int)SizeXSlider.value;
    }

    public void OnYSliderUpdate()
    {
        SizeY = (int)SizeYSlider.value;
    }

    // ok the enxt to methods are getting really messy. TODO: clean up
    public void OnXTextUpdate()
    {
        if (SizeXInput.text == "") { return; } // so you can backspace and type in your own value
        SizeX = TextToValidBoardDimension(SizeXInput.text);
    }

    public void OnYTextUpdate()
    {
        if (SizeYInput.text == "") { return; }
        SizeY = TextToValidBoardDimension(SizeYInput.text);
    }

    private static int TextToValidBoardDimension(string text)
    {
        int size = 1;
        if (text == "" || text == "-") { return 1; } else { size = int.Parse(text); }
        size = Mathf.Abs(size); // negative value boards are terrible don't let them exist
        if (size > 1000) { size = 1000; } // bigger boards crash the game and are glitchy in general
        if (size < 1) { size = 1; }
        return size;
    }

    public void Done(bool CancelPlacement = false)
    {
        // shitty hack to make sure a new board is generated on the next new board menu open. Works by tricking the caching system into thinking SizeX has changed
        PreviousSizeX = -69696969;

        // close the menu and disable the script
        NewBoardCanvas.enabled = false;

        if (CancelPlacement)
        {
            GameplayUIManager.UIState = UIState.None;
            BoardPlacer.CancelPlacement();
        }
        else
        {
            GameplayUIManager.UIState = UIState.BoardBeingPlaced;
        }

        //Input.ResetInputAxes(); // so that if V is used to place the board (a double tap), the board menu is not opened again // unnecessary now that we have UI states?
    }
}