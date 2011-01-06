// ======================================================================================
// File         : UIStatus.cs
// Author       : Wu Jie 
// Last Change  : 12/20/2010 | 21:02:48 PM | Monday,December
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
// class UIStatus 
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

public class UIStatus : MonoBehaviour {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public GameObject boyHpBar = null;
    public GameObject girlHpBar = null;
    public GameObject gameOver = null;
    public GameObject restartCounter = null;
    public GameObject bulletCounter = null;

    protected UIProgressBar boyProgressBar = null;
    protected UIProgressBar girlProgressBar = null;
    protected TextMesh restartCounterText = null;
    protected TextMesh bulletCounterText = null;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Awake () {
        this.boyProgressBar = boyHpBar ? boyHpBar.GetComponent<UIProgressBar>() : null;
        this.girlProgressBar = girlHpBar ? girlHpBar.GetComponent<UIProgressBar>() : null;
        this.restartCounterText = restartCounter ? restartCounter.GetComponent<TextMesh>() : null;
        this.bulletCounterText = bulletCounter ? bulletCounter.GetComponent<TextMesh>() : null;
        gameOver.SetActiveRecursively(false);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Update () {
        PlayerInfo boyInfo = GameRules.Instance().GetPlayerBoyInfo();
        this.boyProgressBar.Value = boyInfo.curHP/boyInfo.maxHP;

        PlayerInfo girlInfo = GameRules.Instance().GetPlayerGirlInfo();
        this.girlProgressBar.Value = 1.0f - girlInfo.curHP/girlInfo.maxHP;

        if ( GameRules.Instance().IsGameOver() ) {
            gameOver.SetActiveRecursively(true);
            if ( this.restartCounterText )
                this.restartCounterText.text = string.Format( "{0:0}", GameRules.Instance().RestartCounter() );
        }

        // update bullets
        if ( this.bulletCounterText ) {
            Player_base girl = GameRules.Instance().GetPlayerGirl();
            ShootInfo shootInfo = girl.GetShootInfo();

            // curbullet / totalbullet
            this.bulletCounterText.text = shootInfo.CurBullets() + "/" + shootInfo.TotalBullets();
        }
    }
}
