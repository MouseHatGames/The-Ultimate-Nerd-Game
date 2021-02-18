using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGame : MonoBehaviour {

    public TMPro.TMP_InputField NewGameName;

    private void Update()
    {
        if (RunMainMenu.Instance.NewGameCanvas.enabled)
        {
            if (Input.GetButtonDown("Cancel")) { RunMainMenu.Instance.ShowMainMenu(); }
            else if (Input.GetButtonDown("Confirm")) { StartNewGame(); }
        }
    }

    public void StartNewGame()
    {
        // it's a NEW game, so don't let the user pick a name that already exists (or is invalid)!
        SaveManager.SaveName = FileUtilities.ValidatedUniqueSaveName(NewGameName.text);
        GameplaySaving.LoadLegacySave = false;

        UnityEngine.SceneManagement.SceneManager.LoadScene("gameplay");
    }
}
