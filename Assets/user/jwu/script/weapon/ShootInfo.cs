// ======================================================================================
// File         : ShootInfo.cs
// Author       : Wu Jie 
// Last Change  : 12/13/2010 | 21:36:27 PM | Monday,December
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

// // ------------------------------------------------------------------ 
// // Desc: ComboInfo 
// // ------------------------------------------------------------------ 

// [System.Serializable]
// // public class ComboInfo {
// //     public string animName = "unknown";
// //     public Vector2 validInputTime = new Vector2(0.0f,1.0f);
// //     public float endTime = -1.0f; 
// //     public bool canCharge = false;
// //     public GameObject attack_shape = null;
// //     [System.NonSerialized] public ComboInfo next = null;
// //     // TODO: public collision info 
// // }

// ------------------------------------------------------------------ 
// Desc: ShootInfo
// ------------------------------------------------------------------ 

public class ShootInfo : MonoBehaviour {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public string shootAnim = "unknown";
    public string reloadAnim = "unknown";
    public Transform anchor = null;
    public float shootSpeed = 1.0f;
    public float reloadSpeed = 1.0f;
    public float accuracy = 1.0f;
    public uint maxBullets = 10;

    protected uint curBullets = 10;

    ///////////////////////////////////////////////////////////////////////////////
    // function defines
    ///////////////////////////////////////////////////////////////////////////////

    // Use this for initialization
    void Start () {
        DebugHelper.Assert(this.shootAnim!="unknown", "shootAnim can't be unknown!");
        DebugHelper.Assert(this.reloadAnim!="unknown", "reloadAnim can't be unknown!");
        DebugHelper.Assert(this.anchor, "weapon's anchor not set" );
    }
}
