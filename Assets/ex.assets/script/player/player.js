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

    var rotationV = Input.GetAxis ("Vertical.2") * rotationSpeed * Time.deltaTime;
    var rotationH = Input.GetAxis ("Horizontal.2") * rotationSpeed * Time.deltaTime;

    // Move translation along the object's z-axis
    transform.Translate (moveLR, 0, moveFB);

    // Rotate around our y-axis
    var rot = Quaternion.identity;
    rot.eulerAngles = Vector3(0, rotationH, rotationV);
    aimDir = rot * aimDir;
}
