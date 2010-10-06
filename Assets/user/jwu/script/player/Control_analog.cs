// ======================================================================================
// File         : Control_analog.cs
// Author       : Wu Jie 
// Last Change  : 10/06/2010 | 10:33:14 AM | Wednesday,October
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

public class Control_analog : MonoBehaviour {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public float outlineRadius;
    public Vector2 outlineCenter;

    public Transform analog;

    ///////////////////////////////////////////////////////////////////////////////
    // function defines
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	void Start () {
        DebugHelper.Assert( analog, "pls set the analog first!" );
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
            Vector2 delta = screen_pos - outlineCenter;
            if ( delta.magnitude <= outlineRadius ) {
                float limit = Mathf.Min(delta.magnitude, outlineRadius - 40);
                Vector2 analog_center = outlineCenter + delta.normalized * limit; 
                analog.position = new Vector3( analog_center.x, analog_center.y, analog.position.z ); 
                // TODO: moveDir = delta.normalized;
            }
        }
	}
}
