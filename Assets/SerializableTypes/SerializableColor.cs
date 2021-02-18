// this is actually an original asset, I made this based on the other serializable components.
// of note: this type does not have transparency

using UnityEngine;
using System;
using System.Collections;

[System.Serializable]
public struct SerializableColor
{
    public float r;

    public float g;

    public float b;

    public SerializableColor(float rR, float rG, float rB)
    {
        r = rR;
        g = rG;
        b = rB;
    }

    public SerializableColor(Color rValue)
    {
        r = rValue.r;
        g = rValue.g;
        b = rValue.b;
    }

    public override string ToString()
    {
        return String.Format("[{0}, {1}, {2}]", r, g, b);
    }

    public static implicit operator Color(SerializableColor rValue)
    {
        return new Color(rValue.r, rValue.g, rValue.b);
    }

    public static implicit operator SerializableColor(Color rValue)
    {
        return new SerializableColor(rValue.r, rValue.g, rValue.b);
    }
}