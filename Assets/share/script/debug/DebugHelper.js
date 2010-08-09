// ======================================================================================
// File         : DebugHelper.js
// Author       : Wu Jie 
// Last Change  : 08/09/2010 | 22:02:59 PM | Monday,August
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// functions
///////////////////////////////////////////////////////////////////////////////

// ------------------------------------------------------------------ 
// Desc: DrawCircle
// ------------------------------------------------------------------ 

// DrawCircleX
static function DrawCircleX ( _center : Vector3, _radius : float, _color : Color ) {
    DrawCircle ( Quaternion.Euler(0.0,0.0,90.0), _center, _radius, _color );
}

// DrawCircleY
static function DrawCircleY ( _center : Vector3, _radius : float, _color : Color ) {
    DrawCircle ( Quaternion.identity, _center, _radius, _color );
}

// DrawCircleZ
static function DrawCircleZ ( _center : Vector3, _radius : float, _color : Color ) {
    DrawCircle ( Quaternion.Euler(90.0,0.0,0.0), _center, _radius, _color );
}

// 
static function DrawCircle ( _rot: Quaternion, _center : Vector3, _radius : float, _color : Color ) {
    //
    var two_pi = 2.0 * Mathf.PI;
    var segments = 32.0;
    var step = two_pi / segments;
    var theta = 0.0;

    //
    var last = _center + _rot * ( _radius * Vector3( Mathf.Cos(theta), 0.0, Mathf.Sin(theta) ) );
    theta += step;

    //
    for ( var i = 1; i <= segments; ++i ) {
        var cur = _center + _rot * ( _radius * Vector3( Mathf.Cos(theta), 0.0, Mathf.Sin(theta) ) );
        Debug.DrawLine ( last, cur, _color );
        last = cur;
        theta += step;
    }
}

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

// 1 params
static function DrawBall ( _center : Vector3 ) {
    DrawBall ( _center, 1.0, Color.white );
}

// 2 params
static function DrawBall ( _center : Vector3, _radius : float ) {
    DrawBall ( _center, _radius, Color.white );
}

// 3 params
static function DrawBall ( _center : Vector3, _radius : float, _color : Color ) {
    DebugHelper.DrawCircleX ( _center, _radius, _color );
    DebugHelper.DrawCircleY ( _center, _radius, _color );
    DebugHelper.DrawCircleZ ( _center, _radius, _color );
}
