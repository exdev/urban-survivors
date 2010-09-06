using UnityEngine;
using UnityEditor;
using System.Collections;
using BehaveLibrary;

[ CustomEditor( typeof( BehaveAsset ) ) ]
public class BehaveAssetEditor : UnityEditor.Editor
{
    // jwu MODIFY: I'm not sure { 
	override public void OnInspectorGUI()
    // } jwu MODIFY end 
	{
		EditorGUILayout.Separator();
		GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label( BehaveLibrary.Resources.BehaveLogo );
			GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		EditorGUILayout.Separator();
		
		if( BehaveMenu.ValidateEditLibrary() && GUILayout.Button( "Edit library" ) )
		{
			BehaveMenu.EditLibrary();
		}
		
		EditorGUILayout.Separator();
		
		if( BehaveMenu.ValidateCompile() && GUILayout.Button( "Build library debug" ) )
		{
			BehaveMenu.Compile();
		}
		
		if( BehaveMenu.ValidateCompileRelease() && GUILayout.Button( "Build library release" ) )
		{
			BehaveMenu.CompileRelease();
		}
		
		EditorGUILayout.Separator();
		
		GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label( "Behave version " + BehaveLibrary.Resources.Version );
		GUILayout.EndHorizontal();
	}
}
