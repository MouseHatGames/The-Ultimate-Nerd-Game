// note that outputs can only have one connection, and that's to an input!

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Output : MonoBehaviour
{
    public bool On;

    private Renderer Renderer;
    public MeshFilter Mesh;

    // all IO connections which have this output as their output
    private List<InputOutputConnection> IOConnections = new List<InputOutputConnection>();

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
        InputOutputConnection[] bitches = IOConnections.ToArray();
        foreach(InputOutputConnection bitch in bitches)
        {
            RemoveIOConnection(bitch);
        }
    }

    public List<InputOutputConnection> GetIOConnections()
    {
        return IOConnections;
    }

    public Transform WireReference; // for referencing the point wires should attach to on the output

    private void Awake()
    {
        Mesh = GetComponent<MeshFilter>();
        Renderer = GetComponent<Renderer>();
        Renderer.material.color = MiscellaneousSettings.CircuitOffColor; // without this line, there is a single frame where the output is the default colour

        WireReference = transform.GetChild(0);
        if (gameObject.layer == 5) { CombinedMeshRenderer = Renderer; return; } // a shitty hack to fix a bug where in the built game the combined output mesh would be invisible in the selection menu
        QueueMeshRecalculation();

        BehaviorManager.UpdatedOutputs.Add(this);
    } 

    private void OnDestroy()
    {
        BehaviorManager.UpdatedOutputs.Remove(this);
        MegaCircuitMesh.RemoveOutput(this);
    }

    bool PreviouslyOn; // this kind of caching is used because setting material colors every frame is expensive
    public bool PartOfMegaCircuitMesh;
    float StableTime;
    private bool MeshRecalculationQueued;
    public bool AutoCombineOnStableAllowed = true;
    public void VisualUpdate()
    {
        // queueing is so that the expensive RecalculateCombinedMesh is only run a max of once per frame
        // important this is done before the rest of VisualUpdate
        if (MeshRecalculationQueued)
        {
            RecalculateCombinedMesh();
            MeshRecalculationQueued = false;
        }

        if (On != PreviouslyOn)
        {
            CombinedMeshRenderer.material.color = AppropriateColor();
            PreviouslyOn = On;

            MegaCircuitMesh.RemoveOutput(this);
            StableTime = 0;
        }
        else
        {
            StableTime += Time.deltaTime;

            if (StableTime > MegaCircuitMesh.MinStableCircuitTime 
                && MegaCircuitMesh.ValidMeshCalculationFrame 
                && !PartOfMegaCircuitMesh
                && AutoCombineOnStableAllowed)
            {
                MegaCircuitMesh.AddOutput(this);
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

    public static GameObject SharedOutputMeshPrefab;

    public GameObject CombinedMeshObject;
    public MeshFilter CombinedMeshFilter;
    public Renderer CombinedMeshRenderer;
    public void SetupCombinedMeshObject()
    {
        if (CombinedMeshObject != null) { Destroy(CombinedMeshObject); }

        CombinedMeshObject = Instantiate(SharedOutputMeshPrefab);
        CombinedMeshFilter = CombinedMeshObject.GetComponent<MeshFilter>();
        CombinedMeshRenderer = CombinedMeshObject.GetComponent<Renderer>();       

        CombinedMeshObject.layer = gameObject.layer; // really only needed for selection menu
    }



    // Recalculate combined mesh
    // mesh is the output plus all its IOConnections, since they all change color together
    private void RecalculateCombinedMesh()
    {
        SetupCombinedMeshObject();

        List<MeshFilter> CombineMeshes = new List<MeshFilter>();
        foreach (InputOutputConnection connection in IOConnections)
        {
            CombineMeshes.Add(connection.Mesh);
            connection.Renderer.enabled = false;
        }
        CombineMeshes.Add(Mesh);
        Renderer.enabled = false;

        CombineInstance[] combine = new CombineInstance[CombineMeshes.Count];
        for (int i = 0; i < CombineMeshes.Count; i++)
        {
            combine[i].mesh = CombineMeshes[i].sharedMesh;
            combine[i].transform = CombineMeshes[i].transform.localToWorldMatrix;
        }

        CombinedMeshObject.transform.parent = null;
        CombinedMeshObject.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        CombinedMeshFilter.mesh = new Mesh();
        CombinedMeshFilter.mesh.CombineMeshes(combine);
        CombinedMeshObject.transform.parent = transform.parent;


        if (On)
        {
            CombinedMeshRenderer.material.color = MiscellaneousSettings.CircuitOnColor;
            PreviouslyOn = true;
        }
        else
        {
            CombinedMeshRenderer.material.color = MiscellaneousSettings.CircuitOffColor;
            PreviouslyOn = false;
        }

        MegaCircuitMesh.RemoveOutput(this);
    }
}