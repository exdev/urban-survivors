//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------

#define AUTO_ORIENT_ITEMS
#define AUTO_SCALE_ITEMS



using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// Interface for any list object that is to be
// part of a UIScrollList list.
public interface IUIListObject : IUIObject
{
	/// <summary>
	/// Returns whether this object serves as a
	/// container for other controls.
	/// </summary>
	/// <returns>True if a container, false otherwise.</returns>
	bool IsContainer();

/*	/// <summary>
	/// Gets/sets whether this object is a clone
	/// (has been cloned from a prefab or other
	/// scene object).
	/// </summary>
	bool IsClone
	{
		get;
		set;
	}
*/
	//  Finds the outer edges of the object.
	void FindOuterEdges();

	/// <summary>
	/// The position of the top-left edge of the object.
	/// </summary>
	Vector2 TopLeftEdge { get; }

	/// <summary>
	/// The position of the bottom-right edge of the object.
	/// </summary>
	Vector2 BottomRightEdge { get; }

	/// <summary>
	/// Hides the object.
	/// </summary>
	/// <param name="tf">Whether to hide the object or not.</param>
	void Hide(bool tf);

	bool Managed
	{
		get;
	}

	/// <summary>
	/// Sets the Rect (specified in world space)
	/// against which this object will be clipped.
	/// </summary>
	Rect3D ClippingRect
	{
		get;
		set;
	}

	/// <summary>
	/// Accessor for whether or not the object is
	/// to be clipped against a clipping rect.
	/// </summary>
	bool Clipped
	{
		get;
		set;
	}

	/// <summary>
	/// Un-clips the object (undoes any clipping
	/// specified by ClippingRect).
	/// </summary>
	void Unclip();

	/// <summary>
	/// Updates the object's collider based upon the
	/// extents of its content.
	/// </summary>
	void UpdateCollider();

	GameObject gameObject
	{
		get;
	}

	Transform transform
	{
		get;
	}

	// Sets the scroll list that contains this object.
	void SetList(UIScrollList c);

	/// <summary>
	/// The index of the object in the list.
	/// </summary>
	int Index
	{
		get;
		set;
	}

	/// <summary>
	/// The text to be displayed by the object
	/// </summary>
	string Text
	{
		get;
		set;
	}

	/// <summary>
	/// Gets the reference to the SpriteText object
	/// attached to this object, if any. (Null if
	/// there is none.)
	/// </summary>
	SpriteText TextObj
	{
		get;
	}

	/// <summary>
	/// Holds "boxed" data for the control.
	/// This can be used to associate any
	/// object or value with the control
	/// for later reference and use.
	/// </summary>
	object Data
	{
		get;
		set;
	}

	/// <summary>
	/// Sets/Gets whether this object is the currently
	/// selected item in the list.
	/// </summary>
	bool selected
	{
		get;
		set;
	}

	/// <summary>
	/// Provided for compatibility with destroying
	/// sprite-based controls.
	/// </summary>
	void Delete();
}


[System.Serializable]
public class PrefabListItem
{
	public GameObject item;
	public string itemText;
}



/// <remarks>
/// Acts as a container of list items (basically buttons)
/// and manages them in such a manner as to allow the
/// user to scroll through the list.
/// NOTE: For proper appearance, list items should each 
/// be of uniform width when the list is vertical, and of 
/// uniform height when the list is horizontal. Besides
/// that, list items can vary in size.
/// </remarks>
[AddComponentMenu("EZ GUI/Controls/Scroll List")]
public class UIScrollList : MonoBehaviour, IUIObject
{
	/// <remarks>
	/// The possible orientations of a scroll list.
	/// * HORIZONTAL will cause the list items to be
	/// side-by-side, and the list will scroll right 
	/// and left.
	/// * VERTICAL will cause the list items to be
	/// displayed vertically and the list will scroll
	/// up and down.
	/// </remarks>
	public enum ORIENTATION
	{
		HORIZONTAL,
		VERTICAL
	};


	/// <summary>
	/// When true, scrolling will operate like a
	/// scrollable list on an iPhone - when a
	/// pointing device (normally a finger) is
	/// dragged across it, it scrolls and coasts
	/// to an eventual stop if not stopped 
	/// manually.
	/// </summary>
	public bool touchScroll = true;

	/// <summary>
	/// An optional slider control that will control
	/// the scroll position of the list.
	/// </summary>
	public UISlider slider;

	/// <summary>
	/// The orientation of the list - horizontal or
	/// vertical.
	/// </summary>
	public ORIENTATION orientation;

	/// <summary>
	/// The extents of the viewable area, in local
	/// space units.  The contents of the list will
	/// be clipped to this area. Ex: If an area of
	/// 10x10 is specified, the list contents will
	/// be clipped 5 units above the center of the
	/// scroll list's GameObject, 5 units below,
	/// and 5 units on either side.
	/// </summary>
	public Vector2 viewableArea;
	
	// Used to clip items in the list:
	protected Rect3D clientClippingRect;

	/// <summary>
	/// Empty space to put between each list item.
	/// </summary>
	public float itemSpacing;

	/// <summary>
	/// When true, items added or inserted to this list will
	/// be activated recursively.  This is STRONGLY recommended
	/// when using managed sprites/controls.
	/// </summary>
	public bool activateWhenAdding = true;

	/// <summary>
	/// When true, items will be clipped each frame in which
	/// it is detected that the list has moved, rotated, or
	/// scaled.  Use this if you are having clipping errors
	/// after scaling, rotating, etc, a list as part of a
	/// panel transition, etc.  Leave set to false otherwise
	/// to improve performance.
	/// </summary>
	public bool clipWhenMoving = false;

	/// <summary>
	/// Distance the pointer must travel beyond
	/// which when it is released, the item
	/// under the pointer will not be considered
	/// to have been selected.  This allows
	/// touch scrolling without inadvertently
	/// selecting the item under the pointer.
	/// </summary>
	public float dragThreshold = float.NaN;

	/// <summary>
	/// Optional array of items in the scene that 
	/// are to be added to the list on startup.
	/// These will be added before the items in
	/// the prefabItems array.
	/// </summary>
	public GameObject[] sceneItems = new GameObject[0];

