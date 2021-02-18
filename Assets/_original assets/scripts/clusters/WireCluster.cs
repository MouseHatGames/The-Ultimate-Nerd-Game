using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; // for the fancy list.distinct stuff
using References;

public class WireCluster : VisualUpdaterWithMeshCombining
{
    private List<CircuitInput> ConnectedInputs = new List<CircuitInput>(); // inputs connected to the group
    private List<CircuitOutput> ConnectedOutputs = new List<CircuitOutput>(); // outputs connected to the group

    private List<CircuitLogicComponent> ConnectedCircuitLogicComponents = new List<CircuitLogicComponent>(); // of all the inputs in the group, this is all the circuit logic components they're part of

    public bool On; // whether the whole cluster is on or off

    protected override void AfterAwake()
    {
        QueueCircuitLogicUpdate();
    }

    protected override void WhenDestroyed()
    {
        // just make sure it's not in any list
        FinishedUpdating = true;
        BehaviorManager.UpdatingClusters.Remove(this);
        BehaviorManager.ClustersQueuedForMeshRecalculation.Remove(this);
    }

    bool PreviouslyOnVisual;
    public override void VisualUpdate()
    {
        if (On != PreviouslyOnVisual)
        {
            VisualsChanged();
            PreviouslyOnVisual = On;
        }
        else
        {
            VisualsHaventChanged();
        }
    }

    protected override void SetProperMaterialAndMegaMeshComponentMaterialType()
    {
        if (On) { MegaMeshComponent.MaterialType = MaterialType.CircuitOn; Renderer.material = Materials.CircuitOnAlwaysOnTop; }
        else { MegaMeshComponent.MaterialType = MaterialType.CircuitOff; Renderer.material = Materials.CircuitOffAlwaysOnTop; }
    }

    // if any connected output is on, every input in the whole cluster should turn on as well
    private bool PreviouslyOnCircuit;
    public void CircuitLogicUpdate()
    {
        On = false;
        CircuitLogicUpdateQueued = false;

        foreach (CircuitOutput output in ConnectedOutputs)
        {
            if (output.On)
            {
                On = true;
                break;
            }
        }

        if (On == PreviouslyOnCircuit) { return; }

        PreviouslyOnCircuit = On;
        foreach (CircuitInput input in ConnectedInputs)
        {
            input.On = On;
        }
        QueueVisualUpdate();
        UpdateConnectedComponents();
    }

    private void UpdateConnectedComponents()
    {
        foreach(CircuitLogicComponent component in ConnectedCircuitLogicComponents)
        {
            component.QueueCircuitLogicUpdate();
        }
    }

    // connectedinputs list management
    public void ConnectInput(CircuitInput penis)
    {
        if (penis.On != On) { QueueCircuitLogicUpdate(); }

        ConnectedInputs.Add(penis);
        penis.Cluster = this;
        QueueMeshRecalculation();

        if (penis.CircuitLogicComponent != null)
        {
            ConnectedCircuitLogicComponents.Add(penis.CircuitLogicComponent);

            // the following two lines are so that if you connect a component to the cluster it accurately checks for an update
            penis.On = On;
            penis.CircuitLogicComponent.QueueCircuitLogicUpdate();
        }
    }

    public void RemoveInput(CircuitInput penis)
    {
        ConnectedInputs.Remove(penis);
        penis.Cluster = null;
        if (!penis.IsSnappingPeg) { penis.Renderer.enabled = true; }
        penis.On = false; // it makes me nervous to set the state of this outside of the circuit update cycle, but this only really happens when the player deletes something so it should be okay... plus, if it gets reconnected to a cluster in the same frame, that cluster will set it to its proper state during the next update cycle before it has any effect on anything
        QueueMeshRecalculation();

        if (penis.CircuitLogicComponent != null)
        {
            ConnectedCircuitLogicComponents.Remove(penis.CircuitLogicComponent);
            penis.CircuitLogicComponent.QueueCircuitLogicUpdate();
        }
    }

    public CircuitInput[] GetConnectedInputs()
    {
        return ConnectedInputs.ToArray();
    }

    // connectedoutputs list management
    public void ConnectOutput(CircuitOutput penis)
    {
        ConnectedOutputs.Add(penis);
        if (penis.On != On) { QueueCircuitLogicUpdate(); }
    }

    public void RemoveOutput(CircuitOutput penis)
    {
        ConnectedOutputs.Remove(penis);
    }

    public CircuitOutput[] GetConnectedOutputs()
    {
        return ConnectedOutputs.ToArray();
    }

    // Recalculate combined mesh
    // mesh is of all the inputs and all the IIConnections in the cluster, since that's all that can be guaranteed to change color together
    public void RecalculateCombinedMesh()
    {
        MegaMeshManager.RemoveComponentImmediately(MegaMeshComponent);

        List<MeshFilter> MeshFilters = new List<MeshFilter>();
        foreach(CircuitInput input in ConnectedInputs)
        {
            if (!input.IsSnappingPeg)
            {
                MeshFilters.Add(input.MeshFilter);
                input.Renderer.enabled = false;

                foreach (InputInputConnection IIConnection in input.IIConnections)
                {
                    if (!MeshFilters.Contains(IIConnection.MeshFilter)) { MeshFilters.Add(IIConnection.MeshFilter); } // check so we don't get duplicates
                    IIConnection.Renderer.enabled = false;
                }
            }
        }

        transform.parent = null;
        transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

        MeshFilter.mesh = new Mesh();
        MeshFilter.mesh.CombineMeshes(MegaMeshManager.GetScaledCombineInstances(MeshFilters, 1.002f)); // the minimum viable value for extremely large clusters
        transform.parent = StuffConnector.ProperClusterParent(this);

        VisualsChanged();
        QueueVisualUpdate();

        MegaMeshComponent.Mesh = new Mesh();
        MegaMeshComponent.Mesh.CombineMeshes(MegaMeshManager.GetCombineInstances(MeshFilters));

        MeshRecalculationQueued = false;
    }

    bool MeshRecalculationQueued = false; // to avoid duplicate additions to the list over a single frame
    public void QueueMeshRecalculation()
    {
        if (MeshRecalculationQueued) { return; }

        BehaviorManager.ClustersQueuedForMeshRecalculation.Add(this);
        MeshRecalculationQueued = true;
    }

    bool CircuitLogicUpdateQueued = false;
    public void QueueCircuitLogicUpdate()
    {
        if (CircuitLogicUpdateQueued) { return; }

        BehaviorManager.UpdatingClusters.Add(this);
        CircuitLogicUpdateQueued = true;
    }
}