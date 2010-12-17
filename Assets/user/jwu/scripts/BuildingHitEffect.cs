// ======================================================================================
// File         : BuildingHitEffect.cs
// Author       : Wu Jie 
// Last Change  : 12/17/2010 | 23:13:57 PM | Friday,December
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
//
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////
//
///////////////////////////////////////////////////////////////////////////////

public class BuildingHitEffect : MonoBehaviour {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    protected static GameObject fxHitBullet = null;
    public GameObject FX_HIT_bullet = null;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	void Awake () {
        if ( fxHitBullet == null && this.FX_HIT_bullet ) {
            fxHitBullet = (GameObject)Instantiate( this.FX_HIT_bullet );
        }
	}
	
    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnTriggerEnter ( Collider _other ) {
        if ( fxHitBullet != null ) {
            fxHitBullet.transform.position = _other.transform.position;
            fxHitBullet.transform.rotation = _other.transform.rotation;
            fxHitBullet.particleEmitter.Emit();
        }
    }
}
