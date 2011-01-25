// ======================================================================================
// File         : Player_boy.cs
// Author       : Wu Jie 
// Last Change  : 10/08/2010 | 23:24:12 PM | Friday,October
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;


///////////////////////////////////////////////////////////////////////////////
// class Player_boy
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

public class Player_boy : Player_base {

    ///////////////////////////////////////////////////////////////////////////////
    // actions, conditions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: Action_Move 
    // ------------------------------------------------------------------ 

    class Action_Move : FSM.Action {
        Player_boy playerBoy; 

        public Action_Move ( Player_boy _playerBoy ) {
            this.playerBoy = _playerBoy;
        }

        public override void exec () {
            this.playerBoy.Act_Movement();
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    class Action_FallDown : FSM.Action {
        Player_boy playerBoy; 

        public Action_FallDown ( Player_boy _playerBoy ) {
            this.playerBoy = _playerBoy;
        }

        public override void exec () {
            this.playerBoy.Act_FallDown();
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: Condition_isMoving 
    // ------------------------------------------------------------------ 

    class Condition_isMoving : FSM.Condition {
        Player_boy playerBoy = null;

        public Condition_isMoving ( Player_boy _playerBoy ) {
            this.playerBoy = _playerBoy;
        }

        public override bool exec () { 
            return this.playerBoy.IsMoving();
        } 
    }

    // ------------------------------------------------------------------ 
    // Desc: Condition_isMeleeButtonDown 
    // ------------------------------------------------------------------ 

    class Condition_isMeleeButtonDown : FSM.Condition {
        Player_boy playerBoy = null;

        public Condition_isMeleeButtonDown ( Player_boy _playerBoy ) {
            this.playerBoy = _playerBoy;
        }

        public override bool exec () {
            return this.playerBoy.MeleeButtonDown();
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: Condition_isAttacking 
    // ------------------------------------------------------------------ 

    class Condition_isAttacking : FSM.Condition {
        Player_boy playerBoy = null;

        public Condition_isAttacking ( Player_boy _playerBoy ) {
            this.playerBoy = _playerBoy;
        }

        public override bool exec () {
            AttackInfo atk_info = playerBoy.GetAttackInfo();
            if ( atk_info.curCombo == null )
                return false;
            return this.playerBoy.IsPlayingAnim( atk_info.curCombo.animName, atk_info.curCombo.endTime );
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: Action_StartCombo 
    // ------------------------------------------------------------------ 

    class Action_StartCombo : FSM.Action {
        Player_boy playerBoy = null;
        public Action_StartCombo ( Player_boy _playerBoy ) {
            this.playerBoy = _playerBoy;
        }

        public override void exec () {
            this.playerBoy.StartCombo();
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    class Action_NextCombo : FSM.Action {
        Player_boy playerBoy = null;
        public Action_NextCombo ( Player_boy _playerBoy ) {
            this.playerBoy = _playerBoy;
        }

        public override void exec () {
            this.playerBoy.NextCombo();
        }
    }

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public Vector3 screenDir = Vector3.zero;
    public Transform upperBody;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	protected new void Start () {
        base.Start();

        //
        this.InitAnim();
        this.InitFSM();
	}
	
    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	protected new void Update () {
        HandleInput();
        this.fsm.tick(); // update state machine
        ProcessMovement (); // handle steering
        // ShowDebugInfo(); // DEBUG
	}

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected new void ShowDebugInfo () {
        DebugHelper.ScreenPrint("== player boy debug info ==");
        base.ShowDebugInfo();
        DebugHelper.ScreenPrint("curHP = " + this.playerInfo.curHP);
        DebugHelper.ScreenPrint("maxHP = " + this.playerInfo.maxHP);
    }


    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void LateUpdate () {
        // reset the internal state.
        this.moveDir = Vector3.zero; 
        this.lastHit.stunType = HitInfo.StunType.none;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void InitAnim () {
        // setup the animation state
        AnimationState state;
        string[] anim_keys0 = { 
            "moveforward", 
            "idle",
            "downIdle"
        };
        foreach (string key in anim_keys0) {
            state = this.anim[key];
            state.layer = 0;
            state.wrapMode = WrapMode.Loop;
            state.weight = 1.0f;
            state.enabled = false;
        }

        //
        string[] anim_keys0_once = { 
            "fallDown",
            "getUp"
        };
        foreach (string key in anim_keys0_once) {
            state = this.anim[key];
            state.layer = 0;
            state.wrapMode = WrapMode.Once;
            state.weight = 1.0f;
            state.enabled = false;
        }

        //
        string[] melee_anims = { 
            "melee1_copy", 
            "melee2_copy", 
        };
        foreach (string key in melee_anims) {
            state = this.anim[key];
            state.layer = 1;
            state.wrapMode = WrapMode.Once;
            state.weight = 1.0f;
            state.enabled = false;
            state.AddMixingTransform(this.upperBody);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void InitFSM () {

        // ======================================================== 
        // setup states
        // ======================================================== 

        // idle
        FSM.State state_idle = new FSM.State( "Idle", 
                                              new Action_PlayAnim(this.anim,"idle"), 
                                              null, 
                                              null );
        // walk
        FSM.State state_walk = new FSM.State( "Walk", 
                                              new Action_PlayAnim(this.anim,"moveforward"), 
                                              null, // new Action_Move(this),
                                              null );
        // melee
        FSM.State state_idleMelee = new FSM.State( "IdleMelee", 
                                               new Action_PlayAnim(this.anim,"idle"), 
                                               new Action_NextCombo(this), 
                                               null );
        // walk melee
        FSM.State state_walkMelee = new FSM.State( "WalkMelee", 
                                               new Action_PlayAnim(this.anim,"moveforward"), 
                                               new Action_NextCombo(this), 
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
                                               new Action_PlayAnim(this.anim,"getUp"), 
                                               null,
                                               null );
        // get hit
        FSM.State state_onStun = new FSM.State( "OnStun", 
                                               new Action_ActOnStun(this), 
                                               null,
                                               null );

        // ======================================================== 
        // condition 
        // ======================================================== 

        FSM.Action act_startCombo = new Action_StartCombo(this); 
        FSM.Action act_Disable = new Action_DisableSteering(this);
        FSM.Action act_Enable = new Action_EnableSteering(this);

        FSM.Condition cond_isMoving = new Condition_isMoving(this);
        FSM.Condition cond_isMeleeButtonDown = new Condition_isMeleeButtonDown(this);
        FSM.Condition cond_isAttacking = new Condition_isAttacking(this);
        FSM.Condition cond_isOnStun = new Condition_isOnStun(this);
        FSM.Condition cond_noHP = new Condition_noHP(this.playerInfo);

        // ======================================================== 
        // setup transitions
        // ======================================================== 

        // idle to ...
        state_idle.AddTransition( new FSM.Transition( state_down, cond_noHP, act_Disable ) );
        state_idle.AddTransition( new FSM.Transition( state_onStun, cond_isOnStun, act_Disable ) );
        state_idle.AddTransition( new FSM.Transition( state_walk, cond_isMoving, null ) );
        state_idle.AddTransition( new FSM.Transition( state_idleMelee, cond_isMeleeButtonDown, act_startCombo ) );

        // walk to ...
        state_walk.AddTransition( new FSM.Transition( state_down, cond_noHP, act_Disable ) );
        state_walk.AddTransition( new FSM.Transition( state_onStun, cond_isOnStun, act_Disable ) );
        state_walk.AddTransition( new FSM.Transition( state_idle, new FSM.Condition_not(cond_isMoving), null ) );
        state_walk.AddTransition( new FSM.Transition( state_walkMelee, cond_isMeleeButtonDown, act_startCombo ) );

        // idle melee to ...
        state_idleMelee.AddTransition( new FSM.Transition( state_down, cond_noHP, act_Disable ) );
        state_idleMelee.AddTransition( new FSM.Transition( state_onStun, cond_isOnStun, act_Disable ) );
        state_idleMelee.AddTransition( new FSM.Transition( state_idle, 
                                                       new FSM.Condition_and(new FSM.Condition_not(cond_isMoving),
                                                                             new FSM.Condition_not(cond_isAttacking)), 
                                                       null ) );
        state_idleMelee.AddTransition( new FSM.Transition( state_walk, 
                                                       new FSM.Condition_and(cond_isMoving, 
                                                                             new FSM.Condition_not(cond_isAttacking) ),
                                                       null ) );
        state_idleMelee.AddTransition( new FSM.Transition( state_walkMelee, 
                                                       new FSM.Condition_and(cond_isMoving, cond_isAttacking ),
                                                       null ) );

        // walk melee to ..
        state_walkMelee.AddTransition( new FSM.Transition( state_down, cond_noHP, act_Disable ) );
        state_walkMelee.AddTransition( new FSM.Transition( state_onStun, cond_isOnStun, act_Disable ) );
        state_walkMelee.AddTransition( new FSM.Transition( state_idle, 
                                                       new FSM.Condition_and(new FSM.Condition_not(cond_isMoving),
                                                                             new FSM.Condition_not(cond_isAttacking)), 
                                                       null ) );
        state_walkMelee.AddTransition( new FSM.Transition( state_idleMelee, 
                                                       new FSM.Condition_and(new FSM.Condition_not(cond_isMoving),cond_isAttacking),
                                                       null ) );
        state_walkMelee.AddTransition( new FSM.Transition( state_walk, 
                                                       new FSM.Condition_and(cond_isMoving,
                                                                             new FSM.Condition_not(cond_isAttacking)), 
                                                       null ) );
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
                                                       act_Disable ) );
        state_getUp.AddTransition( new FSM.Transition( state_idle, 
                                                       new FSM.Condition_not( new Condition_isPlayingAnim( this, "getUp" ) ), 
                                                       act_Enable ) );
        // on hit to ...
        state_onStun.AddTransition( new FSM.Transition( state_down, cond_noHP, null ) );
        state_onStun.AddTransition( new FSM.Transition ( state_idle, 
                                                        new FSM.Condition_not(new Condition_isStunning(this) ),
                                                        act_Enable ) );
        state_onStun.AddTransition( new FSM.Transition ( state_onStun, 
                                                         cond_isOnStun,
                                                         null ) );

        // init fsm
        this.fsm.init(state_idle);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void HandleInput() {
        this.screenDir = screenPad ? screenPad.GetMoveDirection() : Vector2.zero;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void ProcessMovement () {
        if ( this.steeringState != SteeringState.disable ) {
            if ( IsMoving() == false )
                this.Stop();
            else
                this.Act_Movement();
        }

        // handle steering
        Vector3 force = Vector3.zero;
        if ( this.steeringState == SteeringState.moving ) {
            force = this.moveDir * base.maxForce;
            force.y = 0.0f;
        }
        else if ( this.steeringState == SteeringState.braking || 
                  this.steeringState == SteeringState.disable ) {
            ApplyBrakingForce();
        }
        ApplySteeringForce( force );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void StartCombo () {
        AttackInfo atk_info = this.GetAttackInfo();
        ComboInfo first_combo = atk_info.combo_entry;
        atk_info.curCombo = first_combo;
        this.anim[first_combo.animName].normalizedSpeed = atk_info.speed;
        this.anim.Rewind(first_combo.animName); // NOTE: without rewind, you can't play one animation continuesly
        this.anim.CrossFade(first_combo.animName);

        // adjust the orientation
        // AdjustOrientation();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void NextCombo () {
        AttackInfo atk_info = this.GetAttackInfo();
        if ( atk_info.curCombo == null )
            return;

        AnimationState curAnim = this.anim[atk_info.curCombo.animName];
        ComboInfo nextCombo = atk_info.curCombo.next;
        if ( nextCombo == null )
            return;

        // if we have input
        if ( screenPad.MeleeButtonDown() ) {
            // if we are in the valid input range
            if ( curAnim.time >= atk_info.curCombo.validInputTime.x 
                 && curAnim.time <= atk_info.curCombo.validInputTime.y )
            {
                atk_info.waitForNextCombo = true;
            }
        }
        else if ( atk_info.waitForNextCombo ) {
            if ( curAnim.time >= atk_info.curCombo.validInputTime.y ) {
                this.anim[nextCombo.animName].normalizedSpeed = atk_info.speed;
                this.anim.Rewind(nextCombo.animName); // NOTE: without rewind, you can't play one animation continuesly
                this.anim.CrossFade(nextCombo.animName);
                atk_info.curCombo = nextCombo;
                atk_info.waitForNextCombo = false;
                // adjust the orientation
                // AdjustOrientation();
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public bool IsMoving () { return this.screenDir.sqrMagnitude > 0.0f; }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public bool MeleeButtonDown () { return screenPad.MeleeButtonDown(); }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void StartMeleeAttack () {
        AttackInfo atk_info = this.GetAttackInfo();
        atk_info.curCombo.attack_shape.active = true;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void EndMeleeAttack () {
        AttackInfo atk_info = this.GetAttackInfo();
        atk_info.curCombo.attack_shape.active = false;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void Act_Movement () {
        Transform mainCamera = Camera.main.transform;
        Vector3 dir = mainCamera.TransformDirection(this.screenDir.normalized); 
        dir.y = 0.0f;
        dir.Normalize();
        this.Move(dir);

        // adjust move animation speed
        this.anim["moveforward"].normalizedSpeed = Mathf.Max(this.StepSpeed * this.CurSpeed(),1.0f);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void Act_FallDown () {
        this.isDown = true;
        this.anim.CrossFade("fallDown");
        StartCoroutine( WaitForRecover() );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void AdjustOrientation () {
        if ( this.screenDir == Vector3.zero )
            return;

        // adjust the orientation
        Transform mainCamera = Camera.main.transform;
        Vector3 dir = mainCamera.TransformDirection(this.screenDir.normalized); 
        dir.y = 0.0f;
        dir.Normalize();
        this.transform.forward = dir;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    new void OnTriggerEnter ( Collider _other ) {
        base.OnTriggerEnter(_other);
        screenPad.gameObject.SendMessage ( "OnBoyHit" );
    }
}
