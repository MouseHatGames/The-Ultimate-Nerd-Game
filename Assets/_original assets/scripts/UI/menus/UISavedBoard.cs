using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UISavedBoard : MonoBehaviour
{
    public Image image;
    public TextMeshProUGUI Title;
    public TextMeshProUGUI Info;

    public string FilePath;

    public void Pressed()
    {
        LoadBoardMenu.Instance.ChangeSelectedBoard(this);
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