// ======================================================================================
// File         : CoroutineHelper.cs
// Author       : Wu Jie 
// Last Change  : 02/15/2011 | 22:51:15 PM | Tuesday,February
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

// ------------------------------------------------------------------ 
// Desc: this function can help while using yield new WaitForSeconds(1.0) it will affect by timeScale. 
// ------------------------------------------------------------------ 

public class CoroutineHelper {
    public static IEnumerator WaitForRealSeconds ( float _sec ) {
        float pauseEndTime = Time.realtimeSinceStartup + _sec;
        while ( Time.realtimeSinceStartup < pauseEndTime ) {
            yield return 0;
        }
    }

}
