// ======================================================================================
// File         : aim.js
// Author       : Wu Jie 
// Last Change  : 07/19/2010 | 21:13:19 PM | Monday,July
// Description  : 
// ======================================================================================

// The target we are following
var target : Transform;

// The distance in the x-z plane to the target
var distance = -5.0;

// the height we want the camera to be above the target
var height = 2.0;

// right offset
var right = 2.0;

// How much we 
var moveDamping = 2.0;

// Place the script in the Camera-Control group in the component menu
@script AddComponentMenu("Camera-Control/3rd-shooter")

function LateUpdate () {
	// Early out if we don't have a target
	if (!target)
		return;

    //
    currentRight = transform.position.x;
    currentHeight = transform.position.y;
    currentDistance = transform.position.z;

    wantedPos = target.position
              + target.TransformDirection(Vector3.right) * right 
              + target.TransformDirection(Vector3.up) * height
              + target.TransformDirection(Vector3.forward) * distance;

    // Damp
    transform.position.x = Mathf.Lerp (transform.position.x, wantedPos.x, moveDamping * Time.deltaTime);
    transform.position.y = Mathf.Lerp (transform.position.y, wantedPos.y, moveDamping * Time.deltaTime);
    transform.position.z = Mathf.Lerp (transform.position.z, wantedPos.z, moveDamping * Time.deltaTime);

    // always lookat the aiming point
    var player = target.GetComponent("player");
    transform.LookAt ( player.aimPos );
}
