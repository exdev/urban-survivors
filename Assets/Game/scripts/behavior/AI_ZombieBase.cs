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
    // Desc: 
    // ------------------------------------------------------------------ 

    protected class Action_CleanDeadBody : FSM.Action {
        GameObject go = null;

        public Action_CleanDeadBody ( GameObject _go ) {
            this.go = _go;
        } 

        public override void exec () {
            GameObject.Destroy(this.go);
        }
    }

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

    [System.Serializable]
    public class DropItem {
        public GameObject prefab;
        public float probability;
    }

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    protected static GameObject fxHitBullet = null;
    protected static GameObject fxHitMelee = null;
    protected Probability dropItemProbability = new Probability();

    public ZombieInfo zombieInfo = new ZombieInfo();
    public GameObject FX_HIT_bullet = null;
    public GameObject FX_HIT_melee = null;
    public float deadBodyKeepTime = 2.0f;
    public DropItem[] dropItems;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Awake () {
        if ( fxHitBullet == null && this.FX_HIT_bullet ) {
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

        float[] weights = new float[dropItems.Length]; 
        for ( int i = 0; i < dropItems.Length; ++i ) {
            weights[i] = dropItems[i].probability;
        }
        dropItemProbability.Init(weights);

        InitInfo();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnDisable () {
        // Debug.Log("hello world");
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void InitInfo () {
        this.zombieInfo.curHP = this.zombieInfo.maxHP;
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

    protected new void ShowDebugInfo () {
        base.ShowDebugInfo();

        //
        DebugHelper.ScreenPrint ( "target pos = " + this.targetPos );
        DebugHelper.ScreenPrint ( "curHP = " + this.zombieInfo.curHP );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public virtual void OnDead () {
        int i = dropItemProbability.GetIndex();
        if ( dropItems[i].prefab )
            GameObject.Instantiate( dropItems[i].prefab, transform.position, Quaternion.identity );
    }
}

