// ======================================================================================
// File         : ai_test.js
// Author       : Wu Jie 
// Last Change  : 09/18/2010 | 22:44:44 PM | Saturday,September
// Description  : 
// ======================================================================================

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

private var dest : Vector3; 
private var counter = 0.0;

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

function Start () {
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
            Random.Range(-20.0,20.0), 
            0.0,
            Random.Range(-20.0,20.0) 
        );
    }
}

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

function Update () {
    // movement
    GetRandomDest();
    var delta = dest - transform.position;
    if ( delta.magnitude > 1.0 ) {
        transform.position += delta.normalized * 20.0 * Time.deltaTime;
    }

    //
    var player = GameObject.FindWithTag("Player");
    var wanted_rot = Quaternion.LookRotation( player.transform.position - transform.position );
    transform.rotation = Quaternion.Slerp ( transform.rotation, wanted_rot, Time.deltaTime * 8.0 );
}

