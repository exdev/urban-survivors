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
        Vector3 velocity = controller.velocity;
        // DebugHelper.ScreenPrint( "velocity: " + velocity );
        Debug.DrawLine ( transform.position, transform.position + velocity, Color.white );
        // } DEBUG end 

        //
        HandleInput();
        ProcessMovement ();
        ProcessAnimation();
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
            state.enabled = false;
        }

        state = anim["melee1"];
        state.layer = 1;
        state.wrapMode = WrapMode.Once;
        state.weight = 1.0f;
        state.enabled = false;

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
        Vector2 screen_dir = screenPad ? screenPad.GetMoveDirection() : Vector2.zero;
        if ( screen_dir.magnitude >= 0.0f ) {
            moveDir.x = screen_dir.x;
            moveDir.y = screen_dir.y;
            Transform mainCamera = Camera.main.GetComponent( typeof(Transform) ) as Transform;
            moveDir = mainCamera.TransformDirection(moveDir.normalized); 
            moveDir.y = 0.0f;
            moveDir = moveDir.normalized;
        }
        // TEMP { 
        if ( Input.GetKeyDown(KeyCode.Space) ) {
            anim.CrossFade("melee1");
        }
        // } TEMP end 
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    private void ProcessMovement () {
        float f = Mathf.Pow(drag, Time.deltaTime);

        Vector3 vel = controller.velocity;
        vel *= f;
        vel += moveDir * maxSpeed * Time.deltaTime;
        if ( vel.magnitude > maxSpeed ) 
            vel = vel.normalized * maxSpeed;

        // apply gravity
        if ( controller.isGrounded == false )
            vel.y = -10.0f;

        controller.Move(vel * Time.deltaTime);

        if ( moveDir.magnitude > 0.0f )
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
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    private void ProcessAnimation () {
        if ( moveDir.magnitude > 0.0f ) {
            anim["moveforward"].normalizedSpeed = StepSpeed * controller.velocity.magnitude;
            anim.CrossFade("moveforward");
        }
        else {
            anim.CrossFade("idle");
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    private void StartMeleeAttack () {
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    private void EndMeleeAttack () {
    }
}
