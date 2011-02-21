// ======================================================================================
// File         : Spawner_point.cs
// Author       : Wu Jie 
// Last Change  : 11/07/2010 | 19:33:56 PM | Sunday,November
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////
// defines
///////////////////////////////////////////////////////////////////////////////

public class Spawner_point : Spawner_base {

    public float radius = 0.0f;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public new void Awake () {
        base.Awake();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public override void DoSpawn () {
        // check if the character is too close to spawn point 
        Vector3 boyPos = Game.PlayerBoy().transform.position;
        Vector3 girlPos = Game.PlayerGirl().transform.position;
        float dist = Game.SpawnDistance();
        float sqrSpawnDistance = dist * dist;
        if ( (this.transform.position - boyPos).sqrMagnitude < sqrSpawnDistance ||
             (this.transform.position - girlPos).sqrMagnitude < sqrSpawnDistance )
            return;

        //
        int amount = calcSpawnAmount();
        Object obj;
        while ( amount > 0 ) {
            Vector2 offset = Random.insideUnitCircle * this.radius;
            obj = Instantiate( Prefab, 
                               transform.position + new Vector3( offset.x, 0.0f, offset.y ),
                               transform.rotation );
            GameObject go = obj as GameObject;
            AI_ZombieBase zb = go.GetComponent<AI_ZombieBase>();
            if (zb) {
                if ( zb.tag == "zombie_girl" )  {
                    zb.zombieInfo.curHP = zb_girl_hp;
                    zb.zombieInfo.maxHP = zb_girl_hp;
                    zb.zombieInfo.attack = zb_girl_attack;
                }
                else if ( zb.tag == "zombie_no1" ) {
                    zb.zombieInfo.curHP = zb_no1_hp;
                    zb.zombieInfo.maxHP = zb_no1_hp;
                    zb.zombieInfo.attack = zb_no1_attack;
                }
            }
            this.existObjects.Add(obj);
            --amount;
        }
    }
}
