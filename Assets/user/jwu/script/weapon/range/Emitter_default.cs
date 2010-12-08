// ======================================================================================
// File         : Emitter_default.cs
// Author       : Wu Jie 
// Last Change  : 09/27/2010 | 21:46:38 PM | Monday,September
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

public class Emitter_default : Emitter {

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public override void Emit ( GameObject _bullet ) {
        // create a bullet, and rotate it based on the vector inputRotation
        Instantiate(_bullet, transform.position, transform.rotation );
    }
}
