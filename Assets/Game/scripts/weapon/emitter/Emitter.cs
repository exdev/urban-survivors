// ======================================================================================
// File         : Emitter.cs
// Author       : Wu Jie 
// Last Change  : 09/27/2010 | 21:37:55 PM | Monday,September
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

public class Emitter : MonoBehaviour {

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    protected static GameObject fireEffect = null;
    public GameObject prefab_FireEffect = null;

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	protected void Awake () {
        if ( fireEffect == null && this.prefab_FireEffect ) {
            fireEffect = (GameObject)Instantiate( this.prefab_FireEffect );
        }
	}

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public virtual int Emit ( Transform _anchor, GameObject _bullet, bool _activeReload ) {
        Debug.Log("warning! pls reimplement emitter in sub-class");
        return 0;
    }
}
