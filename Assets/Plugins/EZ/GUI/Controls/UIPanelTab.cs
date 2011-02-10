//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


using UnityEngine;
using System.Collections;


/// <remarks>
/// Allows functionality similar to a tab when paired
/// with a UIPanel such that when the button is clicked,
/// the associated panel will be shown.
/// By setting up a UIPanelManager with multiple panels
/// and pairing each panel with a UIPanelTab control,
/// you can create the effect of a tabbed interface.
/// When set to toggle, dismissals use the forward
/// transition type.
/// </remarks>
[AddComponentMenu("EZ GUI/Controls/Panel Tab")]
public class UIPanelTab : UIRadioBtn
{
	/// <summary>
	/// When true, the button will toggle the
	/// associated panel on and off with each
	/// press of the tab.
	/// </summary>
	public bool toggle;

	/// <summary>
	/// The optional panel manager that contains the panel(s)
	/// we will be bringing up/dismissing.
	/// This can be left to None/null if there is
	/// only one UIPanelManager object in the scene.
	/// NOTE: For other panels to be hideAtStart when this
	/// one is shown requires the use of a UIPanelManager.
	/// </summary>
	public UIPanelManager panelManager;

	/// <summary>
	/// Reference to the panel to show/hide.
	/// </summary>
	public UIPanelBase panel;

	/// <summary>
	/// Indicates whether the associated panel
	/// is to be considered to be showing at
	/// the start.  This value is only used if
	/// the panel is not associated with a
	/// UIPanelManager.  This allows toggling
	/// to keep track of when the panel needs
	/// to be shown or dismissed.
	/// </summary>
	public bool panelShowingAtStart = true;

	protected bool panelIsShowing = true;

	public override void Start()
	{
		base.Start();

		panelIsShowing = panelShowingAtStart;

		// Try to get a manager:
		if (panelManager == null)
		{
			if(panel != null)
				if (panel.Container != null)
					panelManager = (UIPanelManager) panel.Container;

			// If we still don't have anything:
/*
			if(panelManager == null)
				if (UIPanelManager.instance != null)
					panelManager = UIPanelManager.instance;
*/
		}

		Value = panelIsShowing;

		// Since hiding while managed depends on
		// setting our mesh extents to 0, and the
		// foregoing code causes us to not be set
		// to 0, re-hide ourselves:
		if (managed && m_hidden)
			Hide(true);
	}

	public override void OnInput(ref POINTER_INFO ptr)
	{
		if (deleted)
			return;

		base.OnInput(ref ptr);

		if (!m_controlIsEnabled || IsHidden())
		{
			return;
		}

		if(panel == null)
			return;

		if (ptr.evt == whenToInvoke)
		{
			if(toggle)
			{
				if(panelManager != null)
				{
					if (panelManager.CurrentPanel == panel)
					{
						panelManager.Dismiss(UIPanelManager.MENU_DIRECTION.Forwards);
						panelIsShowing = false;
					}
					else
					{
						panelManager.BringIn(panel);
						panelIsShowing = true;
					}
				}
				else
				{
					if(panelIsShowing)
						panel.StartTransition(UIPanelManager.SHOW_MODE.DismissForward);
					else
						panel.StartTransition(UIPanelManager.SHOW_MODE.BringInForward);

					panelIsShowing = !panelIsShowing;
				}

				Value = panelIsShowing;
			}
			else
			{
				if (panelManager != null)
					panelManager.BringIn(panel, UIPanelManager.MENU_DIRECTION.Forwards);
				else
					panel.StartTransition(UIPanelManager.SHOW_MODE.BringInForward);
			}
		}
	}

	public override void Copy(SpriteRoot s)
	{
		Copy(s, ControlCopyFlags.All);
	}

	public override void Copy(SpriteRoot s, ControlCopyFlags flags)
	{
		base.Copy(s, flags);

		if (!(s is UIPanelTab))
			return;

		UIPanelTab b = (UIPanelTab)s;

		if ((flags & ControlCopyFlags.Settings) == ControlCopyFlags.Settings)
		{
			toggle = b.toggle;
			panelManager = b.panelManager;
			panel = b.panel;
			panelShowingAtStart = b.panelShowingAtStart;
		}

		if ((flags & ControlCopyFlags.State) == ControlCopyFlags.State)
		{
			panelIsShowing = b.panelIsShowing;
		}
	}


	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <returns>Returns a reference to the component.</returns>
	new static public UIPanelTab Create(string name, Vector3 pos)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		return (UIPanelTab)go.AddComponent(typeof(UIPanelTab));
	}

	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <param name="rotation">Rotation of the object.</param>
	/// <returns>Returns a reference to the component.</returns>
	new static public UIPanelTab Create(string name, Vector3 pos, Quaternion rotation)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		go.transform.rotation = rotation;
		return (UIPanelTab)go.AddComponent(typeof(UIPanelTab));
	}
}
