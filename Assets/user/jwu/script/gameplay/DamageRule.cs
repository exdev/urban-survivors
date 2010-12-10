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

    public float[] hitBackForceList;
    protected Dictionary<DamageInfo.HitBackType,float> hitBackTypeToForce = new Dictionary<DamageInfo.HitBackType,float>();

    public string[] hitAnimList;
    protected Dictionary<HitInfo.HitType,string> hitTypeToAnim = new Dictionary<HitInfo.HitType,string>(); 

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
        for ( int i = 0; i < this.hitBackForceList.Length; ++i ) {
            this.hitBackTypeToForce[(DamageInfo.HitBackType)i] = this.hitBackForceList[i];
        }
        this.hitBackForceList = null;

        //
        for ( int i = 0; i < this.hitAnimList.Length; ++i ) {
            this.hitTypeToAnim[(HitInfo.HitType)i] = this.hitAnimList[i];
        }
        this.hitAnimList = null;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public float HitBackForce ( DamageInfo.HitBackType _type ) {
        return this.hitBackTypeToForce[_type]; 
    } 

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public string HitAnim ( HitInfo.HitType _type ) {
        return this.hitTypeToAnim[_type]; 
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
            // TODO:
        }
        else if ( _dmgInfo.damageType == DamageInfo.DamageType.energy_bullet ) {
            // TODO:
        }
        return 0.0f;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    float SolidMeleeDamage ( ActorInfo _defender, DamageInfo _dmgInfo ) 
    {
        ActorInfo attacker = _dmgInfo.owner_info;
        if ( attacker.isBerserk ) {
            // TODO: there is no document about boy's damage in berserk state
        }
        float dmgOutput = _dmgInfo.DP;
        _defender.curHP -= dmgOutput;
        return dmgOutput;
    }
}
