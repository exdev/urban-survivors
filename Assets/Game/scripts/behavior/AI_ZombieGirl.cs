// ======================================================================================
// File         : AI_ZombieGirl.cs
// Author       : Wu Jie 
// Last Change  : 12/14/2010 | 17:52:30 PM | Tuesday,December
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

public class AI_ZombieGirl : AI_ZombieBase {

    ///////////////////////////////////////////////////////////////////////////////
    // actions, conditions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: Condition_isAttacking 
    // ------------------------------------------------------------------ 

    class Condition_isAttacking : FSM.Condition {
        Actor actor = null;

        public Condition_isAttacking ( Actor _actor ) {
            this.actor = _actor;
        }

        public override bool exec () {
            return this.actor.IsPlayingAnim("attack_copy");
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: Action_Attack 
    // ------------------------------------------------------------------ 

    class Action_Attack : FSM.Action {
        AI_ZombieGirl zombieGirl = null;
        public Action_Attack ( AI_ZombieGirl _zombieGirl ) {
            this.zombieGirl = _zombieGirl;
        }

        public override void exec () {
            this.zombieGirl.ActAttack();
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    class Action_ActOnDead : FSM.Action {
        AI_ZombieGirl zombieGirl = null;

        public Action_ActOnDead ( AI_ZombieGirl _zombieGirl ) {
            this.zombieGirl = _zombieGirl;
        }

        public override void exec () {
            this.zombieGirl.ActOnDead();
        }
    }

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    protected static GameObject fxDead = null;

    public GameObject atkShape;
    public GameObject FX_dead = null;

    ///////////////////////////////////////////////////////////////////////////////
    // function defines
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Awake () {
        if ( fxDead  == null && this.FX_dead ) {
            fxDead = (GameObject)Instantiate( this.FX_dead );
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected void InitAnim () {
        AnimationState state;

        state = this.anim["walk"];
        state.normalizedSpeed = this.StepSpeed;
        state.layer = 0;
        state.wrapMode = WrapMode.Loop;
        state.weight = 1.0f;
        state.enabled = false;

        state = this.anim["attack_copy"];
        state.normalizedSpeed = 1.2f;
        state.layer = 0;
        state.wrapMode = WrapMode.Once;
        state.weight = 1.0f;
        state.enabled = false;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void InitFSM () {

        FSM.Condition cond_isPlayerInAttackRange = new Condition_isPlayerInAttackRange(this,60.0f,2.0f);
        FSM.Condition cond_isAttacking = new Condition_isAttacking(this);
        FSM.Condition cond_noHP = new Condition_noHP(this.zombieInfo);

        // ======================================================== 
        // setup states
        // ======================================================== 

        // seekPlayers
        FSM.State state_seekPlayers = new FSM.State( "SeekPlayers", 
                                                     new Action_PlayAnim(this.anim,"walk"), 
                                                     new Action_MoveToNearestAlivedPlayer(0.01f,this), 
                                                     new Action_StopMoving(this) );
        // attack
        FSM.State state_attack = new FSM.State( "Attack", 
                                                new Action_Attack(this), 
                                                null,
                                                null );
        // dead
        FSM.State state_dead = new FSM.State( "Dead",
                                              new Action_ActOnDead(this), 
                                              null,
                                              null );

        // ======================================================== 
        // setup transitions
        // ======================================================== 

        // seek to ...
        state_seekPlayers.AddTransition( new FSM.Transition( state_dead,
                                                             cond_noHP,
                                                             null ) );
        state_seekPlayers.AddTransition( new FSM.Transition( state_attack,
                                                             cond_isPlayerInAttackRange,
                                                             null ) );
        // attack to ...
        state_attack.AddTransition( new FSM.Transition( state_dead,
                                                        cond_noHP,
                                                        null ) );
        state_attack.AddTransition( new FSM.Transition( state_seekPlayers,
                                                        new FSM.Condition_not(cond_isAttacking),
                                                        null ) );

        //
        fsm.init(state_seekPlayers);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected new void Start () {
        base.Start();

        this.InitAnim();
        this.InitFSM();

        // HARDCODE { 
        DebugHelper.Assert(this.atkShape, "attack shape not assigned");
        // DebugHelper.Assert(this.atkAttachedBone, "attack attached bone not assigned");
        // this.atkShape.transform.parent = atkAttachedBone;
        this.atkShape.active = false;
        DamageInfo dmgInfo = this.atkShape.GetComponent<DamageInfo>();
        dmgInfo.owner_info = this.zombieInfo;
        dmgInfo.owner = this.gameObject;
        // } HARDCODE end 

        // KEEPME { 
        // StartCoroutine(GetRandomDest(2.0));
        // StartCoroutine(GetNearestPlayerPos(2.0f));
        // } KEEPME end 
    }

    // KEEPME { 
    // // ------------------------------------------------------------------ 
    // // Desc: 
    // // ------------------------------------------------------------------ 

    // IEnumerator GetNearestPlayerPos ( float _tickTime ) {
    //     while ( true ) {
    //         GameObject[] players = GameRules.Instance().GetPlayers();
    //         float nearest = 999.0f;
    //         foreach( GameObject player in players ) {
    //             float len = (player.transform.position - transform.position).magnitude;
    //             if ( len < nearest ) {
    //                 nearest = len;
    //                 wanted_pos = player.transform.position;
    //                 target = player.transform;
    //             }
    //         }
    //         yield return new WaitForSeconds (_tickTime);
    //     }
    // }

    // // ------------------------------------------------------------------ 
    // // Desc: 
    // // ------------------------------------------------------------------ 

    // IEnumerator GetRandomDest( float _tickTime ) {
    //     while ( true ) {
    //         Vector3 delta = wanted_pos - transform.position;
    //         if ( delta.magnitude < 0.01f ) {
    //             wanted_pos = new Vector3( 
    //                                  Random.Range(-10.0f,10.0f), 
    //                                  0.0f,
    //                                  Random.Range(-10.0f,10.0f) 
    //                                 );
    //         }
    //         else {
    //             wanted_pos = new Vector3( 
    //                                  Random.Range(-10.0f,10.0f), 
    //                                  0.0f,
    //                                  Random.Range(-10.0f,10.0f) 
    //                                 );
    //             yield return new WaitForSeconds (_tickTime);
    //         }
    //     }
    // }
    // } KEEPME end 

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected new void Update () {
        base.Update();

        this.fsm.tick(); // update state machine
        ProcessMovement();

        // ShowDebugInfo (); // DEBUG
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void ActAttack () {
        this.anim.Rewind("attack_copy");
        this.anim.CrossFade("attack_copy");
        this.transform.forward = targetPos - this.transform.position;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void ActOnDead () {
        fxDead.transform.position = this.transform.position;
        fxDead.transform.rotation = this.transform.rotation;
        fxDead.particleEmitter.Emit();
        GameObject.Destroy(this.gameObject);
    } 

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void ProcessMovement () {
        // handle steering
        Vector3 force = Vector3.zero;
        if ( this.steeringState == SteeringState.seeking ) {
            force = GetSteering_Seek_MaxForces ( this.targetPos );
            force.y = 0.0f;
        }
        else if ( this.steeringState == SteeringState.braking ) {
            ApplyBrakingForce();
        }
        ApplySteeringForce(force);
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

        DamageRule.Instance().CalculateDamage( this.zombieInfo, dmgInfo );
    }
}


