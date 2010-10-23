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

    //
    private Vector2 move_dir;
    private int moveID = -1;
    private Vector2 aiming_dir = Vector2.up;
    private int aimingID = -1;
    private List<Touch> available_touches = new List<Touch>();

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public Camera hud_camera;
    public Transform analog;
    public Transform aimingNeedle;

    public Circle move_zone;
    public float move_limitation;

    public Circle aiming_zone;

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
        Touch aiming_finger = new Touch();
        bool tracMoveFinger = false;
        bool tracAimingFinger = false;

        // first check if move finger invalid
        foreach ( Touch t in Input.touches ) {
            // if we found them all, skip search the list.
            if ( tracMoveFinger && tracAimingFinger ) {
                break;
            }

            // we found the the move finger to trac 
            if ( tracMoveFinger == false ) {
                if ( t.fingerId == moveID ) {
                    if ( t.phase == TouchPhase.Ended ||
                         t.phase == TouchPhase.Canceled ) {
                        moveID = -1;
                    }
                    else {
                        move_finger = t;
                        tracMoveFinger = true;
                    }
                    continue;
                }
            }

            // we found the aiming finger to trac
            if ( tracAimingFinger == false ) {
                if ( t.fingerId == aimingID ) {
                    if ( t.phase == TouchPhase.Ended ||
                         t.phase == TouchPhase.Canceled ) {
                        aimingID = -1;
                    }
                    else {
                        aiming_finger = t;
                        tracAimingFinger = true;
                    }
                    continue;
                }
            }
        }

        // NOTE: this will protect the code in UnityRemote mode { 
        if ( tracMoveFinger == false ) moveID = -1;
        if ( tracAimingFinger == false ) aimingID = -1;
        // } NOTE end 

        //
        foreach ( Touch t in Input.touches ) {
            if ( t.phase == TouchPhase.Ended ||
                 t.phase == TouchPhase.Canceled ) {
                continue;
            }

            // skip move/aiming finger if we tracing it.
            if ( t.fingerId == moveID || t.fingerId == aimingID ) {
                continue;
            }

            // record the finger in the move zone as move finger
            Vector2 screenPos = t.position;
            if ( t.phase == TouchPhase.Began ) {
                if ( move_zone.Contains(screenPos) ) {
                    move_finger = t;
                    moveID = t.fingerId;
                    continue;
                }
                else if ( aiming_zone.Contains(screenPos) ) {
                    aiming_finger = t;
                    aimingID = t.fingerId;
                    continue;
                }
            }

            // those un-handle touches, will recognized as screen touch.
            available_touches.Add(t);
        }

        // now process move by move_finger
        if ( moveID != -1 ) {
            HandleMove(move_finger.position);
        }
        if ( aimingID != -1 ) {
            HandleAiming(aiming_finger.position);
        }

        // DEBUG { 
        foreach ( Touch t in Input.touches ) {
            DebugHelper.ScreenPrint("touch position: " + t.position);
        }
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
            iTween.MoveTo ( analog.gameObject, args );
        }
	}

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void HandleMove ( Vector2 _screenPos ) {
        iTween.Stop (analog.gameObject,"move");
        Vector2 delta = _screenPos - move_zone.center;
        move_dir = delta.normalized;
        float len = delta.magnitude;
        Vector2 final_pos = move_zone.center + move_dir * Mathf.Min( len, move_limitation );

        Vector3 worldpos = hud_camera.ScreenToWorldPoint( new Vector3( final_pos.x, final_pos.y, 1 ) );
        analog.position = new Vector3( worldpos.x, worldpos.y, analog.position.z ); 
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void HandleAiming ( Vector2 _screenPos ) {
        // the screen touch priority is higher than aiming_zone
        if ( available_touches.Count != 0 ) {
            Touch t = GetLastTouch();
            // aiming_dir = 
        }
        else {
            Vector2 delta = _screenPos - aiming_zone.center;
            aiming_dir = -delta.normalized;
            Vector2 up = Vector2.up;

            // use cross to get the direction of the rotation.
            float sin_theta = aiming_dir.x * up.y - aiming_dir.y * up.x; 
            float degrees = Vector2.Angle( aiming_dir, Vector2.up );
            aimingNeedle.eulerAngles = new Vector3( 0.0f, 0.0f, -1.0f * degrees * Mathf.Sign(sin_theta) );
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public Vector2 GetMoveDirection () { return move_dir; }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public Vector2 GetAimingDirection () { return aiming_dir; }

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
