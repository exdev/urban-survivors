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
        Animation anim = null;
        public Action_Attack ( Animation _anim ) {
            this.anim = _anim;
        }

        public override void exec () {
            this.anim.Rewind("attack_copy");
            this.anim.CrossFade("attack_copy");
        }
    }

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    // HACK { 
    public GameObject atkShape;
    public Transform atkAttachedBone;
    // } HACK end 

    ///////////////////////////////////////////////////////////////////////////////
    // defines
    ///////////////////////////////////////////////////////////////////////////////

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
                                                new Action_Attack(this.anim), 
                                                null,
                                                null );

        // ======================================================== 
        // setup transitions
        // ======================================================== 

        // seek to ...
        state_seekPlayers.AddTransition( new FSM.Transition( state_attack,
                                                             cond_isPlayerInAttackRange,
                                                             null ) );
        // attack to ...
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

        // HACK { 
        DebugHelper.Assert(this.atkShape, "attack shape not assigned");
        DebugHelper.Assert(this.atkAttachedBone, "attack attached bone not assigned");
        this.atkShape.transform.parent = atkAttachedBone;
        this.atkShape.active = false;
        // } HACK end 

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

    void Update () {
        this.fsm.tick(); // update state machine
        ProcessMovement();

        // TODO { 
        // // material
        // Renderer r = transform.Find("zombieGirl").renderer;
        // r.material.color = new Color ( 1.0f, HP/100.0f, HP/100.0f );
        // if ( HP <= 0.0f ) {
        //     Destroy(gameObject);
        // }
        // } TODO end 

        // DEBUG { 
        // // draw velocity
        // Vector3 vel = base.Velocity(); 
        // DebugHelper.DrawLine ( transform.position, 
        //                        transform.position + vel * 3.0f, 
        //                        new Color(0.0f,1.0f,0.0f) );
        // // draw smoothed acceleration
        // Vector3 acc = base.smoothedAcceleration;
        // DebugHelper.DrawLine ( transform.position, 
        //                        transform.position + acc * 3.0f, 
        //                        new Color(1.0f,0.0f,1.0f) );
        // // draw target pos
        // DebugHelper.DrawDestination ( this.targetPos );
        // DebugHelper.DrawCircleY( transform.position, 5.0f, Color.yellow );

        // // debug info
        // DebugHelper.ScreenPrint ( "AI_ZombieGirl steering state: " + this.steeringState );
        // DebugHelper.ScreenPrint ( "AI_ZombieGirl current state: " + fsm.CurrentState().name );
        // } DEBUG end 
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
            ApplyBrakingForce(10.0f);
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
	
}


