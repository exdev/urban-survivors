// ======================================================================================
// File         : SuperDuplicateEditor.cs
// Author       : Wu Jie 
// Last Change  : 11/18/2010 | 00:05:21 AM | Thursday,November
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
// class SuperDuplicateEditor 
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

[CustomEditor(typeof(SuperDuplicateHelper))]
class SuperDuplicateEditor : Editor {

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [MenuItem ("ex/Super Duplicate %#d")]
    static void SuperDuplicate () {
        // DISABLE: int count = Selection.transforms.Length;
        Transform[] transforms = Selection.GetTransforms(SelectionMode.TopLevel|SelectionMode.Editable|SelectionMode.ExcludePrefab);
        int count = transforms.Length;
        
        // check if we select some game objects.
        if ( count == 0 ) {
            EditorUtility.DisplayDialog ("Error", "Please select a GameObject in the scene.", "OK");
            return;
        }

        // create an empty gameobject with SuperDuplicateHelper
        GameObject helperGO = new GameObject("SuperDuplicateHelper");
        helperGO.transform.position = Vector3.zero;
        helperGO.transform.rotation = Quaternion.identity;
        SuperDuplicateHelper helper = helperGO.AddComponent<SuperDuplicateHelper>();
        helper.Position = helperGO.transform.position; 
        helper.Rotation = helperGO.transform.eulerAngles; 

        // now duplicate our selection game objects, and set their parents to the SuperDuplicateHelper
        GameObject[] GOs = new GameObject[count]; 
        for ( int i = 0; i < count; ++i ) {
            GameObject go = transforms[i].gameObject;
            GOs[i] = Object.Instantiate ( go, go.transform.position, go.transform.rotation ) as GameObject; 
            GOs[i].transform.parent = helperGO.transform;
        }

        Selection.activeObject = helperGO;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public override void OnInspectorGUI () {
        SuperDuplicateHelper helper = (SuperDuplicateHelper)target;
        helper.Position = EditorGUILayout.Vector3Field ("Position", helper.Position);
        helper.Rotation = EditorGUILayout.Vector3Field ("Rotation", helper.Rotation);

        if (GUI.changed)
            EditorUtility.SetDirty (target);
    }
}