	/// <summary>
	/// Optional array of item prefabs that are
	/// to be added to the list on startup.
	/// These will be added after the items in
	/// the sceneItems array.
	/// </summary>
	public PrefabListItem[] prefabItems = new PrefabListItem[0];

	/// <summary>
	/// Reference to the script component with the method
	/// you wish to invoke when an item is selected.
	/// </summary>
	public MonoBehaviour scriptWithMethodToInvoke;
	
	/// <summary>
	/// A string containing the name of the method to be invoked
	/// when an item is selected.
	/// </summary>
	public string methodToInvokeOnSelect;

	/// <summary>
	/// (Optional) Manager to which instantiated list items should be added
	/// once created. If none is set, sprites must either be unmanaged,
	/// or must already have been added to a manager.
	/// </summary>
	public SpriteManager manager;


	// The extents of the content held in the list.
	// Similar to viewableArea, except this is the
	// partially-hidden area that is taken up by all
	// the items in the list.
	// The value indicates the extent in the direction
	// of the list's orientation.
	protected float contentExtents;

	// Reference to the currently selected item.
	protected IUIListObject selectedItem;

	// The scroll position of the list.
	// (0 == beginning, 1 == end)
	protected float scrollPos;

	// The GameObject that will ease the process
	// of moving our list items about.
	protected GameObject mover;

	// Items in our list.
	protected List<IUIListObject> items = new List<IUIListObject>();

	// Items which are presently visible.
	protected List<IUIListObject> visibleItems = new List<IUIListObject>();
	// Temporary version of the above:
	protected List<IUIListObject> tempVisItems = new List<IUIListObject>();

	protected bool m_controlIsEnabled = true;
	protected IUIContainer container;

	protected EZInputDelegate inputDelegate;
	protected EZValueChangedDelegate changeDelegate;

	// Cached transform info of the list, so if it changes,
	// we can update our client clipping rect.
	protected Vector3 cachedPos;
	protected Quaternion cachedRot;
	protected Vector3 cachedScale;

	// Tells us if Start() has already run
	protected bool m_started = false;


	// Scrolling vars:
	protected bool isScrolling;					// Is true when the list is scrolling.
	protected bool noTouch;						// Indicates whether there's currently an active touch/click/drag
	protected const float reboundSpeed = 1f;	// Rate at which the list will rebound from the edges.
	protected const float overscrollAllowance = 0.5f; // The percentage of the viewable area that can be "over-scrolled" (scrolled beyond the end/beginning of the list)
	protected const float scrollDecelCoef = 0.4f;	// The scroll deceleration coefficient. Determines how fast a flick-scroll slows down on its own.
	protected const float lowPassKernelWidthInSeconds = 0.03f; // Inertial sampling width, in seconds
	protected const float scrollDeltaUpdateInterval = 0.0166f;// The standard framerate update interval, in seconds
	protected const float lowPassFilterFactor = scrollDeltaUpdateInterval / lowPassKernelWidthInSeconds;
	float scrollInertia;						// Holds onto some of the energy from previous scrolling action for a while
	protected const float backgroundColliderOffset = 0.01f; // How far behind the list items the background collider should be placed.
	protected float scrollMax;					// Absolute max we can scroll beyond the edge of the list.
	float scrollDelta;


	void Awake()
	{
		// Create our mover object:
		mover = new GameObject();
		mover.name = "Mover";
		mover.transform.parent = transform;
		mover.transform.localPosition = Vector3.zero;
		mover.transform.localRotation = Quaternion.identity;
		mover.transform.localScale = Vector3.one;
	}

	void Start()
	{
		if (m_started)
			return;
		m_started = true;

		cachedPos = transform.position;
		cachedRot = transform.rotation;
		cachedScale = transform.lossyScale;
		CalcClippingRect();

		if (slider != null)
			slider.SetValueChangedDelegate( SliderMoved );

		// Create a background box collider to catch
		// input events between list items:
		if(collider == null && touchScroll)
		{
			BoxCollider bc = (BoxCollider) gameObject.AddComponent(typeof(BoxCollider));
			bc.size = new Vector3(viewableArea.x, viewableArea.y, 0.001f);
			bc.center = Vector3.forward * backgroundColliderOffset; // Set the collider behind where the list items will be.
			bc.isTrigger = true;
		}

		for(int i=0; i<sceneItems.Length; ++i)
			if(sceneItems[i] != null)
				AddItem(sceneItems[i]);

		for (int i = 0; i < prefabItems.Length; ++i)
			if(prefabItems[i] != null)
			{
				// If this one is null, use the first prefab:
				if (prefabItems[i].item == null)
				{
					if(prefabItems[0].item != null)
						CreateItem(prefabItems[0].item, (prefabItems[i].itemText == "") ? (null) : (prefabItems[i].itemText));
				}
				else
				{
					CreateItem(prefabItems[i].item, (prefabItems[i].itemText == "") ? (null) : (prefabItems[i].itemText));
				}
			}

		// Use the default threshold if ours
		// has not been set to anything:
		if (float.IsNaN(dragThreshold))
			dragThreshold = UIManager.instance.dragThreshold;
	}

	// Updates the clipping rect if, for example, the viewable area changes.
	protected void CalcClippingRect()
	{
		clientClippingRect.FromPoints(new Vector3(-viewableArea.x * 0.5f, viewableArea.y * 0.5f),
									  new Vector3(viewableArea.x * 0.5f, viewableArea.y * 0.5f),
									  new Vector3(-viewableArea.x * 0.5f, -viewableArea.y * 0.5f));
		clientClippingRect.MultFast(transform.localToWorldMatrix);

		for(int i=0; i<items.Count; ++i)
		{
			if(items[i].TextObj != null)
				items[i].TextObj.ClippingRect = clientClippingRect;
		}
	}

	// Is called when the optional slider control
	// is moved.
	public void SliderMoved(IUIObject slider)
	{
		ScrollListTo(((UISlider)slider).Value);
	}

