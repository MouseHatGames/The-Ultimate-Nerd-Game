using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckOutDemoWorld : MonoBehaviour
{
    private void Start()
    {
        SetProperVisibility();
    }

    private void SetProperVisibility()
    {
        gameObject.SetActive(!Settings.Get("SeenDemoWorld", false));
    }

    // triggered by the button to open the load game menu
    public void OnOpenLoadGameMenu()
    {
        Settings.Save("SeenDemoWorld", true);
        SetProperVisibility();
    }
}