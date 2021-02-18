using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.PostProcessing;

public class OptionsMenu : MonoBehaviour
{
    public AudioMixer GlobalMixer;
    public PostProcessingProfile Profile;

    // sound
    public TextMeshProUGUI GlobalVolumeLabel;
    public Slider GlobalVolumeSlider;
    public TextMeshProUGUI MusicVolumeLabel;
    public Slider MusicVolumeSlider;
    public TextMeshProUGUI SFXVolumeLabel;
    public Slider SFXVolumeSlider;

    // controls
    public TextMeshProUGUI XSensitivityLabel;
    public Slider XSensitivitySlider;
    public TextMeshProUGUI YSensitivityLabel;
    public Slider YSensitivitySlider;
    public Toggle InvertMouseXToggle;
    public Toggle InvertMouseYToggle;
    public TMP_Dropdown ConnectionModeDropdown;
    public TMP_Dropdown BoardMenuDropdown;

    // graphics
    public TextMeshProUGUI MaxFPSLabel;
    public Slider MaxFPSSlider;
    public Toggle ShowFPSToggle;
    public TMP_Dropdown VSyncDropdown;
    public TMP_Dropdown AADropdown;
    public Toggle AnisotropicFilteringToggle;
    public Toggle EnableShadowsToggle;
    public TextMeshProUGUI ShadowDistanceLabel;
    public Slider ShadowDistanceSlider;
    public Toggle HeadBobToggle;
    public TextMeshProUGUI FOVLabel;
    public Slider FOVSlider;
    public Toggle MotionBlurToggle;
    public Toggle DitheringToggle;
    public Toggle BloomToggle;
    public TextMeshProUGUI BloomIntensityLabel;
    public Slider BloomIntensitySlider;
    public Toggle AOToggle;
    public TextMeshProUGUI AOIntensityLabel;
    public Slider AOIntensitySlider;
    public TextMeshProUGUI AORadiusLabel;
    public Slider AORadiusSlider;
    public TMP_Dropdown AOSampleCountDropdown;
    public Toggle AODownsamplingToggle;
    public Toggle HighPrecisionAOToggle;
    public Toggle FullscreenToggle;
    public TMP_Dropdown ResolutionDropdown;



    // Use this for initialization
    void Start () {
        // load settings from settings.txt and make the menu reflect them
        ApplyAllSettings();
	}

    public void ApplyAllSettings()
    {
        ApplyGlobalVolume();
        ApplyMusicVolume();
        ApplySFXVolume();

        ApplyXSensitivity();
        ApplyYSensitivity();
        ApplyInvertMouseX();
        ApplyInvertMouseY();
        ApplyConnectionMode();
        ApplyBoardMenuMode();

        ApplyMaxFPS();
        ApplyShowFPS();
        ApplyVSync();
        ApplyAA();
        ApplyAnisotropic();
        ApplyShadowToggle();
        ApplyShadowDistance();
        ApplyHeadBob();
        ApplyFOV();
        ApplyMotionBlur();
        ApplyDithering();
        ApplyBloom();
        ApplyBloomIntensity();
        ApplyAmbientOcclusion();
        ApplyAOIntensity();
        ApplyAORadius();
        ApplyAOSamples();
        ApplyAODownsampling();
        ApplyAOHighPrecision();
        ApplyFullscreen();
    }

