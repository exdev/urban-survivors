// ======================================================================================
// File         : Player_base.cs
// Author       : Wu Jie 
// Last Change  : 10/12/2010 | 00:45:53 AM | Tuesday,October
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////
// class Player_base
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

public class Player_base : MonoBehaviour {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public float maxSpeed = 50.0f;
    public float maxHP = 100.0f;

    protected ScreenPad screenPad;
    protected float curHP = 60.0f;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public float GetHP () { return curHP/maxHP; }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	protected void Start () {
        screenPad = GameObject.Find("HUD").GetComponent(typeof(ScreenPad)) as ScreenPad;
        DebugHelper.Assert( screenPad, "screenPad not found" );
        // curHP = maxHP;
    }
}

