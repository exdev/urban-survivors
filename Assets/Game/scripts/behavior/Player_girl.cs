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
using System.Collections.Generic;

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
    // Desc: Action_Move 
    // ------------------------------------------------------------------ 

    class Action_Move : FSM.Action {
        Player_girl playerGirl; 

        public Action_Move ( Player_girl _playerGirl ) {
            this.playerGirl = _playerGirl;
        }

        public override void exec () {
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
    // Desc: Action_FallDown 
    // ------------------------------------------------------------------ 

    class Action_FallDown : FSM.Action {
        Player_girl playerGirl; 

        public Action_FallDown ( Player_girl _playerGirl ) {
            this.playerGirl = _playerGirl;
        }

        public override void exec () {
            this.playerGirl.Act_FallDown();
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

    class Condition_isAmmoFull : FSM.Condition {
        Player_girl playerGirl = null;

        public Condition_isAmmoFull ( Player_girl _playerGirl ) {
            this.playerGirl = _playerGirl;
        }

        public override bool exec () {
            ShootInfo shootInfo = this.playerGirl.GetShootInfo();
            return shootInfo.isAmmoFull();
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

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    class Condition_isReloadButtonDown : FSM.Condition {
        Player_girl playerGirl = null;

        public Condition_isReloadButtonDown ( Player_girl _playerGirl ) {
            this.playerGirl = _playerGirl;
        }

        public override bool exec () {
            return this.playerGirl.ReloadButtonDown();
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

	protected new void Update () {
        HandleInput ();
        this.fsm.tick(); // update state machine
        ProcessMovement (); // handle steering
        // ShowDebugInfo(); // DEBUG
	}

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected new void ShowDebugInfo () {
        DebugHelper.ScreenPrint("== player girl debug info ==");
        base.ShowDebugInfo();
        DebugHelper.ScreenPrint("curHP = " + this.playerInfo.curHP);
        DebugHelper.ScreenPrint("maxHP = " + this.playerInfo.maxHP);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void LateUpdate () {
        if ( this.isDown == false ) {
            // NOTE: upper-body rotation must be calculate after lower-body.
            this.lowerBody.forward = this.aimDir;
            this.upperBody.forward = this.aimDir;
        }

        // reset the internal state.
        this.shootButtonTriggered = false;
        this.lastHit.stunType = HitInfo.StunType.none;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    private void InitAnim () {
        // animation
        AnimationState state;
        string[] anim_keys0 = { 
            "idle",
            "downIdle",
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

        string[] anim_keys0_once = { 
            "fallDown",
            // "getUp", 
            // "turnRight", 
            // "turnLeft", 
        };
        foreach (string key in anim_keys0_once) {
            state = this.anim[key];
            state.layer = 0;
            state.wrapMode = WrapMode.Once;
            state.weight = 1.0f;
            state.enabled = false;
        }

        string[] anim_keys1 = { 
            "shootSMG",
            "reload_smg",
            "hit1",
            "hit2"
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
        // walk
        FSM.State state_walk = new FSM.State( "walk", 
                                              null,
                                              new Action_Move(this), 
                                              null );
        // idle shooting
        FSM.State state_idleShooting = new FSM.State( "idle_shooting", 
                                                      new Action_Shooting(this), 
                                                      null,
                                                      null );
        // walk shooting
        FSM.State state_walkShooting = new FSM.State( "walk_shooting", 
                                                      new Action_Shooting(this), 
                                                      new Action_Move(this), 
                                                      null );
        // reloading
        FSM.State state_idleReloading = new FSM.State( "idle_reloading", 
                                                       new Action_Reloading(this), 
                                                       null,
                                                       null );
        // walk reloading
        FSM.State state_walkReloading = new FSM.State( "walk_reloading", 
                                                       new Action_Reloading(this), 
                                                       new Action_Move(this), 
                                                       null );
        // down
        FSM.State state_down = new FSM.State( "Down", 
                                               new Action_FallDown(this), 
                                               null,
                                               null );
        // downIdle
        FSM.State state_downIdle = new FSM.State( "DownIdle", 
                                                  new Action_PlayAnim(this.anim,"downIdle"), 
                                                  null,
                                                  null );
        // getUp
        FSM.State state_getUp = new FSM.State( "GetUp", 
                                               null, // TODO: new Action_PlayAnim(this.anim,"getUp") 
                                               null,
                                               null );
        // get stun
        FSM.State state_onStun = new FSM.State( "OnStun", 
                                               new Action_ActOnStun(this), 
                                               null,
                                               null );

        // ======================================================== 
        // condition 
        // ======================================================== 

        FSM.Condition cond_isShootButtonTriggered = new Condition_isShootButtonTriggered(this);
        FSM.Condition cond_isNotShooting = new FSM.Condition_not( new Condition_isShooting(this) );
        FSM.Condition cond_isNotReloading = new FSM.Condition_not( new Condition_isReloading(this) );
        FSM.Condition cond_isOutOfAmmo = new Condition_isOutOfAmmo(this);
        FSM.Condition cond_canReload = new FSM.Condition_and( new FSM.Condition_not(new Condition_isAmmoFull(this)), 
                                                              new Condition_isReloadButtonDown(this) );
        FSM.Condition cond_isOnStun = new Condition_isOnStun(this);
        FSM.Condition cond_noHP = new Condition_noHP(this.playerInfo);

        FSM.Condition cond_isMoving = new Condition_isMoving(this);
        FSM.Condition cond_isNotMoving = new FSM.Condition_not(cond_isMoving);

        // DELME { 
        // FSM.Condition cond_isShooting = new Condition_isShooting(this);
        // } DELME end 

        // ======================================================== 
        // setup transitions
        // ======================================================== 

         
                                        
        FSM.Action action_Move = new Action_Move(this);
        FSM.Action action_Enable = new Action_EnableSteering(this);
        FSM.Action action_Disable = new Action_DisableSteering(this);
        FSM.Action action_Idle = new Action_PlayAnim(this.anim,"idle");

        // idle to ...
        state_idle.AddTransition( new FSM.Transition( state_down, cond_noHP, action_Disable ) );
        state_idle.AddTransition( new FSM.Transition( state_onStun, cond_isOnStun, null ) );
        state_idle.AddTransition( new FSM.Transition( state_walk, cond_isMoving, action_Move ) );
        state_idle.AddTransition( new FSM.Transition( state_idleShooting, cond_isShootButtonTriggered, null ) );
        state_idle.AddTransition( new FSM.Transition( state_idleReloading, cond_canReload, null ) );

        // walk to ...
        state_walk.AddTransition( new FSM.Transition( state_down, cond_noHP, action_Disable ) );
        state_walk.AddTransition( new FSM.Transition( state_onStun, cond_isOnStun, null ) );
        state_walk.AddTransition( new FSM.Transition( state_idle, cond_isNotMoving, action_Idle ) );
        state_walk.AddTransition( new FSM.Transition( state_walkShooting, cond_isShootButtonTriggered, null ) );
        state_walk.AddTransition( new FSM.Transition( state_walkReloading, cond_canReload, null ) );

        // idle shooting to ...
        state_idleShooting.AddTransition( new FSM.Transition( state_down, cond_noHP, action_Disable ) );
        state_idleShooting.AddTransition( new FSM.Transition( state_onStun, cond_isOnStun, null ) );
        state_idleShooting.AddTransition( new FSM.Transition( state_idleReloading, new FSM.Condition_or ( cond_isOutOfAmmo, cond_canReload ), null ) );
        state_idleShooting.AddTransition( new FSM.Transition( state_idle, cond_isNotShooting, null ) );
        state_idleShooting.AddTransition( new FSM.Transition( state_walkShooting, cond_isMoving, action_Move ) );

        // walk shooting to ...
        state_walkShooting.AddTransition( new FSM.Transition( state_down, cond_noHP, action_Disable ) );
        state_walkShooting.AddTransition( new FSM.Transition( state_onStun, cond_isOnStun, null ) );
        state_walkShooting.AddTransition( new FSM.Transition( state_walkReloading, new FSM.Condition_or ( cond_isOutOfAmmo, cond_canReload ), null ) );
        state_walkShooting.AddTransition( new FSM.Transition( state_walk, cond_isNotShooting, null ) );
        state_walkShooting.AddTransition( new FSM.Transition( state_idleShooting, cond_isNotMoving, action_Idle ) );

        // idle reload to ...
        state_idleReloading.AddTransition( new FSM.Transition( state_down, cond_noHP, action_Disable ) );
        state_idleReloading.AddTransition( new FSM.Transition( state_onStun, cond_isOnStun, null ) );
        state_idleReloading.AddTransition( new FSM.Transition( state_idle, cond_isNotReloading, null ) );
        state_idleReloading.AddTransition( new FSM.Transition( state_walkReloading, cond_isMoving, action_Move ) );

        // walk reload to ...
        state_walkReloading.AddTransition( new FSM.Transition( state_down, cond_noHP, action_Disable ) );
        state_walkReloading.AddTransition( new FSM.Transition( state_onStun, cond_isOnStun, null ) );
        state_walkReloading.AddTransition( new FSM.Transition( state_walk, cond_isNotReloading, null ) );
        state_walkReloading.AddTransition( new FSM.Transition( state_idleReloading, cond_isNotMoving, action_Idle ) );

        // down to ...
        state_down.AddTransition( new FSM.Transition( state_downIdle, 
                                                      new FSM.Condition_not( new Condition_isPlayingAnim( this, "fallDown" ) ), 
                                                      null ) );
        // downIdle to ...
        state_downIdle.AddTransition( new FSM.Transition( state_getUp, 
                                                          new FSM.Condition_not( new Condition_isDown(this) ), 
                                                          null ) );
        // getUp to ...
        state_getUp.AddTransition( new FSM.Transition( state_down, 
                                                       cond_noHP,
                                                       action_Disable ) );
        state_getUp.AddTransition( new FSM.Transition( state_idle, 
                                                       new FSM.Condition_not( new Condition_isPlayingAnim( this, "getUp" ) ), 
                                                       action_Enable ) );
        // on hit to ...
        state_onStun.AddTransition( new FSM.Transition( state_down, cond_noHP, action_Disable ) );
        state_onStun.AddTransition( new FSM.Transition ( state_idle, 
                                                         new FSM.Condition_not(new Condition_isStunning(this) ),
                                                         null ) );
        state_onStun.AddTransition( new FSM.Transition ( state_onStun, 
                                                         cond_isOnStun,
                                                         null ) );

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
        if ( screenPad.CanShoot() ) {
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
        Vector3 force = Vector3.zero;
        if ( this.steeringState != SteeringState.disable ) {

            CharacterController fo = this.followTarget.collider as CharacterController;
            Vector3 dir = (this.transform.position - fo.transform.position).normalized;
            bool avoiding = false;

            // process avoidance.
            if ( Vector3.Dot ( fo.transform.forward, dir ) > -0.707f ) {

                Vector3 newPos = this.transform.position + this.controller.velocity * Time.deltaTime;
                Vector3 newFoPos = fo.transform.position + fo.velocity * Time.deltaTime;
                float next_distance = (newPos - newFoPos).magnitude;
                float minCollideDistance = fo.radius + this.controller.radius;

                if ( next_distance <= minCollideDistance ) {
                    force = GetSteering_Flee_MaxForces( this.followTarget.transform.position ); 
                    if ( Vector3.Angle(force,fo.velocity) < 10.0f ) {
                    }
                    this.Avoid();
                    avoiding = true;
                }
            }

            // process following
            if ( avoiding == false ) {
                if ( Vector3.Dot ( fo.transform.forward, dir ) > -0.707f ) {
                    this.Seek(this.followTarget.transform.position + this.followTarget.transform.forward * -this.followDistance );
                }
                else {
                    float cur_distance = (this.transform.position - this.followTarget.transform.position).magnitude;
                    // if we are far away target
                    if ( cur_distance >= this.followDistance * 1.2f )
                        this.Seek(this.followTarget.transform.position);
                    else if ( cur_distance <= this.followDistance )
                        this.Stop();
                }
            }
        }

        // handle steering
        if ( this.steeringState == SteeringState.seeking ) {
            force = GetSteering_Seek_MaxForces ( this.targetPos );
            force.y = 0.0f;
            Act_Movement(); // HACK
        }
        // HACK: should use behavior-tree fix this { 
        else if ( this.steeringState == SteeringState.braking || 
                  this.steeringState == SteeringState.disable ) {
        // } HACK end 
            ApplyBrakingForce();
        }
        ApplySteeringForce( force );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public bool IsMoving () { return this.steeringState != SteeringState.braking; }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public bool ReloadButtonDown () { return screenPad.ReloadButtonDown(); }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public bool ShootButtonTriggered () { return this.shootButtonTriggered; }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public Vector2 GetAutoLockDir ( Vector2 _dir ) {
        // TODO { 
        // Vector2 my_pos = new Vector2 ( this.transform.position.x, 
        //                                this.transform.position.z );
        // List<GameObject> enemies = GameRules.Instance().GetEnemies();
        // foreach ( GameObject enemy in enemies ) {
        //     CharacterController ctrl = enemy.collider as CharacterController;
        //     if ( ctrl ) {
        //         float radius = ctrl.radius; 

        //         Vector2 min = new Vector2 ( enemy.collider.bounds.min.x,
        //                                     enemy.collider.bounds.min.z );
        //         Vector2 max = new Vector2 ( enemy.collider.bounds.max.x,
        //                                     enemy.collider.bounds.max.z );
        //         Vector2 dmin = min - my_pos; 
        //         Vector2 dmax = max - my_pos; 
        //         float deg1 = Vector2.Angle(_dir,dmin);
        //         float deg2 = Vector2.Angle(_dir,dmax);

        //         // DebugHelper.ScreenPrint("deg1: " + deg1);
        //         // DebugHelper.ScreenPrint("deg2: " + deg2);
        //     }
        // }
        // } TODO end 
        // TODO:
        return _dir;
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
            shootInfo.Reload();
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void Act_FallDown () {
        this.transform.forward = this.upperBody.forward;
        this.isDown = true;
        // this.anim.CrossFade("fallDown", 0.3f, PlayMode.StopAll);
        this.anim.Play("fallDown", PlayMode.StopAll);
        StartCoroutine( WaitForRecover() );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    new void OnTriggerEnter ( Collider _other ) {
        base.OnTriggerEnter(_other);
        screenPad.gameObject.SendMessage ( "OnGirlHit" );
    }
}
