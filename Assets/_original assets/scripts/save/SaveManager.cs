// this is overwhelmingly complex for me. The code here will not be designed for efficiency, but for working at all, simplicity, and readability.
// also we have great loading screens so it's fine to let people watch them longer :)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityStandardAssets.Characters.FirstPerson;
using References;
using SavedObjects;
using CielaSpike;

public static class SaveManager
{
    private static string TrueSaveName = "error_ save name was not set!";
    public static string SaveName
    {
        get { return TrueSaveName; }
        set { TrueSaveName = FileUtilities.ValidatedFileName(value); }
    }
    public static string SavePath { get { return Application.persistentDataPath + "/saves/" + SaveName; } }
    public static string InfoPath { get { return SavePath + "/worldinfo.txt"; } }
    public static string PlayersPath { get { return SavePath + "/players"; } } // 👌
    public static string RegionsPath { get { return SavePath + "/regions"; } }

    public static List<ObjectInfo> ActiveSaveObjects = new List<ObjectInfo>();

    public static void SaveAllSynchronously()
    {
        SavePlayerPosition();
        SaveMiscData();

        List<SavedObjectV2> TopLevelObjects = GetTopLevelObjects();
        FileUtilities.SaveToFile(RegionsPath, "world.tung", TopLevelObjects);

        Debug.Log("saved game synchronously");
    }

    public static IEnumerator SaveAllAsynchronously()
    {
        yield return Ninja.JumpToUnity;
        SavePlayerPosition();
        SaveMiscData();
        string path = RegionsPath;

        List<SavedObjectV2> TopLevelObjects = GetTopLevelObjects();

        yield return Ninja.JumpBack;
        FileUtilities.SaveToFile(path, "world.tung", TopLevelObjects);

        Debug.Log("saved game asynchronously");
    }

    public static List<SavedObjectV2> GetTopLevelObjects()
    {
        List<SavedObjectV2> TopLevelObjects = new List<SavedObjectV2>();

        foreach (ObjectInfo save in ActiveSaveObjects)
        {
            if (save.transform.parent == null) { TopLevelObjects.Add(SavedObjectUtilities.CreateSavedObjectFrom(save)); }
        }

        return TopLevelObjects;
    }

    public static void SaveMiscData()
    {
        ES3.Save<int>("SaveFormatVersion", 2, InfoPath);
        ES3.Save<string>("WorldType", "Legacy", InfoPath);
    }

    public static void LoadAll()
    {
        MegaMeshManager.ClearReferences();
        BehaviorManager.AllowedToUpdate = false; // don't let circuitry updates fuck us up while we're loading the game
        BehaviorManager.ClearAllLists();

        // remove all existing stuff in the world before loading the new stuff
        ObjectInfo[] saveobjects = UnityEngine.Object.FindObjectsOfType<ObjectInfo>(); // all active instances of SaveObject
        foreach (ObjectInfo save in saveobjects) { UnityEngine.Object.Destroy(save.gameObject); }

        List<SavedObjectV2> TopLevelObjects = (List<SavedObjectV2>)FileUtilities.LoadFromFile(RegionsPath, "world.tung");
        if (TopLevelObjects != null)
        {
            foreach (SavedObjectV2 save in TopLevelObjects)
            {
                SavedObjectUtilities.LoadSavedObject(save);
            }
        }

        RecalculateAllClustersEverywhereWithDelay();
        LoadPlayerPosition();
        GameplayUIManager.UIState = UIState.None;
    }

    public static void RecalculateAllClustersEverywhereWithDelay()
    {
        // the best way to do this is with a monobehaviour, so we create a new gameobject then add that behaviour to it
        GameObject penis = new GameObject();
        penis.AddComponent<PauseDelayThenRecalculate>();
    }

    public static void RecalculateAllClustersEverywhere()
    {
        // we used to destroy all existing clusters, but I took this out to make through pegs work in 0.2. I sure hope this wasn't necessary!
        //WireCluster[] existingclusters = UnityEngine.Object.FindObjectsOfType<WireCluster>();
        //foreach (WireCluster oldcluster in existingclusters) { UnityEngine.Object.Destroy(oldcluster.gameObject); }

        Wire[] wires = UnityEngine.Object.FindObjectsOfType<Wire>();

        foreach (Wire wire in wires)
        {
            wire.FindPoints();
            StuffConnector.LinkConnection(wire);
        }

        SnappingPeg.SnapEverywhere();
        MegaMeshManager.AddComponentsEverywhere();

        // and FINALLY, allow circuitry updates again
        BehaviorManager.AllowedToUpdate = true;
    }

    public static void SavePlayerPosition()
    {
        Transform PlayerTransform = FirstPersonController.Instance.transform;
        ES3.Save<Vector3>("PlayerPosition", PlayerTransform.position, PlayersPath + "/player");

        Vector2 PlayerRotation = new Vector2(PlayerTransform.GetChild(0).transform.localEulerAngles.x, PlayerTransform.localEulerAngles.y);
        ES3.Save<Vector2>("PlayerRotation", PlayerRotation, PlayersPath + "/player");

        ES3.Save<bool>("Flying", FirstPersonController.Instance.Flying, PlayersPath + "/player");
    }

    public static void LoadPlayerPosition()
    {
        if (!File.Exists(PlayersPath + "/player")) { return; }

        Transform PlayerTransform = FirstPersonController.Instance.transform;
        Vector2 PlayerRotation = ES3.Load("PlayerRotation", PlayersPath + "/player", Vector2.zero);

        // check if we're falling out of world - if so, reject the new position
        Vector3 PlayerPosition = ES3.Load("PlayerPosition", PlayersPath + "/player", PlayerTransform.position);
        if (PlayerPosition.y < -10)
        {
            return;
        }

        PlayerTransform.position = PlayerPosition;
        FirstPersonController.Instance.m_MouseLook.m_CharacterTargetRot = Quaternion.Euler(0, PlayerRotation.y, 0);
        FirstPersonController.Instance.m_MouseLook.m_CameraTargetRot = Quaternion.Euler(PlayerRotation.x, 0, 0);

        FirstPersonController.Instance.Flying = ES3.Load("Flying", PlayersPath + "/player", false);
    }
}