    // this function works by deleting all settings keys and then re-applying all settings, which loads their default values
    public void ResetOptionsToDefault()
    {
        ES3.DeleteKey("GlobalVolume", "settings.txt");
        ES3.DeleteKey("MusicVolume", "settings.txt");
        ES3.DeleteKey("SFXVolume", "settings.txt");

        ES3.DeleteKey("XSensitivity", "settings.txt");
        ES3.DeleteKey("YSensitivity", "settings.txt");
        ES3.DeleteKey("InvertMouseX", "settings.txt");
        ES3.DeleteKey("InvertMouseY", "settings.txt");

        ES3.DeleteKey("MaxFPS", "settings.txt");
        ES3.DeleteKey("ShowFPS", "settings.txt");
        ES3.DeleteKey("VSyncType", "settings.txt");
        ES3.DeleteKey("AntiAliasingLevel", "settings.txt");
        ES3.DeleteKey("AnisotropicFiltering", "settings.txt");
        ES3.DeleteKey("EnableShadows", "settings.txt");
        ES3.DeleteKey("ShadowDistance", "settings.txt");
        ES3.DeleteKey("HeadBob", "settings.txt");
        ES3.DeleteKey("FOV", "settings.txt");
        ES3.DeleteKey("MotionBlur", "settings.txt");
        ES3.DeleteKey("Dithering", "settings.txt");
        ES3.DeleteKey("Bloom", "settings.txt");
        ES3.DeleteKey("BloomIntensity", "settings.txt");
        ES3.DeleteKey("AmbientOcclusion", "settings.txt");
        ES3.DeleteKey("AOIntensity", "settings.txt");
        ES3.DeleteKey("AORadius", "settings.txt");
        ES3.DeleteKey("AOSamples", "settings.txt");
        ES3.DeleteKey("AODownsampling", "settings.txt");
        ES3.DeleteKey("AOHighPrecision", "settings.txt");
        ES3.DeleteKey("Fullscreen", "settings.txt");
        ES3.DeleteKey("ConnectionMode", "settings.txt");


        ApplyAllSettings();
    }


    // audio
    // global volume
    public void OnGlobalVolumeSliderChange()
    {
        float value = GlobalVolumeSlider.value;
        Settings.Save("GlobalVolume", value);
        ApplyGlobalVolume();
    }
    private void ApplyGlobalVolume()
    {
        float value = Settings.Get("GlobalVolume", 100f);
        GlobalVolumeLabel.text = "Global Volume: " + value;
        GlobalVolumeSlider.value = value;

        float Decibels;
        if (value == 0) { Decibels = -144.0f; }
        else { Decibels = 20.0f * Mathf.Log10(value / 100); }

        GlobalMixer.SetFloat("GlobalVolume", Decibels);
    }

    // music volume
    public void OnMusicVolumeSliderChange()
    {
        float value = MusicVolumeSlider.value;
        Settings.Save("MusicVolume", value);
        ApplyMusicVolume();
    }
    private void ApplyMusicVolume()
    {
        float value = Settings.Get("MusicVolume", 100f);
        MusicVolumeLabel.text = "Music Volume: " + value;
        MusicVolumeSlider.value = value;

        float Decibels;
        if (value == 0) { Decibels = -144.0f; }
        else { Decibels = 20.0f * Mathf.Log10(value / 100); }

        GlobalMixer.SetFloat("MusicVolume", Decibels);
    }

    // SFX volume
    public void OnSFXVolumeSliderChange()
    {
        float value = SFXVolumeSlider.value;
        Settings.Save("SFXVolume", value);
        ApplySFXVolume();
    }
    private void ApplySFXVolume()
    {
        float value = Settings.Get("SFXVolume", 100f);
        SFXVolumeLabel.text = "SFX Volume: " + value;
        SFXVolumeSlider.value = value;

        float Decibels;
        if (value == 0) { Decibels = -144.0f; }
        else { Decibels = 20.0f * Mathf.Log10(value / 100); }

        GlobalMixer.SetFloat("SFXVolume", Decibels);
    }


    // controls
    // X sensitivity
    public void OnXSensitivitySliderChange()
    {
        float value = XSensitivitySlider.value;
        Settings.Save("XSensitivity", value);
        ApplyXSensitivity();

        if (Input.GetButton("Mod")) { YSensitivitySlider.value = value; }
    }
    private void ApplyXSensitivity()
    {
        float value = Settings.Get("XSensitivity", 20f);
        XSensitivityLabel.text = "X Sensitivity: " + value;
        XSensitivitySlider.value = value;

        if (SettingsApplier.Instance != null) { SettingsApplier.Instance.LoadXSensitivity(); }
    }

    // Y sensitivity
    public void OnYSensitivitySliderChange()
    {
        float value = YSensitivitySlider.value;
        Settings.Save("YSensitivity", value);
        ApplyYSensitivity();

        if (Input.GetButton("Mod")) { XSensitivitySlider.value = value; }
    }
    private void ApplyYSensitivity()
    {
        float value = Settings.Get("YSensitivity", 20f);
        YSensitivityLabel.text = "Y Sensitivity: " + value;
        YSensitivitySlider.value = value;

        if (SettingsApplier.Instance != null) { SettingsApplier.Instance.LoadYSensitivity(); }
    }

