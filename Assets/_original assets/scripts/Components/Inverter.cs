using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inverter : CircuitLogicComponent
{
    public CircuitInput Input;
    public CircuitOutput Output;

	protected override void OnAwake ()
    {
        Input = GetComponentInChildren<CircuitInput>();
        Output = GetComponentInChildren<CircuitOutput>();
	}

    private void Start()
    {
        if (gameObject.layer == 5) { Output.GetRenderer.material.color = Settings.CircuitOnColor; } // quick dirty hack to get inverters to show up properly in the selection menu
    }

    protected override void CircuitLogicUpdate()
    {
        Output.On = !Input.On; // pretty crazy that this one line of code can build computers!
    }
}