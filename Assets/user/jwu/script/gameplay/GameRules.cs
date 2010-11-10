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
    protected GameObject startPoint = null;

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
            playerGirl = GameObject.FindWithTag("player.girl");

            //
            startPoint = GameObject.Find("StartPoint");
            if ( startPoint ) {
                PlacePlayerAtStartPoint();
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void PlacePlayerAtStartPoint () {
        Vector3 start_pos = startPoint.transform.position;
        Quaternion start_rot = startPoint.transform.rotation;

        GameObject boy = GameRules.Instance().GetPlayerBoy();
        if (boy) {
            boy.transform.position = start_pos;
            boy.transform.rotation = start_rot;
        }

        GameObject girl = GameRules.Instance().GetPlayerGirl();
        if (girl) {
            girl.transform.position = start_pos - boy.transform.forward * 2.0f;
            girl.transform.rotation = start_rot;
        }
        Camera.main.transform.position = new Vector3(start_pos.x, 20.0f, start_pos.z); 
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
