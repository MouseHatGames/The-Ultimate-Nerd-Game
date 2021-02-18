using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseDelayThenRecalculate : MonoBehaviour
{
	// Use this for initialization
	void Awake () {
        StartCoroutine(Delaycoroutine());
	}

	
    IEnumerator Delaycoroutine()
    {
        yield return new WaitForEndOfFrame();
        SaveManager.RecalculateAllClustersEverywhere();
        Destroy(gameObject);
    }
}