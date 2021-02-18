using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public static class LegacySaveLoader
{
    public static void LoadLegacySave()
    {
        MegaMeshManager.ClearReferences();
        BehaviorManager.AllowedToUpdate = false; // don't let circuitry updates fuck us up while we're loading the game
        BehaviorManager.ClearAllLists();

        List<SavedObject> TopLevelObjects = ES3.Load<List<SavedObject>>("TopLevelObjects", "saves/" + SaveManager.SaveName + ".tung");
        foreach (SavedObject save in TopLevelObjects)
        {
            LoadLegacySaveObject(save, null);
        }

        LegacyLoadPlayer(SaveManager.SaveName);

        SaveManager.RecalculateAllClustersEverywhereWithDelay();
        MegaMeshManager.AddComponentsEverywhere();
        GameplayUIManager.UIState = UIState.None;
    }

    private static void LoadLegacySaveObject(SavedObject save, Transform parent)
    {
        GameObject LoadedObject = Object.Instantiate(LegacySaveObjectsList.LegacyObjectTypeToPrefab(save.ObjectType), parent);

        ObjectInfo newsave = LoadedObject.AddComponent<ObjectInfo>();
        newsave.ComponentType = LegacySaveObjectsList.LegacyObjectTypeToComponentType(save.ObjectType);

        LoadedObject.transform.localPosition = save.LocalPosition;
        LoadedObject.transform.localEulerAngles = save.LocalEulerAngles;

        LegacyCustomData.LoadData(LoadedObject, save);
        foreach(SavedObject child in save.Children)
        {
            LoadLegacySaveObject(child, LoadedObject.transform);
        }
    }

    private static void LegacyLoadPlayer(string legacysavename)
    {
        if (!ES3.KeyExists("PlayerPosition", "saves/" + legacysavename + ".tung") || !ES3.KeyExists("PlayerRotation", "saves/" + legacysavename + ".tung")) { return; }

        Vector3 PlayerPosition = ES3.Load<Vector3>("PlayerPosition", "saves/" + legacysavename + ".tung");
        Vector2 PlayerRotation = ES3.Load<Vector2>("PlayerRotation", "saves/" + legacysavename + ".tung");

        if (PlayerPosition.y < -10)
        {
            return;
        }
        FirstPersonController.Instance.transform.position = PlayerPosition;

        FirstPersonController.Instance.m_MouseLook.m_CharacterTargetRot = Quaternion.Euler(0, PlayerRotation.y, 0);
        FirstPersonController.Instance.m_MouseLook.m_CameraTargetRot = Quaternion.Euler(PlayerRotation.x, 0, 0);
    }
}

// the 0.1 SavedObject class
/// <summary>
/// DO NOT USE THIS, THIS IS THE OLD VERSION!
/// </summary>
public class SavedObject
{
    public string ObjectType;

    public SavedObject[] Children;

    public object[] CustomDataArray;

    public Vector3 LocalPosition;
    public Vector3 LocalEulerAngles;
}



public class LegacyCustomData
{
    public static void LoadData(GameObject LoadedObject, SavedObject save)
    {
        if (save.ObjectType == "CircuitBoard") { LoadCircuitBoard(LoadedObject, save); }
        else if (save.ObjectType == "Wire") { LoadWire(LoadedObject, save); }
        else if (save.ObjectType == "Inverter") { LoadInverter(LoadedObject, save); }
        else if (save.ObjectType == "Peg") { LoadPeg(LoadedObject, save); }
        else if (save.ObjectType == "Delayer") { LoadDelayer(LoadedObject, save); }
        else if (save.ObjectType == "Through Peg") { LoadThroughPeg(LoadedObject, save); }
        else if (save.ObjectType == "Switch" || save.ObjectType == "Panel Switch") { LoadSwitch(LoadedObject, save); }
        else if (save.ObjectType == "Button" || save.ObjectType == "Panel Button") { LoadButton(LoadedObject, save); }
        else if (save.ObjectType == "Display" || save.ObjectType == "Panel Display") { LoadDisplay(LoadedObject, save); }
        else if (save.ObjectType == "Label" || save.ObjectType == "Panel Label") { LoadLabel(LoadedObject, save); }
        else if (save.ObjectType == "Blotter" || save.ObjectType == "Through Blotter") { LoadBlotter(LoadedObject, save); }

        else
        {
            Debug.LogError("ObjectType not found. ObjectType was " + save.ObjectType);
        }
    }

    private static void LoadCircuitBoard(GameObject LoadedObject, SavedObject save)
    {
        object[] data = save.CustomDataArray;
        CircuitBoard board = LoadedObject.GetComponent<CircuitBoard>();

        board.x = (int)data[0];
        board.z = (int)data[1];
        board.SetBoardColor((Color)data[2]);

        board.CreateCuboid();
    }

    private static void LoadWire(GameObject LoadedObject, SavedObject save)
    {
        if ((bool)save.CustomDataArray[0] == true)
        {
            LoadedObject.AddComponent<InputInputConnection>();
        }
        else
        {
            LoadedObject.AddComponent<InputOutputConnection>();
        }

        Vector3 oldscale = (Vector3)save.CustomDataArray[1];

        LoadedObject.transform.localScale = new Vector3(0.05f, 0.02f, oldscale.z);
    }

    private static void LoadButton(GameObject LoadedObject, SavedObject save)
    {
        return;
    }

