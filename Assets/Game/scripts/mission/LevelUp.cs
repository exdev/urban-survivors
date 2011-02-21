// ======================================================================================
// File         : LevelUp.cs
// Author       : Wu Jie 
// Last Change  : 02/19/2011 | 00:39:52 AM | Saturday,February
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
// class LevelUp
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

public class LevelUp : MonoBehaviour {
    public GameObject[] targets; 
    public int max_alive_up = 1;
    public float zb_girl_hp_up = 1.0f;
    public float zb_girl_attack_up = 1.0f;
    public float zb_no1_hp_up = 1.0f;
    public float zb_no1_attack_up = 1.0f;
    public int killed_count_up = 10;
    public float hpRecoverAmount = 999;

    void OnLevelUp () {
        Debug.Log("level up");

        if ( killed_count_up > 0 ) {
            Game.Mission().SendMessage ( "IncreaseCompleteCount", killed_count_up );
        }

        foreach ( GameObject go in targets ) {
            if ( max_alive_up > 0 ) {
                go.SendMessage( "IncreaseMaxAlive", max_alive_up );
            }
            if ( zb_girl_hp_up > 0.0f ) {
                go.SendMessage( "IncreaseHp_zb_girl", zb_girl_hp_up );
            }
            if ( zb_girl_attack_up > 0.0f ) {
                go.SendMessage( "IncreaseAttack_zb_girl", zb_girl_attack_up );
            }
            if ( zb_no1_hp_up > 0.0f ) {
                go.SendMessage( "IncreaseHp_zb_no1", zb_no1_hp_up );
            }
            if ( zb_no1_attack_up > 0.0f ) {
                go.SendMessage( "IncreaseAttack_zb_no1", zb_no1_attack_up );
            }
        }

        Game.PlayerBoy().SendMessage( "OnRecover", hpRecoverAmount );
        Game.PlayerGirl().SendMessage( "OnRecover", hpRecoverAmount );
    }
}
