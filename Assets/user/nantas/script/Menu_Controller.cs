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
	
	public void Begin()
	{
		// Do our initial intro-zoom at start
		mainMenu.BringIn(startUp);
		Debug.Log("began");
		
		// Move our fog to match:
		//MoveForward();
	}
	
	public void buttonPressed()
	{
		Debug.Log("button pressed!");
		btnBack.Hide(false);
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
		btnBack.Delete();
		
	}


}

