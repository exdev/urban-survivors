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
#if UNITY_IPHONE
    int moveID = -1;
    int aimingID = -1;
    int meleeID = -1;
    List<Touch> availableTouches = new List<Touch>();
#endif
    Vector2 moveDir;
    Vector2 aimingDir = Vector2.up;
    float shootCounter = 0.0f;
    bool meleeButtonDown = false;

    Circle moveZone = new Circle();
    Circle aimingZone = new Circle();
    Rect meleeZone = new Rect();
    Transform moveAnalog;

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public Camera hudCamera;
    public Transform moveAnchor;
    public float moveLimitation;
    public Transform aimingAnchor;
    public Transform meleeOutline;

    public bool useRemoteTouch = false;
    public float shootingDuration = 3.0f;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	void Awake () {
        DebugHelper.Assert( this.hudCamera != null, "pls assign hudCamera" );
        DebugHelper.Assert( this.moveAnchor != null, "pls assign move anchor" );
        DebugHelper.Assert( this.aimingAnchor != null, "pls assign aiming anchor" );
        DebugHelper.Assert( this.meleeOutline != null, "pls assign meleeOutline" );

        // always turn on keyboard and mouse when in PC version.
#if !UNITY_IPHONE
        this.useRemoteTouch = false; 
#else
        if ( Application.isEditor == false )
            this.useRemoteTouch = true; 
#endif

        // DEBUG: used for developing
        transform.Find("DEV_CENTER").gameObject.SetActiveRecursively(false);

        //
        this.moveAnalog = this.moveAnchor.Find("Analog");
        DebugHelper.Assert( this.moveAnalog != null, "pls assign moveAnalog" );
        this.moveZone.center = this.hudCamera.WorldToScreenPoint(this.moveAnchor.position); 
        this.moveZone.radius = 69.0f;

        //
        // this.aimingNeedle = this.aimingAnchor.Find("Needle");
        // DebugHelper.Assert( this.aimingNeedle != null, "pls assign aimingNeedle" );
        this.aimingZone.center = this.hudCamera.WorldToScreenPoint(this.aimingAnchor.position); 
        this.aimingZone.radius = 64.0f;

        //
        PackedSprite sp = meleeOutline.GetComponent<PackedSprite>();
        int half_width = (int)sp.width/2;
        int half_height = (int)sp.height/2;
        Vector2 meleePos = this.hudCamera.WorldToScreenPoint(this.meleeOutline.position);
        this.meleeZone = new Rect( meleePos.x - half_width,  
                                   meleePos.y - half_height, 
                                   sp.width, 
                                   sp.height ); 
    }
	
    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	void Update () {
        // minus shoot duration
        if ( this.shootCounter > 0.0f )
            this.shootCounter -= Time.deltaTime;

        if ( this.useRemoteTouch ) {
#if UNITY_IPHONE
            // NOTE: you can use this to check your count. if ( touches.Count == 1 ) {
            this.moveDir = Vector2.zero;
            this.availableTouches.Clear();

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
                    if ( t.fingerId == this.moveID ) {
                        if ( t.phase == TouchPhase.Ended ||
                             t.phase == TouchPhase.Canceled ) {
                            this.moveID = -1;
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
                    if ( t.fingerId == this.aimingID ) {
                        if ( t.phase == TouchPhase.Ended ||
                             t.phase == TouchPhase.Canceled ) {
                            this.aimingID = -1;
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
            if ( tracMoveFinger == false ) this.moveID = -1;
            if ( tracAimingFinger == false ) this.aimingID = -1;
            // } NOTE end 

            //
            foreach ( Touch t in Input.touches ) {
                if ( t.phase == TouchPhase.Ended ||
                     t.phase == TouchPhase.Canceled ) {

                    // we found the release touch is meleeID, 
                    if ( t.fingerId == this.meleeID ) {
                        this.meleeID = -1;
                        meleeButtonDown = false;
                    }

                    continue;
                }

                // skip move/aiming finger if we tracing it.
                if ( t.fingerId == this.moveID || 
                     t.fingerId == this.aimingID ||
                     t.fingerId == this.meleeID ) {
                    continue;
                }

                // record the finger in the move zone as move finger
                Vector2 screenPos = t.position;
                if ( t.phase == TouchPhase.Began ) {
                    if ( this.moveZone.Contains(screenPos) ) {
                        move_finger = t;
                        this.moveID = t.fingerId;
                        continue;
                    }
                    else if ( this.aimingZone.Contains(screenPos) ) {
                        aiming_finger = t;
                        this.aimingID = t.fingerId;
                        continue;
                    }
                    else if ( this.meleeZone.Contains(screenPos) ) {
                        this.meleeID = t.fingerId;
                        continue;
                    }
                }

                // those un-handle touches, will recognized as screen touch.
                this.availableTouches.Add(t);
            }

            // process move by move_finger
            if ( this.moveID != -1 ) {
                HandleMove(move_finger.position);
            }
            // process aiming by first check if we have screenPad, then aimingID.
            HandleAiming(aiming_finger.position);
            if ( this.meleeID != -1 ) {
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
                Vector2 screenPos = this.moveLimitation * dir.normalized + this.moveZone.center;
                HandleMove(screenPos);
                HandleAiming(Vector2.zero);
                if ( Input.GetButton("Fire") )
                    this.shootCounter = this.shootingDuration;
                this.meleeButtonDown = Input.GetKeyDown(KeyCode.Space);
            } // end if ( !this.useRemoteTouch )

            // if there is no move, keep the moveAnalog at the center of the moveZone. 
            if ( MathHelper.IsZerof(this.moveDir.sqrMagnitude) ) {
                Vector3 worldpos = this.hudCamera.ScreenToWorldPoint( new Vector3( this.moveZone.center.x, this.moveZone.center.y, 1 ) );
                // moveAnalog.transform.position = new Vector3( worldpos.x, worldpos.y, moveAnalog.transform.position.z ); 

                Hashtable args = iTween.Hash( "position", worldpos,
                                              "time", 0.1f,
                                              "easetype", iTween.EaseType.easeInCubic 
                                            );
                // iTween.MoveTo ( moveAnalog, worldpos, 0.2f );
                iTween.MoveTo ( this.moveAnalog.gameObject, args );
            }
        }

        // ------------------------------------------------------------------ 
        // Desc: 
        // ------------------------------------------------------------------ 

        void HandleMove ( Vector2 _screenPos ) {
            iTween.Stop (this.moveAnalog.gameObject,"move");
            Vector2 delta = _screenPos - this.moveZone.center;
            this.moveDir = delta.normalized;
            float len = delta.magnitude;
            Vector2 final_pos = this.moveZone.center + this.moveDir * Mathf.Min( len, this.moveLimitation );

            Vector3 worldpos = this.hudCamera.ScreenToWorldPoint( new Vector3( final_pos.x, final_pos.y, 1 ) );
            this.moveAnalog.position = new Vector3( worldpos.x, worldpos.y, this.moveAnalog.position.z ); 
        }

        // ------------------------------------------------------------------ 
        // Desc: 
        // ------------------------------------------------------------------ 

        void HandleAiming ( Vector2 _screenPos ) {
            if ( this.useRemoteTouch ) {
#if UNITY_IPHONE
                // the screen touch priority is higher than aimingZone
                if ( this.availableTouches.Count != 0 ) {
                    GameObject girl = GameRules.Instance().GetPlayerGirl();
                    Vector3 girlScreenPos = Camera.main.WorldToScreenPoint(girl.transform.position);
                    Vector2 girlScreenPos_v2 = new Vector2(girlScreenPos.x, girlScreenPos.y); 

                    Touch t = GetLastTouch();
                    Vector2 delta = t.position - girlScreenPos_v2;
                    this.aimingDir = delta.normalized;
                    this.shootCounter = this.shootingDuration;
                }
                else if ( this.aimingID != -1 ) {
                    Vector2 delta = _screenPos - this.aimingZone.center;
                    this.aimingDir = -delta.normalized;
                    this.shootCounter = this.shootingDuration;
                }
#endif
            } else {
                GameObject girl = GameRules.Instance().GetPlayerGirl();
                Vector3 girlScreenPos = Camera.main.WorldToScreenPoint(girl.transform.position);
                Vector2 girlScreenPos_v2 = new Vector2(girlScreenPos.x, girlScreenPos.y); 
                Vector2 delta = new Vector2(Input.mousePosition.x,Input.mousePosition.y) - girlScreenPos_v2;
                this.aimingDir = delta.normalized;
            } // end if ( !this.useRemoteTouch )

            // use cross to get the direction of the rotation.
            Vector2 up = Vector2.up;
            float sin_theta = this.aimingDir.x * up.y - this.aimingDir.y * up.x; 
            float degrees = Vector2.Angle( this.aimingDir, Vector2.up );
            this.aimingAnchor.localEulerAngles = new Vector3( 0.0f, 0.0f, -1.0f * degrees * Mathf.Sign(sin_theta) );
        }

        // ------------------------------------------------------------------ 
        // Desc: 
        // ------------------------------------------------------------------ 

        public Vector2 GetMoveDirection () { return this.moveDir; }

        // ------------------------------------------------------------------ 
        // Desc: 
        // ------------------------------------------------------------------ 

        public Vector2 GetAimingDirection () { return this.aimingDir; }

        // ------------------------------------------------------------------ 
        // Desc: 
        // ------------------------------------------------------------------ 

        public bool CanShoot () { return this.shootCounter > 0.0f; }

        // ------------------------------------------------------------------ 
        // Desc: 
        // ------------------------------------------------------------------ 

        public bool MeleeButtonDown () { return meleeButtonDown; }

        // ------------------------------------------------------------------ 
        // Desc: 
        // ------------------------------------------------------------------ 

        public List<Touch> AvailableTouches () { return this.availableTouches; } 

        // ------------------------------------------------------------------ 
        // Desc: 
        // ------------------------------------------------------------------ 

        public Touch GetLastTouch () { 
            DebugHelper.Assert ( this.availableTouches.Count != 0, "the availableTouches is empty." );
            return this.availableTouches[this.availableTouches.Count-1];
        }
}
