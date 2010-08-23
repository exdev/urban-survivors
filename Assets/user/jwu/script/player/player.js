// ======================================================================================
// File         : player.js
// Author       : Wu Jie 
// Last Change  : 07/19/2010 | 21:27:33 PM | Monday,July
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// variables
///////////////////////////////////////////////////////////////////////////////

var maxSpeed = 50.0; // the max speed for running.
var degreeToRot = 15.0;
var rotTime = 1.0;

// DELME: I think this should remove soon, and use follow terrain technique. 
var horizon = 1.5; // the fixed height for player.

// Q: why don't we just use UpperBody in this game?
// A: this metho will make sure the 'upper-body' is specific by user regardless the name of the entity, 
//    so it can be flexiable enough for different game.
var upperBody : Transform;
var lowerBody : Transform;

// for camera
private var zoomIn = false;
private var aimPos = Vector3.zero;

// for movement
private var moveFB = 0;
private var moveLR = 0;
private var rotating = false;

// for DEBUG:
private var dbgText = "";


///////////////////////////////////////////////////////////////////////////////
// functions
///////////////////////////////////////////////////////////////////////////////

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

private function HandleInput () {
    // DISABLE { 
    // DEBUG: dbgText = "hori: " + Input.GetAxis ("Horizontal") + " vert: " + Input.GetAxis ("Vertical");
    // moveFB = Input.GetAxis ("Vertical") * camForward * moveSpeed * Time.deltaTime;
    // moveLR = Input.GetAxis ("Horizontal") * camRight * moveSpeed * Time.deltaTime;
    // } DISABLE end 

    moveFB = 0;
    moveLR = 0;

    // TODO: use unity player control system { 
    if ( Input.GetKey("w") )
        moveFB += 1;
    if ( Input.GetKey("s") )
        moveFB -= 1;
    if ( Input.GetKey("d") )
        moveLR += 1;
    if ( Input.GetKey("a") )
        moveLR -= 1;
    // } TODO end 

    // camera zoom or not
    if ( Input.GetKey(KeyCode.LeftShift) ) {
        zoomIn = true; 
    } else {
        zoomIn = false; 
    }

    var ray = Camera.main.ScreenPointToRay (Input.mousePosition); 
    var plane = Plane ( Vector3.up, -upperBody.position.y );
    var dist = 0.0;
    plane.Raycast( ray, dist );
    aimPos = ray.origin + ray.direction * dist;
    // aimPos.y = upperBody.position.y;

    // DEBUG { 
    // Debug.DrawRay ( ray.origin, ray.direction * 100, Color.yellow ); // camera look-foward ray
    DebugHelper.DrawBall ( aimPos, 0.2, Color.red ); // player aiming position
    Debug.DrawLine ( upperBody.position, aimPos, Color.red ); // player aiming direction
    // } DEBUG end 

    dbgText = "FB: " + moveFB + " LR: " + moveLR + "zoom: " + zoomIn + "\n";
    dbgText += "up_pos: " + upperBody.position + "\n";
    dbgText += "aimpos: " + aimPos + "\n";
}

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

function ProcessCamera () {
    var cam = Camera.main.GetComponent(Camera);
    var wantedFov = 60.0;
    if ( zoomIn ) {
        wantedFov = 40.0;
    }
    cam.fieldOfView = Mathf.Lerp (cam.fieldOfView, wantedFov, 8.0 * Time.deltaTime);
}

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

function ProcessMovement () {

    // ======================================================== 
    // process translate 
    // ======================================================== 

    // get camera forward and right
    var mainCamera = Camera.main.GetComponent(Transform);
    var camForward = Vector3( mainCamera.forward.x, 0.0, mainCamera.forward.z );
    camForward.Normalize();
    var camRight = Vector3( mainCamera.right.x, 0.0, mainCamera.right.z );
    camRight.Normalize();

    //
    var linearFB = moveFB * camForward; 
    var linearLR = moveLR * camRight; 
    var dir = (linearFB + linearLR).normalized; // 100.0 is a scale value for force.

    rigidbody.AddForce ( dir * maxSpeed, ForceMode.Acceleration );
    // rigidbody.AddForce ( dir * maxSpeed, ForceMode.VelocityChange );
    transform.position = new Vector3( transform.position.x, horizon, transform.position.z );
}

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

