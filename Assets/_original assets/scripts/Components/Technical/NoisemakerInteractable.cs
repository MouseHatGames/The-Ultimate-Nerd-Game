// hopefully ECS will put an end to all this bull

public class NoisemakerInteractable : Interactable
{
    private Noisemaker noisemaker;
    private void Awake()
    {
        noisemaker = GetComponent<Noisemaker>();
    }

    public override void Interact()
    {
        NoisemakerMenu.NoisemakerBeingEdited = noisemaker;
        NoisemakerMenu.Instance.InitiateMenu();
    }
}