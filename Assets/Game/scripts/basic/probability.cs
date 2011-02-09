// ======================================================================================
// File         : probability.cs
// Author       : Wu Jie 
// Last Change  : 02/09/2011 | 17:51:36 PM | Wednesday,February
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
// class Probability
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

public class Probability {
    List<float> cdf = new List<float>();

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void Init ( float[] _weights ) {
        cdf.Clear();
        if ( _weights.Length == 0 )
            return;

        float total_weights = 0.0f; 
        for ( int i = 0; i < _weights.Length; ++i ) {
            total_weights += _weights[i];
        }

        cdf.Add(_weights[0]/total_weights);
        for ( int i = 1; i < _weights.Length - 1; ++i ) {
            cdf.Add(_weights[i]/total_weights);
            cdf[i] = cdf[i] + cdf[i-1];
        }
        cdf.Add(1.0f);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public int GetIndex () {
        float y = Random.Range(0.0f,1.0f);
        for ( int i = 0; i < cdf.Count; ++i ) {
            if ( y < cdf[i] )
                return i;
        }
        return -1; // this is error, should not happend.
    }
}
