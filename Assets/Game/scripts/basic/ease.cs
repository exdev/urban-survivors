// ======================================================================================
// File         : ease.cs
// Author       : Wu Jie 
// Last Change  : 02/21/2011 | 10:24:20 AM | Monday,February
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
//
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////
//
///////////////////////////////////////////////////////////////////////////////

public class Ease {

    // ------------------------------------------------------------------ 
    // Desc: quad 
    // ------------------------------------------------------------------ 

    static public float quad_in ( float _t ) { return _t*_t; }
    static public float quad_out ( float _t ) { return -_t*(_t-2); }
    static public float quad_inout ( float _t ) {
        _t*=2.0f;
        if (_t < 1) {
            return _t*_t/2.0f;
        } else {
            --_t;
            return -0.5f * (_t*(_t-2) - 1);
        }
    }
    static public float quad_outin ( float _t ) {
        if (_t < 0.5f) return quad_out (_t*2)/2;
        return quad_in((2*_t)-1)/2 + 0.5f;
    }

    // ------------------------------------------------------------------ 
    // Desc: cubic 
    // ------------------------------------------------------------------ 

    static public float cubic_in ( float _t ) { return _t*_t*_t; }
    static public float cubic_out ( float _t ) {
        _t-=1.0f;
        return _t*_t*_t + 1;
    }
    static public float cubic_inout ( float _t ) {
        _t*=2.0f;
        if(_t < 1) {
            return 0.5f*_t*_t*_t;
        } else {
            _t -= 2.0f;
            return 0.5f*(_t*_t*_t + 2);
        }
    }
    static public float cubic_outin ( float _t ) {
        if (_t < 0.5f) return cubic_out (2*_t)/2;
        return cubic_in (2*_t - 1)/2 + 0.5f;
    }

    // ------------------------------------------------------------------ 
    // Desc: quart
    // ------------------------------------------------------------------ 

    static public float quart_in ( float _t ) { return _t*_t*_t*_t; }
    static public float quart_out ( float _t ) {
        _t-= 1.0f;
        return - (_t*_t*_t*_t- 1);
    }
    static public float quart_inout ( float _t ) {
        _t*=2;
        if (_t < 1) return 0.5f*_t*_t*_t*_t;
        else {
            _t -= 2.0f;
            return -0.5f * (_t*_t*_t*_t- 2);
        }
    }
    static public float quart_outin ( float _t ) {
        if (_t < 0.5f) return quart_out (2*_t)/2;
        return quart_in (2*_t-1)/2 + 0.5f;
    }

    // ------------------------------------------------------------------ 
    // Desc: quint
    // ------------------------------------------------------------------ 

    static public float quint_in ( float _t ) { return _t*_t*_t*_t*_t; }
    static public float quint_out ( float _t ) {
        _t-=1.0f;
        return _t*_t*_t*_t*_t + 1;
    }
    static public float quint_inout ( float _t ) {
        _t*=2.0f;
        if (_t < 1) return 0.5f*_t*_t*_t*_t*_t;
        else {
            _t -= 2.0f;
            return 0.5f*(_t*_t*_t*_t*_t + 2);
        }
    }
    static public float quint_outin ( float _t ) {
        if (_t < 0.5f) return quint_out (2*_t)/2;
        return quint_in (2*_t - 1)/2 + 0.5f;
    }

    // ------------------------------------------------------------------ 
    // Desc: sine 
    // ------------------------------------------------------------------ 

    static public float sine_in ( float _t ) {
        return (_t == 1.0f) ? 1.0f : -Mathf.Cos(_t * Mathf.PI/2) + 1.0f;
    }
    static public float sine_out ( float _t ) {
        return Mathf.Sin(_t* Mathf.PI/2);
    }
    static public float sine_inout ( float _t ) {
        return -0.5f * (Mathf.Cos(Mathf.PI*_t) - 1);
    }
    static public float sine_outin ( float _t ) {
        if (_t < 0.5f) return sine_out (2*_t)/2;
        return sine_in (2*_t - 1)/2 + 0.5f;
    }

