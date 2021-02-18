// attached to the cube that is the base of a lever object

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using References;

public class Switch : Interactable
{
    // the switch itself, to make it rotate
    public GameObject VisualSwitch;

    // the output of the lever. Must be assigned in inspector
    public CircuitOutput output;

	// Use this for initialization
	void Awake ()
    {
        // set switch colour
        VisualSwitch.GetComponent<Renderer>().material.color = Settings.InteractableColor;
    }

    public override void Interact()
    {
        On = !On;
        UpdateLever();
    }

    public bool On; // technically we could use output.on but this method is less prone to bugs

    public void UpdateLever(bool usesound = true)
    {
        if (On)
        {
            output.On = true;
           // VisualSwitch.transform.localPosition = new Vector3(0, 0.865f, 0.114f);
            VisualSwitch.transform.localEulerAngles = new Vector3(-40, 0, 0);
            if (usesound) { SoundPlayer.PlaySoundAt(Sounds.SwitchOn, transform); }
        }
        else
        {
            output.On = false;
           // VisualSwitch.transform.localPosition = new Vector3(0, 0.865f, -0.114f);
            VisualSwitch.transform.localEulerAngles = new Vector3(40, 0, 0);
            if (usesound) { SoundPlayer.PlaySoundAt(Sounds.SwitchOff, transform); }
        }
    }
}