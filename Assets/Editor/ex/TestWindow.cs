// ======================================================================================
// File         : TestWindow.cs
// Author       : Wu Jie 
// Last Change  : 11/17/2010 | 23:09:19 PM | Wednesday,November
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEditor;
using UnityEngine;

///////////////////////////////////////////////////////////////////////////////
// defines
///////////////////////////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////////////
// class TestWindow
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

public class TestWindow : EditorWindow {

    string myString = "Hello World";
    bool groupEnabled;
    bool myBool = true;
    float myFloat = 1.23f;
    
    // ------------------------------------------------------------------ 
    // Desc: 
    // Add menu named "Test Window" to the Window menu
    // ------------------------------------------------------------------ 

    [MenuItem ("Window/Test Window")]
    static void Init () {
        // Get existing open window or if none, make a new one:
        EditorWindow.GetWindow (typeof (TestWindow));
    }
    
    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnGUI () {
        GUILayout.Label ("Base Settings", EditorStyles.boldLabel);
            myString = EditorGUILayout.TextField ("Text Field", myString);
        
        groupEnabled = EditorGUILayout.BeginToggleGroup ("Optional Settings", groupEnabled);
            myBool = EditorGUILayout.Toggle ("Toggle", myBool);
            myFloat = EditorGUILayout.Slider ("Slider", myFloat, -3, 3);
        EditorGUILayout.EndToggleGroup ();
    }
}
