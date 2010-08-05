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

    var linear = Vector3 ( 0.0, 0.0, 0.0 );
    if ( moveFB || moveLR ) { // if we are moving.
        linear = moveFB * camForward + moveLR * camRight;
        linear *= acceleration;
    } 
    else if ( velocity.magnitude >= 0.001 ) { // if we still not stopped.
        linear = -velocity;
        linear.Normalize(); 
        linear *= deceleration;
    }

    // update velocity
    velocity += linear * Time.deltaTime;
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
