using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplaySaving : MonoBehaviour {

    // Use this for initialization
    void Start () {
        // make sure the save name contains valid characters only
        SaveManager.SaveName = NewGame.ValidateSaveName(SaveManager.SaveName);

        if (ES3.FileExists("saves/" + SaveManager.SaveName + ".tung"))
        {
            SaveManager.LoadAll();
        }
        else
        {
            // shitty hack. There's a bug where new games don't work properly because some essential functions are only found
            // in SaveManager.LoadAll. Hence the following.
            SaveManager.SaveAll();
            SaveManager.LoadAll();
        }

        // save the values to settings.txt if they do not exist
        if (!ES3.KeyExists("AutosaveInterval", "settings.txt")) { ES3.Save<float>("AutosaveInterval", 120f, "settings.txt"); }
        if (!ES3.KeyExists("BackupInterval", "settings.txt")) { ES3.Save<float>("BackupInterval", 300f, "settings.txt"); }
        if (!ES3.KeyExists("MaxBackupsPerSave", "settings.txt")) { ES3.Save<int>("MaxBackupsPerSave", 10, "settings.txt"); }

        StartCoroutine(AutoSave());
        StartCoroutine(Backup());
	}

    private void OnApplicationQuit()
    {
        SaveManager.SaveAll();
    }

    public IEnumerator AutoSave()
    {
        float AutosaveInterval = ES3.Load<float>("AutosaveInterval", "settings.txt", 10); ;

        while (true)
        {
            yield return new WaitForSeconds(AutosaveInterval);
            SaveManager.SaveAll();
        }
    }

    public IEnumerator Backup()
    {
        float BackupInterval = ES3.Load<float>("BackupInterval", "settings.txt", 200f);
        int MaxBackups = ES3.Load<int>("MaxBackupsPerSave", "settings.txt", 10);

        // because CopyFile won't automatically create directories but Save will
        if(!ES3.DirectoryExists("saves/backups/" + SaveManager.SaveName)) {
            ES3.Save<string>("info", "Contains periodic backups of " + SaveManager.SaveName + "."
            + "You can adjust the frequency and number of backups kept by editing settings.txt, as well as how long to keep backup folders after the file they're backing up is deleted.",
            "saves/backups/" + SaveManager.SaveName + "/info.txt");
        }

        while (true)
        {
            yield return new WaitForSeconds(BackupInterval);

            // copy the save to create a backup
            ES3.CopyFile("saves/" + SaveManager.SaveName + ".tung", "saves/backups/" + SaveManager.SaveName + "/" + NewGame.ValidateSaveName(System.DateTime.Now.ToLocalTime().ToString("u")) + ".tung");

            // delete excess backups
            string[] Backups = ES3.GetFiles("saves/backups/" + SaveManager.SaveName);
            System.Array.Sort(Backups); // get it in alphabetical order, so oldest backups are at the start of the array
            int ExcessBackups = Backups.Length - MaxBackups;
            for (int i = ExcessBackups - 2; i >= 0; i--)
            {
                ES3.DeleteFile("saves/backups/" + SaveManager.SaveName + "/" + Backups[i]);
            }

            Debug.Log("backed up game");
        }
    }
}
