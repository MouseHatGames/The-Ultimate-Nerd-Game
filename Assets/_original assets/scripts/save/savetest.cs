using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class savetest : MonoBehaviour {

    [Button]
    public void RunSaveAll()
    {
         SaveManager.SaveAll();
    }

    [Button]
    public void RunLoadAll()
    {
        SaveManager.LoadAll();
    }

    [Button]
    public void RunRecalculateAllClusters()
    {
        SaveManager.RecalculateAllClustersEverywhere();
    }
}
