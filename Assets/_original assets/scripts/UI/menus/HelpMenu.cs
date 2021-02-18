// GameplayUIManager calls the static methods when the UI changes.

using UnityEngine;
using NaughtyAttributes;

public class HelpMenu : MonoBehaviour
{
    public Canvas HelpCanvas;

    public TMPro.TextMeshProUGUI Content;

    private static HelpMenu Instance;
    private void Awake() { Instance = this; }

    // Use this for initialization
    void Start () {
        HelpCanvas.enabled = Settings.Get("ShowHelp", true);
        SetContentText(Default);
	}

    public static void ToggleHelp()
    {
        if (Instance == null) { return; } // as on the main menu
        Instance.HelpCanvas.enabled = !Instance.HelpCanvas.enabled;
        Settings.Save("ShowHelp", Instance.HelpCanvas.enabled);
    }

    public static void ShowAppropriate(UIState state)
    {
        if (Instance == null) { return; }

        switch (state)
        {
            case UIState.None:
                SetContentText(Instance.Default);
                break;

            case UIState.BoardMenu:
                SetContentText(Instance.BoardMenu);
                break;

            case UIState.NewBoardMenu:
                SetContentText(Instance.NewBoardMenu);
                break;

            case UIState.PaintBoardMenu:
                SetContentText(Instance.PaintBoardMenu);
                break;

            case UIState.TextEditMenu:
                SetContentText(Instance.TextEditMenu);
                break;

            case UIState.BoardBeingPlaced:
                SetContentText(Instance.BoardBeingPlaced);
                break;

            case UIState.ChooseDisplayColor:
                SetContentText(Instance.ChooseDisplayColor);
                break;

            case UIState.NoisemakerMenu:
                SetContentText(Instance.NoisemakerMenu);
                break;

            case UIState.StackBoardMenu:
                SetContentText(Instance.StackBoardMenu);
                break;
        }
    }

    private static void SetContentText(string text)
    {
        Instance.Content.text = text;
    }

    [ResizableTextArea] [SerializeField] private string Default;

    [ResizableTextArea] [SerializeField] private string BoardMenu;

    [ResizableTextArea] [SerializeField] private string BoardBeingPlaced;

    [ResizableTextArea] [SerializeField] private string PaintBoardMenu;

    [ResizableTextArea] [SerializeField] private string NewBoardMenu;

    [ResizableTextArea] [SerializeField] private string TextEditMenu;

    [ResizableTextArea] [SerializeField] private string ChooseDisplayColor;

    [ResizableTextArea] [SerializeField] private string NoisemakerMenu;

    [ResizableTextArea] [SerializeField] private string StackBoardMenu;

    [ResizableTextArea] [SerializeField] private string StateNotFound;
}