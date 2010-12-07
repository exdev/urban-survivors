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

///////////////////////////////////////////////////////////////////////////////
// actions, transitions
///////////////////////////////////////////////////////////////////////////////

// ------------------------------------------------------------------ 
// Desc: Action_MoveToNearestPlayer 
// ------------------------------------------------------------------ 

class Action_MoveToNearestPlayer : FSM.Action_periodic {
    public Action_MoveToNearestPlayer ( float _interval ) {
        base.Interval = _interval;
    }

    public override void exec ( GameObject _self ) {
        Vector3 targetPos = Vector3.zero;
        GameObject[] players = GameRules.Instance().GetPlayers();
        float nearest = 999.0f;
        foreach( GameObject player in players ) {
            float len = (player.transform.position - _self.transform.position).magnitude;
            if ( len < nearest ) {
                nearest = len;
                targetPos = player.transform.position;
            }
        }

        AI_ZombieNormal zombieNormal = _self.GetComponent<AI_ZombieNormal>();
        zombieNormal.Seek(targetPos);
    }
}

// ------------------------------------------------------------------ 
// Desc: Action_Attack 
// ------------------------------------------------------------------ 

class Action_Attack : FSM.Action_periodic {
    public Action_Attack ( float _interval ) {
        base.Interval = _interval;
    }

    public override void exec ( GameObject _self ) {
        Animation anim_comp = _self.GetComponent<Animation>();
        anim_comp.CrossFade("attack1");
    }
}

// ------------------------------------------------------------------ 
// Desc: Action_StopMoving 
// ------------------------------------------------------------------ 

class Action_StopMoving : FSM.Action {
    public Action_StopMoving () {
    }

    public override void exec ( GameObject _self ) {
        AI_ZombieNormal zombieNormal = _self.GetComponent<AI_ZombieNormal>();
        zombieNormal.Stop();
    }
}

// ------------------------------------------------------------------ 
// Desc: Transition_Idle_to_Seek 
// ------------------------------------------------------------------ 

class Transition_Idle_to_Seek : FSM.Transition {
    float range = 5.0f;

    public Transition_Idle_to_Seek ( FSM.State _dest, float _range ) {
        base.dest_state = _dest;
        range = _range;
    }

    public override bool check ( GameObject _self ) { 
        GameObject[] players = GameRules.Instance().GetPlayers();
        foreach( GameObject player in players ) {
            float len = (player.transform.position - _self.transform.position).magnitude;
            if ( len < range ) {
                return true;
            }
        }
        return false; 
    } 
}

// ------------------------------------------------------------------ 
// Desc: Transition_Seek_to_Idle 
// ------------------------------------------------------------------ 

class Transition_Seek_to_Idle : FSM.Transition {
    float range = 10.0f;

    public Transition_Seek_to_Idle ( FSM.State _dest, float _range ) {
        base.dest_state = _dest;
        range = _range;
    }

    public override bool check ( GameObject _self ) { 
        GameObject[] players = GameRules.Instance().GetPlayers();
        foreach( GameObject player in players ) {
            float len = (player.transform.position - _self.transform.position).magnitude;
            if ( len < range ) {
                return false;
            }
        }
        return true; 
    } 
}

// ------------------------------------------------------------------ 
// Desc: Transition_Seek_to_Attack 
// ------------------------------------------------------------------ 

class Transition_Seek_to_Attack : FSM.Transition {
    float range = 1.0f;

    public Transition_Seek_to_Attack ( FSM.State _dest, float _range ) {
        base.dest_state = _dest;
        range = _range;
    }

    public override bool check ( GameObject _self ) { 
        GameObject targetGO = null;
        GameObject[] players = GameRules.Instance().GetPlayers();
        float nearest = 999.0f;
        foreach( GameObject player in players ) {
            float len = (player.transform.position - _self.transform.position).magnitude;
            if ( len < nearest ) {
                nearest = len;
                targetGO = player;
            }
        }
        if ( nearest > range ) 
            return false;

        // if we near target, check if we face it.
        AI_ZombieNormal zombieNormal = _self.GetComponent<AI_ZombieNormal>();
        bool result = zombieNormal.IsAhead( targetGO.transform.position, Mathf.Cos(30.0f*Mathf.Deg2Rad) );
        return result;
    } 
}

