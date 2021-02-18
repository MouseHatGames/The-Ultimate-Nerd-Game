using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using References;

public class Display : VisualUpdaterWithMeshCombining
{
    public CircuitInput Input; // must be assigned in editor

    public DisplayColor DisplayColor;
    public void NewlyPlaced()
    {
        DisplayColor = EditDisplayColorMenu.ColorOfNewDisplays;
    }

    protected override void AfterAwake()
    {
        MegaMeshComponent.Mesh = MeshFilter.sharedMesh;
        MegaMeshManager.ScaleMesh(MeshFilter, 1.005f);
    }

    private void Start()
    {
        // I have absolutely no fucking clue why this works.
        // However, if the method is called directly instead of via Invoke, it will appear off even while on for boards created during the initial period of Stack Board.
        Invoke("SetProperMaterialAndMegaMeshComponentMaterialType", 1);

        // also done right at the start, for obvious reasons
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
        if (!Input.On) { MegaMeshComponent.MaterialType = MaterialType.DisplayOff; Renderer.material = Materials.DisplayOff; }
        else { SetOnColorAndMaterialType(); }
    }

    protected void SetOnColorAndMaterialType()
    {
        switch (DisplayColor)
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