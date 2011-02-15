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

public class ZombieKiller : MissionBase {

    ///////////////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////////////

    Vector3 startSceneInitPos;

    ///////////////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////////////

    [System.NonSerialized]
    [HideInInspector]
    public int CurrentCount = 0; 
    public int CompleteCount = 100; 
    public GameObject StartScene = null;
    public GameObject FinishScene = null;

    // mission text
    public SpriteText txtDeadZombeCounter = null;

    // StartScene
    public SpriteText txtZombieToKill = null;

    // FinishScene
    public SpriteText txtZombieKilled = null;
    public SpriteText txtTime = null;
    public SpriteText txtDamageTaken = null;
    public SpriteText txtBulletUsed = null;
    public SpriteText txtPerfectReloads = null;
    public SpriteText txtExpBounus = null;

    public SpriteText txtNext = null;
    public SpriteText txtMenu = null;

    //
    public float StartSceneShowForSeconds = 2.0f;

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
        this.txtDeadZombeCounter.gameObject.SetActiveRecursively(true);
        StartScene.SetActiveRecursively(false);
        FinishScene.SetActiveRecursively(false);
	}

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void ShowMissionComplete ( bool _show ) {
        FinishScene.SetActiveRecursively(_show);

        // // hide mission complete scene
        // this.txtMissionComplete.gameObject.SetActiveRecursively(_show);
        // this.txtZombieKilled.gameObject.SetActiveRecursively(_show);
        // this.txtTime.gameObject.SetActiveRecursively(_show);
        // this.txtDamageTaken.gameObject.SetActiveRecursively(_show);
        // this.txtBulletUsed.gameObject.SetActiveRecursively(_show);
        // this.txtPerfectReloads.gameObject.SetActiveRecursively(_show);
        // this.txtExpBounus.gameObject.SetActiveRecursively(_show);
        // this.txtNext.gameObject.SetActiveRecursively(_show);
        // this.txtMenu.gameObject.SetActiveRecursively(_show);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	public override IEnumerator StartMission () {
        StartScene.transform.position = startSceneInitPos; 
        StartScene.SetActiveRecursively(true);
        txtZombieToKill.Text = "kill " + this.CompleteCount + " zombies !!!";

        Vector3 from = startSceneInitPos + new Vector3( -1000.0f, 0.0f, 0.0f ); 
        Vector3 to = startSceneInitPos + new Vector3( 1000.0f, 0.0f, 0.0f ); 

        iTween.MoveFrom ( this.StartScene, iTween.Hash( "position", from, 
                                                        "time", 1.0f,
                                                        "easetype", iTween.EaseType.easeOutBack 
                                                      ) );
        yield return new WaitForSeconds(this.StartSceneShowForSeconds); 
        iTween.MoveTo ( this.StartScene, iTween.Hash( "position", to, 
                                                      "time", 1.0f,
                                                      "easetype", iTween.EaseType.easeOutBack, 
                                                      "oncomplete", "HideStartScene",
                                                      "oncompletetarget", this.gameObject
                                                    ) );
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

        if ( this.CurrentCount >= this.CompleteCount ) {
            // TODO { 
            // Time.timeScale = 0.0f;
            // ShowMissionComplete(true);
            // } TODO end 
        }
	}

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnZombieDead () { this.CurrentCount++; }
}
