// other scripts on the object should reference this one for detecting interaction
// shortest script ever. Wowee.

using UnityEngine;

public class Interactable : MonoBehaviour {

    // if interactable objects want to stop being interacted with, they can set this to false during the cycle they take action
    public bool Interacted;

}
