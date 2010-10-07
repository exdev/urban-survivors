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

    public Camera hud_camera;
    public GameObject analog;
    public Circle move_zone;
    public float move_limitation;

    private Vector2 move_dir;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	void Start () {
        DebugHelper.Assert( hud_camera != null, "pls assign hud_camera" );
        DebugHelper.Assert( analog != null, "pls assign analog" );

        // DEBUG { 
        transform.Find("dev_center").gameObject.SetActiveRecursively(false);
        // } DEBUG end 
    }
	
    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	void Update () {
#if UNITY_IPHONE
// #if FALSE
        // NOTE: you can use this to check your count. if ( touches.Count == 1 ) {
        move_dir = Vector2.zero;
        foreach ( Touch t in Input.touches ) {
            Vector2 screenPos = new Vector2 ( t.position.x, t.position.y );
            // DEBUG { 
            DebugHelper.ScreenPrint ( "touch point in screen: " + screenPos );
            // float len = (screenPos - move_zone.center).magnitude;
            // DebugHelper.ScreenPrint ( "len is " + len );
            // } DEBUG end 

            // touch move
            if ( move_zone.Contains(screenPos) ) {
                HandleMove(screenPos);
            }
        }
#else
        Vector2 screenPos = new Vector2 ( Input.mousePosition.x, Input.mousePosition.y );
        // DEBUG { 
        DebugHelper.ScreenPrint ( "mouse point in screen: " + screenPos );
        // } DEBUG end 

        // touch move
        if ( Input.GetMouseButton(0) && move_zone.Contains(screenPos) ) {
            HandleMove(screenPos);
        }
#endif
	}

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void HandleMove ( Vector2 _screenPos ) {
        Vector2 delta = _screenPos - move_zone.center;
        move_dir = delta.normalized;
        float len = delta.magnitude;
        Vector2 final_pos = move_zone.center + move_dir * Mathf.Min( len, move_limitation );

        Vector3 worldpos = hud_camera.ScreenToWorldPoint( new Vector3( final_pos.x, final_pos.y, 1 ) );
        analog.transform.position = new Vector3( worldpos.x, worldpos.y, analog.transform.position.z ); 
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public Vector2 GetMoveDirection () { return move_dir; }
}
