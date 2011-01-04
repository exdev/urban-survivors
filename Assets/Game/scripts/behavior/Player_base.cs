// ======================================================================================
// File         : Player_base.cs
// Author       : Wu Jie 
// Last Change  : 11/29/2010 | 21:21:45 PM | Monday,November
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

public class Player_base : Actor {

    ///////////////////////////////////////////////////////////////////////////////
    // conditions, actions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected class Condition_isDown : FSM.Condition {
        Player_base player = null;

        public Condition_isDown ( Player_base _player ) {
            this.player = _player;
        }

        public override bool exec () {
            return player.IsDown();
        }
    }

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    protected static GameObject fxHitBite = null;

    public GameObject FX_HIT_bite = null;
    public PlayerInfo playerInfo = new PlayerInfo();
    public Transform weaponAnchor = null;

    protected bool isDown = false;
    static protected ScreenPad screenPad = null;
    protected GameObject curWeapon = null;
    protected GameObject weapon1 = null;
    protected GameObject weapon2 = null;


    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected IEnumerator WaitForRecover () {
        // Debug.Log("before recover time" + Time.time ); // DEBUG
        yield return new WaitForSeconds (this.playerInfo.recoverTime);
        // Debug.Log("after recover time" + Time.time ); // DEBUG

        // it is possible that we use HP pack save the player
        if ( GameRules.Instance().IsGameOver() == false
             && this.IsDown() == true ) {
            this.Recover(10.0f);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Awake () {
        if ( fxHitBite == null && this.FX_HIT_bite ) {
            fxHitBite = (GameObject)Instantiate( this.FX_HIT_bite );
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	protected new void Start () {
        base.Start();

        // check values
        DebugHelper.Assert( this.weaponAnchor, "can't find WeaponAnchor");

        // init hud
        if ( screenPad == null ) {
            GameObject hud = null;
            GameObject hud_s = GameObject.Find("HUD_s");
            GameObject hud_m = GameObject.Find("HUD_m");
            hud = hud_s;

            // if ( GameRules.Instance().IsMultiPlayer() ) {
            //     hud = hud_m;
            //     if ( hud_s ) hud_s.SetActiveRecursively(false);
            // }
            // else {
            //     hud = hud_s;
            //     if ( hud_m ) hud_m.SetActiveRecursively(false);
            // }

            if ( hud ) {
                screenPad = hud.GetComponent(typeof(ScreenPad)) as ScreenPad;
            }

#if UNITY_IPHONE
            if ( Application.isEditor == false ) {
                DebugHelper.Assert( screenPad, "screenPad not found" );
            }
#endif
        }

        InitInfo();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	protected new void Update () {
        base.Update();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void InitInfo () {
        // TODO: we should load info from saved data or check point.
        // info.serialize();

        if ( this.playerInfo.weapon1 != WeaponBase.WeaponID.unknown )
            this.ChangeWeapon(this.playerInfo.weapon1);
        else if ( this.playerInfo.weapon2 != WeaponBase.WeaponID.unknown )
            this.ChangeWeapon(this.playerInfo.weapon2);
        DebugHelper.Assert( this.curWeapon, "can't find any valid weapon" );

        this.playerInfo.curHP = this.playerInfo.maxHP;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void ChangeWeapon( WeaponBase.WeaponID _id ) {
        GameObject weaponGO = WeaponBase.Instance().GetWeapon(_id); 
        weaponGO.SetActiveRecursively(true);
        this.curWeapon = weaponGO; 
        this.curWeapon.transform.parent = this.weaponAnchor;
        this.curWeapon.transform.localPosition = Vector3.zero;
        this.curWeapon.transform.localRotation = Quaternion.identity;
        this.SetCurWeaponOwner();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected ShootInfo GetShootInfo () { return this.curWeapon.GetComponent<ShootInfo>(); }
    protected AttackInfo GetAttackInfo () { return this.curWeapon.GetComponent<AttackInfo>(); }
    protected DamageInfo GetDamageInfo () { return this.curWeapon.GetComponent<DamageInfo>(); }
    protected void SetCurWeaponOwner () { 
        DamageInfo dmgInfo = this.GetDamageInfo();
        dmgInfo.owner_info = this.playerInfo; 
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public bool IsDown () { return this.isDown; } 
    public void Recover ( float _hp ) { 
        this.isDown = false; 
        this.playerInfo.curHP = Mathf.Min( this.playerInfo.curHP + _hp, this.playerInfo.maxHP );
    } 
}

