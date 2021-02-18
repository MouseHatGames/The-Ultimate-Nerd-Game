using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BoardFunctions
{
    public static void CreateNewBoard(int x, int z)
    {
        GameObject NewBoard = Object.Instantiate(References.Prefabs.CircuitBoard, BoardPlacer.ReferenceObject.transform);

        // set the board's dimensions
        CircuitBoard board = NewBoard.GetComponent<CircuitBoard>();
        board.x = x;
        board.z = z;
        board.CreateCuboid();

        BoardPlacer.NewBoardBeingPlaced(NewBoard);
    }

    // sets the board being looked at to the board being placed so it can be moved/rotated/deleted without removing all its children first
    public static void MoveExistingBoard()
    {
        RaycastHit hit;
        if (Physics.Raycast(FirstPersonInteraction.Ray(), out hit, Settings.ReachDistance, Wire.IgnoreWiresLayermask))
        {
            if (hit.collider.tag == "CircuitBoard")
            {
                GameObject MoveThis = hit.collider.gameObject;
                if (Input.GetButton("Mod"))
                {
                    GameObject RootBoard = hit.collider.transform.root.gameObject;
                    if (RootBoard.tag == "CircuitBoard") { MoveThis = RootBoard; } // make sure to check for circuitboard in case of mounts
                }
                BoardPlacer.NewBoardBeingPlaced(MoveThis);

                GameplayUIManager.UIState = UIState.BoardBeingPlaced;
                return;
            }
        }

        SoundPlayer.PlaySoundGlobal(References.Sounds.FailDoSomething);
        GameplayUIManager.UIState = UIState.None;
    }

    public static void CloneBoard()
    {
        RaycastHit hit;
        if (Physics.Raycast(FirstPersonInteraction.Ray(), out hit, Settings.ReachDistance, Wire.IgnoreWiresLayermask))
        {
            if (hit.collider.tag == "CircuitBoard")
            {
                GameObject CloneThis = hit.collider.gameObject;
                if (Input.GetButton("Mod"))
                {
                    GameObject RootBoard = hit.collider.transform.root.gameObject;
                    if (RootBoard.tag == "CircuitBoard") { CloneThis = RootBoard; }
                }

                GameObject newboard = Object.Instantiate(CloneThis, new Vector3(0, -2000, 0), Quaternion.identity); // instantiated so far away so that nothing can interfere with connection.findpoints's raycasts
                RecalculateClustersOfBoard(newboard);
                BoardPlacer.NewBoardBeingPlaced(newboard);

                GameplayUIManager.UIState = UIState.BoardBeingPlaced;
                return;
            }
        }

        SoundPlayer.PlaySoundGlobal(References.Sounds.FailDoSomething);
        GameplayUIManager.UIState = UIState.None;
    }

    // finds all connections connected to an input or output which is a child of BoardBeingPlaced. If that connection does not have BoardBeingPlaced above it in the heirarchy, destroy it
    public static void DestroyAllWiresConnectedToBoardButNotPartOfIt(GameObject board)
    {
        CircuitInput[] Inputs = board.GetComponentsInChildren<CircuitInput>();
        CircuitOutput[] Outputs = board.GetComponentsInChildren<CircuitOutput>();

        // annoying InvalidOperationException protection
        List<InputInputConnection> IIConnectionsToDestroy = new List<InputInputConnection>();
        List<InputOutputConnection> IOConnectionsToDestroy = new List<InputOutputConnection>();

        foreach (CircuitInput input in Inputs)
        {
            foreach (InputInputConnection connection in input.IIConnections)
            {
                if (!StuffConnector.IsChildOf(connection.transform, board.transform))
                {
                    IIConnectionsToDestroy.Add(connection);
                }
            }

            foreach (InputOutputConnection connection in input.IOConnections)
            {
                if (!StuffConnector.IsChildOf(connection.transform, board.transform))
                {
                    IOConnectionsToDestroy.Add(connection);
                }
            }
        }

        foreach (CircuitOutput output in Outputs)
        {
            foreach (InputOutputConnection connection in output.GetIOConnections())
            {
                if (!StuffConnector.IsChildOf(connection.transform, board.transform))
                {
                    IOConnectionsToDestroy.Add(connection);
                }
            }
        }

        foreach (InputInputConnection connection in IIConnectionsToDestroy)
        {
            StuffDeleter.DestroyIIConnection(connection);
        }
        foreach (InputOutputConnection connection in IOConnectionsToDestroy)
        {
            StuffDeleter.DestroyIOConnection(connection);
        }
    }

    public static void RecalculateClustersOfBoard(GameObject board)
    {
        foreach (WireCluster cluster in board.GetComponentsInChildren<WireCluster>())
        {
            cluster.transform.parent = null;
            Object.Destroy(cluster.gameObject);
        }

        // get all the stuff that needs to have its clusters recalculated
        CircuitInput[] inputs = board.GetComponentsInChildren<CircuitInput>();
        CircuitOutput[] outputs = board.GetComponentsInChildren<CircuitOutput>();
        Wire[] Wires = board.GetComponentsInChildren<Wire>();

        // clear the connections of each input and output, since they'll still have stuff from their cloned boards
        foreach (CircuitInput input in inputs)
        {
            input.IIConnections.Clear();
            input.IOConnections.Clear();
        }
        foreach (CircuitOutput output in outputs)
        {
            output.ClearIOConnections();
        }

        // have the connections find their new points, since they'll still have their points set as the boards they were cloned from
        foreach (Wire wire in Wires)
        {
            wire.FindPoints();
        }

        StuffDeleter.RecalculateClustersFromInputs(inputs); // todo: since we've already gotten the lists of wires, it would be more efficient to pass them to this function instead of letting it re-find them
    }
}