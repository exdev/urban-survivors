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

    public Texture tex_analog;
    public Texture tex_edge;

    private float analog_region;

    private Vector2 analog_pos;

    ///////////////////////////////////////////////////////////////////////////////
    // function defines
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	void Start () {
        DebugHelper.Assert( tex_analog, "pls set the analog texture first!" );
        DebugHelper.Assert( tex_edge, "pls set the edge texture first!" );
        analog_region = tex_edge.width;

        // int pos_x = -20;
        // int pos_y = Screen.height - tex_edge.height;
        analog_pos.x = 40;
        analog_pos.y = Screen.height - 10;
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

        if ( touches.Count == 1 ) {
            Touch t = touches[0];
            analog_pos = t.position;
        }
	}

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnGUI() {
        int pos_x = -20;
        int pos_y = Screen.height - tex_edge.height; 

        GUI.DrawTexture ( new Rect( pos_x, pos_y, tex_edge.width, tex_edge.height ), 
                          tex_edge, 
                          ScaleMode.ScaleToFit, 
                          true, 
                          0.0f );

        int half_width = tex_analog.width / 2;
        int half_height = tex_analog.height / 2;
        GUI.DrawTexture ( new Rect( analog_pos.x - half_width, 
                                    Screen.height - analog_pos.y - half_height, 
                                    tex_analog.width, 
                                    tex_analog.height ), 
                          tex_analog, 
                          ScaleMode.ScaleToFit, 
                          true, 
                          0.0f );
    }
}
