// a nice button that stays down for 10 ticks when pressed

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour {

    // the button itself, to make it get pressed
    public GameObject button;

    // the lever interactable, to detect clicking on it
    public Interactable interactable;

    // the output of the lever
    public Output output;

    // grab the default position of the button so different designs of buttons will all work
    public Vector3 DefaultPosition;

    public AudioSource ButtonDownSound;

    void Awake()
    {
        // assing all those variables
        button = transform.GetChild(0).gameObject;
        interactable = GetComponentInChildren<Interactable>();
        output = GetComponentInChildren<Output>();
        DefaultPosition = button.transform.localPosition;

        // set button colour
        button.GetComponent<Renderer>().material.color = MiscellaneousSettings.InteractableColor;

        BehaviorManager.UpdatedButtons.Add(this);
    }

    private void OnDestroy()
    {
        BehaviorManager.UpdatedButtons.Remove(this);
    }

    public int ButtonDownTime = 11; //counts how many FixedUpdate cycles the button has been down
    public void CircuitLogicUpdate()
    {
        if (interactable.Interacted)
        {
            ButtonDown();
            interactable.Interacted = false;
            ButtonDownTime = 0;
        }

        if(ButtonDownTime < 10)
        {
            ButtonDownTime++;
        }

        if(ButtonDownTime == 10)
        {
            ButtonUp();
            ButtonDownTime++;
        }
    }

    void ButtonDown()
    {
        button.transform.localPosition = DefaultPosition - new Vector3(0, 0.15f, 0);
        output.On = true;
        ButtonDownSound.Play();
    }

    void ButtonUp()
    {
        button.transform.localPosition = DefaultPosition;
        output.On = false;
    }
}
