using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Delayer : CircuitLogicComponent {

    public CircuitInput Input;
    public CircuitOutput Output;

    // Use this for initialization
    protected override void OnAwake()
    {
        Input = GetComponentInChildren<CircuitInput>();
        Output = GetComponentInChildren<CircuitOutput>();
    }

    public int DelayCount;
    protected override void CircuitLogicUpdate()
    {
        bool ContinueUpdating = true;

        if (DelayCount == 9)
        {
            Output.On = true;
        }
        else if (DelayCount == 0)
        {
            Output.On = false;
        }

        if (Input.On && Output.On)
        {
            DelayCount = 9; // for more consistent behaviour
            ContinueUpdating = false; // it is important that we stop updating here and not before. Sorry, I don't know why :)
        }
        else if (Input.On && !Output.On)
        {
            DelayCount++;
        }
        else if(!Input.On && Output.On)
        {
            DelayCount--;
        }
        else if(!Input.On && !Output.On)
        {
            DelayCount = 0;
            ContinueUpdating = false; // ditto
        }

        if (ContinueUpdating) { ContinueUpdatingForAnotherTick(); }
    }
}