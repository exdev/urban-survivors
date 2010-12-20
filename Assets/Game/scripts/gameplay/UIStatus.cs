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

    protected UIProgressBar boyProgressBar = null;
    protected UIProgressBar girlProgressBar = null;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Awake () {
        this.boyProgressBar = boyHpBar ? boyHpBar.GetComponent<UIProgressBar>() : null;
        this.girlProgressBar = girlHpBar ? girlHpBar.GetComponent<UIProgressBar>() : null;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Update () {
        PlayerInfo boyInfo = GameRules.Instance().GetPlayerBoyInfo();
        this.boyProgressBar.Value = 1.0f - boyInfo.curHP/boyInfo.maxHP;

        PlayerInfo girlInfo = GameRules.Instance().GetPlayerGirlInfo();
        this.girlProgressBar.Value = girlInfo.curHP/girlInfo.maxHP;
    }
}
