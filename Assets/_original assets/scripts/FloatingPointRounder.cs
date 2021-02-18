// the purpose of this class is to combat the floating point errors that continuously accumulate because computers are assholes. Attach it to components and to boards.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingPointRounder : MonoBehaviour
{
	public void RoundPositionAndRotation()
    {
        // round position to nearest thousandth of a unit, or millimeter
        var position = transform.localPosition;
        position.x = Mathf.Round(position.x / 0.001f) * 0.001f;
        position.y = Mathf.Round(position.y / 0.001f) * 0.001f;
        position.z = Mathf.Round(position.z / 0.001f) * 0.001f;
        transform.localPosition = position;

        // round rotation to nearest tenth of a degree
        var rotation = transform.localEulerAngles;
        rotation.x = Mathf.Round(rotation.x / 0.001f) * 0.001f;
        rotation.y = Mathf.Round(rotation.y / 0.001f) * 0.001f;
        rotation.z = Mathf.Round(rotation.z / 0.001f) * 0.001f;
        transform.localEulerAngles = rotation;
    }

    public void RoundScale()
    {
        var scale = transform.localScale;
        scale.x = Mathf.Round(scale.x / 0.001f) * 0.001f;
        scale.y = Mathf.Round(scale.y / 0.001f) * 0.001f;
        scale.z = Mathf.Round(scale.z / 0.001f) * 0.001f;
        transform.localScale = scale;
    }

    public static void RoundIn(GameObject bigboi, bool RoundScale = false)
    {
        FloatingPointRounder[] boizzz = bigboi.GetComponentsInChildren<FloatingPointRounder>();

        foreach (FloatingPointRounder boi in boizzz)
        {
            if (RoundScale) { boi.RoundScale(); }
            boi.RoundPositionAndRotation();
        }
    }
}