    // ------------------------------------------------------------------ 
    // Desc: expo
    // ------------------------------------------------------------------ 

    static public float expo_in ( float _t ) {
        return (_t==0 || _t == 1.0f) ? _t : Mathf.Pow(2.0f, 10 * (_t - 1)) - 0.001f;
    }
    static public float expo_out ( float _t ) {
        return (_t==1.0f) ? 1.0f : 1.001f * (-Mathf.Pow(2.0f, -10 * _t) + 1);
    }
    static public float expo_inout ( float _t ) {
        if (_t==0.0f) return 0.0f;
        if (_t==1.0f) return 1.0f;
        _t*=2.0f;
        if (_t < 1) return 0.5f * Mathf.Pow(2.0f, 10 * (_t - 1)) - 0.0005f;
        return 0.5f * 1.0005f * (-Mathf.Pow(2.0f, -10 * (_t - 1)) + 2);
    }
    static public float expo_outin ( float _t ) {
        if (_t < 0.5f) return expo_out (2*_t)/2;
        return expo_in (2*_t - 1)/2 + 0.5f;
    }

    // ------------------------------------------------------------------ 
    // Desc: circ 
    // ------------------------------------------------------------------ 

    static public float circ_in ( float _t ) {
        return -(Mathf.Sqrt(1 - _t*_t) - 1);
    }
    static public float circ_out ( float _t ) {
        _t-= 1.0f;
        return Mathf.Sqrt(1 - _t* _t);
    }
    static public float circ_inout ( float _t ) {
        _t*=2.0f;
        if (_t < 1) {
            return -0.5f * (Mathf.Sqrt(1 - _t*_t) - 1);
        } else {
            _t -= 2.0f;
            return 0.5f * (Mathf.Sqrt(1 - _t*_t) + 1);
        }
    }
    static public float circ_outin ( float _t ) {
        if (_t < 0.5f) return circ_out (2*_t)/2;
        return circ_in (2*_t - 1)/2 + 0.5f;
    }

    // ------------------------------------------------------------------ 
    // Desc: elastic
    // ------------------------------------------------------------------ 

    static float __elastic_in_helper ( float _t, 
                                    float _b, 
                                    float _c, 
                                    float _d, 
                                    float _a, 
                                    float _p )
    {
        float t_adj,_s;

        if (_t==0) return _b;
        t_adj = _t/_d;
        if (t_adj==1) return _b+_c;

        if ( _a < Mathf.Abs (_c) ) {
            _a = _c;
            _s = _p / 4.0f;
        } else {
            _s = _p / (float)2*Mathf.PI * Mathf.Asin(_c/_a);
        }

        t_adj -= 1.0f;
        return -(_a*Mathf.Pow(2.0f,10*t_adj) * Mathf.Sin( (t_adj*_d-_s)*(float)2*Mathf.PI/_p )) + _b;

    }
    static float __elastic_out_helper ( float _t, 
                                              float _b /*dummy*/, 
                                              float _c, 
                                              float _d /*dummy*/, 
                                              float _a, 
                                              float _p )
    {
        float _s;

        if (_t==0) return 0;
        if (_t==1) return _c;

        if(_a < _c) {
            _a = _c;
            _s = _p / 4.0f;
        } else {
            _s = _p / (float)2*Mathf.PI * Mathf.Asin(_c / _a);
        }

        return (_a*Mathf.Pow(2.0f,-10*_t) * Mathf.Sin( (_t-_s)*(float)2*Mathf.PI/_p ) + _c);
    }

