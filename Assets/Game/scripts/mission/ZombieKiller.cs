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

public class ZombieKiller : MonoBehaviour {

    [System.NonSerialized]
    [HideInInspector]
    public int CurrentCount = 0; 
    public int CompleteCount = 100; 

    public SpriteText txtDeadZombeCounter = null;

    public SpriteText txtMissionComplete = null;

    public SpriteText txtZombieKilled = null;
    public SpriteText txtTime = null;
    public SpriteText txtDamageTaken = null;
    public SpriteText txtBulletUsed = null;
    public SpriteText txtPerfectReloads = null;
    public SpriteText txtExpBounus = null;

    public SpriteText txtNext = null;
    public SpriteText txtMenu = null;

    ///////////////////////////////////////////////////////////////////////////////
    // function defines
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	void Start () {
        this.txtDeadZombeCounter.gameObject.SetActiveRecursively(true);
        ShowMissionComplete(false);
	}

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void ShowMissionComplete ( bool _show ) {
        // hide mission complete scene
        this.txtMissionComplete.gameObject.SetActiveRecursively(_show);
        this.txtZombieKilled.gameObject.SetActiveRecursively(_show);
        this.txtTime.gameObject.SetActiveRecursively(_show);
        this.txtDamageTaken.gameObject.SetActiveRecursively(_show);
        this.txtBulletUsed.gameObject.SetActiveRecursively(_show);
        this.txtPerfectReloads.gameObject.SetActiveRecursively(_show);
        this.txtExpBounus.gameObject.SetActiveRecursively(_show);
        this.txtNext.gameObject.SetActiveRecursively(_show);
        this.txtMenu.gameObject.SetActiveRecursively(_show);
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
