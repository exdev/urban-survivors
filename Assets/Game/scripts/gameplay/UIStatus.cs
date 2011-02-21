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

    public GameObject[] ShowHideControls;
    protected Vector3[] showHideInitPos;

    public UIProgressBar boyProgressBar = null;
    public PackedSprite boyFace = null; 

    public UIProgressBar girlProgressBar = null;
    public PackedSprite girlFace = null; 

    public SpriteText bulletCounterText = null;
    public SpriteText totalBulletCounterText = null;

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

    public GameObject hint_lowAmmo = null; 
    public GameObject hint_reloadBin = null; 
    public GameObject hint_tapAgain = null; 

    protected ScreenPad screenPad = null;
    protected Transform initBoyTrans; 
    protected Transform initGirlTrans; 

    protected delegate void StateUpdate();
    protected StateUpdate ReloadButtonState;
    protected bool blinkText = false;
    protected bool showControls = false;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Awake () {
        screenPad = GetComponent<ScreenPad>();

        this.initBoyTrans = boyFace.transform;
        this.initGirlTrans = girlFace.transform;

        //
        this.showHideInitPos = new Vector3[ShowHideControls.Length];
        for ( int i = 0; i < ShowHideControls.Length; ++i ) {
            showHideInitPos[i] = ShowHideControls[i].transform.localPosition;
            Vector3 offset = showHideInitPos[i].normalized * 400.0f;
            ShowHideControls[i].transform.localPosition = new Vector3( showHideInitPos[i].x + offset.x,
                                                                       showHideInitPos[i].y + offset.y,
                                                                       showHideInitPos[i].z );
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Start () {
        // hide active reload bar at the beginning
        activeReloadBar.SetColor( new Color( 1.0f, 1.0f, 1.0f, 0.0f ) );
        activeReloadFloat.SetColor( new Color( 1.0f, 1.0f, 1.0f, 0.0f ) );
        activeReloadZone.SetColor( new Color( 1.0f, 1.0f, 1.0f, 0.0f ) );

        //
        this.hint_lowAmmo.SetActiveRecursively(false);
        this.hint_reloadBin.SetActiveRecursively(false);
        this.hint_tapAgain.SetActiveRecursively(false);

        //
        this.ActiveAimingZone(false);
        this.ActiveMovingZone(false);
        this.ActiveMeleeButton(false);

        // init reload button state
        DisableReloadButton();
        ReloadButtonState = UpdateReloadDeactive;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public IEnumerator ShowControls ( bool _show, float _sec ) {
        this.showControls = _show;
        if ( _show ) {
            yield return StartCoroutine ( CoroutineHelper.WaitForRealSeconds(0.1f) ); 

            // process fade in
            for ( int i = 0; i < ShowHideControls.Length; ++i ) {
                iTween.MoveTo( ShowHideControls[i],
                               iTween.Hash( "position", showHideInitPos[i],
                                            "time", _sec, 
                                            "isLocal", true,
                                            "easetype", iTween.EaseType.easeOutBack,
                                            "ignoretimescale", true
                                          ) );
            }
        }
        else {
            // reset the button status
            iTween.Stop(this.gameObject, true );

            this.girlFace.transform.position = this.initGirlTrans.position;
            this.girlFace.transform.rotation = this.initGirlTrans.rotation;
            this.boyFace.transform.position = this.initBoyTrans.position;
            this.boyFace.transform.rotation = this.initBoyTrans.rotation;

            this.hint_lowAmmo.SetActiveRecursively(false);
            this.hint_reloadBin.SetActiveRecursively(false);
            this.hint_tapAgain.SetActiveRecursively(false);
            this.ActiveAimingZone(false);
            this.ActiveMovingZone(false);
            this.ActiveMeleeButton(false);

            // hide active reload bar at the beginning
            activeReloadBar.SetColor( new Color( 1.0f, 1.0f, 1.0f, 0.0f ) );
            activeReloadFloat.SetColor( new Color( 1.0f, 1.0f, 1.0f, 0.0f ) );
            activeReloadZone.SetColor( new Color( 1.0f, 1.0f, 1.0f, 0.0f ) );

            // init reload button state
            DisableReloadButton();
            ReloadButtonState = UpdateReloadDeactive;
            yield return StartCoroutine ( CoroutineHelper.WaitForRealSeconds(0.2f) ); 

            // process fade out
            for ( int i = 0; i < ShowHideControls.Length; ++i ) {
                Vector3 pos = showHideInitPos[i];
                pos = new Vector3 ( pos.x + pos.normalized.x * 400.0f,
                                    pos.y + pos.normalized.y * 400.0f,
                                    pos.z );
                iTween.MoveTo( ShowHideControls[i],
                               iTween.Hash( "position", pos,
                                            "time", _sec,
                                            "isLocal", true,
                                            "easetype", iTween.EaseType.easeOutBack,
                                            "ignoretimescale", true
                                          ) );
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Update () {
        PlayerInfo boyInfo = Game.GetPlayerBoy().playerInfo;
        this.boyProgressBar.Value = boyInfo.curHP/boyInfo.maxHP;

        PlayerGirl girl = Game.GetPlayerGirl() as PlayerGirl;
        PlayerInfo girlInfo = Game.GetPlayerGirl().playerInfo;
        this.girlProgressBar.Value = 1.0f - girlInfo.curHP/girlInfo.maxHP;

        // update bullets
        if ( this.showControls ) {
            if ( this.bulletCounterText && this.totalBulletCounterText ) {
                ShootInfo shootInfo = girl.GetShootInfo();

                // bullet counter display color
                if (shootInfo.CurBullets()<=10) {
                    this.bulletCounterText.SetColor(Color.red);
                    if ( this.hint_reloadBin.active == false ) {
                        this.hint_reloadBin.SetActiveRecursively(true);
                        this.hint_reloadBin.SendMessage("StartBlink");
                    }
                }
                else if (shootInfo.CurBullets()<=20) {
                    this.bulletCounterText.SetColor(Color.yellow);
                }
                else {
                    this.bulletCounterText.SetColor(Color.white);
                }

                //
                if ( shootInfo.RemainBullets() <= 50 ) {
                    if ( this.hint_lowAmmo.active == false ) {
                        this.hint_lowAmmo.SetActiveRecursively(true);
                        this.hint_lowAmmo.SendMessage("StartBlink");
                    }
                }
                else
                    this.hint_lowAmmo.SetActiveRecursively(false);

                if ( this.blinkText ) {
                    Color blinkColor = this.bulletCounterText.color;
                    blinkColor.g = Time.time % 0.5f;
                    this.bulletCounterText.SetColor(blinkColor);
                    this.bulletCounterText.SetCharacterSize(70.0f);
                }

                // curbullet / totalbullet
                this.bulletCounterText.Text = shootInfo.CurBullets() + "";
                this.totalBulletCounterText.Text = "/" + shootInfo.RemainBullets();
            }
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
            iTween.ScaleFrom ( this.meleeButton.gameObject, new Vector3( 1.5f, 1.5f, 1.5f ), 0.4f ); 
            Game.GetPlayerBoy().SendMessage("OnMelee");
        }
        else if ( screenPad.MeleeButtonUp() ) {
            iTween.Stop ( this.meleeButton.gameObject, "scale" ); 
            this.meleeButton.transform.localScale = new Vector3( 1.0f, 1.0f, 1.0f );
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
        PlayerGirl girl = Game.GetPlayerGirl() as PlayerGirl;
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
        PlayerGirl girl = Game.GetPlayerGirl() as PlayerGirl;

        //
        if ( girl.IsReloading() == false ) {
            StartCoroutine(HideActiveReloadBar("none"));
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
        reloadButton.color.a = 0.2f;
        reloadButton.SetColor( reloadButton.color );

        reloadindEffect.color.a = 0.2f;
        reloadindEffect.SetColor(reloadindEffect.color );
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

    IEnumerator OnBoyHit () {
        iTween.Stop(this.boyFace.gameObject, "shake" );
        this.boyFace.transform.position = this.initBoyTrans.position;
        this.boyFace.transform.rotation = this.initBoyTrans.rotation;
        yield return new WaitForSeconds(0.1f);
        iTween.ShakePosition( this.boyFace.gameObject, 
                              iTween.Hash( "amount", 10.0f * Vector3.right, 
                                           "time", 0.5f
                                         ) );
        // iTween.ShakeRotation(this.boyFace, 30.0f * Vector3.forward, 0.5f );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    IEnumerator OnGirlHit () {
        iTween.Stop(this.girlFace.gameObject, "shake" );
        this.boyFace.transform.position = this.initBoyTrans.position;
        this.boyFace.transform.rotation = this.initBoyTrans.rotation;
        yield return new WaitForSeconds(0.1f);
        iTween.ShakePosition( this.girlFace.gameObject, 
                              iTween.Hash( "amount", 10.0f * Vector3.right, 
                                           "time", 0.5f
                                         ) );
        // iTween.ShakeRotation(this.girlFace, 30.0f * Vector3.forward, 0.5f );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // NOTE: we can receive "failed", "successful" and "none" 
    // ------------------------------------------------------------------ 

    public IEnumerator HideActiveReloadBar ( string _state ) {
        iTween.Stop ( this.activeReloadFloat.gameObject, "move" ); 
        iTween.Stop ( this.reloadindEffect.gameObject, "rotate" ); 
        this.reloadindEffect.transform.rotation = Quaternion.identity;

        if ( _state == "failed" ) {
            // For Designer: when failed active reload { 
            // this code controls active reload bar failed color. 
            // failed color
            activeReloadBar.SetColor( Color.red );
            activeReloadFloat.SetColor( Color.red );
            activeReloadZone.SetColor( Color.red );

            // shake 
            iTween.ShakePosition( this.activeReloadBar.transform.parent.gameObject, 
                                  20.0f * Vector3.right, 
                                  0.5f );
            // when play shake wait for the shake seconds then fade out.
            yield return new WaitForSeconds(0.5f);
            // } For Designer end 
        }
        else if ( _state == "successful" ) {
            // NOTE: the color already white, you can't explosure color in Unity3d,
            //       I suggest using one more texture/effect overlap the bar for presentation.
            // For Designer: when succeeded active reload { 
            // this code controls active reload bar fade out scale.
            iTween.ScaleTo ( this.activeReloadBar.transform.parent.gameObject, 
                             iTween.Hash( "scale", new Vector3(1.5f, 3.0f, 1.0f),
                                          "time", 0.3f,
                                          "easetype", iTween.EaseType.bounce 
                                        ) );
            // } For Designer end 
        }

        // For Designer { 
        // this code controls active reload bar color.alpha fade "from - to"
        iTween.ValueTo ( this.gameObject, 
                         iTween.Hash( "from", 1.0f, 
                                      "to", 0.0f,
                                      "time", 2.0f,
                                      "easetype", iTween.EaseType.easeOutQuint, 
                                      "onupdatetarget", this.gameObject,
                                      "onupdate", "ActiveReloadBarColorUpdate"
                                    ) );
        this.hint_tapAgain.SetActiveRecursively(false);
        // } For Designer end 
		
    } 

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void ActiveReloadBarColorUpdate ( float _value ) {
        this.activeReloadBar.color.a = _value;
        activeReloadBar.SetColor(activeReloadBar.color);
        this.activeReloadFloat.color.a = _value;
        activeReloadFloat.SetColor(activeReloadFloat.color);
        this.activeReloadZone.color.a = _value;
        activeReloadZone.SetColor(activeReloadZone.color);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void ShowActiveReloadBar () {
        // reset all items
        iTween.Stop ( this.activeReloadBar.transform.parent.gameObject );
        iTween.Stop ( this.gameObject );
        this.activeReloadBar.transform.parent.localScale = new Vector3(1.0f,1.0f,1.0f);
        Color col = new Color( 1.0f, 1.0f, 1.0f, 0.0f );
        activeReloadBar.SetColor(col);
        activeReloadFloat.SetColor(col);
        activeReloadZone.SetColor(col);

        iTween.ValueTo ( this.gameObject, 
                         iTween.Hash( "from", 0.0f, 
                                      "to", 1.0f,
                                      "time", 0.2f,
                                      "easetype", iTween.EaseType.easeOutQuint, 
                                      "onupdatetarget", this.gameObject,
                                      "onupdate", "ActiveReloadBarColorUpdate"
                                    ) );

        // reset the reload float.
        float left_worldpos = this.activeReloadBar.transform.position.x - activeReloadBar.width * 0.5f; 
        this.activeReloadFloat.transform.position = 
            new Vector3( left_worldpos,
                         this.activeReloadFloat.transform.position.y,
                         this.activeReloadFloat.transform.position.z );

        // calculate and place active reload zone.
        PlayerGirl girl = Game.GetPlayerGirl() as PlayerGirl;
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
        this.StopActiveReload();
        PlayerGirl girl = Game.GetPlayerGirl() as PlayerGirl;
        // ShootInfo shootInfo = girl.GetShootInfo();
        this.ShowActiveReloadBar();

        {
            // first enable the reload status
            reloadButton.color.a = 1.0f;
            reloadButton.SetColor(reloadButton.color);
            reloadindEffect.color.a = 1.0f;
            reloadindEffect.SetColor(reloadindEffect.color);

            iTween.ScaleFrom ( this.reloadButton.gameObject, new Vector3( 1.5f, 1.5f, 1.5f ), 0.4f ); 
            iTween.RotateBy ( this.reloadindEffect.gameObject, 
                              iTween.Hash( "amount", Vector3.forward,
                                           "time", 1.0f,
                                           "easetype", iTween.EaseType.easeInOutQuad, 
                                           "looptype", iTween.LoopType.loop 
                                         ) ); 
        }

        {
            //
            float reloadTime = girl.ReloadTime();
            Vector3 newPos = this.activeReloadFloat.transform.position 
                + new Vector3( activeReloadBar.width, 0.0f, 0.0f );
            iTween.MoveTo ( this.activeReloadFloat.gameObject, 
                            iTween.Hash( "position", newPos,
                                         "time", reloadTime,
                                         "easetype", iTween.EaseType.easeOutQuart 
                                       ) ); 

            //
            girl.SendMessage("OnReload");
            this.hint_reloadBin.SetActiveRecursively(false);
            if ( this.hint_tapAgain.active == false ) {
                this.hint_tapAgain.SetActiveRecursively(true);
                this.hint_tapAgain.SendMessage("StartBlink");
            }
            this.ReloadButtonState = UpdateAcceptActiveReload;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void OnActiveReload () {
        PlayerGirl girl = Game.GetPlayerGirl() as PlayerGirl;

        float float_pos = this.activeReloadFloat.transform.position.x;
        float min_arpos = this.activeReloadZone.transform.position.x
            - this.activeReloadZone.width * 0.5f;
        float max_arpos = this.activeReloadZone.transform.position.x
            + this.activeReloadZone.width * 0.5f;

        if ( float_pos >= min_arpos && float_pos <= max_arpos ) {
            girl.SendMessage("OnActiveReload");
            StartCoroutine(HideActiveReloadBar("successful"));
            DisableReloadButton();
            this.ReloadButtonState = UpdateReloadDeactive;
        }
        else {
            StartCoroutine(HideActiveReloadBar("failed"));
            DisableReloadButton();
            this.ReloadButtonState = UpdateReloadDeactive;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    IEnumerator GoActiveReloadForSeconds ( float _seconds ) {
        this.girlFace.PlayAnim("goBerserk");
        this.blinkText = true;
        yield return new WaitForSeconds(_seconds); 
        StopActiveReload();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void StopActiveReload () {
        this.StopCoroutine("GoActiveReloadForSeconds");
        this.blinkText = false;
		this.bulletCounterText.SetCharacterSize(50.0f);
        this.girlFace.StopAnim();
    }
}
