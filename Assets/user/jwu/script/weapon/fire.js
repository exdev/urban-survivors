// ======================================================================================
// File         : fire.js
// Author       : Wu Jie 
// Last Change  : 08/30/2010 | 23:37:41 PM | Monday,August
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// properties
///////////////////////////////////////////////////////////////////////////////

var bullet : GameObject; 
var freq = 0.1;

private var timer = 0.0;
private var cur_player;

///////////////////////////////////////////////////////////////////////////////
// defines
///////////////////////////////////////////////////////////////////////////////

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

function Invoke () {
    // create a bullet, and rotate it based on the vector inputRotation
    var spawn_bullet = Instantiate(bullet, transform.position, transform.rotation );
    // Physics.IgnoreCollision( spawn_bullet.collider, cur_player.collider );
    CollisionIgnoreManager.Instance().AddIgnore( spawn_bullet.collider, Constant.mask_bullet, Constant.mask_player|Constant.mask_bullet );
}

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

function Start () {
    timer = 0.0;
    cur_player = GameObject.FindWithTag ("Player");
}

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

function Update () {
    if ( Input.GetMouseButtonDown(0) ) {
        Invoke();
    }
    else if ( Input.GetMouseButton(0) ) {
        timer += Time.deltaTime;
        if ( timer >= freq ) {
            timer = 0.0;
            Invoke();
        }
    }
}

