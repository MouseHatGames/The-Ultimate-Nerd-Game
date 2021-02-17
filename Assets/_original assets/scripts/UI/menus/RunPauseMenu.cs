using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RunPauseMenu : MonoBehaviour {

    public Canvas PauseCanvas;
    public TMPro.TextMeshProUGUI PlayingOn;

	// Use this for initialization
	void Start () {
        ResumeGame();
        PlayingOn.text = "Playing on " + SaveManager.SaveName +". Your game has been saved.";
	}
	
	// Update is called once per frame
	void Update () {
        // dear lord this is a mess. TODO: overhaul UI management code
        if (Input.GetButtonDown("Cancel"))
        {
            SaveManager.SaveAll();

            if (!UIManager.SomeOtherMenuIsOpen)
            {
                UIManager.UnlockMouseAndDisableFirstPersonLooking();
                UIManager.SomeOtherMenuIsOpen = true;
                PauseCanvas.enabled = true;
                Time.timeScale = 0;
            }
            else
            {
                ResumeGame();
            }
        }

        if (Input.GetButtonDown("ToggleGameplayUI")) // not the best place for this to go but whatever I do what I want
        {
            GameplayUICanvas.enabled = !GameplayUICanvas.enabled;
        }
	}

    public Canvas GameplayUICanvas;

    public void ResumeGame()
    {
        PauseCanvas.enabled = false;
        OptionsCanvas.enabled = false;
        AboutCanvas.enabled = false;

        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null); // otherwise you can interact with UI with your keyboard from gameplay, by accident

        UIManager.SomeOtherMenuIsOpen = false;
        Time.timeScale = 1;
        UIManager.LockMouseAndEnableFirstPersonLooking();
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

    public void BackToPause()
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
        Time.timeScale = 1; // so that the main menu runs
        UnityEngine.SceneManagement.SceneManager.LoadScene("main menu");
    }
}
