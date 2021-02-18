// attached to the cube that is the base of a lever object

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using References;

public class Switch : Interactable {

    // the lever itself, to make it rotate. This script should be attached to it
    public GameObject lever;

    // the output of the lever. Must be assigned in inspector
    public CircuitOutput output;

	// Use this for initialization
	void Awake () {
        // assing all those variables
        lever = gameObject;

        // set switch colour
        lever.GetComponent<Renderer>().material.color = Settings.InteractableColor;
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
            lever.transform.localRotation = Quaternion.Euler(-40, 0, 0);
            if (usesound) { SoundPlayer.PlaySoundAt(Sounds.SwitchOn, transform); }
        }
        else
        {
            output.On = false;
            lever.transform.localRotation = Quaternion.Euler(40, 0, 0);
            if (usesound) { SoundPlayer.PlaySoundAt(Sounds.SwitchOff, transform); }
        }
    }
}