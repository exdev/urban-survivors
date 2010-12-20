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
#if UNITY_IPHONE
    private int moveID = -1;
    private int aimingID = -1;
    private int meleeID = -1;
#endif
    private Vector2 aiming_dir = Vector2.up;
    private bool canFire = false;
    private bool meleeButtonDown = false;
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
    public Circle melee_zone;
    public bool useKeyboardAndMouse = false;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	void Start () {
        DebugHelper.Assert( hud_camera != null, "pls assign hud_camera" );
        DebugHelper.Assert( analog != null, "pls assign analog" );
#if !UNITY_IPHONE
        useKeyboardAndMouse = true; // always turn on keyboard and mouse when in PC version.
#endif

        // DEBUG { 
        transform.Find("dev_center").gameObject.SetActiveRecursively(false);
        // } DEBUG end 
    }
	
    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	void Update () {
        if ( useKeyboardAndMouse == false ) {
#if UNITY_IPHONE
            // NOTE: you can use this to check your count. if ( touches.Count == 1 ) {
            move_dir = Vector2.zero;
            available_touches.Clear();
            canFire = false;

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

                    // we found the release touch is meleeID, 
                    if ( t.fingerId == meleeID ) {
                        meleeID = -1;
                        meleeButtonDown = false;
                    }

                    continue;
                }

                // skip move/aiming finger if we tracing it.
                if ( t.fingerId == moveID || 
                     t.fingerId == aimingID ||
                     t.fingerId == meleeID ) {
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
                    else if ( melee_zone.Contains(screenPos) ) {
                        meleeID = t.fingerId;
                        continue;
                    }
                }

                // those un-handle touches, will recognized as screen touch.
                available_touches.Add(t);
            }

            // process move by move_finger
            if ( moveID != -1 ) {
                HandleMove(move_finger.position);
            }
            // process aiming by first check if we have screenPad, then aimingID.
            HandleAiming(aiming_finger.position);
            if ( meleeID != -1 ) {
                meleeButtonDown = true;
            }

            // DEBUG { 
            // foreach ( Touch t in Input.touches ) {
            //     DebugHelper.ScreenPrint("touch position: " + t.position);
            // }
            // } DEBUG end 
#endif
        } else {
            // handle keyboard move
            float moveFB = Input.GetAxisRaw("Vertical");
            float moveLR = Input.GetAxisRaw("Horizontal");
            Vector2 dir = new Vector2(moveLR,moveFB);
            Vector2 screenPos = move_limitation * dir.normalized + move_zone.center;
            HandleMove(screenPos);
            HandleAiming(Vector2.zero);
            this.canFire = Input.GetButton("Fire");
            this.meleeButtonDown = Input.GetKeyDown(KeyCode.Space);
        } // end if ( useKeyboardAndMouse == false )

        // if there is no move, keep the analog at the center of the move_zone. 
        if ( MathHelper.IsZerof(move_dir.sqrMagnitude) ) {
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
            if ( useKeyboardAndMouse == false ) {
#if UNITY_IPHONE
                // the screen touch priority is higher than aiming_zone
                if ( available_touches.Count != 0 ) {
                    GameObject girl = GameRules.Instance().GetPlayerGirl();
                    Vector3 girlScreenPos = Camera.main.WorldToScreenPoint(girl.transform.position);
                    Vector2 girlScreenPos_v2 = new Vector2(girlScreenPos.x, girlScreenPos.y); 

                    Touch t = GetLastTouch();
                    Vector2 delta = t.position - girlScreenPos_v2;
                    aiming_dir = delta.normalized;
                    canFire = true;
                }
                else if ( aimingID != -1 ) {
                    Vector2 delta = _screenPos - aiming_zone.center;
                    aiming_dir = -delta.normalized;
                    canFire = true;
                }
#endif
            } else {
                GameObject girl = GameRules.Instance().GetPlayerGirl();
                Vector3 girlScreenPos = Camera.main.WorldToScreenPoint(girl.transform.position);
                Vector2 girlScreenPos_v2 = new Vector2(girlScreenPos.x, girlScreenPos.y); 
                Vector2 delta = new Vector2(Input.mousePosition.x,Input.mousePosition.y) - girlScreenPos_v2;
                aiming_dir = delta.normalized;
            } // end if ( useKeyboardAndMouse == false )

            // use cross to get the direction of the rotation.
            Vector2 up = Vector2.up;
            float sin_theta = aiming_dir.x * up.y - aiming_dir.y * up.x; 
            float degrees = Vector2.Angle( aiming_dir, Vector2.up );
            aimingNeedle.eulerAngles = new Vector3( 0.0f, 0.0f, -1.0f * degrees * Mathf.Sign(sin_theta) );
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

        public bool CanFire () { return canFire; }

        // ------------------------------------------------------------------ 
        // Desc: 
        // ------------------------------------------------------------------ 

        public bool MeleeButtonDown () { return meleeButtonDown; }

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
