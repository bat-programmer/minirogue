using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavEnemyEspada : NavEnemyBase
{

    protected override void Start()
    {
        base.Start();

        var forwarder = GetComponentInChildren<AnimationEventForwarder>();
        if (forwarder != null)
        {
            forwarder.parentEnemy = this;
        }
    }

    

}
