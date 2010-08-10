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
var horizon = 10.0;
// var acceleration = 2.0; // from move to run
// var deceleration = 2.0; // from run to move

private var aimPos = Vector3.zero;
private var moveFB = 0;
private var moveLR = 0;

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

    var ray = Camera.main.ScreenPointToRay (Input.mousePosition); 
    var plane = Plane ( Vector3.up, transform.position.y );
    var dist = 0.0;
    plane.Raycast( ray, dist );
    aimPos = ray.origin + ray.direction * dist;

    // DEBUG { 
    Debug.DrawRay ( ray.origin, ray.direction * 100, Color.yellow );
    DebugHelper.DrawBall ( aimPos, 1.0, Color.red );
    // } DEBUG end 

    dbgText = "FB: " + moveFB + " LR: " + moveLR + "\n";
}

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

function ProcessMovement () {
    // ======================================================== 
    // process rotations 
    // ======================================================== 

    var aimDir = aimPos - transform.position; 
    aimDir.y = 0.0;
    aimDir.Normalize();
    transform.forward = aimDir;

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
    var dir = (linearFB + linearLR).normalized * 1000.0; // 1000.0 is a scale value for force.

    rigidbody.AddForce ( dir * maxSpeed * Time.deltaTime );
    transform.position = new Vector3( transform.position.x, horizon, transform.position.z );
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

    dbgText = "";

    // Get the horizontal and vertical axis.
    // Get the mouse aiming pos.
    HandleInput ();

    // Process translation and rotation.
    ProcessMovement ();

    // DEBUG { 
    var velocity = rigidbody.GetPointVelocity(transform.position);
    dbgText += "velocity: " + velocity;
    Debug.DrawLine ( transform.position, transform.position + velocity, Color.white );
    // } DEBUG end 
}

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

function OnGUI () {
    GUI.Label ( Rect (10, 10, 200, 50), dbgText.ToString() );
}
