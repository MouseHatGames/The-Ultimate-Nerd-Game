using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPSEditor : MonoBehaviour
{
    public TMPro.TMP_InputField InputBoi;

    public static TPSEditor Instance;
    private void Awake()
    {
        Instance = this;
        InputBoi.text = Settings.Get("CircuitUpdatesPerSecond", 100f).ToString();
    }

    public void SetUpdateRate()
    {
        float persecond = float.Parse(InputBoi.text);
        BehaviorManager.SetUpdateRate(persecond);
    }

    public void Step()
    {
        BehaviorManager.Step();
    }
}
