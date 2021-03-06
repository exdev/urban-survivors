// ======================================================================================
// File         : PlayerBase.cs
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
// class PlayerBase
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

public class PlayerBase : Actor {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    protected static GameObject fxHitBite = null;

    public GameObject FX_HIT_bite = null;
    public PlayerInfo playerInfo = new PlayerInfo();
    public Transform weaponAnchor = null;

    protected GameObject curWeapon = null;
    protected GameObject weapon1 = null;
    protected GameObject weapon2 = null;
    protected bool isGod = false;


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
        if ( Game.IsGameOver() == false
             && this.noHP() ) {
            this.SendMessage( "OnRecover", 10.0f );
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected new void Awake () {
        base.Awake();

        if ( fxHitBite == null && this.FX_HIT_bite ) {
            fxHitBite = (GameObject)Instantiate( this.FX_HIT_bite );
        }

        // check values
        DebugHelper.Assert( this.weaponAnchor, "can't find WeaponAnchor");
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	protected new void Start () {
        base.Start();
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

    public ShootInfo GetShootInfo () { return this.curWeapon.GetComponent<ShootInfo>(); }
    public AttackInfo GetAttackInfo () { return this.curWeapon.GetComponent<AttackInfo>(); }
    public DamageInfo GetDamageInfo () { return this.curWeapon.GetComponent<DamageInfo>(); }
    protected void SetCurWeaponOwner () { 
        DamageInfo dmgInfo = this.GetDamageInfo();
        dmgInfo.owner_info = this.playerInfo; 
        dmgInfo.owner = this.gameObject;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public bool noHP () { return playerInfo.curHP <= 0.0f; } 

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void AsGod ( bool _isGod ) { this.isGod = _isGod; }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected bool ApplyDamage ( Collider _other ) {
        // don't do anything if player is down
        if ( this.noHP() )
            return false;

        // god don't get hurt
        if ( this.isGod )
            return false;

        //
        DamageInfo dmgInfo = null;
        if ( _other.gameObject.layer == Layer.melee_enemy ) {
            dmgInfo = _other.GetComponent<DamageInfo>();

            if ( fxHitBite != null ) {
                fxHitBite.transform.position = _other.transform.position;
                fxHitBite.transform.rotation = _other.transform.rotation;
                fxHitBite.particleEmitter.Emit();
            }
        }
        else {
            return false;
        }

        // if we don't get damage info, just return
        DebugHelper.Assert( dmgInfo, "can't find damage info for given layer" );
        if ( dmgInfo == null ) {
            return false;
        }
        float dmgOutput = DamageRule.Instance().CalculateDamage( this.playerInfo, dmgInfo );
        this.playerInfo.accDmgNormal += dmgOutput;
        Game.Mission().SendMessage ( "OnPlayerHit", dmgOutput );

        if ( this.playerInfo.accDmgNormal >= this.playerInfo.normalStun ) {
            this.lastHit.stunType = HitInfo.StunType.normal;
            this.playerInfo.accDmgNormal = 0.0f;
        }
        else {
            this.lastHit.stunType = HitInfo.StunType.none;
        }

        // this.lastHit.position = _other.transform.position;
        this.lastHit.position = dmgInfo.owner.transform.position; // HACK:
        this.lastHit.normal = _other.transform.right;
        Vector3 dir = _other.transform.position - transform.position;
        dir.y = 0.0f;
        dir.Normalize();
        this.lastHit.knockBackForce = dir * DamageRule.Instance().KnockBackForce(dmgInfo.knockBackType);  

        return true;
    }
}

