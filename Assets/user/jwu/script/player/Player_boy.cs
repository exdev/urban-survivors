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
// action, transitions
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

    public override void exec ( GameObject _self ) {
        this.anim_comp["moveforward"].normalizedSpeed 
            = Mathf.Max(this.playerBoy.StepSpeed * this.playerBoy.CurSpeed(),1.0f);
    }
}

// ------------------------------------------------------------------ 
// Desc: Transition_Idle_to_Walk 
// ------------------------------------------------------------------ 

class Transition_Idle_to_Walk : FSM.Transition {
    Player_boy playerBoy = null;

    public Transition_Idle_to_Walk ( FSM.State _dest, Player_boy _playerBoy ) {
        base.dest_state = _dest;
        this.playerBoy = _playerBoy;
    }

    public override bool check ( GameObject _self ) { 
        return this.playerBoy.IsMoving();
    } 
}

// ------------------------------------------------------------------ 
// Desc: Transition_Idle_to_Melee 
// ------------------------------------------------------------------ 

class Transition_Idle_to_Melee : FSM.Transition {
    Player_boy playerBoy = null;

    public Transition_Idle_to_Melee ( FSM.State _dest, Player_boy _playerBoy ) {
        base.dest_state = _dest;
        this.playerBoy = _playerBoy;
    }

    public override bool check ( GameObject _self ) {
        return this.playerBoy.MeleeButtonTriggered();
    }
}

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

class Transition_Walk_to_Idle : FSM.Transition {
    Player_boy playerBoy = null;

    public Transition_Walk_to_Idle ( FSM.State _dest, Player_boy _playerBoy ) {
        base.dest_state = _dest;
        this.playerBoy = _playerBoy;
    }

    public override bool check ( GameObject _self ) { 
        return this.playerBoy.IsMoving() == false;
    } 
}

// ------------------------------------------------------------------ 
// Desc: Transition_Walk_to_Melee 
// ------------------------------------------------------------------ 

class Transition_Walk_to_Melee : FSM.Transition {
    Player_boy playerBoy = null;

    public Transition_Walk_to_Melee ( FSM.State _dest, Player_boy _playerBoy ) {
        base.dest_state = _dest;
        this.playerBoy = _playerBoy;
    }

    public override bool check ( GameObject _self ) {
        return this.playerBoy.MeleeButtonTriggered();
    }
}

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

class Transition_Melee_to_Idle : FSM.Transition {
    Player_boy playerBoy = null;

    public Transition_Melee_to_Idle ( FSM.State _dest, Player_boy _playerBoy ) {
        base.dest_state = _dest;
        this.playerBoy = _playerBoy;
    }

    public override bool check ( GameObject _self ) { 
        return (this.playerBoy.IsMoving() == false 
                && this.playerBoy.IsPlayingAnim("melee1_copy", 0.5f) == false);
    } 
}

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

class Transition_Melee_to_Walk : FSM.Transition {
    Player_boy playerBoy = null;

    public Transition_Melee_to_Walk ( FSM.State _dest, Player_boy _playerBoy ) {
        base.dest_state = _dest;
        this.playerBoy = _playerBoy;
    }

    public override bool check ( GameObject _self ) { 
        return (this.playerBoy.IsMoving()
                && this.playerBoy.IsPlayingAnim("melee1_copy", 0.5f) == false);
    } 
}

///////////////////////////////////////////////////////////////////////////////
// class Player_boy
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

[RequireComponent (typeof (Animation))]
public class Player_boy : Player_base {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    protected Vector3 moveDir = Vector3.zero;
    protected Animation anim;
    protected FSM fsm = new FSM();
    protected bool meleeButtonTriggered = false;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	protected new void Start () {
        base.Start();
        this.InitAnim ();
        this.InitFSM();
	}
	
    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	void Update () {
        //
        HandleInput();

        // update state machine
        fsm.tick();

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
        DebugHelper.ScreenPrint ( "Player_boy current state: " + fsm.CurrentState().name );
        // } DEBUG end 
	}

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void InitAnim () {
        // get the animation component
        anim = gameObject.GetComponent(typeof(Animation)) as Animation;

        // setup the animation state
        AnimationState state;
        string[] anim_keys = { "moveforward" };
        foreach (string key in anim_keys) {
            state = anim[key];
            state.layer = 0;
            state.wrapMode = WrapMode.Loop;
            state.weight = 1.0f;
            state.enabled = false;
        }

        state = anim["melee1_copy"];
        state.layer = 0;
        state.wrapMode = WrapMode.Once;
        state.weight = 1.0f;
        state.enabled = false;

        state = anim["idle"];
        state.wrapMode = WrapMode.Loop;
        state.layer = 0;
        state.weight = 1.0f;
        state.enabled = true;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void InitFSM () {
        // init states
        FSM.State State_Idle = new FSM.State( "Idle", 
                                              new Action_PlayAnim(anim,"idle"), 
                                              null, 
                                              null );
        FSM.State State_Walk = new FSM.State( "Walk", 
                                              new Action_PlayAnim(anim,"moveforward"), 
                                              new Action_AdjustMoveSpeed(anim,this),
                                              null );
        FSM.State State_Melee = new FSM.State( "Melee", 
                                               new Action_PlayAnim(anim,"melee1_copy"), 
                                               null,
                                               null );

        // connect idle transitions
        {
            Transition_Idle_to_Walk trans1 = new Transition_Idle_to_Walk(State_Walk,this);
            Transition_Idle_to_Melee trans2 = new Transition_Idle_to_Melee(State_Melee,this);
            State_Idle.AddTransition(trans1);
            State_Idle.AddTransition(trans2);
        }

        // connect move towards players transitions
        {
            Transition_Walk_to_Idle trans1 = new Transition_Walk_to_Idle(State_Idle,this);
            Transition_Walk_to_Melee trans2 = new Transition_Walk_to_Melee(State_Melee,this);

            State_Walk.AddTransition(trans1);
            State_Walk.AddTransition(trans2);
        }

        // connect melee transitions
        {
            Transition_Melee_to_Idle trans1 = new Transition_Melee_to_Idle(State_Idle,this);
            Transition_Melee_to_Walk trans2 = new Transition_Melee_to_Walk(State_Idle,this);
            State_Melee.AddTransition(trans1);
            State_Melee.AddTransition(trans2);
        }

        // init fsm
        fsm.init(gameObject,State_Idle);
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
        if ( fsm.CurrentState().name == "Melee" ) {
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

    public bool IsMoving () { return this.moveDir.sqrMagnitude > 0.0f; }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public bool MeleeButtonTriggered () { return this.meleeButtonTriggered; }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public bool IsPlayingAnim ( string _animName, float _percentage = 1.0f ) { 
        AnimationState state = anim[_animName];
        DebugHelper.Assert( state != null, "can't find animation state: " + _animName );
        return state.enabled && state.time/state.length <= _percentage;
    }

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
