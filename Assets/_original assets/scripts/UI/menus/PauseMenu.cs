using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CielaSpike;

public class PauseMenu : MonoBehaviour {

    public static PauseMenu Instance;
    private void Awake() { Instance = this; }

    public Canvas PauseCanvas;
    public TMPro.TextMeshProUGUI PlayingOn;

	// Use this for initialization
	void Start ()
    {
        PlayingOn.text = "Playing on " + SaveManager.SaveName +". Your game has been saved.";
	}
	
	// This method is run every frame the pause menu is active
	public void RunPauseMenu () {
        if (Input.GetButtonDown("Cancel"))
        {
            ResumeGame();
        }
	}

    public void PauseGame()
    {
        this.StartCoroutineAsync(SaveManager.SaveAllAsynchronously());
        GameplayUIManager.UIState = UIState.PauseMenuOrSubMenu;
        PauseCanvas.enabled = true;
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        PauseCanvas.enabled = false;
        OptionsCanvas.enabled = false;
        AboutCanvas.enabled = false;

        GameplayUIManager.DeselectAllUIElements();

        Time.timeScale = 1;
        GameplayUIManager.UIState = UIState.None;
    }

    public Canvas OptionsCanvas;
    public Canvas AboutCanvas;

    public void OpenOptions()
    {
        PauseCanvas.enabled = false;
        AboutCanvas.enabled = false;
        OptionsCanvas.enabled = true;
    }

    public void OpenAbout()
    {
        PauseCanvas.enabled = false;
        AboutCanvas.enabled = true;
        OptionsCanvas.enabled = false;
    }

    public void ShowMainMenu()
    {
        PauseCanvas.enabled = true;
        AboutCanvas.enabled = false;
        OptionsCanvas.enabled = false;
    }


    public void QuitToDesktop()
    {
        Application.Quit();
    }

    public void QuitToMainMenu()
    {
        EverythingHider.HideEverything();
        Time.timeScale = 1; // so that the main menu runs
        UnityEngine.SceneManagement.SceneManager.LoadScene("main menu");
    }
}
