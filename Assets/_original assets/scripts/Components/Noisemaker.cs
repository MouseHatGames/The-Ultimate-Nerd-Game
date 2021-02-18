using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using References;

[RequireComponent(typeof(AudioSource))]
public class Noisemaker : VisualUpdaterWithMeshCombining
{
    public CircuitInput Input;

    public AudioSource Audio;

    [SerializeField] private float TrueToneFrequency; // serialized so it can be copied in CloneBoard
    public float ToneFrequency
    {
        get { return TrueToneFrequency; }
        set
        {
            TrueToneFrequency = value;
            GenerateClip(Audio.isPlaying);
        }
    }
    public void NewlyPlaced()
    {
        ToneFrequency = NoisemakerMenu.FrequencyForNewNoisemakers;
    }

    protected override void AfterAwake()
    {
        Renderer.material.color = Settings.NoisemakerOffColor;
        MegaMeshComponent.MaterialType = MaterialType.NoisemakerOff;

        if (ToneFrequency == 0) { ToneFrequency = NoisemakerMenu.FrequencyForNewNoisemakers; }

        GenerateClip(false);

        MegaMeshComponent.Mesh = MeshFilter.sharedMesh;
        MegaMeshManager.ScaleMesh(MeshFilter, 1.002f);
    }

    private void Start()
    {
        SetProperMaterialAndMegaMeshComponentMaterialType();
    }

    [SerializeField] bool PreviouslyOn; // serialized so it can be copied in cloneboard
    public override void VisualUpdate()
    {
        if (PreviouslyOn != Input.On)
        {
            VisualsChanged();
            PreviouslyOn = Input.On;
        }
        else
        {
            VisualsHaventChanged();
        }
    }

    protected override void SetProperMaterialAndMegaMeshComponentMaterialType()
    {
        if (Input.On)
        {
            Renderer.material = Materials.NoisemakerOn;
            MegaMeshComponent.MaterialType = MaterialType.NoisemakerOn;

            StartPlaying();
        }
        else if (!Input.On)
        {
            Renderer.material = Materials.NoisemakerOff;
            MegaMeshComponent.MaterialType = MaterialType.NoisemakerOff;

            StopPlaying();
        }
    }

    public void StartPlaying()
    {
        if (!Audio.isPlaying)
        {
            Audio.Play(); FadeVolume(1, 0.05f);
            MusicPlayer.InterruptMusic();
            //Audio.Play(); Audio.loop = true;
        }
    }

    public void StopPlaying()
    {
        if (Audio.isPlaying)
        {
            FadeVolume(0, 0.05f);
            //Audio.loop = false;
        }
    }

    // because it's easier than calling startcoroutine
    private void FadeVolume(float EndVolume, float FadeLength)
    {
        StartCoroutine(FadeVolumeRoutine(EndVolume, FadeLength));
    }

    // we fade the volume like this to avoid pops when it starts and stops
    private IEnumerator FadeVolumeRoutine(float EndVolume, float FadeLength)
    {
        float StartVolume = Audio.volume;

        float StartTime = Time.time;

        while (Time.time < StartTime + FadeLength)
        {
            Audio.volume = StartVolume + ((EndVolume - StartVolume) * ((Time.time - StartTime) / FadeLength));
            yield return null;
        }

        if (EndVolume == 0) { Audio.Stop(); }
        else if (!Audio.isPlaying) { Audio.Play(); Debug.Log("boobs!"); }
    }

    // fuckin hardcore procedural audio stuff

    private void GenerateClip(bool PlayOnceDone)
    {
        Audio.Stop();

        Audio.clip = AudioClip.Create("wave", (int)(44100 / ToneFrequency), 1, 44100, false);

        float[] samples = new float[Audio.clip.samples];
        Audio.clip.SetData(CreateSineWaveData(samples), 0);

        if (PlayOnceDone) { Audio.Play(); }
    }

    private float[] CreateSineWaveData(float[] data)
    {
        for (int i = 0; i < data.Length; i ++)
        {
            data[i] = Mathf.Sin(2 * Mathf.PI * i * (1f / data.Length)) * 0.2f; // amplitude decreased to 20% to prevent sound clipping
        }

        return data;
    }

    // idea: have more functions for differently shaped waves. Squares, triangles, sine sums, look up more types
}