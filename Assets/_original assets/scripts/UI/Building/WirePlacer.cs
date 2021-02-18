using UnityEngine;
using References;

public static class WirePlacer
{
    private static GameObject WireBeingPlaced;
    
    private static GameObject SelectedPeg;

    private static GameObject PegBeingLookedAt;

    private static GameObject[] ObjectsInvolvedWithPlacing { get { return new GameObject[] { WireBeingPlaced, SelectedPeg, PegBeingLookedAt }; } }

    public static ConnectionMode ConnectionMode = ConnectionMode.HoldDown;

    private static bool AutoHidePlacingGhostWhileConnecting = Settings.Get("AutoHidePlacingGhostWhileConnecting", true);
    private static bool PlacingGhostWasHiddenBeforeConnecting = ComponentPlacer.ShowPlacingGhost; // necessary to avoid BUGS!

    private static void ConnectionInitial()
    {
        RaycastHit hit;
        if (!Physics.Raycast(FirstPersonInteraction.Ray(), out hit, Settings.ReachDistance, Wire.IgnoreWiresLayermask)) { return; }
        if (hit.collider.tag == "Input" || hit.collider.tag == "Output") // if it's an input or output...
        {
            SelectedPeg = hit.collider.gameObject; // ..make it the selected peg
            StuffPlacer.OutlineObject(SelectedPeg);

            PegBeingLookedAt = null; // fixes SetPegBeingLookedAt removing the outline

            if (AutoHidePlacingGhostWhileConnecting)
            {
                StuffPlacer.DeleteThingBeingPlaced();
                PlacingGhostWasHiddenBeforeConnecting = ComponentPlacer.ShowPlacingGhost;
                ComponentPlacer.ShowPlacingGhost = false;
            }
            StuffRotater.AllowedToDoRotation = false; // so you can rotate wires while placing them
            StuffDeleter.AllowedToDoDeleting = false; // prevents a bug with how null is not the same as destroyed

            SoundPlayer.PlaySoundAt(Sounds.ConnectionInitial, SelectedPeg);
        }
        else
        {
            SoundPlayer.PlaySoundGlobal(Sounds.FailDoSomething);

            DoneConnecting();
        }
    }

    private static void ConnectionFinal()
    {
        if (CurrentWirePlacementIsValid())
        {
            StuffPlacer.RemoveOutlineFromObject(WireBeingPlaced);
            WireBeingPlaced.GetComponent<Wire>().SetPegsBasedOnPoints();
            StuffConnector.LinkConnection(WireBeingPlaced);
            StuffConnector.SetAppropriateConnectionParent(WireBeingPlaced);

            WireBeingPlaced.AddComponent<ObjectInfo>().ComponentType = ComponentType.Wire;
            WireBeingPlaced.GetComponent<BoxCollider>().enabled = true;

            SoundPlayer.PlaySoundAt(Sounds.ConnectionFinal, WireBeingPlaced);
            WireBeingPlaced = null;
        }
        else if (SelectedPeg != null)
        {
            SoundPlayer.PlaySoundAt(Sounds.FailDoSomething, SelectedPeg); // I'm using SelectedPeg instead of WireBeingPlaced here because I'm lazy; WireBeingPlaced might not exist
        }

        if (AutoHidePlacingGhostWhileConnecting)
        {
            ComponentPlacer.ShowPlacingGhost = PlacingGhostWasHiddenBeforeConnecting;
        }

        DoneConnecting();
    }

    public static void DoneConnecting()
    {
        StuffPlacer.RemoveOutlineFromObjects(ObjectsInvolvedWithPlacing, true); // DestroyImmediate is true because more outline code is run on the same frame and it needs to know that there is no outline on some things
        if (WireBeingPlaced != null) { Object.Destroy(WireBeingPlaced); }
        SelectedPeg = null;
        PegBeingLookedAt = null;
        StuffRotater.AllowedToDoRotation = true;
        StuffDeleter.AllowedToDoDeleting = true;
    }

