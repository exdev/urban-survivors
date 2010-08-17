// ======================================================================================
// File         : follow.js
// Author       : Wu Jie 
// Last Change  : 08/17/2010 | 21:52:10 PM | Tuesday,August
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

@script AddComponentMenu("Camera-Control/Topdown Follow")

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

    //
    var wanted_rot = Quaternion.identity;
    wanted_rot.eulerAngles = Vector3( pitch, yaw, 0.0 );
    transform.rotation = wanted_rot;

    //
	wantedHeight = target.position.y + height;

    //
    wantedLocation = target.position;

    //
    transform.position = wantedLocation;
    transform.position.y = wantedHeight;
}
