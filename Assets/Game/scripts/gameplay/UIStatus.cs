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

    public PackedSprite activeReloadBar = null;
    public PackedSprite activeReloadFloat = null;
    public PackedSprite activeReloadZone = null;

    protected ScreenPad screenPad = null;
    protected Transform initBoyTrans; 
    protected Transform initGirlTrans; 

    protected delegate void StateUpdate();
    protected StateUpdate ReloadButtonState;

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

        this.HideActiveReloadBar();

        // 
        this.ActiveAimingZone(false);
        this.ActiveMovingZone(false);
        this.ActiveMeleeButton(false);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Start () {
        DisableReloadButton();
        ReloadButtonState = UpdateReloadDeactive;
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
            GameRules.Instance().GetPlayerBoy().SendMessage("OnMelee");
        }
        this.ActiveMeleeButton(screenPad.MeleeButtonPressing());

        // reload button
        ReloadButtonState();
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

    void UpdateReloadDeactive () {
        PlayerGirl girl = GameRules.Instance().GetPlayerGirl() as PlayerGirl;
        ShootInfo shootInfo = girl.GetShootInfo();

        if ( shootInfo.NoBulletForReloading() == false &&
             shootInfo.isAmmoFull() == false )
        {
            EnableReloadButton ();
            ReloadButtonState = UpdateAcceptReload;
            return;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void UpdateAcceptReload () {
        // reload button
        if ( screenPad.ReloadButtonDown() ) {
            this.OnReload();
            return;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void UpdateAcceptActiveReload () {
        PlayerGirl girl = GameRules.Instance().GetPlayerGirl() as PlayerGirl;

        //
        if ( girl.IsReloading() == false ) {
            HideActiveReloadBar();
            DisableReloadButton();
            ReloadButtonState = UpdateReloadDeactive;
            return;
        }

        //
        if ( screenPad.ReloadButtonDown() ) {
            this.OnActiveReload();
            return;
        }

        //
        if ( screenPad.ReloadButtonPressing() ) {
            reloadButton.color.a = 1.0f;
            reloadButton.SetColor(reloadButton.color);
            reloadindEffect.color.a = 1.0f;
            reloadindEffect.SetColor(reloadindEffect.color);
        }
        else {
            reloadButton.color.a = 0.5f;
            reloadButton.SetColor(reloadButton.color);
            reloadindEffect.color.a = 0.5f;
            reloadindEffect.SetColor(reloadindEffect.color);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void DisableReloadButton () {
        reloadButton.color.a = 0.1f;
        reloadButton.SetColor(reloadButton.color);
        reloadindEffect.color.a = 0.1f;
        reloadindEffect.SetColor(reloadindEffect.color);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void EnableReloadButton () {
        reloadButton.color.a = 0.5f;
        reloadButton.SetColor(reloadButton.color);
        reloadindEffect.color.a = 0.5f;
        reloadindEffect.SetColor(reloadindEffect.color);
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

    public void HideActiveReloadBar () {
        this.activeReloadBar.color.a = 0.0f;
        activeReloadBar.SetColor(activeReloadBar.color);
        this.activeReloadFloat.color.a = 0.0f;
        activeReloadFloat.SetColor(activeReloadFloat.color);
        this.activeReloadZone.color.a = 0.0f;
        activeReloadZone.SetColor(activeReloadZone.color);

        iTween.Stop ( this.activeReloadFloat.gameObject, "move" ); 
        iTween.Stop ( this.reloadindEffect.gameObject, "rotate" ); 
        this.reloadindEffect.transform.rotation = Quaternion.identity;
    } 

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void ShowActiveReloadBar () {
        this.activeReloadBar.color.a = 1.0f;
        activeReloadBar.SetColor(activeReloadBar.color);
        this.activeReloadFloat.color.a = 1.0f;
        activeReloadFloat.SetColor(activeReloadFloat.color);
        this.activeReloadZone.color.a = 1.0f;
        activeReloadZone.SetColor(activeReloadZone.color);

        // reset the reload float.
        float left_worldpos = this.activeReloadBar.transform.position.x - activeReloadBar.width * 0.5f; 
        this.activeReloadFloat.transform.position = 
            new Vector3( left_worldpos,
                         this.activeReloadFloat.transform.position.y,
                         this.activeReloadFloat.transform.position.z );

        // calculate and place active reload zone.
        PlayerGirl girl = GameRules.Instance().GetPlayerGirl() as PlayerGirl;
        ShootInfo shootInfo = girl.GetShootInfo();
        Vector2 zoneInPercentage = shootInfo.CalcActiveReloadZone();

        //
        float left = activeReloadBar.width * zoneInPercentage.x; 
        float right = activeReloadBar.width * zoneInPercentage.y; 
        this.activeReloadZone.transform.position = new Vector3( left_worldpos + (left + right) * 0.5f,
                                                                this.activeReloadZone.transform.position.y,
                                                                this.activeReloadZone.transform.position.z );  
        this.activeReloadZone.SetSize( right - left, this.activeReloadZone.height );
    } 


    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void OnReload () {
        //
        PlayerGirl girl = GameRules.Instance().GetPlayerGirl() as PlayerGirl;
        // ShootInfo shootInfo = girl.GetShootInfo();
        this.ShowActiveReloadBar();

        {
            // first enable the reload status
            reloadButton.color.a = 1.0f;
            reloadButton.SetColor(reloadButton.color);
            reloadindEffect.color.a = 1.0f;
            reloadindEffect.SetColor(reloadindEffect.color);

            iTween.ScaleFrom ( this.reloadButton.gameObject, new Vector3( 1.5f, 1.5f, 1.5f ), 0.2f ); 
            Hashtable args = iTween.Hash( "amount", Vector3.forward,
                                          "time", 1.0f,
                                          "easetype", iTween.EaseType.easeInOutQuad, 
                                          "looptype", iTween.LoopType.loop 
                                        );
            iTween.RotateBy ( this.reloadindEffect.gameObject, args ); 
        }

        {
            //
            float reloadTime = girl.ReloadTime();
            Vector3 newPos = this.activeReloadFloat.transform.position 
                + new Vector3( activeReloadBar.width, 0.0f, 0.0f );
            Hashtable args = iTween.Hash( "position", newPos,
                                          "time", reloadTime,
                                          "easetype", iTween.EaseType.easeOutQuart 
                                        );
            iTween.MoveTo ( this.activeReloadFloat.gameObject, args ); 

            //
            girl.SendMessage("OnReload");
            this.ReloadButtonState = UpdateAcceptActiveReload;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void OnActiveReload () {
        PlayerGirl girl = GameRules.Instance().GetPlayerGirl() as PlayerGirl;

        float float_pos = this.activeReloadFloat.transform.position.x;
        float min_arpos = this.activeReloadZone.transform.position.x
            - this.activeReloadZone.width * 0.5f;
        float max_arpos = this.activeReloadZone.transform.position.x
            + this.activeReloadZone.width * 0.5f;

        if ( float_pos >= min_arpos && float_pos <= max_arpos ) {
            girl.SendMessage("OnActiveReload");
            HideActiveReloadBar();
            DisableReloadButton();
            this.ReloadButtonState = UpdateReloadDeactive;
        }
        else {
            HideActiveReloadBar();
            DisableReloadButton();
            this.ReloadButtonState = UpdateReloadDeactive;
        }
    }
}
