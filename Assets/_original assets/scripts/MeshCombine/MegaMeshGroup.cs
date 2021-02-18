using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegaMeshGroup : MonoBehaviour
{
    public Renderer Renderer;
    public Mesh Mesh;

    public MaterialType MaterialType;

    private void Awake()
    {
        Renderer = GetComponent<Renderer>();
        Mesh = GetComponent<MeshFilter>().mesh;

        RecalculateMesh(); // not 100% sure why this is necessary but ¯\_(ツ)_/¯
    }

    public bool full = false; // if true, this group is full of meshes and cannot accept any more
    public int TotalVertexCount = 0; // used instead of Mesh.vertexcount because we need to check for fullness several times per frame in between adding more components, but mesh recalculation is only done a max of once per frame

    private HashSet<MegaMeshComponent> Components = new HashSet<MegaMeshComponent>();

    public void AddComponent(MegaMeshComponent component, bool AlwaysAddImmediate = false)
    {
        if (component.MaterialType != MaterialType) { Debug.LogError("component material type does not match group"); return; }

        Components.Add(component);
        component.Group = this;

        if (MegaMeshManager.IsDynamicMaterialType(this) && !AlwaysAddImmediate)
        {
            RecalculateNextOpportunity();
        }
        else
        {
            RecalculateNextFrame();
        }

        TotalVertexCount += component.Mesh.vertexCount;
        DetermineFullness();
    }

    public void RemoveComponentStaggered(MegaMeshComponent component)
    {
        Removal(component);
        RecalculateNextOpportunity();
    }

    public void RemoveComponentImmediately(MegaMeshComponent component)
    {
        Removal(component);
        RecalculateNextFrame();
    }

    private void Removal(MegaMeshComponent component)
    {
        Components.Remove(component);
        component.Group = null;
        component.Renderer.enabled = true;

        TotalVertexCount -= component.Mesh.vertexCount;
        DetermineFullness();
    }

    // the queueing is so that the expensive mesh recalculating only happens a maximum of once per frame
    bool MeshRecalculationQueued = false;
    public void RecalculateNextFrame()
    {
        if (MeshRecalculationQueued) { return; }

        BehaviorManager.MegaMeshGroupsQueuedForRecalculation.Add(this);
        MeshRecalculationQueued = true;
    }

    private bool QueuedForStaggeredRecalculation = false;
    public void RecalculateNextOpportunity()
    {
        if (MeshRecalculationQueued) { return; }
        if (QueuedForStaggeredRecalculation) { return; }
        if (!MegaMeshManager.IsDynamicMaterialType(this))
        {
            Debug.LogError("tried to do staggered recalculation for static material type");
            RecalculateNextFrame();
            return;
        }

        BehaviorManager.DynamicMegaMeshesThatNeedRecalculating.Add(this);
        QueuedForStaggeredRecalculation = true;
    }

    public void RecalculateMesh() // being public is necessary so behaviormanager can access it. No other class should call this directly
    {
        if (TotalVertexCount > 65530) // a little low, just to be safe
        {
            Mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        }
        else
        {
            Mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt16;
        }

        Mesh.CombineMeshes(MegaMeshManager.GetCombineInstancesAndDisableRenderers(Components));

        TotalVertexCount = Mesh.vertexCount; // in case it got screwed up somewhere, it will be set properly on RecalculateMesh
        DetermineFullness();

        MeshRecalculationQueued = false;
        QueuedForStaggeredRecalculation = false;
    }

    private void DetermineFullness()
    {
        int MaxAllowedVertices;
        if (MegaMeshManager.IsDynamicMaterialType(this))
        {
            MaxAllowedVertices = MegaMeshManager.MaxVerticesPerDynamicMesh;
        }
        else
        {
            MaxAllowedVertices = MegaMeshManager.MaxVerticesPerStaticMesh;
        }

        full = TotalVertexCount >= MaxAllowedVertices;
    }

    // not used for anything at the moment
    public int RecalculateVerticesCount()
    {
        int verticescount = 0;
        foreach (MegaMeshComponent component in Components)
        {
            verticescount += component.Mesh.vertexCount;
        }

        return verticescount;
    }
}