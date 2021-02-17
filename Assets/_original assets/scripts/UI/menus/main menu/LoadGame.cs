using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LoadGame : MonoBehaviour {

    public GameObject LoadGamePrefab;
    public RectTransform Parent;

    public UISaveFile SelectedSaveFile;

    public RunMainMenu runmainmenu;

	// Use this for initialization
	void Start () {
        UISaveFile.loadgame = this;

        GenerateLoadGamesMenu();
        DeleteOldBackupFolders();
	}

    //private IEnumerator ContinuouslyCheckForNewSaves()
    //{
    //    string[] PreviousSaveList = ES3.GetFiles("saves/");

    //    while (true)
    //    {
    //        yield return new WaitForSeconds(5);
    //        string[] NewSaveList = ES3.GetFiles("saves/");
    //        if(NewSaveList != PreviousSaveList) // for some reason, this check will always return true. Abandoning this method for now
    //        {
    //            PreviousSaveList = NewSaveList;
    //            GenerateLoadGamesMenu();
    //            Debug.Log("a change in the saves list was detected and the menu was regenerated");
    //        }
    //    }
    //}

    private void Update()
    {
        if (RunMainMenu.Instance.LoadGameCanvas.enabled)
        {
            if (Input.GetButtonDown("Cancel")) { RunMainMenu.Instance.ShowMainMenu(); }
            if (Input.GetButtonDown("Confirm")) { Load(); }
            if (Input.GetKeyDown(KeyCode.F5)) { GenerateLoadGamesMenu(); }
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


        string[] FilesInSavesDirectory = ES3.GetFiles("saves");

        // get only .tung files and parse out the extension
        List<string> saves = new List<string>();
        foreach(string file in FilesInSavesDirectory)
        {
            if (file.Substring(file.Length - 5, 5) == ".tung") // if the extension is .tung
            {
                saves.Add(file.Substring(0, file.Length - 5));
            }

        }

        // generate the actual menu
        for(int i = 0; i < saves.Count; i++)
        {
            GameObject penis = Instantiate(LoadGamePrefab, Parent);
            penis.name = saves[i];          
           // penis.transform.localPosition = new Vector3(0, -10 + i * -110, 0); // old method was to set position here, broken in 2017.3. Details on why in UISaveFile

            UISaveFile vagina = penis.GetComponent<UISaveFile>();
            vagina.FileName = saves[i];
            vagina.Title.text = saves[i];
            vagina.SetPosition(i); // new method of setting position

            // to find the size of the file, we have to load an ES3File instance
            ES3File file = new ES3File("saves/" + saves[i] + ".tung");

            vagina.Info.text =
                (file.Size() / 1024).ToString() + " kb | " // get file size
                + ES3.GetTimestamp("saves/" + saves[i] + ".tung").ToLocalTime() // get the local time of the last time the save was modified
                .ToString(); //.ToString("u"); // the "u" argument is to get a universally sortable date time string, which I prefer personally. https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings

            //vagina.Info.text = vagina.Info.text.Substring(0, vagina.Info.text.Length - 1); // the datetime formatter puts an annoying Z at the end. This gets rid of it

            UISaveFiles.Add(vagina);
        }

        // set the content field to the correct length for the number of saves
        Parent.sizeDelta = new Vector2(784, 10 + saves.Count * 110);
    }

    public List<UISaveFile> UISaveFiles;

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
    }

    public void OpenSavesFolder()
    {
        System.Diagnostics.Process.Start(@Application.persistentDataPath + "/saves");
    }

    public void Load()
    {
        if (SelectedSaveFile == null) { return; }

        SaveManager.SaveName = SelectedSaveFile.FileName;
        UnityEngine.SceneManagement.SceneManager.LoadScene("gameplay");
    }

    public TMPro.TMP_InputField RenameInput;
    // when the button is pressed on the Load Game canvas. The canvas transition is handled by the buttons themselves
    public void Rename()
    {
        if (SelectedSaveFile == null) { return; }

        runmainmenu.ShowRenameGame();
        RenameInput.text = SelectedSaveFile.FileName;
        RenameInput.ActivateInputField();
    }

    // when the button is pressed to rename the thing
    public void SetNewName()
    {
        string NewName = NewGame.ValidatedUniqueSaveName(RenameInput.text);
        string OldName = SelectedSaveFile.FileName;
        ES3.RenameFile("saves/" + OldName + ".tung", "saves/" + NewName + ".tung");

        GenerateLoadGamesMenu();

        if (!ES3.DirectoryExists("saves/backups/" + OldName)) { return; }
        // transfer the backups
        // because CopyFile won't create directories but Save will
        ES3.Save<string>("info", "Contains periodic backups of " + NewName + "."
            + "You can adjust the frequency and number of backups kept by editing settings.txt, as well as how long to keep backup folders after the file they're backing up is deleted.",
            "saves/backups/" + NewName + "/info.txt");

        string[] backups = ES3.GetFiles("saves/backups/" + OldName);
        foreach(string backup in backups)
        {
            if(backup == "info.txt") { break; } // as this was already created when the file was renamed
            ES3.CopyFile("saves/backups/" + OldName + "/" + backup, "saves/backups/" + NewName + "/" + backup);
        }
        ES3.DeleteDirectory("saves/backups/" + OldName);
    }

    public void DuplicateGame()
    {
        ES3.CopyFile("saves/" + SelectedSaveFile.FileName + ".tung", "saves/" + NewGame.ValidatedUniqueSaveName(SelectedSaveFile.FileName) + ".tung");
        GenerateLoadGamesMenu();
    }

    public void DeleteGame()
    {
        if (SelectedSaveFile == null) { return; }
        runmainmenu.ShowDeleteGame();
    }

    // this is when the yes, delete! button is pressed in the delete canvas. Canvas transition is all handled by buttons
    public void ConfirmDelete()
    {
        string DeletedFile = SelectedSaveFile.FileName;
        ES3.DeleteFile("saves/" + DeletedFile + ".tung");

        ES3.Save<bool>("FileDeleted", true, "saves/backups/" + DeletedFile + "/info.txt");
        ES3.Save<DateTime>("DeletedOn", DateTime.Now, "saves/backups/" + DeletedFile + "/info.txt");

        GenerateLoadGamesMenu();
    }

    void DeleteOldBackupFolders()
    {
        // create the keys in settings.txt if they don't exist yet
        if (!ES3.KeyExists("DaysToKeepBackupsOfDeletedSaves", "settings.txt")) { ES3.Save<int>("DaysToKeepBackupsOfDeletedSaves", 10, "settings.txt"); }

        if (!ES3.DirectoryExists("saves/backups/")) { return; }

        string[] BackupFolders = ES3.GetDirectories("saves/backups");
        foreach(string BackupFolder in BackupFolders)
        {
            if(ES3.Load<bool>("FileDeleted", "saves/backups/" + BackupFolder + "/info.txt", false))
            {
                DateTime deletedon = ES3.Load<DateTime>("DeletedOn", "saves/backups/" + BackupFolder + "/info.txt");
                TimeSpan TimeSinceDeletion = DateTime.Now - deletedon;
                if(TimeSinceDeletion.Days > ES3.Load<int>("DaysToKeepBackupsOfDeletedSaves", "settings.txt", 10))
                {
                    ES3.DeleteDirectory("saves/backups/" + BackupFolder);
                }
            }
        }

    }
}
