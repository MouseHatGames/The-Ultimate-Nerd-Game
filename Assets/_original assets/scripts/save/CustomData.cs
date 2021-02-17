using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomData {

	public static object[] SaveObjectData(SaveThisObject save) // updates a SaveThisObject to have its current relevant CustomData[] values
    {
        if (save.ObjectType == "CircuitBoard") { return CircuitBoardData(save); }
        if (save.ObjectType == "Wire") { return WireData(save); }
        if (save.ObjectType == "Inverter") { return InverterData(save); }
        if (save.ObjectType == "Peg") { return PegData(save); }
        if (save.ObjectType == "Delayer") { return DelayerData(save); }
        if (save.ObjectType == "Through Peg") { return ThroughPegData(save); }
        if (save.ObjectType == "Switch" || save.ObjectType == "Panel Switch") { return SwitchData(save); }
        if (save.ObjectType == "Button" || save.ObjectType == "Panel Button") { return ButtonData(save); }
        if (save.ObjectType == "Display" || save.ObjectType == "Panel Display") { return DisplayData(save); }
        if (save.ObjectType == "Label" || save.ObjectType == "Panel Label") { return LabelData(save); }
        if (save.ObjectType == "Blotter" || save.ObjectType == "Through Blotter") { return BlotterData(save); }

        else
        {
            Debug.LogError("ObjectType not found. ObjectType was " + save.ObjectType);
            return null;
        }
    }

    public static void LoadData(SaveThisObject save)
    {
        if (save.ObjectType == "CircuitBoard") { LoadCircuitBoard(save); }
        else if (save.ObjectType == "Wire") { LoadWire(save); }
        else if (save.ObjectType == "Inverter") { LoadInverter(save); }
        else if (save.ObjectType == "Peg") { LoadPeg(save); }
        else if (save.ObjectType == "Delayer") { LoadDelayer(save); }
        else if (save.ObjectType == "Through Peg") { LoadThroughPeg(save); }
        else if (save.ObjectType == "Switch" || save.ObjectType == "Panel Switch") { LoadSwitch(save); }
        else if (save.ObjectType == "Button" || save.ObjectType == "Panel Button") { LoadButton(save); }
        else if (save.ObjectType == "Display" || save.ObjectType == "Panel Display") { LoadDisplay(save); }
        else if (save.ObjectType == "Label" || save.ObjectType == "Panel Label") { LoadLabel(save); }
        else if (save.ObjectType == "Blotter" || save.ObjectType == "Through Blotter") { LoadBlotter(save); }
    }


    public static object[] CircuitBoardData(SaveThisObject save)
    {
        CircuitBoard board = save.GetComponent<CircuitBoard>();

        // data saved in boards: board size x, board size z, color
        object[] data = {
            board.x,
            board.z,
            save.GetComponent<Renderer>().material.color
        };

        return data;
    }

    public static void LoadCircuitBoard(SaveThisObject save)
    {
        object[] data = save.CustomDataArray;
        CircuitBoard board = save.GetComponent<CircuitBoard>();

        board.x = (int)data[0];
        board.z = (int)data[1];
        board.BoardColor = (Color)data[2];
        save.GetComponent<Renderer>().material.color = (Color)data[2];

        board.CreateCuboid();
    }

    public static object[] WireData(SaveThisObject save)
    {
        object[] data = new object[2];

        InputInputConnection IIConnection = save.GetComponent<InputInputConnection>();
        if(IIConnection != null)
        {
            data[0] = true; // bool of the array is set to true if IIConnection, false if IOConnection
        }
        else
        {
            data[0] = false;
        }

        data[1] = save.transform.localScale;
       
        return data;
    }

    public static void LoadWire(SaveThisObject save)
    {
        if((bool)save.CustomDataArray[0] == true)
        {
            save.gameObject.AddComponent<InputInputConnection>();
        }
        else
        {
            save.gameObject.AddComponent<InputOutputConnection>();
        }

        save.transform.localScale = (Vector3)save.CustomDataArray[1];
    }

    public static object[] ButtonData(SaveThisObject save)
    {
        Button button = save.GetComponent<Button>();

        object[] data = {
            button.output.On,
            button.ButtonDownTime // we save gritty details like this because people can build carefully timed circuits that would get fucked up if the number was reset each time
        };

        return data;
    }

    public static void LoadButton(SaveThisObject save)
    {
        object[] data = save.CustomDataArray;
        Button button = save.GetComponent<Button>();

        button.output.On = (bool)data[0];
        button.ButtonDownTime = (int)data[1];
    }

    public static object[] DelayerData(SaveThisObject save)
    {
        Delayer delayer = save.GetComponent<Delayer>();

        object[] data = {
            delayer.Input.On,
            delayer.Output.On,
            delayer.DelayCount
        };

        return data;
    }

    public static void LoadDelayer(SaveThisObject save)
    {
        object[] data = save.CustomDataArray;
        Delayer delayer = save.GetComponent<Delayer>();

        delayer.Input.On = (bool)data[0];
        delayer.Output.On = (bool)data[1];
        delayer.DelayCount = (int)data[2];
    }

    public static object[] DisplayData(SaveThisObject save)
    {
        Display display = save.GetComponent<Display>();

        object[] data = {
            display.Input.On
        };

        return data;
    }

    public static void LoadDisplay(SaveThisObject save)
    {
        object[] data = save.CustomDataArray;
        Display display = save.GetComponent<Display>();

        display.Input.On = (bool)data[0];
    }

    public static object[] InverterData(SaveThisObject save)
    {
        NotGate notgate = save.GetComponent<NotGate>();

        object[] data = {
            notgate.Input.On,
            notgate.Output.On
        };

        return data;
    }

    public static void LoadInverter(SaveThisObject save)
    {
        object[] data = save.CustomDataArray;
        NotGate notgate = save.GetComponent<NotGate>();

        notgate.Input.On = (bool)data[0];
        notgate.Output.On = (bool)data[1];
    }

    public static object[] LabelData(SaveThisObject save)
    {
        Label label = save.GetComponent<Label>();

        object[] data = {
            label.text.text,
            label.text.fontSize
        };

        return data;
    }

    public static void LoadLabel(SaveThisObject save)
    {
        object[] data = save.CustomDataArray;
        Label label = save.GetComponent<Label>();

        label.text.text = (string)data[0];
        label.text.fontSize = (float)data[1];
    }

    public static object[] SwitchData(SaveThisObject save)
    {
        Switch circuitswitch = save.GetComponent<Switch>();

        object[] data = {
            circuitswitch.On
        };

        return data;
    }

    public static void LoadSwitch(SaveThisObject save)
    {
        object[] data = save.CustomDataArray;
        Switch circuitswitch = save.GetComponent<Switch>();

        circuitswitch.On = (bool)data[0];
        circuitswitch.UpdateLeverNoSound(); // apply the effects of the bool we just loaded
    }

    public static object[] PegData(SaveThisObject save)
    {
        CircuitInput input = save.GetComponent<CircuitInput>();

        object[] data = {
            input.On
        };

        return data;
    }

    public static void LoadPeg(SaveThisObject save)
    {
        object[] data = save.CustomDataArray;
        CircuitInput input = save.GetComponent<CircuitInput>();

        input.On = (bool)data[0];
    }

    public static object[] ThroughPegData(SaveThisObject save)
    {
        CircuitInput[] inputs = save.GetComponentsInChildren<CircuitInput>();

        object[] data = {
            inputs[0].On
        };

        return data;
    }

    public static void LoadThroughPeg(SaveThisObject save)
    {
        object[] data = save.CustomDataArray;
        CircuitInput[] inputs = save.GetComponentsInChildren<CircuitInput>();

        inputs[0].On = (bool)data[0];
        inputs[1].On = (bool)data[0];
    }

    public static object[] BlotterData(SaveThisObject save)
    {
        Blotter blotter = save.GetComponent<Blotter>();

        object[] data = {
            blotter.Input.On,
            blotter.Output.On
        };

        return data;
    }

    public static void LoadBlotter(SaveThisObject save)
    {
        object[] data = save.CustomDataArray;
        Blotter blotter = save.GetComponent<Blotter>();

        blotter.Input.On = (bool)data[0];
        blotter.Output.On = (bool)data[1];
    }
}

