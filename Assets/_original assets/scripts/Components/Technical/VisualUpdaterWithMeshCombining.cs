using UnityEngine;

public abstract class VisualUpdaterWithMeshCombining : MonoBehaviour
{
    protected MeshFilter MeshFilter; // VERY IMPORTANT - THIS MUST BE A MESHFILTER, NOT A MESH. DO NOT ASK ME WHY. I DO NOT KNOW. I JUST KNOW that it causes BIG SCARY HARD TO TRACE BUGS
    protected Renderer Renderer;
    protected MegaMeshComponent MegaMeshComponent;
    public Renderer GetRenderer { get { return Renderer; } }

    protected void Awake()
    {
        Renderer = GetComponent<Renderer>();
        MeshFilter = GetComponent<MeshFilter>();
        MegaMeshComponent = GetComponent<MegaMeshComponent>();

        QueueVisualUpdate();

        AfterAwake();
    }
    protected virtual void AfterAwake() { } // if the inherited class needs to do anything on awake it can do so here

    protected float StableTime; // how long the visuals of the object have been stable
    public bool AllowedToCombineOnStable = true;

    public abstract void VisualUpdate(); // run every frame the class in BehaviorManager.UpdatingVisually. This should handle triggering of VisualsChanged and VisualsHaventChanged

    public void ForceVisualRefresh() { VisualsChanged(); } // some cases where visuals need to be updated but they won't be triggered when running VisualUpdate

    protected void VisualsChanged()
    {
        SetProperMaterialAndMegaMeshComponentMaterialType();
        MegaMeshManager.RemoveComponentStaggered(MegaMeshComponent);
        StableTime = 0;
    }

    protected void VisualsHaventChanged()
    {
        StableTime += Time.deltaTime;

        if (StableTime > Settings.StableCircuitTime && AllowedToCombineOnStable)
        {
            if (gameObject.layer != 5) { MegaMeshManager.AddComponent(MegaMeshComponent); } // don't combine in the UI
            DoneVisualUpdate();
        }
    }

    protected abstract void SetProperMaterialAndMegaMeshComponentMaterialType();

    bool VisualUpdateQueued = false;
    public void QueueVisualUpdate()
    {
        if (VisualUpdateQueued) { return; }

        BehaviorManager.CurrentlyUpdatingVisually.Add(this);
        VisualUpdateQueued = true;
        FinishedUpdating = false;
    }

    public void DoneVisualUpdate()
    {
        FinishedUpdating = true;
        VisualUpdateQueued = false;
    }
    public bool FinishedUpdating;

    protected void OnDestroy()
    {
        DoneVisualUpdate();
        MegaMeshManager.RemoveComponentImmediately(MegaMeshComponent);
        WhenDestroyed();
    }
    protected virtual void WhenDestroyed() { }
}