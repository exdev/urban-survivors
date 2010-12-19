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

public class AI_ZombieNormal : AI_ZombieBase {

    ///////////////////////////////////////////////////////////////////////////////
    // actions, conditions
    ///////////////////////////////////////////////////////////////////////////////

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
    // Desc: Action_ActGetStun 
    // ------------------------------------------------------------------ 

    class Action_ActOnStun : FSM.Action {
        AI_ZombieNormal zombieNormal = null;

        public Action_ActOnStun ( AI_ZombieNormal _zombieNormal ) {
            this.zombieNormal = _zombieNormal;
        }

        public override void exec () {
            this.zombieNormal.ActOnStun();
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

    class Condition_isOnStun : FSM.Condition {
        AI_ZombieNormal zombieNormal = null;

        public Condition_isOnStun ( AI_ZombieNormal _zombieNormal ) {
            this.zombieNormal = _zombieNormal;
        }

        public override bool exec () {
            return this.zombieNormal.isGetStun();
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    class Condition_isStunning : FSM.Condition {
        AI_ZombieNormal zombieNormal = null;

        public Condition_isStunning ( AI_ZombieNormal _zombieNormal ) {
            this.zombieNormal = _zombieNormal;
        }

        public override bool exec () {
            return this.zombieNormal.IsPlayingAnim("hit1") ||
                this.zombieNormal.IsPlayingAnim("hit2");
        }
    }

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public float attackDistance = 1.5f;
    public GameObject atkShape = null;
    protected HitInfo lastHit = new HitInfo();

    ///////////////////////////////////////////////////////////////////////////////
    // function defines
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected void InitAnim () {
        AnimationState state;

        string[] anim_keys0 = { 
            "moveForward", 
            "idle1", 
            "idle2" 
        };
        foreach (string key in anim_keys0) {
            state = this.anim[key];
            state.layer = 0;
            state.wrapMode = WrapMode.Loop;
            state.weight = 1.0f;
            state.enabled = false;
        }

        string[] anim_keys1 = { 
            "attack1", 
            "hit1", 
            "hit2" 
        };
        foreach (string key in anim_keys1) {
            state = this.anim[key];
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
                                                     new Action_MoveToNearestAlivedPlayer(0.5f,this), 
                                                     new Action_StopMoving(this) );
        // attack
        FSM.State state_attack = new FSM.State( "Attack", 
                                                new Action_Attack(this.anim), 
                                                null,
                                                null );
        // get hit
        FSM.State state_onStun = new FSM.State( "OnStun", 
                                               new Action_ActOnStun(this), 
                                               null,
                                               null );
        // on dead
        FSM.State state_onDead = new FSM.State( "OnDead",
                                                new Action_ActOnDead(this), 
                                                null, // TODO: dead clean up
                                                null );
        // dead
        FSM.State state_dead = new FSM.State( "Dead",
                                              new Action_CleanDeadBody(this.gameObject), 
                                              null,
                                              null );

        // ======================================================== 
        // condition 
        // ======================================================== 

        FSM.Condition cond_isPlayerInAttackRange = new Condition_isPlayerInAttackRange(this,30.0f,this.attackDistance);
        FSM.Condition cond_isAttacking = new Condition_isAttacking(this);
        FSM.Condition cond_isOnStun = new Condition_isOnStun(this);
        FSM.Condition cond_noHP = new Condition_noHP(this.zombieInfo);
        // TODO: FSM.Condition cond_isStunning = new Condition_isStunning(this);

        // ======================================================== 
        // setup transitions
        // ======================================================== 

        // idle to ...
        state_idle.AddTransition( new FSM.Transition( state_onDead,
                                                      cond_noHP,
                                                      null ) );
        state_idle.AddTransition( new FSM.Transition( state_onStun,
                                                      cond_isOnStun,
                                                      null ) );
        state_idle.AddTransition( new FSM.Transition( state_seekPlayers, 
                                                      new Condition_playerInRange( this.transform, 20.0f ), 
                                                      null ) );

        // seekPlayers to ...
        state_seekPlayers.AddTransition( new FSM.Transition( state_onDead,
                                                             cond_noHP,
                                                             null ) );
        state_seekPlayers.AddTransition( new FSM.Transition( state_onStun,
                                                             cond_isOnStun,
                                                             null ) );
        state_seekPlayers.AddTransition( new FSM.Transition( state_attack,
                                                             cond_isPlayerInAttackRange,
                                                             null ) );
        state_seekPlayers.AddTransition( new FSM.Transition( state_idle, 
                                                             new FSM.Condition_not( new Condition_playerInRange( this.transform, 40.0f ) ) , 
                                                             null ) );
        // attack to ...
        state_attack.AddTransition( new FSM.Transition( state_onDead,
                                                        cond_noHP,
                                                        null ) );
        state_attack.AddTransition( new FSM.Transition( state_onStun,
                                                        cond_isOnStun,
                                                        null ) );
        state_attack.AddTransition( new FSM.Transition ( state_idle, 
                                                         new FSM.Condition_not(cond_isAttacking),
                                                         null ) );
        // on hit to ...
        state_onStun.AddTransition( new FSM.Transition( state_onDead,
                                                       cond_noHP,
                                                       null ) );
        state_onStun.AddTransition( new FSM.Transition ( state_idle, 
                                                        new FSM.Condition_not(new Condition_isStunning(this) ),
                                                        null ) );
        state_onStun.AddTransition( new FSM.Transition ( state_onStun, 
                                                         cond_isOnStun,
                                                         null ) );

        // on dead to ...
        state_onDead.AddTransition( new FSM.Transition( state_dead,
                                                        new FSM.Condition_waitForSeconds(this.deadBodyKeepTime),
                                                        null ) );

        // init fsm
        fsm.init(state_idle);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected new void Start () {
        base.Start();

        // HARDCODE { 
        DebugHelper.Assert(this.atkShape, "attack shape not assigned");
        this.atkShape.active = false;
        DamageInfo dmgInfo = this.atkShape.GetComponent<DamageInfo>();
        dmgInfo.owner_info = this.zombieInfo;
        // } HARDCODE end 

        this.InitAnim();
        this.InitFSM();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected new void Update () {
        base.Update();

        this.fsm.tick(); // update state machine
        ProcessMovement();

        // reset values
        this.lastHit.stunType = HitInfo.StunType.none;

        // ShowDebugInfo (); // DEBUG
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected new void ShowDebugInfo () {
        base.ShowDebugInfo();

        // draw target pos
        DebugHelper.DrawDestination ( this.targetPos );
        DebugHelper.DrawCircleY( transform.position, 5.0f, Color.yellow );

        // debug attack range
        Vector3 targetDir = (this.targetPos - transform.position).normalized;
        float cosTheta = Vector3.Dot ( transform.forward, targetDir );
        DebugHelper.ScreenPrint ( "face to target angle: " + Mathf.Acos(cosTheta) * Mathf.Rad2Deg );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void ProcessMovement () {
        // don't do anything if we disable the steer
        if (  this.steeringState == SteeringState.disable ) {
            // TEMP HARDCODE { 
            if ( this.anim.IsPlaying("death1") == false && 
                 this.anim.IsPlaying("death2") == false )
            {
                this.transform.position =
                    new Vector3 ( this.transform.position.x,
                                  this.transform.position.y - Time.deltaTime * 0.3f,
                                  this.transform.position.z );
            }
            // } TEMP HARDCODE end 
            return;
        }

        // handle steering
        Vector3 force = Vector3.zero;
        if ( this.steeringState == SteeringState.seeking ) {
            force = GetSteering_Seek_LimitByMaxSpeed ( this.targetPos );
            force.y = 0.0f;
        }
        else if ( this.steeringState == SteeringState.braking ) {
            ApplyBrakingForce(10.0f);
        }
        ApplySteeringForce(force);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnTriggerEnter ( Collider _other ) {
        DamageInfo dmgInfo = null;
        if ( _other.gameObject.layer == Layer.melee_player ) {
            Transform parent = _other.transform.parent;
            DebugHelper.Assert( parent, "melee collider's parent is null" );
            if ( parent == null ) {
                return;
            }
            dmgInfo = parent.GetComponent<DamageInfo>();

            // show the melee hit effect
            if ( fxHitMelee != null ) {
                fxHitMelee.transform.position = _other.transform.position;
                fxHitMelee.transform.rotation = _other.transform.rotation;
                fxHitMelee.particleEmitter.Emit();
            }
        }
        else if ( _other.gameObject.layer == Layer.bullet_player ) {
            BulletInfo bulletInfo = _other.GetComponent<BulletInfo>();
            dmgInfo = bulletInfo.ownerDamageInfo;

            // show the bullet hit effect
            if ( fxHitBullet != null ) {
                fxHitBullet.transform.position = _other.transform.position;
                fxHitBullet.transform.rotation = _other.transform.rotation;
                fxHitBullet.particleEmitter.Emit();
            }
        }
        else {
            return;
        }

        // if we don't get damage info, just return
        DebugHelper.Assert( dmgInfo, "can't find damage info for given layer" );
        if ( dmgInfo == null ) {
            return;
        }

        float dmgOutput = DamageRule.Instance().CalculateDamage( this.zombieInfo, dmgInfo );

        // caculate accumulate damage
        if ( _other.gameObject.layer == Layer.melee_player ) {
            this.zombieInfo.accDmgNormal += dmgOutput;
            this.zombieInfo.accDmgSerious += dmgOutput;
        }
        else if ( _other.gameObject.layer == Layer.bullet_player ) {
            this.zombieInfo.accDmgNormal += dmgOutput;
        }

        // serious stun have higher priority
        if ( this.zombieInfo.accDmgSerious >= this.zombieInfo.seriousStun ) {
            this.lastHit.stunType = HitInfo.StunType.serious;
            this.zombieInfo.accDmgSerious = 0.0f;
        }
        else if ( this.zombieInfo.accDmgNormal >= this.zombieInfo.normalStun ) {
            this.lastHit.stunType = HitInfo.StunType.normal;
            this.zombieInfo.accDmgNormal = 0.0f;
        }
        else {
            this.lastHit.stunType = HitInfo.StunType.none;
        }

        this.lastHit.position = _other.transform.position;
        this.lastHit.normal = _other.transform.right;
        Vector3 dir = _other.transform.position - transform.position;
        dir.y = 0.0f;
        dir.Normalize();
        this.lastHit.knockBackForce = dir * DamageRule.Instance().KnockBackForce(dmgInfo.knockBackType);  

        // TODO: if hit light, face it { 
        // transform.forward = -_other.transform.forward;
        // } TODO end 
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void ActOnStun () {
        // NOTE: it could be possible we interupt to hit when zombie is attacking.
        this.atkShape.active = false;

        string animName = DamageRule.Instance().HitAnim(this.lastHit.stunType);
        if ( animName == "unknown" )
            return;

        // Debug.Log("animName: " + animName); // DEBUG
        this.anim.Rewind(animName);
        this.anim.Play(animName);
    } 

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void ActOnDead () {
        // DISABLE { 
        // Vector3 hitPos = this.lastHit.position;
        // hitPos.y = 0.0f;
        // if ( this.IsBehind(hitPos) )
        //     this.anim.CrossFade("death1");
        // else
        //     this.anim.CrossFade("death2");
        // } DISABLE end 
        float r = Random.Range(0.0f,1.0f);
        if ( r < 0.4f )
            this.anim.CrossFade("death1");
        else
            this.anim.CrossFade("death2");

        // make sure we disable all attack shapes
        this.atkShape.active = false;
        this.gameObject.layer = Layer.dead_body;
        this.DisableSteering();
    } 

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public bool isGetStun () {
        return this.lastHit.stunType != HitInfo.StunType.none;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public bool arriveDestination () {
        return (this.targetPos - transform.position).magnitude < 0.1f;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	void AttackOn (){
        this.atkShape.active = true;
	}
	
    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	void AttackOff (){
        this.atkShape.active = false;
	}
}
