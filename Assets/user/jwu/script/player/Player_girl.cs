// ======================================================================================
// File         : Player_girl.cs
// Author       : Wu Jie 
// Last Change  : 10/12/2010 | 00:44:29 AM | Tuesday,October
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////
// class 
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

public class Player_girl : Player_base {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    private Vector3 moveDir;
    private Animation anim;

    // rotate
    private Vector3 aimPos = Vector3.zero;

    // movement
    private float moveFB = 0;
    private float moveLR = 0;
    private bool rotating = false;
    private bool need_rewind = true;

    // Q: why don't we just use UpperBody in this game?
    // A: this metho will make sure the 'upper-body' is specific by user regardless the name of the entity, 
    //    so it can be flexiable enough for different game.
    public Transform upperBody;
    public Transform lowerBody;
    public GameObject curWeapon;

    public float degreeToRot = 15.0f;
    public float rotTime = 1.0f;

    public GameObject followTarget;

    ///////////////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	new void Start () {
        base.Start();
        initAnim ();
	}
	
    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	void Update () {
	}

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void FixedUpdate () {
        ProcessMovement ();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    private void initAnim () {
        // get the animation component
        anim = gameObject.GetComponent(typeof(Animation)) as Animation;

        // setup the animation state
        // AnimationState state;
        // string[] anim_keys = { "moveforward" };
        // foreach (string key in anim_keys) {
        //     state = anim[key];
        //     state.layer = 0;
        //     state.wrapMode = WrapMode.Loop;
        //     state.weight = 1.0f;
        //     state.blendMode = AnimationBlendMode.Blend;
        //     state.enabled = false;
        // }
        // state = anim["idle"];
        // state.wrapMode = WrapMode.Loop;
        // state.layer = 0;
        // state.weight = 1.0f;
        // state.enabled = true;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    private void ProcessMovement () {
        // in following mode
        if ( followTarget ) {
        }
        else {
        }
    }
}
