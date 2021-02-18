using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using SavedObjects;

public class DemoSaveLoader : MonoBehaviour {

	// Use this for initialization
	void Awake ()
    {
        LoadDemo();
	}

    void LoadDemo()
    {
        if (Directory.Exists(Application.persistentDataPath + "/saves/demo")) { return; }

        TextAsset DemoWorldInfo = Resources.Load<TextAsset>("demo/worldinfo");
        TextAsset DemoWorldPlayer = Resources.Load<TextAsset>("demo/players/player");
        TextAsset DemoWorldRegion = Resources.Load<TextAsset>("demo/regions/world");

        Directory.CreateDirectory(Application.persistentDataPath + "/saves/demo");
        Directory.CreateDirectory(Application.persistentDataPath + "/saves/demo/players");
        Directory.CreateDirectory(Application.persistentDataPath + "/saves/demo/regions");

        FileUtilities.SaveBytesToFile(Application.persistentDataPath + "/saves/demo/", "worldinfo.txt", DemoWorldInfo.bytes);
        FileUtilities.SaveBytesToFile(Application.persistentDataPath + "/saves/demo/players", "player", DemoWorldPlayer.bytes);
        FileUtilities.SaveBytesToFile(Application.persistentDataPath + "/saves/demo/regions", "world.tung", DemoWorldRegion.bytes);

        Resources.UnloadAsset(DemoWorldInfo);
        Resources.UnloadAsset(DemoWorldPlayer);
        Resources.UnloadAsset(DemoWorldRegion);

        Debug.Log("demo not found in saves folder, adding it");
    }
}