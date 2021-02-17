using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Display : MonoBehaviour {

    public CircuitInput Input;
    public Renderer Renderer;

	// Use this for initialization
	void Awake () {
        Input = GetComponentInChildren<CircuitInput>();
        Renderer = GetComponentInChildren<Renderer>();
        Renderer.material.color = MiscellaneousSettings.DisplayOffColor; // shitty fix to a shitty bug that makes displays white when first placed

        BehaviorManager.UpdatedDisplays.Add(this);
	}

    private void OnDestroy()
    {
        BehaviorManager.UpdatedDisplays.Remove(this);
    }

    // Update is called once per frame
    bool PreviouslyOn;
    public void VisualUpdate()
    {
        if(PreviouslyOn != Input.On)
        {
            Renderer.material.color = AppropriateColor();
            PreviouslyOn = Input.On;
        }
	}

    private Color AppropriateColor()
    {
        if (Input.On) { return MiscellaneousSettings.DisplayOnColor; }
        else { return MiscellaneousSettings.DisplayOffColor; }
    }
}
