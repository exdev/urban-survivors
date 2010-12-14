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
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public enum SteeringState {
        seeking,
        braking,
    };

    public float StepSpeed = 0.5f;

    protected Animation anim = null;
    protected FSM fsm = new FSM();
    protected Vector3 targetPos;
    protected SteeringState steeringState = SteeringState.braking;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	protected new void Start () {
        base.Start();
        // init the player basic values.
        this.anim = gameObject.GetComponent(typeof(Animation)) as Animation;
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

