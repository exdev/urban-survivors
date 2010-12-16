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
    // Desc: Action_MoveToNearestPlayer 
    // ------------------------------------------------------------------ 

    class Action_AdjustMoveSpeed : FSM.Action {
        Animation anim_comp;
        Player_boy playerBoy = null;

        public Action_AdjustMoveSpeed ( Animation _anim, Player_boy _playerBoy ) {
            this.anim_comp = _anim;
            this.playerBoy = _playerBoy;
        }

        public override void exec () {
            this.anim_comp["moveforward"].normalizedSpeed 
                = Mathf.Max(this.playerBoy.StepSpeed * this.playerBoy.CurSpeed(),1.0f);
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

    protected Vector3 moveDir = Vector3.zero;
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

	void Update () {
        HandleInput();
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
        DebugHelper.ScreenPrint ( "Player_boy current state: " + this.fsm.CurrentState().name );
        // DebugHelper.ScreenPrint ( "Player_boy current HP: " + this.playerInfo.curHP );
        // } DEBUG end 
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
            "idle"
        };
        foreach (string key in anim_keys0) {
            state = this.anim[key];
            state.layer = 0;
            state.wrapMode = WrapMode.Loop;
            state.weight = 1.0f;
            state.enabled = false;
        }

        state = this.anim["melee1_copy"];
        state.layer = 0;
        state.wrapMode = WrapMode.Once;
        state.weight = 1.0f;
        state.enabled = false;
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
                                              new Action_AdjustMoveSpeed(this.anim,this),
                                              null );
        // melee
        FSM.State state_melee = new FSM.State( "Melee", 
                                               new Action_StartCombo(this), 
                                               new Action_NextCombo(this), 
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
        state_idle.AddTransition( new FSM.Transition( state_walk, cond_isMoving, null ) );
        state_idle.AddTransition( new FSM.Transition( state_melee, cond_isMeleeButtonTriggered, null ) );

        // walk to ...
        state_walk.AddTransition( new FSM.Transition( state_idle, new FSM.Condition_not(cond_isMoving), null ) );
        state_walk.AddTransition( new FSM.Transition( state_melee, cond_isMeleeButtonTriggered, null ) );

        // melee to ...
        state_melee.AddTransition( new FSM.Transition( state_idle, 
                                                       new FSM.Condition_and(new FSM.Condition_not(cond_isMoving),
                                                                             new FSM.Condition_not(cond_isAttacking)), 
                                                       null ) );
        state_melee.AddTransition( new FSM.Transition( state_walk, 
                                                       new FSM.Condition_and(cond_isMoving,
                                                                             new FSM.Condition_not(cond_isAttacking)), 
                                                       null ) );

        // init fsm
        this.fsm.init(state_idle);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void HandleInput() {
        Vector2 screen_dir = screenPad ? screenPad.GetMoveDirection() : Vector2.zero;
        if ( screen_dir.sqrMagnitude >= 0.0f ) {
            this.moveDir.x = screen_dir.x;
            this.moveDir.y = screen_dir.y;
            Transform mainCamera = Camera.main.GetComponent( typeof(Transform) ) as Transform;
            this.moveDir = mainCamera.TransformDirection(this.moveDir.normalized); 
            this.moveDir.y = 0.0f;
            this.moveDir = this.moveDir.normalized;
        }
        // TEMP: change to screenPad { 
        if ( Input.GetKeyDown(KeyCode.Space) ) {
            this.meleeButtonTriggered = true;
        }
        // } TEMP end 
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void ProcessMovement () {
        // stop moving when in melee attack state.
        if ( this.fsm.CurrentState().name == "Melee" ) {
            ApplyBrakingForce(10.0f);
            ApplySteeringForce(Vector3.zero);
        }
        else {
            if ( MathHelper.IsZerof(this.moveDir.sqrMagnitude) ) {
                ApplyBrakingForce(10.0f);
            }
            ApplySteeringForce( this.moveDir * base.maxForce );
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
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void NextCombo () {
        AttackInfo atk_info = this.GetAttackInfo();
        if ( atk_info.curCombo == null )
            return;

        // if we have input
        if ( this.meleeButtonTriggered ) {
            AnimationState curAnim = this.anim[atk_info.curCombo.animName];
            ComboInfo nextCombo = atk_info.curCombo.next;
            if ( nextCombo == null )
                return;

            // if we are in the valid input range
            if ( curAnim.time >= atk_info.curCombo.validInputTime.x 
                 && curAnim.time <= atk_info.curCombo.validInputTime.y )
            {
                this.anim[nextCombo.animName].normalizedSpeed = atk_info.speed;
                this.anim.Rewind(atk_info.curCombo.animName);
                this.anim.CrossFade(nextCombo.animName);
                atk_info.curCombo = nextCombo;
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public bool IsMoving () { return this.moveDir.sqrMagnitude > 0.0f; }

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
