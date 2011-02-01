// ======================================================================================
// File         : Spawner_zone.cs
// Author       : Wu Jie 
// Last Change  : 12/20/2010 | 16:33:42 PM | Monday,December
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////
// defines
///////////////////////////////////////////////////////////////////////////////

public class Spawner_zone : Spawner_base {

    public Vector3 size = new Vector3(10.0f, 0.0f, 10.0f );

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public override void DoSpawn () {
        int amount = calcSpawnAmount();
        Object obj;
        while ( amount > 0 ) {
            obj = Instantiate( Prefab, 
                               transform.position + new Vector3( Random.Range( -size.x * 0.5f, size.x * 0.5f ),
                                                                 Random.Range( -size.y * 0.5f, size.y * 0.5f ),
                                                                 Random.Range( -size.z * 0.5f, size.z * 0.5f ) ) , 
                               transform.rotation );
            this.existObjects.Add(obj);
            --amount;
        }
    }
}

