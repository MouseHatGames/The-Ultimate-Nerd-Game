// note that outputs can only have one connection, and that's to an input!

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using References;

public class CircuitOutput : VisualUpdaterWithMeshCombining
{
    [SerializeField] private bool TrueOn; // this needs to be serialized so that unity can copy its state using Object.Instantiate
    public bool On
    {
        get { return TrueOn; }
        set
        {
            if (value == TrueOn) { return; } // if there's no value change, DON'T DO ANYTHING

            TrueOn = value;

            QueueVisualUpdate();

            // pass the chain of updating on to the clusters
            foreach(InputOutputConnection connection in IOConnections)
            {
                connection.Input.Cluster.QueueCircuitLogicUpdate(); // TODO figure out: does doing this have a significant performance hit over having a direct reference to a list of connected clusters?
            }
        }
    }

    // all IO connections which have this output as their output
    private List<InputOutputConnection> IOConnections = new List<InputOutputConnection>();
    public List<InputOutputConnection> GetIOConnections() { return IOConnections; } // we do not want to allow other classes to directly modify IOConnections, but in some cases they must access it

    public void AddIOConnection(InputOutputConnection connection)
    {
        IOConnections.Add(connection);
        QueueMeshRecalculation();
    }

    public void RemoveIOConnection(InputOutputConnection connection)
    {
        IOConnections.Remove(connection);
        QueueMeshRecalculation();
    }

    public void ClearIOConnections()
    {
        InputOutputConnection[] bitches = IOConnections.ToArray(); // must be an array to avoid iteration errors
        foreach(InputOutputConnection bitch in bitches)
        {
            RemoveIOConnection(bitch);
        }
    }


    private MeshFilter BlotMeshFilter; // the acutal peg of the output
    protected override void AfterAwake()
    {
        BlotMeshFilter = GetComponent<MeshFilter>(); // VERY IMPORTANT that this is done first!
        RecalculateCombinedMesh();
    }

    protected override void WhenDestroyed()
    {
        FinishedUpdating = true;
        BehaviorManager.OutputsQueuedForMeshRecalculation.Remove(this);
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

    public GameObject CombinedMeshObject;
    public void SetupCombinedMeshObject()
    {
        MegaMeshManager.RemoveComponentImmediately(MegaMeshComponent); // so we don't get destroyed mesh components in there. Don't need to re-add it because the mesh has changed anyway

        if (CombinedMeshObject != null) { Destroy(CombinedMeshObject); }

        CombinedMeshObject = Instantiate(Prefabs.CombinedMeshGroup);
        MeshFilter = CombinedMeshObject.GetComponent<MeshFilter>();
        Renderer = CombinedMeshObject.GetComponent<Renderer>();

        MegaMeshComponent = CombinedMeshObject.AddComponent<MegaMeshComponent>();
        SetProperMaterialAndMegaMeshComponentMaterialType();

        CombinedMeshObject.layer = gameObject.layer; // really only needed for selection menu
    }

    // Recalculate combined mesh
    // mesh is the output plus all its IOConnections, since they all change color together
    public void RecalculateCombinedMesh()
    {
        SetupCombinedMeshObject();

        List<MeshFilter> CombineMeshes = new List<MeshFilter>();
        foreach (InputOutputConnection connection in IOConnections)
        {
            CombineMeshes.Add(connection.MeshFilter);
            connection.Renderer.enabled = false;
        }
        CombineMeshes.Add(BlotMeshFilter);

        CombinedMeshObject.transform.parent = null;
        CombinedMeshObject.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        MeshFilter.mesh = new Mesh();
        MeshFilter.mesh.CombineMeshes(MegaMeshManager.GetScaledCombineInstances(CombineMeshes, 1.002f));
        CombinedMeshObject.transform.parent = transform.parent;

        MegaMeshComponent.Mesh = new Mesh();
        MegaMeshComponent.Mesh.CombineMeshes(MegaMeshManager.GetCombineInstances(CombineMeshes));

        if (On)
        {
            Renderer.material = Materials.CircuitOnAlwaysOnTop;
            PreviouslyOnVisual = true;
        }
        else
        {
            Renderer.material = Materials.CircuitOffAlwaysOnTop;
            PreviouslyOnVisual = false;
        }

        MeshRecalculationQueued = false;
    }

    bool MeshRecalculationQueued = false; // to avoid duplicate additions to the list over a single frame
    public void QueueMeshRecalculation()
    {
        if (MeshRecalculationQueued) { return; }

        BehaviorManager.OutputsQueuedForMeshRecalculation.Add(this);
        MeshRecalculationQueued = true;
    }
}