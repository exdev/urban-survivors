// ======================================================================================
// File         : ai.js
// Author       : Wu Jie 
// Last Change  : 07/25/2010 | 23:11:24 PM | Sunday,July
// Description  : 
// ======================================================================================

#pragma strict 

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

// DISABLE var btree : BehaveTree;

var HP = 100.0;

var move_speed = 1.0;
var rot_speed = 4.0;

var state_move = false;
var state_attack = false;

private var wanted_pos : Vector3; 
private var wanted_rot : Quaternion; 

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

function Start () {
    // DISABLE { 
    // btree = new BehaveTree("MyBehave");
    // btree.init( 
    //     BTSeq( [ BT_move() ] ) 
    // );
    // } DISABLE end 

    // init ai
    wanted_pos = transform.position;
    wanted_rot = transform.rotation;

    // GetRandomDest(2.0);
    GetPlayerPos(2.0);
}

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

function GetPlayerPos ( _tickTime : float ) {
    while ( true ) {
        var player = GameObject.FindWithTag("Player");
        wanted_pos = player.transform.position;
        yield WaitForSeconds (_tickTime);
    }
}

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

function GetRandomDest( _tickTime : float ) {
    while ( true ) {
        var delta = wanted_pos - transform.position;
        if ( delta.magnitude < 0.01 ) {
            wanted_pos = Vector3( 
                Random.Range(-10.0,10.0), 
                0.0,
                Random.Range(-10.0,10.0) 
            );
        }
        else {
            wanted_pos = Vector3( 
                Random.Range(-10.0,10.0), 
                0.0,
                Random.Range(-10.0,10.0) 
            );
            yield WaitForSeconds (_tickTime);
        }
    }
}

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

function Update () {
    // DISABLE { 
    // btree.tick();
    // } DISABLE end 

    // reset the state
    var player = GameObject.FindWithTag("Player");
    state_attack = false;
    state_move = false;

    // move ai
    var delta = player.transform.position - transform.position;
    if ( delta.magnitude >= 2.0 ) {
        // transform.position += delta.normalized * move_speed * Time.deltaTime;
        transform.position += transform.forward * move_speed * Time.deltaTime;

        state_move = true;
    }
    else {
        state_move = false;
        state_attack = true;
    }

    // rotate ai
    wanted_rot = Quaternion.LookRotation( player.transform.position - transform.position );
    wanted_rot.x = 0.0; wanted_rot.z = 0.0;
    transform.rotation = Quaternion.Slerp ( 
        transform.rotation, 
        wanted_rot, 
        rot_speed * Time.deltaTime
    );

    // material
    var r = transform.Find("zombieGirl").renderer;
    r.material.color.g = HP/100.0;
    r.material.color.b = HP/100.0;
    if ( HP <= 0.0 ) {
        Destroy(gameObject);
    }
}

