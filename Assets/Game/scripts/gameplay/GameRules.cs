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
    public float enemyDeadFarAway = 10.0f;

    public GameObject gameOver = null;
    public SpriteText restartCounterText = null;
    public SpriteText deadZombeCounter = null;

    protected static GameRules instance  = null;

    protected PlayerBase playerBoy = null;
    protected PlayerBase playerGirl = null;
    protected GameObject startPoint = null;
    protected bool isGameOver = false;
    protected float restartCounter = 0.0f;
    protected int deadZombies = 0;

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
            playerBoy = goBoy.GetComponent<PlayerBase>(); 
            GameObject goGirl = GameObject.FindWithTag("player_girl");
            playerGirl = goGirl.GetComponent<PlayerBase>();
            gameOver.SetActiveRecursively(false);

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
            if ( this.playerBoy.noHP() && this.playerGirl.noHP() ) {
                this.isGameOver = true;
                this.playerBoy.StopCoroutine("WaitForRecover");
                this.playerGirl.StopCoroutine("WaitForRecover");
                this.restartCounter = this.RestartForSeconds; 
                this.gameOver.SetActiveRecursively(true);
            }
        }
        else {
            if ( this.restartCounterText )
                this.restartCounterText.Text = string.Format( "{0:0}", GameRules.Instance().RestartCounter() );

            this.restartCounter -= Time.deltaTime;
            if ( this.restartCounter <= 0.0f )
                Application.LoadLevel(0);
        }

        if ( this.deadZombeCounter )
            this.deadZombeCounter.Text = "dead zombies: " + this.deadZombies;

        // kill enemies when far away than 
        List<GameObject> enemies = GetEnemies ();
        Vector3 boyPos = playerBoy.transform.position;
        Vector3 girlPos = playerGirl.transform.position;

        foreach ( GameObject go in enemies ) {
            if ( (go.transform.position - boyPos).magnitude >= enemyDeadFarAway ||
                 (go.transform.position - girlPos).magnitude >= enemyDeadFarAway ) 
            {
                Destroy(go);
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

        GameObject boy = GameRules.Instance().GetPlayerBoy().gameObject;
        if (boy) {
            boy.transform.position = start_pos;
            boy.transform.rotation = start_rot;
        }

        GameObject girl = GameRules.Instance().GetPlayerGirl().gameObject;
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

    public PlayerBase GetPlayerBoy () { return playerBoy; }
    public PlayerInfo GetPlayerBoyInfo () { return playerBoy.playerInfo; }

    public PlayerBase GetPlayerGirl () { return playerGirl; }
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

    public void PickupBullets ( int _bullets ) {
        ShootInfo shootInfo = playerGirl.GetShootInfo();
        shootInfo.AddBullets(_bullets);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    // DELME { 
    // public void PlayerRecover ( float _amount ) {
    //     // TODO: I think recover girl first would be a better way.

    //     PlayerInfo boyInfo = playerBoy.playerInfo;
    //     PlayerInfo girlInfo = playerGirl.playerInfo;

    //     float hpLoseBoy = boyInfo.maxHP - boyInfo.curHP;
    //     float hpLoseGirl = girlInfo.maxHP - girlInfo.curHP;

    //     if ( playerGirl.IsDown() || hpLoseGirl > hpLoseBoy ) {
    //         playerGirl.Recover(_amount);
    //         float hpLeft = hpLoseGirl - _amount;
    //         if ( hpLeft > 0.0f )
    //             playerBoy.Recover(hpLeft);
    //     }
    //     else {
    //         playerBoy.Recover(_amount);
    //         float hpLeft = hpLoseBoy - _amount;
    //         if ( hpLeft > 0.0f )
    //             playerGirl.Recover(hpLeft);
    //     }
    // }
    // } DELME end 

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
            PlayerBase p = player.GetComponent<PlayerBase>();
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

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void CountDeadZombie () { this.deadZombies++; }
}
