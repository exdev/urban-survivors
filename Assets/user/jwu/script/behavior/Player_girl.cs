// ======================================================================================
// File         : Player_girl.cs
// Author       : Wu Jie 
// Last Change  : 10/12/2010 | 00:44:29 AM | Tuesday,October
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////
// class 
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

[RequireComponent (typeof (Animation))]
public class Player_girl : Player_base {

    // private
    private Vector3 moveDir = Vector3.zero;
    private Vector3 aimDir = Vector3.forward;
    // DISABLE: private bool rotating = false;

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public float degreePlayMoveLeftRight = 60.0f;

    // Q: why don't we just use UpperBody in this game?
    // A: this method will make sure the 'upper-body' is specific by user regardless the name of the entity, 
    //    so it can be flexiable enough for different game.
    public Transform upperBody;
    public Transform lowerBody;

    public GameObject followTarget;
    public float followDistance = 1.5f;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	protected new void Start () {
        base.Start();
        DebugHelper.Assert( degreePlayMoveLeftRight <= 90.0f, "the degree can't larger than 90.0" );
        InitAnim ();
        InitFSM ();
	}
	
    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	void Update () {
        HandleInput ();
        // TODO: this.fsm.tick(); // update state machine
        ProcessMovement (); // handle steering

        ProcessAnimation (); // DELME

        // DEBUG { 
        // draw velocity
        Vector3 vel = base.Velocity(); 
        DebugHelper.DrawLine ( transform.position, 
                               transform.position + vel,
                               new Color(0.0f,1.0f,0.0f) );
        // draw smoothed acceleration
        Vector3 acc = base.smoothedAcceleration;
        DebugHelper.DrawLine ( transform.position, 
                               transform.position + acc,
                               new Color(1.0f,0.0f,1.0f) );

        // debug info
        // DebugHelper.ScreenPrint ( "Player_girl current state: " + this.fsm.CurrentState().name );
        // } DEBUG end 
	}

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void LateUpdate () {
        PostAnim ();

        // reset the internal state.
        this.moveDir = Vector3.zero; 
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    private void InitAnim () {
        // get the animation component
        anim = gameObject.GetComponent(typeof(Animation)) as Animation;

        // animation
        AnimationState state;
        string[] anim_keys = { "moveForward", "moveBackward", "moveRight", "moveLeft" };
        foreach (string key in anim_keys) {
            state = anim[key];
            state.layer = 0;
            state.wrapMode = WrapMode.Loop;
            state.weight = 1.0f;
            state.enabled = true;
        }

        state = anim["turnRight"];
        state.wrapMode = WrapMode.Once;
        state.layer = 0;
        state.weight = 1.0f;

        state = anim["turnLeft"];
        state.wrapMode = WrapMode.Once;
        state.layer = 0;
        state.weight = 1.0f;

        state = anim["idle"];
        state.wrapMode = WrapMode.Loop;
        state.layer = 0;
        state.weight = 1.0f;
        state.enabled = true;

        anim.SyncLayer(0);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void InitFSM () {

        // // ======================================================== 
        // // setup states
        // // ======================================================== 

        // // idle
        // FSM.State state_idle = new FSM.State( "Idle", 
        //                                       new Action_PlayAnim(this.anim,"idle"), 
        //                                       null, 
        //                                       null );
        // // walk
        // FSM.State state_walk = new FSM.State( "Walk", 
        //                                       new Action_PlayAnim(this.anim,"moveforward"), 
        //                                       new Action_AdjustMoveSpeed(this.anim,this),
        //                                       null );
        // // melee
        // FSM.State state_melee = new FSM.State( "Melee", 
        //                                        new Action_StartCombo(this), 
        //                                        new Action_NextCombo(this), 
        //                                        null );

        // // ======================================================== 
        // // condition 
        // // ======================================================== 

        // FSM.Condition cond_isMoving = new Condition_isMoving(this);
        // FSM.Condition cond_isMeleeButtonTriggered = new Condition_isMeleeButtonTriggered(this);
        // FSM.Condition cond_isAttacking = new Condition_isAttacking(this);

        // // ======================================================== 
        // // setup transitions
        // // ======================================================== 

        // // idle to ...
        // state_idle.AddTransition( new FSM.Transition( state_walk, cond_isMoving, null ) );
        // state_idle.AddTransition( new FSM.Transition( state_melee, cond_isMeleeButtonTriggered, null ) );

        // // walk to ...
        // state_walk.AddTransition( new FSM.Transition( state_idle, new FSM.Condition_not(cond_isMoving), null ) );
        // state_walk.AddTransition( new FSM.Transition( state_melee, cond_isMeleeButtonTriggered, null ) );

        // // melee to ...
        // state_melee.AddTransition( new FSM.Transition( state_idle, 
        //                                                new FSM.Condition_and(new FSM.Condition_not(cond_isMoving),
        //                                                                      new FSM.Condition_not(cond_isAttacking)), 
        //                                                null ) );
        // state_melee.AddTransition( new FSM.Transition( state_walk, 
        //                                                new FSM.Condition_and(cond_isMoving,
        //                                                                      new FSM.Condition_not(cond_isAttacking)), 
        //                                                null ) );

        // init fsm
        // this.fsm.init(state_idle);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    private void HandleInput() {
        // get move direction
        if ( this.followTarget == null ) {
            Vector2 screen_dir = screenPad.GetMoveDirection();
            if ( screen_dir.sqrMagnitude >= 0.0f ) {
                this.moveDir.x = screen_dir.x;
                this.moveDir.y = screen_dir.y;
                Transform mainCamera = Camera.main.transform;
                this.moveDir = mainCamera.TransformDirection(this.moveDir.normalized); 
                this.moveDir.y = 0.0f;
                this.moveDir = this.moveDir.normalized;
            }
        }

        // get direction by screenPad
        Vector2 aimDir2D = screenPad.GetAimingDirection();
        aimDir = Vector3.zero; 
        aimDir.x = aimDir2D.x; aimDir.y = aimDir2D.y; 
        aimDir = Camera.main.transform.TransformDirection(aimDir);
        aimDir.y = 0.0f;
        aimDir = aimDir.normalized;

        // if we have weapon in hand.
        if ( screenPad.CanFire() ) {
            if ( curWeapon ) {
                Fire fire = curWeapon.GetComponent(typeof(Fire)) as Fire;
                if (fire) {
                    fire.Trigger();
                }
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    private void ProcessMovement () {
        if ( this.followTarget != null ) {
            Vector3 dir = (transform.position - this.followTarget.transform.position).normalized;
            // DISABLE: Vector3 dir = -this.followTarget.transform.forward;
            Vector3 destination = dir * this.followDistance + this.followTarget.transform.position;
            Vector3 delta = destination - transform.position;

            if ( delta.magnitude < 0.5f ) {
                ApplyBrakingForce(10.0f);
            }
            else {
                this.moveDir = delta.normalized; 
            }

            // DEBUG { 
            DebugHelper.DrawCircleY( this.followTarget.transform.position, this.followDistance, Color.yellow );
            DebugHelper.DrawBall( destination, 0.2f, Color.green );
            // } DEBUG end 
        }

        //
        if ( MathHelper.IsZerof(this.moveDir.sqrMagnitude) )
            ApplyBrakingForce(10.0f);
        ApplySteeringForce( this.moveDir * base.maxForce );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    private void ProcessAnimation () {
        // get the animation forward,right so that we can pickup the proper animation.
        Vector3 curVelocity = controller.velocity; 
        Vector3 vel_ubspace = upperBody.worldToLocalMatrix * curVelocity;
        vel_ubspace.y = 0.0f;
        vel_ubspace.Normalize();

        // TODO: if nothings move, crossfade to idle... so rotate, movement no need for idle. { 
        // if ( vel_ubspace.sqrMagnitude < 0.2 )
        if ( this.curSpeed < 0.1  ) {
            // float fadeSpeed = 5.0f * Time.deltaTime;
            anim.CrossFade("idle");
        }
        // } TODO end 
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    private void PostAnim () {
        // process lower-body rotation
        float angle = Vector3.Angle ( moveDir, aimDir );
        // DebugHelper.ScreenPrint("angle = " + angle); // DEBUG
        string animName = "";
        if ( MathHelper.IsZerof(moveDir.sqrMagnitude) == false ) {
            if ( angle > 180.0f - degreePlayMoveLeftRight ) {
                // lowerBody.forward = -moveDir;
                animName = "moveBackward";
            }
            else if ( angle < degreePlayMoveLeftRight ) {
                // lowerBody.forward = moveDir;
                animName = "moveForward";
            }
            else {
                Vector3 up = Vector3.Cross(moveDir,aimDir);
                if ( up.y > 0.0f ) {
                    // lowerBody.forward = Quaternion.Euler(0,90,0) * moveDir;
                    animName = "moveLeft";
                }
                else {
                    // lowerBody.forward = Quaternion.Euler(0,-90,0) * moveDir;
                    animName = "moveRight";
                }
            }
            lowerBody.forward = aimDir;
            anim[animName].normalizedSpeed = StepSpeed * controller.velocity.magnitude;
            if ( anim.IsPlaying(animName) == false )
                anim.CrossFade(animName,0.3f);
        }
        else {
            lowerBody.forward = aimDir;
        }

        // NOTE: upper-body rotation must be calculate after lower-body.
        // process upper-body rotation
        upperBody.forward = aimDir;
    }
}
