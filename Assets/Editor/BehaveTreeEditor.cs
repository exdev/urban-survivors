using UnityEngine;
using UnityEditor;
using BehaveLibrary;



public class BehaveTreeEditor : EditorWindow, ITreeEditorWindow
{
	private static BehaveTreeEditor instance;


	public BehaveTreeEditor()
	{
		if( instance != null )
		{
			Debug.LogError( "Trying to create two instances of singleton. Self destruction in 3..." );
			Destroy( this );
			return;
		}
		
		BehaveLibrary.TreeEditor.Init( this );
		
		instance = this;
		
		title = "Behave editor";
	}
	
	
	
	public void OnDestroy()
	{
		instance = null;
		BehaveLibrary.TreeEditor.Instance.OnDestroy();
	}
	
	
	
	public void OnFocus()
	{
		if( BehaveEditor.Instance != null )
		{
			Selection.activeObject = BehaveEditor.Instance;
		}
	}
	
	
	
	public static BehaveTreeEditor Instance
	{
		get
		{
			if( instance == null )
			{
				new BehaveTreeEditor();
			}
			
			return instance;
		}
	}
	
	
	
    public Rect Position
    {
        get
		{
			return position;
		}
        set
		{
			this.position = value;
		}
    }
    


    public BehaveLibrary.TreeEditor Editor
    {
        get
		{
			return BehaveLibrary.TreeEditor.Instance;
		}
    }
    


    new public void Repaint()
	{
		base.Repaint();
	}
	
	
	
    new public void Close()
	{
		base.Close();
	}
	
	
	
    new public void Show()
	{
		base.Show();
	}
	
	
	
	new public void Focus()
	{
		base.Focus();
	}
	
	
	
	public bool HasFocus
	{
		get
		{
			return EditorWindow.focusedWindow == this;
		}
	}
	
	
	
	public void OnGUI()
	{
		Editor.OnGUI();
	}
}
