using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegaCircuitMesh : MonoBehaviour {

    // the following four variables are all assigned in the inspector
    [SerializeField] private MeshFilter OnMesh;
    [SerializeField] private Renderer OnRenderer;
    [SerializeField] private MeshFilter OffMesh;
    [SerializeField] private Renderer OffRenderer;

    public static MegaCircuitMesh Instance;

    private void Awake()
    {
        Instance = this;

        if (!ES3.KeyExists("MinTimeBetweenMeshRecalculations", "settings.txt")) { ES3.Save<float>("MinTimeBetweenMeshRecalculations", (float)5, "settings.txt"); }
        MinTimeBetweenMeshRecalculations = ES3.Load<float>("MinTimeBetweenMeshRecalculations", "settings.txt", 5);
        if (!ES3.KeyExists("MinStableCircuitTime", "settings.txt")) { ES3.Save<float>("MinStableCircuitTime", (float)5, "settings.txt"); }
        MinStableCircuitTime = ES3.Load<float>("MinStableCircuitTime", "settings.txt");
    }

    private void Start()
    {
        // done in Start so that when I get around to making circuit on/off color configurable I can do it in Awake with no issue
        OnRenderer.material.color = MiscellaneousSettings.CircuitOnColor;
        OffRenderer.material.color = MiscellaneousSettings.CircuitOffColor;
    }

    // variables that help us make sure the expensive mesh recalculation stuff isn't done every frame
    public static float MinTimeBetweenMeshRecalculations;
    public static float MinStableCircuitTime;
    public static bool ValidMeshCalculationFrame;


    private static List<WireCluster> OnClusters = new List<WireCluster>();
    private static List<WireCluster> OffClusters = new List<WireCluster>();
    private static List<Output> OnOutputs = new List<Output>();
    private static List<Output> OffOutputs = new List<Output>();

    public static void AddCluster(WireCluster cluster)
    {
        cluster.PartOfMegaCircuitMesh = true;
        cluster.Renderer.enabled = false;

        if (cluster.On)
        {
            if (!OnClusters.Contains(cluster)) { OnClusters.Add(cluster); }
            OnMeshRecalculationQueued = true;
        }
        else
        {
            if (!OffClusters.Contains(cluster)) { OffClusters.Add(cluster); }
            OffMeshRecalculationQueued = true;
        }
    }

    public static void RemoveCluster(WireCluster cluster)
    {
        cluster.PartOfMegaCircuitMesh = false;
        cluster.Renderer.enabled = true;

        if (OnClusters.Contains(cluster))
        {
            OnClusters.Remove(cluster);
            OnMeshRecalculationQueued = true;
        }
        else if (OffClusters.Contains(cluster))
        {
            OffClusters.Remove(cluster);
            OffMeshRecalculationQueued = true;
        }
    }

    public static void AddOutput(Output output)
    {
        output.PartOfMegaCircuitMesh = true;
        output.CombinedMeshRenderer.enabled = false;

        if (output.On)
        {
            if (!OnOutputs.Contains(output)) { OnOutputs.Add(output); }
            OnMeshRecalculationQueued = true;
        }
        else
        {
            if (!OffOutputs.Contains(output)) { OffOutputs.Add(output); }
            OffMeshRecalculationQueued = true;
        }
    }

    public static void RemoveOutput(Output output)
    {
        output.PartOfMegaCircuitMesh = false;
        output.CombinedMeshRenderer.enabled = true;

        if (OnOutputs.Contains(output))
        {
            OnOutputs.Remove(output);
            OnMeshRecalculationQueued = true;
        }
        else if (OffOutputs.Contains(output))
        {
            OffOutputs.Remove(output);
            OffMeshRecalculationQueued = true;
        }
    }

    private static bool OnMeshRecalculationQueued;
    private static bool OffMeshRecalculationQueued;

    private float TimeSinceLastValidMeshCalculationFrame;
    public void VisualUpdate()
    {
        // since BehaviorManager runs this code AFTER the visual updates of wirecluster and output,
        // MeshRecalculationQueued will always refelct the state needed for each frame.
        if (OnMeshRecalculationQueued)
        {
            RecalculateMegaMesh(true);
            OnMeshRecalculationQueued = false;
        }
        if (OffMeshRecalculationQueued)
        {
            RecalculateMegaMesh(false);
            OffMeshRecalculationQueued = false;
        }

        // cycle it so that ValidMeshCalculationFrame - which tells WireCluster and Output whether they can add themselves to a mega circuit mesh - is true
        // only every MinTimeBetweenMeshRecalculations seconds
        if(TimeSinceLastValidMeshCalculationFrame > MinTimeBetweenMeshRecalculations)
        {
            ValidMeshCalculationFrame = true;
            TimeSinceLastValidMeshCalculationFrame = 0;
        }
        else
        {
            ValidMeshCalculationFrame = false;
            TimeSinceLastValidMeshCalculationFrame += Time.deltaTime;
        }
    }

    private void RecalculateMegaMesh(bool On)
    {
        MeshFilter Mesh;
        List<WireCluster> Clusters;
        List<Output> Outputs;

        if (On)
        {
            Mesh = OnMesh;
            Clusters = OnClusters;
            Outputs = OnOutputs;
        }
        else
        {
            Mesh = OffMesh;
            Clusters = OffClusters;
            Outputs = OffOutputs;
        }

        CombineInstance[] combine = new CombineInstance[Clusters.Count + Outputs.Count];
        int TotalVerts = 0;
        for (int i = 0; i < Clusters.Count; i++)
        {
            combine[i].mesh = Clusters[i].Mesh.mesh;
            combine[i].transform = Clusters[i].transform.localToWorldMatrix;
            TotalVerts += Clusters[i].Mesh.mesh.vertexCount; // if this is slow or something I can make the mesh always 32 bits. Don't know how performance-intensive that would be though.
        }

        int ClustersCount = Clusters.Count; // caching it so we don't have to get the list's count 500 times

        for (int i = 0; i < Outputs.Count; i++)
        {
            combine[ClustersCount + i].mesh = Outputs[i].CombinedMeshFilter.mesh;
            combine[ClustersCount + i].transform = Outputs[i].CombinedMeshObject.transform.localToWorldMatrix;
            TotalVerts += Outputs[i].CombinedMeshFilter.mesh.vertexCount;
        }

        Mesh.mesh = new Mesh();

        // make sure the index buffer of the mega mesh has enough bits for all its vertices
        if (TotalVerts > 65530) { Mesh.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; }
        else { Mesh.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt16; }

        Mesh.mesh.CombineMeshes(combine);
    }

    //// the queueing system here is so that RecalculateMegaMesh isn't run too often.
    //private static List<WireCluster> AddClusterQueue;
    //private static List<WireCluster> RemoveClusterQueue;
    //private static List<Output> AddOutputQueue;
    //private static List<Output> RemoveOutputQueue;

    //private void DealWithQueue()
    //{

    //}
}