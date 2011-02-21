// ======================================================================================
// File         : ZombieKiller.cs
// Author       : Wu Jie 
// Last Change  : 02/14/2011 | 16:44:34 PM | Monday,February
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

///////////////////////////////////////////////////////////////////////////////
// class ZombieKiller
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

public class ZombieKiller : MissionBase {

    ///////////////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////////////

    delegate void StateUpdate();
    StateUpdate State;

    Vector3 startSceneInitPos;
    float missionReportStartTime;

    float timeMissionStart = 0.0f;
    float timeMissionFinished = 0.0f;

    int CurrentCount = 0; 
    float DamageTaken = 0.0f;
    int BulletUsed = 0;
    int PerfectReloadCount = 0;

    ///////////////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////////////

    public int CompleteCount = 100; 
    public GameObject StartScene = null;
    public GameObject FinishScene = null;

    // mission text
    public PackedSprite zombieFace = null;
    public SpriteText txtDeadZombeCounter = null;

    // StartScene
    public SpriteText txtStage = null;
    public SpriteText txtZombieToKill = null;

    // FinishScene
    public SpriteText txtZombieKilled = null;
    public SpriteText txtTime = null;
    public SpriteText txtDamageTaken = null;
    public SpriteText txtBulletUsed = null;
    public SpriteText txtPerfectReloads = null;
    public SpriteText txtExpBounus = null;

    public UIButton btnNext = null;
    public UIButton btnMenu = null;

    //
    public float StartSceneShowForSeconds = 2.0f;
    public float MissionReportForSeconds = 2.0f;

