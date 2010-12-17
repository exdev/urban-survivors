// ======================================================================================
// File         : AI_ZombieBase.cs
// Author       : Wu Jie 
// Last Change  : 12/17/2010 | 09:42:09 AM | Friday,December
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////
// defines
///////////////////////////////////////////////////////////////////////////////

public class AI_ZombieBase : Actor {

    ///////////////////////////////////////////////////////////////////////////////
    // actions, conditions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: Action_MoveToNearestAlivedPlayer 
    // ------------------------------------------------------------------ 

    protected class Action_MoveToNearestAlivedPlayer : FSM.Action_periodic {
        Actor actor = null;

        public Action_MoveToNearestAlivedPlayer ( float _interval, Actor _actor ) 
            : base ( 0.0f, _interval )
        {
            this.actor = _actor;
        }

        public override void exec () {
            float dist = 0.0f;
            Transform player = null;
            GameRules.Instance().GetNearestAlivedPlayer( this.actor.transform,
                                                         out player,
                                                         out dist );
            // it is possible that all the player down
            if ( player != null ) {
                this.actor.Seek(player.position);
            }
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
            GameRules.Instance().GetNearestAlivedPlayer( this.actor.transform,
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

    protected static GameObject fxHitBullet = null;
    protected static GameObject fxHitMelee = null;

    public ActorInfo zombieInfo = new ActorInfo();
    public GameObject FX_HIT_bullet = null;
    public GameObject FX_HIT_melee = null;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Awake () {
        if ( fxHitBullet  == null && this.FX_HIT_bullet ) {
            fxHitBullet = (GameObject)Instantiate( this.FX_HIT_bullet );
        }
        if ( fxHitMelee == null && this.FX_HIT_melee ) {
            fxHitMelee = (GameObject)Instantiate( this.FX_HIT_melee );
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	protected new void Start () {
        base.Start();
    }
}
