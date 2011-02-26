using UnityEngine;
using System.Collections;

public class Menu_Controller : MonoBehaviour 
{
	public UIPanelManager mainMenu;		// Our main menu manager
	public UIPanel startUp;
	public UIButton btnBack;
	public UIPanel firstPanelWithBack;
	public UIButton btnSP;
	public UIButton btnMP;
	public UIButton btnKill;
	public UIButton btnCollect;
	public GameObject titleImage;
    
    protected GameObject options;
	
	// Use this for initialization
	void Awake () 
	{
		//btnBack.Hide(true);
		// Do our intro-zoom in 1 second
		Invoke("Begin", 0.5f);
        options = GameObject.Find("MainMenuOptions");
        DebugHelper.Assert( options != null, "can't find MainMenuOptions game object" );
	}
	
	//bring in first panel "tap to start"
	public void Begin()
	{
		// Do our initial intro-zoom at start
		mainMenu.BringIn(startUp);
		Debug.Log("began");

	}
	
	//player tapped to start real menu
	public void startMenu ()
	{
		Hashtable ht = new Hashtable();
		ht.Add("x",285);
		ht.Add("y",10);
		ht.Add("z",50);
		ht.Add("time",0.3);
		Debug.Log("button pressed!");
		iTween.MoveTo(titleImage, ht);
		//btnBack.Hide(false);
	}
	
	//singleplayer choosed
	public void singlePlayer() {
		Debug.Log("singleplayer mode");
        options.GetComponent<MainMenuOptions>().isMultiPlayer = false;
	}
	
	//multiplayer choosed
	public void multiPlayer () {
		Debug.Log("multiplayer mode");
        options.GetComponent<MainMenuOptions>().isMultiPlayer = true;
	}
	
	//zombie killer choosed
	public void missionKill ()
	{
		//todo
		Debug.Log("zombie killer mode");
		//btnBack.Hide(true);
		//after saving variable and stuff, start loading
		Invoke("startLoading",1.0f);
	}
	
	//collector mode choosed
	public void missionCollect ()
	{
		//todo
		Debug.Log("collecting mode");
		//btnBack.Hide(true);
		//after saving variable and stuff, start loading
		Invoke("startLoading",1.0f);
	
		
	}
	
	public void moveForward()
	{
		mainMenu.MoveForward();
	}
	
	public void getBack()
	{
		mainMenu.MoveBack();
		btnSP.SetState(0);
		btnMP.SetState(0);
		btnKill.SetState(0);
		btnCollect.SetState(0);
		//if (mainMenu.CurrentPanel == firstPanelWithBack)
	}
	
	public void startLoading()
	{
	
		Application.LoadLevel("arena_size");
		
	}


}

