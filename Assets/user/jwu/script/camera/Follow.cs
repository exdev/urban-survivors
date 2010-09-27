// ======================================================================================
// File         : Follow.cs
// Author       : Wu Jie 
// Last Change  : 09/27/2010 | 23:01:05 PM | Monday,September
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////
// defines
///////////////////////////////////////////////////////////////////////////////

[AddComponentMenu("Camera-Control/Topdown Follow")]
public class Follow: MonoBehaviour {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    // The target we are following
    public Transform target;

    // the height we want the camera to be above the target
    public float height = 5.0f;
    public float pitch = 70.0f; 
    public float yaw = -90.0f;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void LateUpdate () {
        // Early out if we don't have a target
        if (!target)
            return;

        //
        Quaternion wanted_rot = Quaternion.identity;
        wanted_rot.eulerAngles = new Vector3( pitch, yaw, 0.0f );
        transform.rotation = wanted_rot;

        //
        float wantedHeight = target.position.y + height;

        //
        Vector3 wantedLocation = target.position;

        //
        transform.position = new Vector3(wantedLocation.x, wantedHeight, wantedLocation.z);
    }
}