    private static GameObject PreviousPegBeingLookedAt;
    private static GameObject PreviousSelectedPeg;
    public static void RunWirePlacing()
    {
        if (Input.GetButtonDown("ToggleConnectionMode"))
        {
            // this is a really gross hacky way of doing it but whatever yolo am i right
            if (ConnectionMode == ConnectionMode.HoldDown) { ConnectionMode = ConnectionMode.MultiPhase; }
            else if (ConnectionMode == ConnectionMode.MultiPhase) { ConnectionMode = ConnectionMode.Chained; }
            else { ConnectionMode = ConnectionMode.HoldDown; }
        }

        // do the proper placing methods depending on the user's settings
        if (ConnectionMode == ConnectionMode.HoldDown) { HoldDownPlacing(); }
        else if (ConnectionMode == ConnectionMode.MultiPhase) { MultiphasePlacing(); }
        else if (ConnectionMode == ConnectionMode.Chained) { ChainedPlacing(); }

        if (Input.GetButtonDown("TogglePlacingGhost") && Input.GetButton("Mod"))
        {
            ShowPreWiringPegOutlines = !ShowPreWiringPegOutlines;
           // Settings.Save("ShowPreWiringPegOutlines", ShowPreWiringPegOutlines);
            if (ShowPreWiringPegOutlines) { StuffPlacer.OutlineObject(PegBeingLookedAt); }
            else { StuffPlacer.RemoveOutlineFromObject(PegBeingLookedAt); }
        }

        PollRotationInput();
        SetPegBeingLookedAt();

        // don't do all the outlining stuff if nothing's changed. A small optimization. (also it makes some code more convenient)
        if (PreviousPegBeingLookedAt == PegBeingLookedAt && PreviousSelectedPeg == SelectedPeg) { return; }
        else { PreviousPegBeingLookedAt = PegBeingLookedAt; PreviousSelectedPeg = SelectedPeg; } // keep these values updated

        // if the above things changed, odds are we need a new WireBeingPlaced
        if (WireBeingPlaced != null) { Object.Destroy(WireBeingPlaced); }

        if ((SelectedPeg == null || PegBeingLookedAt == null) // if we have less than two pegs
            || (SelectedPeg.tag == "Output" && PegBeingLookedAt.tag == "Output")) // or if our pegs are both outputs
        {
            OutlineColor color;
            if (SelectedPeg == null && PegBeingLookedAt != null) { color = OutlineColor.blue; } // if we're not in the process of placing and just looking at a peg, make it blue
            else { color = OutlineColor.red; } // otherwise - since the other possibilities of the above if statement are all invalid placements - make it red
            SetOutlineColorOfObjectsInvolvedWithPlacing(color);
            return;
        }

        PlaceNewWire();

        if (CurrentWirePlacementIsValid())
        {
            SetOutlineColorOfObjectsInvolvedWithPlacing(OutlineColor.green);
        }
        else
        {
            SetOutlineColorOfObjectsInvolvedWithPlacing(OutlineColor.red);
        }
    }

    private static void HoldDownPlacing()
    {
        if (Input.GetButtonDown("Connect"))
        {
            ConnectionInitial();
        }

        if (Input.GetButtonUp("Connect"))
        {
            ConnectionFinal();
        }
    }

    private static void MultiphasePlacing()
    {
        if (Input.GetButtonDown("Connect"))
        {
            if (SelectedPeg == null) { ConnectionInitial(); }
            else { ConnectionFinal(); }
        }
    }

    private static void ChainedPlacing()
    {
        if (Input.GetButtonDown("Connect"))
        {
            if (SelectedPeg == null) { ConnectionInitial(); }
            else { ConnectionFinal(); ConnectionInitial(); }
        }
    }


    private static bool ShowPreWiringPegOutlines = true; //= Settings.Get("ShowPreWiringPegOutlines", true);
    // if we're looking at a peg, set PegBeingLookedAt to that. Otherwise, set it to null.
    private static void SetPegBeingLookedAt()
    {
        RaycastHit hit;
        if (Physics.Raycast(FirstPersonInteraction.Ray(), out hit, Settings.ReachDistance, Wire.IgnoreWiresLayermask))
        {
            if (hit.collider.gameObject == PegBeingLookedAt || hit.collider.gameObject == SelectedPeg) { return; }

            if (hit.collider.tag == "Input" || hit.collider.tag == "Output")
            {
                StuffPlacer.RemoveOutlineFromObject(PegBeingLookedAt); // in case you look directly from one peg to another peg
                PegBeingLookedAt = hit.collider.gameObject;

                if (SelectedPeg == null && !ShowPreWiringPegOutlines) { return; }
                StuffPlacer.OutlineObject(PegBeingLookedAt);

                // play the sound only if we're in the middle of making a connection
                if (SelectedPeg != null)
                {
                    SoundPlayer.PlaySoundAt(Sounds.ConnectionInitial, PegBeingLookedAt);
                }
            }
            else
            {
                StuffPlacer.RemoveOutlineFromObject(PegBeingLookedAt);
                PegBeingLookedAt = null;
            }
        }
        else
        {
            StuffPlacer.RemoveOutlineFromObject(PegBeingLookedAt);
            PegBeingLookedAt = null;
        }
    }

    private static void PollRotationInput()
    {
        if (WireBeingPlaced == null) { return; }

        if (Input.GetButtonDown("Rotate"))
        {
            float degrees = 90;
            if (Input.GetButton("Mod")) { degrees = 22.5f; }
            if (Input.GetAxis("Rotate") < 0) { degrees = -degrees; }

            RotateWireBeingPlaced(degrees);
            PersistentDegrees += degrees;
        }
    }

    private static void RotateWireBeingPlaced(float degrees)
    {
        WireBeingPlaced.transform.RotateAround(WireBeingPlaced.transform.position, WireBeingPlaced.transform.forward, degrees);
    }

    private static float PersistentDegrees
    {
        get { return RealPersistentDegrees; }
        set // keep it between 0 and 360
        {
            while (value > 360) { value -= 360; }
            while (value < 0) { value += 360; }
            RealPersistentDegrees = value;
        }
    }
    private static float RealPersistentDegrees;

