// because you STILL can't fucking assign static variables in the inspector!!!
// I should really learn editor scripting and then write an editor script that lets me do that

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticObjectAssigner : MonoBehaviour {

    // public versions of static prefabs that need assigning to
    public GameObject WirePrefab;
    public GameObject ClusterPrefab;
    public GameObject SharedOutputMeshPrefab;
    public GameObject MegaBoardMeshPrefab;
    public Transform MegaMeshesParent;

    public TextEditMenu texteditmenu;

    public GameObject CircuitBoard;
    public GameObject Wire;
    public GameObject Inverter;
    public GameObject Peg;
    public GameObject Delayer;
    public GameObject ThroughPeg;
    public GameObject Switch;
    public GameObject Button;
    public GameObject Display;
    public GameObject LabelObject;
    public GameObject PanelSwitch;
    public GameObject PanelButton;
    public GameObject PanelDisplay;
    public GameObject PanelLabel;
    public GameObject Blotter;
    public GameObject ThroughBlotter;


    // Use this for initialization
    void Awake () {
        StuffConnecter.WirePrefab = WirePrefab;
        StuffConnecter.ClusterPrefab = ClusterPrefab;
        Output.SharedOutputMeshPrefab = SharedOutputMeshPrefab;
        MegaBoardMeshManager.MegaBoardMeshPrefab = MegaBoardMeshPrefab;
        MegaBoardMeshManager.MegaMeshesParent = MegaMeshesParent;

        Label.texteditmenu = texteditmenu;


        SaveObjectsList.CircuitBoard = CircuitBoard;
        SaveObjectsList.Wire = Wire;
        SaveObjectsList.Inverter = Inverter;
        SaveObjectsList.Peg = Peg;
        SaveObjectsList.Delayer = Delayer;
        SaveObjectsList.ThroughPeg = ThroughPeg;
        SaveObjectsList.Switch = Switch;
        SaveObjectsList.Button = Button;
        SaveObjectsList.Display = Display;
        SaveObjectsList.Label = LabelObject;
        SaveObjectsList.PanelSwitch = PanelSwitch;
        SaveObjectsList.PanelButton = PanelButton;
        SaveObjectsList.PanelDisplay = PanelDisplay;
        SaveObjectsList.PanelLabel = PanelLabel;
        SaveObjectsList.Blotter = Blotter;
        SaveObjectsList.ThroughBlotter = ThroughBlotter;
    }
}