// ======================================================================================
// File         : Spawner_base.cs
// Author       : Wu Jie 
// Last Change  : 11/07/2010 | 19:28:22 PM | Sunday,November
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

public class Spawner_base : MonoBehaviour {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public GameObject Prefab;
    public int totalAmount = -1;
    public int minAlive = 1; // The minimum number of pawns that will be spawned every time the code execute.
    public int maxAlive = 1; // The maximum number of pawns that will be spawned every time the code execute.

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public virtual void exec () {}

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public int calcSpawnAmount () {
        // never exec the code when we reach the limitation.
        if ( totalAmount == 0 )
            return 0;

        int amount = Random.Range( minAlive, maxAlive );
        if ( totalAmount != -1 ) {
            amount = Mathf.Min(totalAmount,amount);
            totalAmount = totalAmount - amount;
        }
        return amount;
    }

}
