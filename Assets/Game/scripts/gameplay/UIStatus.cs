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
    public UIProgressBar girlProgressBar = null;
    public TextMesh restartCounterText = null;
    public TextMesh bulletCounterText = null;

    public PackedSprite aimingOutline = null;
    public PackedSprite aimingNeedle = null;

    public PackedSprite moveOutline = null;
    public PackedSprite moveAnalog = null;

    public PackedSprite meleeButton = null;
    public PackedSprite reloadButton = null;

    protected ScreenPad screenPad = null;

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

        // 
        this.ActiveAimingZone(false);
        this.ActiveMovingZone(false);
        this.ActiveMeleeButton(false);
        this.ActiveReloadButton(false);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Update () {
        PlayerInfo boyInfo = GameRules.Instance().GetPlayerBoyInfo();
        this.boyProgressBar.Value = boyInfo.curHP/boyInfo.maxHP;

        PlayerInfo girlInfo = GameRules.Instance().GetPlayerGirlInfo();
        this.girlProgressBar.Value = 1.0f - girlInfo.curHP/girlInfo.maxHP;

        if ( GameRules.Instance().IsGameOver() ) {
            gameOver.SetActiveRecursively(true);
            if ( this.restartCounterText )
                this.restartCounterText.text = string.Format( "{0:0}", GameRules.Instance().RestartCounter() );
        }

        // update bullets
        if ( this.bulletCounterText ) {
            Player_base girl = GameRules.Instance().GetPlayerGirl();
            ShootInfo shootInfo = girl.GetShootInfo();

            // curbullet / totalbullet
            this.bulletCounterText.text = shootInfo.CurBullets() + "/" + shootInfo.TotalBullets();
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
        if ( screenPad.ReloadButtonDown() )
            this.ActiveReloadButton(true);
        else if ( screenPad.ReloadButtonUp() )
            this.ActiveReloadButton(false);
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

    void ActiveReloadButton ( bool _active ) {
        if ( _active ) {
            reloadButton.PlayAnim("active");
        }
        else {
            reloadButton.PlayAnimInReverse("active");
        }
    }
}
