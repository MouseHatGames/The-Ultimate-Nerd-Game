// everything being done in the default UI state

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public static class FirstPersonInteraction
{
	// called once per frame
	public static void DoInteraction ()
    {
        if (Input.GetButtonDown("BoardMenu"))
        {
            BoardMenu.Instance.InitializeBoardMenu();
            Done();
            return;
        }

        if (Input.GetButtonDown("PickComponent"))
        {
            SelectionMenu.Instance.PickComponent();
        }

        if (Input.GetButtonDown("Interact"))
        {
            RaycastHit hit;
            if (Physics.Raycast(Ray(), out hit, Settings.ReachDistance, IgnorePlayerLayermask))
            {
                if (hit.collider.tag == "Interactable") // if the cast hits an interactable such as a button or lever, interact with it
                {
                    hit.collider.GetComponent<Interactable>().Interact();
                    return; // so we can't place stuff too, as they are bound to the same key
                }
            }
        }

        if (Input.GetButtonDown("Zoom"))
        {
            FirstPersonCamera.fieldOfView = 10;
            FirstPersonController.Instance.m_MouseLook.XSensitivity /= 3;
            FirstPersonController.Instance.m_MouseLook.YSensitivity /= 3;
        }
        if (Input.GetButtonUp("Zoom"))
        {
            SettingsApplier.Instance.LoadFOV();
            SettingsApplier.Instance.LoadXSensitivity();
            SettingsApplier.Instance.LoadYSensitivity();
        }

        if (Input.GetButtonDown("Cancel"))
        {
            PauseMenu.Instance.PauseGame();
        }

        if (Input.GetButtonDown("UndoBoardDelete"))
        {
            BoardFunctions.RestoreMostRecentlyDeletedBoard();
            return;
        }

        ComponentPlacer.RunComponentPlacing();
        WirePlacer.RunWirePlacing();
        StuffDeleter.RunGameplayDeleting();
        StuffRotater.RunGameplayRotation();
        SelectionMenu.Instance.RunBuildMenu();
        LookThroughBoard.Run();
    }

    public static void Done()
    {
        StuffPlacer.DeleteThingBeingPlaced();
        WirePlacer.DoneConnecting();
        SelectionMenu.Instance.FuckOff();
    }

    // because Camera.main is actually unbelieveably slow
    private static Camera CachedFPSCam;
    public static Camera FirstPersonCamera
    {
        get
        {
            if (CachedFPSCam == null) { CachedFPSCam = FirstPersonController.Instance.m_Camera; }
            return CachedFPSCam;
        }
    }

    // Handy way to save code in lots of places that need it
    public static Ray Ray()
    {
        return new Ray(FirstPersonCamera.transform.position, FirstPersonCamera.transform.forward);
    }

    // only hit world, wire, and default. God this is awful
    public static readonly LayerMask IgnorePlayerLayermask = (1 << 0) | (1 << 8) | (1 << 9);
}