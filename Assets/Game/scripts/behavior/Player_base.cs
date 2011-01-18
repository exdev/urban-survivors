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

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected class Condition_isStunning : FSM.Condition {
        Player_base player = null;

        public Condition_isStunning ( Player_base _player ) {
            this.player = _player;
        }

        public override bool exec () {
            return this.player.IsPlayingAnim("hit1") ||
                this.player.IsPlayingAnim("hit2");
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected class Condition_isOnStun : FSM.Condition {
        Player_base player = null;

        public Condition_isOnStun ( Player_base _player ) {
            this.player = _player;
        }

        public override bool exec () {
            return this.player.isGetStun();
        }
    }

    // ------------------------------------------------------------------ 
    // Desc:
    // ------------------------------------------------------------------ 

    protected class Action_ActOnStun : FSM.Action {
        Player_base player = null;

        public Action_ActOnStun ( Player_base _player ) {
            this.player = _player;
        }

        public override void exec () {
            this.player.ActOnStun();
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

            if ( GameRules.Instance().IsMultiPlayer() ) {
                hud = hud_m;
                if ( hud_s ) hud_s.SetActiveRecursively(false);
            }
            else {
                hud = hud_s;
                if ( hud_m ) hud_m.SetActiveRecursively(false);
            }

            if ( hud ) {
                screenPad = hud.GetComponent<ScreenPad>();
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

    public bool IsDown () { return this.isDown; } 
    public void Recover ( float _hp ) { 
        this.isDown = false; 
        this.playerInfo.curHP = Mathf.Min( this.playerInfo.curHP + _hp, this.playerInfo.maxHP );
    } 

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnTriggerEnter ( Collider _other ) {
        // don't do anything if player is down
        if ( this.isDown )
            return;

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
            return;
        }

        // if we don't get damage info, just return
        DebugHelper.Assert( dmgInfo, "can't find damage info for given layer" );
        if ( dmgInfo == null ) {
            return;
        }
        float dmgOutput = DamageRule.Instance().CalculateDamage( this.playerInfo, dmgInfo );
        this.playerInfo.accDmgNormal += dmgOutput;

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
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public bool isGetStun () {
        return this.lastHit.stunType != HitInfo.StunType.none;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void ActOnStun () {
        // NOTE: it could be possible we interupt to hit when boy is attacking.
        AttackInfo atk_info = this.GetAttackInfo();
        if ( atk_info != null && atk_info.curCombo != null )
            atk_info.curCombo.attack_shape.active = false;

        // HACK: simple random choose animation { 
        // string[] names = {"hit1", "hit2"};
        // string animName = names[Mathf.FloorToInt(Random.Range(0.0f,2.0f))];
        // } HACK end 
        string animName = "hit1";
        if ( IsBehind( this.lastHit.position ) )
            animName = "hit2";

        this.anim.Rewind(animName);
        this.anim.Play(animName);
    } 
}

