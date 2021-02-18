using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnappedConnection : InputInputConnection
{
	public void Initialize()
    {
        transform.parent = StuffConnector.AppropriateConnectionParent(this);
        Renderer.material.color = Settings.SnappingPegColor;
        MegaMeshComponent MMC = gameObject.AddComponent<MegaMeshComponent>();
        MMC.MaterialType = MaterialType.SnappingPeg;
        MegaMeshManager.AddComponent(MMC);

        // man, all this casting is real annoying
        SnappingPeg SnapInput1 = (SnappingPeg)Input1;
        SnappingPeg SnapInput2 = (SnappingPeg)Input2;
        SnapInput1.SnappedConnection = this;
        SnapInput2.SnappedConnection = this;
    }
}