	/// <summary>
	/// Scrolls the list directly to the position
	/// indicated (0-1).
	/// </summary>
	/// <param name="pos">Position of the list - 0-1 (0 == beginning, 1 == end)</param>
	public void ScrollListTo(float pos)
	{
		float amtOfPlay;

		if (float.IsNaN(pos))
			return; // Ignore since the viewing area exactly matches our content, no need to scroll anyway

		if (orientation == ORIENTATION.VERTICAL)
		{
			amtOfPlay = ((contentExtents + itemSpacing) - viewableArea.y);
			mover.transform.localPosition = (Vector3.up * Mathf.Clamp(amtOfPlay, 0, amtOfPlay) * pos);
		}
		else
		{
			amtOfPlay = ((contentExtents + itemSpacing) - viewableArea.x);
			mover.transform.localPosition = (Vector3.right * Mathf.Clamp(amtOfPlay, 0, amtOfPlay) * -pos);
		}
		scrollPos = pos;
		ClipItems();

		if (slider != null)
			slider.Value = scrollPos;
	}

	/// <summary>
	/// Sets or retrieves the scroll position of the list.
	/// The position is given as a value between 0 and 1.
	/// 0 indicates the beginning of the list, 1 the end.
	/// </summary>
	public float ScrollPosition
	{
		get { return scrollPos; }
		set { ScrollListTo(value); }
	}

	/// <summary>
	/// Inserts a list item at the specified position in the list.
	/// </summary>
	/// <param name="item">Reference to the item to be inserted into the list.</param>
	/// <param name="position">0-based index of the position in the list where the item will be placed.</param>
	public void InsertItem(IUIListObject item, int position)
	{
		InsertItem(item, position, null);
	}

	/// <summary>
	/// Inserts a list item at the specified position in the list.
	/// </summary>
	/// <param name="item">Reference to the item to be inserted into the list.</param>
	/// <param name="position">0-based index of the position in the list where the item will be placed.</param>
	/// <param name="text">Text to display in the item (requires that the item has a TextMesh associated with it, preferably in a child GameObject).</param>
	public void InsertItem(IUIListObject item, int position, string text)
	{
		IUIListObject lastItem;
		float contentDelta;

		// Make sure Start() has already run:
		if (!m_started)
			Start();

		// See if the item needs to be enabled:
		if(activateWhenAdding)
			if (!((Component)item).gameObject.active)
				((Component)item).gameObject.SetActiveRecursively(true);

		// Put the item in the correct layer:
		item.gameObject.layer = gameObject.layer;

		// Add the item to our container:
		if (container != null)
			container.AddChild(item.gameObject);


		//-------------------------------------
		// Position our item:
		//-------------------------------------
		item.transform.parent = mover.transform;
#if AUTO_ORIENT_ITEMS
		item.transform.localRotation = Quaternion.identity;
#endif
#if AUTO_SCALE_ITEMS
		item.transform.localScale = Vector3.one;
#endif
		// Go ahead and get the item in the mover's plane
		// on the local Z-axis.  This must be done here
		// before anything that follows because if we are
		// using a perspective camera and these are newly
		// created items, their Start() will be called for
		// the first time when FindOuterEdges() is called
		// either by us, or by the Text property, and if
		// the item isn't already positioned relative to
		// the camera, then its size will be calculated
		// wrong.
		item.transform.localPosition = Vector3.zero;


		if(text != null)
			item.Text = text;

		item.SetList(this);

		// Compute the edges of the item:
		item.FindOuterEdges();
		item.UpdateCollider();
		

		// Clamp our position:
		position = Mathf.Clamp(position, 0, items.Count);

		// Hide the item by default:
		item.Hide(true);
		if (!item.Managed)
			item.gameObject.SetActiveRecursively(false);

		// See if we can just add it to the end:
		if(position == items.Count)
		{
			if (orientation == ORIENTATION.HORIZONTAL)
			{

				if (items.Count > 0)
				{
					lastItem = items[items.Count - 1];
					item.transform.localPosition = new Vector3(lastItem.transform.localPosition.x + lastItem.BottomRightEdge.x + itemSpacing - item.TopLeftEdge.x, 0);
				}
				else
				{
					item.transform.localPosition = new Vector3((viewableArea.x * -0.5f) - item.TopLeftEdge.x + itemSpacing, 0);
				}

				contentDelta = item.BottomRightEdge.x - item.TopLeftEdge.x + itemSpacing;
			}
			else
			{
				if (items.Count > 0)
				{
					lastItem = items[items.Count - 1];
					item.transform.localPosition = new Vector3(0, lastItem.transform.localPosition.y + lastItem.BottomRightEdge.y - itemSpacing - item.TopLeftEdge.y);
				}
				else
				{
					item.transform.localPosition = new Vector3(0, (viewableArea.y * 0.5f) - item.TopLeftEdge.y - itemSpacing);
				}
				contentDelta = item.TopLeftEdge.y - item.BottomRightEdge.y + itemSpacing;
			}

			item.Index = items.Count;
			items.Add(item);

			UpdateContentExtents(contentDelta);
			ClipItems();
		}
		else
		{
			// Else, insert the item in the midst of our list:
			items.Insert(position, item);

			PositionItems();
		}
	}

	/// <summary>
	/// Adds an item to the end of the list.
	/// NOTE: For proper appearance, list items should each 
	/// be of uniform width when the list is vertical, and of 
	/// uniform height when the list is horizontal. Besides
	/// that, list items can vary in size.
	/// </summary>
	/// <param name="item">Reference to a GameObject containing a list item to be added.</param>
	public void AddItem(GameObject itemGO)
	{
		IUIListObject item = (IUIListObject)itemGO.GetComponent(typeof(IUIListObject));
		if (item == null)
		{
			Debug.LogWarning("GameObject \"" + itemGO.name + "\" does not contain any list item component suitable to be added to scroll list \"" + name + "\".");
			return;
		}

		AddItem(item, null);
	}

