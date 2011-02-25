// ======================================================================================
// File         : AttackInfo.cs
// Author       : Wu Jie 
// Last Change  : 12/13/2010 | 15:01:30 PM | Monday,December
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
// Desc: ComboInfo 
// ------------------------------------------------------------------ 

[System.Serializable]
public class ComboInfo {
    public string animName = "unknown";
    public Vector2 validInputTime = new Vector2(0.0f,1.0f);
    public float endTime = -1.0f; 
    public bool canCharge = false;
    public GameObject attack_shape = null;
    public AudioSource sfx_wipe = null;
    [System.NonSerialized] public ComboInfo next = null;
    // TODO: public collision info 
}

// ------------------------------------------------------------------ 
// Desc: AttackInfo
// ------------------------------------------------------------------ 

public class AttackInfo : MonoBehaviour {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public ComboInfo[] comboList;
    public float speed = 2.0f;
    [System.NonSerialized] public ComboInfo combo_entry = null;
    [System.NonSerialized] public ComboInfo curCombo = null;
    [System.NonSerialized] public bool waitForNextCombo = false;

    ///////////////////////////////////////////////////////////////////////////////
    // function defines
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: Use this for initialization
    // ------------------------------------------------------------------ 

	void Start () {
        int combo_count = this.comboList.Length;
        for ( int i = 0; i < combo_count; ++i ) {
            ComboInfo combo = this.comboList[i];
            DebugHelper.Assert(combo.endTime!=-1.0f, "endTime can't be minus!");
            DebugHelper.Assert(combo.animName!="unknown", "animName can't be unknown!");
            DebugHelper.Assert(combo.validInputTime.x < combo.validInputTime.y, "input time is invalid!");
            DebugHelper.Assert(combo.attack_shape, "please define attack shape!");

            combo.attack_shape.active = false;
            int next = i+1;
            if ( next != combo_count )
                combo.next = this.comboList[next];
        }
        this.combo_entry = this.comboList[0];
        this.comboList = null;
	}

    // TODO: we should use SweepTestAll instead { 
    // ------------------------------------------------------------------ 
    // Desc: NOTE: we have to update physics by ourself. 
    // ------------------------------------------------------------------ 

    void FixedUpdate () {
        if ( this.curCombo != null ) {
            Rigidbody rb = this.curCombo.attack_shape.GetComponent<Rigidbody>();
            rb.MovePosition(this.transform.position);
        }
    }
    // } TODO end 
}
