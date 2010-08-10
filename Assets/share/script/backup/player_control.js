// ======================================================================================
// File         : player_control.js
// Author       : Wu Jie 
// Last Change  : 08/10/2010 | 23:40:08 PM | Tuesday,August
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// variables
///////////////////////////////////////////////////////////////////////////////

var maxSpeed = 50.0; // the max speed for running
var acceleration = 2.0; // from move to run
var deceleration = 2.0; // from run to move

private var velocity = Vector3.zero;
private var curSpeed = 0.0;
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

    // DISABLE: use rigidbody { 
    // var linearFB = moveFB * camForward; 
    // var linearLR = moveLR * camRight; 

    // rigidbody.AddForce ( (linearFB + linearLR).normalized * 1000.0 * 50.0 * Time.deltaTime );
    // transform.position = new Vector3( transform.position.x, 10.0, transform.position.z );
    // } DISABLE end 

    // 
    // DISABLE: acceleration without decelerate adjustment { 
    var dir = Vector3.zero;
    if ( moveFB || moveLR ) {
        var linearFB = moveFB * camForward; 
        var linearLR = moveLR * camRight; 
        dir = linearFB + linearLR; 
        dir.Normalize();
        curSpeed += acceleration * Time.deltaTime;
        if ( curSpeed > maxSpeed ) {
            curSpeed = maxSpeed;
        } 
    }
    else if ( curSpeed > 0.0 ) {
        curSpeed -= deceleration * Time.deltaTime;
        dir = velocity;
        dir.Normalize();
    }
    velocity = dir * curSpeed;
    transform.position += velocity * Time.deltaTime;
    // } DISABLE end 

    // DISABLE { 
    // var linearFB = moveFB * camForward;
    // var linearLR = moveLR * camRight;

    // // FB
    // if ( moveFB ) {
    //     linearFB *= acceleration;
    // }
    // else if ( velocity.x > 0.01 ) {
    //     linearFB = -linearFB;
    //     linearFB *= deceleration;
    // }

    // // LR
    // if ( moveLR ) {
    //     linearLR *= acceleration;
    // }
    // else if ( velocity.z > 0.01 ) {
    //     linearLR = -linearLR;
    //     linearLR *= deceleration;
    // }

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

    // if ( moveFB )
    //     Debug.DrawLine ( transform.position, transform.position + linearFB, Color.red );
    // if ( moveLR )
    //     Debug.DrawLine ( transform.position, transform.position + linearLR, Color.blue );
    // } DEBUG end 
}

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

function OnGUI () {
    GUI.Label ( Rect (10, 10, 200, 50), dbgText.ToString() );
}
