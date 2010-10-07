// ======================================================================================
// File         : Player_Control.cs
// Author       : Wu Jie 
// Last Change  : 10/07/2010 | 19:17:04 PM | Thursday,October
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////
// class Player_Control
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

public class Player_Control : MonoBehaviour {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    private HUD hud_info;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	void Start () {
        hud_info = GameObject.Find("HUD").GetComponent(typeof(HUD)) as HUD;
        DebugHelper.Assert( hud_info, "hud_info not found" );
	}
	
    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	void Update () {
        // DEBUG { 
        Vector3 velocity = rigidbody.GetPointVelocity(transform.position);
        DebugHelper.ScreenPrint( "velocity: " + velocity );
        Debug.DrawLine ( transform.position, transform.position + velocity, Color.white );
        // } DEBUG end 
	}

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void FixedUpdate () {
        ProcessMovement ();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    private void ProcessMovement () {

        Vector2 screen_dir = hud_info.GetMoveDirection();
        Vector3 dir = new Vector3( screen_dir.x, screen_dir.y, 0.0f ); 

        Transform mainCamera = Camera.main.GetComponent( typeof(Transform) ) as Transform;
        dir = mainCamera.TransformDirection(dir.normalized); 
        dir.y = 0.0f;
        dir = dir.normalized;

        float maxSpeed = 100.0f; // TEMP
        rigidbody.AddForce ( dir * maxSpeed, ForceMode.Acceleration );
        // rigidbody.AddForce ( dir * maxSpeed, ForceMode.VelocityChange );
        transform.position = new Vector3( transform.position.x, 1.0f, transform.position.z );
    }
}
