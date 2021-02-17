// handles raycasts from the camera and stuff. Triggers methods in StuffPlacer, StuffConnector and StuffDeleter.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonInteraction : MonoBehaviour {

    // the camera used to cast rays
    public static Camera FirstPersonCamera;
    public Camera PublicFirstPersonCamera; // because unity is bullshit and won't let me directly assign static variables

    public static ConnectionMode ConnectionMode;
    public static bool MultiPhaseConnectionInProgress;

    // Use this for initialization
    void Start () {
        FirstPersonCamera = PublicFirstPersonCamera;
	}
	
	// Update is called once per frame
	void Update () {

        if (UIManager.SomeOtherMenuIsOpen) { return; } // don't do any of this stuff while new board menu/text edit menu is open

        // hopefully declaring these every frame instead of just when a button is pushed doesn't impact performance
        RaycastHit hit;
        Transform cam = FirstPersonCamera.transform;

        // placing
        if (Input.GetButtonDown("Place") && BoardPlacer.BoardBeingPlaced == null) // the second check is so that it can't mess up board placing by placing a boardobject
        {
            if(Physics.Raycast(cam.position, cam.forward, out hit, MiscellaneousSettings.ReachDistance))
            {
                if(hit.collider.tag == "Interactable") // if the cast hits an interactable such as a button or lever, interact with it
                {
                    hit.collider.GetComponent<Interactable>().Interacted = true;
                }

                else // if none of the special conditions are found, actually place the thing lol
                {
                    StuffPlacer.PlaceOnSomething(hit);
                }
            }

        }

        // rotation
        if (Input.GetButtonDown("Rotate"))
        {
            if (Physics.Raycast(cam.position, cam.forward, out hit, MiscellaneousSettings.ReachDistance)) // send a raycast out from the camera in the direction it's facing. The player is set to IgnoreRaycast so the cast goes through that.
            {
                StuffRotater.RotateThing(hit.collider.gameObject);
            }
        }

        // rotation lock
        if (Input.GetButtonDown("RotationLock"))
        {
            StuffPlacer.ToggleRotationLock();
        }

        // deleting
        if (Input.GetButtonDown("Delete"))
        {
            if (Physics.Raycast(cam.position, cam.forward, out hit, MiscellaneousSettings.ReachDistance))
            {
                StuffDeleter.DeleteThing(hit.collider.gameObject);
            }
        }

        // connection creation
        if(ConnectionMode == ConnectionMode.HoldDown)
        {
            if (Input.GetButtonDown("Connect"))
            {
                if (Physics.Raycast(cam.position, cam.forward, out hit, MiscellaneousSettings.ReachDistance))
                {
                    StuffConnecter.ConnectionInitial(hit);
                }
            }

            if (Input.GetButtonUp("Connect"))
            {
                Physics.Raycast(cam.position, cam.forward, out hit, MiscellaneousSettings.ReachDistance);

                if (hit.transform != null) // if it hits stuff, connect stuff
                {
                    StuffConnecter.ConnectionFinal(hit);
                }
                else // otherwise, just deselect the selected thing
                {
                    StuffConnecter.ConnectionFinal();
                }
            }
        }
        else if(ConnectionMode == ConnectionMode.MultiPhase)
        {
            if (Input.GetButtonDown("Connect"))
            {
                if (Physics.Raycast(cam.position, cam.forward, out hit, MiscellaneousSettings.ReachDistance))
                {
                    if (!MultiPhaseConnectionInProgress) { StuffConnecter.ConnectionInitial(hit); MultiPhaseConnectionInProgress = true; }
                    else { StuffConnecter.ConnectionFinal(hit); MultiPhaseConnectionInProgress = false; }
                }
            }
        }


        // look through board
        if (Input.GetButtonDown("LookThroughBoard"))
        {
            if (Physics.Raycast(cam.position, cam.forward, out hit, MiscellaneousSettings.ReachDistance))
            {
                LookThroughBoard.Initial(hit);
            }
        }

        if (Input.GetButtonUp("LookThroughBoard"))
        {
            LookThroughBoard.Final();
        }
    }
}


public enum ConnectionMode
{
    MultiPhase,
    HoldDown
}