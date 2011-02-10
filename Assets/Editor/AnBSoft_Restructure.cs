//-----------------------------------------------------------------
//  Copyright 2011 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


using UnityEngine;
using UnityEditor;
using System.IO;


public class AnBSoft_Restructure : ScriptableObject
{
	static string srcFolder; // The current source folder
	static string destFolder; // The current destination folder
	
	[MenuItem("Tools/Restructure A&B Soft folders")]
	static void Restructure()
	{
		// Source folders:
		string assets = Application.dataPath + "/";
		string srcEditorRoot = assets + "Editor/";
		string srcEditors = srcEditorRoot + "Editors";
		string srcInspectors = srcEditorRoot + "Inspectors";
		string srcWizards = srcEditorRoot + "Wizards";
		string srcFont = assets + "Font/";
		string srcMaterials = assets + "Materials/";
		string srcShaders = assets + "Shaders/";
		
		// Destination folders:
		string destAnBRoot = assets + "Plugins/AnBSoft Common/";
		string destAnBEditorRoot = srcEditorRoot + "AnBSoft/";
		string destAnBEditors = destAnBEditorRoot + "Editors";
		string destAnBInspectors = destAnBEditorRoot + "Inspectors";
		string destAnBWizards = destAnBEditorRoot + "Wizards";
		string destEZGUIFont = destAnBRoot + "Standard Font/";
		string destCommonMaterial = destAnBRoot + "Standard Material/";
		string destCommonShaders = destAnBRoot + "Standard Shaders/";
		

		//////////////////////////////////////////////////////////
		// Move Editor files:
		//////////////////////////////////////////////////////////
				
		CreateFolder(destAnBEditorRoot);
		MoveFolder(srcEditors, destAnBEditors);
		MoveFolder(srcWizards, destAnBWizards);
		MoveFolder(srcInspectors, destAnBInspectors);
		
		srcFolder = srcEditorRoot;
		destFolder = destAnBInspectors + "/";
		
		if(!Directory.Exists(destAnBInspectors))
			CreateFolder(destAnBInspectors);
		
		Move("GameSaverEditor.cs");
		Move("SaveGameManagerHelperEditor.cs");
				
		//////////////////////////////////////////////////////////
		// Move Font files:
		//////////////////////////////////////////////////////////
		srcFolder = srcFont;
		destFolder = destEZGUIFont;

		if(File.Exists(srcFont + "Hoefler text.txt"))
			CreateFolder(destFolder);
		
		Move("Hoefler text.png");
		Move("Hoefler text.mat");
		Move("Hoefler text.txt");
		
		//////////////////////////////////////////////////////////
		// Move Material files:
		//////////////////////////////////////////////////////////
		srcFolder = srcMaterials;
		destFolder = destCommonMaterial;
		
		if(File.Exists(srcMaterials + "Sprite Material.mat"))
			CreateFolder(destFolder);
		
		Move("Sprite Material.mat");
		
		//////////////////////////////////////////////////////////
		// Move Shader files:
		//////////////////////////////////////////////////////////
		srcFolder = srcShaders;
		destFolder = destCommonShaders;
		
		if(File.Exists(srcShaders + "Transparent Vertex Colored.shader"))
			CreateFolder(destFolder);
		
		Move("Sprite Cutout.shader");
		Move("Transparent Vertex Colored.shader");
		
		
		//////////////////////////////////////////////////////////
		// Clean up:
		//////////////////////////////////////////////////////////
		RemoveFolder(srcFont);
		RemoveFolder(srcMaterials);
		RemoveFolder(srcShaders);
		AssetDatabase.Refresh();
		
			
		// Display finished message:
		EditorUtility.DisplayDialog("Finished!", "Folder restructring completed.", "OK");
	}
			
			
	protected static void Move(string file)
	{
		string assets = Application.dataPath;
		string src = srcFolder.Substring(assets.Length-6);
		string dest = destFolder.Substring(assets.Length-6);
		
		string ret = AssetDatabase.MoveAsset(src + file, dest + file);
		
		if(!string.IsNullOrEmpty(ret))
		{
			//Debug.Log("Src: " + src + file + "\n" + "Dst: " + dest + file + "\n" + ret);
		}
	}
	
	protected static void MoveFolder(string src, string dest)
	{
		try
		{
			Directory.Move(src, dest + "/");
		}
		catch
		{
			// Ignore...
		}
	}
		
	protected static void CreateFolder(string name)
	{
		try
		{
			Directory.CreateDirectory(name);
			AssetDatabase.Refresh();
		}
		catch
		{
			// Ignore...
		}
	}
	
	protected static void RemoveFolder(string name)
	{
		try
		{
			Directory.Delete(name);
		}
		catch
		{
			// Ignore...
		}
	}
}