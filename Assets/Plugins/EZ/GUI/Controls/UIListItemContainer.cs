//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------

#define AUTO_SET_LAYER
#define SET_LAYERS_RECURSIVELY


using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <remarks>
/// UIListItemContainer can be added to a scroll list, but does
/// not provide any control functionality of its own.  Instead,
/// it serves as a container for one or more other controls that
/// will appear within the scroll list.  In this way, it is 
/// somewhat similar to a panel.  Like a panel, to cause it to
/// contain other controls, place them as children to the
/// UIListItemContainer's GameObject.
/// </remarks>
[System.Serializable]
[AddComponentMenu("EZ GUI/Controls/List Item Container")]
public class UIListItemContainer : ControlBase, IUIListObject, IUIContainer
{
	// Array of UI Objects (buttons, etc)
	protected List<SpriteRoot> uiObjs = new List<SpriteRoot>();

	// Array of text objects:
	protected List<SpriteText> textObjs = new List<SpriteText>();

	protected bool m_started;



	// Use this for initialization
	public virtual void Start () 
	{
		// Only run start once!
		if (m_started)
			return;

		m_started = true;

		ScanChildren();
	}

	// Scans all child objects looking for SpriteRoot and SpriteText objects
	public void ScanChildren()
	{
		uiObjs.Clear();

		SpriteRoot obj;
		Component[] objs = transform.GetComponentsInChildren(typeof(SpriteRoot), true);

		for (int i = 0; i < objs.Length; ++i)
		{
			// Don't add ourselves as children:
			if (objs[i] == this)
				continue;

#if AUTO_SET_LAYER
			// Only reset the child object layers if we're in-line
			// with the UIManager:
			if (gameObject.layer == UIManager.instance.gameObject.layer)
#if SET_LAYERS_RECURSIVELY
				UIPanelManager.SetLayerRecursively(objs[i].gameObject, gameObject.layer);
	#else
				objs[i].gameObject.layer = gameObject.layer;
	#endif
#endif
			obj = (SpriteRoot)objs[i];
			if(obj is AutoSpriteControlBase)
			{
				if(((AutoSpriteControlBase)obj).RequestContainership(this))
					uiObjs.Add(obj);
			}
			else // Just add it:
				uiObjs.Add(obj);
		}

		//----------------------------------------------------
		// Again for SpriteText:
		//----------------------------------------------------

		textObjs.Clear();

		SpriteText txt;
		Component[] txts = transform.GetComponentsInChildren(typeof(SpriteText), true);

		for (int i = 0; i < txts.Length; ++i)
		{
			// Don't add ourselves as children:
			if (txts[i] == this)
				continue;

			// Only process text objects that aren't already associated
			// with controls:
			txt = (SpriteText)txts[i];

			if (txt.Parent != null)
				continue;

#if AUTO_SET_LAYER
			// Only reset the child object layers if we're in-line
			// with the UIManager:
			if (gameObject.layer == UIManager.instance.gameObject.layer)
#if SET_LAYERS_RECURSIVELY
				UIPanelManager.SetLayerRecursively(txt.gameObject, gameObject.layer);
#else
				txt.gameObject.layer = gameObject.layer;
#endif
#endif
			textObjs.Add(txt);
		}
	}


	public void AddChild(GameObject go)
	{
		SpriteRoot obj = (SpriteRoot)go.GetComponent(typeof(SpriteRoot));

		if(obj != null)
		{
			if(obj is AutoSpriteControlBase)
			{
				if (((AutoSpriteControlBase)obj).Container != (IUIContainer)this)
					((AutoSpriteControlBase)obj).Container = this;
			}
			uiObjs.Add(obj);
		}
		else
		{
			SpriteText txt = (SpriteText)go.GetComponent(typeof(SpriteText));
			if (txt != null)
			{
				textObjs.Add(txt);
			}
		}
	}

	public void RemoveChild(GameObject go)
	{
		SpriteRoot obj = (SpriteRoot)go.GetComponent(typeof(SpriteRoot));

		if (obj != null)
		{
			for(int i=0; i<uiObjs.Count; ++i)
			{
				if(uiObjs[i] == obj)
				{
					uiObjs.RemoveAt(i);
					break;
				}
			}

			if (obj is AutoSpriteControlBase)
				if (((AutoSpriteControlBase)obj).Container == (IUIContainer)this)
					((AutoSpriteControlBase)obj).Container = null;
		}
		else
		{
			SpriteText txt = (SpriteText)go.GetComponent(typeof(SpriteText));
			if (txt != null)
			{
				for (int i = 0; i < textObjs.Count; ++i)
				{
					if (textObjs[i] == txt)
					{
						textObjs.RemoveAt(i);
						break;
					}
				}
			}
		}
	}

