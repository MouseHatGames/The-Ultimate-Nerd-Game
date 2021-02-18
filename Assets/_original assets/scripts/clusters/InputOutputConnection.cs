using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;

public class InputOutputConnection : Wire {

    private CircuitInput RealInput;
    private CircuitOutput RealOutput;
    public CircuitInput Input
    {
        get { return RealInput; }
        set
        {
            RealInput = value;
            Point1 = GetWireReference(value.transform);
            value.IOConnections.Add(this);
        }
    }
    public CircuitOutput Output
    {
        get { return RealOutput; }
        set
        {
            if(value == null) { Debug.LogError("Tried to set output to null"); Destroy(gameObject); return; }
            RealOutput = value;
            Point2 = GetWireReference(value.transform);
            value.AddIOConnection(this);
        }
    }

    public override void SetPegsBasedOnPoints()
    {
        if(Point1.parent.tag == "Input")
        {
            Input = Point1.parent.GetComponent<CircuitInput>();
            Output = Point2.parent.GetComponent<CircuitOutput>();
        }
        else
        {
            Input = Point2.parent.GetComponent<CircuitInput>();
            Output = Point1.parent.GetComponent<CircuitOutput>();
        }
    }

}