    private static void PlaceNewWire()
    {
        WireBeingPlaced = Object.Instantiate(Prefabs.Wire);
        StuffPlacer.OutlineObject(WireBeingPlaced);
        Wire addedwire = null;
        if(SelectedPeg.tag == "Input" && PegBeingLookedAt.tag == "Input")
        {
            addedwire = WireBeingPlaced.AddComponent<InputInputConnection>();

            addedwire.Point1 = Wire.GetWireReference(SelectedPeg);
            addedwire.Point2 = Wire.GetWireReference(PegBeingLookedAt);
        }
        else
        {
            addedwire = WireBeingPlaced.AddComponent<InputOutputConnection>();

            if(SelectedPeg.tag == "Input")
            {
                addedwire.Point1 = Wire.GetWireReference(SelectedPeg);
                addedwire.Point2 = Wire.GetWireReference(PegBeingLookedAt);
            }
            else
            {
                addedwire.Point2 = Wire.GetWireReference(SelectedPeg);
                addedwire.Point1 = Wire.GetWireReference(PegBeingLookedAt);
            }
        }

        addedwire.DrawWire();
        addedwire.GetComponent<BoxCollider>().enabled = false;
        RotateWireBeingPlaced(PersistentDegrees); // so if you want to place many wires in a row with custom rotation you don't have to keep giving them that custom rotation
    }

    // check if there is already a connection between two pegs
    public static bool ConnectionExists(GameObject peg1, GameObject peg2)
    {
        if (peg1 == null || peg2 == null) { return true; }
        if (peg1.tag == "Output" && peg2.tag == "Output") { return true; } // technically this should return null or something, but aside from the fact that booleans are non-nullable, we should disable any connection between two outputs

        // find the types that each thing is by trying to access its CircuitInput or Output
        CircuitInput thing1input = peg1.GetComponent<CircuitInput>();
        CircuitOutput thing1output = peg1.GetComponent<CircuitOutput>();
        CircuitInput thing2input = peg2.GetComponent<CircuitInput>();
        CircuitOutput thing2output = peg2.GetComponent<CircuitOutput>();

        if (thing1input != null && thing2input != null) // if this would be an InputInputConnection
        {
            foreach (InputInputConnection connection in thing1input.IIConnections)
            {
                // return true if any of the point's connections have the two things as points. Probably there's a terser way to do this but sadly I don't know that particular nuance of C#.
                if (connection.Input1 == thing1input && connection.Input2 == thing2input)
                {
                    return true;
                }
                if (connection.Input1 == thing2input && connection.Input2 == thing1input)
                {
                    return true;
                }
            }
        }

        else if (thing1input != null && thing2output != null) // if this would be an InputOutputConnection and thing1 is the input
        {
            foreach (InputOutputConnection connection in thing1input.IOConnections)
            {
                if (connection.Input == thing1input && connection.Output == thing2output)
                {
                    return true;
                }
            }
        }

        else if (thing1output != null && thing2input != null) // if this would be an InputOutputConnection and thing2 is the input
        {
            foreach (InputOutputConnection connection in thing2input.IOConnections)
            {
                if (connection.Input == thing2input && connection.Output == thing1output)
                {
                    return true;
                }
            }
        }

        // if none of the true cases were found, return false
        return false;
    }

    public static bool CurrentWirePlacementIsValid()
    {
        // below: some invalid placement conditions. We must do CanConnect as well as CanFindPoints because each can sometimes return true while the other returns false; sorry, future me, but I'm too lazy to type out exactly what those scenarios are. Figure it out yourself you ungrateful piece of shit
        if (ConnectionExists(SelectedPeg, PegBeingLookedAt) || !WireBeingPlaced.GetComponent<Wire>().CanFindPoints())
        {
            return false;
        }

        InputInputConnection IIconnection = WireBeingPlaced.GetComponent<InputInputConnection>();
        if (IIconnection != null)
        {
            // for the edge case of color displays
            if (ComponentPlacer.FullComponent(IIconnection.Point1) == ComponentPlacer.FullComponent(IIconnection.Point2)) { return false; }
        }

        // a snapping peg may only have one non-snapped connection
        SnappingPeg TheSnappyPeg = SelectedPeg.GetComponent<SnappingPeg>();
        if (TheSnappyPeg == null) { TheSnappyPeg = PegBeingLookedAt.GetComponent<SnappingPeg>(); }
        if(TheSnappyPeg != null)
        {
            int MaxConnections = TheSnappyPeg.SnappedConnection == null ? 1 : 2;
            if (TheSnappyPeg.IIConnections.Count + TheSnappyPeg.IOConnections.Count >= MaxConnections) { return false; }
        }

        return true;
    }

    private static void SetOutlineColorOfObjectsInvolvedWithPlacing(OutlineColor color)
    {
        StuffPlacer.SetObjectsOutlineColor(ObjectsInvolvedWithPlacing, color);
    }
}

// connection modes are just different ways of triggering ConnectionInitial and ConnectionFinal with buttons
public enum ConnectionMode
{
    MultiPhase,
    HoldDown,
    Chained
}