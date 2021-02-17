using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; // for the fancy list.distinct stuff

public class WireCluster : MonoBehaviour {

    [SerializeField] private List<CircuitInput> ConnectedInputs; // inputs connected to the group
    [SerializeField] private List<Output> ConnectedOutputs; // outputs connected to the group

    public bool On; // whether the whole cluster is on or off

    public MeshFilter Mesh;
    public Renderer Renderer;
    private void Awake()
    {
        Renderer = GetComponent<Renderer>();
        Mesh = GetComponent<MeshFilter>();

        BehaviorManager.UpdatedClusters.Add(this);
    }

    private void OnDestroy()
    {
        BehaviorManager.UpdatedClusters.Remove(this);
        MegaCircuitMesh.RemoveCluster(this);
    }

    private void Start()
    {
        // a bug can sometimes duplicate connected outputs in the list when creating new clusters. This is my hacky fix.
        ConnectedInputs = ConnectedInputs.Distinct().ToList();
        ConnectedOutputs = ConnectedOutputs.Distinct().ToList();

        // to prevent things going off for a tick when clusters are recalculated
        UpdateClusterState(); // TODO should go in awake???????????????????????
    }

    bool PreviouslyOn; // this kind of caching is used because setting material colors every frame is expensive
    public bool PartOfMegaCircuitMesh;
    float StableTime;
    private bool MeshRecalculationQueued;
    public bool AutoCombineOnStableAllowed = true;
    public void VisualUpdate()
    {
        // queueing is so that the expensive RecalculateCombinedMesh is only run a max of once per frame
        if (MeshRecalculationQueued)
        {
            RecalculateCombinedMesh();
            MeshRecalculationQueued = false;
        }

        if (On != PreviouslyOn)
        {
            Renderer.material.color = AppropriateColor();
            PreviouslyOn = On;

            MegaCircuitMesh.RemoveCluster(this);
            StableTime = 0;
        }
        else
        {
            // this whole thing is for circuits that don't change state often to be added to a mega mesh
            StableTime += Time.deltaTime;

            if(StableTime > MegaCircuitMesh.MinStableCircuitTime 
                && MegaCircuitMesh.ValidMeshCalculationFrame 
                && !PartOfMegaCircuitMesh
                && AutoCombineOnStableAllowed)
            {
                MegaCircuitMesh.AddCluster(this);
            }
        }
    }

    private Color AppropriateColor()
    {
        if (On) { return MiscellaneousSettings.CircuitOnColor; }
        else { return MiscellaneousSettings.CircuitOffColor; }
    }

    public void QueueMeshRecalculation()
    {
        MeshRecalculationQueued = true;
    }

    public void CircuitLogicUpdate()
    {
        UpdateClusterState();
    }

    // if any connected output is on, every input in the whole cluster should turn on as well
    void UpdateClusterState()
    {
        foreach (Output output in ConnectedOutputs)
        {
            if (output.On)
            {
                On = true;
                return; // this should stop the function as soon as it finds an output that's on
            }
        }
        // if no on output was found, everything should turn off
        On = false;
    }

    // connectedinputs list management
    public void ConnectInput(CircuitInput penis)
    {
        ConnectedInputs.Add(penis);
        penis.Cluster = this;
        QueueMeshRecalculation();
    }

    public void RemoveInput(CircuitInput penis)
    {
        ConnectedInputs.Remove(penis);
        penis.Cluster = null;
        penis.Renderer.enabled = true;
        QueueMeshRecalculation();
    }

    public CircuitInput[] GetConnectedInputs()
    {
        return ConnectedInputs.ToArray();
    }

    // connectedoutputs list management
    public void ConnectOutput(Output penis)
    {
        ConnectedOutputs.Add(penis);
    }

    public void RemoveOutput(Output penis)
    {
        ConnectedOutputs.Remove(penis);
    }

    public Output[] GetConnectedOutputs()
    {
        return ConnectedOutputs.ToArray();
    }

    // Recalculate combined mesh
    // mesh is of all the inputs and all the IIConnections in the cluster, since that's all that can be guaranteed to change color together
    private void RecalculateCombinedMesh()
    {
        List<MeshFilter> CombineMeshes = new List<MeshFilter>();
        foreach(CircuitInput input in ConnectedInputs)
        {
            CombineMeshes.Add(input.Mesh);
            input.Renderer.enabled = false;

            foreach(InputInputConnection IIConnection in input.IIConnections)
            {
                if (!CombineMeshes.Contains(IIConnection.Mesh)) { CombineMeshes.Add(IIConnection.Mesh); } // check is so we don't get duplicates
                IIConnection.Renderer.enabled = false;
            }
        }


        CombineInstance[] combine = new CombineInstance[CombineMeshes.Count];
        for(int i = 0; i < CombineMeshes.Count; i++)
        {
            combine[i].mesh = CombineMeshes[i].sharedMesh;
            combine[i].transform = CombineMeshes[i].transform.localToWorldMatrix;
        }

        transform.parent = null;
        transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        Mesh.mesh = new Mesh();
        Mesh.mesh.CombineMeshes(combine);
        transform.parent = ProperClusterParent();

        if (On)
        {
            Renderer.material.color = MiscellaneousSettings.CircuitOnColor;
            PreviouslyOn = true;
        }
        else
        {
            Renderer.material.color = MiscellaneousSettings.CircuitOffColor;
            PreviouslyOn = false;
        }

        MegaCircuitMesh.RemoveCluster(this);
    }

    private Transform ProperClusterParent()
    {
        int ShallowestDepthInHeirarchy = 1000000000;
        Transform ProperParent = transform;
        foreach(CircuitInput input in ConnectedInputs)
        {
            int DepthInHeirarchy = StuffConnecter.DepthInHeirarchy(input.transform);
            if (ShallowestDepthInHeirarchy > DepthInHeirarchy)
            {
                ProperParent = input.transform.parent;
                ShallowestDepthInHeirarchy = DepthInHeirarchy;
            }
        }

        return ProperParent;
    }
}