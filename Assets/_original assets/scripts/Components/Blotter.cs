// blotters are effectively one-way pegs.

using UnityEngine;

public class Blotter : CircuitLogicComponent {

    public CircuitInput Input;
    public CircuitOutput Output;

    protected override void OnAwake()
    {
        Input = GetComponentInChildren<CircuitInput>();
        Output = GetComponentInChildren<CircuitOutput>();
    }

    protected override void CircuitLogicUpdate()
    {
        Output.On = Input.On;
    }
}