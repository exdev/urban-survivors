// ======================================================================================
// File         : spawn.js
// Author       : Wu Jie 
// Last Change  : 09/18/2010 | 19:39:50 PM | Saturday,September
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// properties
///////////////////////////////////////////////////////////////////////////////

var zombie_prototype : GameObject;
var max_zombies = 20;

///////////////////////////////////////////////////////////////////////////////
// defines
///////////////////////////////////////////////////////////////////////////////

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

function Awake () {
    for ( i = 0; i < max_zombies; ++i ) {
        var rot = transform.rotation;
        rot.eulerAngles.y += Random.Range(-180, 180);

        var pos = Vector3 ( 
            Random.Range(-20.0, 20.0), 
            0.0,
            Random.Range(-20.0, 20.0) 
        );
        var zombie = Instantiate(zombie_prototype, pos, rot );
    }
}

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

function Update () {
    var zombies = GameObject.FindGameObjectsWithTag ("Zombie");
    var count = max_zombies - zombies.length;
    for ( i = 0; i < count; ++i ) {
        var rot = transform.rotation;
        rot.eulerAngles.y += Random.Range(-180, 180);

        var pos = Vector3 ( 
            Random.Range(-20.0, 20.0), 
            0.0,
            Random.Range(-20.0, 20.0) 
        );
        Instantiate(zombie_prototype, pos, rot );
    }
}

