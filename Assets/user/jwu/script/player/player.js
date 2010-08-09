// ======================================================================================
// File         : player.js
// Author       : Wu Jie 
// Last Change  : 07/19/2010 | 21:27:33 PM | Monday,July
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// variables
///////////////////////////////////////////////////////////////////////////////

var maxSpeed = 50.0; // the max speed for running
var acceleration = 2.0; // from move to run
var deceleration = 2.0; // from run to move
// TODO: var rotationSpeed = 100.0; // no used yet
var velocity = Vector3 ( 0.0, 0.0, 0.0 );

private var dbgText = "null";
private var moveFB = 0;
private var moveLR = 0;

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

    dbgText = "FB: " + moveFB + " LR: " + moveLR;
}

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

function Start () {
    // TODO:
}

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

function Update () {

    // ======================================================== 
    // process translate 
    // ======================================================== 

    var mainCamera = Camera.main.GetComponent(Transform);
    var camForward = Vector3( mainCamera.forward.x, 0.0, mainCamera.forward.z );
    camForward.Normalize();
    var camRight = Vector3( mainCamera.right.x, 0.0, mainCamera.right.z );
    camRight.Normalize();

    // Get the horizontal and vertical axis.
    HandleInput ();

    var speed = 20.0;

    var linearFB = moveFB * camForward; 
    var linearLR = moveLR * camRight; 

    // FB
    if ( moveFB ) {
        linearFB *= acceleration;
    }
    else if ( Mathf.Abs(velocity.x) >= 0.1 ) {
        linearFB.x = -velocity.x;
        linearFB.Normalize();
        linearFB *= deceleration;
    }
    else
        velocity.x = 0.0;

    // LR
    if ( moveLR ) {
        linearLR *= acceleration;
    }
    else if ( Mathf.Abs(velocity.z) >= 0.1 ) {
        linearLR.z = -velocity.z;
        linearLR.Normalize();
        linearLR *= deceleration;
    }
    else
        velocity.z = 0.0;

    // update velocity
    velocity += (linearFB + linearLR) * Time.deltaTime;
    if ( velocity.magnitude > maxSpeed ) {
        velocity.Normalize();
        velocity *= maxSpeed;
    } 

    // now position
    transform.Translate (velocity * Time.deltaTime);

    dbgText += "velocity: " + velocity; 

    // ======================================================== 
    // process rotations 
    // ======================================================== 

    // TODO: rotate should be automatically { 
    // var rotationH = Input.GetAxis ("Mouse X") * rotationSpeed * Time.deltaTime;
    // var rotationV = Input.GetAxis ("Mouse Y") * rotationSpeed * Time.deltaTime;
    // } TODO end 

    // Rotate around our y-axis
    // var rot = Quaternion.identity;
    // rot.eulerAngles = Vector3(rotationV, rotationH, 0.0);
    // aimDir = rot * aimDir;

    // DEBUG { 
    // dbgText = "roth = " + rotationH + ", rotv = " + rotationV;
    // } DEBUG end 
}

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

function OnGUI () {
    GUI.Label ( Rect (10, 10, 200, 20), dbgText.ToString() );
}