	/// <summary>
	/// Adds an item to the end of the list.
	/// NOTE: For proper appearance, list items should each 
	/// be of uniform width when the list is vertical, and of 
	/// uniform height when the list is horizontal. Besides
	/// that, list items can vary in size.
	/// </summary>
	/// <param name="item">Reference to the list item to be added.</param>
	public void AddItem(IUIListObject item)
	{
		AddItem(item, null);
	}

	/// <summary>
	/// Adds an item to the end of the list.
	/// NOTE: For proper appearance, list items should each 
	/// be of uniform width when the list is vertical, and of 
	/// uniform height when the list is horizontal. Besides
	/// that, list items can vary in size.
	/// </summary>
	/// <param name="item">Reference to the list item to be added.</param>
	/// <param name="text">Text to display in the item (requires that the item has a TextMesh associated with it, preferably in a child GameObject).</param>
	public void AddItem(IUIListObject item, string text)
	{
		InsertItem(item, items.Count, text);
	}

	/// <summary>
	/// Instantiates a new list item based on the
	/// prefab to clone, adds it to the end of the 
	/// list and returns a reference to it.
	/// NOTE: The prefab/GameObject is required to
	/// have a component that implements IUIListObject.
	/// NOTE: For proper appearance, list items should each 
	/// be of uniform width when the list is vertical, and of 
	/// uniform height when the list is horizontal. Besides
	/// that, list items can vary in size.
	/// </summary>
	/// <param name="prefab">GameObject/Prefab which is to be instantiated and added to the end of the list.</param>
	/// <returns>Reference to the newly instantiated list item. Null if an error occurred, such as if the prefab does not contain a required component type.</returns>
	public IUIListObject CreateItem(GameObject prefab)
	{
		return CreateItem(prefab, items.Count, null);
	}

	/// <summary>
	/// Instantiates a new list item based on the
	/// prefab to clone, adds it to the end of the 
	/// list and returns a reference to it.
	/// NOTE: The prefab/GameObject is required to
	/// have a component that implements IUIListObject.
	/// NOTE: For proper appearance, list items should each 
	/// be of uniform width when the list is vertical, and of 
	/// uniform height when the list is horizontal. Besides
	/// that, list items can vary in size.
	/// </summary>
	/// <param name="prefab">GameObject/Prefab which is to be instantiated and added to the end of the list.</param>
	/// <param name="text">Text to display in the item (requires that the item has a TextMesh associated with it, preferably in a child GameObject).</param>
	/// <returns>Reference to the newly instantiated list item. Null if an error occurred, such as if the prefab does not contain a required component type.</returns>
	public IUIListObject CreateItem(GameObject prefab, string text)
	{
		return CreateItem(prefab, items.Count, text);
	}

	/// <summary>
	/// Instantiates a new list item based on the
	/// prefab to clone, adds it to the list at the position 
	/// specified by "position" and returns a reference to it.
	/// NOTE: The prefab/GameObject is required to
	/// have a component that implements IUIListObject.
	/// NOTE: For proper appearance, list items should each 
	/// be of uniform width when the list is vertical, and of 
	/// uniform height when the list is horizontal. Besides
	/// that, list items can vary in size.
	/// </summary>
	/// <param name="prefab">GameObject/Prefab which is to be instantiated and added to the end of the list.</param>
	/// <param name="position">0-based index where the new item will be placed in the list.</param>
	/// <returns>Reference to the newly instantiated list item. Null if an error occurred, such as if the prefab does not contain a required component type.</returns>
	public IUIListObject CreateItem(GameObject prefab, int position)
	{
		return CreateItem(prefab, position, null);
	}

	/// <summary>
	/// Instantiates a new list item based on the
	/// prefab to clone, adds it to the list at the position 
	/// specified by "position" and returns a reference to it.
	/// NOTE: The prefab/GameObject is required to
	/// have a component that implements IUIListObject.
	/// NOTE: For proper appearance, list items should each 
	/// be of uniform width when the list is vertical, and of 
	/// uniform height when the list is horizontal. Besides
	/// that, list items can vary in size.
	/// </summary>
	/// <param name="prefab">GameObject/Prefab which is to be instantiated and added to the end of the list.</param>
	/// <param name="position">0-based index where the new item will be placed in the list.</param>
	/// <param name="text">Text to display in the item (requires that the item has a TextMesh associated with it, preferably in a child GameObject).</param>
	/// <returns>Reference to the newly instantiated list item. Null if an error occurred, such as if the prefab does not contain a required component type.</returns>
	public IUIListObject CreateItem(GameObject prefab, int position, string text)
	{
		IUIListObject newItem;
		GameObject go;

		newItem = (IUIListObject)prefab.GetComponent(typeof(IUIListObject));

		if (null == newItem)
			return null;

		if (manager != null)
		{	// Managed:
			if(newItem.IsContainer())
			{
				// This object contains other sprite-based objects:
				go = (GameObject)Instantiate(prefab);

				Component[] sprites = go.GetComponentsInChildren(typeof(SpriteRoot));
				for (int i = 0; i < sprites.Length; ++i)
					manager.AddSprite((SpriteRoot)sprites[i]);
			}
			else
			{
				SpriteRoot s = manager.CreateSprite(prefab);

				if (s == null)
					return null;

				go = s.gameObject;
			}
		}
		else // Unmanaged:
		{
			go = (GameObject)Instantiate(prefab);
		}

		newItem = (IUIListObject)go.GetComponent(typeof(IUIListObject));

		if (newItem == null)
			return null;

		//newItem.IsClone = true;

		// Make sure it has a collider:
/*
		if (go.collider == null)
		{
			go.AddComponent(typeof(BoxCollider));
		}
*/

		InsertItem(newItem, position, text);
		return newItem;
	}

