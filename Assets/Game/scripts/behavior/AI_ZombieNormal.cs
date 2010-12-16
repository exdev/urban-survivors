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

    public ActorInfo zombie_info = new ActorInfo();
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

        FSM.Condition cond_isPlayerInAttackRange = new Condition_isPlayerInAttackRange(this,30.0f,this.attackDistance);
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

        // HARDCODE { 
        DebugHelper.Assert(this.atkShape, "attack shape not assigned");
        this.atkShape.active = false;
        DamageInfo dmgInfo = this.atkShape.GetComponent<DamageInfo>();
        dmgInfo.owner_info = this.zombie_info;
        // } HARDCODE end 

        this.InitAnim();
        this.InitFSM();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Update () {
        this.fsm.tick(); // update state machine
        ProcessMovement();

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

    void ProcessMovement () {
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

        /*float dmgOutput =*/ DamageRule.Instance().CalculateDamage( this.zombie_info, dmgInfo );

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
