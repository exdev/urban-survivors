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
        public Animation anim_comp;
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
            Attack_info atk_info = playerBoy.GetAttackInfo();
            if ( atk_info.curCombo == null )
                return false;
            return this.playerBoy.IsPlayingAnim( atk_info.curCombo.animName, 
                                                 atk_info.curCombo.endTime );

            // DELME { 
            // Combo_info combo = atk_info.combo_entry;
            // while ( combo != null ) {
            //     bool attacking = this.playerBoy.IsPlayingAnim( combo.animName, combo.endTime );
            //     if ( attacking )
            //         return true;
            //     combo = combo.next;
            // }
            // return false;
            // } DELME end 
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

    public Player_info player_info = new Player_info();
    public Transform weaponAnchor = null;

    protected Vector3 moveDir = Vector3.zero;
    protected bool meleeButtonTriggered = false;
    protected GameObject curWeapon = null;
    protected GameObject weapon1 = null;
    protected GameObject weapon2 = null;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	protected new void Start () {
        base.Start();

        // init the player basic values.
        DebugHelper.Assert( this.weaponAnchor, "can't find WeaponAnchor");

        //
        this.InitInfo();
        this.InitAnim();
        this.InitFSM();
	}
	
    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	void Update () {
        //
        HandleInput();

        // update state machine
        this.fsm.tick();

        // handle steering
        ProcessMovement ();

        // reset the internal state.
        this.meleeButtonTriggered = false;
        this.moveDir = Vector3.zero; 

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
        // } DEBUG end 
	}

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void InitInfo () {
        // TODO: we should load info from saved data or check point.
        // info.serialize();

        if ( this.player_info.weapon1 != WeaponBase.WeaponID.unknown )
            this.ChangeWeapon(this.player_info.weapon1);
        else if ( this.player_info.weapon2 != WeaponBase.WeaponID.unknown )
            this.ChangeWeapon(this.player_info.weapon2);
        DebugHelper.Assert( this.curWeapon, "can't find any valid weapon" );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void InitAnim () {
        // setup the animation state
        AnimationState state;
        string[] anim_keys = { "moveforward" };
        foreach (string key in anim_keys) {
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

        state = this.anim["idle"];
        state.wrapMode = WrapMode.Loop;
        state.layer = 0;
        state.weight = 1.0f;
        state.enabled = true;
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

    private void HandleInput() {
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

    private void ProcessMovement () {
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

    public void ChangeWeapon( WeaponBase.WeaponID _id ) {
        GameObject weaponGO = WeaponBase.Instance().GetWeapon(_id); 
        weaponGO.SetActiveRecursively(true);
        this.curWeapon = weaponGO; 
        this.curWeapon.transform.parent = this.weaponAnchor;
        this.curWeapon.transform.localPosition = Vector3.zero;
        this.curWeapon.transform.localRotation = Quaternion.identity;
        // TEMP HACK { 
        // this.curWeapon.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 90.0f);
        // } TEMP HACK end 

        Attack_info attackInfo = this.GetAttackInfo();
        attackInfo.setOwnerInfo(this.player_info);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void StartCombo () {
        Attack_info atk_info = this.GetAttackInfo();
        Combo_info first_combo = atk_info.combo_entry;
        atk_info.curCombo = first_combo;
        this.anim.Rewind(first_combo.animName);
        this.anim.CrossFade(first_combo.animName);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void NextCombo () {
        Attack_info atk_info = this.GetAttackInfo();
        if ( atk_info.curCombo == null )
            return;

        // if we have input
        if ( this.meleeButtonTriggered ) {
            AnimationState curAnim = this.anim[atk_info.curCombo.animName];
            Combo_info nextCombo = atk_info.curCombo.next;
            if ( nextCombo == null )
                return;

            // if we are in the valid input range
            if ( curAnim.time >= atk_info.curCombo.validInputTime.x 
                 && curAnim.time <= atk_info.curCombo.validInputTime.y )
            {
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

    public Attack_info GetAttackInfo () { return this.curWeapon.GetComponent<Attack_info>(); }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    private void StartMeleeAttack () {
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    private void EndMeleeAttack () {
    }
}
