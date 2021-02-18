using UnityEngine;

// todo: replace with proper loading screen
public class EverythingHider : MonoBehaviour 
{
    private static Canvas HideEverythingCanvas;
    private void Awake()
    {
        HideEverythingCanvas = GetComponent<Canvas>();
    }

    public static void HideEverything()
    {
        HideEverythingCanvas.enabled = true;
    }

    public static void UnHideEverything()
    {
        HideEverythingCanvas.enabled = false;
    }
}