    ///////////////////////////////////////////////////////////////////////////////
    // function defines
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Awake () {
        startSceneInitPos = StartScene.transform.position;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	void Start () {
        ResetMission();
	}

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void ResetMission () {
        this.zombieFace.gameObject.SetActiveRecursively(false);
        this.txtDeadZombeCounter.gameObject.SetActiveRecursively(false);
        StartScene.SetActiveRecursively(false);
        FinishScene.SetActiveRecursively(false);
        
        this.CurrentCount = 0;
        this.DamageTaken = 0.0f;
        this.BulletUsed = 0;
        this.PerfectReloadCount = 0;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	public override IEnumerator StartMission () {
        ResetMission();
        Game.PlayAsGod(false);
        StartCoroutine(Game.Pause());

        StartScene.transform.position = startSceneInitPos; 
        StartScene.SetActiveRecursively(true);
        int levelCount = this.GetComponent<MissionLevels>().CurrentLevel();
        txtStage.Text = "stage " + levelCount;
        txtZombieToKill.Text = "kill " + this.CompleteCount + " zombies !!!";

        Vector3 from = startSceneInitPos + new Vector3( -1000.0f, 0.0f, 0.0f ); 
        Vector3 to = startSceneInitPos + new Vector3( 1000.0f, 0.0f, 0.0f ); 

        iTween.MoveFrom ( this.StartScene, iTween.Hash( "position", from, 
                                                        "time", 1.0f,
                                                        "easetype", iTween.EaseType.easeOutBack, 
                                                        "ignoretimescale", true
                                                      ) );
        yield return StartCoroutine ( CoroutineHelper.WaitForRealSeconds(this.StartSceneShowForSeconds+1.0f) ); 
        iTween.MoveTo ( this.StartScene, iTween.Hash( "position", to, 
                                                      "time", 1.0f,
                                                      "easetype", iTween.EaseType.easeOutBack, 
                                                      "ignoretimescale", true,
                                                      "oncomplete", "HideStartScene",
                                                      "oncompletetarget", this.gameObject
                                                    ) );

        yield return StartCoroutine(Game.Run());

        this.zombieFace.gameObject.SetActiveRecursively(true);
        this.txtDeadZombeCounter.gameObject.SetActiveRecursively(true);
        this.txtDeadZombeCounter.Text = "dead zombies: " + this.CurrentCount + "/" + this.CompleteCount;
        iTween.ScaleFrom ( this.zombieFace.gameObject, new Vector3( 2.0f, 2.0f, 2.0f ), 0.6f ); 
        iTween.ScaleFrom ( this.txtDeadZombeCounter.gameObject, new Vector3( 2.0f, 2.0f, 2.0f ), 0.6f ); 

        timeMissionStart = Time.time;
        State = UpdateMission;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void HideStartScene () {
        StartScene.SetActiveRecursively(false);
    }
	
    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	void Update () {
        if ( this.txtDeadZombeCounter ) {
            this.txtDeadZombeCounter.Text = 
                "dead zombies: " + this.CurrentCount + "/" + this.CompleteCount;
        }

        if ( State != null )
            State();
	}

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void UpdateMission () {
        // don't do anything if game over
        if ( Game.IsGameOver() )
            return;

        if ( this.CurrentCount >= this.CompleteCount ) {
            State = null;
            StartCoroutine(OnMissionComplete());
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void UpdateMissionReport () {
        float delta = Time.realtimeSinceStartup - missionReportStartTime;
        if ( delta >= MissionReportForSeconds ) {
            State = WaitForUserInput;
        }

        // we use 0.5 seconds to refresh
        float ratio = Mathf.Min( 1.0f, delta/0.5f );

        // report list
        this.txtZombieKilled.Text = "zombie killed: " + (int)(this.CurrentCount * ratio);
        this.txtTime.Text = "time: " 
            + string.Format( "{0:0}", (timeMissionFinished - timeMissionStart) * ratio )
            + " sec";
        this.txtDamageTaken.Text = "damage taken: " 
            + string.Format( "{0:0}", this.DamageTaken * ratio );
        this.txtBulletUsed.Text = "bullet used: " + (int)(this.BulletUsed * ratio);
        this.txtPerfectReloads.Text = "perfect reloads: " + (int)(this.PerfectReloadCount * ratio);
        // TODO { 
        // this.txtExpBounus.Text = "EXP Bounus: " + ;
        // } TODO end 
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void WaitForUserInput () {
        // NOTE: it will wait for OnNextMission

        // bool hasTouched = false;
        // if ( Game.ScreenPad().useRemoteTouch ) {
        //     foreach ( Touch t in Input.touches ) {
        //         if ( t.phase == TouchPhase.Began ) {
        //             hasTouched = true;
        //             break;
        //         }
        //     }
        // }
        // else {
        //     hasTouched = Input.GetButton("Fire"); 
        // }

        // //
        // if ( hasTouched ) {
        //     State = null;
        //     StartCoroutine(this.StartMission());
        // }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    IEnumerator OnMissionComplete() {
        Game.PlayAsGod(true);
        Time.timeScale = 0.2f;
        yield return StartCoroutine ( CoroutineHelper.WaitForRealSeconds(2.0f) ); 

        // pause the game
        StartCoroutine(Game.Pause());

        // go to mission report state
        this.zombieFace.gameObject.SetActiveRecursively(false);
        this.txtDeadZombeCounter.gameObject.SetActiveRecursively(false);
        FinishScene.SetActiveRecursively(true);
        missionReportStartTime = Time.realtimeSinceStartup;
        timeMissionFinished = Time.time;
        State = UpdateMissionReport;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnNextMission () {
        State = null;
        SendMessage("OnMissionLevelUp");
        StartCoroutine(this.StartMission());
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnMainMenu () {
        Time.timeScale = 1.0f;
        Application.LoadLevel("mainMenu");
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnZombieDead () { 
        this.CurrentCount++; 
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnPlayerHit ( float _dmg ) {
        this.DamageTaken += _dmg;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnBulletUsed ( int _count ) {
        this.BulletUsed += _count;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnPerfectReload () {
        this.PerfectReloadCount++;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void IncreaseCompleteCount ( int _amount ) {
        this.CompleteCount += _amount;
    }
}
