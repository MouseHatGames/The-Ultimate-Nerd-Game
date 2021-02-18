using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegaMeshComponent : MonoBehaviour
{
    public Renderer Renderer;
    public Mesh Mesh;

    public MegaMeshGroup Group;

    public MaterialType MaterialType;

    private void Awake()
    {
        Renderer = GetComponent<Renderer>();
        if (Mesh == null) { Mesh = GetComponent<MeshFilter>().sharedMesh; }
    }

    private void OnDestroy()
    {
        MegaMeshManager.RemoveComponentImmediately(this);
    }

    public Color Color { get { return Renderer.material.color; } }
}