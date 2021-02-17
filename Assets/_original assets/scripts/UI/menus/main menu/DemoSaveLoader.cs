using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoSaveLoader : MonoBehaviour {

	// Use this for initialization
	void Awake () {
        LoadDemo();
	}
	
	void LoadDemo()
    {
        if (ES3.FileExists("saves/demo.tung"))
        {
            return;
        }

        TextAsset DemoSave = Resources.Load<TextAsset>("demo");
        ES3.SaveRaw(DemoSave.bytes, "saves/demo.tung");

        Resources.UnloadAsset(DemoSave);
    }
}
