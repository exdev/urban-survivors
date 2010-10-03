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

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    private static DebugHelper instance = null;

    // debug text
    private string debug_text = "";

    // fps counter
    private float updateInterval = 0.5f;
    private float lastInterval = 0.0f;
    private float fps = 0.0f;
    private int frames = 0;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Awake () {
        if( instance == null )
            instance = this;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Update () {
        // count fps
        ++frames;
        float timeNow = Time.realtimeSinceStartup;
        if( timeNow > lastInterval + updateInterval ) {
            fps = frames / (timeNow - lastInterval);
            frames = 0;
            lastInterval = timeNow;
        }

        // clear debug text
        debug_text = "";
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnGUI () {
        GUILayout.Label ( "fps: " + fps.ToString("f2") );
        GUILayout.Label ( debug_text );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public static void ScreenPrint ( string _text ) {
        instance.debug_text = instance.debug_text + _text + "\n"; 
    }

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