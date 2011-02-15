// ======================================================================================
// File         : Actor.cs
// Author       : Wu Jie 
// Last Change  : 12/09/2010 | 16:46:15 PM | Thursday,December
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////
// class PlayerBase
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

[RequireComponent (typeof (Animation))]
public class Actor : Steer {

    ///////////////////////////////////////////////////////////////////////////////
    // actions, conditions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: Action_StopMoving 
    // ------------------------------------------------------------------ 

    protected class Action_StopMoving : FSM.Action {
        Actor actor = null;

        public Action_StopMoving ( Actor _actor ) {
            this.actor = _actor;
        }

        public override void exec () {
            this.actor.Stop();
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: Action_DisableSteering 
    // ------------------------------------------------------------------ 

    protected class Action_DisableSteering : FSM.Action {
        Actor actor = null;

        public Action_DisableSteering ( Actor _actor ) {
            this.actor = _actor;
        }

        public override void exec () {
            this.actor.DisableSteering();
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: Action_EnableSteering 
    // ------------------------------------------------------------------ 

    protected class Action_EnableSteering : FSM.Action {
        Actor actor = null;

        public Action_EnableSteering ( Actor _actor ) {
            this.actor = _actor;
        }

        public override void exec () {
            this.actor.Stop();
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected class Condition_noHP : FSM.Condition {
        ActorInfo info = null;

        public Condition_noHP ( ActorInfo _info ) {
            this.info = _info;
        }

        public override bool exec () {
            return info.curHP <= 0.0f;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected class Condition_isPlayingAnim : FSM.Condition {
        Actor actor = null;
        string animName = "unknown";
        float endTime = 0.0f;

        public Condition_isPlayingAnim ( Actor _actor, string _animName, float _endTime = -1.0f ) {
            this.actor = _actor;
            this.animName = _animName;
            this.endTime = _endTime;
        }

        public override bool exec () {
            return this.actor.IsPlayingAnim(this.animName,this.endTime);
        }
    }

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public enum SteeringState {
        moving,
        seeking,
        braking,
        avoiding,
        disable,
    };

    public float StepSpeed = 0.5f;

    protected HitInfo lastHit = new HitInfo();
    protected Animation anim = null;
    protected FSM fsm = new FSM();
    protected Vector3 targetPos;
    protected Vector3 moveDir;
    protected SteeringState steeringState = SteeringState.braking;

    protected delegate void StateUpdate();
    protected StateUpdate[] States = new StateUpdate[4]; // we allow for different layer states update at the same time.

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected new void Awake () {
        base.Awake();

        // init the player basic values.
        this.anim = gameObject.GetComponent(typeof(Animation)) as Animation;
        this.targetPos = transform.position;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	protected new void Start () {
        base.Start();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	protected new void Update () {
        base.Update();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public bool IsPlayingAnim ( string _animName, float _endTime = -1.0f ) { 
        AnimationState state = this.anim[_animName];
        if ( state == null )
            return false;
        float endTime = _endTime;
        bool result = this.anim.IsPlaying(_animName);

        if ( endTime < 0.0f )
            return result;

        return result && state.time <= endTime;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void Move ( Vector3 _dir ) {
        this.moveDir = _dir;
        this.steeringState = SteeringState.moving;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void Seek ( Vector3 _pos ) {
        this.targetPos = _pos;
        this.steeringState = SteeringState.seeking;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void Stop () {
        this.steeringState = SteeringState.braking;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void Avoid () {
        this.steeringState = SteeringState.avoiding;
    }


    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void DisableSteering () {
        this.steeringState = SteeringState.disable;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected new void ShowDebugInfo () {
        base.ShowDebugInfo();

        // debug info
        DebugHelper.ScreenPrint ( "steering state: " + this.steeringState );
        DebugHelper.ScreenPrint ( "fsm state: " + this.fsm.CurrentState().name );

        // debug animation
        // foreach ( AnimationState animS in this.anim ) {
        //     DebugHelper.ScreenPrint ( animS.name + ": " + animS.enabled );
        // }
    }
}

