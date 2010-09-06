using UnityEngine;
using UnityEditor;
using System.Collections;
using BehaveLibrary;

public class BehaveEditor : ScriptableObject, IEditorWindow
{
	private static BehaveEditor instance;
	private BehaveLibrary.Editor editor;
	
	
	
	public BehaveEditor()
	{
		if( instance != null )
		{
			Debug.LogError( "Trying to create two instances of singleton. Self destruction in 3..." );
			DestroyImmediate( this );
			return;
		}
		
		if( this.Editor == null )
		{
			Debug.LogError( "Failed to link with library implementation" );
			DestroyImmediate( this );
			return;
		}
		
		instance = this;
	}
	
	
	
	public void OnDestroy()
	{
		Editor.OnDestroy();
		instance = null;
	}
	
	
	
	public static BehaveEditor Instance
	{
		get
		{
			if( instance == null )
			{
				new BehaveEditor();
			}
			
			return instance;
		}
	}
	
	
	
	public void Init()
	{
		this.editor = BehaveLibrary.Editor.Instance;
	}
	
	
	
    public BehaveLibrary.Editor Editor
    {
        get
		{
			if( editor == null )
			{
				if( BehaveLibrary.Editor.Instance == null )
				{
					BehaveLibrary.Editor.Init( this );
				}
				
				Init();
			}
			
			return editor;
		}
    }
    


    public IBehaveAsset SelectedAsset
    {
        get
		{
			return Selection.activeObject as BehaveAsset;
		}
    }



	public void SaveLibrary( LibraryAsset libraryAsset, IBehaveAsset behaveAsset )
	{
		behaveAsset.Data = libraryAsset.GetData();
		EditorUtility.SetDirty( ( BehaveAsset )behaveAsset );
	}
	
	
	
	public string GetLibraryName( IBehaveAsset asset )
	{
		string name;
		
		name = AssetDatabase.GetAssetPath( ( BehaveAsset )asset );
		name = name.Substring( name.LastIndexOf( "/" ) + 1 );
		name = name.Substring( 0, name.LastIndexOf( "." ) );
		
		return name;
	}
}