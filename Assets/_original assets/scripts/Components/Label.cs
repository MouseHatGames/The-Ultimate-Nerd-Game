using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Label : Interactable
{
    public TextMeshPro text;
    public void NewlyPlaced()
    {
        text.fontSize = TextEditMenu.MostRecentFontSize;
    }

    // Use this for initialization
    void Awake ()
    {
        text = GetComponentInChildren<TextMeshPro>();
	}

    public override void Interact()
    {
        TextEditMenu.Instance.Initialize(text);
    }
}