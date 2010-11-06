// ======================================================================================
// File         : GameRules.cs
// Author       : Wu Jie 
// Last Change  : 10/29/2010 | 01:08:32 AM | Friday,October
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

[System.Serializable]
public class GameRules : MonoBehaviour {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////////
    // members
    ///////////////////////////////////////////////////////////////////////////////

    protected static GameRules instance  = null;
    protected GameObject playerBoy = null;
    protected GameObject playerGirl = null;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public static GameRules Instance() {
        return instance;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Awake () {
        if( instance == null ) {
            instance = this;

            //
            playerBoy = GameObject.FindWithTag("player.boy");
            DebugHelper.Assert(playerBoy,"can't find palyer.boy in the scene");
            playerGirl = GameObject.FindWithTag("player.girl");
            DebugHelper.Assert(playerGirl,"can't find palyer.girl in the scene");
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public GameObject[] GetPlayers () {
        GameObject[] goList = new GameObject[2];
        goList[0] = playerBoy;
        goList[1] = playerGirl;
        return goList;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public GameObject GetPlayerBoy () { return playerBoy; }
    public GameObject GetPlayerGirl () { return playerGirl; }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public List<GameObject> GetEnemies () { 
        List<GameObject> enemies = new List<GameObject>();
        enemies.AddRange ( GameObject.FindGameObjectsWithTag("zombie") ); 
        return enemies;
    }
}