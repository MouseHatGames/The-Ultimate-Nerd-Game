using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using SavedObjects;

public class RunMainMenu : MonoBehaviour
{
    public static RunMainMenu Instance;

    private void Update()
    {
        if (AboutCanvas.enabled || OptionsCanvas.enabled) // other menus handled in their own scripts. These ones can't since they're prefabs, duplicated in the gameplay scene
        {
            if (Input.GetButtonDown("Cancel")) { ShowMainMenu(); }
        }

        RunKonamiCodeDetection();
    }

    public Canvas MainMenuCanvas;
    public Canvas NewGameCanvas;
    public Canvas LoadGameCanvas;
    public Canvas RenameGameCanvas;
    public Canvas DeleteGameCanvas;
    public Canvas OptionsCanvas;
    public Canvas AboutCanvas;

    public AudioSource MainMenuMusic;

    public void HideAll()
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
        HideAll();
        MainMenuCanvas.enabled = true;
    }

    public void ShowNewGame()
    {
        HideAll();
        NewGameCanvas.enabled = true;
    }

    public void ShowLoadGame()
    {
        HideAll();
        LoadGameCanvas.enabled = true;
    }

    public void ShowRenameGame()
    {
        HideAll();
        RenameGameCanvas.enabled = true;
    }

    public void ShowDeleteGame()
    {
        HideAll();
        DeleteGameCanvas.enabled = true;
    }

    public void ShowOptions()
    {
        HideAll();
        OptionsCanvas.enabled = true;
    }

    public void ShowAbout()
    {
        HideAll();
        AboutCanvas.enabled = true;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void Awake()
    {
        GameplayUIManager.UIState = UIState.MainMenu;
        Instance = this;

        MegaMeshManager.ClearReferences();
        BehaviorManager.ClearAllLists();

        ShowMainMenu();

        string CurrentVersion = "0.2.0";
        string LastLoadedVersion = Settings.Get("LastLoadedVersion", CurrentVersion);
        if (LastLoadedVersion != CurrentVersion) { ES3.DeleteFile("settings.txt"); } // some settings are obsolete and must be reset

        EverythingHider.UnHideEverything();
    }
    

    private void Start()
    {
        // this should go somewhere better...
        Application.runInBackground = !Settings.Get("PauseOnDefocus", false);
    }


    // konami code easter egg stuff

    [SerializeField] private Image CompanyLogo;
    [SerializeField] private Sprite OldCompanyLogo;
    [SerializeField] private Image GameLogo;
    [SerializeField] private Sprite OldGameLogo;

    private int ID = 0;
    private void RunKonamiCodeDetection()
    {
        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(KonamiCode[ID]))
            {
                ID++;
                if(ID == KonamiCode.Length)
                {
                    CompanyLogo.sprite = OldCompanyLogo;
                    GameLogo.sprite = OldGameLogo;
                    GameLogo.preserveAspect = true;
                    GameLogo.GetComponent<RectTransform>().sizeDelta = new Vector2(1200, 1200);
                    GameLogo.GetComponent<RectTransform>().anchoredPosition = new Vector2(625, 375);
                }
            }
            else
            {
                ID = 0;
            }
        }
    }

    private KeyCode[] KonamiCode =
    {
        KeyCode.UpArrow,
        KeyCode.UpArrow,
        KeyCode.DownArrow,
        KeyCode.DownArrow,
        KeyCode.LeftArrow,
        KeyCode.RightArrow,
        KeyCode.LeftArrow,
        KeyCode.RightArrow,
        KeyCode.B,
        KeyCode.A
    };
}