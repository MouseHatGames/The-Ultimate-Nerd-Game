using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotGate : MonoBehaviour {

    public CircuitInput Input;
    public Output Output;

	void Awake () {
        Input = GetComponentInChildren<CircuitInput>();
        Output = GetComponentInChildren<Output>();

        BehaviorManager.UpdatedNotGates.Add(this);
	}

    private void Start()
    {
        // the only purpose of this is to fix the inverter's appearance in the selection menu, since for some reason
        // that inverter doesn't get added to BehaviorManager's list 
        if (gameObject.layer == 5)
        {
            Output.CombinedMeshRenderer.material.color = MiscellaneousSettings.CircuitOnColor;
        }
    }

    private void OnDestroy()
    {
        BehaviorManager.UpdatedNotGates.Remove(this);
    }

    public void CircuitLogicUpdate()
    {
        Output.On = !Input.On; // pretty crazy that this one line of code can build computers!
    }
}