    static public float elastic_in ( float _t, float _a, float _p ) {
        return __elastic_in_helper(_t, 0, 1, 1, _a, _p);
    }
    static public float elastic_out ( float _t, float _a, float _p ) {
        return __elastic_out_helper(_t, 0, 1, 1, _a, _p);
    }
    static public float elastic_inout ( float _t, float _a, float _p ) {
        float _s;

        if (_t==0) return 0.0f;
        _t*=2.0f;
        if (_t==2) return 1.0f;

        if(_a < 1.0f) {
            _a = 1.0f;
            _s = _p / 4.0f;
        } else {
            _s = _p / (float)2*Mathf.PI * Mathf.Asin(1.0f / _a);
        }

        if (_t < 1) return -.5f*(_a*Mathf.Pow(2.0f,10*(_t-1)) * Mathf.Sin( (_t-1-_s)*(float)2*Mathf.PI/_p ));
        return _a*Mathf.Pow(2.0f,-10*(_t-1)) * Mathf.Sin( (_t-1-_s)*(float)2*Mathf.PI/_p )*.5f + 1.0f;
    }
    static public float elastic_outin ( float _t, float _a, float _p ) {
        if (_t < 0.5f) return __elastic_out_helper(_t*2, 0, 0.5f, 1.0f, _a, _p);
        return __elastic_in_helper(2*_t - 1.0f, 0.5f, 0.5f, 1.0f, _a, _p);
    }

    // ------------------------------------------------------------------ 
    // Desc: back 
    // ------------------------------------------------------------------ 

    static public float back_in ( float _t, float _s ) {
        return _t*_t*((_s+1)*_t - _s);
    }
    static public float back_out ( float _t, float _s ) {
        _t-= 1.0f;
        return _t*_t*((_s+1)*_t+ _s) + 1;
    }
    static public float back_inout ( float _t, float _s ) {
        _t *= 2.0f;
        if (_t < 1) {
            _s *= 1.525f;
            return 0.5f*(_t*_t*((_s+1)*_t - _s));
        } else {
            _t -= 2;
            _s *= 1.525f;
            return 0.5f*(_t*_t*((_s+1)*_t+ _s) + 2);
        }
    }
    static public float back_outin ( float _t, float _s ) {
        if (_t < 0.5f) return back_out (2*_t, _s)/2;
        return back_in(2*_t - 1, _s)/2 + 0.5f;
    }

    // ------------------------------------------------------------------ 
    // Desc: bounce
    // ------------------------------------------------------------------ 

    static float __bounce_out_helper ( float _t, 
                                       float _c, 
                                       float _a )
    {
        if (_t == 1.0f) return _c;
        if (_t < (4/11.0f)) {
            return _c*(7.5625f*_t*_t);
        } else if (_t < (8/11.0f)) {
            _t -= (6/11.0f);
            return -_a * (1.0f - (7.5625f*_t*_t + 0.75f)) + _c;
        } else if (_t < (10/11.0f)) {
            _t -= (9/11.0f);
            return -_a * (1.0f - (7.5625f*_t*_t + 0.9375f)) + _c;
        } else {
            _t -= (21/22.0f);
            return -_a * (1.0f - (7.5625f*_t*_t + 0.984375f)) + _c;
        }
    }
    static public float bounce_in ( float _t, float _a ) {
        return 1.0f - __bounce_out_helper(1.0f-_t, 1.0f, _a);
    }
    static public float bounce_out ( float _t, float _a ) {
        return __bounce_out_helper(_t, 1, _a);
    }
    static public float bounce_inout ( float _t, float _a ) {
        if (_t < 0.5f) return bounce_in (2*_t, _a)/2;
        else return (_t == 1.0f) ? 1.0f : bounce_out (2*_t - 1, _a)/2 + 0.5f;
    }
    static public float bounce_outin ( float _t, float _a ) {
        if (_t < 0.5f) return __bounce_out_helper(_t*2, 0.5f, _a);
        return 1.0f - __bounce_out_helper (2.0f-2*_t, 0.5f, _a);
    }

    // ------------------------------------------------------------------ 
    // Desc: smooth 
    // ------------------------------------------------------------------ 

    static public float smooth ( float _t ) {
        if ( _t <= 0.0f ) return 0.0f;
        if ( _t >= 1.0f ) return 1.0f;
        return _t*_t*(3.0f - 2.0f*_t);
    }

    // ------------------------------------------------------------------ 
    // Desc: fade
    // ------------------------------------------------------------------ 

    static public float fade ( float _t ) {
        if ( _t <= 0.0f ) return 0.0f;
        if ( _t >= 1.0f ) return 1.0f;
        return _t*_t*_t*(_t*(_t*6.0f-15.0f)+10.0f);
    }
}
