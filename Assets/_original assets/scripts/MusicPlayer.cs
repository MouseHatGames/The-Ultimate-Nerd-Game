using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioClip[] Tracks;

    public AudioSource musicsource;

	void Start ()
    {
        StartCoroutine(PlayMusic());
        DontInterruptYet = false;
	}

	private IEnumerator PlayMusic()
    {
        float TimeBetweenMusic = Settings.Get("TimeBetweenMusic", 200f);

        while (true)
        {
            DontInterruptYet = true;

            yield return new WaitForSecondsRealtime(TimeBetweenMusic / 2);

            DontInterruptYet = false;

            yield return new WaitForSecondsRealtime(TimeBetweenMusic / 2);

            AudioClip NewClip = Tracks[NewRandomSongID()];
            musicsource.clip = NewClip;
            musicsource.Play();
            Debug.Log("new clip playing");

            yield return new WaitForSecondsRealtime(NewClip.length); // realtime is used so that pausing the game doesn't fuck up the counting
        }
    }

    // makes sure you can't get the same song twice in a row
    private int CurrentlyPlayingSongID = -1;
    private int NewRandomSongID()
    {
        int newid = CurrentlyPlayingSongID;
        while (newid == CurrentlyPlayingSongID || newid < 0)
        {
            newid = Random.Range(0, Tracks.Length); // array[array.length - 1] returns the last value in it and Random.Range is max value exclusive
        }

        CurrentlyPlayingSongID = newid;
        return newid;
    }

    private static MusicPlayer Instance;
    private static bool DontInterruptYet;
    private void Awake()
    {
        Instance = this;
        AllowMusicInterruption = Settings.Get("AllowNoisemakersToInterruptMusic", true);
    }

    // used by noisemakers so they can stop the music
    private static bool AllowMusicInterruption;
    public static void InterruptMusic()
    {
        if (!AllowMusicInterruption) { return; }
        if (DontInterruptYet) { return; } // the stuff after this is pretty expensive, so we avoid it being called a ton when there are a lot of noisemakers running.

        Instance.musicsource.Stop();
        Instance.StopAllCoroutines();
        Instance.StartCoroutine(Instance.PlayMusic());
    }
}