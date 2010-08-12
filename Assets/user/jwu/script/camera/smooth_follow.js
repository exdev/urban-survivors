// ======================================================================================
// File         : smooth_follow.js
// Author       : Wu Jie 
// Last Change  : 08/12/2010 | 21:49:15 PM | Thursday,August
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// public properties
///////////////////////////////////////////////////////////////////////////////

// The target we are following
var target : Transform;

 // the height we want the camera to be above the target
var height = 5.0;
var pitch = 70.0; 
var yaw = -90.0;

// damping
var heightDamping = 2.0;
var horizonDamping = 2.0;
var rotationDamping = 3.0;

// Place the script in the Camera-Control group in the component menu
@script AddComponentMenu("Camera-Control/Topdown Smooth Follow")

///////////////////////////////////////////////////////////////////////////////
// defines
///////////////////////////////////////////////////////////////////////////////

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

function LateUpdate () {
	// Early out if we don't have a target
	if (!target)
		return;

    // ======================================================== 
    // rotation 
    // ======================================================== 

    // smooth update rotation
    var wanted_rot = Quaternion.identity;
    wanted_rot.eulerAngles = Vector3( pitch, yaw, 0.0 );
    transform.rotation = Quaternion.Slerp( transform.rotation, wanted_rot, rotationDamping * Time.deltaTime );
	
    // ======================================================== 
    // translation
    // ======================================================== 

    // smooth update current height
	wantedHeight = target.position.y + height;
	currentHeight = transform.position.y;
	currentHeight = Mathf.Lerp (currentHeight, wantedHeight, heightDamping * Time.deltaTime);
	
	// Set the height of the camera
	currentLocation = transform.position;

    // center the camera to target
    cos_theta = Vector3.Dot( Vector3.up, transform.forward );
    distance = cos_theta * currentHeight;
    wantedLocation = target.position + transform.forward * distance;

    // smooth update location
    currentLocation.x = Mathf.Lerp (currentLocation.x, wantedLocation.x, horizonDamping * Time.deltaTime);
    currentLocation.z = Mathf.Lerp (currentLocation.z, wantedLocation.z, horizonDamping * Time.deltaTime);

    // ======================================================== 
    // 
    // ======================================================== 

    // update the exact position
    transform.position = currentLocation;
    transform.position.y = currentHeight;
}