// ------------------------------------------------------------------ 
// Desc: Transition_Attack_to_Seek
// ------------------------------------------------------------------ 

class Transition_Attack_to_Seek : FSM.Transition {
    float range = 1.0f;

    public Transition_Attack_to_Seek ( FSM.State _dest, float _range ) {
        base.dest_state = _dest;
        range = _range;
    }

    public override bool check ( GameObject _self ) { 
        // TODO: add end event for attacking { 
        // we still attacking
        // Animation anim_comp = _self.GetComponent<Animation>();
        // if ( anim_comp.IsPlaying("attack1") )
        //     return false;
        // } TODO end 

        GameObject targetGO = null;
        GameObject[] players = GameRules.Instance().GetPlayers();
        float nearest = 999.0f;
        foreach( GameObject player in players ) {
            float len = (player.transform.position - _self.transform.position).magnitude;
            if ( len < nearest ) {
                nearest = len;
                targetGO = player;
            }
        }

        // if we don't have any (range) near player
        if ( nearest > range ) 
            return true;

        // if we got near target, check if we face it.
        AI_ZombieNormal zombieNormal = _self.GetComponent<AI_ZombieNormal>();
        bool result = zombieNormal.IsAhead( targetGO.transform.position, Mathf.Cos(30.0f*Mathf.Deg2Rad) );
        return !result;
    } 
}

///////////////////////////////////////////////////////////////////////////////
// class AI_ZombieNormal
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

public class AI_ZombieNormal : Steer {
    public enum SteeringState {
        seeking,
        braking,
    };

    protected Animation anim;
    protected FSM fsm = new FSM();
    protected Vector3 targetPos;
    protected SteeringState steeringState = SteeringState.braking;

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void Seek ( Vector3 _pos ) {
        targetPos = _pos;
        steeringState = SteeringState.seeking;
    }

    public void Stop () {
        steeringState = SteeringState.braking;
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
        // init states
        FSM.State State_Idle = new FSM.State( "Idle", 
                                              new Action_PlayAnim(anim,"idle1"), 
                                              null, 
                                              null );
        FSM.State State_SeekPlayers = new FSM.State( "SeekPlayers", 
                                                     new Action_PlayAnim(anim,"moveForward"), 
                                                     new Action_MoveToNearestPlayer(1.0f), 
                                                     new Action_StopMoving() );
        FSM.State State_Attack = new FSM.State( "Attack", 
                                                null, 
                                                new Action_Attack(0.5f), 
                                                null );

        // connect idle transitions
        {
            Transition_Idle_to_Seek trans1 = new Transition_Idle_to_Seek( State_SeekPlayers, 5.0f);
            State_Idle.AddTransition(trans1);
        }

        // connect move towards players transitions
        {
            Transition_Seek_to_Idle trans1 = new Transition_Seek_to_Idle( State_Idle, 7.0f);
            Transition_Seek_to_Attack trans2 = new Transition_Seek_to_Attack( State_Attack, 2.0f);

            State_SeekPlayers.AddTransition(trans1);
            State_SeekPlayers.AddTransition(trans2);
        }

        // connect attack transitions
        {
            Transition_Attack_to_Seek trans1 = new Transition_Attack_to_Seek( State_SeekPlayers, 2.0f );
            State_Attack.AddTransition(trans1);
        }

        // init fsm
        fsm.init(gameObject,State_Idle);
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
        // update state machine
        fsm.tick();

        // handle steering
        Vector3 force = Vector3.zero;
        if ( this.steeringState == SteeringState.seeking ) {
            float distance = (transform.position - this.targetPos).magnitude;
            if ( distance < 2.0f ) {
                ApplyBrakingForce(10.0f);

                // face the target
                float rot_speed = 2.0f; // TEMP
                Vector3 dir = targetPos - transform.position;
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
        // } DEBUG end 
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnCollisionEnter ( Collision _other ) {
        transform.forward = -_other.transform.forward;
        anim.Rewind("hit2");
        anim.CrossFade("hit2");
    }
}
