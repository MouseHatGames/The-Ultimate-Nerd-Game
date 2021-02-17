using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegaBoardMesh : MonoBehaviour {

    private void Awake()
    {
        Mesh = GetComponent<MeshFilter>();
    }

    public MeshFilter Mesh;

    public List<CircuitBoard> IncludedBoards = new List<CircuitBoard>();

    // recalculate the mega mesh
    public void RecalculateMegaMesh()
    {
        CombineInstance[] combine = new CombineInstance[IncludedBoards.Count];
        int TotalVerts = 0;
        for (int i = 0; i < IncludedBoards.Count; i++)
        {
            combine[i].mesh = IncludedBoards[i].ThisMesh;
            combine[i].transform = IncludedBoards[i].transform.localToWorldMatrix;
            TotalVerts += IncludedBoards[i].ThisMesh.vertexCount;
        }

        Mesh.mesh = new Mesh();

        // make sure the index buffer of the mega mesh has enough bits for all its vertices
        if (TotalVerts > 65530)
        {
            Mesh.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        }
        else
        {
            Mesh.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt16;
        }

        Mesh.mesh.CombineMeshes(combine);

        //  Debug.Log("Mega Mesh vertices: " + Mesh.mesh.vertexCount);
    }

}
