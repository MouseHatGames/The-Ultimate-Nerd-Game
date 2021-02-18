using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SavedObjects;

namespace SavedObjects
{
    [Serializable]
    public abstract class SavedObjectV2
    {
        // transform data, every saved object needs this
        public SerializableVector3 LocalPosition;
        public SerializableVector3 LocalEulerAngles;

        public SavedObjectV2[] Children;
        public virtual bool CanHaveChildren { get { return false; } }
    }

    [Serializable]
    public class SavedCustomObject : SavedObjectV2
    {
        public object[] CustomData;
        public override bool CanHaveChildren { get { return true; } }
    }

    [Serializable]
    public class SavedCircuitBoard : SavedObjectV2
    {
        public int x;
        public int z;
        public SerializableColor color;

        public override bool CanHaveChildren { get { return true; } }
    }

    [Serializable]
    public class SavedWire : SavedObjectV2
    {
        public bool InputInput;
        public float length;
    }

    [Serializable]
    public class SavedButton : SavedObjectV2
    {
        // buttons have no important data!
    }

    [Serializable]
    public class SavedPanelButton : SavedButton { }

    [Serializable]
    public class SavedDelayer : SavedObjectV2
    {
        public bool OutputOn;
        public int DelayCount;
    }

    [Serializable]
    public class SavedDisplay : SavedObjectV2
    {
        public DisplayColor Color;
    }

    [Serializable]
    public class SavedPanelDisplay : SavedDisplay { }

    [Serializable]
    public class SavedInverter : SavedObjectV2
    {
        public bool OutputOn;
    }

    [Serializable]
    public class SavedLabel : SavedObjectV2
    {
        public string text;
        public float FontSize;
    }

    [Serializable]
    public class SavedPanelLabel : SavedLabel { }

    [Serializable]
    public class SavedSwitch : SavedObjectV2
    {
        public bool on;
    }

    [Serializable]
    public class SavedPanelSwitch : SavedSwitch { }

    [Serializable]
    public class SavedPeg : SavedObjectV2
    {

    }

    [Serializable]
    public class SavedThroughPeg : SavedPeg { }

    [Serializable]
    public class SavedSnappingPeg : SavedPeg { }

    [Serializable]
    public class SavedVerticalSnappingPeg : SavedSnappingPeg { }

    [Serializable]
    public class SavedBlotter : SavedObjectV2
    {
        public bool OutputOn;
    }

    [Serializable]
    public class SavedThroughBlotter : SavedBlotter { }

    [Serializable]
    public class SavedColorDisplay : SavedObjectV2 { }

    [Serializable]
    public class SavedPanelColorDisplay : SavedColorDisplay { }

    [Serializable]
    public class SavedNoisemaker : SavedObjectV2
    {
        public float ToneFrequency;
    }

    [Serializable]
    public class SavedMount: SavedObjectV2
    {
        public override bool CanHaveChildren { get { return true; } }
    }
}