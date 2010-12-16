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
    // Desc: Action_MoveToNearestPlayer 
    // ------------------------------------------------------------------ 

    protected class Action_MoveToNearestPlayer : FSM.Action_periodic {
        Actor actor = null;

        public Action_MoveToNearestPlayer ( float _interval, Actor _actor ) 
            : base ( 0.0f, _interval )
        {
            this.actor = _actor;
        }

        public override void exec () {
            float dist = 0.0f;
            Transform player = null;
            GameRules.Instance().GetNearestPlayer( this.actor.transform,
                                                   out player,
                                                   out dist );
            this.actor.Seek(player.position);
        }
    }

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
    // Desc: Condition_isPlayerInAttackRange
    // ------------------------------------------------------------------ 

    protected class Condition_isPlayerInAttackRange : FSM.Condition {
        Actor actor = null;
        float degrees = 30.0f;
        float distance = 2.0f;

        public Condition_isPlayerInAttackRange ( Actor _actor, float _degrees, float _dist ) {
            this.actor = _actor;
            this.degrees = _degrees;
            this.distance = _dist;
        }

        public override bool exec () {
            float dist = 0.0f;
            Transform player = null;
            GameRules.Instance().GetNearestPlayer( this.actor.transform,
                                                   out player,
                                                   out dist );
            if ( dist > this.distance ) // not in distance 
                return false;

            // if we near target, check if we face it.
            bool result = this.actor.IsAhead( player.position, 
                                              Mathf.Cos(this.degrees*Mathf.Deg2Rad) );
            return result;
        }
    }

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    protected static GameObject bulletHitEffect = null;
    protected static GameObject meleeHitEffect = null;

    public enum SteeringState {
        seeking,
        braking,
    };

    public float StepSpeed = 0.5f;
    public GameObject prefab_BulletHitEffect = null;
    public GameObject prefab_MeleeHitEffect = null;

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

    void Awake () {
        if ( bulletHitEffect == null && this.prefab_BulletHitEffect ) {
            bulletHitEffect = (GameObject)Instantiate( this.prefab_BulletHitEffect );
        }
        if ( meleeHitEffect == null && this.prefab_MeleeHitEffect ) {
            meleeHitEffect = (GameObject)Instantiate( this.prefab_MeleeHitEffect );
        }
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

