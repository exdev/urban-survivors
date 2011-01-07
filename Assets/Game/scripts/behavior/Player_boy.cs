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
    // Desc: Condition_isMeleeButtonTriggered 
    // ------------------------------------------------------------------ 

    class Condition_isMeleeButtonTriggered : FSM.Condition {
        Player_boy playerBoy = null;

        public Condition_isMeleeButtonTriggered ( Player_boy _playerBoy ) {
            this.playerBoy = _playerBoy;
        }

        public override bool exec () {
            return this.playerBoy.MeleeButtonTriggered();
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
            return this.playerBoy.IsPlayingAnim( atk_info.curCombo.animName, 
                                                 atk_info.curCombo.endTime );
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
    protected bool meleeButtonTriggered = false;

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
        this.meleeButtonTriggered = false;
        this.moveDir = Vector3.zero; 
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
            "melee1_copy", 
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
                                              new Action_Move(this),
                                              new Action_StopMoving(this) );
        // melee
        FSM.State state_melee = new FSM.State( "Melee", 
                                               new Action_StartCombo(this), 
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

        // ======================================================== 
        // condition 
        // ======================================================== 

        FSM.Condition cond_isMoving = new Condition_isMoving(this);
        FSM.Condition cond_isMeleeButtonTriggered = new Condition_isMeleeButtonTriggered(this);
        FSM.Condition cond_isAttacking = new Condition_isAttacking(this);

        // ======================================================== 
        // setup transitions
        // ======================================================== 

        // idle to ...
        state_idle.AddTransition( new FSM.Transition( state_down, new Condition_noHP(this.playerInfo), null ) );
        state_idle.AddTransition( new FSM.Transition( state_walk, cond_isMoving, null ) );
        state_idle.AddTransition( new FSM.Transition( state_melee, cond_isMeleeButtonTriggered, null ) );

        // walk to ...
        state_walk.AddTransition( new FSM.Transition( state_down, new Condition_noHP(this.playerInfo), null ) );
        state_walk.AddTransition( new FSM.Transition( state_idle, new FSM.Condition_not(cond_isMoving), null ) );
        state_walk.AddTransition( new FSM.Transition( state_melee, cond_isMeleeButtonTriggered, null ) );

        // melee to ...
        state_melee.AddTransition( new FSM.Transition( state_down, new Condition_noHP(this.playerInfo), null ) );
        state_melee.AddTransition( new FSM.Transition( state_idle, 
                                                       new FSM.Condition_and(new FSM.Condition_not(cond_isMoving),
                                                                             new FSM.Condition_not(cond_isAttacking)), 
                                                       null ) );
        state_melee.AddTransition( new FSM.Transition( state_walk, 
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
                                                       new Condition_noHP( this.playerInfo ),
                                                       null ) );
        state_getUp.AddTransition( new FSM.Transition( state_idle, 
                                                       new FSM.Condition_not( new Condition_isPlayingAnim( this, "getUp" ) ), 
                                                       null ) );

        // init fsm
        this.fsm.init(state_idle);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void HandleInput() {
        this.screenDir = screenPad ? screenPad.GetMoveDirection() : Vector2.zero;
        if ( screenPad.MeleeButtonDown() ) {
            this.meleeButtonTriggered = true;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void ProcessMovement () {
        // stop moving when in melee attack state.
        // if ( this.fsm.CurrentState().name == "Melee" ) {
        //     ApplyBrakingForce(10.0f);
        //     ApplySteeringForce(Vector3.zero);
        // }
        // else {
        //     if ( MathHelper.IsZerof(this.moveDir.sqrMagnitude) ) {
        //         ApplyBrakingForce(10.0f);
        //     }
        //     ApplySteeringForce( this.moveDir * base.maxForce );
        // }

        // handle steering
        Vector3 force = Vector3.zero;
        if ( this.steeringState == SteeringState.moving ) {
            force = this.moveDir * base.maxForce;
            force.y = 0.0f;
            ApplySteeringForce( force );
        }
        else if ( this.steeringState == SteeringState.braking ) {
            ApplyBrakingForce(10.0f);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void StartCombo () {
        AttackInfo atk_info = this.GetAttackInfo();
        ComboInfo first_combo = atk_info.combo_entry;
        atk_info.curCombo = first_combo;
        this.anim[first_combo.animName].normalizedSpeed = atk_info.speed;
        this.anim.Rewind(first_combo.animName);
        this.anim.CrossFade(first_combo.animName);

        // adjust the orientation
        AdjustOrientation();
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
        if ( this.meleeButtonTriggered ) {
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
                this.anim.Rewind(atk_info.curCombo.animName);
                this.anim.CrossFade(nextCombo.animName);
                atk_info.curCombo = nextCombo;
                atk_info.waitForNextCombo = false;
                // adjust the orientation
                AdjustOrientation();
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

    public bool MeleeButtonTriggered () { return this.meleeButtonTriggered; }

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

    void OnTriggerEnter ( Collider _other ) {
        // don't do anything if player is down
        if ( this.isDown )
            return;

        //
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
        // //     this.lastHit.stunType = HitInfo.StunType.light;
        // // else if ( dmgOutput >= 20.0f )
        // //     this.lastHit.stunType = HitInfo.StunType.normal;
        // this.lastHit.stunType = HitInfo.StunType.normal;
        // // } TODO end 

        // this.lastHit.position = _other.transform.position;
        // this.lastHit.normal = _other.transform.right;
        // Vector3 dir = _other.transform.position - transform.position;
        // dir.y = 0.0f;
        // dir.Normalize();
        // this.lastHit.knockBackForce = dir * DamageRule.Instance().KnockBackForce(dmgInfo.knockBackType);  

        // // TODO: if hit light, face it { 
        // // transform.forward = -_other.transform.forward;
        // // } TODO end 
        // } TODO end 
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void Act_Movement () {
        Transform mainCamera = Camera.main.GetComponent( typeof(Transform) ) as Transform;
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
        Transform mainCamera = Camera.main.GetComponent( typeof(Transform) ) as Transform;
        Vector3 dir = mainCamera.TransformDirection(this.screenDir.normalized); 
        dir.y = 0.0f;
        dir.Normalize();
        this.transform.forward = dir;
    }
}
