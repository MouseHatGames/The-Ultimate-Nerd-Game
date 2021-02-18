using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoisemakerMenu : HorizontalScrollMenuWithSelection
{
    public static NoisemakerMenu Instance;
    private void Awake()
    {
        Instance = this;
    }

    public static Noisemaker NoisemakerBeingEdited;

    // since UI buttons can't directly start coroutines
    public void TestNote()
    {
        StartCoroutine(TestNoteRoutine(NoisemakerBeingEdited));
    }

    private IEnumerator TestNoteRoutine(Noisemaker NoisemakerBeingTested)
    {
        NoisemakerBeingTested.StartPlaying();
        yield return new WaitForSeconds(1);
        if (!NoisemakerBeingTested.Input.On)
        {
            NoisemakerBeingTested.StopPlaying();
        }
    }

    // buttons press this
    public void SetNote(int HalfStepsAboveC)
    {
        NoteInHalfStepsAboveC = HalfStepsAboveC;
        SelectedThing = HalfStepsAboveC;
        UpdateSelectedThing();
        UpdateFrequency();
    }

    public Slider OctavesSlider;
    public TMPro.TextMeshProUGUI OctavesLabel;
    // the slider triggers this when it updates
    public void UpdateOctave()
    {
        OctavesAbove0 = (int)OctavesSlider.value;
        UpdateFrequency();

        OctavesLabel.text = "Octave: " + OctavesSlider.value.ToString();

        GameplayUIManager.DeselectAllUIElements();
    }

    private int NoteInHalfStepsAboveC;
    private int OctavesAbove0;

    private void UpdateFrequency()
    {
        float newfrequency =
            32.70f // C1, the lowest note a noisemaker can play
            * Mathf.Pow(
            1.059463094359f, // a very important mathematical constant!
            12 * OctavesAbove0 + NoteInHalfStepsAboveC); // the total half steps above C1

        NoisemakerBeingEdited.ToneFrequency = newfrequency;
    }

    public static float FrequencyForNewNoisemakers = 440;

    public void RunNoisemakerMenu()
    {
        if (SelectedThingJustChanged)
        {
            SetNote(SelectedThing);
        }

        // when scrolling past the end of the notes, change the octave if appropriate
        if(GameplayUIManager.ScrollUp() && SelectedThing == 0)
        {
            OctavesSlider.value--; // will automatically prevent us from going below the min
        }
        if(GameplayUIManager.ScrollDown() && SelectedThing == 11)
        {
            OctavesSlider.value++;
        }

        if (Input.GetButton("Mod"))
        {
            if (GameplayUIManager.ScrollDown()) { OctavesSlider.value--; }
            if (GameplayUIManager.ScrollUp()) { OctavesSlider.value++; }
        }
        else
        {
            ScrollThroughMenu();
        }

        // hotkeys
        if (Input.GetButtonDown("Jump") || Input.GetButtonDown("Submit") || Input.GetButtonDown("Confirm")) { Done(); }

        if (Input.GetButtonDown("Cancel"))
        {
            // revert, and close the menu without setting a new FrequencyForNewNoisemakers
            Revert();
            Canvas.enabled = false;
            GameplayUIManager.UIState = UIState.None;
        }

        PollForKeyCodes();
    }

    private void PollForKeyCodes()
    {
        bool Shifting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        if (Input.GetKeyDown(KeyCode.C))
        {
            SelectedThing = !Shifting ? 0 : 1;
            UpdateSelectedThing();
        }

        else if (Input.GetKeyDown(KeyCode.D))
        {
            SelectedThing = !Shifting ? 2 : 3;
            UpdateSelectedThing();
        }

        else if (Input.GetKeyDown(KeyCode.E))
        {
            SelectedThing = !Shifting ? 4 : 5;
            UpdateSelectedThing();
        }

        else if (Input.GetKeyDown(KeyCode.F))
        {
            SelectedThing = !Shifting ? 5 : 6;
            UpdateSelectedThing();
        }

        else if (Input.GetKeyDown(KeyCode.G))
        {
            SelectedThing = !Shifting ? 7 : 8;
            UpdateSelectedThing();
        }

        else if (Input.GetKeyDown(KeyCode.A))
        {
            SelectedThing = !Shifting ? 9 : 10;
            UpdateSelectedThing();
        }

        else if (Input.GetKeyDown(KeyCode.B))
        {
            SelectedThing = !Shifting ? 11 : 0;
            UpdateSelectedThing();
        }
    }

    private static float InitialFrequency;

    public void InitiateMenu()
    {
        Canvas.enabled = true;
        GameplayUIManager.UIState = UIState.NoisemakerMenu;

        SetUIBasedOnFrequency(NoisemakerBeingEdited.ToneFrequency);
        InitialFrequency = NoisemakerBeingEdited.ToneFrequency;
    }

    private void SetUIBasedOnFrequency(float Frequency)
    {
        // make the UI reflect the note
        // I legitimately have no idea why this works
        float ilovemygirlfriend = Mathf.Log(Frequency, 1.059463094359f);

        OctavesAbove0 = (int)ilovemygirlfriend / 12 - 5;
        NoteInHalfStepsAboveC = (int)ilovemygirlfriend % 12;

        SetNote(NoteInHalfStepsAboveC);
        OctavesSlider.value = OctavesAbove0;
        UpdateOctave();
    }

    public void Revert()
    {
        SetUIBasedOnFrequency(InitialFrequency);
    }

    public void Done()
    {
        Canvas.enabled = false;
        GameplayUIManager.UIState = UIState.None;

        StuffPlacer.DeleteThingBeingPlaced(); // quick shitty fix for FrequencyForNewNoisemakers not working properly
        FrequencyForNewNoisemakers = NoisemakerBeingEdited.ToneFrequency; // only update this variable during this method, so players can check notes without screwing it up
    }
}