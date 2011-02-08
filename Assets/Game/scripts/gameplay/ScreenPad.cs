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
    int moveID = -1;
    int aimingID = -1;
    int meleeID = -1;
    int reloadID = -1;

    Circle meleeZone = new Circle();
    Circle reloadZone = new Circle();
    Circle moveZone = new Circle();
    Circle aimingZone = new Circle();
    List<Touch> availableTouches = new List<Touch>();

    Vector2 moveDir = Vector2.zero;
    Vector2 aimingDir = Vector2.up;
    float shootCounter = 0.0f;
    Transform moveAnalog;

    int meleeButtonStat = 0; // 1 down, 2 up
    int reloadButtonStat = 0; // 1 down, 2 up
    int moveZoneStat = 0; // 1 down, 2 up
    int aimingZoneStat = 0; // 1 down, 2 up

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public Camera hudCamera;
    public Transform moveAnchor;
    public float moveLimitation;
    public Transform aimingAnchor;
    public Transform meleeOutline;
    public Transform reloadOutline;

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
        DebugHelper.Assert( this.reloadOutline != null, "pls assign reloadOutline" );

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

        this.meleeZone.center = this.hudCamera.WorldToScreenPoint(this.meleeOutline.position);
        this.meleeZone.radius = 50.0f;

        this.reloadZone.center = this.hudCamera.WorldToScreenPoint(this.reloadOutline.position);
        this.reloadZone.radius = 50.0f;
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

            this.reloadButtonStat = 0;
            this.meleeButtonStat = 0;
            this.aimingZoneStat = 0;
            this.moveZoneStat = 0;

            // first check if move finger invalid
            Touch move_finger = new Touch();
            Touch aiming_finger = new Touch();
            foreach ( Touch t in Input.touches ) {
                // if we just touch the zone
                if ( t.phase == TouchPhase.Began ) 
                {
                    if ( this.moveID == -1 && this.moveZone.Contains(t.position) ) {
                        this.moveID = t.fingerId;
                        move_finger = t;
                        this.moveZoneStat = 1;
                        continue;
                    }
                    else if ( this.aimingID == -1 && this.aimingZone.Contains(t.position) ) {
                        this.aimingID = t.fingerId;
                        aiming_finger = t;
                        this.aimingZoneStat = 1;
                        continue;
                    }
                    else if ( this.meleeID == -1 && this.meleeZone.Contains(t.position) ) {
                        this.meleeID = t.fingerId;
                        this.meleeButtonStat = 1;
                        continue;
                    }
                    else if ( this.reloadID == -1 && this.reloadZone.Contains(t.position) ) {
                        this.reloadID = t.fingerId;
                        this.reloadButtonStat = 1;
                        continue;
                    }
                }

                // check fingerId
                if ( t.fingerId == this.moveID ) {
                    if ( t.phase == TouchPhase.Ended ) {
                        this.moveID = -1;
                        this.moveZoneStat = 2;
                    }
                    else
                        move_finger = t;
                }
                else if ( t.fingerId == this.aimingID ) {
                    if ( t.phase == TouchPhase.Ended ) {
                        this.aimingID = -1;
                        this.aimingZoneStat = 2;
                    }
                    else
                        aiming_finger = t;
                }
                else if ( t.fingerId == this.meleeID ) {
                    if ( t.phase == TouchPhase.Ended ) {
                        this.meleeID = -1;
                        this.meleeButtonStat = 2;
                    }
                }
                else if ( t.fingerId == this.reloadID ) {
                    if ( t.phase == TouchPhase.Ended ) {
                        this.reloadID = -1;
                        this.reloadButtonStat = 2;
                    }
                }
                // if none of them are the above IDs, add them to availableTouches.
                else {
                    this.availableTouches.Add(t);
                }
            }

            // process move by move_finger
            if ( this.moveID != -1 ) {
                HandleMove( move_finger.position );
            }

            // process aiming by first check if we have screenPad, then aimingID.
            HandleAiming(aiming_finger.position);

            // DEBUG { 
            // foreach ( Touch t in Input.touches ) {
            //     DebugHelper.ScreenPrint("touch position: " + t.position);
            // }
            // } DEBUG end 
