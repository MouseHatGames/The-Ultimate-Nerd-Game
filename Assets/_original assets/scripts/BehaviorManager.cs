// My replacement for Unity's update loop. Allows for much better managing of what happens in what order every frame,
// and also allows circuit updates to be independant from the physics loop.

using System.Collections.Generic;
using UnityEngine;

public class BehaviorManager : MonoBehaviour
{
    public static string Greetings = "Hello, modder. I hope you're having a nice day :)";

    private CustomFixedUpdate CircuitLogicUpdate;
    private CustomFixedUpdate MeshQueueing;

    private void Awake()
    {
        float CircuitUpdatesPerSecond = Settings.Get("CircuitUpdatesPerSecond", 100f);
        CircuitLogicUpdate = new CustomFixedUpdate(OnCircuitLogicUpdate, CircuitUpdatesPerSecond);

        float MeshGroupRecalculationsPerSecond = Settings.Get("MaxMeshGroupRecalculationsPerSecond", 20f);
        MeshQueueing = new CustomFixedUpdate(QueueRecalculationForAppropriateDynamicMegaMeshGroups, MeshGroupRecalculationsPerSecond);
    }

    public static bool AllowedToUpdate = true; // set to false and then back to true by SaveManager.LoadAll to make sure circuitry doesn't run before it's finished loading.

    private void Update ()
    {
        ScreenshotTaker.RunScreenshotTaking();

        if (!AllowedToUpdate) { return; }

        GameplayUIManager.RunGameplayUI();

        CircuitLogicUpdate.Update(); // might affect VisualStuffUpdate
        VisualStuffUpdate(); // might affect mega mesh stuff

        MeshQueueing.Update();
        RecalculateQueuedMegaMeshGroups();
	}

    private static void QueueRecalculationForAppropriateDynamicMegaMeshGroups(float dt)
    {
        if (DynamicMegaMeshesThatNeedRecalculating.Count < 1) { return; }
        DynamicMegaMeshesThatNeedRecalculating[0].RecalculateNextFrame();
        DynamicMegaMeshesThatNeedRecalculating.RemoveAt(0);
    }

    private static void OnCircuitLogicUpdate(float dt)
    {
        //DebugNumerOfDuplicates(UpdatingCircuitLogicComponents, "UpdatingCircuitLogicComponents");
        //DebugNumerOfDuplicates(UpdatingClusters, "UpdatingClusters");

        // for loops are much faster than foreach loops!

        for (int i = 0; i < UpdatingClusters.Count; i++) { UpdatingClusters[i].CircuitLogicUpdate(); } // clusters determine their state based on output states
        UpdatingClusters.Clear(); // each cluster updates only once so they are cleared immediately

        for (int i = 0; i < UpdatingCircuitLogicComponents.Count; i++) { UpdatingCircuitLogicComponents[i].DoCircuitLogicUpdate(); } // then, the logic components calculate output states.
        UpdatingCircuitLogicComponents.Clear();

        // deal with components that update over multiple ticks. At the time of this comment, only Delayers do this
        UpdatingCircuitLogicComponents.AddRange(ContinuousUpdatingCircuitLogicComponents);
        ContinuousUpdatingCircuitLogicComponents.Clear();


        //System.Threading.Tasks.Parallel.ForEach(UpdatingCircuitLogicComponents, component => { component.CircuitLogicUpdate(); }); // never mind turns out multithreading is really hard lol
    }

    private static void VisualStuffUpdate()
    {
        int i = 0;
        while (i < CurrentlyUpdatingVisually.Count)
        {
            VisualUpdaterWithMeshCombining visualboi = CurrentlyUpdatingVisually[i];

            if (visualboi.FinishedUpdating || visualboi == null)
            {
                // remove from the list in O(1) time by sending the last thing in the list to this position in the list and then removing the last thing in the list
                int LastInList = CurrentlyUpdatingVisually.Count - 1;
                CurrentlyUpdatingVisually[i] = CurrentlyUpdatingVisually[LastInList];
                CurrentlyUpdatingVisually.RemoveAt(LastInList);
            }
            else
            {
                // if we have not done the fancy removal thing, we need to move on to the next item
                visualboi.VisualUpdate();
                i++;
            }
        }
    }

    private static void RecalculateQueuedMegaMeshGroups()
    {
        for (int i = 0; i < ClustersQueuedForMeshRecalculation.Count; i++) { ClustersQueuedForMeshRecalculation[i].RecalculateCombinedMesh(); }
        for (int i = 0; i < OutputsQueuedForMeshRecalculation.Count; i++) { OutputsQueuedForMeshRecalculation[i].RecalculateCombinedMesh(); }
        for (int i = 0; i < MegaMeshGroupsQueuedForRecalculation.Count; i++) { MegaMeshGroupsQueuedForRecalculation[i].RecalculateMesh(); }

        ClearMeshRecalculationLists(); // since everything above should only be run once until it's queued again
    }

    // code managing these lists must be VERY CAREFUL to never have duplicates in the lists. We use lits over a hash IEnumerable because of the iteration performance

