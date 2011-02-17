// ======================================================================================
// File         : DamageInfo.cs
// Author       : Wu Jie 
// Last Change  : 12/09/2010 | 16:16:56 PM | Thursday,December
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

public class DamageInfo : MonoBehaviour {
    public enum DamageType {
        none,
        solid_melee,
        energy_melee,
        solid_bullet,
        energy_bullet,
    };

    public enum KnockBackType {
        none,
        low,
        mid,
        high,
    };

    public float DP = 20.0f;
    public DamageType damageType = DamageType.none;
    public KnockBackType knockBackType = KnockBackType.none;
    [System.NonSerialized] public GameObject owner = null;
    [System.NonSerialized] public ActorInfo owner_info = null;
    [System.NonSerialized] public float arDamageIncrease = 1.0f; 
}
