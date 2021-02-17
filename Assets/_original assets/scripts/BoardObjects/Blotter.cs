// very similar to NotGate.cs

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blotter : MonoBehaviour {

    public CircuitInput Input;
    public Output Output;

    void Awake()
    {
        Input = GetComponentInChildren<CircuitInput>();
        Output = GetComponentInChildren<Output>();

        BehaviorManager.UpdatedBlotters.Add(this);
    }

    private void OnDestroy()
    {
        BehaviorManager.UpdatedBlotters.Remove(this);
    }

    public void CircuitLogicUpdate()
    {
        Output.On = Input.On;
    }
}
