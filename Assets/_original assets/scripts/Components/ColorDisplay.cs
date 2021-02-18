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
        MegaMeshManager.ScaleMesh(MeshFilter, 1.0005f);
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
                Renderer.material = Materials.DisplayOff;
                MegaMeshComponent.MaterialType = MaterialType.DisplayOff;
                break;

            case DisplayColor.Red:
                Renderer.material = Materials.DisplayRed;
                MegaMeshComponent.MaterialType = MaterialType.DisplayRed;
                break;

            case DisplayColor.Green:
                Renderer.material = Materials.DisplayGreen;
                MegaMeshComponent.MaterialType = MaterialType.DisplayGreen;
                break;

            case DisplayColor.Blue:
                Renderer.material = Materials.DisplayBlue;
                MegaMeshComponent.MaterialType = MaterialType.DisplayBlue;
                break;

            case DisplayColor.Yellow:
                Renderer.material = Materials.DisplayYellow;
                MegaMeshComponent.MaterialType = MaterialType.DisplayYellow;
                break;

            case DisplayColor.Orange:
                Renderer.material = Materials.DisplayOrange;
                MegaMeshComponent.MaterialType = MaterialType.DisplayOrange;
                break;

            case DisplayColor.Purple:
                Renderer.material = Materials.DisplayPurple;
                MegaMeshComponent.MaterialType = MaterialType.DisplayPurple;
                break;

            case DisplayColor.White:
                Renderer.material = Materials.DisplayWhite;
                MegaMeshComponent.MaterialType = MaterialType.DisplayWhite;
                break;

            case DisplayColor.Cyan:
                Renderer.material = Materials.DisplayCyan;
                MegaMeshComponent.MaterialType = MaterialType.DisplayCyan;
                break;
        }
    }
}