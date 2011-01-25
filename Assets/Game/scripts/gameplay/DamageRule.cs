// ======================================================================================
// File         : DamageRule.cs
// Author       : Wu Jie 
// Last Change  : 12/10/2010 | 11:10:26 AM | Friday,December
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
// Desc: class DamageRule
// ------------------------------------------------------------------ 

public class DamageRule : MonoBehaviour {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public float[] knockBackForceList;
    protected Dictionary<DamageInfo.KnockBackType,float> knockBackTypeToForce = new Dictionary<DamageInfo.KnockBackType,float>();

    public string[] hitAnimList;
    protected Dictionary<HitInfo.StunType,string> stunTypeToAnim = new Dictionary<HitInfo.StunType,string>(); 

    protected static DamageRule instance  = null;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public static DamageRule Instance() { return instance; }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Awake () {
        if( instance == null ) {
            instance = this;
        }

        //
        for ( int i = 0; i < this.knockBackForceList.Length; ++i ) {
            this.knockBackTypeToForce[(DamageInfo.KnockBackType)i] = this.knockBackForceList[i];
        }
        this.knockBackForceList = null;

        //
        for ( int i = 0; i < this.hitAnimList.Length; ++i ) {
            this.stunTypeToAnim[(HitInfo.StunType)i] = this.hitAnimList[i];
        }
        this.hitAnimList = null;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public float KnockBackForce ( DamageInfo.KnockBackType _type ) {
        return this.knockBackTypeToForce[_type]; 
    } 

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public string HitAnim ( HitInfo.StunType _type ) {
        return this.stunTypeToAnim[_type]; 
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public float CalculateDamage ( ActorInfo _defender, DamageInfo _dmgInfo ) 
    {
        if ( _dmgInfo.damageType == DamageInfo.DamageType.solid_melee ) {
            return SolidMeleeDamage(_defender,_dmgInfo);
        }
        else if ( _dmgInfo.damageType == DamageInfo.DamageType.energy_melee ) {
            // TODO:
        }
        else if ( _dmgInfo.damageType == DamageInfo.DamageType.solid_bullet ) {
            return SolidBulletDamage(_defender,_dmgInfo);
        }
        else if ( _dmgInfo.damageType == DamageInfo.DamageType.energy_bullet ) {
            // TODO:
        }
        return 0.0f;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    float SolidMeleeDamage ( ActorInfo _defender, DamageInfo _dmgInfo ) {
        ActorInfo attacker = _dmgInfo.owner_info;
        if ( attacker.isBerserk ) {
            // TODO: there is no document about boy's damage in berserk state
        }
        float dmgOutput = _dmgInfo.DP;
        _defender.curHP -= dmgOutput;
        return dmgOutput;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    float SolidBulletDamage ( ActorInfo _defender, DamageInfo _dmgInfo ) {
        ActorInfo attacker = _dmgInfo.owner_info;
        if ( attacker.isBerserk ) {
            // TODO: there is no document about boy's damage in berserk state
        }

        float dmgOutput = _dmgInfo.DP;
        // FIXME, HACK { this will have a bug, because DamageInfo is a reference, so the bullet that shooted will change the state. { 
        if ( _dmgInfo.isActiveReload )
            dmgOutput *= 1.2f;
        // } FIXME, HACK end 
        _defender.curHP -= dmgOutput;
        return dmgOutput;
    }
}
