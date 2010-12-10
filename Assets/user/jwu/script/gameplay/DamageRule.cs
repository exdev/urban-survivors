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
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void CalculateDamage ( Actor_info _defender, Damage_info _dmgInfo ) 
    {
        if ( _dmgInfo.damageType == Damage_info.DamageType.solid_melee ) {
            SolidMeleeDamage(_defender,_dmgInfo);
        }
        else if ( _dmgInfo.damageType == Damage_info.DamageType.energy_melee ) {
            // TODO:
        }
        else if ( _dmgInfo.damageType == Damage_info.DamageType.solid_bullet ) {
            // TODO:
        }
        else if ( _dmgInfo.damageType == Damage_info.DamageType.energy_bullet ) {
            // TODO:
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void SolidMeleeDamage ( Actor_info _defender, Damage_info _dmgInfo ) 
    {
        Actor_info attacker = _dmgInfo.owner_info;
        if ( attacker.isBerserk ) {
            // TODO: there is no document about boy's damage in berserk state
        }
        _defender.curHP -= _dmgInfo.DP;
    }
}