    // stuff that needs to be updated in the circuit logic code
    public static List<WireCluster> UpdatingClusters = new List<WireCluster>();
    public static List<CircuitLogicComponent> UpdatingCircuitLogicComponents = new List<CircuitLogicComponent>();
    public static List<CircuitLogicComponent> ContinuousUpdatingCircuitLogicComponents = new List<CircuitLogicComponent>(); // components that update for more than one tick

    // visually updating stuff
    public static List<VisualUpdaterWithMeshCombining> CurrentlyUpdatingVisually = new List<VisualUpdaterWithMeshCombining>();

    // meshes to be recalculated
    public static List<MegaMeshGroup> MegaMeshGroupsQueuedForRecalculation = new List<MegaMeshGroup>();
    public static List<WireCluster> ClustersQueuedForMeshRecalculation = new List<WireCluster>();
    public static List<CircuitOutput> OutputsQueuedForMeshRecalculation = new List<CircuitOutput>();

    public static List<MegaMeshGroup> DynamicMegaMeshesThatNeedRecalculating = new List<MegaMeshGroup>();

    public static void ClearAllLists()
    {
        UpdatingClusters.Clear();
        UpdatingCircuitLogicComponents.Clear();
        CurrentlyUpdatingVisually.Clear();
        MegaMeshGroupsQueuedForRecalculation.Clear();
        ClustersQueuedForMeshRecalculation.Clear();
        OutputsQueuedForMeshRecalculation.Clear();
        DynamicMegaMeshesThatNeedRecalculating.Clear();
    }

    private static void ClearMeshRecalculationLists()
    {
        MegaMeshGroupsQueuedForRecalculation.Clear();
        ClustersQueuedForMeshRecalculation.Clear();
        OutputsQueuedForMeshRecalculation.Clear();
    }


    private void OnDestroy() // hopefully called during scene changes too
    {
        ClearAllLists();
    }

    private void OnApplicationQuit()
    {
        float PreviousSecondsPlayed = ES3.Load("SecondsPlayed", "TimePlayed.txt", 0f);
        ES3.Save<float>("SecondsPlayed", PreviousSecondsPlayed + Time.unscaledTime, "TimePlayed.txt");
    }

#if UNITY_EDITOR

    [NaughtyAttributes.Button]
    private void DebugCounts()
    {
        Debug.Log("updating: clusters - " + UpdatingClusters.Count + ". CLCs - " + UpdatingCircuitLogicComponents.Count + ". Continuous CLCs - " + ContinuousUpdatingCircuitLogicComponents.Count + ". Visuals - " + CurrentlyUpdatingVisually.Count + ".");
    }

    private static void DebugNumerOfDuplicates(List<CircuitLogicComponent> ass, string listname)
    {
        Dictionary<CircuitLogicComponent, int> breasts = new Dictionary<CircuitLogicComponent, int>();

        foreach (CircuitLogicComponent dick in ass)
        {
            if (breasts.ContainsKey(dick))
            {
                breasts[dick]++;
            }
            else
            {
                breasts.Add(dick, 1);
            }
        }

        foreach (KeyValuePair<CircuitLogicComponent, int> pussy in breasts)
        {
            if (pussy.Value > 1) { Debug.LogFormat(pussy.Value + " duplicates of object named " + pussy.Key.name + " in " + listname); }
        }
    }

    private static void DebugNumerOfDuplicates(List<WireCluster> ass, string listname)
    {
        Dictionary<WireCluster, int> breasts = new Dictionary<WireCluster, int>();

        foreach (WireCluster dick in ass)
        {
            if (breasts.ContainsKey(dick))
            {
                breasts[dick]++;
            }
            else
            {
                breasts.Add(dick, 1);
            }
        }

        foreach (KeyValuePair<WireCluster, int> pussy in breasts)
        {
            if (pussy.Value > 1) { Debug.LogFormat(pussy.Value + " duplicates of object named " + pussy.Key.name + " in " + listname); }
        }
    }

    private static void DebugNumerOfDuplicates(List<MegaMeshGroup> ass, string listname)
    {
        Dictionary<MegaMeshGroup, int> breasts = new Dictionary<MegaMeshGroup, int>();

        foreach (MegaMeshGroup dick in ass)
        {
            if (breasts.ContainsKey(dick))
            {
                breasts[dick]++;
            }
            else
            {
                breasts.Add(dick, 1);
            }
        }

        foreach (KeyValuePair<MegaMeshGroup, int> pussy in breasts)
        {
            if (pussy.Value > 1) { Debug.LogFormat(pussy.Value + " duplicates of object named " + pussy.Key.name + " in " + listname); }
        }
    }

    private static void DebugNumerOfDuplicates(List<CircuitOutput> ass, string listname)
    {
        Dictionary<CircuitOutput, int> breasts = new Dictionary<CircuitOutput, int>();

        foreach (CircuitOutput dick in ass)
        {
            if (breasts.ContainsKey(dick))
            {
                breasts[dick]++;
            }
            else
            {
                breasts.Add(dick, 1);
            }
        }

        foreach (KeyValuePair<CircuitOutput, int> pussy in breasts)
        {
            if (pussy.Value > 1) { Debug.LogFormat(pussy.Value + " duplicates of object named " + pussy.Key.name + " in " + listname); }
        }
    }

#endif
}