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

///////////////////////////////////////////////////////////////////////////////
// class Line 
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

public class Line {
    public Vector3 start;
    public Vector3 end;
    public Color color;
}

///////////////////////////////////////////////////////////////////////////////
// class DebugHelper
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

public class DebugHelper : MonoBehaviour {

    static DebugHelper instance = null;

    // debug text
    string debug_text = "";

    // fps counter
    float updateInterval = 0.5f;
    float lastInterval = 0.0f;
    float fps = 0.0f;
    int frames = 0;

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public bool showFps = true;
    public bool showDebugText = true;
    public bool showHotPoints = true;

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

    IEnumerator CleanDebugText () {
        yield return new WaitForEndOfFrame();
        debug_text = "";
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
        // NOTE: the OnGUI call multiple times in one frame, so we just clear text here.
        StartCoroutine(CleanDebugText());
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnGUI () {
        if ( showFps )
            GUILayout.Label ( "fps: " + fps.ToString("f2") );
        if ( showDebugText )
            GUILayout.Label ( debug_text );
        if ( showHotPoints ) {
            foreach ( Touch t in Input.touches ) {
                if (t.phase != TouchPhase.Ended && t.phase != TouchPhase.Canceled) {
                    Vector2 screen_pos = new Vector2 ( t.position.x, Screen.height - t.position.y );
                    // TODO { 
                    // GUI.DrawTexture ( new Rect( screen_pos.x,
                    //                             screen_pos.y,
                    //                             tex_hotpoint.width, 
                    //                             tex_hotpoint.height ), 
                    //                   tex_hotpoint, 
                    //                   ScaleMode.ScaleToFit, 
                    //                   true, 
                    //                   0.0f );
                    // } TODO end 
                    GUI.Box ( new Rect( screen_pos.x-5, screen_pos.y-5, 10, 10 ), "" );
                }
            }
        }
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
            DebugHelper.DrawLine ( last, cur, _color );
            last = cur;
            theta += step;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public static void DrawLine ( Vector3 _start, Vector3 _end, Color _color ) {
        Debug.DrawLine ( _start, _end, _color );
        // TODO: use LinesGR.cs method!
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

    public static void DrawDestination ( Vector3 _dest ) {
        DebugHelper.DrawCircleY ( _dest, 0.5f, new Color(1.0f,0.0f,0.0f) );
        DebugHelper.DrawCircleY ( _dest, 0.1f, new Color(1.0f,1.0f,0.0f) );
        DebugHelper.DrawLine ( _dest, _dest + Vector3.up * 2.0f, new Color(1.0f,1.0f,0.0f) );
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
