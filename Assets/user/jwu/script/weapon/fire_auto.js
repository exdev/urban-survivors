// ======================================================================================
// File         : fire_auto.js
// Author       : Wu Jie 
// Last Change  : 09/18/2010 | 09:35:03 AM | Saturday,September
// Description  : 
// ======================================================================================

class fire_auto extends fire {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    var bullet : GameObject; 
    var freq = 0.1;

    private var lastFireTime = 0.0;
    private var emitter : emitter;

    ///////////////////////////////////////////////////////////////////////////////
    // defines
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    function Start () {
        super.Start();
        lastFireTime = Time.time;
        emitter = GetComponent("emitter");
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    virtual function Trigger () {
        if ( Time.time - lastFireTime >= freq ) {
            lastFireTime = Time.time;
            if ( emitter ) {
                emitter.Emit(bullet);
            }
            else {
                Debug.Log("warning: your weapon don't have emitter!");
            }
        }
    }
}

