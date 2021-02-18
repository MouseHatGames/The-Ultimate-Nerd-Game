// a shitty hack for mounts, since I can't get a premade mesh to work properly with the material

using UnityEngine;

public class Mount : MonoBehaviour 
{
    public bool LoadedMount = false; // set in LoadSaveObject

    private void Start()
    {
        if (!LoadedMount)// only do this for new mounts. This is uncomfortably hacky
        {
            GameObject TheBoardPart = Instantiate(References.Prefabs.CircuitBoard, transform);
            TheBoardPart.transform.localPosition = new Vector3(-0.15f, 0.65f, -0.15f);
            TheBoardPart.transform.localEulerAngles = new Vector3(0, 90, 90);
            TheBoardPart.AddComponent<ObjectInfo>().ComponentType = ComponentType.CircuitBoard;
            TheBoardPart.tag = "PlaceOnlyCircuitBoard";

            Destroy(TheBoardPart.GetComponent<MegaMeshComponent>());

            CircuitBoard board = TheBoardPart.GetComponent<CircuitBoard>();
            board.CreateCuboid();
            board.SetBoardColor(Color.white);

            // necessary for the board to appear in the selection menu
            MegaMeshManager.RemoveComponentImmediatelyOf(TheBoardPart);
            if(gameObject.layer == 5) { TheBoardPart.layer = 5; }

            if(StuffPlacer.GetThingBeingPlaced == gameObject) { StuffPlacer.NewThingBeingPlaced(gameObject); } // holy shit this is a terrible line of code. God damn.
        }
        else
        {
            transform.GetChild(1).tag = "PlaceOnlyCircuitBoard"; // this tag is applied to prevent actions like board moving and painting
        }

        Destroy(this);
    }
}