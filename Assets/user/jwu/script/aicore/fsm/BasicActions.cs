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
        anim_comp = _anim;
        anim_name = _name;
    }

    public override void exec () {
        anim_comp.CrossFade(anim_name);
    }
}
