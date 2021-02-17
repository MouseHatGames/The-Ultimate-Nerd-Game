using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Delayer : MonoBehaviour {

    public CircuitInput Input;
    public Output Output;

    // Use this for initialization
    void Awake()
    {
        Input = GetComponentInChildren<CircuitInput>();
        Output = GetComponentInChildren<Output>();

        BehaviorManager.UpdatedDelayers.Add(this);
    }

    private void OnDestroy()
    {
        BehaviorManager.UpdatedDelayers.Remove(this);
    }

    public int DelayCount;
    public void CircuitLogicUpdate()
    {
        if(DelayCount == 9) // this number is what it is so that buttons can create a single pulse in a chain of delayers
        {
            Output.On = true;
        }
        else if(DelayCount == 0)
        {
            Output.On = false;
        }

        if (Input.On && Output.On)
        {
            DelayCount = 9; // for more consistent behaviour
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
        }
    }
}
