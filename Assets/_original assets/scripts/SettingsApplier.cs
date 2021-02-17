using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class SettingsApplier : MonoBehaviour {

    public static SettingsApplier Instance;

	// Use this for initialization
	void Start () {
        Instance = this;

        LoadXSensitivity();
        LoadYSensitivity();
        LoadShowFPS();
        LoadHeadBob();
        LoadFOV();
	}
	
	public void LoadXSensitivity()
    {
        float value = ES3.Load<float>("XSensitivity", "settings.txt", 20);
        if (ES3.Load<bool>("InvertMouseX", "settings.txt", false)) { value = -value; }
        FirstPersonController.Instance.m_MouseLook.XSensitivity = value / 20;
    }

    public void LoadYSensitivity()
    {
        float value = ES3.Load<float>("YSensitivity", "settings.txt", 20);
        if (ES3.Load<bool>("InvertMouseY", "settings.txt", false)) { value = -value; }
        FirstPersonController.Instance.m_MouseLook.YSensitivity = value / 20;
    }

    public void LoadShowFPS()
    {
        if (ES3.Load<bool>("ShowFPS", "settings.txt", false))
        {
            FPSShower.Instance.enabled = true;
        }
        else
        {
            FPSShower.Instance.enabled = false;
        }
    }

    public void LoadHeadBob()
    {
        FirstPersonController.Instance.m_UseHeadBob = ES3.Load<bool>("HeadBob", "settings.txt", true);
    }

    public void LoadFOV()
    {
        Camera.main.fieldOfView = ES3.Load<float>("FOV", "settings.txt", 70);
    }
}
