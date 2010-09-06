using UnityEngine;
using UnityEditor;
using BehaveLibrary;



public class BehaveMenu : ScriptableObject
{
	[MenuItem( "Help/Behave documentation &%?" )]
	public static void Documentation()
	{
		BehaveLibrary.Resources.Documentation();
	}
	
		
	[MenuItem( "Assets/Behave/Edit library &%e" )]
	public static void EditLibrary()
	{
		EditLibraryAsset( ( BehaveAsset )Selection.activeObject );
	}
	
	
	[MenuItem( "Assets/Behave/Edit library &%e", true )]
	public static bool ValidateEditLibrary()
	{
		return Selection.activeObject is BehaveAsset;
	}
	
	
	[MenuItem( "Assets/Behave/Build library debug &%b" )]
	public static void Compile()
	{
		BehaveAsset asset;
		
		asset = Selection.activeObject as BehaveAsset;
		LibraryAsset library = LibraryAsset.LoadFromData( asset.data, asset );
		
		Compiler.Instance().Compile( library, AssetDatabase.GetAssetPath( asset ), true );
	}
	
	
	[MenuItem( "Assets/Behave/Build library debug &%b", true )]
	public static bool ValidateCompile()
	{
		return Selection.activeObject is BehaveAsset && Compiler.Instance().State == Compiler.CompilerState.Ready;
	}
	
	
	[MenuItem( "Assets/Behave/Build library release #&%b" )]
	public static void CompileRelease()
	{
		BehaveAsset asset;
		
		asset = Selection.activeObject as BehaveAsset;
		LibraryAsset library = LibraryAsset.LoadFromData( asset.data, asset );
		
		Compiler.Instance().Compile( library, AssetDatabase.GetAssetPath( asset ), false );
	}
	
	
	[MenuItem( "Assets/Behave/Build library release #&%b", true )]
	public static bool ValidateCompileRelease()
	{
		return Selection.activeObject is BehaveAsset && Compiler.Instance().State == Compiler.CompilerState.Ready;
	}
	
	
	[MenuItem("Assets/Create/Behave library")]
	public static void Create()
	{
		BehaveAsset asset;
		string name = "NewBehaveLibrary";
		int nameIdx = 0;
		
		while( System.IO.File.Exists( Application.dataPath + "/" + name + nameIdx + ".asset" ) )
		{
			nameIdx++;
		}

		asset = new BehaveAsset();
		asset.Data = ( new LibraryAsset() ).GetData();
		AssetDatabase.CreateAsset( asset, "Assets/" + name + nameIdx + ".asset" );
		Selection.activeObject = asset;
		
        // jwu MODIFY: for unity 2.6 the FocusProjectView changes to FocusProjectWindow { 
        EditorUtility.FocusProjectWindow();
        // } jwu MODIFY end 
		
		EditLibrary();
	}
	
	
	
	[ MenuItem( "Assets/Behave/Create/Collection" ) ]
	public static void CreateCollection()
	{
		BehaveLibrary.Editor.Instance.CreateCollection();
	}
	
	
	
	[ MenuItem( "Assets/Behave/Create/Collection", true ) ]
	public static bool ValidateCreateCollection()
	{
		return BehaveLibrary.Editor.Instance != null && BehaveLibrary.Editor.Instance.ValidateCreateCollection();
	}
	
	
	
	[ MenuItem( "Assets/Behave/Create/Tree" ) ]
	public static void CreateTree()
	{
		BehaveLibrary.Editor.Instance.CreateTree();
	}
	
	
	
	[ MenuItem( "Assets/Behave/Create/Tree", true ) ]
	public static bool ValidateCreateTree()
	{
		return BehaveLibrary.Editor.Instance != null && BehaveLibrary.Editor.Instance.ValidateCreateTree();
	}
	
	

	[ MenuItem( "CONTEXT/Behave/Form new tree" ) ]
	public static void FormNewTree()
	{
		BehaveLibrary.Editor.Instance.FormNewTree();
	}
	
	
	
	[ MenuItem( "CONTEXT/Behave/Form new tree", true ) ]
	public static bool ValidateFormNewTree()
	{
		return BehaveLibrary.Editor.Instance.ValidateFormNewTree();
	}
	
	
	
	[ MenuItem( "CONTEXT/Behave/Purge sub-tree" ) ]
	public static void PurgeSubTree()
	{
		BehaveLibrary.Editor.Instance.PurgeSubTree();
	}
	
	
	
	[ MenuItem( "CONTEXT/Behave/Purge sub-tree", true ) ]
	public static bool ValidatePurgeSubTree()
	{
		return BehaveLibrary.Editor.Instance.ValidatePurgeSubTree();
	}
	
	
	
	[ MenuItem( "Window/Behave browser" ) ]
	public static void ShowBrowser()
	{
		if( BehaveBrowser.Instance == null )
		{
			Debug.LogError( "Failed to set up Behave browser" );
		}
		
		BehaveLibrary.Browser.Instance.Show();
		BehaveLibrary.Browser.Instance.Focus();
		BehaveLibrary.Browser.Instance.Repaint();
	}
	
	
	
	[ MenuItem( "Window/Behave tree editor" ) ]
	public static void ShowTreeEditor()
	{
		if( BehaveTreeEditor.Instance == null )
		{
			Debug.LogError( "Failed to set up Behave tree editor" );
		}
		
		BehaveLibrary.TreeEditor.Instance.Show();
		BehaveLibrary.TreeEditor.Instance.Focus();
		BehaveLibrary.TreeEditor.Instance.Repaint();
	}
	
	
	
	[ MenuItem( "Help/About Behave..." ) ]
	public static void About()
	{
		BehaveAbout.Instance.ShowUtility();
	}
	
	
	
	public static void EditLibraryAsset( BehaveAsset asset )
	{
		BehaveEditor behaveEditor;
		
		if( BehaveLibrary.Editor.Instance == null )
		{
			behaveEditor = new BehaveEditor();
			BehaveLibrary.Editor.Init( behaveEditor );
			behaveEditor.Init();
		}
		
		BehaveLibrary.Editor.Instance.Asset = asset;
		
		Selection.activeObject = asset;
		
		ShowBrowser();
		ShowTreeEditor();
	}
}
