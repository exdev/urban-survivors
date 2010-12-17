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
// class Player_base
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
    };

    public float StepSpeed = 0.5f;

    protected Animation anim = null;
    protected FSM fsm = new FSM();
    protected Vector3 targetPos;
    protected Vector3 moveDir;
    protected SteeringState steeringState = SteeringState.braking;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Awake () {
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	protected new void Start () {
        base.Start();
        // init the player basic values.
        this.anim = gameObject.GetComponent(typeof(Animation)) as Animation;
        this.targetPos = transform.position;
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
}

