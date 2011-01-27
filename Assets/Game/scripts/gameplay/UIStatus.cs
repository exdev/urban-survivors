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
    // actions, conditions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: Condition_isReloadButtonDown 
    // ------------------------------------------------------------------ 

    class Condition_isReloadButtonDown : FSM.Condition {
        ScreenPad screenPad = null;

        public Condition_isReloadButtonDown ( ScreenPad _screenPad ) {
            this.screenPad = _screenPad;
        }

        public override bool exec () {
            return screenPad.ReloadButtonDown();
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: Condition_canReload 
    // ------------------------------------------------------------------ 

    class Condition_canReload : FSM.Condition {
        Player_girl girl;

        public Condition_canReload ( Player_girl _girl ) {
            this.girl = _girl;
        }

        public override bool exec () {
            ShootInfo shootInfo = this.girl.GetShootInfo();

            // get current reload button state
            return this.girl.IsReloading() == false && 
                shootInfo.isAmmoFull() == false && 
                shootInfo.NoBulletForReloading() == false;
        }
    }

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

    void InitFSM () {
        //
        Condition_isReloadButtonDown cond_isReloadButtonDown = new Condition_isReloadButtonDown(this.screenPad); 
        Condition_canReload cond_canReload = new Condition_canReload( GameRules.Instance().GetPlayerGirl() as Player_girl ); 

        // TODO: we should process reload here, and send the reload message to girl, so that it can perform reload
        // TODO: same as boy.
        // Action_onReload act_onReload = new Action_onReload(this);
        // Action_onReload act_onActiveReload = new Action_onActiveReload(this);

        // accept_reload
        FSM.State state_acceptReload = new FSM.State( "accept_reload", 
                                                      null, 
                                                      null, 
                                                      null );
        // accept_activeReload 
        FSM.State state_acceptActiveReload = new FSM.State( "accept_activeReload", 
                                                            null, 
                                                            null, 
                                                            null );
        // disable
        FSM.State state_disable = new FSM.State( "disable", 
                                                 null, 
                                                 null, 
                                                 null );
        //
        state_acceptReload.AddTransition( new FSM.Transition( state_disable, new FSM.Condition_not(cond_canReload), null ) );
        state_acceptReload.AddTransition( new FSM.Transition( state_acceptActiveReload, cond_isReloadButtonDown, null /*act_onReload*/ ) );

        //
        state_acceptActiveReload.AddTransition( new FSM.Transition( state_disable, cond_isReloadButtonDown, null /*act_onActiveReload*/ ) );

        //
        state_disable.AddTransition( new FSM.Transition( state_acceptReload, cond_canReload, null ) );
    }

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

    void Start () {
        InitFSM();
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
        // this.lastReloadBtnState = this.reloadBtnState;

        // Player_girl girl = GameRules.Instance().GetPlayerGirl() as Player_girl;
        // ShootInfo shootInfo = girl.GetShootInfo();

        // // get current reload button state
        // if ( girl.IsReloading() == false && 
        //      shootInfo.isAmmoFull() == false && 
        //      shootInfo.NoBulletForReloading() == false ) 
        // {
        //     this.reloadBtnState = ReloadBtnState.accept_reload;
        // }
        // else if ( girl.IsReloading() && 
        //           this.lastReloadBtnState != ReloadBtnState.disable ) {
        //     this.reloadBtnState = ReloadBtnState.accept_activeReload;
        // }
        // else {
        //     this.reloadBtnState = ReloadBtnState.disable;
        // }

        // // check if we need process 
        // if ( this.lastReloadBtnState != this.reloadBtnState ) {
        //     if ( this.reloadBtnState == ReloadBtnState.accept_reload ) {
        //         reloadButton.color.a = 1.0f;
        //         reloadButton.SetColor(reloadButton.color);
        //         reloadindEffect.color.a = 0.5f;
        //         reloadindEffect.SetColor(reloadindEffect.color);
        //     }
        //     else if ( this.reloadBtnState == ReloadBtnState.accept_activeReload ) {
        //         Hashtable args = iTween.Hash( "amount", new Vector3(0.0f, 0.0f, 1.0f),
        //                                       "time", 1.0f,
        //                                       "easetype", iTween.EaseType.easeOutCubic, 
        //                                       "looptype", iTween.LoopType.loop 
        //                                     );
        //         iTween.RotateBy ( reloadindEffect.gameObject, args );
        //     }
        // }
        // else {
        //     //
        //     if ( this.reloadBtnState == ReloadBtnState.accept_activeReload ) {
        //         if ( screenPad.ReloadButtonDown() ) {
        //             iTween.Stop (reloadButton.gameObject, true);
        //         }
        //         else if ( screenPad.ReloadButtonUp() ) {
        //             reloadButton.gameObject.transform.localScale = new Vector3(1.0f,1.0f,1.0f);
        //             Hashtable args = iTween.Hash( "scale", new Vector3(1.5f,1.5f,1.0f),
        //                                           "time", 0.5f,
        //                                           "easetype", iTween.EaseType.easeOutCirc 
        //                                         );
        //             iTween.ScaleFrom ( reloadButton.gameObject, args );
        //         }
        //     } 
        // }

        // //
        // if ( this.reloadBtnState != ReloadBtnState.disable ) {
        //     if ( screenPad.ReloadButtonDown() ) {
        //         reloadButton.PlayAnim("active");
        //         reloadindEffect.PlayAnim("active");
        //     }
        //     else if ( screenPad.ReloadButtonUp() ) {
        //         reloadButton.PlayAnimInReverse("active");
        //         reloadindEffect.PlayAnimInReverse("active");
        //     }
        // }
        // else {
        //     reloadButton.color.a = 0.5f;
        //     reloadButton.SetColor(reloadButton.color);
        //     reloadindEffect.color.a = 0.2f;
        //     reloadindEffect.SetColor(reloadindEffect.color);
        // }
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
