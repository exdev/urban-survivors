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

    ///////////////////////////////////////////////////////////////////////////////
    // actions, conditions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: Action_FollowTarget 
    // ------------------------------------------------------------------ 

    class Action_FollowTarget : FSM.Action {
        Player_girl playerGirl; 

        public Action_FollowTarget ( Player_girl _playerGirl ) {
            this.playerGirl = _playerGirl;
        }

        public override void exec () {
            this.playerGirl.Act_Movement();
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: Action_Shoot 
    // ------------------------------------------------------------------ 

    class Action_Shoot : FSM.Action {
        Player_girl playerGirl; 

        public Action_Shoot ( Player_girl _playerGirl ) {
            this.playerGirl = _playerGirl;
        }

        public override void exec () {
            // TODO: this.playerGirl.Act_Movement();
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: Condition_TargetInRange 
    // ------------------------------------------------------------------ 

    class Condition_TargetInRange : FSM.Condition {
        Transform girl = null;
        Transform target = null;
        float dist = 1.0f;

        public Condition_TargetInRange ( Transform _girl, Transform _target, float _dist ) {
            this.girl = _girl;
            this.target = _target;
            this.dist = _dist;
        }

        public override bool exec () {
            return (this.girl.position - this.target.position).magnitude <= dist;
        }
    }

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

    // protected
    protected Vector3 moveDir = Vector3.zero;
    protected Vector3 aimDir = Vector3.forward;

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
        this.fsm.tick(); // update state machine

        // TODO: 
        ProcessMovement (); // handle steering
        // TODO { 
        if ( MathHelper.IsZerof(moveDir.sqrMagnitude) == false )
            Act_Movement();
        // } TODO end 

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
        DebugHelper.ScreenPrint ( "Player_girl current state: " + this.fsm.CurrentState().name );
        // } DEBUG end 
	}

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void LateUpdate () {
        // NOTE: upper-body rotation must be calculate after lower-body.
        this.lowerBody.forward = this.aimDir;
        this.upperBody.forward = this.aimDir;

        // reset the internal state.
        this.moveDir = Vector3.zero; 
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    private void InitAnim () {
        // get the animation component
        this.anim = gameObject.GetComponent(typeof(Animation)) as Animation;

        // animation
        AnimationState state;
        string[] anim_keys = { 
            "idle",
            "moveForward", 
            "moveBackward", 
            "moveRight", 
            "moveLeft" 
        };
        foreach (string key in anim_keys) {
            state = this.anim[key];
            state.layer = 0;
            state.wrapMode = WrapMode.Loop;
            state.weight = 1.0f;
            state.enabled = true;
        }

        // DISABLE { 
        // state = this.anim["turnRight"];
        // state.wrapMode = WrapMode.Once;
        // state.layer = 0;
        // state.weight = 1.0f;

        // state = this.anim["turnLeft"];
        // state.wrapMode = WrapMode.Once;
        // state.layer = 0;
        // state.weight = 1.0f;
        // } DISABLE end 

        state = this.anim["shootSMG"];
        state.wrapMode = WrapMode.Once;
        state.layer = 1;
        state.weight = 1.0f;

        this.anim.SyncLayer(0);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void InitFSM () {

        // ======================================================== 
        // setup states
        // ======================================================== 

        // idle
        FSM.State state_idle = new FSM.State( "idle", 
                                              new Action_PlayAnim(this.anim,"idle"), 
                                              null, 
                                              null );
        // following
        FSM.State state_following = new FSM.State( "following", 
                                                   new Action_FollowTarget(this), 
                                                   new Action_FollowTarget(this), 
                                                   null );
        // shooting
        FSM.State state_shooting = new FSM.State( "shooting", 
                                                  new Action_Shoot(this), 
                                                  null,
                                                  null );

        // ======================================================== 
        // condition 
        // ======================================================== 

        FSM.Condition cond_isNearTarget = new Condition_TargetInRange( this.transform,
                                                                       this.followTarget.transform,
                                                                       this.followDistance );
        FSM.Condition cond_isFarAwayTarget = new FSM.Condition_not(new Condition_TargetInRange( this.transform,
                                                                                                this.followTarget.transform,
                                                                                                this.followDistance ));
        // FSM.Condition cond_isMeleeButtonTriggered = new Condition_isMeleeButtonTriggered(this);
        // FSM.Condition cond_isAttacking = new Condition_isAttacking(this);

        // ======================================================== 
        // setup transitions
        // ======================================================== 

        // idle to ...
        // state_idle.AddTransition( new FSM.Transition( state_following, cond_isFarAwayTarget, null ) );
        // state_idle.AddTransition( new FSM.Transition( state_melee, cond_isMeleeButtonTriggered, null ) );

        // following to ...
        // state_following.AddTransition( new FSM.Transition( state_idle, cond_isNearTarget, null ) );
        // state_walk.AddTransition( new FSM.Transition( state_melee, cond_isMeleeButtonTriggered, null ) );

        // shooting to ...
        // state_shooting.AddTransition( new FSM.Transition( state_idle, 
        //                                                   new FSM.Condition_and(new FSM.Condition_not(cond_isMoving),
        //                                                                         new FSM.Condition_not(cond_isAttacking)), 
        //                                                   null ) );
        // state_melee.AddTransition( new FSM.Transition( state_walk, 
        //                                                new FSM.Condition_and(cond_isMoving,
        //                                                                      new FSM.Condition_not(cond_isAttacking)), 
        //                                                null ) );

        // init fsm
        this.fsm.init(state_idle);
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
        this.aimDir = Vector3.zero; 
        this.aimDir.x = aimDir2D.x; 
        this.aimDir.y = aimDir2D.y; 
        this.aimDir = Camera.main.transform.TransformDirection(this.aimDir);
        this.aimDir.y = 0.0f;
        this.aimDir = this.aimDir.normalized;

        // if we have weapon in hand.
        if ( screenPad.CanFire() ) {
            if ( this.curWeapon ) {
                Fire fire = this.curWeapon.GetComponent<Fire>();
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

    public void Act_Movement () {
        // process lower-body rotation
        float angle = Vector3.Angle ( this.moveDir, this.aimDir );
        // DebugHelper.ScreenPrint("angle = " + angle); // DEBUG
        string animName = "";
        if ( angle > 180.0f - degreePlayMoveLeftRight ) {
            animName = "moveBackward";
        } else if ( angle < degreePlayMoveLeftRight ) {
            animName = "moveForward";
        } else {
            Vector3 up = Vector3.Cross(moveDir,aimDir);
            if ( up.y > 0.0f )
                animName = "moveLeft";
            else
                animName = "moveRight";
        }
        this.anim[animName].normalizedSpeed = this.StepSpeed * this.controller.velocity.magnitude;
        if ( this.anim.IsPlaying(animName) == false )
            this.anim.CrossFade(animName,0.3f);
    }
}
