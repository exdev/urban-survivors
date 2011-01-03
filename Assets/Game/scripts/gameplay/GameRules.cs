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

    public float RestartForSeconds = 5.0f; 
    public bool multiPlayer = false;

    protected static GameRules instance  = null;
    protected Player_base playerBoy = null;
    protected Player_base playerGirl = null;
    protected GameObject startPoint = null;
    protected bool isGameOver = false;
    protected float restartCounter = 0.0f;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public static GameRules Instance() { return instance; }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Awake () {
        if( instance == null ) {
            instance = this;

            //
            GameObject goBoy = GameObject.FindWithTag("player_boy");
            playerBoy = goBoy.GetComponent<Player_base>(); 
            GameObject goGirl = GameObject.FindWithTag("player_girl");
            playerGirl = goGirl.GetComponent<Player_base>();

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

    void Update () {
        // check if we got game over status
        if ( this.isGameOver == false ) {
            if ( this.playerBoy.IsDown() && this.playerGirl.IsDown() ) {
                this.isGameOver = true;
                this.restartCounter = this.RestartForSeconds; 
            }
        }
        else {
            this.restartCounter -= Time.deltaTime;
            if ( this.restartCounter <= 0.0f ) {
                Application.LoadLevel(0);
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public bool IsMultiPlayer () { return this.multiPlayer; } 
    public bool IsGameOver () { return this.isGameOver; } 
    public float RestartCounter () { return this.restartCounter; } 

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
        goList[0] = playerBoy.gameObject;
        goList[1] = playerGirl.gameObject;
        return goList;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public GameObject GetPlayerBoy () { return playerBoy.gameObject; }
    public PlayerInfo GetPlayerBoyInfo () { return playerBoy.playerInfo; }

    public GameObject GetPlayerGirl () { return playerGirl.gameObject; }
    public PlayerInfo GetPlayerGirlInfo () { return playerGirl.playerInfo; }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public List<GameObject> GetEnemies () { 
        List<GameObject> enemies = new List<GameObject>();
        enemies.AddRange ( GameObject.FindGameObjectsWithTag("zombie_girl") ); 
        enemies.AddRange ( GameObject.FindGameObjectsWithTag("zombie_no1") ); 
        return enemies;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public List<GameObject> GetEnemiesByTag ( string _tagName ) { 
        List<GameObject> enemies = new List<GameObject>();
        enemies.AddRange ( GameObject.FindGameObjectsWithTag(_tagName) ); 
        return enemies;
    }


    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void GetNearestPlayer ( Transform _self, out Transform _player, out float _dist ) { 
        Transform target = null;
        float nearest = 999.0f;

        GameObject[] players = GameRules.Instance().GetPlayers();
        foreach( GameObject player in players ) {
            float len = (player.transform.position - _self.position).magnitude;
            if ( len < nearest ) {
                nearest = len;
                target = player.transform;
            }
        }

        _player = target;
        _dist = nearest;
    }


    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void GetNearestAlivedPlayer ( Transform _self, out Transform _player, out float _dist ) { 
        Transform target = null;
        float nearest = 9999.0f;

        GameObject[] players = GameRules.Instance().GetPlayers();
        foreach( GameObject player in players ) {
            Player_base p = player.GetComponent<Player_base>();
            if ( p.playerInfo.curHP <= 0.0f ) {
                continue;
            }

            float len = (player.transform.position - _self.position).magnitude;
            if ( len < nearest ) {
                nearest = len;
                target = player.transform;
            }
        }

        _player = target;
        _dist = nearest;
    }
}
