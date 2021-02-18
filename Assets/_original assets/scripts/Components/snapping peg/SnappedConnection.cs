using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnappedConnection : InputInputConnection
{
    public override Material DefaultMaterial { get { return References.Materials.SnappingPeg; } }

    public void Initialize()
    {
        transform.parent = StuffConnector.AppropriateConnectionParent(this);

        MegaMeshComponent MMC = gameObject.AddComponent<MegaMeshComponent>();
        MMC.MaterialType = MaterialType.SnappingPeg;
        MegaMeshManager.AddComponent(MMC);

        SetThisAsSnappedConnectionOfPegs();
    }

    public void SetThisAsSnappedConnectionOfPegs()
    {
        // man, all this casting is real annoying
        SnappingPeg SnapInput1 = (SnappingPeg)Input1;
        SnappingPeg SnapInput2 = (SnappingPeg)Input2;
        SnapInput1.SnappedConnection = this;
        SnapInput2.SnappedConnection = this;
    }
}