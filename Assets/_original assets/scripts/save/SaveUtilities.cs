using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using SavedObjects;
using References;

public static class SavedObjectUtilities
{
    public static SavedObjectV2 CreateSavedObjectFrom(GameObject worldobject)
    {
        ObjectInfo worldobjectinfo = worldobject.GetComponent<ObjectInfo>();
        if (worldobjectinfo == null) { return null; }

        return CreateSavedObjectFrom(worldobjectinfo);
    }

    public static SavedObjectV2 CreateSavedObjectFrom(ObjectInfo worldsave)
    {
        //SavedObjectV2 newsavedobject = SaveManager.ObjectTypeToSavedObjectType(save.ObjectType);

        SavedObjectV2 newsavedobject = null;

        switch (worldsave.ComponentType)
        {
            case ComponentType.CircuitBoard:
                CircuitBoard board = worldsave.GetComponent<CircuitBoard>();
                newsavedobject = new SavedCircuitBoard
                {
                    x = board.x,
                    z = board.z,
                    color = board.GetBoardColor
                };
                break;

            case ComponentType.Wire:
                InputInputConnection IIConnection = worldsave.GetComponent<InputInputConnection>();
                newsavedobject = new SavedWire
                {
                    InputInput = IIConnection,
                    length = worldsave.transform.localScale.z
                };
                break;

            case ComponentType.Button:
                newsavedobject = new SavedButton();
                break;

            case ComponentType.PanelButton:
                newsavedobject = new SavedPanelButton();
                break;

            case ComponentType.Delayer:
                Delayer delayer = worldsave.GetComponent<Delayer>();
                newsavedobject = new SavedDelayer
                {
                    OutputOn = delayer.Output.On,
                    DelayCount = delayer.DelayCount
                };
                break;

            case ComponentType.Display:
                Display display = worldsave.GetComponentInChildren<Display>();
                newsavedobject = new SavedDisplay
                {
                    Color = display.DisplayColor
                };
                break;

            case ComponentType.PanelDisplay:
                Display paneldisplay = worldsave.GetComponentInChildren<Display>();
                newsavedobject = new SavedPanelDisplay
                {
                    Color = paneldisplay.DisplayColor
                };
                break;

            case ComponentType.Inverter:
                Inverter notgate = worldsave.GetComponent<Inverter>();
                newsavedobject = new SavedInverter
                {
                    OutputOn = notgate.Output.On
                };
                break;

            case ComponentType.Label:
                Label label = worldsave.GetComponent<Label>();
                newsavedobject = new SavedLabel
                {
                    text = label.text.text,
                    FontSize = label.text.fontSize
                };
                break;

            case ComponentType.PanelLabel:
                Label panellabel = worldsave.GetComponent<Label>();
                newsavedobject = new SavedPanelLabel
                {
                    text = panellabel.text.text,
                    FontSize = panellabel.text.fontSize
                };
                break;

            case ComponentType.Switch: // SWITCH-FUCKING-CEPTION
                Switch circuitswitch = worldsave.GetComponentInChildren<Switch>();
                newsavedobject = new SavedSwitch
                {
                    on = circuitswitch.On
                };
                break;

            case ComponentType.PanelSwitch:
                Switch panelswitch = worldsave.GetComponentInChildren<Switch>();
                newsavedobject = new SavedPanelSwitch
                {
                    on = panelswitch.On
                };
                break;

            case ComponentType.Peg:
                newsavedobject = new SavedPeg();
                break;

            case ComponentType.ThroughPeg:
                newsavedobject = new SavedThroughPeg();
                break;

            case ComponentType.Blotter:
                Blotter blotter = worldsave.GetComponent<Blotter>();
                newsavedobject = new SavedBlotter
                {
                    OutputOn = blotter.Output.On
                };
                break;

            case ComponentType.ThroughBlotter:
                Blotter throughblotter = worldsave.GetComponent<Blotter>();
                newsavedobject = new SavedThroughBlotter
                {
                    OutputOn = throughblotter.Output.On
                };
                break;

            case ComponentType.ColorDisplay:
                newsavedobject = new SavedColorDisplay();
                break;

            case ComponentType.PanelColorDisplay:
                newsavedobject = new SavedPanelColorDisplay();
                break;

            case ComponentType.Noisemaker:
                Noisemaker noisemaker = worldsave.GetComponentInChildren<Noisemaker>();
                newsavedobject = new SavedNoisemaker
                {
                    ToneFrequency = noisemaker.ToneFrequency
                };
                break;

            case ComponentType.SnappingPeg:
                newsavedobject = new SavedSnappingPeg();
                break;

            case ComponentType.Mount:
                newsavedobject = new SavedMount();
                break;

            case ComponentType.none:
                Debug.LogError("BIG ERROR tried to save a component with no type!");
                break;
        }

        newsavedobject.LocalPosition = worldsave.transform.localPosition;
        newsavedobject.LocalEulerAngles = worldsave.transform.localEulerAngles;

        if (newsavedobject.CanHaveChildren)
        {
            newsavedobject.Children = FindChildSaves(worldsave);
        }

        return newsavedobject;
    }

