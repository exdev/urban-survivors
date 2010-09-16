// ======================================================================================
// File         : ai.js
// Author       : Wu Jie 
// Last Change  : 07/25/2010 | 23:11:24 PM | Sunday,July
// Description  : 
// ======================================================================================

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

var target: Transform;
var btree : BehaveTree;

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

function Start () {
    btree = new BehaveTree("MyBehave");
    btree.init( BTSeq( [BTCond("is walk"), BTAct("hello world") ] ) );
    // Debug.Log( "var1 = " + btree.myvar + ", var2 = " + btree.myvar2 );
}

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

function Update () {
    // transform.position.x += Mathf.Cos(Time.time) * 10.0 * Time.deltaTime;
    // transform.position.z += Mathf.Sin(Time.time) * 10.0 * Time.deltaTime;
    var wanted_rot = Quaternion.LookRotation( target.position - transform.position );
    transform.rotation = Quaternion.Slerp ( transform.rotation, wanted_rot, Time.deltaTime * 4.0 );
    btree.tick();
}

