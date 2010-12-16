// ======================================================================================
// File         : test_rotate.js
// Author       : Wu Jie 
// Last Change  : 07/27/2010 | 09:50:49 AM | Tuesday,July
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// properties
///////////////////////////////////////////////////////////////////////////////

var rot_speed = 1.0;

///////////////////////////////////////////////////////////////////////////////
// functions
///////////////////////////////////////////////////////////////////////////////

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

function Update () {
    var wanted_rot = Quaternion.identity;
    wanted_rot.eulerAngles = Vector3 ( 0.0, 0.0, 180.0 );
    // NOTE: when we do this rotation, the quaternion and euler lerp will be different.
    // in this case, quaternion do the perfect job. { 
    // wanted_rot.eulerAngles = Vector3 ( 0.0, 180.0, 180.0 );
    // } NOTE end 

    var algo = 1;
    if ( algo == 0 ) { // euler lerp
        transform.eulerAngles = Vector3( 
            Mathf.Lerp ( transform.eulerAngles.x,  wanted_rot.eulerAngles.x, rot_speed * 0.1 ), 
            Mathf.Lerp ( transform.eulerAngles.y,  wanted_rot.eulerAngles.y, rot_speed * 0.1 ), 
            Mathf.Lerp ( transform.eulerAngles.z,  wanted_rot.eulerAngles.z, rot_speed * 0.1 )
        );
    }
    else if ( algo == 1 ) { // euler angle lerp
        transform.eulerAngles = Vector3(
            Mathf.LerpAngle ( transform.eulerAngles.x,  wanted_rot.eulerAngles.x, rot_speed * 0.1 ), 
            Mathf.LerpAngle ( transform.eulerAngles.y,  wanted_rot.eulerAngles.y, rot_speed * 0.1 ), 
            Mathf.LerpAngle ( transform.eulerAngles.z,  wanted_rot.eulerAngles.z, rot_speed * 0.1 )
        );
    }
    else if ( algo == 2 ) { // slerp
        transform.rotation = Quaternion.Slerp( transform.rotation, wanted_rot, rot_speed * 0.1 );
    }
}
