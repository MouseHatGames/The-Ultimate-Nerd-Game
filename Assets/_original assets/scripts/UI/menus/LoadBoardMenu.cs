using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using SavedObjects;

public class LoadBoardMenu : MonoBehaviour
{
    public GameObject SavedBoardPrefab;
    public RectTransform Parent;

    public UISavedBoard SelectedBoard;

    public static LoadBoardMenu Instance;
    void Start()
    {
        Instance = this;

        GenerateLoadBoardsMenu();
    }

    public Canvas LoadBoardCanvas;
    public Canvas DeleteBoardCanvas;
    public Canvas RenameBoardCanvas;

    private void HideAll()
    {
        LoadBoardCanvas.enabled = false;
        DeleteBoardCanvas.enabled = false;
        RenameBoardCanvas.enabled = false;
    }
    public void ShowLoadBoard()
    {
        HideAll();
        LoadBoardCanvas.enabled = true;
    }
    public void ShowRename()
    {
        HideAll();
        RenameBoardCanvas.enabled = true;
    }
    public void ShowDelete()
    {
        HideAll();
        DeleteBoardCanvas.enabled = true;
    }

    public void Initialize()
    {
        ShowLoadBoard();
        GameplayUIManager.UIState = UIState.LoadBoard;
    }

    public void RunLoadBoardMenu()
    {
        if (RenameBoardCanvas.enabled)
        {
            if (Input.GetButtonDown("Cancel")) { ShowLoadBoard(); }
            if (Input.GetButtonDown("Confirm")) { ShowLoadBoard(); SetNewName(); }
        }
        else if (DeleteBoardCanvas.enabled)
        {
            if (Input.GetButtonDown("Cancel")) { ShowLoadBoard(); }
        }
        else
        {
            if (Input.GetButtonDown("Cancel")) { Cancel(); }
            if (Input.GetButtonDown("Confirm") || Input.GetButtonDown("BoardMenu")) { Load(); }
            if (Input.GetKeyDown(KeyCode.F5)) { GenerateLoadBoardsMenu(); }
        }
    }

    public void GenerateLoadBoardsMenu()
    {
        // delete the existing menu, if it exists. The menu needs to be regenerated when something is renamed or deleted
        foreach (UISavedBoard uisave in UISavedBoards)
        {
            Destroy(uisave.gameObject);
        }
        UISavedBoards.Clear();

        int Saves = LoadBoardsAndReturnTheNumberOfBoards();

        // set the content field to the correct length for the number of saves
        Parent.sizeDelta = new Vector2(784, 20 + Saves * 110);
    }

    public int LoadBoardsAndReturnTheNumberOfBoards()
    {
        string BoardsDirectory = Application.persistentDataPath + "/savedboards/";
        if (!Directory.Exists(BoardsDirectory))
        {
            Directory.CreateDirectory(BoardsDirectory);
        }

        string[] files = Directory.GetFiles(BoardsDirectory);

        // make sure they're .tungboard files
        List<string> boardpaths = new List<string>();
        foreach (string file in files)
        {
            if (file.Substring(file.Length - 10) == ".tungboard")
            {
                boardpaths.Add(file);
            }
        }

        for (int i = 0; i < boardpaths.Count; i++)
        {
            GameObject penis = Instantiate(SavedBoardPrefab, Parent);
            FileInfo penisinfo = new FileInfo(boardpaths[i]);

            UISavedBoard vagina = penis.GetComponent<UISavedBoard>();
            vagina.FilePath = boardpaths[i];
            vagina.Title.text = Path.GetFileNameWithoutExtension(boardpaths[i]);

            vagina.SetPosition(i); // new method of setting position


            vagina.Info.text =
                penisinfo.Length.ToString() + " bytes" // get file size
                ;// + " | " + penisinfo.LastAccessTime.ToString(); // get the local time of the last time the save was modified

            UISavedBoards.Add(vagina);
        }

        Debug.Log(boardpaths.Count.ToString() + " saved boards");
        return boardpaths.Count;
    }

    public List<UISavedBoard> UISavedBoards = new List<UISavedBoard>();

    Color32 deselectedcolor = new Color32(212, 212, 212, 255);
    Color32 selectedcolor = new Color32(62, 159, 255, 255);
    public void ChangeSelectedSave(UISavedBoard selected)
    {
        // double click functionality
        if (selected == SelectedBoard) { Load(); return; }

        SelectedBoard = selected;

        // change its color, change all the other colors back to default
        foreach (UISavedBoard save in UISavedBoards)
        {
            save.image.color = deselectedcolor;
        }

        selected.image.color = selectedcolor;
    }

    public void OpenBoardsFolder()
    {
        System.Diagnostics.Process.Start(@Application.persistentDataPath + "/savedboards");
    }

    public void Load()
    {
        if (SelectedBoard == null) { return; }

        SavedObjectV2 save = (SavedObjectV2)FileUtilities.LoadFromFile(SelectedBoard.FilePath);
        GameObject LoadedBoard = SavedObjectUtilities.LoadSavedObject(save);

        HideAll();

        LoadedBoard.transform.position = new Vector3(0, -2000, 0);
        BoardFunctions.RecalculateClustersOfBoard(LoadedBoard);

        BoardPlacer.NewBoardBeingPlaced(LoadedBoard);
        GameplayUIManager.UIState = UIState.BoardBeingPlaced;
    }

    public TMPro.TMP_InputField RenameInput;
    // when the button is pressed on the Load Game canvas. The canvas transition is handled by the buttons themselves
    public void Rename()
    {
        if (SelectedBoard == null) { return; }

        ShowRename();
        RenameInput.text = SelectedBoard.Title.text;
        RenameInput.ActivateInputField();
    }

    // when the button is pressed to rename the thing
    public void SetNewName()
    {
        string NewName = FileUtilities.ValidatedUniqueBoardName(RenameInput.text);
        string OldPath = SelectedBoard.FilePath;

        File.Move(OldPath,
            Application.persistentDataPath + "/savedboards/" + NewName + ".tungboard");

        ShowLoadBoard();
        GenerateLoadBoardsMenu();
    }

    public void Delete()
    {
        if (SelectedBoard == null) { return; }

        ShowDelete();
    }

    // this is when the yes, delete! button is pressed in the delete canvas
    public void ConfirmDelete()
    {
        File.Delete(SelectedBoard.FilePath);

        ShowLoadBoard();
        GenerateLoadBoardsMenu();
    }

    public void Cancel()
    {
        HideAll();
        GameplayUIManager.UIState = UIState.None;
    }
}