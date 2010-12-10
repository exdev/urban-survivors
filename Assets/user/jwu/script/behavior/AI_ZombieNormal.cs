// ======================================================================================
// File         : AI_ZombieNormal.cs
// Author       : Wu Jie 
// Last Change  : 11/21/2010 | 21:21:07 PM | Sunday,November
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

///////////////////////////////////////////////////////////////////////////////
// class AI_ZombieNormal
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

public class AI_ZombieNormal : Actor {

    ///////////////////////////////////////////////////////////////////////////////
    // actions, conditions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: Action_MoveToNearestPlayer 
    // ------------------------------------------------------------------ 

    class Action_MoveToNearestPlayer : FSM.Action_periodic {
        AI_ZombieNormal zombieNormal = null;

        public Action_MoveToNearestPlayer ( float _interval, AI_ZombieNormal _zombieNormal ) 
            : base ( 0.0f, _interval )
        {
            this.zombieNormal = _zombieNormal;
        }

        public override void exec () {
            float dist = 0.0f;
            Transform player = null;
            GameRules.Instance().GetNearestPlayer( this.zombieNormal.transform,
                                                   out player,
                                                   out dist );
            this.zombieNormal.Seek(player.position);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: Action_Attack 
    // ------------------------------------------------------------------ 

    // class Action_Attack : FSM.Action_periodic {
    class Action_Attack : FSM.Action {
        Animation anim = null;
        public Action_Attack ( Animation _anim ) {
            this.anim = _anim;
        }

        public override void exec () {
            this.anim.Rewind("attack1");
            this.anim.CrossFade("attack1");
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: Action_StopMoving 
    // ------------------------------------------------------------------ 

    class Action_StopMoving : FSM.Action {
        AI_ZombieNormal zombieNormal = null;

        public Action_StopMoving ( AI_ZombieNormal _zombieNormal ) {
            this.zombieNormal = _zombieNormal;
        }

        public override void exec () {
            this.zombieNormal.Stop();
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: Action_ActOnHit 
    // ------------------------------------------------------------------ 

    class Action_ActOnHit : FSM.Action {
        AI_ZombieNormal zombieNormal = null;

        public Action_ActOnHit ( AI_ZombieNormal _zombieNormal ) {
            this.zombieNormal = _zombieNormal;
        }

        public override void exec () {
            this.zombieNormal.ActOnHit();
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    class Action_ActOnDead : FSM.Action {
        AI_ZombieNormal zombieNormal = null;

        public Action_ActOnDead ( AI_ZombieNormal _zombieNormal ) {
            this.zombieNormal = _zombieNormal;
        }

        public override void exec () {
            this.zombieNormal.ActOnDead();
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: Condition_playerInRange 
    // ------------------------------------------------------------------ 

    class Condition_playerInRange : FSM.Condition {
        float range = 5.0f;
        Transform trans = null;

        public Condition_playerInRange ( Transform _trans, float _range ) {
            this.range = _range;
            this.trans = _trans;
        }

        public override bool exec () { 
            GameObject[] players = GameRules.Instance().GetPlayers();
            foreach( GameObject player in players ) {
                float len = (player.transform.position - this.trans.position).magnitude;
                if ( len < range ) {
                    return true;
                }
            }
            return false; 
        } 
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    class Condition_arriveDestination : FSM.Condition {
        AI_ZombieNormal zombieNormal = null;
        public Condition_arriveDestination ( AI_ZombieNormal _zombieNormal ) {
            this.zombieNormal = _zombieNormal;
        }
        public override bool exec () {
            return this.zombieNormal.arriveDestination();
        }
    } 

    // ------------------------------------------------------------------ 
    // Desc: Condition_isPlayerInAttackRange
    // ------------------------------------------------------------------ 

    class Condition_isPlayerInAttackRange : FSM.Condition {
        AI_ZombieNormal zombieNormal = null;
        float degrees = 30.0f;

        public Condition_isPlayerInAttackRange ( AI_ZombieNormal _zombieNormal, float _degrees ) {
            this.zombieNormal = _zombieNormal;
            this.degrees = _degrees;
        }

        public override bool exec () {
            float dist = 0.0f;
            Transform player = null;
            GameRules.Instance().GetNearestPlayer( this.zombieNormal.transform,
                                                   out player,
                                                   out dist );
            if ( dist > 2.0f ) // not in distance 
                return false;

            // if we near target, check if we face it.
            bool result = this.zombieNormal.IsAhead( player.position, 
                                                     Mathf.Cos(this.degrees*Mathf.Deg2Rad) );
            return result;
        }

    }

    // ------------------------------------------------------------------ 
    // Desc: Condition_isAttacking 
    // ------------------------------------------------------------------ 

    class Condition_isAttacking : FSM.Condition {
        AI_ZombieNormal zombieNormal = null;

        public Condition_isAttacking ( AI_ZombieNormal _zombieNormal ) {
            this.zombieNormal = _zombieNormal;
        }

        public override bool exec () {
            return this.zombieNormal.IsPlayingAnim("attack1");
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    class Condition_isOnHit : FSM.Condition {
        AI_ZombieNormal zombieNormal = null;

        public Condition_isOnHit ( AI_ZombieNormal _zombieNormal ) {
            this.zombieNormal = _zombieNormal;
        }

        public override bool exec () {
            return this.zombieNormal.isOnHit();
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    class Condition_noHP : FSM.Condition {
        AI_ZombieNormal zombieNormal = null;

        public Condition_noHP ( AI_ZombieNormal _zombieNormal ) {
            this.zombieNormal = _zombieNormal;
        }

        public override bool exec () {
            ActorInfo actInfo = this.zombieNormal.zombie_info;
            return actInfo.curHP <= 0.0f;
        }
    }

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public enum SteeringState {
        seeking,
        braking,
    };

    public ActorInfo zombie_info = new ActorInfo();
    protected Vector3 targetPos;
    protected SteeringState steeringState = SteeringState.braking;
    protected HitInfo lastHit = new HitInfo();

    ///////////////////////////////////////////////////////////////////////////////
    // function defines
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void Seek ( Vector3 _pos ) {
        this.targetPos = _pos;
        this.steeringState = SteeringState.seeking;
    }

    public void Stop () {
        this.steeringState = SteeringState.braking;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected void InitAnim () {
        AnimationState state;
        anim = transform.GetComponent(typeof(Animation)) as Animation;

        string[] anim_keys1 = { "moveForward", "idle1", "idle2" };
        foreach (string key in anim_keys1) {
            state = anim[key];
            state.layer = 0;
            state.wrapMode = WrapMode.Loop;
            state.weight = 1.0f;
            state.enabled = false;
        }

        string[] anim_keys2 = { "attack1", "hit1", "hit2" };
        foreach (string key in anim_keys2) {
            state = anim[key];
            state.layer = 0;
            state.wrapMode = WrapMode.Once;
            state.weight = 1.0f;
            state.enabled = false;
        }
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
                                              new Action_PlayAnim(this.anim,"idle1"), 
                                              new Action_RandomAnim(4.0f, 4.0f, this.anim, new string[]{"idle1","idle2"} ),
                                              null );
        // seekPlayers
        FSM.State state_seekPlayers = new FSM.State( "SeekPlayers", 
                                                     new Action_PlayAnim(this.anim,"moveForward"), 
                                                     new Action_MoveToNearestPlayer(0.5f,this), 
                                                     new Action_StopMoving(this) );
        // attack
        FSM.State state_attack = new FSM.State( "Attack", 
                                                new Action_Attack(this.anim), 
                                                null,
                                                null );
        // get hit
        FSM.State state_onHit = new FSM.State( "OnHit", 
                                               new Action_ActOnHit(this), 
                                               null,
                                               null );
        // dead
        FSM.State state_Dead = new FSM.State( "Dead", 
                                               new Action_ActOnDead(this), 
                                               null, // TODO: dead clean up
                                               null );

        // ======================================================== 
        // condition 
        // ======================================================== 

        FSM.Condition cond_isPlayerInAttackRange = new Condition_isPlayerInAttackRange(this,30.0f);
        FSM.Condition cond_isAttacking = new Condition_isAttacking(this);
        FSM.Condition cond_isOnHit = new Condition_isOnHit(this);
        FSM.Condition cond_noHP = new Condition_noHP(this);

        // ======================================================== 
        // setup transitions
        // ======================================================== 

        // idle to ...
        state_idle.AddTransition( new FSM.Transition( state_Dead,
                                                      cond_noHP,
                                                      null ) );
        state_idle.AddTransition( new FSM.Transition( state_onHit,
                                                      cond_isOnHit,
                                                      null ) );
        state_idle.AddTransition( new FSM.Transition( state_seekPlayers, 
                                                      new Condition_playerInRange( this.transform, 5.0f ), 
                                                      null ) );

        // seekPlayers to ...
        state_seekPlayers.AddTransition( new FSM.Transition( state_Dead,
                                                             cond_noHP,
                                                             null ) );
        state_seekPlayers.AddTransition( new FSM.Transition( state_onHit,
                                                             cond_isOnHit,
                                                             null ) );
        state_seekPlayers.AddTransition( new FSM.Transition( state_attack,
                                                             cond_isPlayerInAttackRange,
                                                             null ) );
        state_seekPlayers.AddTransition( new FSM.Transition( state_idle, 
                                                             new FSM.Condition_not( new Condition_playerInRange( this.transform, 7.0f ) ) , 
                                                             null ) );
        // attack to ...
        state_attack.AddTransition( new FSM.Transition( state_Dead,
                                                        cond_noHP,
                                                        null ) );
        state_attack.AddTransition( new FSM.Transition( state_onHit,
                                                        cond_isOnHit,
                                                        null ) );
        state_attack.AddTransition( new FSM.Transition ( state_idle, 
                                                         new FSM.Condition_not(cond_isAttacking),
                                                         null ) );
        // on hit to ...
        state_onHit.AddTransition( new FSM.Transition( state_Dead,
                                                       cond_noHP,
                                                       null ) );
        state_onHit.AddTransition( new FSM.Transition ( state_idle, 
                                                        new FSM.Condition_not(cond_isOnHit),
                                                        null ) );
        // TODO { 
        // state_onHit.AddTransition( new FSM.Transition ( state_onHit, 
        //                                                 cond_isOnHit,
        //                                                 null ) );
        // } TODO end 

        // init fsm
        fsm.init(state_idle);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected new void Start () {
        base.Start();

        this.targetPos = transform.position;
        this.InitAnim();
        this.InitFSM();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Update () {
        this.fsm.tick(); // update state machine

        // stop if we arrive target position
        if ( (this.targetPos - transform.position).magnitude < 0.1f ) {
            this.Stop();
        }

        // handle steering
        Vector3 force = Vector3.zero;
        if ( this.steeringState == SteeringState.seeking ) {
            float distance = (transform.position - this.targetPos).magnitude;
            if ( distance < 2.0f ) {
                ApplyBrakingForce(10.0f);

                // face the target
                float rot_speed = 2.0f; // TEMP
                Vector3 dir = this.targetPos - transform.position;
                Quaternion wanted_rot = Quaternion.LookRotation(dir);
                wanted_rot.x = 0.0f; wanted_rot.z = 0.0f;
                transform.rotation = Quaternion.Slerp ( 
                                                       transform.rotation, 
                                                       wanted_rot, 
                                                       rot_speed * Time.deltaTime
                                                      );
            }
            else {
                force = GetSteering_Seek_LimitByMaxSpeed ( this.targetPos );
                force.y = 0.0f;
            }
        }
        else if ( this.steeringState == SteeringState.braking ) {
            ApplyBrakingForce(10.0f);
        }
        ApplySteeringForce(force);

        // reset values
        this.lastHit.hitType = HitInfo.HitType.none;

        // DEBUG { 
        // draw velocity
        Vector3 vel = base.Velocity(); 
        DebugHelper.DrawLine ( transform.position, 
                               transform.position + vel * 3.0f, 
                               new Color(0.0f,1.0f,0.0f) );
        // draw smoothed acceleration
        Vector3 acc = base.smoothedAcceleration;
        DebugHelper.DrawLine ( transform.position, 
                               transform.position + acc * 3.0f, 
                               new Color(1.0f,0.0f,1.0f) );
        // draw target pos
        DebugHelper.DrawDestination ( this.targetPos );
        DebugHelper.DrawCircleY( transform.position, 5.0f, Color.yellow );

        // debug info
        DebugHelper.ScreenPrint ( "AI_ZombieNormal steering state: " + this.steeringState );
        DebugHelper.ScreenPrint ( "AI_ZombieNormal current state: " + fsm.CurrentState().name );

        // Vector3 targetDir = (this.targetPos - transform.position).normalized;
        // float cosTheta = Vector3.Dot ( transform.forward, targetDir );
        // DebugHelper.ScreenPrint ( "angle: " + Mathf.Acos(cosTheta) * Mathf.Rad2Deg );
        // DebugHelper.ScreenPrint ( "target pos: " + this.targetPos );

        // DEBUG actorInfo
        DebugHelper.ScreenPrint ( "curHP = " + this.zombie_info.curHP );
        DebugHelper.ScreenPrint ( "maxHP = " + this.zombie_info.maxHP );

        // DEBUG animation
        // foreach ( AnimationState animS in this.anim ) {
        //     DebugHelper.ScreenPrint ( animS.name + ": " + animS.enabled );
        // }
        // } DEBUG end 
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnTriggerEnter ( Collider _other ) {
        if ( _other.gameObject.layer == Layer.melee_player 
          || _other.gameObject.layer == Layer.bullet_player ) 
        {
            DamageInfo dmgInfo = _other.gameObject.GetComponent<DamageInfo>();
            float dmgOutput = DamageRule.Instance().CalculateDamage( this.zombie_info, dmgInfo );

            // TODO { 
            // if ( dmgOutput < 20.0f )
            //     this.lastHit.hitType = HitInfo.HitType.light;
            // else if ( dmgOutput >= 20.0f )
            //     this.lastHit.hitType = HitInfo.HitType.normal;
            this.lastHit.hitType = HitInfo.HitType.normal;
            // } TODO end 

            this.lastHit.position = _other.transform.position;
            this.lastHit.normal = _other.transform.right;
            Vector3 dir = _other.transform.position - transform.position;
            dir.y = 0.0f;
            dir.Normalize();
            this.lastHit.hitBackForce = dir * DamageRule.Instance().HitBackForce(dmgInfo.hitBackType);  

            // TODO: if hit light, face it { 
            // transform.forward = -_other.transform.forward;
            // } TODO end 
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void ActOnHit () {
        string animName = DamageRule.Instance().HitAnim(this.lastHit.hitType);
        if ( animName == "unknown" )
            return;

        this.anim.Rewind(animName);
        this.anim.Play(animName);
    } 

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void ActOnDead () {
        Vector3 hitPos = this.lastHit.position;
        hitPos.y = 0.0f;
        if ( this.IsBehind(hitPos) )
            this.anim.CrossFade("death1");
        else
            this.anim.CrossFade("death2");
    } 

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public bool isOnHit () {
        return this.lastHit.hitType != HitInfo.HitType.none;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public bool arriveDestination () {
        return (this.targetPos - transform.position).magnitude < 0.1f;
    }
}
