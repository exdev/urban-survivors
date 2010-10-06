// ======================================================================================
// File         : HUD_noused.cs
// Author       : Wu Jie 
// Last Change  : 10/06/2010 | 08:18:27 AM | Wednesday,October
// Description  : this script is just backup what I have researched in using SM2 for HUD 
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

public class HUD_noused : MonoBehaviour {

    private Camera hud_camera;
    private GameObject move_widget;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    IEnumerator Init () {
        yield return new WaitForEndOfFrame();

        PackedSprite sprite = move_widget.transform.Find("Outline").GetComponent("PackedSprite") as PackedSprite;
        DebugHelper.Assert( sprite != null, "can't find PackedSprite" );

        float worldUnitsPerScreenPixel = Vector3.Distance(hud_camera.ScreenToWorldPoint(new Vector3(0, 1, 10)), hud_camera.ScreenToWorldPoint(new Vector3(0, 0, 10)));
        float screen_width = sprite.width / worldUnitsPerScreenPixel;
        float screen_height = sprite.height / worldUnitsPerScreenPixel;
        Vector3 worldpos = hud_camera.ScreenToWorldPoint( new Vector3( screen_width/2, screen_height/2, 1 ) );
        move_widget.transform.position = worldpos;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	void Start () {
        hud_camera = GetComponent("Camera") as Camera;
        DebugHelper.Assert( hud_camera != null, "can't find child hud_camera" );

        move_widget = transform.Find("Move").gameObject;
        DebugHelper.Assert( move_widget != null, "can't find child Move" );

        // we need to wait to the end of the frame to decide the size of the sprite.
        StartCoroutine(Init());
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

