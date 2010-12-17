// ======================================================================================
// File         : ActorInfo.cs
// Author       : Wu Jie 
// Last Change  : 12/09/2010 | 16:32:35 PM | Thursday,December
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
// class ActorInfo
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

[System.Serializable]
public class ActorInfo {

    public enum ActorType {
        unknown,
        player_boy,
        player_girl,
        zombie_normal,
        zombie_aimless,
        zombie_fat,
        zombie_swat,
        zombie_spider
    }

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public ActorType actorType = ActorType.unknown; 
    public float maxHP = 100.0f;
    public bool isBerserk = false;
    [System.NonSerialized] public float curHP = 100.0f;
}

///////////////////////////////////////////////////////////////////////////////
// class PlayerInfo
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

[System.Serializable]
public class PlayerInfo : ActorInfo {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public float recoverTime = 5.0f;
    public WeaponBase.WeaponID weapon1 = WeaponBase.WeaponID.unknown;
    public WeaponBase.WeaponID weapon2 = WeaponBase.WeaponID.unknown;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // TODO: public void serialize () {...} 
}