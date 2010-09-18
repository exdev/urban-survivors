// ======================================================================================
// File         : ai.js
// Author       : Wu Jie 
// Last Change  : 07/25/2010 | 23:11:24 PM | Sunday,July
// Description  : 
// ======================================================================================

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

var btree : BehaveTree;

private var dest : Vector3; 
private var counter = 0.0;

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

    // Debug.Log( "var1 = " + btree.myvar + ", var2 = " + btree.myvar2 );

    dest = transform.position;
    counter = Time.time;
}

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

function GetRandomDest () {
    if ( Time.time - counter > 2.0 ) {
        counter = Time.time;
        dest = Vector3( 
            Random.Range(-10.0,10.0), 
            0.0,
            Random.Range(-10.0,10.0) 
        );
    }
}

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

function Update () {
    // DISABLE { 
    // btree.tick();
    // } DISABLE end 

    // move player
    GetRandomDest();
    var delta = dest - transform.position;
    if ( delta.magnitude > 1.0 ) {
        transform.position += delta.normalized * 10.0 * Time.deltaTime;
    }

    //
    var player = GameObject.FindWithTag("Player");
    var wanted_rot = Quaternion.LookRotation( player.transform.position - transform.position );
    transform.rotation = Quaternion.Slerp ( transform.rotation, wanted_rot, Time.deltaTime * 4.0 );
    var anim = transform.GetComponentInChildren(Animation);
    anim.CrossFade("moveForward");
}

