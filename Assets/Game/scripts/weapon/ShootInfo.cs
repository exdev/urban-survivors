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
    public float arShootSpeed = 2.0f;
    public float arReloadSpeed = 4.0f;
    public float arDamageIncrease = 1.2f;

    public Vector2 arRangeInPercentage = new Vector2( 0.0f, 1.0f );
    public float arLengthInPercentage = 0.2f;

    public AudioClip snd_fire = null;
    public AudioClip snd_onreload = null;

    protected int bullets = 10;
    protected int remainBullets = 100;

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
            dmgInfo.arDamageIncrease = this.arDamageIncrease;

            // adjust shoot speed;
            state = _anim[this.shootAnim];
            state.normalizedSpeed = arShootSpeed;

            // adjust reload speed;
            state = _anim[this.reloadAnim];
            state.normalizedSpeed = arReloadSpeed;
        }
        else {
            DamageInfo dmgInfo = this.GetComponent<DamageInfo>();
            dmgInfo.arDamageIncrease = 1.0f;

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
    public bool NoBulletForReloading () { return this.remainBullets <= 0; } // DELME
    public bool isAmmoFull () { return this.bullets == this.capacity; }
    public int CurBullets () { return this.bullets; }
    public int RemainBullets () { return this.remainBullets; }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void Fire () {
        if ( OutOfAmmo() == false ) {
            audio.PlayOneShot(snd_fire);
            int bullet_count = this.emitter.Emit(this.anchor,this.bullet,activeReloadCounter > 0.0f);
            this.bullets -= bullet_count;
            Game.Mission().SendMessage( "OnBulletUsed", bullet_count );
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void Reload () {
        int amount = this.capacity - this.bullets;
        amount = Mathf.Min( this.remainBullets, amount );
        this.remainBullets -= amount;
        this.bullets += amount;
        audio.PlayOneShot(snd_onreload);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void AddBullets ( int _bullets ) {
        this.remainBullets += _bullets;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public Vector2 CalcActiveReloadZone () {
        float pos = Random.Range( arRangeInPercentage.x, arRangeInPercentage.y );
        float half_len = arLengthInPercentage * 0.5f; 
        if ( pos + half_len > 1.0f )
            return new Vector2( 1.0f - arLengthInPercentage, 
                                1.0f );
        if ( pos - half_len < 0.0f )
            return new Vector2( 0.0f, 
                                arLengthInPercentage );

        return new Vector2( pos - half_len, 
                            pos + half_len );
    }
}
