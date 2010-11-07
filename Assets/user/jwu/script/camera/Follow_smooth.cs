// ======================================================================================
// File         : Follow_smooth.cs
// Author       : Wu Jie 
// Last Change  : 09/27/2010 | 23:04:16 PM | Monday,September
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
public class Follow_smooth: MonoBehaviour {

    ///////////////////////////////////////////////////////////////////////////////
    // public properties
    ///////////////////////////////////////////////////////////////////////////////

    // The target we are following
    public Transform target;

    // the height we want the camera to be above the target
    public float height = 5.0f;
    public float pitch = 70.0f; 
    public float yaw = -90.0f;

    // damping
    public float heightDamping = 2.0f;
    public float horizonDamping = 2.0f;
    public float rotationDamping = 3.0f;

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
        // rotation 
        // ======================================================== 

        // smooth update rotation
        Quaternion wanted_rot = Quaternion.identity;
        wanted_rot.eulerAngles = new Vector3( pitch, yaw, 0.0f );
        transform.rotation = Quaternion.Slerp( transform.rotation, wanted_rot, rotationDamping * Time.deltaTime );

        // ======================================================== 
        // translation
        // ======================================================== 

        // smooth update current height
        float wantedHeight = target.position.y + height;
        float currentHeight = transform.position.y;
        currentHeight = Mathf.Lerp (currentHeight, wantedHeight, heightDamping * Time.deltaTime);

        // Set the height of the camera
        Vector3 currentLocation = transform.position;

        // center the camera to target
        float cos_theta = Vector3.Dot( Vector3.up, transform.forward );
        float distance = cos_theta * currentHeight;
        Vector3 wantedLocation = target.position + transform.forward * distance;

        // smooth update location
        currentLocation.x = Mathf.Lerp (currentLocation.x, wantedLocation.x, horizonDamping * Time.deltaTime);
        currentLocation.z = Mathf.Lerp (currentLocation.z, wantedLocation.z, horizonDamping * Time.deltaTime);

        // ======================================================== 
        // do real transform 
        // ======================================================== 

        // update the exact position
        transform.position = new Vector3 ( currentLocation.x,
                                           currentHeight,
                                           currentLocation.z
                                         );

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
