// attach to objects you want to save

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class SaveThisObject : MonoBehaviour { // monobehavior instance that gets attached to gameobjects

    public string ObjectType; // could be CircuitBoard, NotGate, Peg, ect

    public object[] CustomDataArray;

    // transform data
    public Vector3 LocalPosition;
    public Vector3 LocalEulerAngles; // use euler angles instead of quaternion rotation for more readable save files

    private void Start()
    {
        SaveManager.SaveObjects.Add(this);
    }
    private void OnDestroy()
    {
        SaveManager.SaveObjects.Remove(this);
    }

    public SavedObject Persistent()
    {
        SavedObject persistentsave = new SavedObject();

        persistentsave.ObjectType = ObjectType;
        persistentsave.CustomDataArray = CustomData.SaveObjectData(this); // different for each object type

        // grab current transform values
        persistentsave.LocalPosition = transform.localPosition;
        persistentsave.LocalEulerAngles = transform.localEulerAngles;

        persistentsave.Children = FindChildSaves();

        return persistentsave;
    }

    public void LoadSaveData()
    {
        CustomData.LoadData(this);

        // transform values
        transform.localPosition = LocalPosition;
        transform.localEulerAngles = LocalEulerAngles;
    }

    // get all the SaveOdbjects in the immediate children of the SaveObject
    public SavedObject[] FindChildSaves()
    {
        List<SavedObject> childsaves = new List<SavedObject>();

        for (int i = 0; i < transform.childCount; i++)
        {
            SaveThisObject child = transform.GetChild(i).GetComponent<SaveThisObject>();
            if (child != null) { childsaves.Add(child.Persistent()); }
        }

        return childsaves.ToArray();
    }

    private void OnValidate() // so that it automatically grabs the name when added to a prefab
    {
        ObjectType = gameObject.name;
    }
}

public class SavedObject // we need a non-monobehavior version that is saved and loaded without needing to be attached to a gameobject
{
    public string ObjectType; // could be CircuitBoard, NotGate, Peg, ect

    public SavedObject[] Children;

    public object[] CustomDataArray;

    // transform data
    public Vector3 LocalPosition;
    public Vector3 LocalEulerAngles; // use euler angles instead of quaternion rotation for more readable save files
}
