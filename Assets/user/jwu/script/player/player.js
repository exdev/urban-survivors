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

// Q: why don't we just use UpperBody in this game?
// A: this metho will make sure the 'upper-body' is specific by user regardless the name of the entity, 
//    so it can be flexiable enough for different game.
var lowerBody : Transform;
var upperBody : Transform;

private var aimPos = Vector3.zero;
private var moveFB = 0;
private var moveLR = 0;
private var zoomIn = false;

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
    var plane = Plane ( Vector3.up, transform.position.y );
    var dist = 0.0;
    plane.Raycast( ray, dist );
    aimPos = ray.origin + ray.direction * dist;
    aimPos.y = transform.position.y;

    // DEBUG { 
    // Debug.DrawRay ( ray.origin, ray.direction * 100, Color.yellow ); // camera look-foward ray
    DebugHelper.DrawBall ( aimPos, 1.0, Color.red ); // player aiming position
    Debug.DrawLine ( transform.position, aimPos, Color.red ); // player aiming direction
    // } DEBUG end 

    dbgText = "FB: " + moveFB + " LR: " + moveLR + "zoom: " + zoomIn + "\n";
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
    // process rotations 
    // ======================================================== 

    var aimDir = aimPos - transform.position; 
    aimDir.y = 0.0;
    aimDir.Normalize();
    upperBody.forward = aimDir;

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

function ProcessAnimation () {
    lowerAnim = lowerBody.GetComponent(Animation);
    if ( moveFB > 0 ) {
        lowerAnim.Play("moveForward");
    }
    else if ( moveFB < 0 ) {
        lowerAnim.Play("moveBackward");
    }
    else if ( moveLR > 0 ) {
        lowerAnim.Play("moveRight");
    }
    else if ( moveLR < 0 ) {
        lowerAnim.Play("moveLeft");
    }
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
    ProcessCamera ();
    ProcessMovement ();
    ProcessAnimation ();

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
