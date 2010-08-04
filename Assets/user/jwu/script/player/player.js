// ======================================================================================
// File         : player.js
// Author       : Wu Jie 
// Last Change  : 07/19/2010 | 21:27:33 PM | Monday,July
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// variables
///////////////////////////////////////////////////////////////////////////////

// A very simplistic car driving on the x-z plane.
var moveSpeed = 50.0;
var rotationSpeed = 100.0;

private var dbgText = "null";

///////////////////////////////////////////////////////////////////////////////
// functions
///////////////////////////////////////////////////////////////////////////////

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

    // TODO: the move direction should consistence with camera lookat direction { 
    // Get the horizontal and vertical axis.
    var moveFB = Input.GetAxis ("Vertical") * moveSpeed * Time.deltaTime;
    var moveLR = Input.GetAxis ("Horizontal") * moveSpeed * Time.deltaTime;

    // Move translation along the object's z-axis
    transform.Translate (moveLR, 0, moveFB);
    // } TODO end 

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
