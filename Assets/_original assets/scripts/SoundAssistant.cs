using UnityEngine;
using References;

public class SoundAssistant : MonoBehaviour
{
    private static void PlayUIButtonSoundGlobal()
    {
        SoundPlayer.PlaySoundGlobal(Sounds.UIButton);
    }

    public void PlayUIButtonSound()
    {
        PlayUIButtonSoundGlobal();
    }
}