using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegaMesh : MonoBehaviour {

    private void Awake()
    {
        Mesh = GetComponent<MeshFilter>();
    }

    public static List<MeshFilter> IncludedMeshes = new List<MeshFilter>();

    public static MeshFilter Mesh;

    // add a single mesh
    public static void AddMesh(MegaMeshComponent newmesh)
    {
        IncludedMeshes.Add(newmesh.Mesh);
        newmesh.Renderer.enabled = false;
        RecalculateMegaMesh();
    }

    // add many meshes
    public static void AddMeshes(MegaMeshComponent[] newmeshes)
    {
        foreach(MegaMeshComponent newmesh in newmeshes)
        {
            IncludedMeshes.Add(newmesh.Mesh);
            newmesh.Renderer.enabled = false;
        }

        RecalculateMegaMesh();
    }

    // remove a single mesh
    public static void RemoveMesh(MegaMeshComponent oldmesh)
    {
        IncludedMeshes.Remove(oldmesh.Mesh);
        oldmesh.Renderer.enabled = true;
        RecalculateMegaMesh();
    }

    // remove many meshes
    public static void RemoveMeshes(MegaMeshComponent[] oldmeshes)
    {
        foreach(MegaMeshComponent oldmesh in oldmeshes)
        {
            IncludedMeshes.Remove(oldmesh.Mesh);
            oldmesh.Renderer.enabled = true;
        }

        RecalculateMegaMesh();
    }

    // add all meshes from a gameobject
    public static void AddMeshesFrom(GameObject go)
    {
        AddMeshes(go.GetComponentsInChildren<MegaMeshComponent>());
    }

    // remove all meshes from a gameobject
    public static void RemoveMeshesFrom(GameObject go)
    {
        RemoveMeshes(go.GetComponentsInChildren<MegaMeshComponent>());
    }

    // generate an entirely new mesh by finding all the MegaMeshComponents in the scene - really only useful on first load
    public static void GenerateNewMegaMesh()
    {
        IncludedMeshes.Clear();
        AddMeshes(FindObjectsOfType<MegaMeshComponent>());
    }

    // recalculate the mega mesh
    public static void RecalculateMegaMesh()
    {
        CombineInstance[] combine = new CombineInstance[IncludedMeshes.Count];
        int TotalVerts = 0;
        for (int i = 0; i < IncludedMeshes.Count; i++)
        {
            combine[i].mesh = IncludedMeshes[i].sharedMesh;
            combine[i].transform = IncludedMeshes[i].transform.localToWorldMatrix;
            TotalVerts += IncludedMeshes[i].mesh.vertexCount; // if this is slow, I can hardcode it to 24
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