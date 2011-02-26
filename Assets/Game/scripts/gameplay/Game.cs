// ======================================================================================
// File         : Game.cs
// Author       : Wu Jie 
// Last Change  : 02/15/2011 | 16:10:26 PM | Tuesday,February
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
public class Game : MonoBehaviour {

    static Game instance  = null;

    PlayerBase playerBoy = null;
    PlayerBase playerGirl = null;
    bool isGameOver = false;
    float restartCounter = 0.0f;
    ScreenPad screenPad = null;

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public float RestartForSeconds = 5.0f; 
    public bool multiPlayer = false;
    public float enemyDeadFarAway = 10.0f;
    public float spawnDistance = 10.0f;

    public GameObject gameOver = null;
    public SpriteText restartCounterText = null;

    public StartPoint[] startPoints = null;
    public MissionBase CurrentMission = null;

    public GameObject hud_s = null;
    public GameObject hud_m = null;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Awake () {
        if( instance == null ) {
            instance = this;
            // check if we have main menu options, and apply the options.
            GameObject options = GameObject.Find("MainMenuOptions");
            if ( options ) {
                instance.multiPlayer = options.GetComponent<MainMenuOptions>().isMultiPlayer;
                Debug.Log("multi player = " + instance.multiPlayer );
                GameObject.Destroy(options);
            }

            //
            GameObject goBoy = GameObject.FindWithTag("player_boy");
            playerBoy = goBoy.GetComponent<PlayerBase>(); 
            GameObject goGirl = GameObject.FindWithTag("player_girl");
            playerGirl = goGirl.GetComponent<PlayerBase>();

            //
            if ( startPoints.Length != 0 ) {
                int i = (int)(Random.value * (startPoints.Length-1));
                PlacePlayerAtStartPoint(this.startPoints[i].transform);
            }
            else {
                Debug.LogError("Can't find start point");
            }


            // init hud
            if ( screenPad == null ) {
                GameObject hud = null;
                hud_m.SetActiveRecursively(false);
                hud_s.SetActiveRecursively(false);

                if ( Game.IsMultiPlayer() )
                    hud = hud_m;
                else
                    hud = hud_s;
                hud.SetActiveRecursively(true);

                if ( hud ) {
                    screenPad = hud.GetComponent<ScreenPad>();
                    screenPad.Init();
                }

#if UNITY_IPHONE
                if ( Application.isEditor == false ) {
                    DebugHelper.Assert( screenPad, "screenPad not found" );
                }
#endif
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    IEnumerator Start () {
        gameOver.SetActiveRecursively(false);

        // start the mission
        yield return StartCoroutine(CurrentMission.StartMission());
        UIStatus uistatus = screenPad.GetComponent<UIStatus>();
        yield return uistatus.StartCoroutine( uistatus.ShowControls (true, 1.0f) );
        yield return new WaitForSeconds(1.0f);
        screenPad.AcceptInput(true);
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
                this.restartCounterText.Text = string.Format( "{0:0}", Game.RestartCounter() );

            this.restartCounter -= Time.deltaTime;
            if ( this.restartCounter <= 0.0f )
                Application.LoadLevel(0);
        }

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

        // for DEBUG
        // DebugHelper.ScreenPrint("Total Enimies in the scene: " + this.GetEnemies().Count );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void PlacePlayerAtStartPoint ( Transform _startPoint ) {
        Vector3 start_pos = _startPoint.position;
        Quaternion start_rot = _startPoint.rotation;

        GameObject boy = Game.PlayerBoy().gameObject;
        if (boy) {
            boy.transform.position = start_pos;
            boy.transform.rotation = start_rot;
        }

        GameObject girl = Game.PlayerGirl().gameObject;
        if (girl) {
            girl.transform.position = start_pos - boy.transform.forward * 2.0f;
            girl.transform.rotation = start_rot;
        }
        Camera.main.transform.position = new Vector3(start_pos.x, 20.0f, start_pos.z); 
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public void PlayAsGod ( bool _asGod ) {
        PlayerBoy().AsGod(_asGod);
        PlayerGirl().AsGod(_asGod);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public IEnumerator Pause () {
        instance.screenPad.AcceptInput(false);
        UIStatus uistatus = instance.screenPad.GetComponent<UIStatus>();
        yield return uistatus.StartCoroutine( uistatus.ShowControls (false, 1.0f) );
        Time.timeScale = 0.0f;
    }

    static public IEnumerator Run () {
        UIStatus uistatus = instance.screenPad.GetComponent<UIStatus>();
        yield return uistatus.StartCoroutine( uistatus.ShowControls (true, 1.0f) );
        yield return instance.StartCoroutine ( CoroutineHelper.WaitForRealSeconds(1.0f) ); 
        instance.screenPad.AcceptInput(true);
        Time.timeScale = 1.0f;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public MissionBase Mission() { return instance.CurrentMission; }
    static public float SpawnDistance() { return instance.spawnDistance; }
    static public ScreenPad ScreenPad() { return instance.screenPad; }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public bool IsMultiPlayer () { return instance.multiPlayer; } 
    static public bool IsGameOver () { return instance.isGameOver; } 
    static public float RestartCounter () { return instance.restartCounter; } 

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public GameObject[] GetPlayers () {
        GameObject[] goList = new GameObject[2];
        goList[0] = instance.playerBoy.gameObject;
        goList[1] = instance.playerGirl.gameObject;
        return goList;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public PlayerBase PlayerBoy () { return instance.playerBoy; }
    static public PlayerBase PlayerGirl () { return instance.playerGirl; }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public List<GameObject> GetEnemies () { 
        List<GameObject> enemies = new List<GameObject>();
        enemies.AddRange ( GameObject.FindGameObjectsWithTag("zombie_girl") ); 
        enemies.AddRange ( GameObject.FindGameObjectsWithTag("zombie_no1") ); 
        return enemies;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public List<GameObject> GetEnemiesByTag ( string _tagName ) { 
        List<GameObject> enemies = new List<GameObject>();
        enemies.AddRange ( GameObject.FindGameObjectsWithTag(_tagName) ); 
        return enemies;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public void PickupBullets ( int _bullets ) {
        ShootInfo shootInfo = instance.playerGirl.GetShootInfo();
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

    static public void GetNearestPlayer ( Transform _self, out Transform _player, out float _dist ) { 
        Transform target = null;
        float nearest = 999.0f;

        GameObject[] players = Game.GetPlayers();
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

    static public void GetNearestAlivedPlayer ( Transform _self, out Transform _player, out float _dist ) { 
        Transform target = null;
        float nearest = 9999.0f;

        GameObject[] players = Game.GetPlayers();
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
}
