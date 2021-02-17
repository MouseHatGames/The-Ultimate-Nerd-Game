using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanUpGarbage : MonoBehaviour {

    private void Awake()
    {
        // set these values if they don't exist in files
        // TODO: unify this code that's repeated a hundred fucking times god dammit
        if (!ES3.KeyExists("GarbageCleanupInterval", "settings.txt"))
        {
            ES3.Save<float>("GarbageCleanupInterval", (float)40, "settings.txt"); // it is VERY IMPORTANT that the number here be cast as a float!
        }
        float CleanGarbageInterval = ES3.Load<float>("GarbageCleanupInterval", "settings.txt", 40);
        StartCoroutine(CleanGarbageRoutine(CleanGarbageInterval));
    }

    private IEnumerator CleanGarbageRoutine(float interval)
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);
            Resources.UnloadUnusedAssets();
            Debug.Log("cleaned up garbage");
        }
    }
}
