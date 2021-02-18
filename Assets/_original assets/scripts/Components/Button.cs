// a nice button that stays down for 10 ticks when pressed

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using References;

public class Button : Interactable
{
    public GameObject visualbutton; // the visual representation of the button. It is different from the gameobject this is attached to so we can have the collider not move; if it moved, you could press the button in places where it would immediately unpress
    public CircuitOutput output; // the output of the button. Must be assigned in inspector
    private Vector3 DefaultPosition; // grab the default position of the button so different designs of buttons will all work

    void Awake()
    {
        DefaultPosition = visualbutton.transform.localPosition;

        // set button colour
        visualbutton.GetComponent<Renderer>().material.color = Settings.InteractableColor;

        // don't run Update()
        enabled = false;
    }

    public override void Interact()
    {
        ButtonDown();
    }

    private int TicksDownFor;
    private int MinDownTicks = 2;

    // run every frame the button is held down
    private void Update()
    {
        TicksDownFor++;
        if (TicksDownFor <= MinDownTicks) { return; }

        RaycastHit hit;
        Physics.Raycast(FirstPersonInteraction.Ray(), out hit, Settings.ReachDistance, Wire.IgnoreWiresLayermask);
        if (!Input.GetButton("Interact") || hit.collider == null || hit.collider.gameObject != gameObject) // if we let go of the interact key or if the player is no longer lookin here
        {
            ButtonUp();
        }
    }

    void ButtonDown()
    {
        visualbutton.transform.localPosition = DefaultPosition - new Vector3(0, 0.15f, 0);
        output.On = true;
        TicksDownFor = 0;

        enabled = true; // being updating
        SoundPlayer.PlaySoundAt(Sounds.ButtonDown, transform);
    }

    void ButtonUp()
    {
        visualbutton.transform.localPosition = DefaultPosition;
        output.On = false;

        enabled = false; // stop updating
        SoundPlayer.PlaySoundAt(Sounds.ButtonUp, transform);
    }
}