	// Updates the extents of the content area by
	// the specified amount.
	// change: The amount the content extents have changed (+/-)
	protected void UpdateContentExtents(float change)
	{
		float oldAmtOfPlay;
		float newAmtOfPlay;

		contentExtents += change;

		if (orientation == ORIENTATION.HORIZONTAL)
		{
			// Adjust the scroll position:
			oldAmtOfPlay = ((contentExtents - change + itemSpacing) - viewableArea.x);
			newAmtOfPlay = ((contentExtents + itemSpacing) - viewableArea.x);
			scrollMax = (viewableArea.x / ((contentExtents + itemSpacing) - viewableArea.x)) * overscrollAllowance;
		}
		else
		{
			// Adjust the scroll position:
			oldAmtOfPlay = ((contentExtents - change + itemSpacing) - viewableArea.y);
			newAmtOfPlay = ((contentExtents + itemSpacing) - viewableArea.y);
			scrollMax = (viewableArea.y / ((contentExtents + itemSpacing) - viewableArea.y)) * overscrollAllowance;
		}

		ScrollListTo(Mathf.Clamp01((oldAmtOfPlay * scrollPos) / newAmtOfPlay));
	}

	// Positions list items according to our
	// current scroll position.
	protected void PositionItems()
	{
		if (orientation == ORIENTATION.HORIZONTAL)
			PositionHorizontally();
		else
			PositionVertically();

		UpdateContentExtents(0);	// Just so other stuff gets updated
		ClipItems();
	}

	// Positions list items horizontally.
	protected void PositionHorizontally()
	{
		// Will hold the leading edge of the list throughout
		float edge = (viewableArea.x * -0.5f) + itemSpacing;

		contentExtents = 0;

		for(int i=0; i<items.Count; ++i)
		{
			items[i].transform.localPosition = new Vector3(edge - items[i].TopLeftEdge.x, 0);
			contentExtents += items[i].BottomRightEdge.x - items[i].TopLeftEdge.x + itemSpacing;
			edge += items[i].BottomRightEdge.x - items[i].TopLeftEdge.x + itemSpacing;

			// Assign indices:
			items[i].Index = i;
		}
	}

	// Positions list items vertically.
	protected void PositionVertically()
	{
		// Will hold the leading edge of the list throughout
		float edge = (viewableArea.y * 0.5f) - itemSpacing;

		contentExtents = 0;

		for (int i = 0; i < items.Count; ++i)
		{
			items[i].transform.localPosition = new Vector3(0, edge - items[i].TopLeftEdge.y);
			contentExtents += items[i].TopLeftEdge.y - items[i].BottomRightEdge.y + itemSpacing;
			edge -= items[i].TopLeftEdge.y - items[i].BottomRightEdge.y + itemSpacing;

			// Assign indices:
			items[i].Index = i;
		}
	}


	// Clips list items to the viewable area.
	protected void ClipItems()
	{
		if (mover == null || items.Count < 1)
			return;

		IUIListObject firstItem = null;
		IUIListObject lastItem = null;

		
		if (orientation == ORIENTATION.HORIZONTAL)
		{
			float moverOffset = mover.transform.localPosition.x;
			float itemOffset;
			// Calculate the visible edges insider the mover's local space:
			float leftVisibleEdge = (viewableArea.x * -0.5f) - moverOffset;
			float rightVisibleEdge = (viewableArea.x * 0.5f) - moverOffset;

			// Find the first visible item:
			// Start looking at our approximate scroll position:
			int index = (int)(((float)(items.Count - 1)) * Mathf.Clamp01(scrollPos));
			
			// See if the first item we checked is to the right
			// of our left-most viewable edge:
			itemOffset = items[index].transform.localPosition.x;
			if (items[index].BottomRightEdge.x + itemOffset >= leftVisibleEdge)
			{
				// Search backward until we find one that is not:
				for (index-=1; index > -1 ; --index)
				{
					itemOffset = items[index].transform.localPosition.x;
					if (items[index].BottomRightEdge.x + itemOffset < leftVisibleEdge)
						break;
				}

				// The previous item is the one we're looking for:
				firstItem = items[index + 1];
			}
			else
			{
				// Search forward until we find the first visible item:
				for (; index < items.Count; ++index)
				{
					itemOffset = items[index].transform.localPosition.x;
					if (items[index].BottomRightEdge.x + itemOffset >= leftVisibleEdge)
					{
						// We've found our first visible item:
						firstItem = items[index];
						break;
					}
				}
			}

			if(firstItem != null)
			{
				// Add the first visible item to our list and clip it:
				tempVisItems.Add(firstItem);
				if (!firstItem.gameObject.active)
					firstItem.gameObject.SetActiveRecursively(true);
				firstItem.Hide(false);
				firstItem.ClippingRect = clientClippingRect;

				// See if this is the only visible item:
				itemOffset = firstItem.transform.localPosition.x;
				if (firstItem.BottomRightEdge.x + itemOffset < rightVisibleEdge)
				{
					// Now search forward until we find an item that is outside
					// the viewable area:
					for (index = firstItem.Index + 1; index < items.Count; ++index)
					{
						itemOffset = items[index].transform.localPosition.x;
						if (items[index].BottomRightEdge.x + itemOffset >= rightVisibleEdge)
						{
							// We've found the last visible item
							if (!items[index].gameObject.active)
								items[index].gameObject.SetActiveRecursively(true);
							items[index].Hide(false);
							items[index].ClippingRect = clientClippingRect;
							tempVisItems.Add(items[index]);
							break;
						}
						else
						{
							if (!items[index].gameObject.active)
								items[index].gameObject.SetActiveRecursively(true);
							items[index].Hide(false);
							items[index].Clipped = false;
							tempVisItems.Add(items[index]);
						}
					}
				}
			}
		}
		else
		{
			float moverOffset = mover.transform.localPosition.y;
			float itemOffset;
			// Calculate the visible edges insider the mover's local space:
			float topVisibleEdge = (viewableArea.y * 0.5f) - moverOffset;
			float bottomVisibleEdge = (viewableArea.y * -0.5f) - moverOffset;

			// Find the first visible item:
			// Start looking at our approximate scroll position:
			int index = (int)(((float)(items.Count-1)) * Mathf.Clamp01(scrollPos));

			// See if the first item we checked is below
			// our top-most viewable edge:
			itemOffset = items[index].transform.localPosition.y;
			if (items[index].BottomRightEdge.y + itemOffset <= topVisibleEdge)
			{
				// Search backward until we find one that is not:
				for (index -= 1; index > -1; --index)
				{
					itemOffset = items[index].transform.localPosition.y;
					if (items[index].BottomRightEdge.y + itemOffset > topVisibleEdge)
						break;
				}

				// The previous item is the one we're looking for:
				firstItem = items[index + 1];
			}
			else
			{
				// Search forward until we find the first visible item:
				for (; index < items.Count; ++index)
				{
					itemOffset = items[index].transform.localPosition.y;
					if (items[index].BottomRightEdge.y + itemOffset <= topVisibleEdge)
					{
						// We've found our first visible item:
						firstItem = items[index];
						break;
					}
				}
			}

			if(firstItem != null)
			{
				// Add the first visible item to our list and clip it:
				tempVisItems.Add(firstItem);
				if (!firstItem.gameObject.active)
					firstItem.gameObject.SetActiveRecursively(true);
				firstItem.Hide(false);
				firstItem.ClippingRect = clientClippingRect;

				// See if this is the only visible item:
				itemOffset = firstItem.transform.localPosition.y;
				if (firstItem.BottomRightEdge.y + itemOffset > bottomVisibleEdge)
				{
					// Now search forward until we find an item that is outside
					// the viewable area:
					for (index = firstItem.Index + 1; index < items.Count; ++index)
					{
						itemOffset = items[index].transform.localPosition.y;
						if (items[index].BottomRightEdge.y + itemOffset <= bottomVisibleEdge)
						{
							// We've found the last visible item
							if (!items[index].gameObject.active)
								items[index].gameObject.SetActiveRecursively(true);
							items[index].Hide(false);
							items[index].ClippingRect = clientClippingRect;
							tempVisItems.Add(items[index]);
							break;
						}
						else
						{
							if (!items[index].gameObject.active)
								items[index].gameObject.SetActiveRecursively(true);
							items[index].Hide(false);
							items[index].Clipped = false;
							tempVisItems.Add(items[index]);
						}
					}
				}
			}
		}

		if(firstItem == null)
			return;

		lastItem = tempVisItems[tempVisItems.Count - 1];

		if(visibleItems.Count > 0)
		{
			// Hide any items that are no longer visible:

			// First see if our previous visible list lies entirely outside
			// our new list:
			if (visibleItems[0].Index > lastItem.Index ||
			   visibleItems[visibleItems.Count - 1].Index < firstItem.Index)
			{
				for (int i = 0; i < visibleItems.Count; ++i)
				{
					visibleItems[i].Hide(true);
					if (!visibleItems[i].Managed)
						visibleItems[i].gameObject.SetActiveRecursively(false);
				}
			}
			else
			{
				// Process items until we reach our first currently visible item:
				for (int i = 0; i < visibleItems.Count; ++i)
				{
					if (visibleItems[i].Index < firstItem.Index)
					{
						visibleItems[i].Hide(true);
						if (!visibleItems[i].Managed)
							visibleItems[i].gameObject.SetActiveRecursively(false);
					}
					else
						break;
				}

				// Process items from the end backward until we reach our
				// last currently visible item:
				for (int i = visibleItems.Count - 1; i > -1; --i)
				{
					if (visibleItems[i].Index > lastItem.Index)
					{
						visibleItems[i].Hide(true);
						if (!visibleItems[i].Managed)
							visibleItems[i].gameObject.SetActiveRecursively(false);
					}
					else
						break;
				}
			}
		}

		// Swap our lists:
		List<IUIListObject> swapList = visibleItems;
		visibleItems = tempVisItems;
		tempVisItems = swapList;
		tempVisItems.Clear();
	}

