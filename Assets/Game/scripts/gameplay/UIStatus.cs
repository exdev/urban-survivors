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
    public TextMesh restartCounterText = null;
    public TextMesh bulletCounterText = null;

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

    public enum ReloadBtnState {
        accept_reload,
        accept_activeReload,
        disable
    };
    protected ReloadBtnState reloadBtnState = ReloadBtnState.accept_reload; 
    protected ReloadBtnState lastReloadBtnState = ReloadBtnState.accept_reload; 

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
        reloadButton.PlayAnimInReverse("active");
        reloadindEffect.color.a = 0.5f;
        reloadindEffect.SetColor(reloadindEffect.color);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Update () {
        PlayerInfo boyInfo = GameRules.Instance().GetPlayerBoyInfo();
        this.boyProgressBar.Value = boyInfo.curHP/boyInfo.maxHP;

        Player_girl girl = GameRules.Instance().GetPlayerGirl() as Player_girl;
        PlayerInfo girlInfo = GameRules.Instance().GetPlayerGirlInfo();
        this.girlProgressBar.Value = 1.0f - girlInfo.curHP/girlInfo.maxHP;

        if ( GameRules.Instance().IsGameOver() ) {
            gameOver.SetActiveRecursively(true);
            if ( this.restartCounterText )
                this.restartCounterText.text = string.Format( "{0:0}", GameRules.Instance().RestartCounter() );
        }

        // update bullets
        if ( this.bulletCounterText ) {
            ShootInfo shootInfo = girl.GetShootInfo();

            // curbullet / totalbullet
            this.bulletCounterText.text = shootInfo.CurBullets() + "/" + shootInfo.RemainBullets();
        }

        // KEEPME { 
        // if ( Input.GetKeyDown("k") )
        //     this.ActiveMeleeButton(true);
        // else if ( Input.GetKeyUp("k") )
        //     this.ActiveMeleeButton(false);
        // } KEEPME end 

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
        if ( screenPad.MeleeButtonDown() )
            this.ActiveMeleeButton(true);
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
            aimingNeedle.color.a = 0.5f;
            aimingNeedle.SetColor(aimingNeedle.color);
            aimingOutline.color.a = 0.5f;
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
            moveAnalog.color.a = 0.5f;
            moveAnalog.SetColor(moveAnalog.color);
            moveOutline.color.a = 0.5f;
            moveOutline.SetColor(moveOutline.color);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void ActiveMeleeButton ( bool _active ) {
        if ( _active ) {
            meleeButton.PlayAnim("active");
        }
        else {
            meleeButton.PlayAnimInReverse("active");
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void UpdateReloadButtonState () {
        this.lastReloadBtnState = this.reloadBtnState;

        Player_girl girl = GameRules.Instance().GetPlayerGirl() as Player_girl;
        ShootInfo shootInfo = girl.GetShootInfo();

        // get current reload button state
        if ( girl.IsReloading() == false && 
             shootInfo.isAmmoFull() == false && 
             shootInfo.NoBulletForReloading() == false ) 
        {
            this.reloadBtnState = ReloadBtnState.accept_reload;
        }
        else if ( girl.IsReloading() && 
                  this.lastReloadBtnState != ReloadBtnState.disable ) {
            this.reloadBtnState = ReloadBtnState.accept_activeReload;
        }
        else {
            this.reloadBtnState = ReloadBtnState.disable;
        }

        // check if we need process 
        if ( this.lastReloadBtnState != this.reloadBtnState ) {
            if ( this.reloadBtnState == ReloadBtnState.accept_reload ) {
                reloadButton.color.a = 1.0f;
                reloadButton.SetColor(reloadButton.color);
                reloadindEffect.color.a = 0.5f;
                reloadindEffect.SetColor(reloadindEffect.color);
            }
            else if ( this.reloadBtnState == ReloadBtnState.accept_activeReload ) {
            }
        }

        //
        if ( this.reloadBtnState != ReloadBtnState.disable ) {
            if ( screenPad.ReloadButtonDown() ) {
                reloadButton.PlayAnim("active");
                reloadindEffect.PlayAnim("active");
            }
            else if ( screenPad.ReloadButtonUp() ) {
                reloadButton.PlayAnimInReverse("active");
                reloadindEffect.PlayAnimInReverse("active");
            }
        }
        else {
            reloadButton.color.a = 0.5f;
            reloadButton.SetColor(reloadButton.color);
            reloadindEffect.color.a = 0.2f;
            reloadindEffect.SetColor(reloadindEffect.color);
        }
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
}
