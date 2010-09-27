// ======================================================================================
// File         : Emitter_S.cs
// Author       : Wu Jie 
// Last Change  : 09/27/2010 | 21:47:49 PM | Monday,September
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

public class Emitter_S : Emitter {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public float angle = 30.0f;
    public int max_bullet = 5;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public override void Emit ( GameObject _bullet ) {
        // create a bullet, and rotate it based on the vector inputRotation

        float half_ang = angle * 0.5f;

        for ( int i = 0; i < max_bullet; ++i ) {
            Quaternion rot = transform.rotation;
            rot.eulerAngles = new Vector3( rot.eulerAngles.x, 
                                           rot.eulerAngles.y + Random.Range(-half_ang, half_ang),  
                                           rot.eulerAngles.z 
                                           );

            // KEEPME: var spawn_bullet:GameObject = Instantiate(_bullet, transform.position, rot );
            GameObject spawn_bullet = SpawnManager.Instance().Spawn(_bullet, transform.position, rot );
            if ( spawn_bullet == null ) {
                Debug.Log("failed to spawn bullet");
            }
            spawn_bullet.transform.position += Random.Range(0.0f,3.0f) * spawn_bullet.transform.forward;
            CollisionIgnoreManager.Instance().AddIgnore( spawn_bullet.collider, Constant.mask_bullet, Constant.mask_player|Constant.mask_bullet );
        }
    }
}
