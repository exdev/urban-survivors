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
    public bool triggerWhenEnter = false; 
    public bool triggerWhenExit = false; 
    public float stayForSeconds = -1.0f; 
    public float stayInterval = 1.0f; 

    protected int numTargetInside = 0;

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

    bool IsTargetInTags ( GameObject _go ) {
        bool isInTags = false;
        if ( targetTags.Count != 0 ) {
            foreach ( string tag in targetTags ) {
                if ( _go.tag == tag ) {
                    isInTags = true;
                    break;
                }
            }
        }
        else { // none means all.
            isInTags = true;
        }
        return isInTags;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnTriggerEnter ( Collider _other ) {
        if ( IsTargetInTags(_other.gameObject) ) {
            ++numTargetInside;

            if ( triggerWhenEnter ) {
                DoTrigger();
            }

            // if the first target stay here
            if ( numTargetInside == 1 && stayForSeconds >= 0.0f ) {
                InvokeRepeating("DoTrigger", stayForSeconds, stayInterval);
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnTriggerExit ( Collider _other ) {
        if ( IsTargetInTags(_other.gameObject) ) {
            --numTargetInside;

            if ( numTargetInside == 0 ) {
                CancelInvoke("DoTrigger");

                if ( triggerWhenExit ) {
                    DoTrigger();
                }
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void DoTrigger () {
        if ( base.CanTrigger() ) {
            base.Response();
        }
    }

}; // end class
