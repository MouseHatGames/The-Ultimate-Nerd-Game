using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Label : MonoBehaviour {

    public Interactable interactable;
    public TextMeshPro text;

    public static TextEditMenu texteditmenu;

    // Use this for initialization
    void Awake () {
        interactable = GetComponent<Interactable>();
        text = GetComponentInChildren<TextMeshPro>();
	}

    // Update is called once per frame
    void Update () {
        if (interactable.Interacted)
        {
            interacted();
        }
	}

    void interacted()
    {
        interactable.Interacted = false;
        TextEditMenu.TextBeingEdited = text;
        texteditmenu.enabled = true;
    }
}
