// just for identification purposes

using UnityEngine;

public class GameplayUICanvas : MonoBehaviour
{
    private static GameplayUICanvas Instance;
    private void Awake()
    {
        Instance = this;

        crosshair.color = Settings.Get("CrosshairColor", Color.black);
        crosshair.rectTransform.sizeDelta = Settings.Get("CrosshairSize", new Vector2(6, 6));
    }

    [SerializeField] public Canvas CrosshairCanvas;
    [SerializeField] public UnityEngine.UI.Image crosshair;
    [SerializeField] public Canvas FPSCanvas;

    // this is a bad way to do it. Should just disable the UI camera.
    public static void ToggleVisibility()
    {
        if (Instance == null) { return; }
        bool enabled = !Instance.CrosshairCanvas.enabled;

        Instance.CrosshairCanvas.enabled = enabled;

        if (enabled && OptionsMenu.Instance.ShowFPSToggle.isOn)
            Instance.FPSCanvas.enabled = true;
        else { Instance.FPSCanvas.enabled = false; }
    }
}