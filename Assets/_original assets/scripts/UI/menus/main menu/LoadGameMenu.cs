using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Linq; // for sequenceequal stuff

public class LoadGameMenu : MonoBehaviour
{
    public GameObject SavedGamePrefab;
    public RectTransform Parent;

    public UISaveFile SelectedSaveFile;

    public RunMainMenu runmainmenu;

    public static LoadGameMenu Instance;
	void Start ()
    {
        Instance = this;

        GenerateLoadGamesMenu();
        DeleteOldBackupFolders();

        StartCoroutine(ContinuouslyCheckForNewSaves());
	}

    string[] PreviousSaveList;
    string[] PreviousLegacySaveList;
    private IEnumerator ContinuouslyCheckForNewSaves()
    {
        PreviousSaveList = Directory.GetDirectories(Application.persistentDataPath + "/saves");
        PreviousLegacySaveList = Directory.GetFiles(Application.persistentDataPath + "/saves");

        while (true)
        {
            yield return new WaitForSeconds(1);
            string[] NewSaveList = Directory.GetDirectories(Application.persistentDataPath + "/saves");
            string[] NewLegacySaveList = Directory.GetFiles(Application.persistentDataPath + "/saves");
            if (!NewSaveList.SequenceEqual(PreviousSaveList) || !NewLegacySaveList.SequenceEqual(PreviousLegacySaveList))
            {
                PreviousSaveList = NewSaveList;
                PreviousLegacySaveList = NewLegacySaveList;
                GenerateLoadGamesMenu();

                Debug.Log("a change in the saves list was detected and the menu was regenerated");
            }
        }
    }

    private void Update()
    {
        if (RunMainMenu.Instance.LoadGameCanvas.enabled)
        {
            if (Input.GetButtonDown("Cancel")) { RunMainMenu.Instance.ShowMainMenu(); }
            if (Input.GetButtonDown("Confirm")) { Load(); }
            if (Input.GetKeyDown(KeyCode.F5)) { GenerateLoadGamesMenu(); }
            if (Input.GetKeyDown(KeyCode.F2)) { Rename(); }
        }
        if (RunMainMenu.Instance.RenameGameCanvas.enabled)
        {
            if (Input.GetButtonDown("Cancel")) { RunMainMenu.Instance.ShowLoadGame(); }
            if (Input.GetButtonDown("Confirm")) { RunMainMenu.Instance.ShowLoadGame(); SetNewName(); }
        }
        if (RunMainMenu.Instance.DeleteGameCanvas.enabled)
        {
            if (Input.GetButtonDown("Cancel")) { RunMainMenu.Instance.ShowLoadGame(); }
        }
    }

    public void GenerateLoadGamesMenu()
    {
        // delete the existing menu, if it exists. The menu needs to be regenerated when something is renamed or deleted
        foreach(UISaveFile uisave in UISaveFiles)
        {
            Destroy(uisave.gameObject);
        }
        UISaveFiles.Clear();

        int Saves = LoadSavesAndReturnTheNumberOfSaves();
        int LegacySaves = LoadLegacySavesAndReturnTheNumberOfLegacySaves(Saves);

        // set the content field to the correct length for the number of saves
        Parent.sizeDelta = new Vector2(784, 20 + (Saves + LegacySaves) * 110);
    }

    public int LoadSavesAndReturnTheNumberOfSaves()
    {
        string[] subdirectories = Directory.GetDirectories(Application.persistentDataPath + "/saves");
        PreviousSaveList = subdirectories; // so it doesn't automatically regenerate just after we've generated

        // get valid save directories
        List<string> savepaths = new List<string>();
        foreach(string directory in subdirectories)
        {
            if (File.Exists(directory + "/regions/world.tung"))
            {
                savepaths.Add(directory);
            }
        }

        for(int i = 0; i < savepaths.Count; i++)
        {
            GameObject penis = Instantiate(SavedGamePrefab, Parent);
            DirectoryInfo penisinfo = new DirectoryInfo(savepaths[i]);
            penis.name = penisinfo.Name; // get the name of the directory referenced by the path

            // penis.transform.localPosition = new Vector3(0, -10 + i * -110, 0); // old method was to set position here, broken in 2017.3. Details on why in UISaveFile

            UISaveFile vagina = penis.GetComponent<UISaveFile>();
            vagina.FileName = penisinfo.Name;
            vagina.Title.text = penisinfo.Name;

            vagina.LegacySave = false;
            vagina.LegacyWarning.SetActive(false);

            vagina.SetPosition(i); // new method of setting position


            vagina.Info.text =
                (new FileInfo(savepaths[i] + "/regions/world.tung").Length / 1024).ToString() + " kb | " // get file size
                + penisinfo.LastAccessTime.ToString(); // get the local time of the last time the save was modified

            UISaveFiles.Add(vagina);
        }

        Debug.Log(savepaths.Count.ToString() + " regular saves");
        return savepaths.Count;
    }

