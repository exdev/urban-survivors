// ======================================================================================
// File         : MathHelper.cs
// Author       : Wu Jie 
// Last Change  : 11/27/2010 | 23:34:41 PM | Saturday,November
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
// class MathHelper 
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

public class MathHelper : MonoBehaviour {

    const float float_epsilon = 1e-06f; 

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public bool IsZerof ( float _value, float _eps = MathHelper.float_epsilon ) {
        return Mathf.Abs(_value) <= _eps;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public Vector3 Pendicular ( Vector3 _from, Vector3 _to ) {
        return _from - Vector3.Project(_from,_to);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public Vector3 InsideDeviationAngle ( Vector3 _source, Vector3 _base, float _cosTheta ) {
        // immediately return zero length input vectors
        float sourceLength = _source.magnitude;
        if (sourceLength == 0) return _source;

        // measure the angular diviation of "_source" from "_base"
        Vector3 dir = _source.normalized;
        float cosineOfSourceAngle = Vector3.Dot (dir,_base);

        // _source vector is already inside the cone, just return it
        if (cosineOfSourceAngle >= _cosTheta) return _source;

        // find the portion of "_source" that is perpendicular to "_base"
        Vector3 perp = MathHelper.Pendicular(_source,_base);

        // normalize that perpendicular
        Vector3 unitPerp = perp.normalized;

        // construct a new vector whose length equals the _source vector,
        // and lies on the intersection of a plane (formed the _source and
        // _base vectors) and a cone (whose axis is "_base" and whose
        // angle corresponds to _cosTheta)
        float sinTheta = Mathf.Sqrt (1.0f - (_cosTheta * _cosTheta));
        Vector3 c0 = _base * _cosTheta;
        Vector3 c1 = unitPerp * sinTheta;
        return (c0 + c1) * sourceLength;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public Vector3 OutsideDeviationAngle ( Vector3 _source, Vector3 _base, float _cosTheta ) {
        // immediately return zero length input vectors
        float sourceLength = _source.magnitude;
        if (sourceLength == 0) return _source;

        // measure the angular diviation of "_source" from "_base"
        Vector3 dir = _source.normalized;
        float cosineOfSourceAngle = Vector3.Dot (dir,_base);

        // _source vector is already outside the cone, just return it
        if (cosineOfSourceAngle <= _cosTheta) return _source;

        // find the portion of "_source" that is perpendicular to "_base"
        Vector3 perp = MathHelper.Pendicular(_source,_base);

        // normalize that perpendicular
        Vector3 unitPerp = perp.normalized;

        // construct a new vector whose length equals the _source vector,
        // and lies on the intersection of a plane (formed the _source and
        // _base vectors) and a cone (whose axis is "_base" and whose
        // angle corresponds to _cosTheta)
        float sinTheta = Mathf.Sqrt (1.0f - (_cosTheta * _cosTheta));
        Vector3 c0 = _base * _cosTheta;
        Vector3 c1 = unitPerp * sinTheta;
        return (c0 + c1) * sourceLength;
    }
}
