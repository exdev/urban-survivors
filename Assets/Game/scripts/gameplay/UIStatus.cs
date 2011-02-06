// ======================================================================================
// File         : UIStatus.cs
// Author       : Wu Jie 
// Last Change  : 12/20/2010 | 21:02:48 PM | Monday,December
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

///////////////////////////////////////////////////////////////////////////////
// class UIStatus 
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

public class UIStatus : MonoBehaviour {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public GameObject gameOver = null;
    public UIProgressBar boyProgressBar = null;
    public GameObject boyFace = null; 
    public UIProgressBar girlProgressBar = null;
    public GameObject girlFace = null; 
    public SpriteText restartCounterText = null;
    public SpriteText bulletCounterText = null;

    public PackedSprite aimingOutline = null;
    public PackedSprite aimingNeedle = null;

    public PackedSprite moveOutline = null;
    public PackedSprite moveAnalog = null;

    public PackedSprite meleeButton = null;

    public PackedSprite reloadButton = null;
    public PackedSprite reloadindEffect = null;

    protected ScreenPad screenPad = null;
    protected Transform initBoyTrans; 
    protected Transform initGirlTrans; 

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Awake () {
        DebugHelper.Assert(this.boyProgressBar, "boy progress bar not set");
        DebugHelper.Assert(this.girlProgressBar, "girl progress bar not set");
        DebugHelper.Assert(this.restartCounterText, "restart counter not set");
        DebugHelper.Assert(this.bulletCounterText, "bullet counter not set");
        DebugHelper.Assert(this.gameOver, "gameOver object not set");

        gameOver.SetActiveRecursively(false);
        screenPad = GetComponent<ScreenPad>();

        this.initBoyTrans = boyFace.transform;
        this.initGirlTrans = girlFace.transform;

        // 
        this.ActiveAimingZone(false);
        this.ActiveMovingZone(false);
        this.ActiveMeleeButton(false);

        // 
        // reloadindEffect.color.a = 0.5f;
        // reloadindEffect.SetColor(reloadindEffect.color);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Start () {
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Update () {
        PlayerInfo boyInfo = GameRules.Instance().GetPlayerBoyInfo();
        this.boyProgressBar.Value = boyInfo.curHP/boyInfo.maxHP;

        PlayerGirl girl = GameRules.Instance().GetPlayerGirl() as PlayerGirl;
        PlayerInfo girlInfo = GameRules.Instance().GetPlayerGirlInfo();
        this.girlProgressBar.Value = 1.0f - girlInfo.curHP/girlInfo.maxHP;

        if ( GameRules.Instance().IsGameOver() ) {
            gameOver.SetActiveRecursively(true);
            if ( this.restartCounterText )
                this.restartCounterText.Text = string.Format( "{0:0}", GameRules.Instance().RestartCounter() );
        }

        // update bullets
        if ( this.bulletCounterText ) {
            ShootInfo shootInfo = girl.GetShootInfo();

			// bullet counter display color
			if (shootInfo.CurBullets()<=10)
				this.bulletCounterText.SetColor(Color.red);
				else if (shootInfo.CurBullets()<=20)
					this.bulletCounterText.SetColor(Color.yellow);
			else this.bulletCounterText.SetColor(Color.white);
			
            // curbullet / totalbullet
            this.bulletCounterText.Text = shootInfo.CurBullets() + "/" + shootInfo.RemainBullets();
        }

        // ======================================================== 
        // update ui response
        // ======================================================== 

        // aiming zone
        if ( screenPad.AimingZoneDown() )
            this.ActiveAimingZone(true);
        else if ( screenPad.AimingZoneUp() )
            this.ActiveAimingZone(false);

        // moving zone 
        if ( screenPad.MoveZoneDown() )
            this.ActiveMovingZone(true);
        else if ( screenPad.MoveZoneUp() )
            this.ActiveMovingZone(false);

        // melee button
        if ( screenPad.MeleeButtonDown() ) {
            this.ActiveMeleeButton(true);
            GameRules.Instance().GetPlayerBoy().SendMessage("OnMelee");
        }
        else if ( screenPad.MeleeButtonUp() )
            this.ActiveMeleeButton(false);

        // reload button
        this.UpdateReloadButtonState();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void ActiveAimingZone ( bool _active ) {
        if ( _active ) {
            aimingNeedle.color.a = 1.0f;
            aimingNeedle.SetColor(aimingNeedle.color);
            aimingOutline.color.a = 1.0f;
            aimingOutline.SetColor(aimingOutline.color);
        } else {
            aimingNeedle.color.a = 0.3f;
            aimingNeedle.SetColor(aimingNeedle.color);
            aimingOutline.color.a = 0.3f;
            aimingOutline.SetColor(aimingOutline.color);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void ActiveMovingZone ( bool _active ) {
        if ( _active ) {
            moveAnalog.color.a = 1.0f;
            moveAnalog.SetColor(moveAnalog.color);
            moveOutline.color.a = 1.0f;
            moveOutline.SetColor(moveOutline.color);
        } else {
            moveAnalog.color.a = 0.3f;
            moveAnalog.SetColor(moveAnalog.color);
            moveOutline.color.a = 0.3f;
            moveOutline.SetColor(moveOutline.color);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void ActiveMeleeButton ( bool _active ) {
        if ( _active ) {
            meleeButton.color.a = 1.0f;
            meleeButton.SetColor(meleeButton.color);
            // meleeButton.PlayAnim("active");
        }
        else {
            meleeButton.color.a = 0.3f;
            meleeButton.SetColor(meleeButton.color);
            // meleeButton.PlayAnimInReverse("active");
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void UpdateReloadButtonState () {
        // TODO:
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnBoyHit () {
        iTween.Stop(this.boyFace, "shake" );
        this.boyFace.transform.position = this.initBoyTrans.position;
        this.boyFace.transform.rotation = this.initBoyTrans.rotation;
        iTween.ShakePosition(this.boyFace, 10.0f * Vector3.right, 0.5f );
        // iTween.ShakeRotation(this.boyFace, 30.0f * Vector3.forward, 0.5f );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnGirlHit () {
        iTween.Stop(this.girlFace, "shake" );
        this.girlFace.transform.position = this.initGirlTrans.position;
        this.girlFace.transform.rotation = this.initGirlTrans.rotation;
        iTween.ShakePosition(this.girlFace, 10.0f * Vector3.right, 0.5f );
        // iTween.ShakeRotation(this.girlFace, 30.0f * Vector3.forward, 0.5f );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void OnReload () {
        GameRules.Instance().GetPlayerGirl().SendMessage("OnReload");
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void OnActiveReload () {
        GameRules.Instance().GetPlayerGirl().SendMessage("OnActiveReload");
    }
}
