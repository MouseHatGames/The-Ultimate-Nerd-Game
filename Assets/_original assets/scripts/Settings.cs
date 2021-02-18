// utility class for managing settings.txt
// this is the first static class I ever coded! There were lots of other things that SHOULD have been static classes, but I didn't know you could do this until just now :)

using UnityEngine;

public static class Settings {

    static string SettingsPath = "settings.txt";

    // returns a value from settings.txt, creating it if it doesn't exist.
    // important: this method cannot be called from an instance field initializer on a monobehavior, use awake there instead. It works fine on non-monobehaviors
	public static T Get<T>(string key, T DefaultValue)
    {
        if(ES3.KeyExists(key, SettingsPath)) // if the key exists already,
        {
            return ES3.Load(key, SettingsPath, DefaultValue); // load it from settings.txt.
        }

        else // if the key does not exist,
        {
            ES3.Save<T>(key, DefaultValue, SettingsPath); // create it
            return DefaultValue; // and return the default value.
        }
    }

    // saves a value to settings
    public static void Save<T>(string key, T value)
    {
        ES3.Save<T>(key, value, SettingsPath);
    }


    // static variables that are referenced a lot by other stuff
    // any variable that is only referenced by one class should be stored in that class, not here

    // the distance the player can reach
    public static readonly float ReachDistance = Get("ReachDistance", 20f);

    // how long, in seconds, an output or cluster can be the same state before being combined into a big fat mesh
    public static readonly float StableCircuitTime = Get("StableCircuitTime", 0.25f);

    // stuff beyond this point is non-configurable

    // some colors for common stuff
    public static readonly Color CircuitOnColor = Color.red;
    public static readonly Color CircuitOffColor = Color.black;
    public static readonly Color InteractableColor = new Color32(109, 53, 6, 255);

    public static readonly Color DisplayOffColor = new Color32(32, 32, 32, 255);
    public static readonly Color DisplayRedColor = new Color32(186, 0, 0, 255);
    public static readonly Color DisplayGreenColor = new Color32(20, 150, 0, 255);
    public static readonly Color DisplayBlueColor = new Color32(0, 50, 200, 255);
    public static readonly Color DisplayYellowColor = Colors.VividYellow;
    public static readonly Color DisplayOrangeColor = Colors.VividOrange;
    public static readonly Color DisplayPurpleColor = new Color32(142, 18, 255, 255);
    public static readonly Color DisplayWhiteColor = new Color32(200, 200, 210, 255);

    public static readonly Color DisplayCyanColor = new Color32(0, 219, 206, 255); // a relic from early 0.2 development when color displays were RGB instead of RBY. I'm keeping the color here because I think it's a really pretty color :)

    public static readonly Color NoisemakerOffColor = new Color32(56, 22, 120, 255);
    public static readonly Color NoisemakerOnColor = new Color32(168, 127, 223, 255);

    public static readonly Color SnappingPegColor = new Color32(0, 150, 141, 255);

    // the maximum possible length of a wire
    public static readonly float MaxWireLength = 40;
}