    // invert mouse X
    public void OnInvertMouseXToggle()
    {
        bool value = InvertMouseXToggle.isOn;
        Settings.Save("InvertMouseX", value);
        ApplyInvertMouseX();
    }
    private void ApplyInvertMouseX()
    {
        bool value = Settings.Get("InvertMouseX", false);
        InvertMouseXToggle.isOn = value;

        if (SettingsApplier.Instance != null) { SettingsApplier.Instance.LoadXSensitivity(); }
    }

    // invert mouse Y
    public void OnInvertMouseYToggle()
    {
        bool value = InvertMouseYToggle.isOn;
        Settings.Save("InvertMouseY", value);
        ApplyInvertMouseY();
    }
    private void ApplyInvertMouseY()
    {
        bool value = Settings.Get("InvertMouseY", false);
        InvertMouseYToggle.isOn = value;

        if (SettingsApplier.Instance != null) { SettingsApplier.Instance.LoadYSensitivity(); }
    }

    // connection mode
    public void OnConnectionModeChange()
    {
        int value = ConnectionModeDropdown.value;
        Settings.Save("ConnectionMode", value);
        ApplyConnectionMode();
    }
    private void ApplyConnectionMode()
    {
        int value = Settings.Get("ConnectionMode", 0);
        ConnectionModeDropdown.value = value;
        if (value == 0) { WirePlacer.ConnectionMode = ConnectionMode.HoldDown; }
        else if (value == 1) { WirePlacer.ConnectionMode = ConnectionMode.MultiPhase; }
        else if (value == 2) { WirePlacer.ConnectionMode = ConnectionMode.Chained; }
    }

    // board menu mod
    public void OnBoardMenuModeChange()
    {
        int value = BoardMenuDropdown.value;
        Settings.Save("BoardMenuMode", value);
        ApplyBoardMenuMode();
    }
    private void ApplyBoardMenuMode()
    {
        int value = Settings.Get("BoardMenuMode", 0);
        BoardMenuDropdown.value = value;
        if (value == 0) { BoardMenu.MenuMode = BoardMenuMode.TapTwice; }
        else if (value == 1) { BoardMenu.MenuMode = BoardMenuMode.HoldThenRelease; }
    }


    // graphics
    // max FPS
    public void OnMaxFPSSliderChange()
    {
        int value = (int)MaxFPSSlider.value;
        Settings.Save("MaxFPS", value);
        ApplyMaxFPS();
    }
    private void ApplyMaxFPS()
    {
        int value = Settings.Get("MaxFPS", 60);
        MaxFPSLabel.text = "Max FPS: " + value;
        MaxFPSSlider.value = value;

        Application.targetFrameRate = value;
    }

    // show FPS
    public void OnShowFPSToggle()
    {
        bool value = ShowFPSToggle.isOn;
        Settings.Save("ShowFPS", value);
        ApplyShowFPS();
    }
    private void ApplyShowFPS()
    {
        bool value = Settings.Get("ShowFPS", false);
        ShowFPSToggle.isOn = value;
        if (SettingsApplier.Instance != null) { SettingsApplier.Instance.LoadShowFPS(); }
    }

    // vsync
    public void OnVSyncChange()
    {
        int value = VSyncDropdown.value;
        Settings.Save("VSyncType", value);
        ApplyVSync();
    }
    private void ApplyVSync()
    {
        int value = Settings.Get("VSyncType", 1);
        VSyncDropdown.value = value;
        QualitySettings.vSyncCount = value;
    }

    // anti-aliasing
    public void OnAAChange()
    {
        int value = AADropdown.value;
        Settings.Save("AntiAliasingLevel", value);
        ApplyAA();
    }
    private void ApplyAA()
    {
        int value = Settings.Get("AntiAliasingLevel", 1);
        AADropdown.value = value;

        // get the proper AA level as it corresponds to the dropdown value
        int AACount = 0;
        if (value == 0) { AACount = 0; }
        else if (value == 1) { AACount = 2; }
        else if (value == 2) { AACount = 4; }
        else if (value == 3) { AACount = 8; }

        QualitySettings.antiAliasing = AACount;
    }

