// ======================================================================================
// File         : Source_periodic.cs
// Author       : Wu Jie 
// Last Change  : 10/30/2010 | 01:11:47 AM | Saturday,October
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////
// class 
///////////////////////////////////////////////////////////////////////////////

public class Source_periodic : MonoBehaviour {

    Response_base[] ResponseList;

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public float StartTime = 0.0f;
    public float IntervalTime = 0.0f;
    public int TriggerTimes = -1; // -1 means infinite

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	void Start () {
        DebugHelper.Assert( TriggerTimes != 0, "TriggerTimes can't be zero. pls set -1 or number large than zero." );

        // get all response for precache
        ResponseList = GetComponents<Response_base>();
        DebugHelper.Assert( ResponseList.Length > 0, "There isn't any Response component in your script" );

        //
        StartCoroutine(StartCounter());
	}

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    IEnumerator StartCounter () {
        // if start time is zero, no need to wait.
        if ( StartTime > 0.0f ) {
            yield return new WaitForSeconds (StartTime);
        }

        // execute each response
        foreach( Response_base response in ResponseList ) {
            response.exec();
        }
        --TriggerTimes; // The start time trigger counted as one Time in TriggerTimes.

        // now enter the trigger loops
        while ( TriggerTimes != 0 ) {
            yield return new WaitForSeconds (IntervalTime);

            // execute each response
            foreach( Response_base response in ResponseList ) {
                response.exec();
            }

            // no need to minus trigger times if it is less than zero.
            TriggerTimes = TriggerTimes > 0 ? --TriggerTimes : TriggerTimes;
        }
    }
}
