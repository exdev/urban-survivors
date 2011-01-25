// ======================================================================================
// File         : ShootInfo.cs
// Author       : Wu Jie 
// Last Change  : 12/13/2010 | 21:36:27 PM | Monday,December
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

///////////////////////////////////////////////////////////////////////////////
// defines
///////////////////////////////////////////////////////////////////////////////

// ------------------------------------------------------------------ 
// Desc: ShootInfo
// ------------------------------------------------------------------ 

public class ShootInfo : MonoBehaviour {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public string shootAnim = "unknown";
    public string reloadAnim = "unknown";
    public Transform anchor = null;
    public GameObject bullet = null;
    public float shootSpeed = 1.0f;
    public float reloadSpeed = 1.0f;
    // public float accuracy = 1.0f; // DELME: looks like accuracy is hardcoded in the Emitter.
    public int capacity = 10;
    public float activeReloadTime = 2.0f;

    protected int bullets = 10;
    protected int totalBullets = 100;

    protected Emitter emitter = null;
    protected float activeReloadCounter = 0.0f;

    ///////////////////////////////////////////////////////////////////////////////
    // function defines
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Start () {
        DebugHelper.Assert(this.shootAnim!="unknown", "shootAnim can't be unknown!");
        DebugHelper.Assert(this.reloadAnim!="unknown", "reloadAnim can't be unknown!");
        DebugHelper.Assert(this.anchor, "anchor not set" );
        DebugHelper.Assert(this.bullet, "bullet not set" );
        BulletInfo bulletInfo = this.bullet.GetComponent<BulletInfo>();
        bulletInfo.ownerDamageInfo = this.GetComponent<DamageInfo>();

        this.emitter = GetComponent( typeof(Emitter) ) as Emitter;
        DebugHelper.Assert(this.emitter, "can't find emitter" );
        this.bullets = this.capacity;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Update () {
        this.activeReloadCounter = Mathf.Max ( this.activeReloadCounter - Time.deltaTime, 0.0f );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void ActiveReload ( bool _triggered ) {
        if ( _triggered )
            this.activeReloadCounter = this.activeReloadTime;
        else
            this.activeReloadCounter = 0.0f;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void AdjustAnim ( Animation _anim ) {
        AnimationState state = null;

        if ( activeReloadCounter > 0.0f ) {
            DamageInfo dmgInfo = this.GetComponent<DamageInfo>();
            dmgInfo.isActiveReload = true;

            // adjust shoot speed;
            state = _anim[this.shootAnim];
            state.normalizedSpeed = shootSpeed * 2.0f;

            // adjust reload speed;
            state = _anim[this.reloadAnim];
            state.normalizedSpeed = reloadSpeed * 4.0f;
        }
        else {
            DamageInfo dmgInfo = this.GetComponent<DamageInfo>();
            dmgInfo.isActiveReload = false;

            // adjust shoot speed;
            state = _anim[this.shootAnim];
            state.normalizedSpeed = shootSpeed;

            // adjust reload speed;
            state = _anim[this.reloadAnim];
            state.normalizedSpeed = reloadSpeed;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public bool OutOfAmmo () { return this.bullets <= 0; }
    public bool isAmmoFull () { return this.bullets == this.capacity; }
    public int CurBullets () { return this.bullets; }
    public int TotalBullets () { return this.totalBullets; }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void Fire () {
        if ( OutOfAmmo() == false ) {
            // TODO: bullet consume.
            this.emitter.Emit(this.anchor,this.bullet);
            this.bullets -= 1;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void Reload () {
        int amount = this.capacity - this.bullets;
        amount = Mathf.Min( this.totalBullets, amount );
        this.totalBullets -= amount;
        this.bullets += amount;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void AddBullets ( int _bullets ) {
        this.totalBullets += _bullets;
    }
}
