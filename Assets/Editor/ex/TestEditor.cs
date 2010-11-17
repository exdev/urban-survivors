// ======================================================================================
// File         : TestEditor.cs
// Author       : Wu Jie 
// Last Change  : 11/17/2010 | 23:09:13 PM | Wednesday,November
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
// class TestEditor 
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

class TestEditor : Editor {

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [MenuItem ("ex/Simple Test %#t")]
    static void SimpleTest () {
        EditorWindow curWindow = EditorWindow.mouseOverWindow;
        Debug.Log("Mouse Over Window: " + curWindow.name );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public override void OnInspectorGUI () {
        // GameObject go = target as GameObject;
        // target.lookAtPoint = EditorGUILayout.Vector3Field ("Look At Point", target.lookAtPoint);
        // if (GUI.changed)
        //     EditorUtility.SetDirty (target);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnSceneGUI()
    {
        if (Event.current.type == EventType.MouseMove) {
            Debug.Log("hahaha!" );
        }

        // been told by: http://forum.unity3d.com/threads/34137-SOLVED-Custom-Editor-OnSceneGUI-Scripting?p=221880#post221880
        if (Event.current.type == EventType.Layout) {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        }

        // Handles.BeginGUI();
        // selected = EditorGUILayout.Popup(
        //                                  selected, new string[] { "0", "1", "2" });
        // color = EditorGUILayout.ColorField(color);
        // Handles.EndGUI();
    }
}

