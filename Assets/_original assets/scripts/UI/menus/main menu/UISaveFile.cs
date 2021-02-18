using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UISaveFile : MonoBehaviour
{
    public Image image;
    public TextMeshProUGUI Title;
    public TextMeshProUGUI Info;
    public GameObject LegacyWarning;

    public string FileName;
    public bool LegacySave;

    public void Pressed()
    {
        LoadGameMenu.Instance.ChangeSelectedSave(this);
    }

    // after upgrading to 2017.3, LoadGame's implementation of setting the position was broken. After a lot of experimenting,
    // I determined that any RectTransforms CANNOT be changed during the frame they are created. Hopefully a bug so I
    // can go back to my nice solution later, but for now, this works.
    public void SetPosition(int PositionInSaveList)
    {
        StartCoroutine(SetPositionAfterDelay(PositionInSaveList));
    }

    private IEnumerator SetPositionAfterDelay(int PositionInSaveList)
    {
        yield return new WaitForEndOfFrame();
        transform.localPosition = new Vector3(0, -15 + PositionInSaveList * -110, 0);
    }
}