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
            if (Input.GetButtonDown("Confirm")) { StartNewGame(); }
        }
    }

    public void StartNewGame()
    {
        // it's a NEW game, so don't let the user pick a name that already exists (or is invalid)!
        SaveManager.SaveName = ValidatedUniqueSaveName(NewGameName.text);

        UnityEngine.SceneManagement.SceneManager.LoadScene("gameplay");
    }

    public static string ValidatedUniqueSaveName(string original)
    {
        string newname = ValidateSaveName(original);
        while (ES3.FileExists("saves/" + newname + ".tung")) // so we don't have duplicates
        {
            newname = newname + "-";
        }
        return newname;
    }

    public static string ValidateSaveName(string name)
    {
        string ValidatedSaveName1 = name.Replace('*', '_');
        string ValidatedSaveName2 = ValidatedSaveName1.Replace('\\', '_');
        string ValidatedSaveName3 = ValidatedSaveName2.Replace(':', '_');
        string ValidatedSaveName4 = ValidatedSaveName3.Replace('<', '_');
        string ValidatedSaveName5 = ValidatedSaveName4.Replace('>', '_');
        string ValidatedSaveName6 = ValidatedSaveName5.Replace('?', '_');
        string ValidatedSaveName7 = ValidatedSaveName6.Replace('/', '_');
        string ValidatedSaveName8 = ValidatedSaveName7.Replace('|', '_');
        string ValidatedSaveName9 = ValidatedSaveName8.Replace('"', '_');

        return ValidatedSaveName9;
    }
}
