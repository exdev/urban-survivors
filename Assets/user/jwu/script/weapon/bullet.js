// ======================================================================================
// File         : bullet.js
// Author       : Wu Jie 
// Last Change  : 08/31/2010 | 23:18:02 PM | Tuesday,August
// Description  : 
// ======================================================================================

#pragma strict

///////////////////////////////////////////////////////////////////////////////
// properties
///////////////////////////////////////////////////////////////////////////////

var speed = 20.0;
var lifeTime = 1.0;
var parBulletHit : GameObject;

private var counter = 0.0;

///////////////////////////////////////////////////////////////////////////////
// defines
///////////////////////////////////////////////////////////////////////////////

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

function Update () {
    counter += Time.deltaTime;
    if ( counter >= lifeTime ) {
        Destroy(gameObject);
    }
    transform.position = transform.position + speed * Time.deltaTime * transform.forward;
}

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

function OnCollisionEnter ( other : Collision ) {
    // Debug.Log("touch: " + other.gameObject.name );
    // TODO: play particles
    Instantiate(parBulletHit, transform.position, transform.rotation );
    Destroy(gameObject);
    // parBulletHit

    // HARDCODE { 
    var ai:ai_generic = other.collider.GetComponent("ai_generic");
    ai.HP -= 10.0;
    // } HARDCODE end 
}


