// ======================================================================================
// File         : HUD.cs
// Author       : Wu Jie 
// Last Change  : 10/03/2010 | 22:14:10 PM | Sunday,October
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

///////////////////////////////////////////////////////////////////////////////
// defines
///////////////////////////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////////////
// class HUD 
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

public class HUD : MonoBehaviour {

    private Camera hud_camera;
    private GameObject move_widget;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	void Start () {
        hud_camera = GetComponent("Camera") as Camera;
        DebugHelper.Assert( hud_camera != null, "can't find child hud_camera" );

        move_widget = transform.Find("Move").gameObject;
        DebugHelper.Assert( move_widget != null, "can't find child Move" );

        // DEBUG { 
        transform.Find("dev_center").gameObject.SetActiveRecursively(false);
        // } DEBUG end 
    }
	
    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	void Update () {
        List<Touch> touches = new List<Touch>();
        foreach ( Touch t in Input.touches ) {
            if (t.phase != TouchPhase.Ended && t.phase != TouchPhase.Canceled)
                touches.Add(t);
        }

        // NOTE: you can use this to check your count. if ( touches.Count == 1 ) {
        if ( touches.Count == 1 ) {
            Touch t = touches[0];
            // DEBUG { 
            Vector2 screen_pos = new Vector2 ( t.position.x, t.position.y );
            Vector3 worldpos = hud_camera.ScreenToWorldPoint( new Vector3( screen_pos.x, screen_pos.y, 1 ) );
            DebugHelper.ScreenPrint ( "touch point in screen: " + screen_pos );
            DebugHelper.ScreenPrint ( "touch point in world: " + worldpos );
            // } DEBUG end 
        }
	}
}
