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

    // Action
    public class Action {
        public virtual void exec () {
            Debug.LogWarning("Action::exec not implemented, default exec been called.");
        }
    } 

    // Action_list
    public class Action_list : Action {
        Action[] actions;
        public Action_list ( Action[] _actions ) {
            this.actions = _actions;
        }
        public override void exec () {
            for ( int i = 0; i < actions.Length; ++i ) {
                this.actions[i].exec();
            }
        }
    }

    // Action_periodic
    public class Action_periodic : Action {
        private bool firstTick = false;
        private float lastTick = 0.0f;

        public float delay = 0.0f;
        public float interval = 0.0f;

        public Action_periodic ( float _delay, float _interval ) {
            this.delay = _delay;
            this.interval = _interval;
        }

        public void initTimer ( float _initTime ) { 
            this.lastTick = _initTime; 
            this.firstTick = true;
        }

        public bool tickTimer () {
            float deltaTime = Time.time - this.lastTick;

            // if we are first time tick, and have Delay
            if ( this.firstTick && this.delay > 0.0f ) {
                this.firstTick = false;
                if ( deltaTime >= this.delay ) {
                    this.lastTick = Time.time;
                    return true;
                }
                return false;
            }

            // check interval time 
            if ( deltaTime >= this.interval ) {
                this.lastTick = Time.time;
                return true;
            }
            return false;
        }

        public override void exec () {
            Debug.LogWarning("Action::exec not implemented, default exec been called.");
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    // Condition
    public class Condition {
        public virtual bool exec () {
            Debug.LogWarning("Condition::exec not implemented, default exec been called.");
            return false;
        } 
    }

    // Condition_not
    public class Condition_not : Condition {
        Condition cond = null; 
        public Condition_not ( Condition _cond ) { this.cond = _cond; }
        public override bool exec () { return !this.cond.exec(); }
    }

    // Condition_and
    public class Condition_and : Condition {
        Condition cond1 = null; 
        Condition cond2 = null; 
        public Condition_and ( Condition _cond1, Condition _cond2 ) { 
            this.cond1 = _cond1; 
            this.cond2 = _cond2; 
        }
        public override bool exec () { return this.cond1.exec() && this.cond2.exec(); }
    }

    // Condition_or
    public class Condition_or : Condition {
        Condition cond1 = null; 
        Condition cond2 = null; 
        public Condition_or ( Condition _cond1, Condition _cond2 ) { 
            this.cond1 = _cond1; 
            this.cond2 = _cond2; 
        }
        public override bool exec () { return this.cond1.exec() || this.cond2.exec(); }
    }

    //
    public class Condition_waitForSeconds : Condition {
        float seconds = _seconds;
        float startTime = 0.0f;
        bool firstTime = true;
        public Condition_waitForSeconds ( float _seconds ) { 
            this.seconds = _seconds;
        }
        public override bool exec () { 
            // if this is first time tick, just record the time.
            if ( this.firstTime ) {
                this.startTime = Time.time;
                this.firstTime = false;
                return false;
            }

            // if we exceed the time
            if ( Time.time - this.startTime > this.seconds ) {
                this.firstTime = true;
                return true;
            }
            return false;
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
        public State dest_state = null;
        public Condition condition = null;
        public Action action = null;

        public Transition ( State _dest, Condition _cond, Action _act ) {
            this.dest_state = _dest;
            this.condition = _cond;
            this.action = _act;
        }

        public bool check () { 
            return condition.exec(); 
        }
    } 

    ///////////////////////////////////////////////////////////////////////////////
    // private member
    ///////////////////////////////////////////////////////////////////////////////

    State init_state = null;
    State cur_state = null;
    List<Action> cur_actions = new List<Action>();

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

	public void init ( State _initState ) {
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
                if ( this.cur_state.entry_action != null )
                    this.cur_actions.Add ( this.cur_state.entry_action );
            }
        }
        else {
            // check if we have any transition satisfied the condition
            Transition triggeredTrans = null;
            for ( uint i = 0; i < this.cur_state.transition_count; ++i ) {
                Transition trans = this.cur_state.transition_list[i];
                if ( trans.check() ) {
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
            if ( act_p != null ) {
                if ( act_p.tickTimer() )
                    act_p.exec ();
                continue;
            }
            act.exec ();
        }
	}
}
