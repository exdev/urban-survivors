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
        helperGO.AddComponent<SuperDuplicateHelper>();

        // now duplicate our selection game objects, and calculate the center
        Vector3 center = Vector3.zero;
        GameObject[] GOs = new GameObject[count]; 
        for ( int i = 0; i < count; ++i ) {
            GameObject go = transforms[i].gameObject;
            GOs[i] = Object.Instantiate ( go, go.transform.position, go.transform.rotation ) as GameObject; 
            GOs[i].name = go.name;
            center += GOs[i].transform.position; 
        }
        center /= count;

        //  set their parents to the SuperDuplicateHelper
        helperGO.transform.position = center;
        helperGO.transform.rotation = Quaternion.identity;
        foreach ( GameObject go in GOs ) {
            go.transform.parent = helperGO.transform;
        }

        //
        Selection.activeObject = helperGO;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public override void OnInspectorGUI () {
        // SuperDuplicateHelper helper = (SuperDuplicateHelper)target;
        if (GUI.changed)
            EditorUtility.SetDirty (target);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void OnSceneGUI () {
        SuperDuplicateHelper helper = (SuperDuplicateHelper)target;
        Transform go_trans = helper.gameObject.transform;

        // prevent editor select other object when editing SuperDuplicateHelper
        if (Event.current.type == EventType.Layout) {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        }

        // if MouseMove (in move model)
        if ( Event.current.type == EventType.MouseMove ) {
            Ray ray = HandleUtility.GUIPointToWorldRay (Event.current.mousePosition); 

            // DISABLE { 
            // Plane plane = new Plane ( Vector3.up, 0.0f );
            // float dist = 0.0f;
            // plane.Raycast( ray, out dist );
            // Vector3 pos = ray.origin + ray.direction * dist;
            // } DISABLE end 

            Vector3 pos = ray.origin + ray.direction * 500.0f;

            int layerMask = 1 << Layer.ground;
            RaycastHit hit;
            if ( Physics.Raycast (ray.origin, ray.direction, out hit, 10000.0f, layerMask ) ) {
                pos = hit.point;
            }
            go_trans.position = pos;

            return;
        }

        // if MouseUp (finish the job)
        if ( Event.current.type == EventType.MouseUp ) {
            foreach ( Transform child in go_trans ) {
                child.parent = null;
            }
            GameObject.DestroyImmediate(go_trans);
            Selection.activeObject = null;
            return;
        }
    }
}
        
