// ======================================================================================
// File         : fsm.cs
// Author       : Wu Jie 
// Last Change  : 11/21/2010 | 16:04:45 PM | Sunday,November
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
///////////////////////////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////////////
// class FSM
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

public class FSM {

    public const uint MaxTransition = 10;

    ///////////////////////////////////////////////////////////////////////////////
    // base classes
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public class Action {
        public virtual void exec ( GameObject _self ) {
            Debug.LogWarning("Action::exec not implemented, default exec been called.");
        }
    } 

    public class Action_periodic : Action {
        private bool FirstTick = false;
        private float LastTick = 0.0f;

        public float Delay = 0.0f;
        public float Interval = 0.0f;

        public void initTimer ( float _initTime ) { 
            LastTick = _initTime; 
            FirstTick = true;
        }

        public bool tickTimer () {
            float deltaTime = Time.time - LastTick;

            // if we are first time tick, and have Delay
            if ( FirstTick && Delay > 0.0f ) {
                FirstTick = false;
                if ( deltaTime >= Delay ) {
                    LastTick = Time.time;
                    return true;
                }
                return false;
            }

            // check interval time 
            if ( deltaTime >= Interval ) {
                LastTick = Time.time;
                return true;
            }
            return false;
        }

        public override void exec ( GameObject _self ) {
            Debug.LogWarning("Action::exec not implemented, default exec been called.");
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public class State {
        public string name = "";
        public Action entry_action = null;
        public Action exit_action = null;
        public Action step_action = null;
        public Transition[] transition_list = new Transition[MaxTransition];
        public uint transition_count = 0;

        public State ( string _name, Action _entry, Action _step, Action _exit ) {
            name = _name;
            entry_action = _entry;
            step_action = _step;
            exit_action = _exit;
        }

        public void AddTransition ( Transition _trans ) {
            DebugHelper.Assert( _trans != null, "the in transition can't not be null" );
            DebugHelper.Assert( transition_count < MaxTransition, "can't add more transition, the max count is 10!" );
            if ( _trans == null || transition_count >= MaxTransition )
                return;
                
            transition_list[transition_count] = _trans;
            ++transition_count;
        }
    } 

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public class Transition {
        // public State src_state = null;
        public State dest_state = null;
        public Action action = null;

        public virtual bool check ( GameObject _self ) { return false; } 
    } 

    ///////////////////////////////////////////////////////////////////////////////
    // private member
    ///////////////////////////////////////////////////////////////////////////////

    State init_state = null;
    State cur_state = null;
    List<Action> cur_actions = new List<Action>();
    GameObject self = null;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public State CurrentState () { return this.cur_state; }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	public void init ( GameObject _self, State _initState ) {
        this.self = _self;
        DebugHelper.Assert( _initState != null, "init state can't be null, pls set it before using the state machine" );
        this.init_state = _initState;
	}
	
    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	public void tick () {
        // clear the action list
        this.cur_actions.Clear();

        // if we don't have any state, begin with init state.
        if ( this.cur_state == null ) {
            if ( this.init_state != null ) {
                this.cur_state = this.init_state;
                this.cur_actions.Add ( this.cur_state.entry_action );
            }
        }
        else {
            // check if we have any transition satisfied the condition
            Transition triggeredTrans = null;
            for ( uint i = 0; i < this.cur_state.transition_count; ++i ) {
                Transition trans = this.cur_state.transition_list[i];
                if ( trans.check(self) ) {
                    triggeredTrans = trans;
                    break;
                }
            }

            // if we have transition triggered
            if ( triggeredTrans != null ) {
                State nextState = triggeredTrans.dest_state;
                if ( this.cur_state.exit_action != null )
                    this.cur_actions.Add ( this.cur_state.exit_action );
                if ( triggeredTrans.action != null )
                    this.cur_actions.Add ( triggeredTrans.action );
                if ( nextState.entry_action != null )
                    this.cur_actions.Add ( nextState.entry_action );
                this.cur_state = nextState;

                // if the action is periodic action
                Action_periodic action = nextState.step_action as Action_periodic;
                if ( action != null ) {
                    action.initTimer(Time.time); // we set the lastTick so that Action periodic can check by itself.
                }
            }
            // otherwise, just perform current state's action 
            else {
                if ( this.cur_state.step_action != null ) {
                    this.cur_actions.Add ( this.cur_state.step_action );
                }
            }
        }

        // perform the actions
        foreach ( Action act in this.cur_actions ) {
            Action_periodic act_p = act as Action_periodic;
            if ( act_p != null && act_p.tickTimer() )
                act_p.exec (self);
            else
                act.exec (self);
        }
	}
}
