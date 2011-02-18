// ======================================================================================
// File         : MissionLevel.cs
// Author       : Wu Jie 
// Last Change  : 02/18/2011 | 15:55:23 PM | Friday,February
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

///////////////////////////////////////////////////////////////////////////////
// class MissionLevel
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

public class MissionLevel : MonoBehaviour {

    int currentLevel = 0;

    ///////////////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////////////

    public class LevelUpTemplate {
        public string name = "unknown";
        public GameObject[] targets = new GameObject[0];
        public int max_alive_up = 1;
        public float hp_up = 1.0f;
        public float attack_up = 1.0f;
    }
    public class LevelUpBehavior {
        public LevelUpTemplate tmpl;
    }

    public List<LevelUpTemplate> templates = new List<LevelUpTemplate>();
    public LevelUpBehavior[] levels = new LevelUpBehavior[0];


    ///////////////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////////////

    void Awake () {
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnMissionLevelUp () {
        ++currentLevel;

        // TODO:
        // LevelUpBehavior[currentLevel]
    }
}
