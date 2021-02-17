using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.PostProcessing;

public class OptionsMenu : MonoBehaviour {

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
        ES3.Save<float>("GlobalVolume", value, "settings.txt");
        ApplyGlobalVolume();
    }
    private void ApplyGlobalVolume()
    {
        float value = ES3.Load<float>("GlobalVolume", "settings.txt", 100);
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
        ES3.Save<float>("MusicVolume", value, "settings.txt");
        ApplyMusicVolume();
    }
    private void ApplyMusicVolume()
    {
        float value = ES3.Load<float>("MusicVolume", "settings.txt", 100);
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
        ES3.Save<float>("SFXVolume", value, "settings.txt");
        ApplySFXVolume();
    }
    private void ApplySFXVolume()
    {
        float value = ES3.Load<float>("SFXVolume", "settings.txt", 100);
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
        ES3.Save<float>("XSensitivity", value, "settings.txt");
        ApplyXSensitivity();
    }
    private void ApplyXSensitivity()
    {
        float value = ES3.Load<float>("XSensitivity", "settings.txt", 20);
        XSensitivityLabel.text = "X Sensitivity: " + value;
        XSensitivitySlider.value = value;

        if (SettingsApplier.Instance != null) { SettingsApplier.Instance.LoadXSensitivity(); }
    }

    // Y sensitivity
    public void OnYSensitivitySliderChange()
    {
        float value = YSensitivitySlider.value;
        ES3.Save<float>("YSensitivity", value, "settings.txt");
        ApplyYSensitivity();
    }
    private void ApplyYSensitivity()
    {
        float value = ES3.Load<float>("YSensitivity", "settings.txt", 20);
        YSensitivityLabel.text = "Y Sensitivity: " + value;
        YSensitivitySlider.value = value;

        if (SettingsApplier.Instance != null) { SettingsApplier.Instance.LoadYSensitivity(); }
    }

    // invert mouse X
    public void OnInvertMouseXToggle()
    {
        bool value = InvertMouseXToggle.isOn;
        ES3.Save<bool>("InvertMouseX", value, "settings.txt");
        ApplyInvertMouseX();
    }
    private void ApplyInvertMouseX()
    {
        bool value = ES3.Load<bool>("InvertMouseX", "settings.txt", false);
        InvertMouseXToggle.isOn = value;

        if (SettingsApplier.Instance != null) { SettingsApplier.Instance.LoadXSensitivity(); }
    }

    // invert mouse Y
    public void OnInvertMouseYToggle()
    {
        bool value = InvertMouseYToggle.isOn;
        ES3.Save<bool>("InvertMouseY", value, "settings.txt");
        ApplyInvertMouseY();
    }
    private void ApplyInvertMouseY()
    {
        bool value = ES3.Load<bool>("InvertMouseY", "settings.txt", false);
        InvertMouseYToggle.isOn = value;

        if (SettingsApplier.Instance != null) { SettingsApplier.Instance.LoadYSensitivity(); }
    }

    // connection mode
    public void OnConnectionModeChange()
    {
        int value = ConnectionModeDropdown.value;
        ES3.Save<int>("ConnectionMode", value, "settings.txt");
        ApplyConnectionMode();
    }
    private void ApplyConnectionMode()
    {
        int value = ES3.Load<int>("ConnectionMode", "settings.txt", 0);
        ConnectionModeDropdown.value = value;
        if (value == 0) { FirstPersonInteraction.ConnectionMode = ConnectionMode.HoldDown; }
        else if (value == 1) { FirstPersonInteraction.ConnectionMode = ConnectionMode.MultiPhase; }
    }


    // graphics
    // max FPS
    public void OnMaxFPSSliderChange()
    {
        int value = (int)MaxFPSSlider.value;
        ES3.Save<int>("MaxFPS", value, "settings.txt");
        ApplyMaxFPS();
    }
    private void ApplyMaxFPS()
    {
        int value = ES3.Load<int>("MaxFPS", "settings.txt", 60);
        MaxFPSLabel.text = "Max FPS: " + value;
        MaxFPSSlider.value = value;

        Application.targetFrameRate = value;
    }

    // show FPS
    public void OnShowFPSToggle()
    {
        bool value = ShowFPSToggle.isOn;
        ES3.Save<bool>("ShowFPS", value, "settings.txt");
        ApplyShowFPS();
    }
    private void ApplyShowFPS()
    {
        bool value = ES3.Load<bool>("ShowFPS", "settings.txt", false);
        ShowFPSToggle.isOn = value;
        if (SettingsApplier.Instance != null) { SettingsApplier.Instance.LoadShowFPS(); }
    }

    // vsync
    public void OnVSyncChange()
    {
        int value = VSyncDropdown.value;
        ES3.Save<int>("VSyncType", value, "settings.txt");
        ApplyVSync();
    }
    private void ApplyVSync()
    {
        int value = ES3.Load<int>("VSyncType", "settings.txt", 1);
        VSyncDropdown.value = value;
        QualitySettings.vSyncCount = value;
    }

    // anti-aliasing
    public void OnAAChange()
    {
        int value = AADropdown.value;
        ES3.Save<int>("AntiAliasingLevel", value, "settings.txt");
        ApplyAA();
    }
    private void ApplyAA()
    {
        int value = ES3.Load<int>("AntiAliasingLevel", "settings.txt", 1);
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
        ES3.Save<bool>("AnisotropicFiltering", value, "settings.txt");
        ApplyAnisotropic();
    }
    private void ApplyAnisotropic()
    {
        bool value = ES3.Load<bool>("AnisotropicFiltering", "settings.txt", true);
        AnisotropicFilteringToggle.isOn = value;
        AnisotropicFiltering af = AnisotropicFiltering.Disable; if (value) { af = AnisotropicFiltering.Enable; }
        QualitySettings.anisotropicFiltering = af;
    }

    // toggle shadows
    public void OnShadowsToggle()
    {
        bool value = EnableShadowsToggle.isOn;
        ES3.Save<bool>("EnableShadows", value, "settings.txt");
        ApplyShadowToggle();
    }
    private void ApplyShadowToggle()
    {
        bool value = ES3.Load<bool>("EnableShadows", "settings.txt", true);
        EnableShadowsToggle.isOn = value;
        ShadowQuality sq = ShadowQuality.Disable; if (value) { sq = ShadowQuality.All; }
        QualitySettings.shadows = sq;

        ShadowDistanceSlider.interactable = value;
    }

    // shadow distance
    public void OnShadowDistanceSliderChange()
    {
        float value = ShadowDistanceSlider.value;
        ES3.Save<float>("ShadowDistance", value, "settings.txt");
        ApplyShadowDistance();
    }
    private void ApplyShadowDistance()
    {
        float value = ES3.Load<float>("ShadowDistance", "settings.txt", 60);
        ShadowDistanceLabel.text = "Shadow Distance: " + value;
        ShadowDistanceSlider.value = value;

        QualitySettings.shadowDistance = value;
    }

    // head bob
    public void OnHeadBobToggle()
    {
        bool value = HeadBobToggle.isOn;
        ES3.Save<bool>("HeadBob", value, "settings.txt");
        ApplyHeadBob();
    }
    private void ApplyHeadBob()
    {
        bool value = ES3.Load<bool>("HeadBob", "settings.txt", true);
        HeadBobToggle.isOn = value;

        if (SettingsApplier.Instance != null) { SettingsApplier.Instance.LoadHeadBob(); }
    }

    // field of view
    public void OnFOVSliderChange()
    {
        float value = FOVSlider.value;
        ES3.Save<float>("FOV", value, "settings.txt");
        ApplyFOV();
    }
    private void ApplyFOV()
    {
        float value = ES3.Load<float>("FOV", "settings.txt", 70);
        FOVLabel.text = "Field of View: " + value;
        FOVSlider.value = value;

        if (SettingsApplier.Instance != null) { SettingsApplier.Instance.LoadFOV(); }
    }

    // motion blur
    public void OnMotionBlurToggle()
    {
        bool value = MotionBlurToggle.isOn;
        ES3.Save<bool>("MotionBlur", value, "settings.txt");
        ApplyMotionBlur();
    }
    private void ApplyMotionBlur()
    {
        bool value = ES3.Load<bool>("MotionBlur", "settings.txt", false);
        MotionBlurToggle.isOn = value;
        Profile.motionBlur.enabled = value;
    }

    // dithering
    public void OnDitheringToggle()
    {
        bool value = DitheringToggle.isOn;
        ES3.Save<bool>("Dithering", value, "settings.txt");
        ApplyDithering();
    }
    private void ApplyDithering()
    {
        bool value = ES3.Load<bool>("Dithering", "settings.txt", false);
        DitheringToggle.isOn = value;
        Profile.dithering.enabled = value;
    }

    // bloom toggle
    public void OnBloomToggle()
    {
        bool value = BloomToggle.isOn;
        ES3.Save<bool>("Bloom", value, "settings.txt");
        ApplyBloom();
    }
    private void ApplyBloom()
    {
        bool value = ES3.Load<bool>("Bloom", "settings.txt", false);
        BloomToggle.isOn = value;
        Profile.bloom.enabled = value;

        BloomIntensitySlider.interactable = value;
    }

    // bloom intensity
    public void OnBloomIntensitySliderChange()
    {
        float value = BloomIntensitySlider.value;
        ES3.Save<float>("BloomIntensity", value, "settings.txt");
        ApplyBloomIntensity();
    }
    private void ApplyBloomIntensity()
    {
        float value = ES3.Load<float>("BloomIntensity", "settings.txt", 0.4f);
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
        ES3.Save<bool>("AmbientOcclusion", value, "settings.txt");
        ApplyAmbientOcclusion();
    }
    private void ApplyAmbientOcclusion()
    {
        bool value = ES3.Load<bool>("AmbientOcclusion", "settings.txt", false);
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
        ES3.Save<float>("AOIntensity", value, "settings.txt");
        ApplyAOIntensity();
    }
    private void ApplyAOIntensity()
    {
        float value = ES3.Load<float>("AOIntensity", "settings.txt", 1f);
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
        ES3.Save<float>("AORadius", value, "settings.txt");
        ApplyAORadius();
    }
    private void ApplyAORadius()
    {
        float value = ES3.Load<float>("AORadius", "settings.txt", 1f);
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
        ES3.Save<int>("AOSamples", value, "settings.txt");
        ApplyAOSamples();
    }
    private void ApplyAOSamples()
    {
        int value = ES3.Load<int>("AOSamples", "settings.txt", 1);
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
        ES3.Save<bool>("AODownsampling", value, "settings.txt");
        ApplyAODownsampling();
    }
    private void ApplyAODownsampling()
    {
        bool value = ES3.Load<bool>("AODownsampling", "settings.txt", false);
        AODownsamplingToggle.isOn = value;

        var ao = Profile.ambientOcclusion.settings;
        ao.downsampling = value;
        Profile.ambientOcclusion.settings = ao;
    }

    // ambient occulsion high precision
    public void OnAOHighPrecisionToggle()
    {
        bool value = HighPrecisionAOToggle.isOn;
        ES3.Save<bool>("AOHighPrecision", value, "settings.txt");
        ApplyAOHighPrecision();
    }
    private void ApplyAOHighPrecision()
    {
        bool value = ES3.Load<bool>("AOHighPrecision", "settings.txt", false);
        HighPrecisionAOToggle.isOn = value;

        var ao = Profile.ambientOcclusion.settings;
        ao.highPrecision = value;
        Profile.ambientOcclusion.settings = ao;
    }

    // fullscreen
    public void OnFullscreenToggle()
    {
        bool value = FullscreenToggle.isOn;
        ES3.Save<bool>("Fullscreen", value, "settings.txt");
        ApplyFullscreen();
    }
    private void ApplyFullscreen()
    {
        bool value = ES3.Load<bool>("Fullscreen", "settings.txt", true);
        FullscreenToggle.isOn = value;
        Screen.fullScreen = value;
    }
}

// 604 lines without this comment, holy cow. I think this might be the longest class I have ever written.