	/// <summary>
	/// Makes the specified GameObject a child of
	/// this container, including making it a child of
	/// the container's transform.
	/// </summary>
	/// <param name="go">GameObject to make a child of the panel.</param>
	public void MakeChild(GameObject go)
	{
		AddChild(go);

		go.transform.parent = transform;
	}

	/// <summary>
	/// Searches for and returns the first control/element
	/// it encounters with the specified name that
	/// is a child of this container.
	/// </summary>
	/// <param name="elementName">Name of the control/element to be found.</param>
	/// <returns>Returns the first control/element found with the specified name. Null is returned if no matching control is found.</returns>
	public SpriteRoot GetElement(string elementName)
	{
		for (int i = 0; i < uiObjs.Count; ++i)
			if (uiObjs[i].name == elementName)
				return uiObjs[i];

		return null;
	}


	/// <summary>
	/// Searches for and returns the first SpriteText
	/// it encounters with the specified name that
	/// is a child of this container.
	/// </summary>
	/// <param name="elementName">Name of the text element to be found.</param>
	/// <returns>Returns the first text element found with the specified name. Null is returned if no matching control is found.</returns>
	public SpriteText GetTextElement(string elementName)
	{
		for (int i = 0; i < textObjs.Count; ++i)
			if (textObjs[i].name == elementName)
				return textObjs[i];

		return null;
	}


	public override void OnInput(POINTER_INFO ptr)
	{
		if (!m_controlIsEnabled)
		{
			switch (ptr.evt)
			{
				case POINTER_INFO.INPUT_EVENT.NO_CHANGE:
				case POINTER_INFO.INPUT_EVENT.DRAG:
					list.ListDragged(ptr);
					break;
				case POINTER_INFO.INPUT_EVENT.TAP:
				case POINTER_INFO.INPUT_EVENT.RELEASE:
				case POINTER_INFO.INPUT_EVENT.RELEASE_OFF:
					list.PointerReleased();
					break;
			}

			if (Container != null)
			{
				ptr.callerIsControl = true;
				Container.OnInput(ptr);
			}

			return;
		}

		// Do our own tap checking with the list's
		// own threshold:
		if (Vector3.SqrMagnitude(ptr.origPos - ptr.devicePos) > (list.dragThreshold * list.dragThreshold))
		{
			ptr.isTap = false;
			if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
				ptr.evt = POINTER_INFO.INPUT_EVENT.RELEASE;
		}

		if (inputDelegate != null)
			inputDelegate(ref ptr);

		// Change the state if necessary:
		switch (ptr.evt)
		{
			case POINTER_INFO.INPUT_EVENT.NO_CHANGE:
				if (ptr.active)	// If this is a hold
					list.ListDragged(ptr);
				break;
			case POINTER_INFO.INPUT_EVENT.DRAG:
				if (!ptr.isTap)
					list.ListDragged(ptr);
				break;
			case POINTER_INFO.INPUT_EVENT.TAP:
				list.PointerReleased();
				break;
			case POINTER_INFO.INPUT_EVENT.RELEASE:
			case POINTER_INFO.INPUT_EVENT.RELEASE_OFF:
				list.PointerReleased();
				break;
		}

		base.OnInput(ptr);
	}

	public override EZTransitionList GetTransitions(int index)
	{
		return null;
	}

	public override EZTransitionList[] Transitions
	{
		get { return null; }
		set { }
	}

	public override string[] States
	{
		get { return null; }
	}


	// IUIListObject interface compliance:
	Vector2 topLeftEdge, bottomRightEdge;
	Rect3D clippingRect;
	bool clipped = false;
	UIScrollList list;
	protected int index;
	bool m_selected = false;


	public bool IsContainer()
	{
		return true;
	}