    // anisotropic filtering
    public void OnAnisotropicToggle()
    {
        bool value = AnisotropicFilteringToggle.isOn;
        Settings.Save("AnisotropicFiltering", value);
        ApplyAnisotropic();
    }
    private void ApplyAnisotropic()
    {
        bool value = Settings.Get("AnisotropicFiltering", true);
        AnisotropicFilteringToggle.isOn = value;
        AnisotropicFiltering af = AnisotropicFiltering.Disable; if (value) { af = AnisotropicFiltering.Enable; }
        QualitySettings.anisotropicFiltering = af;
    }

    // toggle shadows
    public void OnShadowsToggle()
    {
        bool value = EnableShadowsToggle.isOn;
        Settings.Save("EnableShadows", value);
        ApplyShadowToggle();
    }
    private void ApplyShadowToggle()
    {
        bool value = Settings.Get("EnableShadows", true);
        EnableShadowsToggle.isOn = value;
        ShadowQuality sq = ShadowQuality.Disable; if (value) { sq = ShadowQuality.All; }
        QualitySettings.shadows = sq;

        ShadowDistanceSlider.interactable = value;
    }

    // shadow distance
    public void OnShadowDistanceSliderChange()
    {
        float value = ShadowDistanceSlider.value;
        Settings.Save("ShadowDistance", value);
        ApplyShadowDistance();
    }
    private void ApplyShadowDistance()
    {
        float value = Settings.Get("ShadowDistance", 60f);
        ShadowDistanceLabel.text = "Shadow Distance: " + value;
        ShadowDistanceSlider.value = value;

        QualitySettings.shadowDistance = value;
    }

    // head bob
    public void OnHeadBobToggle()
    {
        bool value = HeadBobToggle.isOn;
        Settings.Save("HeadBob", value);
        ApplyHeadBob();
    }
    private void ApplyHeadBob()
    {
        bool value = Settings.Get("HeadBob", true);
        HeadBobToggle.isOn = value;

        if (SettingsApplier.Instance != null) { SettingsApplier.Instance.LoadHeadBob(); }
    }

    // field of view
    public void OnFOVSliderChange()
    {
        float value = FOVSlider.value;
        Settings.Save("FOV", value);
        ApplyFOV();
    }
    private void ApplyFOV()
    {
        float value = Settings.Get("FOV", 70f);
        FOVLabel.text = "Field of View: " + value;
        FOVSlider.value = value;

        if (SettingsApplier.Instance != null) { SettingsApplier.Instance.LoadFOV(); }
    }

    // motion blur
    public void OnMotionBlurToggle()
    {
        bool value = MotionBlurToggle.isOn;
        Settings.Save("MotionBlur", value);
        ApplyMotionBlur();
    }
    private void ApplyMotionBlur()
    {
        bool value = Settings.Get("MotionBlur", false);
        MotionBlurToggle.isOn = value;
        Profile.motionBlur.enabled = value;
    }

    // dithering
    public void OnDitheringToggle()
    {
        bool value = DitheringToggle.isOn;
        Settings.Save("Dithering", value);
        ApplyDithering();
    }
    private void ApplyDithering()
    {
        bool value = Settings.Get("Dithering", false);
        DitheringToggle.isOn = value;
        Profile.dithering.enabled = value;
    }

    // bloom toggle
    public void OnBloomToggle()
    {
        bool value = BloomToggle.isOn;
        Settings.Save("Bloom", value);
        ApplyBloom();
    }
    private void ApplyBloom()
    {
        bool value = Settings.Get("Bloom", false);
        BloomToggle.isOn = value;
        Profile.bloom.enabled = value;

        BloomIntensitySlider.interactable = value;
    }

    // bloom intensity
    public void OnBloomIntensitySliderChange()
    {
        float value = BloomIntensitySlider.value;
        Settings.Save("BloomIntensity", value);
        ApplyBloomIntensity();
    }
    private void ApplyBloomIntensity()
    {
        float value = Settings.Get("BloomIntensity", 0.4f);
        value = Mathf.Round(value / 0.1f) * 0.1f; // round to 0.1
        BloomIntensityLabel.text = "Bloom Intensity: " + value;
        BloomIntensitySlider.value = value;

        // shitty hack but for some reason is required
        var bloom = Profile.bloom.settings;
        bloom.bloom.intensity = value;
        Profile.bloom.settings = bloom;
    }

