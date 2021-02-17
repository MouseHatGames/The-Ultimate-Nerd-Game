using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorManager : MonoBehaviour {

    private CustomFixedUpdate CircuitLogicUpdate;

    private void Awake()
    {
        // set these values if they don't exist in files
        // TODO: unify this code that's repeated a hundred fucking times god dammit
        if (!ES3.KeyExists("CircuitUpdatesPerSecond", "settings.txt"))
        {
            ES3.Save<float>("CircuitUpdatesPerSecond", (float)100, "settings.txt"); // it is VERY IMPORTANT that the number here be cast as a float!
        }
        float CircuitUpdatesPerSecond = ES3.Load<float>("CircuitUpdatesPerSecond", "settings.txt", 100);

        //ClearLists();
        CircuitLogicUpdate = new CustomFixedUpdate(OnCircuitLogicUpdate, CircuitUpdatesPerSecond);
    }


    public static bool AllowedToUpdate = true; // set to false and then back to true by SaveManager.LoadAll

    void Update () {
        if (!AllowedToUpdate) { return; }

        CircuitLogicUpdate.Update();

        VisualStuffUpdate();
	}

    void OnCircuitLogicUpdate(float dt)
    {
        // update order here is important!!
        foreach (Button button in UpdatedButtons) { button.CircuitLogicUpdate(); } // first, poll for inputs. (this being at the beginning instead of the end isn't really important at the default 100 ticks/second, but at lower values it's noticable. (Probably. Haven't tested it))
        foreach (Switch circuitswitch in UpdatedSwitches) { circuitswitch.CircuitLogicUpdate(); }
        foreach (WireCluster cluster in UpdatedClusters) { cluster.CircuitLogicUpdate(); } // then, clusters determine their state based on output states
        foreach (CircuitInput input in UpdatedInputs) { input.CircuitLogicUpdate(); } // next, inputs set their states to that of their clusters
        foreach (Blotter blotter in UpdatedBlotters) { blotter.CircuitLogicUpdate(); } // finally, the logic components calculate output states.
        foreach (NotGate notgate in UpdatedNotGates) { notgate.CircuitLogicUpdate(); }
        foreach (Delayer delayer in UpdatedDelayers) { delayer.CircuitLogicUpdate(); }
    }

    void VisualStuffUpdate()
    {
        foreach (WireCluster cluster in UpdatedClusters) { cluster.VisualUpdate(); }
        foreach (Output output in UpdatedOutputs) { output.VisualUpdate(); }
        foreach (Display display in UpdatedDisplays) { display.VisualUpdate(); }

        MegaCircuitMesh.Instance.VisualUpdate();
    }

    // everything that gets updated
    // these things all add and remove themselves in their Awake and OnDestroy functions
    public static List<WireCluster> UpdatedClusters = new List<WireCluster>();
    public static List<Output> UpdatedOutputs = new List<Output>();
    public static List<CircuitInput> UpdatedInputs = new List<CircuitInput>();
    public static List<Blotter> UpdatedBlotters = new List<Blotter>();
    public static List<Button> UpdatedButtons = new List<Button>();
    public static List<Delayer> UpdatedDelayers = new List<Delayer>();
    public static List<Display> UpdatedDisplays = new List<Display>();
    public static List<NotGate> UpdatedNotGates = new List<NotGate>();
    public static List<Switch> UpdatedSwitches = new List<Switch>();

    public static void ClearLists()
    {
        UpdatedClusters.Clear();
        UpdatedOutputs.Clear();
        UpdatedInputs.Clear();
        UpdatedBlotters.Clear();
        UpdatedButtons.Clear();
        UpdatedDelayers.Clear();
        UpdatedDisplays.Clear();
        UpdatedNotGates.Clear();
        UpdatedSwitches.Clear();
    }

    private void OnDestroy() // hopefully called during scene changes too
    {
        ClearLists();
    }
}
