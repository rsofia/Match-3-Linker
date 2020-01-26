using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObstacle : TileBase
{
    public override void OnMerge(int stackIndex)
    {
        return; //cannot merge
        //base.OnMerge(stackIndex);
    }

    protected override void OnMouseOver()
    {
        //do nothing because obstacles cannot be merged.
    }
}
