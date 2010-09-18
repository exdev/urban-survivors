// ======================================================================================
// File         : emitter_forward.js
// Author       : Wu Jie 
// Last Change  : 09/18/2010 | 11:00:09 AM | Saturday,September
// Description  : 
// ======================================================================================

class emitter_forward extends emitter {

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    virtual function Emit ( _bullet : GameObject ) {
        // create a bullet, and rotate it based on the vector inputRotation
        var spawn_bullet = Instantiate(_bullet, transform.position, transform.rotation );
        CollisionIgnoreManager.Instance().AddIgnore( spawn_bullet.collider, Constant.mask_bullet, Constant.mask_player|Constant.mask_bullet );
    }
}