	// Called by a list item when it is selected:
	public void DidSelect(IUIListObject item)
	{
		if (selectedItem != null)
			selectedItem.selected = false;

		selectedItem = item;
		item.selected = true;

/*
		if (scriptWithMethodToInvoke != null)
			scriptWithMethodToInvoke.Invoke(methodToInvokeOnSelect, 0);
		if (changeDelegate != null)
			changeDelegate(this);
*/

		DidClick(item);
	}

	// Called when a list item when it is clicked
	public void DidClick(IUIListObject item)
	{
		if (scriptWithMethodToInvoke != null)
			scriptWithMethodToInvoke.Invoke(methodToInvokeOnSelect, 0);
		if (changeDelegate != null)
			changeDelegate(this);
	}

	// Is called by a list item when a drag is detected.
	// For our purposes, it is only called when the drag
	// extends beyond the drag threshold.
	public void ListDragged(POINTER_INFO ptr)
	{
		if (!touchScroll)
			return;	// Ignore

		// Calculate the pointer's motion relative to our control:
		Vector3 inputPoint1;
		Vector3 inputPoint2;
		Vector3 ptrVector;
		float dist;
		Plane ctrlPlane = default(Plane);

		// Early out:
		if(ptr.inputDelta.sqrMagnitude == 0)
		{
			scrollDelta = 0;
			return;
		}

		ctrlPlane.SetNormalAndPosition(mover.transform.forward * -1f, mover.transform.position);

		ctrlPlane.Raycast(ptr.ray, out dist);
		inputPoint1 = ptr.ray.origin + ptr.ray.direction * dist;

		ctrlPlane.Raycast(ptr.prevRay, out dist);
		inputPoint2 = ptr.prevRay.origin + ptr.prevRay.direction * dist;

		// Get the input points into the local space of our list:
		inputPoint1 = transform.InverseTransformPoint(inputPoint1);
		inputPoint2 = transform.InverseTransformPoint(inputPoint2);

		ptrVector = inputPoint1 - inputPoint2;

		// Find what percentage of our content 
		// extent this value represents:
		if (orientation == ORIENTATION.HORIZONTAL)
		{
			scrollDelta = (-ptrVector.x) / ((contentExtents + itemSpacing) - viewableArea.x);
			scrollDelta *= transform.localScale.x;
		}
		else
		{
			scrollDelta = ptrVector.y / ((contentExtents + itemSpacing) - viewableArea.y);
			scrollDelta *= transform.localScale.y;
		}


/*
		ptr.devicePos.z = mover.transform.position.z;
		ptrVector = cam.ScreenToWorldPoint(ptr.devicePos) - cam.ScreenToWorldPoint(ptr.devicePos - ptr.inputDelta);

		if(orientation == ORIENTATION.HORIZONTAL)
		{
			localVector = transform.TransformDirection(Vector3.right);
			scrollDelta = -Vector3.Project(ptrVector, localVector).x;
			scrollDelta *= transform.localScale.x;
			// Find what percentage of our content 
			// extent this value represents:
			scrollDelta /= ( (contentExtents+itemSpacing) - viewableArea.x);
		}
		else
		{
			localVector = transform.TransformDirection(Vector3.up);
			scrollDelta = Vector3.Project(ptrVector, localVector).y;
			scrollDelta *= transform.localScale.y;
			// Find what percentage of our content 
			// extent this value represents:
			scrollDelta /= ((contentExtents + itemSpacing) - viewableArea.y);
		}
*/

		float target = scrollPos + scrollDelta;

		if (target > 1f)
		{
			// Scale our delta according to how close we
			// are to reaching our max scroll:
			scrollDelta *= Mathf.Clamp01(1f - (target - 1f) / scrollMax);
		}
		else if (target < 0)
		{
			scrollDelta *= Mathf.Clamp01(1f + (target / scrollMax));
		}

		ScrollListTo(scrollPos + scrollDelta);

		noTouch = false;
		isScrolling = true;
	}

