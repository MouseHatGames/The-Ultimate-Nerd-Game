using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EpilepsyWarning : MonoBehaviour 
{
    private void Start()
    {
        if (Settings.Get("SeenEpilepsyWarning", false))
        {
            GetComponent<Canvas>().enabled = false;
        }
        else
        {
            RunMainMenu.Instance.HideAll();
            GetComponent<Canvas>().enabled = true;
            Time.timeScale = 0;
            BehaviorManager.AllowedToUpdate = false;
        }
    }

    public void GotIt()
    {
        Settings.Save("SeenEpilepsyWarning", true);
        GetComponent<Canvas>().enabled = false;
        RunMainMenu.Instance.ShowMainMenu();
        RunMainMenu.Instance.MainMenuMusic.Play();
        Time.timeScale = 1;
        BehaviorManager.AllowedToUpdate = true;
    }

    public void InstructionsLink()
    {
        Application.OpenURL("https://www.reddit.com/r/TheUltimateNerdGame/comments/7otiln/a_complete_list_of_things_you_can_edit_in/");
    }
}