// this might be the only file I've ever made with out any "using namespace" thingies at the top. Neato.

public class CircuitUpdateToVisualUpdateInterface : CircuitLogicComponent {

    public VisualUpdaterWithMeshCombining visualboi;

    protected override void CircuitLogicUpdate()
    {
        visualboi.QueueVisualUpdate();
    }
}