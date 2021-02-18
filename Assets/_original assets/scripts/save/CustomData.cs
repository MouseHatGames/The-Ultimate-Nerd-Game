//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using References;
//using SavedObjects;

//public class CustomData {

//	public static void SetCustomData(SavedObjectV2 save, Transform WorldObject) // updates a SaveThisObject to have its current relevant CustomData[] values
//    {
//        if (save.GetType() == typeof(SavedCircuitBoard)) { SetCircuitBoardData((SavedCircuitBoard)save, WorldObject); }
//        if (save is SavedWire) { return WireData(save); }
//        if (save is SavedInverter) { return InverterData(save); }
//        if (save is SavedPeg) { return PegData(save); }
//        if (save is SavedDelayer) { return DelayerData(save); }
//        if (save is SavedThroughPeg) { return ThroughPegData(save); }
//        if (save is SavedSwitch || save.ObjectType == "Panel Switch") { return SwitchData(save); }
//        if (save is SavedButton || save.ObjectType == "Panel Button") { return ButtonData(save); }
//        if (save is SavedDisplay || save.ObjectType == "Panel Display") { return DisplayData(save); }
//        if (save is SavedLabel || save.ObjectType == "Panel Label") { return LabelData(save); }
//        if (save is SavedBlotter || save.ObjectType == "Through Blotter") { return BlotterData(save); }

//        else
//        {
//            Debug.LogError("ObjectType not found. ObjectType was " + save.ObjectType);
//            return null;
//        }
//    }

//    public static void LoadData(GameObject LoadedObject, SavedObjectV2 save)
//    {
//        if (save.ObjectType == "CircuitBoard") { LoadCircuitBoard(LoadedObject, save); }
//        else if (save.ObjectType == "Wire") { LoadWire(LoadedObject, save); }
//        else if (save.ObjectType == "Inverter") { LoadInverter(LoadedObject, save); }
//        else if (save.ObjectType == "Peg") { LoadPeg(LoadedObject, save); }
//        else if (save.ObjectType == "Delayer") { LoadDelayer(LoadedObject, save); }
//        else if (save.ObjectType == "Through Peg") { LoadThroughPeg(LoadedObject, save); }
//        else if (save.ObjectType == "Switch" || save.ObjectType == "Panel Switch") { LoadSwitch(LoadedObject, save); }
//        else if (save.ObjectType == "Button" || save.ObjectType == "Panel Button") { LoadButton(LoadedObject, save); }
//        else if (save.ObjectType == "Display" || save.ObjectType == "Panel Display") { LoadDisplay(LoadedObject, save); }
//        else if (save.ObjectType == "Label" || save.ObjectType == "Panel Label") { LoadLabel(LoadedObject, save); }
//        else if (save.ObjectType == "Blotter" || save.ObjectType == "Through Blotter") { LoadBlotter(LoadedObject, save); }

//        else
//        {
//            Debug.LogError("ObjectType not found. ObjectType was " + save.ObjectType);
//        }
//    }


//    private static void SetCircuitBoardData(SavedCircuitBoard save, Transform worldobject)
//    {
//        CircuitBoard board = worldobject.GetComponent<CircuitBoard>();

//        save.x = board.x;
//        save.z = board.z;
//        save.color = worldobject.GetComponent<Renderer>().material.color;
//    }

//    private static void LoadCircuitBoard(GameObject LoadedObject, SavedCircuitBoard save)
//    {
//        CircuitBoard board = LoadedObject.GetComponent<CircuitBoard>();

//        board.x = save.x;
//        board.z = save.z;
//        board.SetBoardColor(save.color);

//        board.CreateCuboid();
//    }

//    private static void SetWireData(SavedWire save, Transform worldobject)
//    {
//        InputInputConnection IIConnection = worldobject.GetComponent<InputInputConnection>();
//        if(IIConnection != null)
//        {
//            save.InputInput = true; // bool of the array is set to true if IIConnection, false if IOConnection
//        }
//        else
//        {
//            save.InputInput = false;
//        }

//        save.length = worldobject.localScale.z;
//    }

//    private static void LoadWire(GameObject LoadedObject, SavedWire save)
//    {
//        if(save.InputInput)
//        {
//            LoadedObject.AddComponent<InputInputConnection>();
//        }
//        else
//        {
//            LoadedObject.AddComponent<InputOutputConnection>();
//        }

//        LoadedObject.transform.localScale = new Vector3(0.05f, 0.02f, save.length); // todo: merge this in some way with Wire.DrawWire
//    }

//    private static void SetButtonData(SavedButton save)
//    {
//        return; // buttons have no important data!
//    }

//    private static void LoadButton(GameObject LoadedObject, SavedObjectV2 save)
//    {
//        return; // buttons still have no important data!
//    }

//    private static object[] DelayerData(SaveThisObject save)
//    {
//        Delayer delayer = save.GetComponent<Delayer>();

//        object[] data = {
//            delayer.Input.On,
//            delayer.Output.On,
//            delayer.DelayCount
//        };

//        return data;
//    }

//    private static void LoadDelayer(GameObject LoadedObject, SavedObjectV2 save)
//    {
//        object[] data = save.CustomDataArray;
//        Delayer delayer = LoadedObject.GetComponent<Delayer>();

//        delayer.Input.On = (bool)data[0];
//        delayer.Output.On = (bool)data[1];
//        delayer.DelayCount = (int)data[2];
//    }

