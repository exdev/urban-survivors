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

private var velocity = Vector3.zero;
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

    // TODO { 
    // } TODO end 

    dbgText = "FB: " + moveFB + " LR: " + moveLR + "\n";
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
    HandleInput ();

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

    // TEMP DEBUG { 
    // dbgText += "cam_forward: " + mainCamera.forward + "\n";
    // dbgText += "cam_right: " + mainCamera.right + "\n";
    // Debug.DrawLine ( transform.position, transform.position + camForward * 10.0, Color.red );
    // Debug.DrawLine ( transform.position, transform.position + camRight * 10.0, Color.blue );
    // } TEMP DEBUG end 

    // 
    var linearFB = moveFB * camForward; 
    var linearLR = moveLR * camRight; 
    
    // TEMP { 
    Debug.DrawLine ( transform.position, transform.position + linearFB * 10.0, Color.red );
    Debug.DrawLine ( transform.position, transform.position + linearLR * 10.0, Color.blue );
    transform.position += (linearFB + linearLR) * 20.0 * Time.deltaTime;
    // } TEMP end 

    // DISABLE { 
    // // FB
    // if ( moveFB ) {
    //     linearFB *= acceleration;
    // }
    // else if ( Mathf.Abs(velocity.x) >= 0.1 ) {
    //     linearFB.x = -velocity.x;
    //     linearFB.Normalize();
    //     linearFB *= deceleration;
    // }
    // else
    //     velocity.x = 0.0;

    // // LR
    // if ( moveLR ) {
    //     linearLR *= acceleration;
    // }
    // else if ( Mathf.Abs(velocity.z) >= 0.1 ) {
    //     linearLR.z = -velocity.z;
    //     linearLR.Normalize();
    //     linearLR *= deceleration;
    // }
    // else
    //     velocity.z = 0.0;

    // // update velocity
    // velocity += (linearFB + linearLR) * Time.deltaTime;
    // if ( velocity.magnitude > maxSpeed ) {
    //     velocity.Normalize();
    //     velocity *= maxSpeed;
    // } 

    // // now position
    // transform.position += velocity * Time.deltaTime;
    // } DISABLE end 

    // DEBUG { 
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
