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
// defines
///////////////////////////////////////////////////////////////////////////////

// ------------------------------------------------------------------ 
// Desc: Action_MoveToNearestPlayer 
// ------------------------------------------------------------------ 

class Action_MoveToNearestPlayer : FSM.Action_periodic {
    public GameObject self = null;

    public Action_MoveToNearestPlayer ( GameObject _self, float _interval ) {
        self = _self;
        base.Interval = _interval;
    }

    public override void exec () {
        Vector3 targetPos = Vector3.zero;
        GameObject[] players = GameRules.Instance().GetPlayers();
        float nearest = 999.0f;
        foreach( GameObject player in players ) {
            float len = (player.transform.position - self.transform.position).magnitude;
            if ( len < nearest ) {
                nearest = len;
                targetPos = player.transform.position;
            }
        }

        AI_ZombieNormal zombieNormal = self.GetComponent<AI_ZombieNormal>();
        zombieNormal.Seek(targetPos);
    }
}

// ------------------------------------------------------------------ 
// Desc: Transition_Idle_to_Move 
// ------------------------------------------------------------------ 

class Transition_Idle_to_Move : FSM.Transition {
    public GameObject self = null;
    float range = 5.0f;

    public Transition_Idle_to_Move ( FSM.State _dest, GameObject _self, float _range ) {
        base.dest_state = _dest;
        self = _self;
        range = _range;
    }

    public override bool check () { 
        GameObject[] players = GameRules.Instance().GetPlayers();
        foreach( GameObject player in players ) {
            float len = (player.transform.position - self.transform.position).magnitude;
            if ( len < range ) {
                return true;
            }
        }
        return false; 
    } 
}

///////////////////////////////////////////////////////////////////////////////
// class AI_ZombieNormal
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

[RequireComponent(typeof(CharacterController))]
public class AI_ZombieNormal : MonoBehaviour {

    protected Animation anim;
    protected CharacterController controller = null;
    protected FSM fsm = new FSM();

    protected Vector3 targetPos;

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void Seek ( Vector3 _pos ) {
        targetPos = _pos;
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
        FSM.State State_Idle = new FSM.State( "Idle", new Action_PlayAnim(anim,"idle1"), null, null );
        FSM.State State_MoveTowardsPlayer = new FSM.State( "MoveTowardsPlayer", 
                                                           new Action_PlayAnim(anim,"moveForward"), 
                                                           new Action_MoveToNearestPlayer(gameObject,1.0f), 
                                                           null );

        // connect transitions
        Transition_Idle_to_Move trans = new Transition_Idle_to_Move( State_MoveTowardsPlayer, gameObject, 5.0f);
        State_Idle.AddTransition(trans);

        // init fsm
        fsm.init(State_Idle);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Start () {
        controller = GetComponent<CharacterController>();
        targetPos = transform.position;

        InitAnim();
        InitFSM();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Update () {
        // DEBUG { 
        DebugHelper.DrawCircleY( transform.position, 5.0f, Color.yellow );
        // } DEBUG end 

        fsm.tick();

        //
        float move_speed = 1.0f;
        float rot_speed = 0.5f;

        // 
        // DebugHelper.ScreenPrint("targetPos: " + targetPos);
        Vector3 delta = targetPos - transform.position;
        if ( delta.magnitude >= 1.5f ) {
            // apply gravity
            Vector3 vel = transform.forward * move_speed;
            if ( controller.isGrounded == false ) {
                vel.y = -10.0f;
            }
            controller.Move(vel * Time.deltaTime);

            // rotate ai
            Quaternion wanted_rot = Quaternion.LookRotation(delta);
            wanted_rot.x = 0.0f; wanted_rot.z = 0.0f;
            transform.rotation = Quaternion.Slerp ( 
                                                   transform.rotation, 
                                                   wanted_rot, 
                                                   rot_speed * Time.deltaTime
                                                  );
        }
    }
}
