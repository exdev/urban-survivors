// ======================================================================================
// File         : BasicActions.cs
// Author       : Wu Jie 
// Last Change  : 11/21/2010 | 21:40:56 PM | Sunday,November
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

///////////////////////////////////////////////////////////////////////////////
// Actions
///////////////////////////////////////////////////////////////////////////////

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

class Action_PlayAnim : FSM.Action {
    public Animation anim_comp;
    public string anim_name;

    public Action_PlayAnim ( Animation _anim, string _name ) {
        this.anim_comp = _anim;
        this.anim_name = _name;
    }

    public override void exec () {
        this.anim_comp.CrossFade(anim_name);
    }
}

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

class Action_RandomAnim : FSM.Action_periodic {
    public Animation anim_comp;
    public string[] anim_names;

    public Action_RandomAnim ( float _delay, 
                               float _interval, 
                               Animation _anim, 
                               string[] _names ) 
        : base (_delay, _interval )
    {
        this.anim_comp = _anim;
        this.anim_names = _names;
    }

    public override void exec () {
        uint idx = (uint)Random.Range( 0, anim_names.Length );
        this.anim_comp.CrossFade(anim_names[idx]);
    }
}
