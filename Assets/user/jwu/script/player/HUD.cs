// ======================================================================================
// File         : HUD.cs
// Author       : Wu Jie 
// Last Change  : 10/20/2010 | 00:42:23 AM | Wednesday,October
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////
// class defines
///////////////////////////////////////////////////////////////////////////////

public class HUD : MonoBehaviour {

    public GameObject playerBoy;
    public GameObject playerGirl;

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    private GameObject ui_boy_HP;
    private GameObject ui_girl_HP;

    ///////////////////////////////////////////////////////////////////////////////
    // functions defines
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	void Start () {
        ui_boy_HP = transform.Find("dyn_HP_boy").gameObject;
        DebugHelper.Assert( ui_boy_HP != null, "can't find dyn_HP_boy" );

        ui_girl_HP = transform.Find("dyn_HP_girl").gameObject;
        DebugHelper.Assert( ui_girl_HP != null, "can't find child dyn_HP_girl" );
	}
	
    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	void Update () {
        UIProgressBar hpProgressBar = null;
        Player_base player_info = null;

        // handle boy
        hpProgressBar = ui_boy_HP.GetComponent(typeof(UIProgressBar)) as UIProgressBar;
        if ( hpProgressBar ) {
            player_info = playerBoy.GetComponent(typeof(Player_base)) as Player_base;
            hpProgressBar.Value = player_info.GetHP();
        }

        // handle girl
        hpProgressBar = ui_girl_HP.GetComponent(typeof(UIProgressBar)) as UIProgressBar;
        if ( hpProgressBar ) {
            player_info = playerGirl.GetComponent(typeof(Player_base)) as Player_base;
            hpProgressBar.Value = player_info.GetHP();
        }
	}
}
