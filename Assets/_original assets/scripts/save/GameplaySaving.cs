using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using CielaSpike;

public class GameplaySaving : MonoBehaviour
{
    public static bool LoadLegacySave;

//#if UNITY_EDITOR
//    public string LoadThisSave;
//    private void Awake()
//    {
//        SaveManager.SaveName = LoadThisSave;
//    }
//#endif

    void Start () // important that this is start and not awake - lots of important references are set in Awake from other scripts
    {
        if (LoadLegacySave)
        {
            if (!Directory.Exists(Application.persistentDataPath + "/backups/_____pre-0.2/")) { Directory.CreateDirectory(Application.persistentDataPath + "/backups/_____pre-0.2/"); }

            if (!File.Exists(Application.persistentDataPath + "/backups/_____pre-0.2/" + SaveManager.SaveName + ".tung"))
            {
                File.Copy(Application.persistentDataPath + "/saves/" + SaveManager.SaveName + ".tung", Application.persistentDataPath + "/backups/_____pre-0.2/" + SaveManager.SaveName + ".tung");
            }

            LegacySaveLoader.LoadLegacySave();

            {
                File.Delete(Application.persistentDataPath + "/saves/" + SaveManager.SaveName + ".tung"); Debug.Log("file already exists in pre-0.2 backups - deleting permanently");
            }

            SaveManager.SaveName = FileUtilities.ValidatedUniqueSaveName(SaveManager.SaveName); // just in case there is a legacy save with the same name as a new save. We don't want to overwrite the new save!
            PauseMenu.Instance.PlayingOn.text = "Playing on " + SaveManager.SaveName + ". Your game has been saved."; // quick and dirty fix for if the save name changed in the previous line
        }
        else
        {
            if (!Directory.Exists(Application.persistentDataPath + "/saves/" + SaveManager.SaveName)) { SaveManager.SaveAllSynchronously(); }

            SaveManager.LoadAll();
        }

        StartCoroutine(AutoSave());
        this.StartCoroutineAsync(Backup());
	}

    private void OnApplicationQuit()
    {
        StackBoardMenu.Instance.DestroyStuffIfAppropriate();

        if (Time.timeScale == 0) { return; } // no need to save if the game is paused; we already saved when we paused the game
        SaveManager.SaveAllSynchronously();
    }

    private IEnumerator AutoSave()
    {
        float AutosaveInterval = Settings.Get("AutosaveInterval", 120f);

        while (true)
        {
            yield return new WaitForSeconds(AutosaveInterval);
            this.StartCoroutineAsync(SaveManager.SaveAllAsynchronously());
        }
    }

    private IEnumerator Backup()
    {
        yield return Ninja.JumpToUnity;

        float BackupInterval = Settings.Get("BackupInterval", 300f);
        int MaxBackups = Settings.Get("MaxBackupsPerSave", 10);

        if(!Directory.Exists(Application.persistentDataPath + "/backups"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/backups");
        }
        if(!Directory.Exists(Application.persistentDataPath + "/backups/" + SaveManager.SaveName))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/backups/" + SaveManager.SaveName);
        }

        string BackupsDirectoryForThisSave = Application.persistentDataPath + "/backups/" + SaveManager.SaveName;
        string SavePath = SaveManager.SavePath;

        yield return Ninja.JumpBack;

        while (true)
        {
            yield return new WaitForSeconds(BackupInterval);

            // copy the save to create a backup
            FileUtilities.DirectoryCopy(SavePath, BackupsDirectoryForThisSave + "/" + FileUtilities.CurrentTimestamp, true);

            // delete excess backups
            string[] Backups = Directory.GetDirectories(BackupsDirectoryForThisSave);
            System.Array.Sort(Backups); // get it in alphabetical order, so oldest backups are at the start of the array
            int ExcessBackups = Backups.Length - MaxBackups;
            for (int i = ExcessBackups - 2; i >= 0; i--)
            {
                Directory.Delete(Backups[i], true);
                Debug.Log("deleted old backup");
            }

            Debug.Log("backed up game");
        }
    }

    // todo: replace with proper loading screen
    private IEnumerator HideWorldForABit()
    {
        EverythingHider.HideEverything();

        yield return new WaitForEndOfFrame();

        EverythingHider.UnHideEverything();
    }

    private void Awake()
    {
        StartCoroutine(HideWorldForABit());
    }
}