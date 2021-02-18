using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UrlClick : MonoBehaviour
{
    public string URL;

    public void Click()
    {
        Application.OpenURL(URL);
    }
}
