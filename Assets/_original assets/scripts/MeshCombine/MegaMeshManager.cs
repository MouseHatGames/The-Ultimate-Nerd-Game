using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using References;

public static class MegaMeshManager
{
    public static Dictionary<MaterialType, HashSet<MegaMeshGroup>> StandardizedMegaMeshGroups = new Dictionary<MaterialType, HashSet<MegaMeshGroup>>();
    public static Dictionary<Color, HashSet<MegaMeshGroup>> BoardMegaMeshGroups = new Dictionary<Color, HashSet<MegaMeshGroup>>();
    public static Dictionary<Color, HashSet<MegaMeshGroup>> SolidColorMegaMeshGroups = new Dictionary<Color, HashSet<MegaMeshGroup>>();

    public static void ClearReferences()
    {
        // hopefully the actual thingies are destroyed during scene change or something
        StandardizedMegaMeshGroups.Clear();
        BoardMegaMeshGroups.Clear();
    }

    public static int MaxVerticesPerStaticMesh = Settings.Get("MaxVerticesPerStaticMegaMesh", 60000); // static meshes are ones that don't change while circuitry is running; circuit boards, white component bits
    public static int MaxVerticesPerDynamicMesh = Settings.Get("MaxVerticesPerDynamicMegaMesh", 20000); // dynamic meshes are ones that change while circuitry is running; circuitry itself, displays

    public static void AddComponent(MegaMeshComponent component, bool AddAllImmediate = false)
    {
        if (component == null) { return; }
        if (component.MaterialType == MaterialType.Unknown) { return; } // don't do this if we don't know what kind of mega mesh to add it to
        if (component.Group != null) { return; }

        HashSet<MegaMeshGroup> grouplist;
        if (component.MaterialType == MaterialType.CircuitBoard) // the enum check is faster than doing if(component is BoardMegaMeshComponent) according to Stack Overflow
        {
            BoardMegaMeshGroups.TryGetValue(component.Color, out grouplist); // my first time using TryGetValue! grouplist will be set to null if the key doesn't exist
        }
        else if (component.MaterialType == MaterialType.SolidColor)
        {
            SolidColorMegaMeshGroups.TryGetValue(component.Color, out grouplist);
        }
        else // boards are a special case, since we don't know what colors they'll be. Everything else can be handled generically
        {
            if (component.Mesh == null) { return; }
            if (component.Mesh.vertexCount > MaxVerticesPerDynamicMesh) { return; } // if a cluster or output has exceeded the complexity allowed for a mega mesh group, not much point adding it to the group...

            StandardizedMegaMeshGroups.TryGetValue(component.MaterialType, out grouplist);
        }

        AddComponentTo(component, grouplist);
    }

    private static void AddComponentTo(MegaMeshComponent component, HashSet<MegaMeshGroup> GroupList, bool AddAllImmediate = false)
    {
        if (GroupList != null)
        {
            foreach (MegaMeshGroup group in GroupList)
            {
                if (!group.full)
                {
                    group.AddComponent(component, AddAllImmediate); // if any non-full groups are found, add the component to the first one found
                    return;
                }
            }
        }

        // if all groups are full, or if there are no groups, create a new group for the component
        CreateNewGroupFor(component, AddAllImmediate);
    }

    private static void CreateNewGroupFor(MegaMeshComponent component, bool AddImmediate = false)
    {
        MegaMeshGroup newgroup = Object.Instantiate(Prefabs.CombinedMeshGroup, Transforms.MegaMeshParent).AddComponent<MegaMeshGroup>();
        newgroup.MaterialType = component.MaterialType;
        SetGroupMaterial(newgroup);

        // add newgroup to the appropriate list inside the appropriate dictionary. If the appropriate list doesn't exist, create it
        if (component.MaterialType == MaterialType.CircuitBoard)
        {
            if (!BoardMegaMeshGroups.ContainsKey(component.Color))
            {
                BoardMegaMeshGroups.Add(component.Color, new HashSet<MegaMeshGroup>());
            }
            BoardMegaMeshGroups[component.Color].Add(newgroup);

            newgroup.Renderer.material.color = component.Color;
        }
        else if (component.MaterialType == MaterialType.SolidColor)
        {
            if (!SolidColorMegaMeshGroups.ContainsKey(component.Color))
            {
                SolidColorMegaMeshGroups.Add(component.Color, new HashSet<MegaMeshGroup>());
            }
            SolidColorMegaMeshGroups[component.Color].Add(newgroup);

            newgroup.Renderer.material.color = component.Color;
        }
        else
        {
            if (!StandardizedMegaMeshGroups.ContainsKey(component.MaterialType))
            {
                StandardizedMegaMeshGroups.Add(component.MaterialType, new HashSet<MegaMeshGroup>());
            }
            StandardizedMegaMeshGroups[component.MaterialType].Add(newgroup);
        }

        newgroup.AddComponent(component, AddImmediate); // add the component to the new group
    }

