// ======================================================================================
// File         : SyncAnimCurve.cs
// Author       : Wu Jie 
// Last Change  : 12/16/2010 | 15:20:51 PM | Thursday,December
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEditor;
using UnityEngine;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////
// class SyncAnimCurve 
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

public class SyncAnimCurve
{
    const string duplicatePostfix = "_copy";

    static void CopyClip(string importedPath, string copyPath)
    {
        AnimationClip src = AssetDatabase.LoadAssetAtPath(importedPath, typeof(AnimationClip)) as AnimationClip;
        AnimationClip newClip = new AnimationClip();
        newClip.name = src.name + duplicatePostfix;
        AssetDatabase.CreateAsset(newClip, copyPath);
        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/Sync Animation Curve")]
    static void CopyCurvesToDuplicate()
    {
        // Get selected AnimationClip
        AnimationClip imported = Selection.activeObject as AnimationClip;
        if (imported == null)
        {
            EditorUtility.DisplayDialog ("Error", "the file you selected is is not an AnimationClip", "OK");
            return;
        }

        // Find path of copy
        string importedPath = AssetDatabase.GetAssetPath(imported);
        string copyPath = importedPath.Substring(0, importedPath.LastIndexOf("/"));
        copyPath += "/" + imported.name + duplicatePostfix + ".anim";

        AnimationClip copy = AssetDatabase.LoadAssetAtPath(copyPath, typeof(AnimationClip)) as AnimationClip;
        bool direct_copy = false;
        if ( copy == null ) // if the animclip_copy exists, we should get store its AnimationEvents
        {
            CopyClip(importedPath, copyPath);
            direct_copy = true;
        }

        // now, check the copy again to ensure it exists.
        copy = AssetDatabase.LoadAssetAtPath(copyPath, typeof(AnimationClip)) as AnimationClip;
        if (copy == null) {
            EditorUtility.DisplayDialog ("Error", "No copy found at " + copyPath, "OK");
            return;
        }
        // Copy curves from imported to copy
        AnimationClipCurveData[] src_curveData = AnimationUtility.GetAllCurves(imported, true);
        direct_copy = true; // TEMP HACK: you need to finish the TODO to delete this.
        if ( direct_copy ) {
            for ( int i = 0; i < src_curveData.Length; ++i ) {
                AnimationUtility.SetEditorCurve( copy,
                                                 src_curveData[i].path,
                                                 src_curveData[i].type,
                                                 src_curveData[i].propertyName,
                                                 src_curveData[i].curve );
            }
        }
        else {
            // TODO: not finish yet! { 
            // AnimationClipCurveData[] copy_curveData = AnimationUtility.GetAllCurves(copy, true);
            // for ( int i = 0; i < src_curveData.Length; ++i ) {
            //     for ( int j = 0; j < copy_curveData.Length; ++j ) {
            //         if ( src_curveData[i].path == copy_curveData[j].path &&
            //              src_curveData[i].type == copy_curveData[j].type &&
            //              src_curveData[i].propertyName == copy_curveData[j].propertyName ) {
            //             AnimationUtility.SetEditorCurve( copy,
            //                                              src_curveData[i].path,
            //                                              src_curveData[i].type,
            //                                              src_curveData[i].propertyName,
            //                                              src_curveData[i].curve );
            //             copy_curveData.RemoveAt(j);
            //             break;
            //         }
            //     }
            // }
            // } TODO end 
        }


        Debug.Log("Animation Curve " + copy.name + " syncing successed!");
    }
}
