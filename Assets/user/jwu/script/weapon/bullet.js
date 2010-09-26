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
static var parBulletHit_inst:GameObject;

///////////////////////////////////////////////////////////////////////////////
// defines
///////////////////////////////////////////////////////////////////////////////

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

function Awake () {
    if ( parBulletHit_inst == null ) {
        parBulletHit_inst = Instantiate(parBulletHit, transform.position, transform.rotation );
    }
}

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

function Start () {
    counter = 0.0;
}

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

function Update () {
    counter += Time.deltaTime;
    if ( counter >= lifeTime ) {
        SpawnManager.Instance().Destroy(gameObject);
    }
    transform.position = transform.position + speed * Time.deltaTime * transform.forward;
}

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

function OnCollisionEnter ( other : Collision ) {
    // DEBUG { 
    // Debug.Log("touch: " + other.gameObject.name );
    // } DEBUG end 

    // play particles
    if ( parBulletHit_inst != null ) {
        parBulletHit_inst.transform.position = transform.position;
        parBulletHit_inst.transform.rotation = transform.rotation;
        parBulletHit_inst.particleEmitter.Emit();
    }
    else {
        Debug.Log( "warning: the particle instance not instantiate!" );
    }

    // HARDCODE { 
    // it is possible the collider already destroied 
    if ( other.collider != null ) {
        var ai:ai_generic = other.collider.GetComponent("ai_generic");
        if ( ai != null ) {
            ai.HP -= 10.0;
        }
    }
    // } HARDCODE end 

    // destroy bullet
    SpawnManager.Instance().Destroy(gameObject);
}


