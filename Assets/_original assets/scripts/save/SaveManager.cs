// this is overwhelmingly complex for me. The code here will not be designed for efficiency, but for working at all, simplicity, and readability.
// also we have great loading screens so it's fine to let people watch them longer :)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class SaveManager {
    public static string SaveName = "error: save name was not set!";

    public static List<SaveThisObject> SaveObjects = new List<SaveThisObject>();

    public static void SaveAll()
    {
        //SaveThisObject[] saveobjects = Object.FindObjectsOfType<SaveThisObject>(); // all active instances of SaveThisObject
        List<SavedObject> TopLevelObjects = new List<SavedObject>();

        foreach(SaveThisObject save in SaveObjects)
        {
            if (save.transform.parent == null) { TopLevelObjects.Add(save.Persistent()); } // all the actual work to save data - including children - is done in save.Persistent
        }

        ES3.Save<List<SavedObject>>("TopLevelObjects", TopLevelObjects, "saves/"+SaveName+".tung");
        SavePlayerPosition();

        ES3.Save<int>("SaveFormatVersion", 1, "saves/" + SaveName + ".tung");

        Debug.Log("saved game");        
    }

    public static void LoadAll()
    {
        BehaviorManager.AllowedToUpdate = false; // don't let circuitry updates fuck us up while we're loading the game

        // remove all existing stuff in the world before loading the new stuff
        SaveThisObject[] saveobjects = Object.FindObjectsOfType<SaveThisObject>(); // all active instances of SaveObject
        foreach(SaveThisObject save in saveobjects) { Object.Destroy(save.gameObject); }

        // also need to clear this dictionary, since it's static
        MegaBoardMeshManager.MegaBoardMeshesOfColor.Clear();

        List<SavedObject> loadobjects = ES3.Load<List<SavedObject>>("TopLevelObjects", "saves/" + SaveName + ".tung");
        foreach(SavedObject save in loadobjects)
        {
            LoadSaveObject(save, null);
        }

        RecalculateAllClustersEverywhereWithDelay();
        LoadPlayerPosition();

        MegaMesh.GenerateNewMegaMesh();
        MegaBoardMeshManager.GenerateAllMegaBoardMeshes();
    }

    public static void LoadSaveObject(SavedObject save, Transform parent)
    {
        GameObject LoadedObject = Object.Instantiate(SaveObjectsList.ObjectTypeToPrefab(save.ObjectType), parent);

        // copy the save stuff into dis goi
        SaveThisObject newsave = LoadedObject.AddComponent<SaveThisObject>();
        newsave.ObjectType = save.ObjectType;
        newsave.CustomDataArray = save.CustomDataArray;
        newsave.LocalPosition = save.LocalPosition;
        newsave.LocalEulerAngles = save.LocalEulerAngles;

        // apply properties
        newsave.LoadSaveData();

        // load children
        foreach(SavedObject child in save.Children)
        {
            LoadSaveObject(child, LoadedObject.transform);
        }
    }

    public static void RecalculateAllClustersEverywhereWithDelay()
    {
        // the best way to do this is with a monobehaviour, so we create a new gameobject then add that behaviour to it
        GameObject penis = new GameObject();
        penis.AddComponent<PauseDelayThenRecalculate>();
    }

    public static void RecalculateAllClustersEverywhere()
    {
        WireCluster[] existingclusters = Object.FindObjectsOfType<WireCluster>();
        foreach (WireCluster oldcluster in existingclusters) { Object.Destroy(oldcluster.gameObject); } // TODO: make clusters not all individual game objects. Would clean up a lot of code, not to mention the heirarchy...

        // much code copied from BoardPlacer.RecalculateClustersOfCurrentBoard
        // create a new cluster for RecalculateCluster to use
        WireCluster cluster = Object.Instantiate(StuffConnecter.ClusterPrefab).GetComponent<WireCluster>();

        // get all the stuff that needs to have its clusters recalculated
        CircuitInput[] inputs = Object.FindObjectsOfType<CircuitInput>();
        Output[] outputs = Object.FindObjectsOfType<Output>();
        InputInputConnection[] IIConnections = Object.FindObjectsOfType<InputInputConnection>();
        InputOutputConnection[] IOConnections = Object.FindObjectsOfType<InputOutputConnection>();

        // clear the connections of each input and output
        // likely this isn't strictly necessary... possibly through pegs will need it though. Idk. Can't hurt.
        foreach (CircuitInput input in inputs)
        {
            input.IIConnections.Clear();
            input.IOConnections.Clear();
        }
        foreach (Output output in outputs)
        {
            output.ClearIOConnections();
        }

        // have the connections find their points
        foreach (InputInputConnection connection in IIConnections)
        {
            connection.FindPoints();
        }
        foreach (InputOutputConnection connection in IOConnections)
        {
            connection.FindPoints();
        }

        // add everything to the new cluster
        foreach (CircuitInput input in inputs)
        {
            cluster.ConnectInput(input);
        }
        foreach (Output output in outputs)
        {
            cluster.ConnectOutput(output);
        }

        // finally, recalculate that cluster
        StuffDeleter.RecalculateCluster(cluster);

        // and FINALLY, allow circuitry updates again
        BehaviorManager.AllowedToUpdate = true;
    }

    public static void SavePlayerPosition()
    {
        Transform PlayerTransform = FirstPersonController.Instance.transform;

        ES3.Save<Vector3>("PlayerPosition", PlayerTransform.position, "saves/" + SaveName + ".tung");

        Vector2 PlayerRotation = new Vector2(PlayerTransform.GetChild(0).transform.localEulerAngles.x, PlayerTransform.localEulerAngles.y);
        ES3.Save<Vector2>("PlayerRotation", PlayerRotation, "saves/" + SaveName + ".tung");
    }

    public static void LoadPlayerPosition()
    {
        Transform PlayerTransform = FirstPersonController.Instance.transform;

        Vector2 PlayerRotation = ES3.Load<Vector2>("PlayerRotation", "saves/" + SaveName + ".tung");
        FirstPersonController.Instance.m_MouseLook.m_CharacterTargetRot = Quaternion.Euler(0, PlayerRotation.y, 0);
        FirstPersonController.Instance.m_MouseLook.m_CameraTargetRot = Quaternion.Euler(PlayerRotation.x, 0, 0);


        // check if we're falling out of world - if so, reject the new position
        Vector3 PlayerPosition = ES3.Load<Vector3>("PlayerPosition", "saves/" + SaveName + ".tung");
        if(PlayerPosition.y < -10)
        {
            return;
        }

        PlayerTransform.position = PlayerPosition;
    }

}
