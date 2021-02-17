// generates and provides functionality for the menu that selects placeable objects

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class BuildMenu : MonoBehaviour {

    // the list of objects in the menu
    [ReorderableList] public List<GameObject> PlaceableObjects;

    // the following lists correspond to the previous one via index and allow easy editing of the appearance of the menu
    [ReorderableList] public List<float> PlaceableObjectScaleFactors;
    [ReorderableList] public List<Vector2> PlaceableObjectOffsets;

    // the menu canvas
    public Canvas MenuCanvas;

    // the prefab for labels
    public GameObject LabelPrefab;

    // the selection image that moves when you scroll and its rect transform
    public RectTransform Selection;

    private void Awake() { StuffPlacer.buildmenu = this; }

    // Use this for initialization
    void Start () { GenerateMenu(); ToggleRotationLockText(); }

    // create the menu with all the prefabs in PlaceableObjects
    [Button]
    public void GenerateMenu()
    {
        for (int i = 0; i < PlaceableObjects.Count; i++) // we use this instead of foreach because a number is used for position
        {
            // create it and make it a child of the menu
            GameObject boob = Instantiate(PlaceableObjects[i], MenuCanvas.transform);

            // set its position
            RectTransform RectTransform = boob.AddComponent<RectTransform>();
            RectTransform.anchorMax = Vector2.zero;
            RectTransform.anchorMin = Vector2.zero;
            RectTransform.pivot = Vector2.zero;

            RectTransform.anchoredPosition = new Vector2(170 + i * 110, 70) + PlaceableObjectOffsets[i];

            // set its scale, based on the list
            RectTransform.localScale *= PlaceableObjectScaleFactors[i];

            // set its rotation
            RectTransform.rotation = Quaternion.Euler(0, 90, 0);

            // make it not cast shadows, that would be weird
            MeshRenderer renderer = boob.GetComponent<MeshRenderer>();
            if (renderer != null) // annoying NullReferenceException protecion
            {
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }

            // add it and all its children to the ui layer so it can be seen by the UI camera
            Transform[] allChildren = boob.GetComponentsInChildren<Transform>();
            foreach(Transform child in allChildren)
            {
                child.gameObject.layer = 5;
            }

            // give it a label
            GameObject label = Instantiate(LabelPrefab, MenuCanvas.transform);
            RectTransform LabelRectTransform = label.GetComponent<RectTransform>();
            LabelRectTransform.anchoredPosition = new Vector2(118 + i * 110, 40);

            TMPro.TMP_Text LabelText = label.GetComponent<TMPro.TMP_Text>();
            LabelText.text = PlaceableObjects[i].name;
        }

        SaveThisObject[] unlawfulSaveThisObjects = MenuCanvas.GetComponentsInChildren<SaveThisObject>();
        foreach(SaveThisObject penis in unlawfulSaveThisObjects) { Destroy(penis); }

        // get rid of components that are a problem in the selection menu
        MegaMeshComponent[] MegaMeshComponents = MenuCanvas.GetComponentsInChildren<MegaMeshComponent>();
        foreach(MegaMeshComponent mmc in MegaMeshComponents)
        {
            Destroy(mmc);
        }
        ThroughPeg[] ThroughPegs = MenuCanvas.GetComponentsInChildren<ThroughPeg>();
        foreach(ThroughPeg tp in ThroughPegs)
        {
            Destroy(tp);
        }
    }

    public int SelectedThing = 0; // the selected thing in the menu

    public float MenuOpenTime = MiscellaneousSettings.PlaceMenuOpenTime + 100; // for the temporary opening stuff. Value is so high so the menu doesn't open when the game starts for no reason
    public bool MenuLockedOpen; // for toggling with ctrl-c

	// does all the functionality for the build menu. Ran every frame that the board menu isn't being used by UIManager
    // this is all fairly bad code. TODO: rewrite it
	public void RunBuildMenu () {

        // constantly reset to 0 if menu is locked open
        if (MenuLockedOpen)
        {
            MenuOpenTime = 0;
        }

        // move selected thing
        if (Input.GetAxis("Mouse ScrollWheel") > 0 || BuildMenu.KeyboardScrollUp()) // scroll up
        {
            if (SelectedThing > 0) // so you can't scroll past None
            {
                SelectedThing--; // go to the previous thing
                UpdateSelectedThing();
            }

            MenuOpenTime = 0; // reset the counter
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0 || BuildMenu.KeyboardScrollDown()) // scroll down
        {
            if (SelectedThing < PlaceableObjects.Count) // so you can't scroll beyond the end
            {
                SelectedThing++; // go to the next thing
                UpdateSelectedThing();
            }

            MenuOpenTime = 0; // reset the counter
        }

        // toggle menu open with ctrl/alt-c
        if (Input.GetButton("Mod"))
        {
            if (Input.GetButtonDown("BuildMenu"))
            {
                if (MenuLockedOpen)
                {
                    MenuOpenTime = MiscellaneousSettings.PlaceMenuOpenTime + 100;
                }
                MenuLockedOpen = !MenuLockedOpen;
            }
        }
        // if mod is not held down, open it when c is held
        else
        {
            if (Input.GetButtonDown("BuildMenu"))
            {
                MenuLockedOpen = true;
            }
            if (Input.GetButtonUp("BuildMenu"))
            {
                MenuOpenTime = MiscellaneousSettings.PlaceMenuOpenTime + 100;
                MenuLockedOpen = false;
            }
        }

        // checks for number key selection
        // I hope this method isn't super laggy but something tells me that it is. In the short term, however, fuck it, because I hate copy-paste coding.
        for(int i=0; i<10; i++)
        {
            if(Input.GetButtonDown("Selection" + (i + 1).ToString()))
            {
                if (Input.GetButton("Mod")) { i += 10; } // so we effectively have up to 20 hotkeys

                if (i <= PlaceableObjects.Count)
                {
                    SelectedThing = i;
                    UpdateSelectedThing();

                    MenuOpenTime = 0;
                }
            }
        }

        // count the menu open time but only when it's relevant/matters
        if (MenuOpenTime <= MiscellaneousSettings.PlaceMenuOpenTime)
        {
            MenuOpenTime += Time.deltaTime;
            MenuCanvas.gameObject.SetActive(true); // we use the gameobject rather than the canvas component because, being a screen space UI, the physical objects must be disabled to hide them
        }
        else
        {
            MenuCanvas.gameObject.SetActive(false);
        }
    }

    void UpdateSelectedThing()
    {
        if(SelectedThing == 0) // none
        {
            StuffPlacer.PrefabToPlace = null;
        }

        else // place something
        {
            StuffPlacer.PrefabToPlace = PlaceableObjects[SelectedThing - 1];
        }

        // set the position of the selection
        Selection.anchoredPosition = new Vector2(17 + 110 * SelectedThing, 65);
    }

    // for toggling the rotationlock text
    public TMPro.TextMeshProUGUI RotationLockText;
    string locked = "Rotation: locked", unlocked = "Rotation: unlocked";

    public void ToggleRotationLockText()
    {
        if (StuffPlacer.RotationLocked) { RotationLockText.text = locked; }
        else { RotationLockText.text = unlocked; }
    }

    public static bool KeyboardScrollUp()
    {
        if (Input.GetButtonDown("Scroll"))
        {
            if(Input.GetAxis("Scroll") > 0) { return true; }
        }

        return false;
    }

    public static bool KeyboardScrollDown()
    {
        if (Input.GetButtonDown("Scroll"))
        {
            if (Input.GetAxis("Scroll") < 0) { return true; }
        }

        return false;
    }

}