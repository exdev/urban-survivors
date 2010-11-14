// ======================================================================================
// File         : Follow_smoothFocus.cs
// Author       : Wu Jie 
// Last Change  : 11/14/2010 | 11:55:08 AM | Sunday,November
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

// Place the script in the Camera-Control group in the component menu
[AddComponentMenu("Camera-Control/Topdown Smooth Follow")]
public class Follow_smoothFocus: MonoBehaviour {

    ///////////////////////////////////////////////////////////////////////////////
    // public properties
    ///////////////////////////////////////////////////////////////////////////////

    // The target we are following
    public Transform target;

    // the height we want the camera to be above the target
    public float Height = 10.0f;
    public float HorizontalOffset = 3.0f;
    public float Yaw = -90.0f;

    // damping
    public float HeightDamping = 2.0f;
    public float HorizontalDamping = 2.0f;
    public float RotationDamping = 3.0f;

    //
    private bool zoomIn = false;

    ///////////////////////////////////////////////////////////////////////////////
    // defines
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void LateUpdate () {
        // Early out if we don't have a target
        if (!target)
            return;

        // ======================================================== 
        // translation
        // ======================================================== 

        // smooth update current height
        float wantedHeight = target.position.y + Height;
        float curHeight = transform.position.y;
        curHeight = Mathf.Lerp (curHeight, wantedHeight, HeightDamping * Time.deltaTime);

        // Set the height of the camera
        Vector3 curPos = transform.position;
        // Vector3 wantedPos = target.position + Vector3.right * HorizontalOffset;

        Quaternion rot = Quaternion.identity;
        rot.eulerAngles = new Vector3(0.0f, Yaw + 90.0f, 0.0f);
        Vector3 dir = rot * Vector3.right;
        Vector3 wantedPos = target.position + dir * HorizontalOffset;

        // smooth update location
        curPos.x = Mathf.Lerp (curPos.x, wantedPos.x, HorizontalDamping * Time.deltaTime);
        curPos.y = curHeight;
        curPos.z = Mathf.Lerp (curPos.z, wantedPos.z, HorizontalDamping * Time.deltaTime);

        // ======================================================== 
        // rotation 
        // ======================================================== 

        // smooth update rotation
        Quaternion wanted_rot = Quaternion.identity;
        wanted_rot = Quaternion.LookRotation( target.position - curPos );
        wanted_rot.eulerAngles = new Vector3( wanted_rot.eulerAngles.x, Yaw, 0.0f );

        // ======================================================== 
        // do real transform 
        // ======================================================== 

        // update the exact position
        transform.rotation = Quaternion.Slerp( transform.rotation, wanted_rot, RotationDamping * Time.deltaTime );
        transform.position = curPos;

        // process zomming
        AdjustZooming ();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void AdjustZooming () {
        Camera cam = GetComponent( typeof(Camera) ) as Camera;
        float wantedFov = 60.0f;
        if ( zoomIn ) {
            wantedFov = 40.0f;
        }
        cam.fieldOfView = Mathf.Lerp (cam.fieldOfView, wantedFov, 8.0f * Time.deltaTime);
    }

}
