// ======================================================================================
// File         : MainMenuOptions.cs
// Author       : Wu Jie 
// Last Change  : 02/25/2011 | 21:03:49 PM | Friday,February
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
//
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////
// class 
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

public class MainMenuOptions : MonoBehaviour {

    public bool isMultiPlayer = false;

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	void Awake () {
#if UNITY_IPHONE
       // lock the screen
       iPhoneKeyboard.autorotateToPortrait = false; 
       iPhoneKeyboard.autorotateToPortraitUpsideDown = false; 
       iPhoneKeyboard.autorotateToLandscapeRight = false; 
       iPhoneKeyboard.autorotateToLandscapeLeft = true;
#endif		
        DontDestroyOnLoad (this.gameObject);
	}
}