    public int LoadLegacySavesAndReturnTheNumberOfLegacySaves(int NumberOfRegularSaves)
    {
        string[] FilesInSavesDirectory = ES3.GetFiles("saves");

        // get only .tung files and parse out the extension
        List<string> saves = new List<string>();
        foreach (string file in FilesInSavesDirectory)
        {
            if (file.Substring(file.Length - 5, 5) == ".tung") // if the extension is .tung
            {
                saves.Add(file.Substring(0, file.Length - 5));
            }

        }

        // generate the actual menu
        for (int i = 0; i < saves.Count; i++)
        {
            GameObject penis = Instantiate(SavedGamePrefab, Parent);
            penis.name = saves[i];
            // penis.transform.localPosition = new Vector3(0, -10 + i * -110, 0); // old method was to set position here, broken in 2017.3. Details on why in UISaveFile

            UISaveFile vagina = penis.GetComponent<UISaveFile>();
            vagina.FileName = saves[i];
            vagina.Title.text = saves[i];

            vagina.LegacySave = true;
            vagina.LegacyWarning.SetActive(true);

            vagina.SetPosition(i + NumberOfRegularSaves); // new method of setting position

            // to find the size of the file, we have to load an ES3File instance
            ES3File file = new ES3File("saves/" + saves[i] + ".tung");

            vagina.Info.text =
                (file.Size() / 1024).ToString() + " kb | " // get file size
                + ES3.GetTimestamp("saves/" + saves[i] + ".tung").ToLocalTime() // get the local time of the last time the save was modified
                .ToString();

            UISaveFiles.Add(vagina);
        }

        Debug.Log(saves.Count.ToString() + " legacy saves");
        return saves.Count;
    }

    public List<UISaveFile> UISaveFiles;

    // so they can be grayed out when a legacy save is selected
    public UnityEngine.UI.Button LoadButton;
    public UnityEngine.UI.Button RenameButton;
    public UnityEngine.UI.Button DuplicateButton;
    public UnityEngine.UI.Button DeleteButton;

    Color32 deselectedcolor = new Color32(212, 212, 212, 255);
    Color32 selectedcolor = new Color32(62, 159, 255, 255);
    public void ChangeSelectedSave(UISaveFile selected)
    {
        // double click functionality
        if (selected == SelectedSaveFile) { Load(); }

        SelectedSaveFile = selected;

        // change its color, change all the other colors back to default
        foreach(UISaveFile save in UISaveFiles)
        {
            save.image.color = deselectedcolor;
        }

        selected.image.color = selectedcolor;

        LoadButton.interactable = true;
        RenameButton.interactable = !selected.LegacySave;
        DuplicateButton.interactable = !selected.LegacySave;
        DeleteButton.interactable = !selected.LegacySave;
    }

    public void OpenSavesFolder()
    {
        System.Diagnostics.Process.Start(@Application.persistentDataPath + "/saves");
    }

    public void Load()
    {
        if (SelectedSaveFile == null) { return; }

        SaveManager.SaveName = SelectedSaveFile.FileName;
        GameplaySaving.LoadLegacySave = SelectedSaveFile.LegacySave;
        UnityEngine.SceneManagement.SceneManager.LoadScene("gameplay");

        EverythingHider.HideEverything();
    }

    public TMPro.TMP_InputField RenameInput;
    // when the button is pressed on the Load Game canvas. The canvas transition is handled by the buttons themselves
    public void Rename()
    {
        if (SelectedSaveFile == null || SelectedSaveFile.LegacySave) { return; }

        runmainmenu.ShowRenameGame();
        RenameInput.text = SelectedSaveFile.FileName;
        RenameInput.ActivateInputField();
    }

    // when the button is pressed to rename the thing
    public void SetNewName()
    {
        string NewName = FileUtilities.ValidatedUniqueSaveName(RenameInput.text);
        string OldName = SelectedSaveFile.FileName;

        Directory.Move(Application.persistentDataPath + "/saves/" + OldName, Application.persistentDataPath + "/saves/" + NewName);

        GenerateLoadGamesMenu();

        if(Directory.Exists(Application.persistentDataPath + "/backups/" + OldName))
        {
            Directory.Move(Application.persistentDataPath + "/backups/" + OldName, Application.persistentDataPath + "/backups/" + NewName);
        }
    }

    public void DuplicateGame()
    {
        if (SelectedSaveFile == null || SelectedSaveFile.LegacySave) { return; }

        FileUtilities.DirectoryCopy(Application.persistentDataPath + "/saves/" + SelectedSaveFile.FileName,
            Application.persistentDataPath + "/saves/" + FileUtilities.ValidatedUniqueSaveName(SelectedSaveFile.FileName), true);

        GenerateLoadGamesMenu();
    }

    public void DeleteGame()
    {
        if (SelectedSaveFile == null || SelectedSaveFile.LegacySave) { return; }

        runmainmenu.ShowDeleteGame();
    }

    // this is when the yes, delete! button is pressed in the delete canvas. Canvas transition is all handled by buttons
    public void ConfirmDelete()
    {
        string DeletedFile = SelectedSaveFile.FileName;
        Directory.Delete(Application.persistentDataPath + "/saves/" + DeletedFile, true);

        ES3.Save<bool>("FileDeleted", true, "backups/" + DeletedFile + "info.txt");
        ES3.Save<DateTime>("DeletedOn", DateTime.Now, "backups/" + DeletedFile + "info.txt");

        GenerateLoadGamesMenu();
    }

    void DeleteOldBackupFolders()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/backups/")) { return; }

        string[] BackupFolders = Directory.GetDirectories(Application.persistentDataPath + "/backups/");
        foreach(string BackupFolder in BackupFolders)
        {
            if(ES3.Load("FileDeleted", "/backups/" + BackupFolder + "/info.txt", false))
            {
                DateTime deletedon = ES3.Load<DateTime>("DeletedOn", "saves/backups/" + BackupFolder + "/info.txt");
                TimeSpan TimeSinceDeletion = DateTime.Now - deletedon;
                if(TimeSinceDeletion.Days > Settings.Get("DaysToKeepBackupsOfDeletedSaves", 10))
                {
                    ES3.DeleteDirectory("saves/backups/" + BackupFolder);
                    Directory.Delete(Application.persistentDataPath + "/backups/" + BackupFolder);
                    Debug.Log("deleted old backups for " + BackupFolder);
                }
            }
        }

    }
}