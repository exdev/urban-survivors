//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


using UnityEngine;
using System.Collections;


/// <remarks>
/// A button that loads the specified scene when clicked.
/// </remakrs>
[AddComponentMenu("EZ GUI/Controls/Load Scene Button")]
public class UIBtnLoadScene : UIButton
{
	/// <summary>
	/// Name of the scene to load when the button is clicked.
	/// </summary>
	public string scene;

	/// <summary>
	/// Optional panel to bring into view just prior to loading.
	/// Use this to display a "Loading..." type message.
	/// </summary>
	public UIPanelBase loadingPanel;

	// Called when the loading panel's transition ends.
	public void LoadSceneDelegate(UIPanelBase panel, EZTransition trans)
	{
		LoadScene();
	}

	public override void OnInput(ref POINTER_INFO ptr)
	{
		base.OnInput(ref ptr);

		if (!m_controlIsEnabled || IsHidden())
			return;

		if (ptr.evt == whenToInvoke)
		{
			if (loadingPanel != null)
			{
				UIPanelManager mgr = (UIPanelManager)loadingPanel.Container;
				if (mgr is UIPanelManager && mgr != null)
				{
					loadingPanel.SetTempTransitionDelegate(LoadSceneDelegate);
					mgr.BringIn(loadingPanel);
				}
				else
				{
					Debug.LogWarning("No panel manager found for panel \"" + loadingPanel.name + "\"!");
				}
			}
			else
				Invoke("LoadScene", delay);
		}
	}

	// Method that loads the scene:
	protected void LoadScene()
	{
		Application.LoadLevel(scene);
	}

	public override void Copy(SpriteRoot s)
	{
		Copy(s, ControlCopyFlags.All);
	}

	public override void Copy(SpriteRoot s, ControlCopyFlags flags)
	{
		base.Copy(s, flags);

		if (!(s is UIBtnLoadScene))
			return;

		UIBtnLoadScene b = (UIBtnLoadScene)s;

		if ((flags & ControlCopyFlags.Settings) == ControlCopyFlags.Settings)
		{
			scene = b.scene;
			loadingPanel = b.loadingPanel;
		}
	}


	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <returns>Returns a reference to the component.</returns>
	new static public UIBtnLoadScene Create(string name, Vector3 pos)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		return (UIBtnLoadScene)go.AddComponent(typeof(UIBtnLoadScene));
	}

	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <param name="rotation">Rotation of the object.</param>
	/// <returns>Returns a reference to the component.</returns>
	new static public UIBtnLoadScene Create(string name, Vector3 pos, Quaternion rotation)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		go.transform.rotation = rotation;
		return (UIBtnLoadScene)go.AddComponent(typeof(UIBtnLoadScene));
	}
}
