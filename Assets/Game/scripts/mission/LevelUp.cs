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
    public float hp_up = 1.0f;
    public float attack_up = 1.0f;
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
            if ( hp_up > 0.0f ) {
                go.SendMessage( "IncreaseZombieHp", hp_up );
            }
            if ( attack_up > 0.0f ) {
                go.SendMessage( "IncreaseZombieAttack", attack_up );
            }
        }

        Game.PlayerBoy().SendMessage( "OnRecover", hpRecoverAmount );
        Game.PlayerGirl().SendMessage( "OnRecover", hpRecoverAmount );
    }
}
