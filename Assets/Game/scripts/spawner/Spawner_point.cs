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

    public int totalAmount = -1;
    public int minAlive = 1; // The minimum number of pawns that will be spawned every time the code execute.
    public int maxAlive = 1; // The maximum number of pawns that will be spawned every time the code execute.

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public override void exec () {
        // never exec the code when we reach the limitation.
        if ( totalAmount == 0 )
            return;

        //
        int amount = Random.Range( minAlive, maxAlive );
        if ( totalAmount != -1 ) {
            amount = Mathf.Min(totalAmount,amount);
            totalAmount = totalAmount - amount;
        }

        //
        while ( amount > 0 ) {
            Instantiate( Prefab, transform.position, transform.rotation );
            --amount;
        }
    }
}
