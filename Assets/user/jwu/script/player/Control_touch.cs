// ======================================================================================
// File         : Control_touch.cs
// Author       : Wu Jie 
// Last Change  : 10/02/2010 | 10:27:56 AM | Saturday,October
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

///////////////////////////////////////////////////////////////////////////////
// class define
///////////////////////////////////////////////////////////////////////////////

public class Control_touch : MonoBehaviour {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public Texture tex_MoveAnalog;
    public Texture tex_MoveOutline;

    private float moveOutlineRadius;
    private Vector2 moveOutlineCenter;
    private Vector2 moveAnalogCenter;
    // private Vector2 moveDir;

    // TODO:
    // Layer (LayerStack, active layer, )
    // Controller ( active, combine by widgets )

    ///////////////////////////////////////////////////////////////////////////////
    // function defines
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	void Start () {
        DebugHelper.Assert( tex_MoveAnalog, "pls set the analog texture first!" );
        DebugHelper.Assert( tex_MoveOutline, "pls set the edge texture first!" );

        // TEMP: init analog pos { 
        // init
        int pos_x = 0;
        int pos_y = Screen.height - tex_MoveOutline.height;

        moveOutlineCenter.x = pos_x + tex_MoveOutline.width/2;
        moveOutlineCenter.y = pos_y + tex_MoveOutline.height/2;
        moveOutlineRadius = tex_MoveOutline.width/2 - 40;

        moveAnalogCenter.x = moveOutlineCenter.x;
        moveAnalogCenter.y = moveOutlineCenter.y;
        // } TEMP end 
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
        foreach ( Touch t in touches ) {
            Vector2 screen_pos = new Vector2 ( t.position.x, Screen.height - t.position.y );

            // check if it is in the region 
            Vector2 delta = screen_pos - moveOutlineCenter;
            if ( delta.magnitude <= moveOutlineRadius ) {
                float limit = Mathf.Min(delta.magnitude, moveOutlineRadius - 40);
                moveAnalogCenter = moveOutlineCenter + delta.normalized * limit;
                // moveDir = delta.normalized;
            }
        }
        DebugHelper.ScreenPrint ( "analog center: " + moveAnalogCenter );
	}

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnGUI() {
        GUI.DrawTexture ( new Rect( moveOutlineCenter.x - tex_MoveOutline.width/2, 
                                    moveOutlineCenter.y - tex_MoveOutline.height/2, 
                                    tex_MoveOutline.width, 
                                    tex_MoveOutline.height ), 
                          tex_MoveOutline, 
                          ScaleMode.ScaleToFit, 
                          true, 
                          0.0f );


        GUI.DrawTexture ( new Rect( moveAnalogCenter.x - tex_MoveAnalog.width/2, 
                                    moveAnalogCenter.y - tex_MoveAnalog.height/2, 
                                    tex_MoveAnalog.width, 
                                    tex_MoveAnalog.height ), 
                          tex_MoveAnalog, 
                          ScaleMode.ScaleToFit, 
                          true, 
                          0.0f );
    }
}
