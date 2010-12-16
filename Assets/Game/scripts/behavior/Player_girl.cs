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
        Transform target;

        public Action_FollowTarget ( Player_girl _playerGirl, Transform _target ) {
            this.playerGirl = _playerGirl;
            this.target = _target;
        }

        public override void exec () {
            this.playerGirl.Seek(this.target.position);
            this.playerGirl.Act_Movement();
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: Action_Shooting 
    // ------------------------------------------------------------------ 

    class Action_Shooting : FSM.Action {
        Player_girl playerGirl; 

        public Action_Shooting ( Player_girl _playerGirl ) {
            this.playerGirl = _playerGirl;
        }

        public override void exec () {
            this.playerGirl.Act_Shooting();
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: Action_Reloading 
    // ------------------------------------------------------------------ 

    class Action_Reloading : FSM.Action {
        Player_girl playerGirl; 

        public Action_Reloading ( Player_girl _playerGirl ) {
            this.playerGirl = _playerGirl;
        }

        public override void exec () {
            this.playerGirl.Act_Reloading();
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

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    class Condition_isShootButtonTriggered : FSM.Condition {
        Player_girl playerGirl = null;

        public Condition_isShootButtonTriggered ( Player_girl _playerGirl ) {
            this.playerGirl = _playerGirl;
        } 

        public override bool exec () {
            return this.playerGirl.ShootButtonTriggered();
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: Condition_isShooting 
    // ------------------------------------------------------------------ 

    class Condition_isShooting : FSM.Condition {
        Player_girl playerGirl = null;

        public Condition_isShooting ( Player_girl _playerGirl ) {
            this.playerGirl = _playerGirl;
        }

        public override bool exec () {
            ShootInfo shootInfo = this.playerGirl.GetShootInfo();
            return this.playerGirl.IsPlayingAnim( shootInfo.shootAnim );
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: Condition_isReloading 
    // ------------------------------------------------------------------ 

    class Condition_isReloading : FSM.Condition {
        Player_girl playerGirl = null;

        public Condition_isReloading ( Player_girl _playerGirl ) {
            this.playerGirl = _playerGirl;
        }

        public override bool exec () {
            ShootInfo shootInfo = this.playerGirl.GetShootInfo();
            return this.playerGirl.IsPlayingAnim( shootInfo.reloadAnim );
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    class Condition_isOutOfAmmo : FSM.Condition {
        Player_girl playerGirl = null;

        public Condition_isOutOfAmmo ( Player_girl _playerGirl ) {
            this.playerGirl = _playerGirl;
        }

        public override bool exec () {
            ShootInfo shootInfo = this.playerGirl.GetShootInfo();
            return shootInfo.OutOfAmmo();
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    class Condition_isMoving : FSM.Condition {
        Player_girl playerGirl = null;

        public Condition_isMoving ( Player_girl _playerGirl ) {
            this.playerGirl = _playerGirl;
        }

        public override bool exec () {
            return this.playerGirl.IsMoving();
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
    protected Vector3 aimDir = Vector3.forward;
    protected bool shootButtonTriggered = false;

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
        ProcessMovement (); // handle steering

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
        // DebugHelper.ScreenPrint ( "Player_girl current HP: " + this.playerInfo.curHP );
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
        this.shootButtonTriggered = false;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    private void InitAnim () {
        // animation
        AnimationState state;
        string[] anim_keys0 = { 
            "idle",
            "moveForward", 
            "moveBackward", 
            "moveRight", 
            "moveLeft" 
        };
        foreach (string key in anim_keys0) {
            state = this.anim[key];
            state.layer = 0;
            state.wrapMode = WrapMode.Loop;
            state.weight = 1.0f;
            state.enabled = false;
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

        string[] anim_keys1 = { 
            "shootSMG",
            "reload_smg"
        };
        foreach (string key in anim_keys1) {
            state = this.anim[key];
            state.wrapMode = WrapMode.Once;
            state.layer = 1;
            state.weight = 1.0f;
            state.AddMixingTransform(this.upperBody);
            state.enabled = false;
        }
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
                                                   new Action_FollowTarget(this,this.followTarget.transform), 
                                                   new Action_FollowTarget(this,this.followTarget.transform), 
                                                   new Action_StopMoving(this) );
        // idle shooting
        FSM.State state_idleShooting = new FSM.State( "idle_shooting", 
                                                      new Action_Shooting(this), 
                                                      null,
                                                      null );
        // walk shooting
        FSM.Action action_walkAndShoot = new FSM.Action_list( new FSM.Action[] {
                                                              new Action_FollowTarget(this,this.followTarget.transform),
                                                              new Action_Shooting(this) 
                                                              }
                                                            );
        FSM.State state_walkShooting = new FSM.State( "walk_shooting", 
                                                      action_walkAndShoot, 
                                                      new Action_FollowTarget(this,this.followTarget.transform), 
                                                      new Action_StopMoving(this) );
        // reloading
        FSM.State state_idleReloading = new FSM.State( "idle_reloading", 
                                                       new Action_Reloading(this), 
                                                       null,
                                                       null );
        // walk reloading
        FSM.Action action_walkAndReload = new FSM.Action_list( new FSM.Action[] {
                                                               new Action_FollowTarget(this,this.followTarget.transform),
                                                               new Action_Reloading(this) 
                                                               }
                                                             );
        FSM.State state_walkReloading = new FSM.State( "walk_reloading", 
                                                       action_walkAndReload, 
                                                       new Action_FollowTarget(this,this.followTarget.transform), 
                                                       new Action_StopMoving(this) );

        // ======================================================== 
        // condition 
        // ======================================================== 

        FSM.Condition cond_isNearTarget = new Condition_TargetInRange( this.transform,
                                                                       this.followTarget.transform,
                                                                       this.followDistance );
        FSM.Condition cond_isFarAwayTarget = new FSM.Condition_not(new Condition_TargetInRange( this.transform,
                                                                                                this.followTarget.transform,
                                                                                                this.followDistance * 1.2f ));
        FSM.Condition cond_isShootButtonTriggered = new Condition_isShootButtonTriggered(this);
        FSM.Condition cond_isNotShooting = new FSM.Condition_not( new Condition_isShooting(this) );
        FSM.Condition cond_isNotReloading = new FSM.Condition_not( new Condition_isReloading(this) );
        FSM.Condition cond_isOutOfAmmo = new Condition_isOutOfAmmo(this);
        // DELME { 
        // FSM.Condition cond_isMoving = new Condition_isMoving(this);
        // FSM.Condition cond_isShooting = new Condition_isShooting(this);
        // } DELME end 

        // ======================================================== 
        // setup transitions
        // ======================================================== 

        // idle to ...
        state_idle.AddTransition( new FSM.Transition( state_following, cond_isFarAwayTarget, null ) );
        state_idle.AddTransition( new FSM.Transition( state_idleShooting, cond_isShootButtonTriggered, null ) );

        // following to ...
        state_following.AddTransition( new FSM.Transition( state_idle, cond_isNearTarget, null ) );
        state_following.AddTransition( new FSM.Transition( state_walkShooting, cond_isShootButtonTriggered, null ) );

        // idle shooting to ...
        state_idleShooting.AddTransition( new FSM.Transition( state_idleReloading, cond_isOutOfAmmo, null ) );
        state_idleShooting.AddTransition( new FSM.Transition( state_idle, cond_isNotShooting, null ) );
        state_idleShooting.AddTransition( new FSM.Transition( state_walkShooting, cond_isFarAwayTarget, null ) );

        // walk shooting to ...
        state_walkShooting.AddTransition( new FSM.Transition( state_walkReloading, cond_isOutOfAmmo, null ) );
        state_walkShooting.AddTransition( new FSM.Transition( state_following, cond_isNotShooting, null ) );
        state_walkShooting.AddTransition( new FSM.Transition( state_idleShooting, cond_isNearTarget, null ) );

        // idle reload to ...
        state_idleReloading.AddTransition( new FSM.Transition( state_idle, cond_isNotReloading, null ) );
        state_idleReloading.AddTransition( new FSM.Transition( state_walkReloading, cond_isFarAwayTarget, null ) );

        // walk reload to ...
        state_walkReloading.AddTransition( new FSM.Transition( state_following, cond_isNotReloading, null ) );
        state_walkReloading.AddTransition( new FSM.Transition( state_idleReloading, cond_isNearTarget, null ) );

        // init fsm
        this.fsm.init(state_idle);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    private void HandleInput() {
        // DISABLE { 
        // // get move direction
        // if ( this.followTarget == null ) {
        //     Vector2 screen_dir = screenPad.GetMoveDirection();
        //     if ( screen_dir.sqrMagnitude >= 0.0f ) {
        //         this.moveDir.x = screen_dir.x;
        //         this.moveDir.y = screen_dir.y;
        //         Transform mainCamera = Camera.main.transform;
        //         this.moveDir = mainCamera.TransformDirection(this.moveDir.normalized); 
        //         this.moveDir.y = 0.0f;
        //         this.moveDir = this.moveDir.normalized;
        //     }
        // }
        // } DISABLE end 

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
            this.shootButtonTriggered = true;
            // TODO { 
            // if ( this.curWeapon ) {
            //     Fire fire = this.curWeapon.GetComponent<Fire>();
            //     if (fire) {
            //         fire.Trigger();
            //     }
            // }
            // } TODO end 
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void ProcessMovement () {
        // handle steering
        Vector3 force = Vector3.zero;
        if ( this.steeringState == SteeringState.seeking ) {
            force = GetSteering_Seek_MaxForces ( this.targetPos );
            force.y = 0.0f;
        }
        else if ( this.steeringState == SteeringState.braking ) {
            ApplyBrakingForce(10.0f);
        }
        ApplySteeringForce( force );

        // DEBUG { 
        DebugHelper.DrawCircleY( this.followTarget.transform.position, this.followDistance, Color.yellow );
        // } DEBUG end 
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void Act_Movement () {
        Vector3 dir = this.controller.velocity.normalized;

        // process lower-body rotation
        float angle = Vector3.Angle ( dir, this.aimDir );
        // DebugHelper.ScreenPrint("angle = " + angle); // DEBUG
        string animName = "";
        if ( angle > 180.0f - degreePlayMoveLeftRight ) {
            animName = "moveBackward";
        } else if ( angle < degreePlayMoveLeftRight ) {
            animName = "moveForward";
        } else {
            Vector3 up = Vector3.Cross(dir,aimDir);
            if ( up.y > 0.0f )
                animName = "moveLeft";
            else
                animName = "moveRight";
        }
        this.anim[animName].normalizedSpeed = this.StepSpeed * this.controller.velocity.magnitude;
        if ( this.anim.IsPlaying(animName) == false )
            this.anim.CrossFade(animName,0.3f);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void Act_Shooting () {
        ShootInfo shootInfo = this.GetShootInfo();
        if ( shootInfo ) {
            if ( this.anim.IsPlaying(shootInfo.shootAnim) == false ) {
                shootInfo.AdjustAnim(this.anim);
                shootInfo.Fire();
                this.anim.Play(shootInfo.shootAnim);
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void Act_Reloading () {
        ShootInfo shootInfo = this.GetShootInfo();
        if ( shootInfo ) {
            shootInfo.AdjustAnim(this.anim);
            this.anim.Play(shootInfo.reloadAnim);
            // TODO: HACK
            shootInfo.Reload(shootInfo.capacity);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public bool IsMoving () { return this.controller.velocity.sqrMagnitude > 0.0f; }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public bool ShootButtonTriggered () { return this.shootButtonTriggered; }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnTriggerEnter ( Collider _other ) {
        DamageInfo dmgInfo = null;
        if ( _other.gameObject.layer == Layer.melee_enemy ) {
            dmgInfo = _other.GetComponent<DamageInfo>();

            if ( fxHitBite != null ) {
                fxHitBite.transform.position = _other.transform.position;
                fxHitBite.transform.rotation = _other.transform.rotation;
                fxHitBite.particleEmitter.Emit();
            }
        }
        else {
            return;
        }

        // if we don't get damage info, just return
        DebugHelper.Assert( dmgInfo, "can't find damage info for given layer" );
        if ( dmgInfo == null ) {
            return;
        }

        /*float dmgOutput =*/ DamageRule.Instance().CalculateDamage( this.playerInfo, dmgInfo );

        // TODO { 
        // // TODO { 
        // // if ( dmgOutput < 20.0f )
        // //     this.lastHit.hitType = HitInfo.HitType.light;
        // // else if ( dmgOutput >= 20.0f )
        // //     this.lastHit.hitType = HitInfo.HitType.normal;
        // this.lastHit.hitType = HitInfo.HitType.normal;
        // // } TODO end 

        // this.lastHit.position = _other.transform.position;
        // this.lastHit.normal = _other.transform.right;
        // Vector3 dir = _other.transform.position - transform.position;
        // dir.y = 0.0f;
        // dir.Normalize();
        // this.lastHit.hitBackForce = dir * DamageRule.Instance().HitBackForce(dmgInfo.hitBackType);  

        // // TODO: if hit light, face it { 
        // // transform.forward = -_other.transform.forward;
        // // } TODO end 
        // } TODO end 
    }
}
