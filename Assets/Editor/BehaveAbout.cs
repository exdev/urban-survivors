using UnityEngine;
using UnityEditor;
using System.Collections;

public class BehaveAbout : EditorWindow
{
	private static BehaveAbout instance;
	private Vector3 scroll;
	
	
	
	public BehaveAbout()
	{
		if( instance != null )
		{
			Debug.LogError( "Trying to create two instances of singleton. Self destruction in 3..." );
			Destroy( this );
			return;
		}
		
		instance = this;
		
		title = "About Behave";
		position = new Rect( ( Screen.width - 500.0f ) / 2.0f, ( Screen.height - 400.0f ) / 2.0f, 500.0f, 400.0f );
		minSize = new Vector2( 500.0f, 400.0f );
		maxSize = new Vector2( 500.0f, 400.0f );
	}
	
	
	
	public void OnDestroy()
	{
		instance = null;
	}
	
	
	
	public static BehaveAbout Instance
	{
		get
		{
			if( instance == null )
			{
				new BehaveAbout();
			}
			
			return instance;
		}
	}
	


	public void OnGUI()
	{
		scroll = BehaveLibrary.Resources.About( scroll );
	}
}