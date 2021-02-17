using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegaMeshComponent : MonoBehaviour {

    public Renderer Renderer;
    public MeshFilter Mesh;

    private void Awake()
    {
        Renderer = GetComponent<Renderer>();
        Mesh = GetComponent<MeshFilter>();
    }
}
