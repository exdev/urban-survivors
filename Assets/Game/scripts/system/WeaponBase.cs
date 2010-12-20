// ======================================================================================
// File         : WeaponBase.cs
// Author       : Wu Jie 
// Last Change  : 12/09/2010 | 10:37:12 AM | Thursday,December
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
// Desc: WeaponBase
// ------------------------------------------------------------------ 

public class WeaponBase : MonoBehaviour {
    public enum WeaponID {
        unknown,
        melee_baseballbat,
        range_smg_starter,
    };

    [System.Serializable]
    public class WeaponPrefabInfo {
        public WeaponID id = WeaponID.unknown;
        public GameObject prefab = null;
    }

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public WeaponPrefabInfo[] weaponList;

    protected static WeaponBase weaponBase = null;
    protected Dictionary<WeaponBase.WeaponID,GameObject> weaponLookupTable 
        = new Dictionary<WeaponBase.WeaponID,GameObject>(); 

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public static WeaponBase Instance() {
        return weaponBase;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Awake () {
        if( weaponBase == null ) {
            weaponBase = this;

            // we need instantiate weapon from prefab
            for ( int i = 0; i < this.weaponList.Length; ++i ) {
                WeaponPrefabInfo prefabInfo = this.weaponList[i];
                GameObject weaponGO 
                    = GameObject.Instantiate(prefabInfo.prefab, 
                                             Vector3.zero, 
                                             Quaternion.identity ) as GameObject; 
                weaponGO.SetActiveRecursively(false);
                DebugHelper.Assert( prefabInfo.id != WeaponID.unknown,
                                    "Are you insane? the weaponID is unknown!" );
                this.weaponLookupTable[prefabInfo.id] = weaponGO;
            }
            this.weaponList = null; // after we init Dictionary, we don't need the array.
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public GameObject GetWeapon ( WeaponID _id ) {
        return this.weaponLookupTable[_id];
    }
}
