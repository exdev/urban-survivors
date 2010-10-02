// ======================================================================================
// File         : DebugHelper.cs
// Author       : Wu Jie 
// Last Change  : 09/27/2010 | 22:14:59 PM | Monday,September
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

public class DebugHelper : MonoBehaviour {

    // ------------------------------------------------------------------ 
    // Desc: DrawCircle
    // ------------------------------------------------------------------ 

    // DrawCircleX
    public static void DrawCircleX ( Vector3 _center, float _radius, Color _color ) {
        DrawCircle ( Quaternion.Euler(0.0f,0.0f,90.0f), _center, _radius, _color );
    }

    // DrawCircleY
    public static void DrawCircleY ( Vector3 _center, float _radius, Color _color ) {
        DrawCircle ( Quaternion.identity, _center, _radius, _color );
    }

    // DrawCircleZ
    public static void DrawCircleZ ( Vector3 _center, float _radius, Color _color ) {
        DrawCircle ( Quaternion.Euler(90.0f,0.0f,0.0f), _center, _radius, _color );
    }

    // 
    public static void DrawCircle ( Quaternion _rot, Vector3 _center, float _radius, Color  _color ) {
        //
        float two_pi = 2.0f * Mathf.PI;
        float segments = 32.0f;
        float step = two_pi / segments;
        float theta = 0.0f;

        //
        Vector3 last = _center + _rot * ( _radius * new Vector3( Mathf.Cos(theta), 0.0f, Mathf.Sin(theta) ) );
        theta += step;

        //
        for ( int i = 1; i <= segments; ++i ) {
            Vector3 cur = _center + _rot * ( _radius * new Vector3( Mathf.Cos(theta), 0.0f, Mathf.Sin(theta) ) );
            Debug.DrawLine ( last, cur, _color );
            last = cur;
            theta += step;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    // 1 params
    public static void DrawBall ( Vector3 _center ) {
        DrawBall ( _center, 1.0f, Color.white );
    }

    // 2 params
    public static void DrawBall ( Vector3 _center, float _radius ) {
        DrawBall ( _center, _radius, Color.white );
    }

    // 3 params
    public static void DrawBall ( Vector3 _center, float _radius, Color _color ) {
        DebugHelper.DrawCircleX ( _center, _radius, _color );
        DebugHelper.DrawCircleY ( _center, _radius, _color );
        DebugHelper.DrawCircleZ ( _center, _radius, _color );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void Assert(bool _comparison, string _message)
    {
        if (!_comparison) {
            Debug.LogWarning("Assert Failed: " + _message);
            Debug.Break();
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void Check(bool _comparison, string _message)
    {
        if (!_comparison) {
            Debug.LogWarning("Check Failed: " + _message);
        }
    }
}
