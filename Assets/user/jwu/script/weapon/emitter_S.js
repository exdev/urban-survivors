// ======================================================================================
// File         : emitter_S.js
// Author       : Wu Jie 
// Last Change  : 09/18/2010 | 11:10:57 AM | Saturday,September
// Description  : 
// ======================================================================================

#pragma strict

///////////////////////////////////////////////////////////////////////////////
// properties
///////////////////////////////////////////////////////////////////////////////

class emitter_S extends emitter {

    var angle = 30.0;
    var max_bullet = 5;

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    virtual function Emit ( _bullet : GameObject ) {
        // create a bullet, and rotate it based on the vector inputRotation

        var half_ang = angle * 0.5;

        for ( i = 0; i < max_bullet; ++i ) {
            var rot = transform.rotation;
            rot.eulerAngles.y += Random.Range(-half_ang, half_ang);

            // KEEPME: var spawn_bullet:GameObject = Instantiate(_bullet, transform.position, rot );
            var spawn_bullet:GameObject = SpawnManager.Instance().Spawn(_bullet, transform.position, rot );
            if ( spawn_bullet == null ) {
                Debug.Log("failed to spawn bullet");
            }
            spawn_bullet.transform.position += Random.Range(0.0,3.0) * spawn_bullet.transform.forward;
            CollisionIgnoreManager.Instance().AddIgnore( spawn_bullet.collider, Constant.mask_bullet, Constant.mask_player|Constant.mask_bullet );
        }
    }
}
