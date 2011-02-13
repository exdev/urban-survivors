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

    public override void DoSpawn () {
        // check if the character is too close to spawn point 
        Vector3 boyPos = GameRules.Instance().GetPlayerBoy().transform.position;
        Vector3 girlPos = GameRules.Instance().GetPlayerGirl().transform.position;
        float dist = GameRules.Instance().spawnDistance;
        float sqrSpawnDistance = dist * dist;
        if ( (this.transform.position - boyPos).sqrMagnitude < sqrSpawnDistance ||
             (this.transform.position - girlPos).sqrMagnitude < sqrSpawnDistance )
            return;

        //
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
