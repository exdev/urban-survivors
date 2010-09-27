// ======================================================================================
// File         : Emitter_default.cs
// Author       : Wu Jie 
// Last Change  : 09/27/2010 | 21:46:38 PM | Monday,September
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

public class Emitter_default : Emitter {

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public override void Emit ( GameObject _bullet ) {
        // create a bullet, and rotate it based on the vector inputRotation
        GameObject spawn_bullet = (GameObject)Instantiate(_bullet, transform.position, transform.rotation );
        CollisionIgnoreManager.Instance().AddIgnore( spawn_bullet.collider, Constant.mask_bullet, Constant.mask_player|Constant.mask_bullet );
    }
}