    public static void RemoveComponentImmediately(MegaMeshComponent component)
    {
        if (component == null) { return; }
        if (component.Group == null) { return; }
        component.Group.RemoveComponentImmediately(component);
    }

    public static void RemoveComponentStaggered(MegaMeshComponent component)
    {
        if (component == null) { return; }
        if (component.Group == null) { return; }
        component.Group.RemoveComponentStaggered(component);
    }

    public static void AddComponentOf(GameObject boner)
    {
        MegaMeshComponent cum = boner.GetComponent<MegaMeshComponent>();
        if (cum != null) { AddComponent(cum); }
    }

    public static void RemoveComponentImmediatelyOf(GameObject boner)
    {
        MegaMeshComponent cum = boner.GetComponent<MegaMeshComponent>();
        if (cum != null) { RemoveComponentImmediately(cum); }
    }

    public static void AddComponents(MegaMeshComponent[] components)
    {
        foreach(MegaMeshComponent component in components)
        {
            AddComponent(component);
        }
    }

    public static void RemoveComponentsImmediately(MegaMeshComponent[] components)
    {
        foreach(MegaMeshComponent component in components)
        {
            RemoveComponentImmediately(component);
        }
    }

    public static void AddComponentsIn(GameObject bigboi)
    {
        MegaMeshComponent[] components = bigboi.GetComponentsInChildren<MegaMeshComponent>();
        AddComponents(components);
    }

    public static void RemoveComponentsImmediatelyIn(GameObject bigboi)
    {
        MegaMeshComponent[] components = bigboi.GetComponentsInChildren<MegaMeshComponent>();
        RemoveComponentsImmediately(components);
    }

    public static void RecalculateGroupFor(MegaMeshComponent component)
    {
        if (component.Group == null) { return; }
        component.Group.RecalculateNextFrame();
    }

    public static void RecalculateGroupsFor(MegaMeshComponent[] components)
    {
        foreach(MegaMeshComponent component in components)
        {
            RecalculateGroupFor(component);
        }
    }

    public static void RecalculateGroupsOf(GameObject jewishpeoplearegenerallyprettychillinmyexperience)
    {
        RecalculateGroupsFor(jewishpeoplearegenerallyprettychillinmyexperience.GetComponentsInChildren<MegaMeshComponent>());
    }

    public static void AddComponentsEverywhere()
    {
        MegaMeshComponent[] allcomponents = Object.FindObjectsOfType<MegaMeshComponent>();
        foreach (MegaMeshComponent component in allcomponents)
        {
            // this check is done because on startup, for reasons I am unsure of, clusters with a snapping peg in them can sometimes have a null sharedmesh, which as you might imagine
            // basically fucks everything up.
            if (component.Mesh != null)
            {
                AddComponent(component, true);
            }
        }
    }


    public static void SetGroupMaterial(MegaMeshGroup group)
    {
        if(group.MaterialType == MaterialType.CircuitBoard)
        {
            group.Renderer.material = Materials.CircuitBoard;
            // color must be set elsewhere
            return;
        }

        switch (group.MaterialType)
        {
            case MaterialType.CircuitOn:
                group.Renderer.material = Materials.CircuitOn;
                break;

            case MaterialType.CircuitOff:
                group.Renderer.material = Materials.CircuitOff;
                break;

            case MaterialType.SolidColor:
                group.Renderer.material = Materials.Default;
                break;

            case MaterialType.DisplayOff:
                group.Renderer.material = Materials.DisplayOff;
                break;

            case MaterialType.DisplayRed:
                group.Renderer.material = Materials.DisplayRed;
                break;

            case MaterialType.DisplayGreen:
                group.Renderer.material = Materials.DisplayGreen;
                break;

            case MaterialType.DisplayBlue:
                group.Renderer.material = Materials.DisplayBlue;
                break;

            case MaterialType.DisplayYellow:
                group.Renderer.material = Materials.DisplayYellow;
                break;

            case MaterialType.DisplayOrange:
                group.Renderer.material = Materials.DisplayOrange;
                break;

            case MaterialType.DisplayPurple:
                group.Renderer.material = Materials.DisplayPurple;
                break;

            case MaterialType.DisplayWhite:
                group.Renderer.material = Materials.DisplayWhite;
                break;

            case MaterialType.DisplayCyan:
                group.Renderer.material = Materials.DisplayCyan;
                break;

            case MaterialType.NoisemakerOn:
                group.Renderer.material = Materials.NoisemakerOn;
                break;

            case MaterialType.NoisemakerOff:
                group.Renderer.material = Materials.NoisemakerOff;
                break;

            case MaterialType.SnappingPeg:
                group.Renderer.material = Materials.SnappingPeg;
                break;
        }
    }

