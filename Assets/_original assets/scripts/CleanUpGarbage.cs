using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanUpGarbage : MonoBehaviour {

    private void Awake()
    {
        float CleanGarbageInterval = Settings.Get("GarbageCleanupInterval", 40f);
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
