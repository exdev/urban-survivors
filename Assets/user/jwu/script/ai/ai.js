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
}

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

function Update () {
    // DISABLE { 
    // btree.tick();
    // } DISABLE end 

    // move towards player

    // transform.position.x += Mathf.Cos(Time.time) * 10.0 * Time.deltaTime;
    // transform.position.z += Mathf.Sin(Time.time) * 10.0 * Time.deltaTime;

    // var wanted_rot = Quaternion.LookRotation( target.position - transform.position );
    // transform.rotation = Quaternion.Slerp ( transform.rotation, wanted_rot, Time.deltaTime * 4.0 );
}

