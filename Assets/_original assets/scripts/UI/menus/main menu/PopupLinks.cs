using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using NaughtyAttributes;

public class PopupLinks : MonoBehaviour
{
    private enum PitchType { email, reddit, discord, secret }
    private PitchType CurrentPitchType;

    [SerializeField]
    private TextMeshProUGUI PitchText;
    [SerializeField]
    private Canvas Canvas;

    [ResizableTextArea]
    public string EmailListPitch;
    private string EmailLink = "http://eepurl.com/c_6LBH";

    [ResizableTextArea]
    public string RedditPitch;
    private string RedditLink = "https://www.reddit.com/r/TheUltimateNerdGame/";

    [ResizableTextArea]
    public string DiscordPitch;
    private string DiscordLink = "https://discord.gg/7WkVJ88";

    [ResizableTextArea]
    public string SecretPitch;
    private string SecretLink = "https://www.youtube.com/watch?v=dQw4w9WgXcQ";

    public void TakeMeThere()
    {
        if (CurrentPitchType == PitchType.email) { Application.OpenURL(EmailLink); }
        else if (CurrentPitchType == PitchType.reddit) { Application.OpenURL(RedditLink); }
        else if (CurrentPitchType == PitchType.discord) { Application.OpenURL(DiscordLink); }
        else if (CurrentPitchType == PitchType.secret) { Application.OpenURL(SecretLink); }
    }

    private void SetPitchText()
    {
        if (CurrentPitchType == PitchType.email) { PitchText.text = EmailListPitch; }
        else if (CurrentPitchType == PitchType.reddit) { PitchText.text = RedditPitch; }
        else if (CurrentPitchType == PitchType.discord) { PitchText.text = DiscordPitch; }
        else if (CurrentPitchType == PitchType.secret) { PitchText.text = SecretPitch; }
    }

    private void EnablePopup()
    {
        RunMainMenu.Instance.HideAll();
        Canvas.enabled = true;
        SetPitchText();
    }

    private void Start()
    {
        // increase by 1, reset to 0 if above 15
        int PopupCounter = Settings.Get("PopupCounter", 0);
        PopupCounter++;
        if (PopupCounter > 15) { PopupCounter = 0; }
        Settings.Save("PopupCounter", PopupCounter);

        if(PopupCounter == 5 && !Settings.Get("DontPitchEmail", false))
        {
            CurrentPitchType = PitchType.email;
            EnablePopup();
        }
        else if (PopupCounter == 10 && !Settings.Get("DontPitchReddit", false))
        {
            CurrentPitchType = PitchType.reddit;
            EnablePopup();
        }
        else if (PopupCounter == 15 && !Settings.Get("DontPitchDiscord", false))
        {
            CurrentPitchType = PitchType.discord;
            EnablePopup();
        }
        else if (Random.Range(0, int.MaxValue) == 69)
        {
            CurrentPitchType = PitchType.secret;
            EnablePopup();
        }
        else
        {
            Canvas.enabled = false;
        }

    }

    public void Close()
    {
        Canvas.enabled = false;
        RunMainMenu.Instance.ShowMainMenu();
    }

    public void DontShowMeAgain()
    {
        if (CurrentPitchType == PitchType.email) { Settings.Save("DontPitchEmail", true); }
        else if (CurrentPitchType == PitchType.reddit) { Settings.Save("DontPitchReddit", true); }
        else if (CurrentPitchType == PitchType.discord) { Settings.Save("DontPitchDiscord", true); }

        Close();
    }
}