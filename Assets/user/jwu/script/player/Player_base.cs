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

[RequireComponent(typeof(CharacterController))]
public class Player_base : MonoBehaviour {

    protected ScreenPad screenPad = null;
    protected float curHP = 60.0f;
    protected Vector3 faceInitPos = Vector3.zero;
	protected PackedSprite faceSprite = null;
    protected CharacterController controller = null;

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public float maxSpeed = 10.0f;
    public float drag = 0.01f; // slow down the movement.
    public float maxHP = 100.0f;

    public GameObject ui_HP;
    public GameObject ui_face;

    public bool inverseHP = false;

    public float StepSpeed = 0.5f;

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
        //
        controller = GetComponent<CharacterController>();

        GameObject hud = GameObject.Find("HUD");
        if ( hud ) {
            screenPad = hud.GetComponent(typeof(ScreenPad)) as ScreenPad;
        }
        // DebugHelper.Assert( screenPad, "screenPad not found" );

        //
        if ( ui_HP ) {
            UIProgressBar hpProgressBar = ui_HP.GetComponent(typeof(UIProgressBar)) as UIProgressBar;
            hpProgressBar.Value = inverseHP ? 1.0f - this.GetHP() : this.GetHP();
        }
        if ( ui_face ) {
            faceInitPos = ui_face.transform.position; 
            faceSprite = ui_face.GetComponent(typeof(PackedSprite)) as PackedSprite;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnTriggerEnter ( Collider _other ) {
        if ( _other.tag == "Zombie" ) {
            curHP -= 5.0f;

            // update UI
            UIProgressBar hpProgressBar = ui_HP.GetComponent(typeof(UIProgressBar)) as UIProgressBar;
            hpProgressBar.Value = inverseHP ? 1.0f - this.GetHP() : this.GetHP();

            // TEMP nantas: { 
			//added animation change when HP is too low
			if ( this.GetHP() <= 0.2f ) {
                if ( faceSprite )
                    faceSprite.PlayAnim(0); //index for 0-goBerserk
			}
            // } TEMP end 
				
            if ( ui_face ) {
                iTween.Stop(ui_face, "shake" );
                ui_face.transform.position = faceInitPos;
                iTween.ShakePosition(ui_face, 10.0f * Vector3.right, 0.5f );
                // iTween.ShakeRotation(ui_face, 30.0f * Vector3.forward, 0.5f );
            }
        }
    }
}

