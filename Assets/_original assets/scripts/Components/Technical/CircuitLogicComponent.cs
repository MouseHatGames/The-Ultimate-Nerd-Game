using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CircuitLogicComponent : MonoBehaviour
{
    public void DoCircuitLogicUpdate()
    {
        CircuitLogicUpdateQueued = false;
        CircuitLogicUpdate();
    }

    protected abstract void CircuitLogicUpdate();

    protected bool CircuitLogicUpdateQueued;
    public void QueueCircuitLogicUpdate()
    {
        if (CircuitLogicUpdateQueued) { return; }

        BehaviorManager.UpdatingCircuitLogicComponents.Add(this);
        CircuitLogicUpdateQueued = true;
    }

    protected void ContinueUpdatingForAnotherTick()
    {
        BehaviorManager.ContinuousUpdatingCircuitLogicComponents.Add(this);
        CircuitLogicUpdateQueued = true;
    }

    protected void Awake()
    {
        OnAwake();
        QueueCircuitLogicUpdate();
    }

    protected virtual void OnAwake() { }
}