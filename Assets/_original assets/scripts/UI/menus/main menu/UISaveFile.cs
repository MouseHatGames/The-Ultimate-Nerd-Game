using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UISaveFile : MonoBehaviour {

    public static LoadGame loadgame; // set in LoadGame.start

    public Image image;
    public TextMeshProUGUI Title;
    public TextMeshProUGUI Info;

    public string FileName;

    private void Awake()
    {
        image = GetComponent<Image>();
        Title = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        Info = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    public void Pressed()
    {
        loadgame.ChangeSelectedSave(this);
    }

    // after upgrading to 2017.3, LoadGame's implementation of setting the position was broken. After a lot of experimenting,
    // I determined that any RectTransforms CANNOT be changed during the frame they are instantiated in. Hopefully a bug so I
    // can go back to my nice solution later, but for now, this works.
    public void SetPosition(int PositionInSaveList)
    {
        StartCoroutine(SetPositionAfterDelay(PositionInSaveList));
    }

    private IEnumerator SetPositionAfterDelay(int PositionInSaveList)
    {
        yield return new WaitForEndOfFrame();
        transform.localPosition = new Vector3(0, -10 + PositionInSaveList * -110, 0);
    }

    [NaughtyAttributes.Button] public void TestTransform() { transform.localPosition = new Vector3(100, 100, 100); }
}
