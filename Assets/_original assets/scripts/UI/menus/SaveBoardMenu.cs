using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SavedObjects;

public class SaveBoardMenu : MonoBehaviour
{
    public static SaveBoardMenu Instance;
    private Canvas Canvas;
    private void Awake()
    {
        Instance = this;
        Canvas = GetComponent<Canvas>();
    }

    public void RunSaveBoardMenu()
    {
        if (Input.GetButtonDown("Cancel")) { Done(); }
        if (Input.GetButtonDown("Confirm")) { ConfirmSaveBoard(); }
    }

    public static void SaveBoard()
    {
        RaycastHit hit;
        if (Physics.Raycast(FirstPersonInteraction.Ray(), out hit, Settings.ReachDistance, Wire.IgnoreWiresLayermask))
        {
            if (hit.collider.tag == "CircuitBoard")
            {
                GameObject SaveThis = hit.collider.gameObject;
                if (Input.GetButton("Mod"))
                {
                    GameObject RootBoard = hit.collider.transform.root.gameObject;
                    if (RootBoard.tag == "CircuitBoard") { SaveThis = RootBoard; } // make sure to check for circuitboard in case of mounts
                }

                Instance.SaveBoardAs(SaveThis);
                return;
            }
            else
            {
                SoundPlayer.PlaySoundGlobal(References.Sounds.FailDoSomething);
            }
        }
        else
        {
            SoundPlayer.PlaySoundGlobal(References.Sounds.FailDoSomething);
        }

        GameplayUIManager.UIState = UIState.None;
    }

    public TMP_InputField BoardNameInput;

    private static GameObject BoardBeingSaved; // use this instead of passing it between functions because of the UI buttons

    private void SaveBoardAs(GameObject BoardToSave)
    {
        if (BoardToSave.tag != "CircuitBoard")
        {
            GameplayUIManager.UIState = UIState.None;
            return;
        }

        BoardBeingSaved = BoardToSave;

        GameplayUIManager.UIState = UIState.SaveBoard;
        Canvas.enabled = true;
        BoardNameInput.text = FileUtilities.ValidatedUniqueBoardName("Saved Board");
        BoardNameInput.ActivateInputField();

        Canvas.enabled = true;
    }

    public void ConfirmSaveBoard()
    {
        if(BoardBeingSaved == null) { Done(); return; }

        SavedObjectV2 BoardSavedObject = SavedObjectUtilities.CreateSavedObjectFrom(BoardBeingSaved);
        FileUtilities.SaveToFile(Application.persistentDataPath + "/savedboards/", FileUtilities.ValidatedUniqueBoardName(BoardNameInput.text) + ".tungboard", BoardSavedObject);

        LoadBoardMenu.Instance.GenerateLoadBoardsMenu();
        Done();
    }

    public void Done()
    {       
        BoardBeingSaved = null;
        Canvas.enabled = false;
        GameplayUIManager.UIState = UIState.None;
    }
}