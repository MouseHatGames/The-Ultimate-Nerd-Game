using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class SettingsApplier : MonoBehaviour
{
    public static SettingsApplier Instance;

	// Use this for initialization
	void Start () {
        Instance = this;

        LoadXSensitivity();
        LoadYSensitivity();
        LoadShowFPS();
        LoadHeadBob();
        LoadFOV();

        FirstPersonController.Instance.m_WalkSpeed = Settings.Get("WalkSpeed", 5f);
        FirstPersonController.Instance.m_RunSpeed = Settings.Get("RunSpeed", 10f);
        FirstPersonController.Instance.m_JumpSpeed = Settings.Get("JumpPower", 10f);
        FirstPersonController.Instance.FlyingSpeed = Settings.Get("FlyingSpeed", 7f);
        FirstPersonController.Instance.FlyingVerticalSpeed = Settings.Get("FlyingVerticalSpeed", 5f);
        FirstPersonController.Instance.FastFlyingSpeed = Settings.Get("FastFlyingSpeed", 15f);
        FirstPersonController.Instance.FastFlyingVerticalSpeed = Settings.Get("FastFlyingVerticalSpeed", 8f);

        FirstPersonController.Instance.GetComponentInChildren<cakeslice.OutlineEffect>().cornerOutlines = Settings.Get("CornerOutlines", true);
    }
	
	public void LoadXSensitivity()
    {
        float value = Settings.Get("XSensitivity", 20f);
        if (Settings.Get("InvertMouseX", false)) { value = -value; }
        FirstPersonController.Instance.m_MouseLook.XSensitivity = value / 20;
    }

    public void LoadYSensitivity()
    {
        float value = Settings.Get("YSensitivity", 20f);
        if (Settings.Get("InvertMouseY", false)) { value = -value; }
        FirstPersonController.Instance.m_MouseLook.YSensitivity = value / 20;
    }

    public void LoadShowFPS()
    {
        if (Settings.Get("ShowFPS", false))
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
        FirstPersonController.Instance.m_UseHeadBob = Settings.Get("HeadBob", true);
    }

    public void LoadFOV()
    {
        Camera.main.fieldOfView = Settings.Get("FOV", 70f);
    }
}