#endif
            } else {
                this.meleeButtonStat = 0;
                this.reloadButtonStat = 0;

                // handle keyboard move
                float moveFB = Input.GetAxisRaw("Vertical");
                float moveLR = Input.GetAxisRaw("Horizontal");
                Vector2 dir = new Vector2(moveLR,moveFB);
                Vector2 screenPos = this.moveLimitation * dir.normalized + this.moveZone.center;
                HandleMove(screenPos);
                HandleAiming(Vector2.zero);
                if ( Input.GetButton("Fire") )
                    this.shootCounter = this.shootingDuration;

                if ( Input.GetKeyDown(KeyCode.Space) )
                    this.meleeButtonStat = 1;
                else if ( Input.GetKeyUp(KeyCode.Space) )
                    this.meleeButtonStat = 2;

                if ( Input.GetKeyDown(KeyCode.R) )
                    this.reloadButtonStat = 1;
                else if ( Input.GetKeyUp(KeyCode.R) )
                    this.reloadButtonStat = 2;
                this.reloadID = Input.GetKey(KeyCode.R) ? 1:-1; 
                this.meleeID = Input.GetKey(KeyCode.Space) ? 1:-1; 
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
                    GameObject girl = GameRules.Instance().GetPlayerGirl().gameObject;
                    Vector3 girlScreenPos = Camera.main.WorldToScreenPoint(girl.transform.position);
                    Vector2 girlScreenPos_v2 = new Vector2(girlScreenPos.x, girlScreenPos.y); 

                    Touch t = GetLastTouch();
                    Vector2 delta = t.position - girlScreenPos_v2;
                    this.aimingDir = delta.normalized;
                    this.shootCounter = this.shootingDuration;
                }
                else if ( this.aimingID != -1 ) {
                    Vector2 delta = _screenPos - this.aimingZone.center;
                    Vector2 desiredDir = delta.normalized; 
                    // DISABLE: this.aimingDir = -delta.normalized; // this is old method, inverse needle
                    // adjust the aiming direction by 
                    desiredDir = (GameRules.Instance().GetPlayerGirl() as PlayerGirl).GetAutoLockDir(desiredDir);
                    this.aimingDir = desiredDir; 
                    this.shootCounter = this.shootingDuration;
                }
#endif
            } else {
                GameObject girl = GameRules.Instance().GetPlayerGirl().gameObject;
                Vector3 girlScreenPos = Camera.main.WorldToScreenPoint(girl.transform.position);
                Vector2 girlScreenPos_v2 = new Vector2(girlScreenPos.x, girlScreenPos.y); 
                Vector2 delta = new Vector2(Input.mousePosition.x,Input.mousePosition.y) - girlScreenPos_v2;
                Vector2 desiredDir = delta.normalized; 
                desiredDir = (GameRules.Instance().GetPlayerGirl() as PlayerGirl).GetAutoLockDir(desiredDir);
                this.aimingDir = desiredDir;
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
        public bool MoveZoneTouching () { return this.moveID != -1; }
        public bool MoveZoneDown () { return this.moveZoneStat == 1; }
        public bool MoveZoneUp () { return this.moveZoneStat == 2; }

        // ------------------------------------------------------------------ 
        // Desc: 
        // ------------------------------------------------------------------ 

        public Vector2 GetAimingDirection () { return this.aimingDir; }
        public bool AimingZoneTouching () { return this.aimingID != -1; }
        public bool AimingZoneDown () { return this.aimingZoneStat == 1; }
        public bool AimingZoneUp () { return this.aimingZoneStat == 2; }

        // ------------------------------------------------------------------ 
        // Desc: 
        // ------------------------------------------------------------------ 

        public bool CanShoot () { return this.shootCounter > 0.0f; }

        // ------------------------------------------------------------------ 
        // Desc: 
        // ------------------------------------------------------------------ 

        public bool MeleeButtonDown () { return this.meleeButtonStat == 1; }
        public bool MeleeButtonUp () { return this.meleeButtonStat == 2; }
        public bool MeleeButtonPressing () { return this.meleeID != -1; }

        // ------------------------------------------------------------------ 
        // Desc: 
        // ------------------------------------------------------------------ 

        public bool ReloadButtonDown () { return this.reloadButtonStat == 1; }
        public bool ReloadButtonUp () { return this.reloadButtonStat == 2; }
        public bool ReloadButtonPressing () { return this.reloadID != -1; }

#if UNITY_IPHONE
        // ------------------------------------------------------------------ 
        // Desc: 
        // ------------------------------------------------------------------ 

        protected Touch GetLastTouch () { 
            DebugHelper.Assert ( this.availableTouches.Count != 0, "the availableTouches is empty." );
            return this.availableTouches[this.availableTouches.Count-1];
        }
#endif
}
