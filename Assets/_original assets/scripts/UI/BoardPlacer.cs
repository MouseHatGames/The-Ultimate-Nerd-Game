using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardPlacer : MonoBehaviour {

    // the board being placed right now. Can be a new board or a cloned board.
    public static GameObject BoardBeingPlaced;
    public static CircuitBoard CircuitBoardBeingPlaced;

    // the reference object for the board being placed, to simplify rotation/translation code
    private static GameObject ReferenceObject;

    // prefab of a board object
    public GameObject BoardPrefab;

    public Camera FirstPersonCamera;

    private void Start()
    {
        ReferenceObject = new GameObject("ReferenceObject");
    }

    private void Update()
    {
        if(BoardBeingPlaced != null)
        {
            BoardPlacing(); // every frame for which we have a board to place, run the code to adjust its position
        }
    }

    void NewBoardBeingPlaced()
    {
        BoardBeingPlaced.transform.parent = ReferenceObject.transform;
        CircuitBoardBeingPlaced = BoardBeingPlaced.GetComponent<CircuitBoard>();

        MegaMesh.RemoveMeshesFrom(BoardBeingPlaced);
        SetChildCircuitsMegaMeshStatus(BoardBeingPlaced, false);

        SetRotationState();
        OutlineBoard(true);
        SetStateOfAllColliders(BoardBeingPlaced, false);
        PlacingOffset = Vector2Int.zero;

        if (!BoardBeingPlaced.GetComponent<SaveThisObject>())
        {
            BoardBeingPlaced.AddComponent<SaveThisObject>().ObjectType = "CircuitBoard";
        }

        if (!HelpMenu.LockOpenMenu) { HelpMenu.Instance.ShowBoardPlacing(); }
    }

    public void CreateNewBoard(int x, int z)
    {
        Destroy(BoardBeingPlaced);

        BoardBeingPlaced = Instantiate(BoardPrefab, ReferenceObject.transform);

        // set the board's dimensions
        CircuitBoard board = BoardBeingPlaced.GetComponent<CircuitBoard>();
        board.x = x;
        board.z = z;
        board.CreateCuboid();

        NewBoardBeingPlaced();
    }

    int layerMask = 1 << 0; // cast against only the default layer
    // handles the first person placing of boards.
    void BoardPlacing()
    {
        if(BoardBeingPlaced == null)
        {
            return;
        }

        // much of this code is the same as that in FirstPersonInteraction. TODO: find a pleasing way to merge this script and FirstPersonInteraction and StuffPlacer...
        RaycastHit hit;
        Transform cam = FirstPersonCamera.transform;
        if (Physics.Raycast(cam.position, cam.forward, out hit, MiscellaneousSettings.ReachDistance, layerMask))
        {
            if (hit.collider.tag == "BoardObject" || hit.collider.tag == "Wire" || hit.collider.tag == "Input" || hit.collider.tag == "Output") // you cannot place on these tags so...
            {
                BoardBeingPlaced.SetActive(false); // ... make it disappear if it's on them
                return;
            }

            BoardBeingPlaced.SetActive(true); // if the cursor moves from an invalid object to a valid one, set it back to active

            if (hit.collider.tag == "CircuitBoard")
            {
                MoveBoardOnBoard(hit);
            }
            else
            {
                MoveBoardOnOther(hit);
            }

            if (Input.GetButtonDown("Rotate"))
            {
                RotateBoard();
            }

            if (Input.GetButtonDown("ToggleBoardFlatness"))
            {
                BoardIsFlat = !BoardIsFlat;
            }

            if (Input.GetButtonDown("BoardRotation"))
            {
                if(Input.GetAxis("BoardRotation") > 0)
                {
                    RotationState += 1;
                }
                else if(Input.GetAxis("BoardRotation") < 0)
                {
                    RotationState -= 1;
                }

                SetRotationState();
            }

            if (!UIManager.SomeOtherMenuIsOpen) // so that this can't be done in the new board menu
            {
                if (Input.GetButtonDown("Place") && !BoardBeingPlacedIntersectingStuff())
                {
                    PlaceBoard(hit);
                }

                if (Input.GetButtonDown("Delete") || Input.GetButtonDown("Cancel"))
                {
                    CancelPlacement();
                }

                if (Input.GetAxis("Mouse ScrollWheel") > 0 || BuildMenu.KeyboardScrollUp()) // scroll up
                {
                    if (Input.GetButton("Mod")) { PlacingOffset.y++; }
                    else { PlacingOffset.x++; }
                    CapPlacingOffset();
                }

                if (Input.GetAxis("Mouse ScrollWheel") < 0 || BuildMenu.KeyboardScrollDown()) // scroll down
                {
                    if (Input.GetButton("Mod")) { PlacingOffset.y--; }
                    else { PlacingOffset.x--; }
                    CapPlacingOffset();
                }

                if (Input.GetButtonDown("ResetBoardOffset")) // middle click
                {
                    PlacingOffset = Vector2Int.zero;
                }
            }
        }
        // don't show the board if the raycast fails
        else
        {
            BoardBeingPlaced.SetActive(false);
        }
    }

    void CapPlacingOffset()
    {
        int MaxX, MaxY;
        if (RotationState == 1 || RotationState == 3)
        {
            MaxX = Mathf.FloorToInt(CircuitBoardBeingPlaced.x / 2);
            MaxY = Mathf.FloorToInt(CircuitBoardBeingPlaced.z / 2);
        }
        else
        {
            MaxX = Mathf.FloorToInt(CircuitBoardBeingPlaced.z / 2);
            MaxY = Mathf.FloorToInt(CircuitBoardBeingPlaced.x / 2);
        }

        if (PlacingOffset.x > MaxX) { PlacingOffset.x = MaxX; }
        if (PlacingOffset.x < -MaxX) { PlacingOffset.x = -MaxX; }

        if (PlacingOffset.y > MaxY) { PlacingOffset.y = MaxY; }
        if (PlacingOffset.y < -MaxY) { PlacingOffset.y = -MaxY; }
    }

    void ApplyPlacingOffset()
    {
        if (BoardIsFlat)
        {
            if (FlatBoardUpsideDown)
            {
                ReferenceObject.transform.Translate(ReferenceObject.transform.up * PlacingOffset.y * 0.3f, Space.World);
                ReferenceObject.transform.Translate(-ReferenceObject.transform.forward * PlacingOffset.x * 0.3f, Space.World);
            }
            else
            {
                ReferenceObject.transform.Translate(ReferenceObject.transform.up * PlacingOffset.y * 0.3f, Space.World);
                ReferenceObject.transform.Translate(ReferenceObject.transform.forward * PlacingOffset.x * 0.3f, Space.World);
            }
        }
        else
        {
            ReferenceObject.transform.Translate(new Vector3(0, 0, PlacingOffset.x) * 0.3f, Space.Self);
        }
    }

    public Vector2Int PlacingOffset;

    // lots of this is copied from StuffPlacer.PlaceOnBoard. TODO: find a way to merge it in a nice way
    void MoveBoardOnBoard(RaycastHit hit)
    {
        // some important variables for later in the function
        Vector3 LocalPosition = hit.collider.transform.InverseTransformPoint(hit.point);
        CircuitBoard ParentBoard = hit.collider.gameObject.GetComponent<CircuitBoard>(); // TODO: cache this! GetComponent is leaky, and this line is run EVERY FRAME!
        Vector2Int BoardCoordinates = new Vector2Int(Mathf.RoundToInt((LocalPosition.x - 0.15f) / 0.3f), Mathf.RoundToInt((LocalPosition.z - 0.15f) / 0.3f)); // get the integer coordinates on the board by converting from the 0.3 scale

        // if the board is being placed on the side of another board, set the rotation and position of the board being placed accordingly

        float YPosition = 0;
        bool edge = true;
        Vector3 LocalRotation = Vector3.zero;

        Transform parentboard = hit.collider.transform;
        if (hit.normal == parentboard.right) { LocalRotation = BoBData.Right; }
        else if (hit.normal == -parentboard.right) { LocalRotation = BoBData.Left; }
        else if (hit.normal == parentboard.forward) { LocalRotation = BoBData.Forward; }
        else if (hit.normal == -parentboard.forward) { LocalRotation = BoBData.Back; }
        else
        {
            YPosition = LocalPosition.y;
            edge = false;
            if (YPosition > 0) { LocalRotation = BoBData.Top; }
            else { LocalRotation = BoBData.Bottom; }
        }

        // cap the coordinates before determining the position
        if (BoardCoordinates.x >= ParentBoard.x) { BoardCoordinates.x = ParentBoard.x - 1; }
        if (BoardCoordinates.y >= ParentBoard.z) { BoardCoordinates.y = ParentBoard.z - 1; }
        if (BoardCoordinates.x < 0) { BoardCoordinates.x = 0; }
        if (BoardCoordinates.y < 0) { BoardCoordinates.y = 0; }

        ReferenceObject.transform.parent = hit.collider.transform;
        ReferenceObject.transform.localPosition = new Vector3(BoardCoordinates.x + 0.5f, YPosition / 0.3f, BoardCoordinates.y + 0.5f) * 0.3f; // the +0.5fs are to make it within the grid, not on the lines
        ReferenceObject.transform.localEulerAngles = LocalRotation;

        if (BoardIsFlat)
        {
            ReferenceObject.transform.Rotate(ReferenceObject.transform.forward, 90, Space.World); // make the board flat

            // position - board center should be at BoardCoordinates
            if (RotationState == 0 || RotationState == 2)
            {
                ReferenceObject.transform.Translate(-ReferenceObject.transform.up * (CircuitBoardBeingPlaced.x / 2 + 0.5f) * 0.3f, Space.World);
            }
            else
            {
                ReferenceObject.transform.Translate(-ReferenceObject.transform.up * (CircuitBoardBeingPlaced.z / 2 + 0.5f) * 0.3f, Space.World);
            }

            // rotation
            if(FlatBoardUpsideDown)
            {
                ReferenceObject.transform.Rotate(ReferenceObject.transform.up, 180, Space.World);
                ReferenceObject.transform.Translate(-ReferenceObject.transform.right * 0.15f, Space.World); // for some reason the previous rotation sinks it into the parent board. No idea why. This is to correct that
            }

            // make it rise out of the board so it's on top, not inside
            ReferenceObject.transform.Translate(ReferenceObject.transform.right * 0.075f, Space.World);
        }
        else
        {
            ReferenceObject.transform.Rotate(ReferenceObject.transform.up, StandingRotation, Space.World);
        }

        if (edge) { ReferenceObject.transform.Translate(hit.normal * 0.15f, Space.World); } // since the boardcoordinates are capped, edge boards will be placed by default half a unit inside the board. This fixes that

        ApplyPlacingOffset();
    }

    void MoveBoardOnOther(RaycastHit hit)
    {
        ReferenceObject.transform.position = hit.point;
        ReferenceObject.transform.up = hit.normal;

        ReferenceObject.transform.Rotate(ReferenceObject.transform.up, FirstPersonCamera.transform.parent.localEulerAngles.y, Space.World); // rotate to follow the camera

        ReferenceObject.transform.parent = null;

        if (BoardIsFlat)
        {
            ReferenceObject.transform.Rotate(ReferenceObject.transform.forward, 90, Space.World); // make the board flat

            // position - board center should be at BoardCoordinates
            if (RotationState == 0 || RotationState == 2)
            {
                ReferenceObject.transform.Translate(-ReferenceObject.transform.up * (CircuitBoardBeingPlaced.x / 2 + 0.5f) * 0.3f, Space.World);
            }
            else
            {
                ReferenceObject.transform.Translate(-ReferenceObject.transform.up * (CircuitBoardBeingPlaced.z / 2 + 0.5f) * 0.3f, Space.World);
            }

            ReferenceObject.transform.Translate(ReferenceObject.transform.right * 0.075f, Space.World);

            // rotation
            if (FlatBoardUpsideDown)
            {
                ReferenceObject.transform.Rotate(ReferenceObject.transform.up, 180, Space.World);
            }
        }
        else
        {
            ReferenceObject.transform.Rotate(ReferenceObject.transform.up, StandingRotation, Space.World);
        }

        ApplyPlacingOffset();
    }

    void PlaceBoard(RaycastHit hit)
    {
        OutlineBoard(false);
        // deparent from reference object. If on a circuitboard, parent it to that
        if (hit.collider.tag == "CircuitBoard")
        {
            BoardBeingPlaced.transform.parent = hit.collider.transform;
        }
        else
        {
            BoardBeingPlaced.transform.parent = null;
        }

        // sometimes the scale of the board being placed can get distorted. I have no idea why. This is an attempt to fix that
        // probably fixed when I fixed being able to open the board menu while placing a board
        BoardBeingPlaced.transform.localScale = Vector3.one;

        SetStateOfAllColliders(BoardBeingPlaced, true);

        StuffPlacer.DestroyIntersectingConnections(BoardBeingPlaced);

        DestroyInvalidWiresOnBoard();

        MegaMesh.AddMeshesFrom(BoardBeingPlaced);
        MegaBoardMeshManager.AddBoardsFrom(BoardBeingPlaced);
        SetChildCircuitsMegaMeshStatus(BoardBeingPlaced, true);

        BoardBeingPlaced = null;

        ReferenceObject.transform.parent = null; // this is important because a circuitboard having any children means it can't be deleted, and this can fuck that up

        Input.ResetInputAxes(); // so that the click to place a board doesn't also immediately place the selected object

        HelpMenu.Instance.ShowDefault();
    }

    public static void CancelPlacement()
    {
        // so that we don't end up with empty clusters when deleting boards.
        // this code is from stuffdeleter.cs. TODO: merge in a nice way
        CircuitInput[] inputs = BoardBeingPlaced.GetComponentsInChildren<CircuitInput>();
        Output[] outputs = BoardBeingPlaced.GetComponentsInChildren<Output>();
        foreach (CircuitInput input in inputs)
        {
            StuffDeleter.DestroyInput(input);
        }
        foreach (Output output in outputs)
        {
            StuffDeleter.DestroyOutput(output);
        }


        Destroy(BoardBeingPlaced);
        HelpMenu.Instance.ShowDefault();

        Input.ResetInputAxes(); // to prevent deleting the board underneath if it exists; stops StuffDeleter from doing its job. This is a bad solution but will be made better with 0.2's code overhaul
        ReferenceObject.transform.parent = null; // without this line the referenceobject is left on the board and you are unable to delete it without move board
    }

    // sets the board being looked at to the board being placed so it can be moved/rotated/deleted without removing all its children first
    public void MoveExistingBoard()
    {
        RaycastHit hit;
        Transform cam = FirstPersonCamera.transform;
        if (Physics.Raycast(cam.position, cam.forward, out hit, MiscellaneousSettings.ReachDistance, layerMask))
        {
            if(hit.collider.tag == "CircuitBoard")
            {
                BoardBeingPlaced = hit.collider.gameObject;
                DestroyAllWiresConnectedToBoardButNotPartOfIt();

                NewBoardBeingPlaced();
                MegaBoardMeshManager.RemoveBoardsFrom(BoardBeingPlaced);
                
                Invoke("NewBoardBeingPlaced", 0.05f);
                // a very shitty fix for a very shitty problem. The first instance of NewBoardBeingPlaced doesn't parent BoardBeingPlaced to ReferenceObject if the board has gained
                // a child board since it was last moved. No idea why.
            }
        }
    }

    public void CloneBoard()
    {
        RaycastHit hit;
        Transform cam = FirstPersonCamera.transform;
        if (Physics.Raycast(cam.position, cam.forward, out hit, MiscellaneousSettings.ReachDistance, layerMask))
        {
            if (hit.collider.tag == "CircuitBoard")
            {
                BoardBeingPlaced = Instantiate(hit.collider.gameObject, new Vector3(0, -2000, 0), Quaternion.identity); // instantiated so far away so that nothing can interfere with connection.findpoints's raycasts

                foreach(WireCluster cluster in BoardBeingPlaced.GetComponentsInChildren<WireCluster>())
                {
                    Destroy(cluster.gameObject);
                }
                foreach(CircuitBoard board in BoardBeingPlaced.GetComponentsInChildren<CircuitBoard>())
                {
                    board.Renderer.enabled = true; // this will by default be disabled, since the board being cloned is part of a mega board mesh
                }


                RecalculateClustersOfCurrentBoard();

                NewBoardBeingPlaced();
                Invoke("NewBoardBeingPlaced", 0.1f); // shitty fix for a shitty bug. When clusters are recalculated, they lose the state of AllowedToCombineOnStable or whatever the variable is.
                // this sets it proper after it's been recalculated and whatnot idk fuck coding
            }
        }
    }

    bool BoardIsFlat;

    float StandingRotation = 90; // initial value is 90 so that if the first board is placed on terrain it will be facing the camera
    bool FlatBoardUpsideDown; // this could simply be a bool called UpsideDown...
    void RotateBoard()
    {
        if (Input.GetAxis("Rotate") > 0)
        {
            if (BoardIsFlat) { FlatBoardUpsideDown = !FlatBoardUpsideDown; }
            else { StandingRotation += 90; }
        }
        else if(Input.GetAxis("Rotate") < 0) // notice how nothing happens if the axis is zero
        {
            if (BoardIsFlat) { FlatBoardUpsideDown = !FlatBoardUpsideDown; }
            else { StandingRotation -= 90; }
        }
    }

    // for rotating along the awkward axis
    int RotationState = 1;
    void SetRotationState()
    {
        // keep it within the range
        while(RotationState > 3)
        {
            RotationState -= 4;
        }
        while(RotationState < 0)
        {
            RotationState += 4;
        }

        CircuitBoard board = BoardBeingPlaced.GetComponent<CircuitBoard>(); // it is gross that this class has so many places where it gets the component of BoardBeingPlaced. TODO: cache it, along with unified code for a new board being set as BoardBeingPlaced

        // possible cases
        if(RotationState == 0)
        {
            BoardBeingPlaced.transform.localEulerAngles = new Vector3(0, 0, 90); // makes the board stand up instead of laying down, so ReferenceObject.up points along the y axis of the board
            BoardBeingPlaced.transform.localPosition = new Vector3(0, 0, -(board.z / 2) * 0.3f - 0.15f);
        }
        else if(RotationState == 1)
        {
            BoardBeingPlaced.transform.localEulerAngles = new Vector3(90, 0, 90);
            BoardBeingPlaced.transform.localPosition = new Vector3(0, board.z * 0.3f, -(board.x / 2) * 0.3f - 0.15f);
        }
        else if(RotationState == 2)
        {
            BoardBeingPlaced.transform.localEulerAngles = new Vector3(180, 0, 90);
            BoardBeingPlaced.transform.localPosition = new Vector3(0, board.x * 0.3f, (board.z / 2) * 0.3f + 0.15f);
        }
        else if(RotationState == 3)
        {
            BoardBeingPlaced.transform.localEulerAngles = new Vector3(270, 0, 90);
            BoardBeingPlaced.transform.localPosition = new Vector3(0, 0, (board.x / 2) * 0.3f + 0.15f);
        }
        else
        {
            Debug.LogError("RotationState not found");
        }

        CapPlacingOffset(); // since the caps change when the rotationstate does
    }

    public LayerMask BoardBeingPlacedIntersectingStuffLayermask; // so we can include ignore raycast and get the player (boards inside the player glitch the player!)
    bool BoardBeingPlacedIntersectingStuff()
    {
        BoxCollider[] boxes = BoardBeingPlaced.GetComponentsInChildren<BoxCollider>();

        foreach(BoxCollider box in boxes)
        {
            Vector3 center = box.transform.TransformPoint(box.center);
            Vector3 halfextents = Vector3.Scale(box.size, box.transform.lossyScale) / 2;
            halfextents = Vector3.Scale(halfextents, new Vector3(0.99f, 0.99f, 0.99f)); // if the cast goes right to the edge, it'll intersect things it's on. This breaks placing on another board. This line fixes that
            Vector3 direction = box.transform.up;
            Quaternion orientation = box.transform.rotation;

            RaycastHit[] hits = Physics.BoxCastAll(center, halfextents, direction, orientation, 0, BoardBeingPlacedIntersectingStuffLayermask); // maxdistance is 0 so that the box doesn't move!
            foreach(RaycastHit hit in hits)
            {
                // ignore world colliders, otherwise it would be impossible to place boards on non-flat terrain. Ignore wires because they just get destroyed when a board is placed in them
                if (hit.collider.tag != "Wire" && hit.collider.tag != "World")
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void OutlineBoard(bool IsOutlined)
    {
        cakeslice.Outline outline = BoardBeingPlaced.GetComponent<cakeslice.Outline>();
        if (BoardBeingPlaced != null && outline != null)
        {
            outline.enabled = IsOutlined; // without this line, turning off the outline is extremely glitchy in puzzling ways (at least in 2017.2.0p1 - hopefully it's fixed in future versions)
            outline.eraseRenderer = !IsOutlined;
        }
    }

    // this also has the effect of the placement raycast ignoring the board being placed
    public void SetStateOfAllColliders(GameObject ThingToDisableAllBoxCollidersOf, bool enabled)
    {
        BoxCollider[] colliders = ThingToDisableAllBoxCollidersOf.GetComponentsInChildren<BoxCollider>();

        foreach(BoxCollider collider in colliders)
        {
            collider.enabled = enabled;
        }

        // also the colliders in wires
        MeshCollider[] mcolliders = ThingToDisableAllBoxCollidersOf.GetComponentsInChildren<MeshCollider>();

        foreach(MeshCollider collider in mcolliders)
        {
            collider.enabled = enabled;
        }
    }

    // finds all connections connected to an input or output which is a child of BoardBeingPlaced. If that connection does not have BoardBeingPlaced above it in the heirarchy, destroy it
    public void DestroyAllWiresConnectedToBoardButNotPartOfIt()
    {
        CircuitInput[] Inputs = BoardBeingPlaced.GetComponentsInChildren<CircuitInput>();
        Output[] Outputs = BoardBeingPlaced.GetComponentsInChildren<Output>();

        // annoying InvalidOperationException protection
        List<InputInputConnection> IIConnectionsToDestroy = new List<InputInputConnection>();
        List<InputOutputConnection> IOConnectionsToDestroy = new List<InputOutputConnection>();

        foreach(CircuitInput input in Inputs)
        {
            foreach(InputInputConnection connection in input.IIConnections)
            {
                if(!StuffConnecter.IsChildOf(connection.transform, BoardBeingPlaced.transform))
                {
                    IIConnectionsToDestroy.Add(connection);
                }
            }

            foreach(InputOutputConnection connection in input.IOConnections)
            {
                if(!StuffConnecter.IsChildOf(connection.transform, BoardBeingPlaced.transform))
                {
                    IOConnectionsToDestroy.Add(connection);
                }
            }
        }

        foreach(Output output in Outputs)
        {
            foreach(InputOutputConnection connection in output.GetIOConnections())
            {
                if(!StuffConnecter.IsChildOf(connection.transform, BoardBeingPlaced.transform))
                {
                    IOConnectionsToDestroy.Add(connection);
                }
            }
        }

        foreach(InputInputConnection connection in IIConnectionsToDestroy)
        {
            StuffDeleter.DestroyIIConnection(connection);
        }
        foreach(InputOutputConnection connection in IOConnectionsToDestroy)
        {
            StuffDeleter.DestroyIOConnection(connection);
        }
    }

    // checks all wires on the board and destroys any that can't connect
    public void DestroyInvalidWiresOnBoard()
    {
        InputInputConnection[] IIConnections = BoardBeingPlaced.GetComponentsInChildren<InputInputConnection>();
        InputOutputConnection[] IOConnections = BoardBeingPlaced.GetComponentsInChildren<InputOutputConnection>();

        foreach (InputInputConnection connection in IIConnections)
        {
            if (!StuffConnecter.CanConnect(connection.gameObject))
            {
                StuffDeleter.DestroyIIConnection(connection);
            }
        }
        foreach(InputOutputConnection connection in IOConnections)
        {
            if (!StuffConnecter.CanConnect(connection.gameObject))
            {
                StuffDeleter.DestroyIOConnection(connection);
            }
        }
    }
    
    public void RecalculateClustersOfCurrentBoard()
    {
        // create a new cluster for RecalculateCluster to use
        WireCluster cluster = Instantiate(StuffConnecter.ClusterPrefab).GetComponent<WireCluster>();

        // get all the stuff that needs to have its clusters recalculated
        CircuitInput[] inputs = BoardBeingPlaced.GetComponentsInChildren<CircuitInput>();
        Output[] outputs = BoardBeingPlaced.GetComponentsInChildren<Output>();
        InputInputConnection[] IIConnections = BoardBeingPlaced.GetComponentsInChildren<InputInputConnection>();
        InputOutputConnection[] IOConnections = BoardBeingPlaced.GetComponentsInChildren<InputOutputConnection>();

        // clear the connections of each input and output, since they'll still have stuff from their cloned boards
        foreach (CircuitInput input in inputs)
        {
            input.IIConnections.Clear();
            input.IOConnections.Clear();
        }
        foreach (Output output in outputs)
        {
            output.ClearIOConnections();
        }

        // have the connections find their new points, since they'll still have their points set as the boards they were cloned from
        foreach (InputInputConnection connection in IIConnections)
        {
            connection.FindPoints();
        }
        foreach(InputOutputConnection connection in IOConnections)
        {
            connection.FindPoints();
        }

        // add everything to the new cluster
        foreach(CircuitInput input in inputs)
        {
            cluster.ConnectInput(input);
        }
        foreach(Output output in outputs)
        {
            cluster.ConnectOutput(output);
        }

        // finally, recalculate that cluster
        StuffDeleter.RecalculateCluster(cluster);

        StartCoroutine(ValidateInputRendererState(inputs));
    }

    // a shitty fix for inputs of cloned boards without a cluster are invisible. Somewhere in the cluster mesh recalculation code, it thinks that these inputs are in a cluster
    // when they are not and so it sets their renderers off. This function fixes that the next frame.
    public static IEnumerator ValidateInputRendererState(CircuitInput[] inputs)
    {
        yield return new WaitForEndOfFrame();
        foreach (CircuitInput input in inputs)
        {
            if (input != null && input.Cluster == null)
            {
                input.Renderer.enabled = true;
            }
        }
    }

    // so that StuffRotater can use this to fix a similar bug. God I can't wait to go through ALL of the code and clean it up
    public static BoardPlacer Instance;
    private void Awake()
    {
        Instance = this;
    }
    public void ValidateInputRendererStateAndStuff(CircuitInput[] inputs) { StartCoroutine(ValidateInputRendererState(inputs)); }

    // used in NewBoardBeingPlaced and PlaceBoard so that when you move/clone a board its circuits don't stay behind!
    private void SetChildCircuitsMegaMeshStatus(GameObject go, bool AllowedToBePartOf)
    {
        WireCluster[] clusters = go.GetComponentsInChildren<WireCluster>();
        Output[] outputs = go.GetComponentsInChildren<Output>();

        foreach(WireCluster cluster in clusters)
        {
            cluster.AutoCombineOnStableAllowed = AllowedToBePartOf;
        }
        foreach(Output output in outputs)
        {
            output.AutoCombineOnStableAllowed = AllowedToBePartOf;
        }

        if (!AllowedToBePartOf)
        {
            foreach (WireCluster cluster in clusters) { MegaCircuitMesh.RemoveCluster(cluster); }
            foreach (Output output in outputs) { MegaCircuitMesh.RemoveOutput(output); }
        }
    }

    // man this class is getting long. Maybe it should be split up into several classes? I think conventional wisdom says that 500 lines is the max length for a class...
}

public class BoBData
{
    // rotations for all six sides of a cube, to point away from the center of the cube
    public static readonly Vector3 Top = new Vector3(0, 0, 0);
    public static readonly Vector3 Bottom = new Vector3(0, 0, 180);
    public static readonly Vector3 Forward = new Vector3(0, 270, 270);
    public static readonly Vector3 Back = new Vector3(0, 270, 90);
    public static readonly Vector3 Left = new Vector3(0, 0, 90);
    public static readonly Vector3 Right = new Vector3(0, 0, 270); 
}