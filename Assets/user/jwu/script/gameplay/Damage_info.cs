// ======================================================================================
// File         : Damage_info.cs
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

public class Damage_info : MonoBehaviour {
    public enum DamageType {
        none,
        solid_melee,
        energy_melee,
        solid_bullet,
        energy_bullet,
    }

    public DamageType damageType = DamageType.none;
    public float DP = 20.0f;
    [System.NonSerialized] public Actor_info owner_info = null;
}
