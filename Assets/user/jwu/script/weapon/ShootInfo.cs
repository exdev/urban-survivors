// ======================================================================================
// File         : ShootInfo.cs
// Author       : Wu Jie 
// Last Change  : 12/13/2010 | 21:36:27 PM | Monday,December
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

// ------------------------------------------------------------------ 
// Desc: ShootInfo
// ------------------------------------------------------------------ 

public class ShootInfo : MonoBehaviour {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public string shootAnim = "unknown";
    public string reloadAnim = "unknown";
    public Transform anchor = null;
    public GameObject bullet = null;
    public float shootSpeed = 1.0f;
    public float reloadSpeed = 1.0f;
    // public float accuracy = 1.0f; // DELME: looks like accuracy is hardcoded in the Emitter.
    public int capacity = 10;

    protected int bullets = 10;
    protected Emitter emitter = null;

    ///////////////////////////////////////////////////////////////////////////////
    // function defines
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Start () {
        DebugHelper.Assert(this.shootAnim!="unknown", "shootAnim can't be unknown!");
        DebugHelper.Assert(this.reloadAnim!="unknown", "reloadAnim can't be unknown!");
        DebugHelper.Assert(this.anchor, "anchor not set" );
        DebugHelper.Assert(this.bullet, "bullet not set" );

        this.emitter = GetComponent( typeof(Emitter) ) as Emitter;
        DebugHelper.Assert(this.emitter, "can't find emitter" );
        this.bullets = this.capacity;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void AdjustAnim ( Animation _anim ) {
        AnimationState state = null;

        // adjust shoot speed;
        state = _anim[this.shootAnim];
        state.normalizedSpeed = shootSpeed;

        // adjust reload speed;
        state = _anim[this.reloadAnim];
        state.normalizedSpeed = reloadSpeed;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public bool OutOfAmmo () { return this.bullets <= 0; }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void Fire () {
        if ( OutOfAmmo() == false ) {
            // TODO: bullet consume.
            this.emitter.Emit(this.bullet);
            this.bullets -= 1;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void Reload ( int _amount ) {
        this.bullets = _amount;
    }
}