function PostAnim () {
    // ======================================================== 
    // process rotations 
    // ======================================================== 

    var aimDir = aimPos - upperBody.position; 
    aimDir.y = 0.0;
    aimDir.Normalize();
    upperBody.forward = aimDir;
    // if ( Vector3.Dot ( upperBody.forward, lowerBody.forward ) )
    var angle = Quaternion.Angle ( upperBody.rotation, lowerBody.rotation );
    if ( angle > degreeToRot ) {
        iTween.RotateTo( lowerBody.gameObject, {
            "rotation": upperBody.eulerAngles,
            "time": rotTime,
            "easeType": iTween.EaseType.easeOutCirc,
            "oncomplete": "onRotateEnd",
            "oncompletetarget": gameObject
            } );
            rotating = true;

        // TODO: we can play this in ProcessAnimation by left/right, and once the anim finished, it will crossfade to idle. 
        // 1. how can it be finished?? 
        // 2. how to detect if left or right?? { 
        if ( moveFB == 0 && moveLR == 0 ) {
            anim = transform.GetComponentInChildren(Animation);
            anim.CrossFade("turnRight");
        }
        // } TODO end 
    }
}

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

function onRotateEnd () {
    rotating = false;
}

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

function ProcessAnimation () {
    // get the animation forward,right so that we can pickup the proper animation.
    var curVelocity = rigidbody.velocity; 
    var vel_lbspace = lowerBody.worldToLocalMatrix * curVelocity;
    vel_lbspace.y = 0.0;
    vel_lbspace.Normalize();
    dbgText += "vel in lowerBody space: " + vel_lbspace + "\n";

    // TODO: once nantas confirm and move the animation-component to the player, we
    // should use GetComponent { 
    anim = transform.GetComponentInChildren(Animation);
    // } TODO end 

    // TODO: if nothings move, crossfade to idle... so rotate, movement no need for idle. { 
    // if ( vel_lbspace.sqrMagnitude < 0.2 )
    if ( moveFB == 0 && moveLR == 0 ) {
        var fadeSpeed = 5.0 * Time.deltaTime;
        anim["moveForward"].weight -= fadeSpeed;
        anim["moveBackward"].weight -= fadeSpeed;
        anim["moveLeft"].weight -= fadeSpeed;
        anim["moveRight"].weight -= fadeSpeed;

        if ( rotating == false ) {
            anim.CrossFade("idle");
        }
    }
    // } TODO end 
    else {
        var anim_f = anim["moveForward"];
        var anim_b = anim["moveBackward"];
        if ( vel_lbspace.z > 0 ) {
            anim_f.weight = vel_lbspace.z * 2.0;
            anim_f.normalizedSpeed = vel_lbspace.z;
            anim_b.weight = 0.0;
        }
        else {
            anim_f.weight = 0.0;
            anim_b.weight = -vel_lbspace.z * 2.0; 
            anim_b.normalizedSpeed = -vel_lbspace.z;
        }

        var anim_l = anim["moveLeft"];
        var anim_r = anim["moveRight"];
        if ( vel_lbspace.x > 0 ) {
            anim_r.weight = vel_lbspace.x * 2.0;
            anim_r.normalizedSpeed = vel_lbspace.x;
            anim_l.weight = 0.0;
        }
        else {
            anim_r.weight = 0.0;
            anim_l.weight = -vel_lbspace.x * 2.0;
            anim_l.normalizedSpeed = -vel_lbspace.x;
        }
    }
}

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

function Start () {
    var state;
    anim = transform.GetComponentInChildren(Animation);
    var anim_keys = ["moveForward", "moveBackward", "moveRight", "moveLeft"];
    for (key in anim_keys) {
        state = anim[key];
        state.layer = 1;
        state.wrapMode = WrapMode.Loop;
        state.weight = 1.0;
        state.blendMode = AnimationBlendMode.Blend;
        state.enabled = true;
    }

    state = anim["turnRight"];
    state.wrapMode = WrapMode.Once;
    state.layer = 0;
    state.weight = 1.0;

    state = anim["idle"];
    state.wrapMode = WrapMode.Loop;
    state.layer = 0;
    state.weight = 1.0;
    state.enabled = true;
}

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

function Update () {

    dbgText = "";

    // Get the horizontal and vertical axis.
    // Get the mouse aiming pos.
    HandleInput ();

    // Process translation and rotation.
    ProcessCamera ();
    // ProcessMovement (); // change this to FixedUpdate to prevent shaking when moving
    ProcessAnimation ();

    // DEBUG { 
    var velocity = rigidbody.GetPointVelocity(transform.position);
    dbgText += "velocity: " + velocity + "\n";
    Debug.DrawLine ( transform.position, transform.position + velocity, Color.white );
    // } DEBUG end 
}

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

function LateUpdate () {
    PostAnim ();
}

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

function FixedUpdate () {
    ProcessMovement ();
}

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

function OnGUI () {
    GUI.Label ( Rect (10, 10, 200, 100), dbgText.ToString() );
}