	public void FindOuterEdges()
	{
		if (!m_started)
			Start();

		topLeftEdge = Vector2.zero;
		bottomRightEdge = Vector2.zero;

		Matrix4x4 sm;
		Matrix4x4 lm = transform.worldToLocalMatrix;
		Vector3 tl, br;

		if (spriteText != null)
		{
			sm = spriteText.transform.localToWorldMatrix;
			tl = lm.MultiplyPoint3x4(sm.MultiplyPoint3x4(spriteText.UnclippedTopLeft));
			br = lm.MultiplyPoint3x4(sm.MultiplyPoint3x4(spriteText.UnclippedBottomRight));
			topLeftEdge.x = Mathf.Min(topLeftEdge.x, tl.x);
			topLeftEdge.y = Mathf.Max(topLeftEdge.y, tl.y);
			bottomRightEdge.x = Mathf.Max(bottomRightEdge.x, br.x);
			bottomRightEdge.y = Mathf.Min(bottomRightEdge.y, br.y);
		}

		// Search SpriteText objects:
		for (int i = 0; i < textObjs.Count; ++i)
		{
			sm = textObjs[i].transform.localToWorldMatrix;
			tl = lm.MultiplyPoint3x4(sm.MultiplyPoint3x4(textObjs[i].UnclippedTopLeft));
			br = lm.MultiplyPoint3x4(sm.MultiplyPoint3x4(textObjs[i].UnclippedBottomRight));
			topLeftEdge.x = Mathf.Min(topLeftEdge.x, tl.x);
			topLeftEdge.y = Mathf.Max(topLeftEdge.y, tl.y);
			bottomRightEdge.x = Mathf.Max(bottomRightEdge.x, br.x);
			bottomRightEdge.y = Mathf.Min(bottomRightEdge.y, br.y);
		}

		// Search sprites and controls:
		for (int i = 0; i < uiObjs.Count; ++i)
		{
			sm = uiObjs[i].transform.localToWorldMatrix;

			if (uiObjs[i] is AutoSpriteControlBase)
			{
				((AutoSpriteControlBase)uiObjs[i]).FindOuterEdges();
				tl = lm.MultiplyPoint3x4(sm.MultiplyPoint3x4(((AutoSpriteControlBase)uiObjs[i]).TopLeftEdge));
				br = lm.MultiplyPoint3x4(sm.MultiplyPoint3x4(((AutoSpriteControlBase)uiObjs[i]).BottomRightEdge));
			}
			else
			{
				tl = lm.MultiplyPoint3x4(sm.MultiplyPoint3x4(uiObjs[i].UnclippedTopLeft));
				br = lm.MultiplyPoint3x4(sm.MultiplyPoint3x4(uiObjs[i].UnclippedBottomRight));
			}
			topLeftEdge.x = Mathf.Min(topLeftEdge.x, tl.x);
			topLeftEdge.y = Mathf.Max(topLeftEdge.y, tl.y);
			bottomRightEdge.x = Mathf.Max(bottomRightEdge.x, br.x);
			bottomRightEdge.y = Mathf.Min(bottomRightEdge.y, br.y);
		}
	}

	public Vector2 TopLeftEdge
	{
		get { return topLeftEdge; }
	}

	public Vector2 BottomRightEdge
	{
		get { return bottomRightEdge; }
	}

	public void Hide(bool tf)
	{
		for (int i = 0; i < uiObjs.Count; ++i)
			uiObjs[i].Hide(tf);

		for (int i = 0; i < textObjs.Count; ++i)
			textObjs[i].Hide(tf);

		if (spriteText != null)
			spriteText.Hide(tf);
	}

	public bool Managed
	{
		get { return false; }
	}

	public Rect3D ClippingRect
	{
		get { return clippingRect; }
		set
		{
			clipped = true;
			clippingRect = value;

			for (int i = 0; i < uiObjs.Count; ++i)
				uiObjs[i].ClippingRect = value;

			for (int i = 0; i < textObjs.Count; ++i)
				textObjs[i].ClippingRect = value;

			if (spriteText != null)
				spriteText.ClippingRect = value;
		}
	}

	public bool Clipped
	{
		get { return clipped; }

		set
		{
			if (value && !clipped)
			{
				clipped = true;
				ClippingRect = clippingRect;
			}
			else if (clipped)
				Unclip();

			clipped = value;
		}
	}

	public void Unclip()
	{
		clipped = false;

		for (int i = 0; i < uiObjs.Count; ++i)
			uiObjs[i].Unclip();

		for (int i = 0; i < textObjs.Count; ++i)
			textObjs[i].Unclip();

		if (spriteText != null)
			spriteText.Unclip();
	}

	public override void UpdateCollider()
	{
		for (int i = 0; i < uiObjs.Count; ++i)
			if(uiObjs[i] is AutoSpriteControlBase)
				((AutoSpriteControlBase)uiObjs[i]).UpdateCollider();
	}

	public void SetList(UIScrollList c)
	{
		list = c;
	}

	public int Index
	{
		get { return index; }
		set { index = value; }
	}

	public SpriteText TextObj
	{
		get { return spriteText; }
	}

	public bool selected
	{
		get { return m_selected; }
		set
		{
			m_selected = value;
		}
	}

	public void Delete()
	{
		for (int i = 0; i < uiObjs.Count; ++i)
			uiObjs[i].Delete();

		for (int i = 0; i < textObjs.Count; ++i)
			textObjs[i].Delete();
	}
}