	// Is called by a list item or internally 
	// when the pointing device is released.
	public void PointerReleased()
	{
		noTouch = true;

		if (scrollInertia != 0)
			scrollDelta = scrollInertia;

		scrollInertia = 0;
	}

	public void OnEnable()
	{
		gameObject.SetActiveRecursively(true);
		ClipItems();
	}


	//---------------------------------------------
	// Accessors:
	//---------------------------------------------
	/// <summary>
	/// Accessor that returns a reference to the
	/// currently selected item.  Null is returned
	/// if no item is currently selected.
	/// Can also be used to set the currently
	/// selected item.
	/// </summary>
	public IUIListObject SelectedItem
	{
		get { return selectedItem; }
		set
		{
			if(value == null)
			{
				selectedItem = null;
				return;
			}

			selectedItem = value;
			selectedItem.selected = true;
		}
	}

	/// <summary>
	/// Sets the item at the specified index as the
	/// currently selected item.
	/// </summary>
	/// <param name="index">The zero-based index of the item.</param>
	public void SetSelectedItem(int index)
	{
		if (index < 0 || index >= items.Count)
			return;

		IUIListObject item = items[index];

		if (selectedItem != null)
			selectedItem.selected = false;

		selectedItem = item;
		item.selected = true;
	}

	/// <summary>
	/// Returns the number of items currently
	/// in the list.
	/// </summary>
	public int Count
	{
		get { return items.Count; }
	}

	/// <summary>
	/// Returns a reference to the specified list item.
	/// </summary>
	/// <param name="index">Index of the item to retrieve.</param>
	/// <returns>Reference to the desired list item, null if index is out of range.</returns>
	public IUIListObject GetItem(int index)
	{
		if (index < 0 || index >= items.Count)
			return null;
		return items[index];
	}

	/// <summary>
	/// Removes the item at the specified index.
	/// Remaining items are repositioned to fill
	/// the gap.
	/// The removed item is destroyed if 'destroy'
	/// is true. Otherwise, it is deactivated.
	/// </summary>
	/// <param name="index">Index of the item to remove.</param>
	public void RemoveItem(int index, bool destroy)
	{
		if (index < 0 || index >= items.Count)
			return;

		// Remove the item from our container:
		if (container != null)
			container.RemoveChild(items[index].gameObject);

		// Unselect it, if necessary:
		if(selectedItem == items[index])
		{
			selectedItem = null;
			items[index].selected = false;
		}

		// Remove the item from our visible list, if it's there:
		visibleItems.Remove(items[index]);

		if (destroy)
		{
			items[index].Delete();
			Destroy(items[index].gameObject);
		}
		else
		{
			// Move to the root of the hierarchy:
			items[index].transform.parent = null;
			// Deactivate:
			items[index].gameObject.SetActiveRecursively(false);
		}

		items.RemoveAt(index);

		// Reposition our items:
		PositionItems();
	}

	/// <summary>
	/// Removes the specified item.
	/// Remaining items are repositioned to fill
	/// the gap.
	/// The removed item is destroyed if 'destroy'
	/// is true. Otherwise, it is deactivated.
	/// </summary>
	/// <param name="index">Reference to the item to be removed.</param>
	public void RemoveItem(IUIListObject item, bool destroy)
	{
		for(int i=0; i<items.Count; ++i)
		{
			if(items[i] == item)
			{
				RemoveItem(i, destroy);
				return;
			}
		}
	}

	/// <summary>
	/// Empties the contents of the list entirely.
	/// Destroys the items if instructed, otherwise
	/// it just deactivates them.
	/// </summary>
	/// <param name="destroy">When true, the list items are actually destroyed. Otherwise, they are deactivated.</param>
	public void ClearList(bool destroy)
	{
		RemoveItemsFromContainer();

		for(int i=0; i<items.Count; ++i)
		{
			// Move them out of the mover object
			// and into the root of the scene
			// hierarchy:
			items[i].transform.parent = null;

			if (destroy)
				Destroy(items[i].gameObject);
			else
				items[i].gameObject.SetActiveRecursively(false);
		}

		visibleItems.Clear();
		items.Clear();
		PositionItems();
	}


	//---------------------------------------------
	// Misc:
	//---------------------------------------------

	public void OnInput(POINTER_INFO ptr)
	{
		if (!m_controlIsEnabled)
		{
			if(Container != null)
			{
				ptr.callerIsControl = true;
				Container.OnInput(ptr);
			}

			return;
		}


		// Do our own tap checking with the list's
		// own threshold:
		if (Vector3.SqrMagnitude(ptr.origPos - ptr.devicePos) > (dragThreshold * dragThreshold))
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
					ListDragged(ptr);
				break;
			case POINTER_INFO.INPUT_EVENT.DRAG:
				if (!ptr.isTap)
					ListDragged(ptr);
				break;
			case POINTER_INFO.INPUT_EVENT.RELEASE:
			case POINTER_INFO.INPUT_EVENT.RELEASE_OFF:
			case POINTER_INFO.INPUT_EVENT.TAP:
				PointerReleased();
				break;
		}

