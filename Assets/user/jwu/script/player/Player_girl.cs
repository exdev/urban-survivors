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

    // private
    private Vector3 moveDir;
    private Animation anim;
    private Vector3 aimDir = Vector3.forward;
    private bool rotating = false;
    private bool need_rewind = true;

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public float degreeToRot = 15.0f;
    public float rotTime = 1.0f;

    // Q: why don't we just use UpperBody in this game?
    // A: this metho will make sure the 'upper-body' is specific by user regardless the name of the entity, 
    //    so it can be flexiable enough for different game.
    public Transform upperBody;
    public Transform lowerBody;
    public GameObject curWeapon;

    public GameObject followTarget;

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
        ProcessAnimation ();
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

    void LateUpdate () {
        PostAnim ();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    private void initAnim () {
        // get the animation component
        anim = gameObject.GetComponent(typeof(Animation)) as Animation;

        // animation
        AnimationState state;
        string[] anim_keys = { "moveForward", "moveBackward", "moveRight", "moveLeft" };
        foreach (string key in anim_keys) {
            state = anim[key];
            state.layer = 1;
            state.wrapMode = WrapMode.Loop;
            state.weight = 1.0f;
            state.blendMode = AnimationBlendMode.Blend;
            state.enabled = true;
        }

        state = anim["turnRight"];
        state.wrapMode = WrapMode.Once;
        state.layer = 0;
        state.weight = 1.0f;

        state = anim["turnLeft"];
        state.wrapMode = WrapMode.Once;
        state.layer = 0;
        state.weight = 1.0f;

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
        // get move direction
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

        // process aiming
        if ( screenPad.AvailableTouches().Count != 0 ) {
            Touch lastTouch = screenPad.GetLastTouch();
            Ray ray = Camera.main.ScreenPointToRay (lastTouch.position); 
            Plane plane = new Plane ( Vector3.up, -upperBody.position.y );
            float dist = 0.0f;
            plane.Raycast( ray, out dist );
            Vector3 aimPos = ray.origin + ray.direction * dist;

            aimDir = aimPos - upperBody.position; 
            aimDir.y = 0.0f;
            aimDir.Normalize();

            // DEBUG { 
            // Debug.DrawRay ( ray.origin, ray.direction * 100, Color.yellow ); // camera look-foward ray
            DebugHelper.DrawBall ( aimPos, 0.2f, Color.red ); // player aiming position
            Debug.DrawLine ( upperBody.position, aimPos, Color.red ); // player aiming direction
            // } DEBUG end 
        }

        // TODO: use screenpad "fire" { 
        // // handle fire 
        // if ( Input.GetButton("Fire") ) {
        //     // if we have weapon in hand.
        //     if ( curWeapon ) {
        //         Fire fire = curWeapon.GetComponent(typeof(Fire)) as Fire;
        //         if (fire) {
        //             fire.Trigger();
        //         }
        //     }
        // }
        // } TODO end 
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    private void ProcessMovement () {
        // in following mode
        if ( followTarget ) {
        }
        else {
            if ( moveDir.magnitude > 0.0f ) {
                rigidbody.AddForce ( moveDir * maxSpeed, ForceMode.Acceleration );
                transform.position = new Vector3( transform.position.x, 0.0f, transform.position.z );
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    private void ProcessAnimation () {
        // get the animation forward,right so that we can pickup the proper animation.
        Vector3 curVelocity = rigidbody.velocity; 
        Vector3 vel_lbspace = lowerBody.worldToLocalMatrix * curVelocity;
        vel_lbspace.y = 0.0f;
        vel_lbspace.Normalize();

        // TODO: if nothings move, crossfade to idle... so rotate, movement no need for idle. { 
        // if ( vel_lbspace.sqrMagnitude < 0.2 )
        if ( moveDir.magnitude == 0.0f ) {
            // float fadeSpeed = 5.0f * Time.deltaTime;
            anim.Blend( "moveForward", 0.0f );
            anim.Blend( "moveBackward", 0.0f );
            anim.Blend( "moveLeft", 0.0f );
            anim.Blend( "moveRight", 0.0f );
            need_rewind = true;

            if ( rotating == false ) {
                anim.CrossFade("idle");
            }
        }
        // } TODO end 
        else {
            // rewind the animation so that each time it moved, it start from the beginning frame.
            if ( need_rewind ) {
                need_rewind = false;
                anim.Rewind("moveForward");
                anim.Rewind("moveBackward");
                anim.Rewind("moveLeft");
                anim.Rewind("moveRight");
            }

            // blend forward/backward
            if ( vel_lbspace.z > 0 ) {
                anim.Blend("moveForward", vel_lbspace.z * 2.0f );
                anim.Blend("moveBackward", 0.0f );
                anim["moveForward"].normalizedSpeed = vel_lbspace.z;
            }
            else {
                anim.Blend("moveForward", 0.0f );
                anim.Blend("moveBackward", -vel_lbspace.z * 2.0f );
                anim["moveBackward"].normalizedSpeed = -vel_lbspace.z;
            }

            // blend left/right
            if ( vel_lbspace.x > 0 ) {
                anim.Blend("moveRight", vel_lbspace.x * 2.0f );
                anim.Blend("moveLeft", 0.0f );
                anim["moveRight"].normalizedSpeed = vel_lbspace.x;
            }
            else {
                anim.Blend("moveRight", 0.0f );
                anim.Blend("moveLeft", -vel_lbspace.x * 2.0f );
                anim["moveLeft"].normalizedSpeed = -vel_lbspace.x;
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    private void PostAnim () {
        // process lower-body rotation
        lowerBody.forward = aimDir;

        // if ( Vector3.Dot ( upperBody.forward, lowerBody.forward ) )
        float angle = Quaternion.Angle ( upperBody.rotation, lowerBody.rotation );
        if ( angle > degreeToRot ) {
            // iTween.Stop (lowerBody.gameObject,"rotate");
            // iTween.RotateTo( lowerBody.gameObject, iTween.Hash (
            //                  "rotation", upperBody.eulerAngles,
            //                  "time", rotTime,
            //                  "easeType", iTween.EaseType.easeOutCubic,
            //                  "oncomplete", "onRotateEnd",
            //                  "oncompletetarget", gameObject
            //                  ) );
            DebugHelper.ScreenPrint("rotating");
            // if ( moveDir.magnitude == 0.0f ) {
            //     Vector3 cross_product = Vector3.Cross(upperBody.forward,lowerBody.forward);
            //     Animation anim = GetComponent( typeof(Animation) ) as Animation;
            //     if ( cross_product.y > 0.0 )
            //         anim.CrossFade("turnRight");
            //     else if ( cross_product.y < 0.0 )
            //         anim.CrossFade("turnLeft");
            // }
            rotating = true;
        }

        // NOTE: upper-body rotation must be calculate after lower-body.
        // process upper-body rotation
        upperBody.forward = aimDir;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    private void onRotateEnd () {
        rotating = false;
    }
}