    // get all the SavedObjects in the immediate children of the SaveObject
    private static SavedObjectV2[] FindChildSaves(ObjectInfo save)
    {
        List<SavedObjectV2> childsaves = new List<SavedObjectV2>();

        for (int i = 0; i < save.transform.childCount; i++)
        {
            ObjectInfo child = save.transform.GetChild(i).GetComponent<ObjectInfo>();
            if (child != null) { childsaves.Add(CreateSavedObjectFrom(child)); } // and here's where the magic happens: a spiralling chain of children getting children!
        }

        return childsaves.ToArray();
    }

    public static GameObject LoadSavedObject(SavedObjectV2 save, Transform parent = null)
    {
        ComponentType componenttype = SavedObjectTypeToObjectType[save.GetType()];

        GameObject LoadedObject = UnityEngine.Object.Instantiate(Prefabs.ComponentTypeToPrefab(componenttype), parent);
        LoadedObject.transform.localPosition = save.LocalPosition;
        LoadedObject.transform.localEulerAngles = save.LocalEulerAngles;

        LoadedObject.AddComponent<ObjectInfo>().ComponentType = componenttype;

        switch (componenttype)
        {
            // there is no case for objects with no special data to load

            case ComponentType.CircuitBoard:
                LoadCircuitBoard(LoadedObject, (SavedCircuitBoard)save);
                break;

            case ComponentType.Wire:
                LoadWire(LoadedObject, (SavedWire)save);
                break;

            case ComponentType.Delayer:
                LoadDelayer(LoadedObject, (SavedDelayer)save);
                break;

            case ComponentType.PanelDisplay:
            case ComponentType.Display:
                LoadDisplay(LoadedObject, (SavedDisplay)save);
                break;

            case ComponentType.Inverter:
                LoadInverter(LoadedObject, (SavedInverter)save);
                break;

            case ComponentType.PanelLabel:
            case ComponentType.Label:
                LoadLabel(LoadedObject, (SavedLabel)save);
                break;

            case ComponentType.PanelSwitch:
            case ComponentType.Switch: // SWITCH-FUCKING-CEPTION
                LoadSwitch(LoadedObject, (SavedSwitch)save);
                break;

            case ComponentType.ThroughBlotter:
            case ComponentType.Blotter:
                LoadBlotter(LoadedObject, (SavedBlotter)save);
                break;

            case ComponentType.Noisemaker:
                LoadNoisemaker(LoadedObject, (SavedNoisemaker)save);
                break;

            case ComponentType.Mount:
                LoadMount(LoadedObject);
                break;
        }

        if (save.CanHaveChildren)
        {
            foreach(SavedObjectV2 child in save.Children)
            {
                LoadSavedObject(child, LoadedObject.transform);
            }
        }

        return LoadedObject;
    }

    private static void LoadCircuitBoard(GameObject LoadedObject, SavedCircuitBoard save)
    {
        CircuitBoard board = LoadedObject.GetComponent<CircuitBoard>();

        board.x = save.x;
        board.z = save.z;
        board.SetBoardColor(save.color);

        board.CreateCuboid();
    }

