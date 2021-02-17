using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RunMainMenu : MonoBehaviour {

    public static RunMainMenu Instance;

    public UnityEngine.Playables.PlayableDirector CameraMovementDirector;

	// Use this for initialization
	void Start () {
        GameObject.Find("CM vcam1").transform.localEulerAngles = new Vector3(90, 0, 0); // for some reason, you cannot set virtual camera rotation in the editor. This is my solution
        ShowMainMenu();
        Instance = this;
        DoBoards();

        ES3.Save<string>("LastLoadedVersion", "0.1", "settings.txt"); // in case it's needed for future compatibility

        // stuff to enable/disable the main menu pan as some people get motion sick from it
        if (!ES3.KeyExists("EnableMainMenuCameraPan", "settings.txt"))
        {
            ES3.Save<bool>("EnableMainMenuCameraPan", true, "settings.txt"); // it is VERY IMPORTANT that the number here be cast as a float!
        }
        CameraMovementDirector.enabled = ES3.Load<bool>("EnableMainMenuCameraPan", "settings.txt", true);
    }

    private void Update()
    {
        if (AboutCanvas.enabled || OptionsCanvas.enabled) // other menus handled in their own scripts. These ones can't since they're prefabs, duplicated in the gameplay scene
        {
            if (Input.GetButtonDown("Cancel")) { ShowMainMenu(); }
        }
    }

    public Canvas MainMenuCanvas;
    public Canvas NewGameCanvas;
    public Canvas LoadGameCanvas;
    public Canvas RenameGameCanvas;
    public Canvas DeleteGameCanvas;
    public Canvas OptionsCanvas;
    public Canvas AboutCanvas;

    private void DisableAllCanvases()
    {
        MainMenuCanvas.enabled = false;
        NewGameCanvas.enabled = false;
        LoadGameCanvas.enabled = false;
        RenameGameCanvas.enabled = false;
        DeleteGameCanvas.enabled = false;
        OptionsCanvas.enabled = false;
        AboutCanvas.enabled = false;
    }

    public void ShowMainMenu()
    {
        DisableAllCanvases();
        MainMenuCanvas.enabled = true;
    }

    public void ShowNewGame()
    {
        DisableAllCanvases();
        NewGameCanvas.enabled = true;
    }

    public void ShowLoadGame()
    {
        DisableAllCanvases();
        LoadGameCanvas.enabled = true;
    }

    public void ShowRenameGame()
    {
        DisableAllCanvases();
        RenameGameCanvas.enabled = true;
    }

    public void ShowDeleteGame()
    {
        DisableAllCanvases();
        DeleteGameCanvas.enabled = true;
    }

    public void ShowOptions()
    {
        DisableAllCanvases();
        OptionsCanvas.enabled = true;
    }

    public void ShowAbout()
    {
        DisableAllCanvases();
        AboutCanvas.enabled = true;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    // some shitty hacks to fix boards on the main menu
    public List<CircuitBoard> Boards = new List<CircuitBoard>();
    public List<Color> BoardColor = new List<Color>();
    public Material BoardMaterial;
    [NaughtyAttributes.Button]
    public void DoBoards()
    {
        foreach(CircuitBoard board in Boards)
        {
            board.CreateCuboid();
            board.Renderer.material = BoardMaterial;
            board.Renderer.material.color = board.BoardColor;
        }
        for(int i = 0; i < Boards.Count; i++)
        {
            Boards[i].CreateCuboid();
            Boards[i].Renderer.material = BoardMaterial;
            Boards[i].Renderer.material.color = BoardColor[i];
        }
    }
    public GameObject SharedOutputMeshPrefab;
    public GameObject ClusterPrefab;
    private void Awake()
    {
        Output.SharedOutputMeshPrefab = SharedOutputMeshPrefab;
        StuffConnecter.ClusterPrefab = ClusterPrefab;
        SaveManager.RecalculateAllClustersEverywhere();
    }
}
