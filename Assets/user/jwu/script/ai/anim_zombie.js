// ======================================================================================
// File         : anim_zombie.js
// Author       : Wu Jie 
// Last Change  : 09/20/2010 | 11:48:25 AM | Monday,September
// Description  : 
// ======================================================================================

#pragma strict 

///////////////////////////////////////////////////////////////////////////////
// members
///////////////////////////////////////////////////////////////////////////////

private var ai : ai_generic; 
private var anim : Animation; 

///////////////////////////////////////////////////////////////////////////////
// function defines
///////////////////////////////////////////////////////////////////////////////

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

protected function InitAnim () {
    ai = GetComponent(ai_generic);

    var state:AnimationState;
    anim = transform.GetComponent(Animation);

    state = anim["walk"];
    state.layer = 1;
    state.wrapMode = WrapMode.Loop;
    state.weight = 1.0;
    state.blendMode = AnimationBlendMode.Blend;
    state.enabled = true;

    state = anim["attack"];
    state.wrapMode = WrapMode.Loop;
    state.layer = 1;
    state.weight = 1.0;
    state.normalizedSpeed = 1.0;
}

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

protected function UpdateAnim () {
    if ( ai.state_move ) {
        anim.CrossFade("walk");
    }
    if ( ai.state_attack ) {
        anim.CrossFade("attack");
    }
}

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

protected function PostAnim () {
}

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

function Start () {
    InitAnim();
}

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

function Update () {
    UpdateAnim();
}

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

function LateUpdate () {
    PostAnim();
}
