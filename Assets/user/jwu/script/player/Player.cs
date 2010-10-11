// ======================================================================================
// File         : player.cs
// Author       : Wu Jie 
// Last Change  : 09/27/2010 | 21:49:32 PM | Monday,September
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

public class Player : MonoBehaviour {

    ///////////////////////////////////////////////////////////////////////////////
    // variables 
    ///////////////////////////////////////////////////////////////////////////////

    public float maxSpeed = 50.0f; // the max speed for running.
    public float degreeToRot = 15.0f;
    public float rotTime = 1.0f;

    // DELME: I think this should remove soon, and use follow terrain technique. 
    public float horizon = 1.5f; // the fixed height for player.

    // Q: why don't we just use UpperBody in this game?
    // A: this metho will make sure the 'upper-body' is specific by user regardless the name of the entity, 
    //    so it can be flexiable enough for different game.
    public Transform upperBody;
    public Transform lowerBody;
    public GameObject curWeapon;

    // for camera
    private bool zoomIn = false;
    private Vector3 aimPos = Vector3.zero;

    // for movement
    private float moveFB = 0;
    private float moveLR = 0;
    private bool rotating = false;
    private bool need_rewind = true;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    private void HandleInput () {
        // DISABLE { 
        // DEBUG: DebugHelper.ScreenPrint ( "hori: " + Input.GetAxis ("Horizontal") + " vert: " + Input.GetAxis ("Vertical") );
        // moveFB = Input.GetAxis ("Vertical") * camForward * moveSpeed * Time.deltaTime;
        // moveLR = Input.GetAxis ("Horizontal") * camRight * moveSpeed * Time.deltaTime;
        // } DISABLE end 

        // get axis raw
        moveFB = Input.GetAxisRaw("Vertical");
        moveLR = Input.GetAxisRaw("Horizontal");

        // camera zoom or not
        if ( Input.GetButton("Zoom") ) {
            zoomIn = true; 
        } else {
            zoomIn = false; 
        }

        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition); 
        Plane plane = new Plane ( Vector3.up, -upperBody.position.y );
        float dist = 0.0f;
        plane.Raycast( ray, out dist );
        aimPos = ray.origin + ray.direction * dist;
        // aimPos.y = upperBody.position.y;

        // handle fire 
        if ( Input.GetButton("Fire") ) {
            // if we have weapon in hand.
            if ( curWeapon ) {
                Fire fire = curWeapon.GetComponent(typeof(Fire)) as Fire;
                if (fire) {
                    fire.Trigger();
                }
            }
        }

        // DEBUG { 
        // Debug.DrawRay ( ray.origin, ray.direction * 100, Color.yellow ); // camera look-foward ray
        DebugHelper.DrawBall ( aimPos, 0.2f, Color.red ); // player aiming position
        Debug.DrawLine ( upperBody.position, aimPos, Color.red ); // player aiming direction
        // } DEBUG end 

