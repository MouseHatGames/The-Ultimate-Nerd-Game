using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextEditMenu : MonoBehaviour
{
    public static TextEditMenu Instance;
    private void Awake() { Instance = this; MostRecentFontSize = 1.75f; }

    public TMP_InputField TextInput;
    public Slider SizeSlider;
    public Canvas Canvas;

    private static TextMeshPro TextBeingEdited;

    public void RunTextMenu()
    {
        if (GameplayUIManager.ScrollDown())
        {
            if (SizeSlider.value > 1)
            {
                SizeSlider.value--;
            }
        }

        if (GameplayUIManager.ScrollUp())
        {
            if (SizeSlider.value < SizeSlider.maxValue)
            {
                SizeSlider.value++;
            }
        }

        if (Input.GetButtonDown("Cancel"))
        {
            Done();
        }
    }

    // turn the menu on, prevent other menus from opening
    public void Initialize(TextMeshPro newtextbeingedited)
    {
        Canvas.enabled = true;
        GameplayUIManager.UIState = UIState.TextEditMenu;

        TextBeingEdited = newtextbeingedited;
        TextInput.text = TextBeingEdited.text;
        SizeSlider.value = TextBeingEdited.fontSize * 4;
        TextInput.ActivateInputField();
    }

    public void OnInputFieldChange()
    {
        TextBeingEdited.text = TextInput.text;
    }

    public void OnSizeSliderChange()
    {
        TextBeingEdited.fontSize = SizeSlider.value / 4;
        MostRecentFontSize = TextBeingEdited.fontSize;
    }

    public static float MostRecentFontSize { get; private set; }

    public void Done()
    {
        Canvas.enabled = false;
        TextInput.DeactivateInputField();
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        TextBeingEdited = null;

        GameplayUIManager.UIState = UIState.None;

        // this is so that if you change the size of the label text and you're in the middle of placing a label, it gets set to the new size
        if (StuffPlacer.GetThingBeingPlaced == null) { return; }
        if (StuffPlacer.GetThingBeingPlaced.GetComponent<Label>()) { StuffPlacer.DeleteThingBeingPlaced(); }
    }
}