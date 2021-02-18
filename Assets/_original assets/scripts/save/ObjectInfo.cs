// attach to objects you want to save

using System.Collections.Generic;
using UnityEngine;

public class ObjectInfo : MonoBehaviour
{
    public ComponentType ComponentType; // could be CircuitBoard, NotGate, Peg, ect

    private void Start()
    {
        SaveManager.ActiveSaveObjects.Add(this);
    }
    private void OnDestroy()
    {
        SaveManager.ActiveSaveObjects.Remove(this);
    }
}