//    private static object[] DisplayData(SaveThisObject save)
//    {
//        Display display = save.GetComponentInChildren<Display>();

//        object[] data = {
//            display.Input.On,
//            display.DisplayColor
//        };

//        return data;
//    }

//    private static void LoadDisplay(GameObject LoadedObject, SavedObjectV2 save)
//    {
//        object[] data = save.CustomDataArray;
//        Display display = LoadedObject.GetComponentInChildren<Display>();

//        display.Input.On = (bool)data[0];
//        display.DisplayColor = (DisplayColor)data[1];
//    }

//    private static object[] InverterData(SaveThisObject save)
//    {
//        Inverter notgate = save.GetComponent<Inverter>();

//        object[] data = {
//            notgate.Input.On,
//            notgate.Output.On
//        };

//        return data;
//    }

//    private static void LoadInverter(GameObject LoadedObject, SavedObjectV2 save)
//    {
//        object[] data = save.CustomDataArray;
//        Inverter notgate = LoadedObject.GetComponent<Inverter>();

//        notgate.Input.On = (bool)data[0];
//        notgate.Output.On = (bool)data[1];
//    }

//    private static object[] LabelData(SaveThisObject save)
//    {
//        Label label = save.GetComponent<Label>();

//        object[] data = {
//            label.text.text,
//            label.text.fontSize
//        };

//        return data;
//    }

//    private static void LoadLabel(GameObject LoadedObject, SavedObjectV2 save)
//    {
//        object[] data = save.CustomDataArray;
//        Label label = LoadedObject.GetComponent<Label>();

//        label.text.text = (string)data[0];
//        label.text.fontSize = (float)data[1];
//    }

//    private static object[] SwitchData(SaveThisObject save)
//    {
//        Switch circuitswitch = save.GetComponentInChildren<Switch>();

//        object[] data = {
//            circuitswitch.On
//        };

//        return data;
//    }

//    private static void LoadSwitch(GameObject LoadedObject, SavedObjectV2 save)
//    {
//        object[] data = save.CustomDataArray;
//        Switch circuitswitch = LoadedObject.GetComponentInChildren<Switch>();

//        circuitswitch.On = (bool)data[0];
//        circuitswitch.UpdateLever(false); // apply the effects of the bool we just loaded, don't play sound
//    }

//    private static object[] PegData(SaveThisObject save)
//    {
//        CircuitInput input = save.GetComponent<CircuitInput>();

//        object[] data = {
//            input.On
//        };

//        return data;
//    }

//    private static void LoadPeg(GameObject LoadedObject, SavedObjectV2 save)
//    {
//        object[] data = save.CustomDataArray;
//        CircuitInput input = LoadedObject.GetComponent<CircuitInput>();

//        input.On = (bool)data[0];
//    }

//    private static object[] ThroughPegData(SaveThisObject save)
//    {
//        CircuitInput[] inputs = save.GetComponentsInChildren<CircuitInput>();

//        object[] data = {
//            inputs[0].On
//        };

//        return data;
//    }

//    private static void LoadThroughPeg(GameObject LoadedObject, SavedObjectV2 save)
//    {
//        object[] data = save.CustomDataArray;
//        CircuitInput[] inputs = LoadedObject.GetComponentsInChildren<CircuitInput>();

//        inputs[0].On = (bool)data[0];
//        inputs[1].On = (bool)data[0];
//    }

//    private static object[] BlotterData(SaveThisObject save)
//    {
//        Blotter blotter = save.GetComponent<Blotter>();

//        object[] data = {
//            blotter.Input.On,
//            blotter.Output.On
//        };

//        return data;
//    }

//    private static void LoadBlotter(GameObject LoadedObject, SavedObjectV2 save)
//    {
//        object[] data = save.CustomDataArray;
//        Blotter blotter = LoadedObject.GetComponent<Blotter>();

//        blotter.Input.On = (bool)data[0];
//        blotter.Output.On = (bool)data[1];
//    }
//}

//public class SaveObjectsList
//{
//    public static GameObject ObjectTypeToPrefab(string ObjectType)
//    {
//        if (ObjectType == "CircuitBoard") { return Prefabs.CircuitBoard; }
//        if (ObjectType == "Wire") { return Prefabs.Wire; }
//        if (ObjectType == "Inverter") { return Prefabs.Inverter; }
//        if (ObjectType == "Peg") { return Prefabs.Peg; }
//        if (ObjectType == "Delayer") { return Prefabs.Delayer; }
//        if (ObjectType == "Through Peg") { return Prefabs.ThroughPeg; }
//        if (ObjectType == "Switch") { return Prefabs.Switch; }
//        if (ObjectType == "Button") { return Prefabs.Button; }
//        if (ObjectType == "Display") { return Prefabs.Display; }
//        if (ObjectType == "Label") { return Prefabs.Label; }
//        if (ObjectType == "Panel Switch") { return Prefabs.PanelSwitch; }
//        if (ObjectType == "Panel Button") { return Prefabs.PanelButton; }
//        if (ObjectType == "Panel Display") { return Prefabs.PanelDisplay; }
//        if (ObjectType == "Panel Label") { return Prefabs.PanelLabel; }
//        if (ObjectType == "Blotter") { return Prefabs.Blotter; }
//        if (ObjectType == "Through Blotter") { return Prefabs.ThroughBlotter; }

//        else { Debug.LogError("Unkonwn object type. Type was " + ObjectType.ToString()); return null; }
//    }
//}