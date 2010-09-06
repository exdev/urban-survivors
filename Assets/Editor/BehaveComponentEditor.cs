using UnityEngine;
using UnityEditor;
using System.Collections;
using BehaveLibrary;

[ CustomEditor( typeof( BehaveEditor ) ) ]
public class BehaveComponentEditor : UnityEditor.Editor
{
    // jwu MODIFY: I'm not sure { 
	override public void OnInspectorGUI()
    // } jwu MODIFY end 
	{
		if( TreeEditor.Instance == null )
		{
			GUILayout.Label( "No tree editor loaded" );
			return;
		}
		
		TreeEditor.Instance.OnInspectorGUI();
	}
}
