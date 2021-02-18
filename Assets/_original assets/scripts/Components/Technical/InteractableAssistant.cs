// a small assistant script to allow multiple hitboxes for interactables that are on separate game objects

public class InteractableAssistant : Interactable
{
    public Interactable TargetInteractable;

    public override void Interact()
    {
        TargetInteractable.Interact();
    }
}