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
using System.Collections.Generic;

///////////////////////////////////////////////////////////////////////////////
// class 
///////////////////////////////////////////////////////////////////////////////

[RequireComponent (typeof (BoxCollider))]
public class Source_collider : Source_base {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public List<string> targetTags = new List<string>();

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected override void Start () {
        base.Start();
        Collider collider = gameObject.GetComponent(typeof(Collider)) as Collider;
        DebugHelper.Assert( collider.isTrigger == true, "the collider must set as a trigger" );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnTriggerEnter ( Collider _other ) {
        bool isInTags = false;
        if ( targetTags.Count != 0 ) {
            foreach ( string tag in targetTags ) {
                if ( _other.gameObject.tag == tag ) {
                    isInTags = true;
                    break;
                }
            }
        }
        else { // none means all.
            isInTags = true;
        }

        //
        if ( isInTags && base.CanTrigger() ) {
            base.Response();
        }
    }

}; // end class