    public static bool IsDynamicMaterialType(MaterialType type)
    {
        if (type == MaterialType.CircuitBoard || type == MaterialType.SolidColor || type == MaterialType.SnappingPeg)
        {
            return false;
        }
        return true;
    }

    public static bool IsDynamicMaterialType(MegaMeshGroup group)
    {
        return IsDynamicMaterialType(group.MaterialType);
    }

    public static bool IsDynamicMaterialType(MegaMeshComponent component)
    {
        return IsDynamicMaterialType(component.MaterialType);
    }

    // generic mesh combining code, used accross mega mesh groups, clusters and outputs

    public static CombineInstance[] GetCombineInstances(MeshFilter[] meshes)
    {
        CombineInstance[] combine = new CombineInstance[meshes.Length];
        for(int i = 0; i < meshes.Length; i++)
        {
            combine[i].mesh = meshes[i].sharedMesh;
            combine[i].transform = meshes[i].transform.localToWorldMatrix;
        }

        return combine;
    }

    public static CombineInstance[] GetCombineInstances(List<MeshFilter> meshes)
    {
        CombineInstance[] combine = new CombineInstance[meshes.Count];
        for (int i = 0; i < meshes.Count; i++)
        {
            combine[i].mesh = meshes[i].sharedMesh;
            combine[i].transform = meshes[i].transform.localToWorldMatrix;
        }

        return combine;
    }

    public static CombineInstance[] GetScaledCombineInstances(List<MeshFilter> meshes, float scale)
    {
        CombineInstance[] combine = new CombineInstance[meshes.Count];
        for (int i = 0; i < meshes.Count; i++)
        {
            combine[i].mesh = meshes[i].sharedMesh;

            meshes[i].transform.localScale *= scale;
            combine[i].transform = meshes[i].transform.localToWorldMatrix;
            meshes[i].transform.localScale *= 1 / scale;
        }

        return combine;
    }

    public static void ScaleMesh(MeshFilter MeshFilter, float scale)
    {
        Vector3[] OriginalVertices = MeshFilter.mesh.vertices;
        Vector3[] ScaledVertices = new Vector3[OriginalVertices.Length];

        for (int i = 0; i < ScaledVertices.Length; i++)
        {
            ScaledVertices[i] = OriginalVertices[i] * scale;
        }

        MeshFilter.mesh.vertices = ScaledVertices;
    }

    //public static CombineInstance[] GetCombineInstancesAndDisableRenderers(List<MegaMeshComponent> components)
    //{
    //    CombineInstance[] combine = new CombineInstance[components.Count];
    //    int i = 0;
    //    foreach(MegaMeshComponent component in components) // since we do several things with the component, getting a single reference to it with foreach is faster than looking it up by address multiple times
    //    {
    //        combine[i].mesh = component.MeshFilter.sharedMesh;
    //        combine[i].transform = component.transform.localToWorldMatrix;
    //        component.Renderer.enabled = false;
    //        i++;
    //    }

    //    return combine;
    //}

    public static CombineInstance[] GetCombineInstancesAndDisableRenderers(HashSet<MegaMeshComponent> components)
    {
        int i = 0;
        CombineInstance[] combine = new CombineInstance[components.Count];
        foreach (MegaMeshComponent component in components)
        {
            combine[i].mesh = component.Mesh;
            combine[i].transform = component.transform.localToWorldMatrix;
            component.Renderer.enabled = false;
            i++;
        }

        return combine;
    }
}

public enum MaterialType
{
    SolidColor,
    CircuitBoard,

    CircuitOn,
    CircuitOff,

    DisplayOff,
    DisplayRed,
    DisplayGreen,
    DisplayBlue,
    DisplayYellow,
    DisplayOrange,
    DisplayPurple,
    DisplayWhite,
    DisplayCyan,

    NoisemakerOn,
    NoisemakerOff,

    SnappingPeg,

    Unknown
}