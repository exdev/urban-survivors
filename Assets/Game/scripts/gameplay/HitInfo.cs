// ======================================================================================
// File         : HitInfo.cs
// Author       : Wu Jie 
// Last Change  : 12/10/2010 | 16:06:01 PM | Friday,December
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
// Desc: HitInfo
// ------------------------------------------------------------------ 

[System.Serializable]
public class HitInfo {
    public enum StunType {
        none    = 0,
        ignore  = 1,
        imue    = 2, 
        light   = 3,
        normal  = 4,
        serious = 5,
    }

    public StunType stunType = StunType.none;
    public Vector3 position = Vector3.zero; 
    public Vector3 normal = Vector3.up; 
    public Vector3 knockBackForce = Vector3.zero; 
}
