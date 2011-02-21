// ======================================================================================
// File         : Spawner_base.cs
// Author       : Wu Jie 
// Last Change  : 11/07/2010 | 19:28:22 PM | Sunday,November
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

///////////////////////////////////////////////////////////////////////////////
// defines
///////////////////////////////////////////////////////////////////////////////

public class Spawner_base : MonoBehaviour {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public GameObject Prefab;
    public int totalAmount = -1;
    public int minAlive = 1; // The minimum number of pawns that will be spawned every time the code execute.
    public int maxAlive = 1; // The maximum number of pawns that will be spawned every time the code execute.
    public bool showSpawns = false; 

    [System.NonSerialized]
    [HideInInspector]
    public List<Object> existObjects = new List<Object>();

    protected float zb_girl_hp = 0.0f;
    protected float zb_girl_attack = 0.0f;

    protected float zb_no1_hp = 0.0f;
    protected float zb_no1_attack = 0.0f;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void Awake () {
        AI_ZombieBase zb = Prefab.GetComponent<AI_ZombieBase>();
        if ( zb ) {
            if ( zb.tag == "zombie_girl" )  {
                zb_girl_hp = zb.zombieInfo.maxHP;
                zb_girl_attack = zb.zombieInfo.attack;
            }
            else if ( zb.tag == "zombie_no1" ) {
                zb_no1_hp = zb.zombieInfo.maxHP;
                zb_no1_attack = zb.zombieInfo.attack;
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public virtual void DoSpawn () {}
    public void DoKill () {
        foreach ( Object obj in existObjects ) {
            Destroy(obj);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public int calcSpawnAmount () {
        // never exec the code when we reach the limitation.
        if ( totalAmount == 0 )
            return 0;

        for ( int i = 0; i < existObjects.Count; ++i ) {
            Object obj = existObjects[i];
            if ( obj == null ) {
                existObjects.RemoveAt(i);
                --i;
            }
        }

        if ( existObjects.Count < maxAlive ) {
            int remain = maxAlive - existObjects.Count; 
            int amount = Random.Range( minAlive, remain );
            if ( totalAmount != -1 ) {
                amount = Mathf.Min(totalAmount,amount);
                totalAmount = totalAmount - amount;
            }
            return amount;
        }

        return 0;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void IncreaseMaxAlive ( int _amount ) {
        maxAlive += _amount;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void IncreaseHp_zb_girl ( float _amount ) { zb_girl_hp += _amount; }
    void IncreaseAttack_zb_girl ( float _amount ) { zb_girl_attack += _amount; }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void IncreaseHp_zb_no1 ( float _amount ) { zb_no1_hp += _amount; }
    void IncreaseAttack_zb_no1 ( float _amount ) { zb_no1_attack += _amount; }
}
