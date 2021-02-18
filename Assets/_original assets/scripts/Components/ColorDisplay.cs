using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using References;

public class ColorDisplay : VisualUpdaterWithMeshCombining
{
    public CircuitInput RedInput;
    public CircuitInput BlueInput;
    public CircuitInput YellowInput;

    public DisplayColor InputStates
    {
        get
        {
            if (!RedInput.On && !BlueInput.On && !YellowInput.On) { return DisplayColor.Off; }
            if (!RedInput.On && !BlueInput.On && YellowInput.On) { return DisplayColor.Yellow; }
            if (!RedInput.On && BlueInput.On && !YellowInput.On) { return DisplayColor.Blue; }
            if (!RedInput.On && BlueInput.On && YellowInput.On) { return DisplayColor.Green; }
            if (RedInput.On && !BlueInput.On && !YellowInput.On) { return DisplayColor.Red; }
            if (RedInput.On && !BlueInput.On && YellowInput.On) { return DisplayColor.Orange; }
            if (RedInput.On && BlueInput.On && !YellowInput.On) { return DisplayColor.Purple; }

            return DisplayColor.White;
        }
    }

    protected override void AfterAwake()
    {
        Renderer.material.color = Settings.DisplayOffColor;

        MegaMeshComponent.Mesh = MeshFilter.sharedMesh;
        MegaMeshManager.ScaleMesh(MeshFilter, 1.005f);
    }

    [SerializeField] DisplayColor PreviousInputStates; // serialized so it can be copied in cloneboard
    public override void VisualUpdate()
    {
        if (PreviousInputStates != InputStates)
        {
            VisualsChanged();
            PreviousInputStates = InputStates;
        }
        else
        {
            VisualsHaventChanged();
        }
    }

    protected override void SetProperMaterialAndMegaMeshComponentMaterialType()
    {
        DisplayColor CurrentState = InputStates;

        switch (CurrentState)
        {
            case DisplayColor.Off:
                Renderer.material = Materials.DisplayOffAlwaysOnTop;
                MegaMeshComponent.MaterialType = MaterialType.DisplayOff;
                break;

            case DisplayColor.Red:
                Renderer.material = Materials.DisplayRedAlwaysOnTop;
                MegaMeshComponent.MaterialType = MaterialType.DisplayRed;
                break;

            case DisplayColor.Green:
                Renderer.material = Materials.DisplayGreenAlwaysOnTop;
                MegaMeshComponent.MaterialType = MaterialType.DisplayGreen;
                break;

            case DisplayColor.Blue:
                Renderer.material = Materials.DisplayBlueAlwaysOnTop;
                MegaMeshComponent.MaterialType = MaterialType.DisplayBlue;
                break;

            case DisplayColor.Yellow:
                Renderer.material = Materials.DisplayYellowAlwaysOnTop;
                MegaMeshComponent.MaterialType = MaterialType.DisplayYellow;
                break;

            case DisplayColor.Orange:
                Renderer.material = Materials.DisplayOrangeAlwaysOnTop;
                MegaMeshComponent.MaterialType = MaterialType.DisplayOrange;
                break;

            case DisplayColor.Purple:
                Renderer.material = Materials.DisplayPurpleAlwaysOnTop;
                MegaMeshComponent.MaterialType = MaterialType.DisplayPurple;
                break;

            case DisplayColor.White:
                Renderer.material = Materials.DisplayWhiteAlwaysOnTop;
                MegaMeshComponent.MaterialType = MaterialType.DisplayWhite;
                break;

            case DisplayColor.Cyan:
                Renderer.material = Materials.DisplayCyanAlwaysOnTop;
                MegaMeshComponent.MaterialType = MaterialType.DisplayCyan;
                break;
        }
    }
}