    private static void LoadDelayer(GameObject LoadedObject, SavedObject save)
    {
        object[] data = save.CustomDataArray;
        Delayer delayer = LoadedObject.GetComponent<Delayer>();

        delayer.Input.On = (bool)data[0];
        delayer.Output.On = (bool)data[1];
        delayer.DelayCount = (int)data[2];
    }

    private static void LoadDisplay(GameObject LoadedObject, SavedObject save)
    {
        object[] data = save.CustomDataArray;
        Display display = LoadedObject.GetComponentInChildren<Display>();

        display.DisplayColor = DisplayColor.Yellow;
        display.Input.On = (bool)data[0];
    }

    private static void LoadInverter(GameObject LoadedObject, SavedObject save)
    {
        object[] data = save.CustomDataArray;
        Inverter notgate = LoadedObject.GetComponent<Inverter>();

        notgate.Input.On = (bool)data[0];
        notgate.Output.On = (bool)data[1];
    }

    private static void LoadLabel(GameObject LoadedObject, SavedObject save)
    {
        object[] data = save.CustomDataArray;
        Label label = LoadedObject.GetComponent<Label>();

        label.text.text = (string)data[0];
        label.text.fontSize = (float)data[1];
    }

    private static void LoadSwitch(GameObject LoadedObject, SavedObject save)
    {
        object[] data = save.CustomDataArray;
        Switch circuitswitch = LoadedObject.GetComponentInChildren<Switch>();

        circuitswitch.On = (bool)data[0];
        circuitswitch.UpdateLever(false); // apply the effects of the bool we just loaded, don't play sound
    }

    private static void LoadPeg(GameObject LoadedObject, SavedObject save)
    {
        object[] data = save.CustomDataArray;
        CircuitInput input = LoadedObject.GetComponent<CircuitInput>();

        input.On = (bool)data[0];
    }

    private static void LoadThroughPeg(GameObject LoadedObject, SavedObject save)
    {
        object[] data = save.CustomDataArray;
        CircuitInput[] inputs = LoadedObject.GetComponentsInChildren<CircuitInput>();

        inputs[0].On = (bool)data[0];
        inputs[1].On = (bool)data[0];
    }

    private static void LoadBlotter(GameObject LoadedObject, SavedObject save)
    {
        object[] data = save.CustomDataArray;
        Blotter blotter = LoadedObject.GetComponent<Blotter>();

        blotter.Input.On = (bool)data[0];
        blotter.Output.On = (bool)data[1];
    }
}

public class LegacySaveObjectsList
{
    public static GameObject LegacyObjectTypeToPrefab(string ObjectType)
    {
        if (ObjectType == "CircuitBoard") { return References.Prefabs.CircuitBoard; }
        if (ObjectType == "Wire") { return References.Prefabs.Wire; }
        if (ObjectType == "Inverter") { return References.Prefabs.Inverter; }
        if (ObjectType == "Peg") { return References.Prefabs.Peg; }
        if (ObjectType == "Delayer") { return References.Prefabs.Delayer; }
        if (ObjectType == "Through Peg") { return References.Prefabs.ThroughPeg; }
        if (ObjectType == "Switch") { return References.Prefabs.Switch; }
        if (ObjectType == "Button") { return References.Prefabs.Button; }
        if (ObjectType == "Display") { return References.Prefabs.Display; }
        if (ObjectType == "Label") { return References.Prefabs.Label; }
        if (ObjectType == "Panel Switch") { return References.Prefabs.PanelSwitch; }
        if (ObjectType == "Panel Button") { return References.Prefabs.PanelButton; }
        if (ObjectType == "Panel Display") { return References.Prefabs.PanelDisplay; }
        if (ObjectType == "Panel Label") { return References.Prefabs.PanelLabel; }
        if (ObjectType == "Blotter") { return References.Prefabs.Blotter; }
        if (ObjectType == "Through Blotter") { return References.Prefabs.ThroughBlotter; }

        else { Debug.LogError("Unkonwn legacy object type. Type was " + ObjectType.ToString()); return null; }
    }

    public static ComponentType LegacyObjectTypeToComponentType(string ObjectType)
    {
        if (ObjectType == "CircuitBoard") { return ComponentType.CircuitBoard; }
        if (ObjectType == "Wire") { return ComponentType.Wire; }
        if (ObjectType == "Inverter") { return ComponentType.Inverter; }
        if (ObjectType == "Peg") { return ComponentType.Peg; }
        if (ObjectType == "Delayer") { return ComponentType.Delayer; }
        if (ObjectType == "Through Peg") { return ComponentType.ThroughPeg; }
        if (ObjectType == "Switch") { return ComponentType.Switch; }
        if (ObjectType == "Button") { return ComponentType.Button; }
        if (ObjectType == "Display") { return ComponentType.Display; }
        if (ObjectType == "Label") { return ComponentType.Label; }
        if (ObjectType == "Panel Switch") { return ComponentType.PanelSwitch; }
        if (ObjectType == "Panel Button") { return ComponentType.PanelButton; }
        if (ObjectType == "Panel Display") { return ComponentType.PanelDisplay; }
        if (ObjectType == "Panel Label") { return ComponentType.PanelLabel; }
        if (ObjectType == "Blotter") { return ComponentType.Blotter; }
        if (ObjectType == "Through Blotter") { return ComponentType.ThroughBlotter; }

        else { Debug.LogError("Unkonwn legacy object type. Type was " + ObjectType.ToString() + ". Returning a wire because enums are non-nullable"); return ComponentType.Wire; }
    }
}