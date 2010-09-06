using UnityEngine;
using UnityEditor;
using BehaveLibrary;

public class BehaveBrowser : EditorWindow, IBrowserWindow
{
	private static BehaveBrowser instance;


	public BehaveBrowser()
	{
		if( instance != null )
		{
			Debug.LogError( "Trying to create two instances of singleton. Self destruction in 3..." );
			Destroy( this );
			return;
		}
		
		BehaveLibrary.Browser.Init( this );
		
		instance = this;
		
		title = "Behave browser";
	}
	
	
	
	public void OnDestroy()
	{
		instance = null;
		BehaveLibrary.Browser.Instance.OnDestroy();
	}
	
	
	
	public static BehaveBrowser Instance
	{
		get
		{
			if( instance == null )
			{
				new BehaveBrowser();
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
    


    public BehaveLibrary.Browser Browser
    {
        get
		{
			return BehaveLibrary.Browser.Instance;
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
		Browser.OnGUI();
	}
}
