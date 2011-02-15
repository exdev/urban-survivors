// ======================================================================================
// File         : Constant.cs
// Author       : Wu Jie 
// Last Change  : 09/27/2010 | 22:24:44 PM | Monday,September
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

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

public class Layer {
    // system layer
    static public int Default           = 0;
    static public int TransparentFX     = 1;
    static public int IgnoreRaycast     = 2;
    static public int Water             = 4;
    // user layer
    static public int UI                = 8;
    static public int bullet_player     = 9;
    static public int player            = 10;
    static public int enemy             = 11;
    static public int building          = 12;
    static public int ground            = 13;
    static public int trigger           = 14;
    static public int melee_player      = 15;
    static public int melee_enemy       = 16;
    static public int dead_body         = 17;
}
