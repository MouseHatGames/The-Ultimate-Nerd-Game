// this is a placeholder for settings and stuff until I can load stuff from a config file

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiscellaneousSettings {

    // the distance the player can reach
    public static float ReachDistance = 20;

    // the maximum distance of a wire
    public static float WireDistance = 40;

    // the time the placement menu stays open for when a quick action is performed i.e. scrolling
    public static float PlaceMenuOpenTime = 0.8f;

    // some colors for common stuff
    public static Color CircuitOnColor = Color.red;
    public static Color CircuitOffColor = Color.black;
    public static Color InteractableColor = new Color32(109, 53, 6, 255);
    public static Color DisplayOnColor = Colors.VividYellow;
    public static Color DisplayOffColor = Colors.CoolGrey;
}