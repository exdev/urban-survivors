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

    protected static GameObject fxHitBite = null;

    public GameObject FX_HIT_bite = null;
    public PlayerInfo playerInfo = new PlayerInfo();
    public Transform weaponAnchor = null;

    protected ScreenPad screenPad = null;
    protected GameObject curWeapon = null;
    protected GameObject weapon1 = null;
    protected GameObject weapon2 = null;


    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

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
        GameObject hud = GameObject.Find("HUD");
        if ( hud ) {
            screenPad = hud.GetComponent(typeof(ScreenPad)) as ScreenPad;
        }
        // DebugHelper.Assert( screenPad, "screenPad not found" );

        InitInfo();
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
}

