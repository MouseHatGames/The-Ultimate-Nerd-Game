// todo: comment this whole thing...

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MegaBoardMeshManager {

    public static Dictionary<Color, MegaBoardMesh> MegaBoardMeshesOfColor = new Dictionary<Color, MegaBoardMesh>();

    public static GameObject MegaBoardMeshPrefab;
    public static Transform MegaMeshesParent;

    private static void AddNewColor(Color newcolor)
    {
        GameObject newMegaBoardMesh = Object.Instantiate(MegaBoardMeshPrefab, MegaMeshesParent);
        newMegaBoardMesh.GetComponent<Renderer>().material.color = newcolor;
        MegaBoardMeshesOfColor.Add(newcolor, newMegaBoardMesh.GetComponent<MegaBoardMesh>());
    }

    public static void AddToMegaBoardMeshList(CircuitBoard board)
    {
        if (!MegaBoardMeshesOfColor.ContainsKey(board.BoardColor))
        {
            AddNewColor(board.BoardColor);
        }

        if (!MegaBoardMeshesOfColor[board.BoardColor].IncludedBoards.Contains(board))
        {
            MegaBoardMeshesOfColor[board.BoardColor].IncludedBoards.Add(board);
        }
    }

    public static void RemoveFromMegaBoardMeshList(CircuitBoard board)
    {
        if (MegaBoardMeshesOfColor.ContainsKey(board.BoardColor))
        {
            MegaBoardMeshesOfColor[board.BoardColor].IncludedBoards.Remove(board);
        }
    }

    public static void AddBoard(CircuitBoard board)
    {
        AddToMegaBoardMeshList(board);
        MegaBoardMeshesOfColor[board.BoardColor].RecalculateMegaMesh();
        board.Renderer.enabled = false;
    }

    public static void AddBoards(CircuitBoard[] boards)
    {
        List<Color> ColorsOfAddedBoards = new List<Color>();
        foreach(CircuitBoard board in boards)
        {
            AddToMegaBoardMeshList(board);
            ColorsOfAddedBoards.Add(board.BoardColor);
            board.Renderer.enabled = false;
        }

        Color[] DistinctColorsOfAddedBoards = ColorsOfAddedBoards.Distinct().ToArray();
        foreach(Color c in DistinctColorsOfAddedBoards)
        {
            MegaBoardMeshesOfColor[c].RecalculateMegaMesh();
        }
    }

    public static void RemoveBoard(CircuitBoard board)
    {
        RemoveFromMegaBoardMeshList(board);
        if (MegaBoardMeshesOfColor.ContainsKey(board.BoardColor)) { MegaBoardMeshesOfColor[board.BoardColor].RecalculateMegaMesh(); }
        board.Renderer.enabled = true;
    }

    public static void RemoveBoards(CircuitBoard[] boards)
    {
        List<Color> ColorsOfRemovedBoards = new List<Color>();
        foreach (CircuitBoard board in boards)
        {
            RemoveFromMegaBoardMeshList(board);
            ColorsOfRemovedBoards.Add(board.BoardColor);
            board.Renderer.enabled = true;
        }

        Color[] DistinctColorsOfAddedBoards = ColorsOfRemovedBoards.Distinct().ToArray();
        foreach (Color c in DistinctColorsOfAddedBoards)
        {
            MegaBoardMeshesOfColor[c].RecalculateMegaMesh();
        }
    }

    public static void AddBoardsFrom(GameObject go)
    {
        AddBoards(go.GetComponentsInChildren<CircuitBoard>());
    }

    public static void RemoveBoardsFrom(GameObject go)
    {
        RemoveBoards(go.GetComponentsInChildren<CircuitBoard>());
    }

    public static void GenerateAllMegaBoardMeshes()
    {
        // for now, we assume MegaBoardMeshesOfColor is empty. This method is intended to be called only when the scene is loaded so it shouldn't be an issue.
        AddBoards(Object.FindObjectsOfType<CircuitBoard>());
        Debug.Log("generated all mega board meshes");
    }
}