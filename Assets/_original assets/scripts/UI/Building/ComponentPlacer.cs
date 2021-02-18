// for placing components

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ComponentPlacer
{
    public static bool ShowPlacingGhost = Settings.Get("ShowPlacingGhost", true);

    public static void RunComponentPlacing()
    {
        if (Input.GetButtonDown("TogglePlacingGhost") && !Input.GetButton("Mod")) // holding mod is for toggling initial placing outline
        {
            ShowPlacingGhost = !ShowPlacingGhost;
            Settings.Save("ShowPlacingGhost", ShowPlacingGhost);

            if (!ShowPlacingGhost)
            {
                StuffPlacer.DeleteThingBeingPlaced();
                StuffPlacer.RotationAboutUpVector = 0;
            }
        }

        if (Input.GetButtonDown("Place") && StuffPlacer.OkayToPlace) // run BEFORE RunStuffPlacing so we can actually get ThingBeingPlaced
        {
            StuffPlacer.GetThingBeingPlaced.AddComponent<ObjectInfo>().ComponentType = SelectionMenu.SelectedComponentType;
        }

        if (ShowPlacingGhost)
        {
            MakeSureThingBeingPlacedIsCorrect();
            StuffPlacer.RunStuffPlacing(true, true); // the booleans are AllowFineRotation and HideWhenInvalidPlacement
        }
        else
        {
            if (Input.GetButtonDown("Place"))
            {
                PlaceAndMoveAllInOneGo();
            }
        }
    }

    private static void MakeSureThingBeingPlacedIsCorrect()
    {
        // replace ThingBeingPlaced when it should be replaced - when you switch selected things or when something was just placed
        if (SelectionMenu.Instance.SelectedThing != 0 &&
           (SelectionMenu.Instance.SelectedThingJustChanged || StuffPlacer.GetThingBeingPlaced == null))
        {
            StuffPlacer.NewThingBeingPlaced(Object.Instantiate(SelectionMenu.SelectedComponent));
            DoThingsForNewComponents(StuffPlacer.GetThingBeingPlaced);
        }

        else if (SelectionMenu.Instance.SelectedThing == 0) { StuffPlacer.DeleteThingBeingPlaced(); } // switching to nothing selected deletes the thing being placed
    }

    private static void PlaceAndMoveAllInOneGo()
    {
        MakeSureThingBeingPlacedIsCorrect();
        if (StuffPlacer.GetThingBeingPlaced != null) { StuffPlacer.GetThingBeingPlaced.AddComponent<ObjectInfo>().ComponentType = SelectionMenu.SelectedComponentType; }
        StuffPlacer.RunStuffPlacing();
        //StuffPlacer.PlaceThingBeingPlaced(); // already called by RunStuffPlacing

        StuffPlacer.DeleteThingBeingPlaced();
    }

    // returns the part of the component attached to nothing or the circuitboard if you hit a child of it
    // not strictly related to the main function of this class, but I can't think of a better place to put this method.
    public static GameObject FullComponent(Transform PartOfComponent)
    {
        while (PartOfComponent.parent != null && PartOfComponent.parent.tag != "CircuitBoard" && PartOfComponent.parent.tag != "PlaceOnlyCircuitBoard")
        {
            PartOfComponent = PartOfComponent.parent; // get the full object if we hit a child of it
        }

        return PartOfComponent.gameObject;
    }

    public static GameObject FullComponent(GameObject PartOfComponent)
    {
        return FullComponent(PartOfComponent.transform);
    }

    public static GameObject FullComponent(Collider PartOfComponent)
    {
        return FullComponent(PartOfComponent.transform);
    }

    // god this sucks. Should be an interface. TODO learn how to use interfaces and make this an interface
    // also, mounts should be a part of this
    private static void DoThingsForNewComponents(GameObject newcomponent)
    {
        Display display = newcomponent.GetComponentInChildren<Display>();
        if (display != null) { display.NewlyPlaced(); return; }

        Label label = newcomponent.GetComponentInChildren<Label>();
        if (label != null) { label.NewlyPlaced(); return; }

        Noisemaker noisemaker = newcomponent.GetComponentInChildren<Noisemaker>();
        if (noisemaker != null) { noisemaker.NewlyPlaced(); return; }
    }
}