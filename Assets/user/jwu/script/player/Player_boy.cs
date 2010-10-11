// ======================================================================================
// File         : Player_boy.cs
// Author       : Wu Jie 
// Last Change  : 10/08/2010 | 23:24:12 PM | Friday,October
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////
// class Player_boy
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

[RequireComponent (typeof (Animation))]
public class Player_boy : Player_base {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    private Vector3 moveDir;
    private Animation anim;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
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
        // DEBUG { 
        Vector3 velocity = rigidbody.GetPointVelocity(transform.position);
        DebugHelper.ScreenPrint( "velocity: " + velocity );
        Debug.DrawLine ( transform.position, transform.position + velocity, Color.white );
        // } DEBUG end 

        //
        HandleInput();
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
        AnimationState state;
        string[] anim_keys = { "moveforward" };
        foreach (string key in anim_keys) {
            state = anim[key];
            state.layer = 0;
            state.wrapMode = WrapMode.Loop;
            state.weight = 1.0f;
            state.blendMode = AnimationBlendMode.Blend;
            state.enabled = false;
        }
        state = anim["idle"];
        state.wrapMode = WrapMode.Loop;
        state.layer = 0;
        state.weight = 1.0f;
        state.enabled = true;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    private void HandleInput() {
        moveDir = Vector3.zero; 
        Vector2 screen_dir = screenPad.GetMoveDirection();
        if ( screen_dir.magnitude >= 0.0f ) {
            moveDir.x = screen_dir.x;
            moveDir.y = screen_dir.y;
            Transform mainCamera = Camera.main.GetComponent( typeof(Transform) ) as Transform;
            moveDir = mainCamera.TransformDirection(moveDir.normalized); 
            moveDir.y = 0.0f;
            moveDir = moveDir.normalized;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    private void ProcessMovement () {
        if ( moveDir.magnitude > 0.0f ) {
            rigidbody.AddForce ( moveDir * maxSpeed, ForceMode.Acceleration );
            transform.position = new Vector3( transform.position.x, 0.0f, transform.position.z );
            transform.forward = moveDir;

            // TODO { 
            // Quaternion rot = Quaternion.identity;
            // rot.SetLookRotation(moveDir);
            // iTween.RotateTo( gameObject, iTween.Hash (
            //                  "rotation", rot.eulerAngles,
            //                  "time", 0.2f,
            //                  "easeType", iTween.EaseType.easeOutCubic
            //                  ) );
            // } TODO end 

            anim.CrossFade("moveforward");
        }
        else {
            anim.CrossFade("idle");
        }
    }
}
