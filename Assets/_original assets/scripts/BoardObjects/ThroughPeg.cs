// this script is a stupid hack. I should really just give inputs multiple connection points.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThroughPeg : MonoBehaviour {

	// Use this for initialization
	void Awake () {
        ChildInputs = GetComponentsInChildren<CircuitInput>();
	}
    CircuitInput[] ChildInputs;
    private void Start()
    {
        StuffConnecter.CreateIIConnection(ChildInputs[0], ChildInputs[1], true);
    }

}
