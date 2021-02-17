// attached to the cube that is the base of a lever object

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour {

    // the lever itself, to make it rotate
    public GameObject lever;

    // the lever interactable, to detect clicking on it
    public Interactable interactable;

    // the output of the lever
    public Output output;

	// Use this for initialization
	void Awake () {
        // assing all those variables
        lever = transform.GetChild(0).gameObject;
        interactable = GetComponentInChildren<Interactable>();
        output = GetComponentInChildren<Output>();

        // set switch colour
        lever.GetComponent<Renderer>().material.color = MiscellaneousSettings.InteractableColor;

        BehaviorManager.UpdatedSwitches.Add(this);
    }

    private void OnDestroy()
    {
        BehaviorManager.UpdatedSwitches.Remove(this);
    }

    public void CircuitLogicUpdate()
    {
        if (interactable.Interacted)
        {
            On = !On;
            UpdateLever();
            interactable.Interacted = false;
        }
    }

    public bool On; // technically we could use output.on but this method is less prone to bugs

    public AudioSource SwitchOn;
    public AudioSource SwitchOff;
    public void UpdateLever()
    {
        if (On)
        {
            output.On = true;
            lever.transform.localRotation = Quaternion.Euler(-40, 0, 0);
            SwitchOn.Play();
        }
        else
        {
            output.On = false;
            lever.transform.localRotation = Quaternion.Euler(40, 0, 0);
            SwitchOff.Play();
        }
    }

    public void UpdateLeverNoSound() // this method exists to be called when the switch is loaded
    {
        if (On)
        {
            output.On = true;
            lever.transform.localRotation = Quaternion.Euler(-40, 0, 0);
        }
        else
        {
            output.On = false;
            lever.transform.localRotation = Quaternion.Euler(40, 0, 0);
        }
    }
}