		if (Container != null)
		{
			ptr.callerIsControl = true;
			Container.OnInput(ptr);
		}
	}


	void Update()
	{
		if (cachedPos != transform.position ||
			cachedRot != transform.rotation ||
			cachedScale != transform.lossyScale)
		{
			cachedPos = transform.position;
			cachedRot = transform.rotation;
			cachedScale = transform.lossyScale;
			CalcClippingRect();
			
			if(clipWhenMoving)
			{
				ClipItems();
			}
		}

		if(isScrolling && noTouch)
		{
			scrollDelta -= (scrollDelta * scrollDecelCoef) * (Time.deltaTime / 0.166f);

			// See if we need to rebound from the edge:
			if (scrollPos < 0)
			{
				scrollPos -= scrollPos * reboundSpeed * (Time.deltaTime / 0.166f);
				// Compute resistance:
				scrollDelta *= Mathf.Clamp01(1f + (scrollPos / scrollMax));
			}
			else if (scrollPos > 1f)
			{
				scrollPos -= (scrollPos - 1f) * reboundSpeed * (Time.deltaTime / 0.166f);
				// Compute resistance:
				scrollDelta *= Mathf.Clamp01(1f - (scrollPos - 1f) / scrollMax);
			}

			if (Mathf.Abs(scrollDelta) < 0.0001f)
			{
				scrollDelta = 0;
				if(scrollPos > -0.0001f && scrollPos < 0.0001f)
					scrollPos = Mathf.Clamp01(scrollPos);
			}

			ScrollListTo(scrollPos + scrollDelta);


			if (slider != null)
				slider.Value = scrollPos;

			if ((scrollPos >= 0 && scrollPos <= 1.001f && scrollDelta == 0))
				isScrolling = false;
		}
		else
		{
			scrollInertia = Mathf.Lerp(scrollInertia, scrollDelta, lowPassFilterFactor);
		}
	}


	// Adds all of the control's items to its container.
	protected void AddItemsToContainer()
	{
		if (container == null)
			return;

		for(int i=0; i<items.Count; ++i)
		{
			container.AddChild(items[i].gameObject);
		}
	}

	// Removes all of the control's items from its container/
	protected void RemoveItemsFromContainer()
	{
		if (container == null)
			return;

		for (int i = 0; i<items.Count; ++i)
		{
			container.RemoveChild(items[i].gameObject);
		}
	}


	public bool controlIsEnabled
	{
		get { return m_controlIsEnabled; }
		set
		{
			m_controlIsEnabled = value;

			for(int i=0; i<items.Count; ++i)
			{
				items[i].controlIsEnabled = value;
			}
		}
	}

	public virtual IUIContainer Container
	{
		get { return container; }
		set
		{
			if (value != container)
			{
				if (container != null)
				{
					RemoveItemsFromContainer();
				}

				container = value;
				AddItemsToContainer();
			}
			else
				container = value;
		}
	}

	public bool RequestContainership(IUIContainer cont)
	{
		Transform t = transform.parent;
		Transform c = ((Component)cont).transform;

		while (t != null)
		{
			if (t == c)
			{
				container = cont;
				return true;
			}
			else if (t.gameObject.GetComponent("IUIContainer") != null)
				return false;

			t = t.parent;
		}

		// Never found *any* containers:
		return false;
	}

	public bool GotFocus() { return false; }
	public void LostFocus() { }
	public string GetInputText(ref KEYBOARD_INFO info) { return null; }
	public string SetInputText(string text, ref int insert) { return null; }

	public EZInputDelegate SetInputDelegate(EZInputDelegate del)
	{
		EZInputDelegate oldDel = inputDelegate;
		inputDelegate = del;
		return oldDel;
	}

	public EZValueChangedDelegate SetValueChangedDelegate(EZValueChangedDelegate del)
	{
		EZValueChangedDelegate oldDel = changeDelegate;
		changeDelegate = del;
		return oldDel;
	}

	void OnDrawGizmosSelected()
	{
		Vector3 ul, ll, lr, ur;

		ul = (transform.position - transform.TransformDirection(Vector3.right * viewableArea.x * 0.5f * transform.lossyScale.x) + transform.TransformDirection(Vector3.up * viewableArea.y * 0.5f * transform.lossyScale.y));
		ll = (transform.position - transform.TransformDirection(Vector3.right * viewableArea.x * 0.5f * transform.lossyScale.x) - transform.TransformDirection(Vector3.up * viewableArea.y * 0.5f * transform.lossyScale.y));
		lr = (transform.position + transform.TransformDirection(Vector3.right * viewableArea.x * 0.5f * transform.lossyScale.x) - transform.TransformDirection(Vector3.up * viewableArea.y * 0.5f * transform.lossyScale.y));
		ur = (transform.position + transform.TransformDirection(Vector3.right * viewableArea.x * 0.5f * transform.lossyScale.x) + transform.TransformDirection(Vector3.up * viewableArea.y * 0.5f * transform.lossyScale.y));

		Gizmos.color = new Color(1f, 0, 0.5f, 1f);
		Gizmos.DrawLine(ul, ll);	// Left
		Gizmos.DrawLine(ll, lr);	// Bottom
		Gizmos.DrawLine(lr, ur);	// Right
		Gizmos.DrawLine(ur, ul);	// Top
	}


	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <returns>Returns a reference to the component.</returns>
	static public UIScrollList Create(string name, Vector3 pos)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		return (UIScrollList)go.AddComponent(typeof(UIScrollList));
	}

	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <param name="rotation">Rotation of the object.</param>
	/// <returns>Returns a reference to the component.</returns>
	static public UIScrollList Create(string name, Vector3 pos, Quaternion rotation)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		go.transform.rotation = rotation;
		return (UIScrollList)go.AddComponent(typeof(UIScrollList));
	}
}
