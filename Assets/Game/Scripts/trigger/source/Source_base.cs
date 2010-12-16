// ======================================================================================
// File         : Source_base.cs
// Author       : Wu Jie 
// Last Change  : 10/31/2010 | 00:29:56 AM | Sunday,October
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////
// Response_base
///////////////////////////////////////////////////////////////////////////////

public class Source_base : MonoBehaviour {

    protected Response_base[] ResponseList;

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public int TriggerTimes = -1; // -1 means infinite

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected bool CanTrigger () { return TriggerTimes != 0; }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected virtual void Start () {
        DebugHelper.Assert( TriggerTimes != 0, "TriggerTimes can't be zero. pls set -1 or number large than zero." );

        // get all response for precache
        ResponseList = GetComponents<Response_base>();
        DebugHelper.Assert( ResponseList.Length > 0, "There isn't any Response component in your script" );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected void Response () {
        // execute each response
        foreach( Response_base response in ResponseList ) {
            response.exec();
        }
        // no need to minus trigger times if it is less than zero.
        TriggerTimes = TriggerTimes > 0 ? --TriggerTimes : TriggerTimes;
    }

}

