// ======================================================================================
// File         : ai_test.js
// Author       : Wu Jie 
// Last Change  : 07/26/2010 | 15:52:06 PM | Monday,July
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// properties
///////////////////////////////////////////////////////////////////////////////

var target : Transform;
var rot_speed = 5.0;

///////////////////////////////////////////////////////////////////////////////
// functions
///////////////////////////////////////////////////////////////////////////////

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

function Update () {
    transform.position.x += 0.1 * Mathf.Cos( Time.time );
    // transform.position.z += 0.1 * Mathf.Sin( Time.time );

    var wanted_rot = Quaternion.LookRotation  ( target.position - transform.position );

    // euler lerp
    // transform.eulerAngles = Vector3( 
    //     Mathf.Lerp ( transform.eulerAngles.x,  wanted_rot.eulerAngles.x, rot_speed * Time.deltaTime ), 
    //     Mathf.Lerp ( transform.eulerAngles.y,  wanted_rot.eulerAngles.y, rot_speed * Time.deltaTime ), 
    //     Mathf.Lerp ( transform.eulerAngles.z,  wanted_rot.eulerAngles.z, rot_speed * Time.deltaTime )
    // );

    // euler angle lerp
    transform.eulerAngles = Vector3(
        Mathf.LerpAngle ( transform.eulerAngles.x,  wanted_rot.eulerAngles.x, rot_speed * Time.deltaTime ), 
        Mathf.LerpAngle ( transform.eulerAngles.y,  wanted_rot.eulerAngles.y, 0.5 * rot_speed * Time.deltaTime ), 
        Mathf.LerpAngle ( transform.eulerAngles.z,  wanted_rot.eulerAngles.z, 0.1 * rot_speed * Time.deltaTime )
    );

    // slerp
    // transform.rotation = Quaternion.Slerp( transform.rotation, wanted_rot, rot_speed * Time.deltaTime );
}
