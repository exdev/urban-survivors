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
using System.Collections.Generic;

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

    protected List<Object> existObjects = new List<Object>();

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public virtual void DoSpawn () {}
    public void DoKill () {
        foreach ( Object obj in existObjects ) {
            Destroy(obj);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public int calcSpawnAmount () {
        // never exec the code when we reach the limitation.
        if ( totalAmount == 0 )
            return 0;

        for ( int i = 0; i < existObjects.Count; ++i ) {
            Object obj = existObjects[i];
            if ( obj == null ) {
                existObjects.RemoveAt(i);
                --i;
            }
        }

        if ( existObjects.Count < maxAlive ) {
            int remain = maxAlive - existObjects.Count; 
            int amount = Random.Range( minAlive, remain );
            if ( totalAmount != -1 ) {
                amount = Mathf.Min(totalAmount,amount);
                totalAmount = totalAmount - amount;
            }
            return amount;
        }

        return 0;
    }
}
