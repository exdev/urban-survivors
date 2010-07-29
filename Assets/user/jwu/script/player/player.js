// ======================================================================================
// File         : player.js
// Author       : Wu Jie 
// Last Change  : 07/19/2010 | 21:27:33 PM | Monday,July
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// variables
///////////////////////////////////////////////////////////////////////////////

var aimDistance = 10.0;

// A very simplistic car driving on the x-z plane.
var moveSpeed = 5.0;
var rotationSpeed = 100.0;

var aimPos : Vector3;
var aimDir : Vector3;

private var dbgText;

///////////////////////////////////////////////////////////////////////////////
// functions
///////////////////////////////////////////////////////////////////////////////

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

function Start () {
    aimDir = transform.TransformDirection(Vector3.forward);
    aimPos = transform.position + aimDir * aimDistance;
}

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

function Update () {
    // update aim pos
    aimPos = transform.position + aimDir * aimDistance;

    // Get the horizontal and vertical axis.
    var moveFB = Input.GetAxis ("Vertical") * moveSpeed * Time.deltaTime;
    var moveLR = Input.GetAxis ("Horizontal") * moveSpeed * Time.deltaTime;

    var rotationH = Input.GetAxis ("Mouse X") * rotationSpeed * Time.deltaTime;
    var rotationV = Input.GetAxis ("Mouse Y") * rotationSpeed * Time.deltaTime;

    // Move translation along the object's z-axis
    transform.Translate (moveLR, 0, moveFB);

    // Rotate around our y-axis
    var rot = Quaternion.identity;
    rot.eulerAngles = Vector3(rotationV, rotationH, 0.0);
    aimDir = rot * aimDir;

    // DEBUG { 
    dbgText = "roth = " + rotationH + ", rotv = " + rotationV;
    // } DEBUG end 
}

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

function OnGUI () {
    GUI.Label ( Rect (10, 10, 200, 20), dbgText.ToString() );
}
