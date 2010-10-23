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

[RequireComponent (typeof (Animation))]
public class Player_girl : Player_base {

    // private
    private Vector3 moveDir;
    private Animation anim;
    private Vector3 aimDir = Vector3.forward;
    // TEMP DISABLE: private bool rotating = false;

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public float degreeToRot = 15.0f;
    public float rotTime = 1.0f;
    public float stepSpeed = 1.0f;
    public float degreePlayMoveLeftRight = 60.0f;

    // Q: why don't we just use UpperBody in this game?
    // A: this metho will make sure the 'upper-body' is specific by user regardless the name of the entity, 
    //    so it can be flexiable enough for different game.
    public Transform upperBody;
    public Transform lowerBody;
    public GameObject curWeapon;

    public GameObject followTarget;
    public float followDistance = 1.5f;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	new void Start () {
        base.Start();
        DebugHelper.Assert( degreePlayMoveLeftRight <= 90.0f, "the degree can't larger than 90.0" );
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
        HandleInput ();
        ProcessFollowing ();
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
            state.layer = 0;
            state.wrapMode = WrapMode.Loop;
            state.weight = 1.0f;
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
        if ( followTarget == null ) {
            Vector2 screen_dir = screenPad.GetMoveDirection();
            if ( screen_dir.magnitude >= 0.0f ) {
                moveDir.x = screen_dir.x;
                moveDir.y = screen_dir.y;
                Transform mainCamera = Camera.main.transform;
                moveDir = mainCamera.TransformDirection(moveDir.normalized); 
                moveDir.y = 0.0f;
                moveDir = moveDir.normalized;
            }
        }

        Vector2 aimDir2D = screenPad.GetAimingDirection();
        aimDir = Vector3.zero; 
        aimDir.x = aimDir2D.x; aimDir.y = aimDir2D.y; 
        aimDir = Camera.main.transform.TransformDirection(aimDir);
        aimDir.y = 0.0f;
        aimDir = aimDir.normalized;

        // process aiming
#if UNITY_IPHONE
        if ( screenPad.AvailableTouches().Count != 0 ) {
            Touch lastTouch = screenPad.GetLastTouch();
            Ray ray = Camera.main.ScreenPointToRay (lastTouch.position); 
#else
            Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition); 
#endif
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

            // handle fire 
#if UNITY_IPHONE
            if ( lastTouch.phase != TouchPhase.Ended ) {
#else
            if ( Input.GetButton("Fire") ) {
#endif
                // if we have weapon in hand.
                if ( curWeapon ) {
                    Fire fire = curWeapon.GetComponent(typeof(Fire)) as Fire;
                    if (fire) {
                        fire.Trigger();
                    }
                }
            }
#if UNITY_IPHONE
        }
#endif
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    private void ProcessFollowing () {
        if (followTarget != null) {
            Vector3 dir = (transform.position - followTarget.transform.position).normalized;
            Vector3 destination = dir * followDistance + followTarget.transform.position;

            Vector3 delta = destination - transform.position;

            // if we are from idle to move
            Vector3 curVelocity = rigidbody.velocity; 
            if ( curVelocity.magnitude < 0.2f ) {
                if ( delta.magnitude > 0.5f ) {
                    moveDir = delta.normalized; 
                }
            }
            else if ( delta.magnitude > 0.1f ) {
                moveDir = delta.normalized; 
            }

            // DEBUG { 
            DebugHelper.DrawCircleY( followTarget.transform.position, followDistance, Color.yellow );
            DebugHelper.DrawBall( destination, 0.2f, Color.green );
            // } DEBUG end 
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    private void ProcessMovement () {
        // 
        if ( moveDir.magnitude > 0.0f ) {
            rigidbody.AddForce ( moveDir * maxSpeed, ForceMode.Acceleration );
            transform.position = new Vector3( transform.position.x, 0.0f, transform.position.z );
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    private void ProcessAnimation () {
        // get the animation forward,right so that we can pickup the proper animation.
        Vector3 curVelocity = rigidbody.velocity; 
        Vector3 vel_ubspace = upperBody.worldToLocalMatrix * curVelocity;
        vel_ubspace.y = 0.0f;
        vel_ubspace.Normalize();

        // TODO: if nothings move, crossfade to idle... so rotate, movement no need for idle. { 
        // if ( vel_ubspace.sqrMagnitude < 0.2 )
        if ( moveDir.magnitude == 0.0f ) {
            // float fadeSpeed = 5.0f * Time.deltaTime;
            anim.CrossFade("idle");
        }
        // } TODO end 
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    private void PostAnim () {
        // process lower-body rotation
        float angle = Vector3.Angle ( moveDir, aimDir );
        // DebugHelper.ScreenPrint("angle = " + angle); // DEBUG
        lowerBody.forward = aimDir;
        string animName = "";
        if ( moveDir.magnitude != 0.0f ) {
            if ( angle > 180.0f - degreePlayMoveLeftRight ) {
                lowerBody.forward = -moveDir;
                animName = "moveBackward";
            }
            else if ( angle < degreePlayMoveLeftRight ) {
                lowerBody.forward = moveDir;
                animName = "moveForward";
            }
            else {
                Vector3 up = Vector3.Cross(moveDir,aimDir);
                if ( up.y > 0.0f ) {
                    animName = "moveLeft";
                }
                else {
                    animName = "moveRight";
                }
            }
            anim.CrossFade(animName);
            anim[animName].normalizedSpeed = stepSpeed;
        }

        // TODO: smooth rotation
        // TEMP DISABLE { 
        // if ( Vector3.Dot ( upperBody.forward, lowerBody.forward ) )
        // float angle = Quaternion.Angle ( upperBody.rotation, lowerBody.rotation );
        // if ( angle > degreeToRot ) {
        //     DebugHelper.ScreenPrint("rotating");
        //     // iTween.Stop (lowerBody.gameObject,"rotate");
        //     // iTween.RotateTo( lowerBody.gameObject, iTween.Hash (
        //     //                  "rotation", upperBody.eulerAngles,
        //     //                  "time", rotTime,
        //     //                  "easeType", iTween.EaseType.easeOutCubic,
        //     //                  "oncomplete", "onRotateEnd",
        //     //                  "oncompletetarget", gameObject
        //     //                  ) );
        //     // if ( moveDir.magnitude == 0.0f ) {
        //     //     Vector3 cross_product = Vector3.Cross(upperBody.forward,lowerBody.forward);
        //     //     Animation anim = GetComponent( typeof(Animation) ) as Animation;
        //     //     if ( cross_product.y > 0.0 )
        //     //         anim.CrossFade("turnRight");
        //     //     else if ( cross_product.y < 0.0 )
        //     //         anim.CrossFade("turnLeft");
        //     // }
        //     rotating = true;
        // }
        // } TEMP DISABLE end 

        // NOTE: upper-body rotation must be calculate after lower-body.
        // process upper-body rotation
        upperBody.forward = aimDir;
    }

    // TODO: smooth rotation
    // TEMP DISABLE { 
    // // ------------------------------------------------------------------ 
    // // Desc: 
    // // ------------------------------------------------------------------ 

    // private void onRotateEnd () {
    //     rotating = false;
    // }
    // } TEMP DISABLE end 
}
