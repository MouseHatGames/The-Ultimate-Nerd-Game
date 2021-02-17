using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSShower : MonoBehaviour {

    public Canvas FPSCanvas;
    public TextMeshProUGUI FPStext;
    public static FPSShower Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        FPSCanvas.enabled = true;
    }

    private void OnDisable()
    {
        FPSCanvas.enabled = false;
    }

    // Update is called once per frame
    void Update () {
        int FPS = (int)(1.0f / Time.unscaledDeltaTime);
        FPStext.text = FPS.ToString(); // this produces a lot of garbage...
	}
}
