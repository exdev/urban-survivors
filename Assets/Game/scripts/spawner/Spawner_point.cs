// ======================================================================================
// File         : Spawner_point.cs
// Author       : Wu Jie 
// Last Change  : 11/07/2010 | 19:33:56 PM | Sunday,November
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

public class Spawner_point : Spawner_base {

    public float radius = 0.0f;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public override void exec () {
        int amount = calcSpawnAmount();
        Object obj;
        while ( amount > 0 ) {
            Vector2 offset = Random.insideUnitCircle * this.radius;
            obj = Instantiate( Prefab, 
                               transform.position + new Vector3( offset.x, 0.0f, offset.y ),
                               transform.rotation );
            this.existObjects.Add(obj);
            --amount;
        }
    }
}
