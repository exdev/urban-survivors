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

    protected ScreenPad screenPad;
    protected float curHP = 60.0f;
    protected Vector3 faceInitPos;

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public float maxSpeed = 50.0f;
    public float maxHP = 100.0f;

    public GameObject ui_HP;
    public GameObject ui_face;

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

        UIProgressBar hpProgressBar = ui_HP.GetComponent(typeof(UIProgressBar)) as UIProgressBar;
        hpProgressBar.Value = this.GetHP();
        faceInitPos = ui_face.transform.position; 
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnTriggerEnter ( Collider _other ) {
        if ( _other.tag == "Zombie" ) {
            curHP -= 5.0f;

            // update UI
            UIProgressBar hpProgressBar = ui_HP.GetComponent(typeof(UIProgressBar)) as UIProgressBar;
            hpProgressBar.Value = this.GetHP();

            iTween.Stop(ui_face, "shake" );
            ui_face.transform.position = faceInitPos;
            iTween.ShakePosition(ui_face, 10.0f * Vector3.right, 0.5f );
            // iTween.ShakeRotation(ui_face, 30.0f * Vector3.forward, 0.5f );
        }
    }
}