public class SaveObjectsList
{
    public static GameObject CircuitBoard;
    public static GameObject Wire;
    public static GameObject Inverter;
    public static GameObject Peg;
    public static GameObject Delayer;
    public static GameObject ThroughPeg;
    public static GameObject Switch;
    public static GameObject Button;
    public static GameObject Display;
    public static GameObject Label;
    public static GameObject PanelSwitch;
    public static GameObject PanelButton;
    public static GameObject PanelDisplay;
    public static GameObject PanelLabel;
    public static GameObject Blotter;
    public static GameObject ThroughBlotter;

    public static GameObject ObjectTypeToPrefab(string ObjectType)
    {
        if (ObjectType == "CircuitBoard") { return CircuitBoard; }
        if (ObjectType == "Wire") { return Wire; }
        if (ObjectType == "Inverter") { return Inverter; }
        if (ObjectType == "Peg") { return Peg; }
        if (ObjectType == "Delayer") { return Delayer; }
        if (ObjectType == "Through Peg") { return ThroughPeg; }
        if (ObjectType == "Switch") { return Switch; }
        if (ObjectType == "Button") { return Button; }
        if (ObjectType == "Display") { return Display; }
        if (ObjectType == "Label") { return Label; }
        if (ObjectType == "Panel Switch") { return PanelSwitch; }
        if (ObjectType == "Panel Button") { return PanelButton; }
        if (ObjectType == "Panel Display") { return PanelDisplay; }
        if (ObjectType == "Panel Label") { return PanelLabel; }
        if (ObjectType == "Blotter") { return Blotter; }
        if (ObjectType == "Through Blotter") { return ThroughBlotter; }

        else { Debug.Log("Error: unkonwn object type. Type was " + ObjectType.ToString()); return null; }
    }
}