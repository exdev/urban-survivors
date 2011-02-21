// ======================================================================================
// File         : HintText.cs
// Author       : Wu Jie 
// Last Change  : 02/21/2011 | 11:09:30 AM | Monday,February
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
//
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

///////////////////////////////////////////////////////////////////////////////
// class 
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

public class HintText : MonoBehaviour {

    List<PackedSprite> sp_list = new List<PackedSprite>();
    List<SpriteText> st_list = new List<SpriteText>();
    bool startBlink = false;
    float startTime = 0.0f;

    ///////////////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////////////

    public float blinkTime = 1.0f;

    ///////////////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	void Start () {
        foreach ( Transform child in transform ) {
            PackedSprite sp = child.gameObject.GetComponent<PackedSprite>();
            if ( sp ) {
                sp_list.Add(sp);
                continue;
            }

            SpriteText st = child.gameObject.GetComponent<SpriteText>();
            if ( st ) {
                st_list.Add(st);
                continue;
            }
        }
	}
	
    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	void Update () {
        if ( startBlink ) {
            float t = Time.time - startTime;
            float r = (t % blinkTime) / blinkTime;
            float v = Ease.quart_out(r);
            float a = Mathf.Lerp( 1.0f, 0.0f, v );

            foreach( PackedSprite sp in sp_list ) {
                sp.color.a = a;
                sp.SetColor(sp.color);
            }
            foreach( SpriteText st in st_list ) {
                st.color.a = a;
                st.SetColor(st.color);
            }
        }
	}

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void StartBlink () {
        startBlink = true;
        startTime = Time.time;
    }
}
