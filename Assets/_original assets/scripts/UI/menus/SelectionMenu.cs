// generates and provides functionality for the menu that selects placeable objects

using System.Collections.Generic;
using UnityEngine;
using References;
using NaughtyAttributes;

public class SelectionMenu : HorizontalScrollMenuWithSelection
{
    public static SelectionMenu Instance;
    private void Awake() { Instance = this; }

    // the list of objects in the menu
    [ReorderableList] public List<ComponentType> PlaceableObjectTypes;

    // the following list corresponds to the previous one via index and allow easy editing of the appearance of the menu
    [ReorderableList] public List<Vector2> PlaceableObjectOffsets;

    // the degree by which to scale everything in the selection menu
    public float PlaceableObjectScaleFactor = 150;

    // the prefab for labels
    public GameObject LabelPrefab;

    // Use this for initialization
    void Start ()
    {
        GenerateMenu();
        SetRotationLockText();
        SelectionMenuOpenTime = Settings.Get("SelectionMenuOpenTime", 0.8f);
    }

    // create the menu with all the prefabs in PlaceableObjects
    [Button]
    public void GenerateMenu()
    {
        MaxSelectedThing = PlaceableObjectTypes.Count;

        for (int i = 0; i < PlaceableObjectTypes.Count; i++)
        {
            // create it and make it a child of the menu
            GameObject reference = Prefabs.ComponentTypeToPrefab(PlaceableObjectTypes[i]);
            GameObject boob = Instantiate(reference, Canvas.transform);

            // set its position
            RectTransform RectTransform = boob.AddComponent<RectTransform>();
            RectTransform.anchorMax = Vector2.zero;
            RectTransform.anchorMin = Vector2.zero;
            RectTransform.pivot = Vector2.zero;

            RectTransform.anchoredPosition = new Vector2(StartingPosition.x + DistanceBetweenPositions + i * DistanceBetweenPositions, 70) + PlaceableObjectOffsets[i];

            // set its scale
            RectTransform.localScale *= PlaceableObjectScaleFactor;

            // set its rotation
            RectTransform.rotation = Quaternion.Euler(0, 90, 0);

            // give it a label
            GameObject label = Instantiate(LabelPrefab, Canvas.transform);
            label.GetComponent<RectTransform>().anchoredPosition = new Vector2(StartingPosition.x + DistanceBetweenPositions + i * DistanceBetweenPositions, 40);
            label.GetComponent<TMPro.TMP_Text>().text = reference.name;
        }

        // put the menu and everything in it on the UI layer so they can be seen by the UI camera
        Transform[] allChildren = Canvas.GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            child.gameObject.layer = 5;
        }

        // get rid of components that are a problem in the selection menu
        MegaMeshComponent[] MegaMeshComponents = Canvas.GetComponentsInChildren<MegaMeshComponent>();
        foreach(MegaMeshComponent mmc in MegaMeshComponents)
        {
            Destroy(mmc);
        }
        //ThroughPeg[] ThroughPegs = Canvas.GetComponentsInChildren<ThroughPeg>();
        //foreach(ThroughPeg tp in ThroughPegs)
        //{
        //    DestroyImmediate(tp);
        //}
    }

    static float SelectionMenuOpenTime;
    float MenuClosesIn = 0; // for the temporary opening stuff
    bool MenuLockedOpen;

    // does all the functionality for the build menu. Ran every frame by GameplayUIManager that the board menu isn't being used
    // this is all fairly bad code. TODO: rewrite it
    public void RunBuildMenu () {

        ScrollThroughMenu();
        if (SelectedThingJustChanged)
        {
            if (!Input.GetButton("BuildMenu"))
            {
                MenuClosesIn = SelectionMenuOpenTime;
            }
        }

        // a somewhat messy chain of logic but the following block results in the proper behavior of MenuLockedOpen
        if (Input.GetButtonDown("BuildMenu"))
        {
            if (Input.GetButton("Mod")) // toggle menu locked open with ctrl/alt-c
            {
                MenuLockedOpen = !MenuLockedOpen;
                MenuClosesIn = 0;
            }
            else
            {
                MenuLockedOpen = false;
                MenuClosesIn = 10000;
            }
        }
        else if (Input.GetButtonUp("BuildMenu"))
        {
            MenuClosesIn = 0;
        }

        // handles locking the menu
        if (MenuLockedOpen)
        {
            Canvas.gameObject.SetActive(true); // we use the gameobject rather than the canvas component because, being a screen space UI, the physical objects must be disabled to hide them
            return;
        }

        // handles temporary closing of the menu
        if(MenuClosesIn > 0)
        {
            Canvas.gameObject.SetActive(true);
            MenuClosesIn -= Time.deltaTime;
        }
        else
        {
            Canvas.gameObject.SetActive(false);
        }
    }

    // trigger when switching to the board menu
    public void FuckOff()
    {
        Canvas.gameObject.SetActive(false);
        MenuClosesIn = 0;
        MenuLockedOpen = false;
    }

    public static GameObject SelectedComponent
    {
        get
        {
            return Prefabs.ComponentTypeToPrefab(SelectedComponentType);
        }
    }

    public static ComponentType SelectedComponentType
    {
        get
        {
            if(Instance.SelectedThing == 0) { return ComponentType.none; }
            return Instance.PlaceableObjectTypes[Instance.SelectedThing - 1];
        }
    }

    // for toggling the rotationlock text
    public TMPro.TextMeshProUGUI RotationLockText;
    string locked = "Rotation: locked", unlocked = "Rotation: unlocked";

    public void SetRotationLockText()
    {
        if (StuffPlacer.RotationLocked) { RotationLockText.text = locked; }
        else { RotationLockText.text = unlocked; }
    }

    // TODO move to a better class
    public void PickComponent()
    {
        RaycastHit hit;
        if (Physics.Raycast(FirstPersonInteraction.Ray(), out hit, Settings.ReachDistance, 1 << 0))
        {
            GameObject ThingWeHit = ComponentPlacer.FullComponent(hit.collider);

            ObjectInfo buttcrack = ThingWeHit.GetComponent<ObjectInfo>();
            if (buttcrack != null)
            {
                for (int i= 0; i < PlaceableObjectTypes.Count; i++)
                {
                    if (PlaceableObjectTypes[i] == buttcrack.ComponentType)
                    {
                        SelectedThing = i + 1;
                        UpdateSelectedThing();
                        MenuClosesIn = SelectionMenuOpenTime;
                        return;
                    }
                }
            }
        }
    }
}