        DebugHelper.ScreenPrint ( "FB: " + moveFB + " LR: " + moveLR );
        DebugHelper.ScreenPrint ( "zoom: " + zoomIn );
        DebugHelper.ScreenPrint ( "up_pos: " + upperBody.position );
        DebugHelper.ScreenPrint ( "aimpos: " + aimPos );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    private void ProcessCamera () {
        Camera cam = Camera.main.GetComponent( typeof(Camera) ) as Camera;
        float wantedFov = 60.0f;
        if ( zoomIn ) {
            wantedFov = 40.0f;
        }
        cam.fieldOfView = Mathf.Lerp (cam.fieldOfView, wantedFov, 8.0f * Time.deltaTime);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    private void ProcessMovement () {

        // ======================================================== 
        // process translate 
        // ======================================================== 

        // get camera forward and right
        Transform mainCamera = Camera.main.GetComponent( typeof(Transform) ) as Transform;
        Vector3 camForward = new Vector3( mainCamera.forward.x, 0.0f, mainCamera.forward.z );
        camForward.Normalize();
        Vector3 camRight = new Vector3( mainCamera.right.x, 0.0f, mainCamera.right.z );
        camRight.Normalize();

        //
        Vector3 linearFB = moveFB * camForward; 
        Vector3 linearLR = moveLR * camRight; 
        Vector3 dir = (linearFB + linearLR).normalized; // 100.0 is a scale value for force.

        rigidbody.AddForce ( dir * maxSpeed, ForceMode.Acceleration );
        // rigidbody.AddForce ( dir * maxSpeed, ForceMode.VelocityChange );
        transform.position = new Vector3( transform.position.x, horizon, transform.position.z );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    private void PostAnim () {
        // ======================================================== 
        // process rotations 
        // ======================================================== 

        Vector3 aimDir = aimPos - upperBody.position; 
        aimDir.y = 0.0f;
        aimDir.Normalize();
        upperBody.forward = aimDir;
        // if ( Vector3.Dot ( upperBody.forward, lowerBody.forward ) )
        float angle = Quaternion.Angle ( upperBody.rotation, lowerBody.rotation );
        if ( angle > degreeToRot ) {
            iTween.RotateTo( lowerBody.gameObject, iTween.Hash (
                             "rotation", upperBody.eulerAngles,
                             "time", rotTime,
                             "delay", 0.05,
                             "easeType", iTween.EaseType.easeOutCubic,
                             "oncomplete", "onRotateEnd",
                             "oncompletetarget", gameObject
                             ) );
            if ( moveFB == 0 && moveLR == 0 ) {
                Vector3 cross_product = Vector3.Cross(upperBody.forward,lowerBody.forward);
                Animation anim = GetComponent( typeof(Animation) ) as Animation;
                if ( cross_product.y > 0.0 )
                    anim.CrossFade("turnRight");
                else if ( cross_product.y < 0.0 )
                    anim.CrossFade("turnLeft");
            }
            rotating = true;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    private void onRotateEnd () {
        rotating = false;
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
        DebugHelper.ScreenPrint( "vel in lowerBody space: " + vel_lbspace );

        // TODO: once nantas confirm and move the animation-component to the player, we
        // should use GetComponent { 
        Animation anim = GetComponent( typeof(Animation) ) as Animation;
        // } TODO end 

        // TODO: if nothings move, crossfade to idle... so rotate, movement no need for idle. { 
        // if ( vel_lbspace.sqrMagnitude < 0.2 )
        if ( moveFB == 0 && moveLR == 0 ) {
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

    void Start () {
        // DELME: this is my first approch to create correct control node for upperBody { 
        // GameObject obj = new GameObject("upperBody helper");
        // obj.transform.rotation = Quaternion.identity;
        // obj.transform.position = upperBody.transform.position;
        // obj.transform.parent = upperBody.parent; 
        // upperBody.parent = obj.transform; 
        // upperBody = obj.transform;
        // } DELME end 

        // this will prevent animation not init problem when the scene don't have Global prefab.
        if ( GameObject.FindWithTag("Global") != null ) {
            CollisionIgnoreManager.Instance().AddIgnore( collider, Constant.mask_player, Constant.mask_none );
        }

        // animation
        AnimationState state;
        Animation anim = GetComponent( typeof(Animation) ) as Animation;
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

    void Update () {

        // Get the horizontal and vertical axis.
        // Get the mouse aiming pos.
        HandleInput ();

        // Process translation and rotation.
        ProcessCamera ();
        // ProcessMovement (); // DELME: change this to FixedUpdate to prevent shaking when moving
        ProcessAnimation ();

        // DEBUG { 
        Vector3 velocity = rigidbody.GetPointVelocity(transform.position);
        DebugHelper.ScreenPrint( "velocity: " + velocity );
        Debug.DrawLine ( transform.position, transform.position + velocity, Color.white );
        // } DEBUG end 
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

    void FixedUpdate () {
        ProcessMovement ();
    }
}

