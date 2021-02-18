using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayInteractable : Interactable {

    private Display display;
    private void Awake()
    {
        display = GetComponent<Display>();
    }

    public override void Interact()
    {
        EditDisplayColorMenu.Instance.InitiateMenu();
        EditDisplayColorMenu.Instance.DisplayBeingEdited = display;
    }
}