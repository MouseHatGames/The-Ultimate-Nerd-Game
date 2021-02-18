// loads the about menu text and contains methods for opening the links in the about menu.

using UnityEngine;
using TMPro;

public class AboutMenu : MonoBehaviour {

	public void OpenEmail() { Application.OpenURL("http://eepurl.com/c_6LBH"); }
    public void OpenReddit() { Application.OpenURL("https://www.reddit.com/r/TheUltimateNerdGame/"); }
    public void OpenDiscord() { Application.OpenURL("https://discord.gg/7WkVJ88"); }
    public void OpenTwitter() { Application.OpenURL("https://twitter.com/mousehatgames"); }

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