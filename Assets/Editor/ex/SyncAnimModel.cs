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
using System.Collections.Generic;
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

    static void RemovedUnusedGO( Transform _src ) {
        // NOTE: you can't detach and destroy child directly when iterate the child list 
        List<Transform> removeList = new List<Transform>();

        // get unused child list
        foreach ( Transform child in _src ) {
            string child_name = child.gameObject.name;
            // HARDCODE { 
            // skip the unsued GO
            if ( child_name == "Camera"
              || child_name == "Lamp"
              || child_name == "Lamp.001" 
              ) {
                removeList.Add (child);
            }
            // } HARDCODE end 
        }

        //
        foreach ( Transform trans in removeList ) {
            trans.parent = null;
            GameObject.DestroyImmediate(trans.gameObject);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static void CopyTagAndLayerRecursively ( string _path, Transform _src, Transform _destRoot ) {
        Transform destChildTrans = null;

        if ( _path == "" )
            destChildTrans = _destRoot;
        else
            destChildTrans = _destRoot.Find(_path);

        if ( destChildTrans == null )
            return;

        GameObject srcGO = _src.gameObject;
        GameObject destGO = destChildTrans.gameObject;

        // copy the tag and layer
        destGO.tag = srcGO.tag;
        destGO.layer = srcGO.layer;

        // copy the components
        Component[] old_comps = srcGO.GetComponents<Component>();
        foreach ( Component old_comp in old_comps ) {
            Component new_comp = destGO.GetComponent( old_comp.GetType() );  

            // if we don't have the component in new prefab, create one.
            if ( new_comp == null )
                new_comp = destGO.AddComponent(old_comp.GetType());

            // clone the component in old prefab to the new one.
            CompHelper.Copy ( old_comp, new_comp );
        }

        foreach ( Transform child in _src ) {
            string child_path = "";
            if ( _path == "" )
                child_path = child.gameObject.name;
            else
                child_path = child.gameObject.name + "/" + _path;
            CopyTagAndLayerRecursively ( child_path, child, _destRoot );
        }
    }

    // // ------------------------------------------------------------------ 
    // // Desc: 
    // // ------------------------------------------------------------------ 

    // static void CopyComponentsRecursively ( string _path, Transform _src, Transform _destRoot ) {
    //     Transform destChildTrans = null;

    //     if ( _path == "" )
    //         destChildTrans = _destRoot;
    //     else
    //         destChildTrans = _destRoot.Find(_path);

    //     if ( destChildTrans == null )
    //         return;

    //     // copy the components from the old prefab.
    //     Component[] old_comps = _src.gameObject.GetComponents<Component>();
    //     foreach ( Component old_comp in old_comps ) {
    //         Component new_comp = destChildTrans.gameObject.GetComponent( old_comp.GetType() );  

    //         // if we don't have the component in new prefab, create one.
    //         if ( new_comp == null )
    //             new_comp = destChildTrans.gameObject.AddComponent(old_comp.GetType());

    //         // clone the component in old prefab to the new one.
    //         CompHelper.Copy ( old_comp, new_comp );
    //     }

    //     //
    //     foreach ( Transform child in _src ) {
    //         string child_path = "";
    //         if ( _path == "" )
    //             child_path = child.gameObject.name;
    //         else
    //             child_path = child.gameObject.name + "/" + _path;
    //         CopyComponentsRecursively ( child_path, child, _destRoot );
    //     }
    // }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static void CopyMissingGORecursively( string _path, Transform _destRoot, Transform _src, Transform _dest ) {
        Transform destChildTrans = null;

        if ( _path == "" )
            destChildTrans = _destRoot;
        else
            destChildTrans = _destRoot.Find(_path);

        // if we don't have the child in dest GameObject, copy it from source.
        if ( destChildTrans == null ) {
            GameObject old_GO = _src.gameObject;
            GameObject new_GO = Object.Instantiate( old_GO, _src.localPosition, _src.localRotation ) as GameObject;
            new_GO.name = old_GO.name;

            // destChildTrans = new_GO.transform;
            new_GO.transform.parent = _dest;
            return;
        }

        foreach ( Transform child in _src ) {
            string child_path = "";
            string child_name = child.gameObject.name;

            //
            if ( _path == "" )
                child_path = child_name;
            else
                child_path = _path + "/" + child_name;

            CopyMissingGORecursively ( child_path, _destRoot, child, destChildTrans );
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
        // NOTE: without instantiating, Unity3D not allowed adding new GO in the prefab (since it will break up the prefab). { 
        // NOTE: you must call GameObject.DestroyImmediate(new_prefabGO); after you create the prefab.
        GameObject new_prefabGO_tmp = AssetDatabase.LoadAssetAtPath(new_prefabPath, typeof(GameObject)) as GameObject;
        GameObject new_prefabGO = EditorUtility.InstantiatePrefab ( new_prefabGO_tmp ) as GameObject;  
        // } NOTE end 
        DebugHelper.Assert( new_prefabGO, "Can't find prefab: " + new_prefabPath );

        // copy GO tag & layer
        RemovedUnusedGO( new_prefabGO.transform );
        CopyTagAndLayerRecursively( "", old_prefabGO.transform, new_prefabGO.transform );
        CopyMissingGORecursively( "", new_prefabGO.transform, old_prefabGO.transform, new_prefabGO.transform );

        // DELME { 
        // // copy the components from the old prefab.
        // Component[] old_comps = old_prefabGO.GetComponents<Component>();
        // foreach ( Component old_comp in old_comps ) {
        //     Component new_comp = new_prefabGO.GetComponent( old_comp.GetType() );  

        //     // if we don't have the component in new prefab, create one.
        //     if ( new_comp == null )
        //         new_comp = new_prefabGO.AddComponent(old_comp.GetType());

        //     // clone the component in old prefab to the new one.
        //     CompHelper.Copy ( old_comp, new_comp );
        // }
        // } DELME end 

        // replace the old prefab with the new one
        // NOTE: use ReplacePrefabOptions.ReplaceNameBased, this will never change the position of the prefab-instances in the Scene.
        EditorUtility.ReplacePrefab( new_prefabGO, old_prefabGO, ReplacePrefabOptions.ReplaceNameBased );
        AssetDatabase.DeleteAsset(new_prefabPath);
        GameObject.DestroyImmediate(new_prefabGO);

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
