using UnityEngine;
using System.Collections;

public class Menu_Controller : MonoBehaviour 
{
	public UIPanelManager mainMenu;		// Our main menu manager
	public UIPanel startUp;
	public UIButton btnBack;
	public UIPanel firstPanelWithBack;
	
	// Use this for initialization
	void Start () 
	{
		btnBack.Hide(true);
		// Do our intro-zoom in 1 second
		Invoke("Begin", 0.5f);
	
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
		Debug.Log("button pressed!");
		btnBack.Hide(false);
	}
	
	//singleplayer choosed
	public void singlePlayer()
	{
		//todo
		Debug.Log("singleplayer mode");
	}
	
	//multiplayer choosed
	public void multiPlayer ()
	{
		//todo
		Debug.Log("multiplayer mode");
	}
	
	//zombie killer choosed
	public void missionKill ()
	{
		//todo
		Debug.Log("zombie killer mode");
		btnBack.Hide(true);
		//after saving variable and stuff, start loading
		Invoke("startLoading",1.0f);
	}
	
	//collector mode choosed
	public void missionCollect ()
	{
		//todo
		Debug.Log("collecting mode");
		btnBack.Hide(true);
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
		//if (mainMenu.CurrentPanel == firstPanelWithBack)
	}
	
	public void startLoading()
	{
	
		Application.LoadLevel("arena_size");
		
	}


}
