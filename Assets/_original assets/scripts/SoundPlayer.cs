// this class is designed with multiplayer in mind. Sounds in world space are heard by all players; "global sounds" are only heard
// by the player that initiates them

using UnityEngine;
using UnityEngine.Audio;

public class SoundPlayer : MonoBehaviour
{
    public static SoundPlayer Instance;
    private void Awake()
    {
        Instance = this;
    }

    public AudioSource GlobalPlayer;

    public AudioMixerGroup SFXGroup;

    public static void PlaySoundGlobal(AudioClip clip)
    {
        if (clip == null) { return; }
        Instance.GlobalPlayer.PlayOneShot(clip);
    }

    public static void PlaySoundGlobal(AudioClip[] clips)
    {
        PlaySoundGlobal(clips[Random.Range(0, clips.Length)]);
    }

    public static void PlaySoundAt(AudioClip clip, Vector3 position)
    {
        if (clip == null) { return; }

        GameObject PlayingBoi = new GameObject("Playing Sound");
        PlayingBoi.transform.position = position;

        AudioSource SoundSource = PlayingBoi.AddComponent<AudioSource>();
        SoundSource.clip = clip;
        SoundSource.outputAudioMixerGroup = Instance.SFXGroup;
        SoundSource.spatialBlend = 1; // the whole point of this is that it's 3D sound

        SoundSource.Play();
        Destroy(PlayingBoi, clip.length); // destroy after clip duration
    }

    public static void PlaySoundAt(AudioClip clip, Transform position)
    {
        PlaySoundAt(clip, position.position);
    }

    public static void PlaySoundAt(AudioClip clip, GameObject position)
    {
        PlaySoundAt(clip, position.transform);
    }


    public static void PlaySoundAt(AudioClip[] clips, Vector3 position)
    {
        PlaySoundAt(clips[Random.Range(0, clips.Length)], position);
    }

    public static void PlaySoundAt(AudioClip[] clips, Transform position)
    {
        PlaySoundAt(clips, position.position);
    }

    public static void PlaySoundAt(AudioClip[] clips, GameObject position)
    {
        PlaySoundAt(clips, position.transform);
    }
}