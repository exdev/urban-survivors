// ======================================================================================
// File         : ScreenPad.cs
// Author       : Wu Jie 
// Last Change  : 10/08/2010 | 23:09:04 PM | Friday,October
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
// class ScreenPad 
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

public class ScreenPad : MonoBehaviour {

    public Camera hud_camera;
    public GameObject analog;
    public Circle move_zone;
    public float move_limitation;

    private Vector2 move_dir;
    private List<Touch> available_touches = new List<Touch>();
    private int moveID = -1;

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
        available_touches.Clear();
        Touch move_finger = new Touch();
        bool tracMoveFinger = false;

        // first check if move finger invalid
        foreach ( Touch t in Input.touches ) {
            if ( t.fingerId == moveID ) {
                if ( t.phase == TouchPhase.Ended ||
                     t.phase == TouchPhase.Canceled ) {
                    moveID = -1;
                }
                else {
                    move_finger = t;
                    tracMoveFinger = true;
                }
                break;
            }
        }

        //
        foreach ( Touch t in Input.touches ) {
            if ( t.phase == TouchPhase.Ended ||
                 t.phase == TouchPhase.Canceled ) {
                continue;
            }

            // skip move finger if we tracing it.
            if ( tracMoveFinger ) {
                if ( t.fingerId == moveID ) {
                    continue;
                }
            }
            else {
                Vector2 screenPos = t.position;

                // record the finger in the move zone as move finger
                if ( t.phase == TouchPhase.Began ) {
                    if ( move_zone.Contains(screenPos) ) {
                        move_finger = t;
                        moveID = t.fingerId;
                        continue;
                    }
                }
            }

            // those un-handle touches, will recognized as screen touch.
            available_touches.Add(t);
        }

        // now process move by move_finger
        if ( moveID != -1 ) {
            HandleMove(move_finger.position);
        }

        // DEBUG { 
        // DebugHelper.ScreenPrint("moveID = " + moveID);
        // foreach ( Touch t in Input.touches ) {
        //     DebugHelper.ScreenPrint("touch: " + t);
        // }
        // } DEBUG end 
#else

        // handle keyboard move
        float moveFB = Input.GetAxisRaw("Vertical");
        float moveLR = Input.GetAxisRaw("Horizontal");
        Vector2 dir = new Vector2(moveLR,moveFB);
        Vector2 screenPos = move_limitation * dir.normalized + move_zone.center;
        HandleMove(screenPos);
#endif
        // if there is no move, keep the analog at the center of the move_zone. 
        if ( move_dir.magnitude == 0.0f ) {
            Vector3 worldpos = hud_camera.ScreenToWorldPoint( new Vector3( move_zone.center.x, move_zone.center.y, 1 ) );
            // analog.transform.position = new Vector3( worldpos.x, worldpos.y, analog.transform.position.z ); 

            Hashtable args = iTween.Hash( "position", worldpos,
                                          "time", 0.1f,
                                          "easetype", iTween.EaseType.easeInCubic 
                                        );
            // iTween.MoveTo ( analog, worldpos, 0.2f );
            iTween.MoveTo ( analog, args );
        }
	}

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void HandleMove ( Vector2 _screenPos ) {
        iTween.Stop (analog,"move");
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

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public List<Touch> AvailableTouches () { return available_touches; } 

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public Touch GetLastTouch () { 
        DebugHelper.Assert ( available_touches.Count != 0, "the available_touches is empty." );
        return available_touches[available_touches.Count-1];
    }
}
