using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour {

    public AudioClip[] Tracks;

    public AudioSource musicsource;

	void Start () {
        StartCoroutine(PlayMusic());

        // set these values if they don't exist in files
        if (!ES3.KeyExists("TimeBetweenMusic", "settings.txt")) { ES3.Save<float>("TimeBetweenMusic", 100f, "settings.txt"); }
	}
	
	private IEnumerator PlayMusic()
    {
        float TimeBetweenMusic = ES3.Load<float>("TimeBetweenMusic", "settings.txt", 30f);

        while (true)
        {
            yield return new WaitForSeconds(TimeBetweenMusic); // realtime is NOT used so that new tracks don't play when the game is paused

            AudioClip NewClip = Tracks[Random.Range(0, Tracks.Length)]; // array[array.length - 1] returns the last value in it and Random.Range is max value exclusive
            musicsource.clip = NewClip;
            musicsource.Play();
            Debug.Log("new clip playing");

            yield return new WaitForSecondsRealtime(NewClip.length); // realtime is used so that pausing the game doesn't fuck up the counting
        }
    }
}
