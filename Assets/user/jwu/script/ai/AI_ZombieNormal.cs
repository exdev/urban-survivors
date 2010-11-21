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

public class AI_ZombieNormal : MonoBehaviour {

    protected Animation anim;
    protected CharacterController controller = null;
    protected FSM fsm = new FSM();

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
        FSM.State State_Idle = new FSM.State( "Idle", new Action_PlayAnim(anim,"idle1"), null, null );
        FSM.State State_MoveTowardsPlayer = new FSM.State( "MoveTowardsPlayer", new Action_PlayAnim(anim,"moveForward"), null, null );

        fsm.init(State_Idle);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Start () {
        controller = GetComponent<CharacterController>();
        InitAnim();
        InitFSM();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Update () {
        fsm.tick();
    }
}
