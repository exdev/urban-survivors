// ======================================================================================
// File         : Attack_info.cs
// Author       : Wu Jie 
// Last Change  : 12/08/2010 | 21:13:45 PM | Wednesday,December
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

///////////////////////////////////////////////////////////////////////////////
// defines
///////////////////////////////////////////////////////////////////////////////

// ------------------------------------------------------------------ 
// Desc: Combo_info 
// ------------------------------------------------------------------ 

[System.Serializable]
public class Combo_info {
    public string animName = "unknown";
    public Vector2 validInputTime = new Vector2(0.0f,1.0f);
    public float endTime = -1.0f; 
    public bool canCharge = false;
    // TODO: public collision info 
}

// ------------------------------------------------------------------ 
// Desc: Attack_info
// ------------------------------------------------------------------ 

public class Attack_info : MonoBehaviour {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public Combo_info[] combo_list;
    protected Actor_info owner_info = null;

    ///////////////////////////////////////////////////////////////////////////////
    // function defines
    ///////////////////////////////////////////////////////////////////////////////

    public void setOwnerInfo ( Actor_info _info ) {
        this.owner_info = _info;
    }

	// Use this for initialization
	void Start () {
        foreach ( Combo_info combo in this.combo_list ) {
            DebugHelper.Assert(combo.endTime!=-1.0f, "endTime can't be minus!");
            DebugHelper.Assert(combo.animName!="unknown", "animName can't be unknown!");
            DebugHelper.Assert(combo.validInputTime.x < combo.validInputTime.y, "input time is invalid!");
        }
	}
}
