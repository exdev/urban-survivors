// ======================================================================================
// File         : SyncAnimModel.cs
// Author       : Wu Jie 
// Last Change  : 11/02/2010 | 21:41:36 PM | Tuesday,November
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEditor;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Reflection;

///////////////////////////////////////////////////////////////////////////////
// class SyncAnimModel 
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

public class SyncAnimModel
{
    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static void CopyTagAndLayerRecursively ( string _path, Transform _src, Transform _destRoot ) {
        Transform destChildTrans = null;

        if ( _path == "" ) {
            destChildTrans = _destRoot;
        }
        else {
            destChildTrans = _destRoot.Find(_path);
        }

        if ( destChildTrans == null )
            return;

        destChildTrans.gameObject.tag = _src.gameObject.tag;
        destChildTrans.gameObject.layer = _src.gameObject.layer;
        foreach ( Transform child in _src ) {
            string child_path = "";
            if ( _path == "" )
                child_path = child.gameObject.name;
            else
                child_path = child.gameObject.name + "/" + _path;
            CopyTagAndLayerRecursively ( child_path, child, _destRoot );
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [MenuItem("Assets/Sync Animation Model (for .blender file)")]
    static void exec() {
        // Get selected animation model
        GameObject curSelection = Selection.activeObject as GameObject;
        if ( curSelection == null ) {
            EditorUtility.DisplayDialog ("Error", "Selected object is not an GameObject.", "OK");
            return;
        }

        //
        string selectionPath = AssetDatabase.GetAssetPath(curSelection);
        Debug.Log("Start syncing animation mesh: " + selectionPath);
        string fileName = Path.GetFileNameWithoutExtension(selectionPath); 

        // get the prefab by selection path, if the prefab not exists, do nothing.
        string prefabDir = Path.GetDirectoryName(Path.GetDirectoryName(selectionPath)) + "/prefabs"; 
        string prefabPath = prefabDir + "/" + fileName + ".prefab";
        GameObject old_prefabGO = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;
        if ( old_prefabGO == null ) {
            EditorUtility.DisplayDialog ("Error", "can't find prefab " + prefabPath, "OK");
            return;
        }

        // get the new prefab by add suffix '_copy'.
        // if the prefab not exists, we should create an empty one.
        string new_prefabPath = prefabDir + "/" + fileName + "_copy.prefab";
        Object new_prefab = AssetDatabase.LoadAssetAtPath(new_prefabPath, typeof(GameObject)) as GameObject;
        if ( new_prefab == null ) {
            new_prefab = EditorUtility.CreateEmptyPrefab(new_prefabPath);
        }
        DebugHelper.Assert( new_prefab, "Can't create prefab: " + Path.GetFileName(new_prefabPath) );
        EditorUtility.ReplacePrefab( curSelection, new_prefab );

        // create new prefab GO 
        GameObject new_prefabGO = AssetDatabase.LoadAssetAtPath(new_prefabPath, typeof(GameObject)) as GameObject;
        DebugHelper.Assert( new_prefabGO, "Can't find prefab: " + new_prefabPath );

        // copy GO tag & layer
        CopyTagAndLayerRecursively( "", old_prefabGO.transform, new_prefabGO.transform );

        // copy the components from the old prefab.
        Component[] old_comps = old_prefabGO.GetComponents<Component>();
        foreach ( Component old_comp in old_comps ) {
            // if we have the component in new prefab, don't do anything.
            if ( new_prefabGO.GetComponent( old_comp.GetType() ) != null )
                continue;

            // clone the component in old prefab to the new one.
            Component new_comp = new_prefabGO.AddComponent(old_comp.GetType());
            CompHelper.Copy ( old_comp, new_comp );
        }

        // replace the old prefab with the new one
        EditorUtility.ReplacePrefab( new_prefabGO, old_prefabGO );
        AssetDatabase.DeleteAsset(new_prefabPath);

        //
        Debug.Log("Animation mesh synced.");
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    // KEEPME { 
    // [MenuItem("Assets/Test")]
    // static void test() {
    //     GameObject anim_prefab = AssetDatabase.LoadAssetAtPath("Assets/user/test/prefabs/PlayerBoy.prefab", typeof(GameObject)) as GameObject;
    //     if ( anim_prefab != null ) {
    //         Component[] comps = anim_prefab.GetComponents<Component>();
    //         foreach( Component comp in comps ) {
    //             Debug.Log("comp type: " + comp.GetType() );
    //         }
    //     }
    // }
    // } KEEPME end 
}
