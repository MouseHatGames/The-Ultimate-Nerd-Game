using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextEditMenu : MonoBehaviour {

    public TMP_InputField TextInput;
    public Slider SizeSlider;
    public Canvas TextEditCanvas;

    public static TextMeshPro TextBeingEdited;

    private void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") < 0) // scroll down
        {
            if (SizeSlider.value > 1)
            {
                SizeSlider.value--;
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0) // scroll up
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
    private void OnEnable()
    {
        TextEditCanvas.enabled = true;
        UIManager.SomeOtherMenuIsOpen = true;
        UIManager.UnlockMouseAndDisableFirstPersonLooking();

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
    }

    public void Done()
    {
        TextEditCanvas.enabled = false;
        TextInput.DeactivateInputField();
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        TextBeingEdited = null;
        UIManager.SomeOtherMenuIsOpen = false;
        UIManager.LockMouseAndEnableFirstPersonLooking();
        Input.ResetInputAxes(); // so that clicking done doesn't place something
        enabled = false;
    }
}