    private static void LoadWire(GameObject LoadedObject, SavedWire save)
    {
        if (save.InputInput)
        {
            LoadedObject.AddComponent<InputInputConnection>();
        }
        else
        {
            LoadedObject.AddComponent<InputOutputConnection>();
        }

        LoadedObject.transform.localScale = new Vector3(0.05f, 0.02f, save.length); // todo: merge this in some way with Wire.DrawWire
    }

    private static void LoadDelayer(GameObject LoadedObject, SavedDelayer save)
    {
        Delayer delayer = LoadedObject.GetComponent<Delayer>();

        delayer.Output.On = save.OutputOn;
        delayer.DelayCount = save.DelayCount;
    }

    private static void LoadDisplay(GameObject LoadedObject, SavedDisplay save)
    {
        Display display = LoadedObject.GetComponentInChildren<Display>();

        display.DisplayColor = save.Color;
    }

    private static void LoadInverter(GameObject LoadedObject, SavedInverter save)
    {
        Inverter inverter = LoadedObject.GetComponent<Inverter>();

        inverter.Output.On = save.OutputOn;
    }

    private static void LoadLabel(GameObject LoadedObject, SavedLabel save)
    {
        Label label = LoadedObject.GetComponent<Label>();

        label.text.text = save.text;
        label.text.fontSize = save.FontSize;
    }

    private static void LoadSwitch(GameObject LoadedObject, SavedSwitch save)
    {
        Switch circuitswitch = LoadedObject.GetComponentInChildren<Switch>();

        circuitswitch.On = save.on;
        circuitswitch.UpdateLever(false); // apply the effects of the bool we just loaded, don't play sound
    }

    private static void LoadBlotter(GameObject LoadedObject, SavedBlotter save)
    {
        Blotter blotter = LoadedObject.GetComponent<Blotter>();

        blotter.Output.On = save.OutputOn;
    }

    private static void LoadNoisemaker(GameObject LoadedObject, SavedNoisemaker save)
    {
        Noisemaker noisemaker = LoadedObject.GetComponentInChildren<Noisemaker>();

        noisemaker.ToneFrequency = save.ToneFrequency;
    }

    private static void LoadMount(GameObject LoadedObject)
    {
        Mount mount = LoadedObject.GetComponent<Mount>();
        mount.LoadedMount = true;
    }

    public static Dictionary<Type, ComponentType> SavedObjectTypeToObjectType = new Dictionary<Type, ComponentType>
    {
        { typeof(SavedCircuitBoard), ComponentType.CircuitBoard },
        { typeof(SavedWire), ComponentType.Wire },
        { typeof(SavedInverter), ComponentType.Inverter },
        { typeof(SavedPeg), ComponentType.Peg },
        { typeof(SavedDelayer), ComponentType.Delayer },
        { typeof(SavedThroughPeg), ComponentType.ThroughPeg },
        { typeof(SavedSwitch), ComponentType.Switch },
        { typeof(SavedButton), ComponentType.Button },
        { typeof(SavedDisplay), ComponentType.Display },
        { typeof(SavedLabel), ComponentType.Label },
        { typeof(SavedPanelSwitch), ComponentType.PanelSwitch },
        { typeof(SavedPanelButton), ComponentType.PanelButton },
        { typeof(SavedPanelDisplay), ComponentType.PanelDisplay },
        { typeof(SavedPanelLabel), ComponentType.PanelLabel },
        { typeof(SavedBlotter), ComponentType.Blotter },
        { typeof(SavedThroughBlotter), ComponentType.ThroughBlotter },
        { typeof(SavedColorDisplay), ComponentType.ColorDisplay },
        { typeof(SavedPanelColorDisplay), ComponentType.PanelColorDisplay },
        { typeof(SavedNoisemaker), ComponentType.Noisemaker },
        { typeof(SavedSnappingPeg), ComponentType.SnappingPeg },
        { typeof(SavedMount), ComponentType.Mount },
    };
}


public enum ComponentType
{
    none,
    CircuitBoard,
    Wire,
    Inverter,
    Peg,
    Delayer,
    ThroughPeg,
    Switch,
    Button,
    Display,
    Label,
    PanelSwitch,
    PanelButton,
    PanelDisplay,
    PanelLabel,
    Blotter,
    ThroughBlotter,
    ColorDisplay,
    PanelColorDisplay,
    Noisemaker,
    SnappingPeg,
    Mount,
}