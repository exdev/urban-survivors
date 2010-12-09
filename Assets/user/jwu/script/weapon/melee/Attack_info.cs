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
    [System.NonSerialized] public Combo_info next = null;
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
    [System.NonSerialized] public Combo_info combo_entry = null;
    [System.NonSerialized] public Combo_info curCombo = null;

    ///////////////////////////////////////////////////////////////////////////////
    // function defines
    ///////////////////////////////////////////////////////////////////////////////

	// Use this for initialization
	void Start () {
        int combo_count = this.combo_list.Length;
        for ( int i = 0; i < combo_count; ++i ) {
            Combo_info combo = this.combo_list[i];
            DebugHelper.Assert(combo.endTime!=-1.0f, "endTime can't be minus!");
            DebugHelper.Assert(combo.animName!="unknown", "animName can't be unknown!");
            DebugHelper.Assert(combo.validInputTime.x < combo.validInputTime.y, "input time is invalid!");

            int next = i+1;
            if ( next != combo_count )
                combo.next = this.combo_list[next];
        }
        this.combo_entry = this.combo_list[0];
        this.combo_list = null;
	}
}
