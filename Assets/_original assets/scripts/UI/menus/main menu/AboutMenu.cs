// loads the about menu text and contains methods for opening the links in the about menu.

using UnityEngine;
using TMPro;

public class AboutMenu : MonoBehaviour
{
    private void Awake()
    {
        LoadReadme();
    }

    public TextMeshProUGUI ReadmeText;
    private void LoadReadme()
    {
        TextAsset readme = Resources.Load<TextAsset>("readme");

        ReadmeText.text = readme.text;
    }
}