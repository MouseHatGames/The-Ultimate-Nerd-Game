using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseDelayThenRecalculate : MonoBehaviour {

	// Use this for initialization
	void Awake () {
        Time.fixedDeltaTime = 10; // for some reason this is needed to prevent errors on load
        StartCoroutine(Delaycoroutine());
	}

	
    IEnumerator Delaycoroutine()
    {
        yield return new WaitForEndOfFrame();
        SaveManager.RecalculateAllClustersEverywhere();
        Time.fixedDeltaTime = 0.01f;
        Destroy(gameObject);
    }
}