    // toggle ambient occulsion
    public void OnAmbientOcclusionToggle()
    {
        bool value = AOToggle.isOn;
        Settings.Save("AmbientOcclusion", value);
        ApplyAmbientOcclusion();
    }
    private void ApplyAmbientOcclusion()
    {
        bool value = Settings.Get("AmbientOcclusion", true);
        AOToggle.isOn = value;
        Profile.ambientOcclusion.enabled = value;

        AOIntensitySlider.interactable = value;
        AORadiusSlider.interactable = value;
        AOSampleCountDropdown.interactable = value;
        AODownsamplingToggle.interactable = value;
        HighPrecisionAOToggle.interactable = value;
    }

    // ambient occulsion intensity
    public void OnAOIntensitySliderChange()
    {
        float value = AOIntensitySlider.value;
        Settings.Save("AOIntensity", value);
        ApplyAOIntensity();
    }
    private void ApplyAOIntensity()
    {
        float value = Settings.Get("AOIntensity", 0.3f);
        value = Mathf.Round(value / 0.1f) * 0.1f; // round to 0.1
        AOIntensityLabel.text = "AO Intensity: " + value;
        AOIntensitySlider.value = value;

        // shitty hack but for some reason is required
        var ao = Profile.ambientOcclusion.settings;
        ao.intensity = value;
        Profile.ambientOcclusion.settings = ao;
    }

    // ambient occlusion radius
    public void OnAORadiusSliderChange()
    {
        float value = AORadiusSlider.value;
        Settings.Save("AORadius", value);
        ApplyAORadius();
    }
    private void ApplyAORadius()
    {
        float value = Settings.Get("AORadius", 0.3f);
        value = Mathf.Round(value / 0.1f) * 0.1f; // round to 0.1
        AORadiusLabel.text = "AO Radius: " + value;
        AORadiusSlider.value = value;

        // shitty hack but for some reason is required
        var ao = Profile.ambientOcclusion.settings;
        ao.radius = value;
        Profile.ambientOcclusion.settings = ao;
    }

    // ambient occlusion samples
    public void OnAOSamplesChange()
    {
        int value = AOSampleCountDropdown.value;
        Settings.Save("AOSamples", value);
        ApplyAOSamples();
    }
    private void ApplyAOSamples()
    {
        int value = Settings.Get("AOSamples", 1);
        AOSampleCountDropdown.value = value;

        var ao = Profile.ambientOcclusion.settings;
        AmbientOcclusionModel.SampleCount sc = AmbientOcclusionModel.SampleCount.Lowest;
        if (value == 1) { sc = AmbientOcclusionModel.SampleCount.Low; }
        else if (value == 2) { sc = AmbientOcclusionModel.SampleCount.Medium; }
        else if (value == 3) { sc = AmbientOcclusionModel.SampleCount.High; }
        ao.sampleCount = sc;
        Profile.ambientOcclusion.settings = ao;
    }

    // ambient occlusion downsampling
    public void OnAODownsamplingToggle()
    {
        bool value = AODownsamplingToggle.isOn;
        Settings.Save("AODownsampling", value);
        ApplyAODownsampling();
    }
    private void ApplyAODownsampling()
    {
        bool value = Settings.Get("AODownsampling", false);
        AODownsamplingToggle.isOn = value;

        var ao = Profile.ambientOcclusion.settings;
        ao.downsampling = value;
        Profile.ambientOcclusion.settings = ao;
    }

    // ambient occulsion high precision
    public void OnAOHighPrecisionToggle()
    {
        bool value = HighPrecisionAOToggle.isOn;
        Settings.Save("AOHighPrecision", value);
        ApplyAOHighPrecision();
    }
    private void ApplyAOHighPrecision()
    {
        bool value = Settings.Get("AOHighPrecision", false);
        HighPrecisionAOToggle.isOn = value;

        var ao = Profile.ambientOcclusion.settings;
        ao.highPrecision = value;
        Profile.ambientOcclusion.settings = ao;
    }

    // fullscreen
    public void OnFullscreenToggle()
    {
        bool value = FullscreenToggle.isOn;
        Settings.Save("Fullscreen", value);
        ApplyFullscreen();
    }
    private void ApplyFullscreen()
    {
        bool value = Settings.Get("Fullscreen", true);
        FullscreenToggle.isOn = value;
        Screen.fullScreen = value;
    }
}

// 604 lines without this comment, holy cow. I think this might be the longest class I have ever written.
// to the jimmy who wrote that comment ~3 months before this one... oh you sweet sweet summer child.