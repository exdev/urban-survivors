//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------




// Tells when set, UIManager will use a faster, but less tolerant
// method of attempting to get the IUIObject component attached 
// to the object that was clicked/touched:
// #define INTOLERANT_GET_COMPONENT




using UnityEngine;
using System.Collections;


/// <remarks>
/// Tracks important information about the status of
/// a pointing device (mouse, finger, arbitrary ray)
/// </remarks>
public struct POINTER_INFO
{
	/// <remarks>
	/// Enum IDs of various input events
	/// </remarks>
	public enum INPUT_EVENT
	{
		/// <remarks>
		/// Nothing happened from the last time 
		/// we polled the device
		/// </remarks>
		NO_CHANGE,								// Nothing happened from the last time we polled the device

		/// <remarks>
		/// The device has been pressed (mouse 
		/// button down, screen touched, etc)
		/// </remarks>
		PRESS,									// The device has been pressed (mouse button down, screen touched, etc)

		/// <remarks>
		/// The device has been released within the
		/// control's area but was not
		/// a tap (mouse button up, touch ended, etc)
		/// </remarks>
		RELEASE,								// The device has been released but was not a tap (mouse button up, touch ended, etc)

		/// <remarks>
		/// The device has been released as a tap 
		/// (mouse button up, touch ended, etc)
		/// </remarks>
		TAP,									// The device has been released as a tap (mouse button up, touch ended, etc)

		/// <remarks>
		/// The device was moved (mouse moved 
		/// with no buttons down)
		/// </remarks>
		MOVE,									// The device was moved (mouse moved with no buttons down)

		/// <summary>
		/// The device has left the area of
		/// the control.
		/// </summary>
		MOVE_OFF,

		/// <summary>
		/// The device was released (mouse button
		/// was released/finger was let go, etc)
		/// outside of the control's area.
		/// </summary>
		RELEASE_OFF,

		/// <remarks>
		/// The device was moved while being pressed (mouse 
		/// moved with button down, touch was dragged, etc)
		/// </remarks>
		DRAG									// The device was moved while being pressed (mouse moved with button down, touch was dragged, etc)
	}

	public enum POINTER_TYPE
	{
		/// <summary>
		/// The pointer type is a mouse.
		/// </summary>
		MOUSE = 0x01,

		/// <summary>
		/// The pointer type is a touchpad touch.
		/// </summary>
		TOUCHPAD = 0x02,

		/// <summary>
		/// The pointer type is either mouse or
		/// touchpad.
		/// </summary>
		MOUSE_TOUCHPAD = 0x03,

		/// <summary>
		/// The pointer is a ray pointer.
		/// </summary>
		RAY = 0x04
	}

	/// <summary>
	/// The type of pointer this is.
	/// </summary>
	public POINTER_TYPE type;

	/// <summary>
	/// ID of the pointer.
	/// </summary>
	public int id;

	/// <summary>
	/// ID of the current action.
	/// A new action ID is assigned
	/// each time the pointer goes
	/// "active" anew.
	/// </summary>
	public int actionID;

	/// <summary>
	/// The type of event the state of this pointer has generated
	/// </summary>
	public INPUT_EVENT evt;

	/// <summary>
	/// Struct that holds info about the raycast hit (if any)
	/// against a UI element.
	/// </summary>
	public RaycastHit hitInfo;

	/// <summary>
	/// A touch is currently active (finger is still on the pad, mouse button is held, etc).
	/// </summary>
	public bool active;

	/// <summary>
	/// Current position of the input device (mouse, finger, whatever).
	/// When using a mouse or touchpad, this is the screen position.
	/// When using a ray, this is the position of the pointer in world
	/// space as projected from the camera a distance specified by the
	/// UIManager's "Ray Depth" value.
	/// </summary>
	public Vector3 devicePos;

	/// <summary>
	/// Original position where the touch/click began.
	/// When using a mouse or touchpad, this is the screen position.
	/// When using a ray, this is the position of the pointer in world
	/// space as projected from the camera a distance specified by the
	/// UIManager's "Ray Depth" value.
	/// </summary>
	public Vector3 origPos;

	/// <summary>
	/// Change in the devicePos since the last polling.
	/// </summary>
	public Vector3 inputDelta;

	/// <summary>
	/// Gets set to false after a touch/click moves beyond the drag threshold.
	/// </summary>
	public bool isTap;

	/// <summary>
	/// The ray projecting into the world for this pointing device.
	/// </summary>
	public Ray ray;

	/// <summary>
	/// The ray from the previous polling.
	/// </summary>
	public Ray prevRay;

	/// <summary>
	/// Depth into the scene the ray is to be/was cast.
	/// </summary>
	public float rayDepth;

	/// <summary>
	/// The IUIObject that this pointer is affecting, if any.
	/// </summary>
	public IUIObject targetObj;

	/// <summary>
	/// The layer mask for this pointer.
	/// </summary>
	public int layerMask;

	/// <summary>
	/// Signals whether the caller that is sending this
	/// pointer info is a control or not.
	/// </summary>
	public bool callerIsControl;


	public void Copy(POINTER_INFO ptr)
	{
		type = ptr.type;
		id = ptr.id;
		actionID = ptr.actionID;
		evt = ptr.evt;
		active = ptr.active;
		devicePos = ptr.devicePos;
		origPos = ptr.origPos;
		inputDelta = ptr.inputDelta;
		ray = ptr.ray;
		prevRay = ptr.prevRay;
		rayDepth = ptr.rayDepth;
		isTap = ptr.isTap;
		targetObj = ptr.targetObj;
		layerMask = ptr.layerMask;
		hitInfo = ptr.hitInfo;
	}

	// Copies just those members needed to 
	// be able to re-use the same polled 
	// input for different cameras.
	public void Reuse(POINTER_INFO ptr)
	{
		evt = ptr.evt;
		actionID = ptr.actionID;
		active = ptr.active;
		devicePos = ptr.devicePos;
		origPos = ptr.origPos;
		inputDelta = ptr.inputDelta;
		isTap = ptr.isTap;
		hitInfo = default(RaycastHit);
	}

	// Intended only to reset values to
	// allow the pointer to be reused for
	// a new event, so do not set things
	// like layerMask or rayDepth.
	public void Reset(int actID)
	{
		actionID = actID;
		evt = INPUT_EVENT.NO_CHANGE;
		active = false;
		devicePos = Vector3.zero;
		origPos = Vector3.zero;
		inputDelta = Vector3.zero;
		ray = default(Ray);
		prevRay = default(Ray);
		isTap = true;
		hitInfo = default(RaycastHit);
	}
}


public struct KEYBOARD_INFO
{
#if UNITY_IPHONE //	|| UNITY_ANDROID
	public iPhoneKeyboardType type;
	public bool autoCorrect;
	public bool multiline;
	public bool secure;
	public bool alert;
#endif
	// The insertion point for the text
	public int insert;
}


/// <summary>
/// Contains settings for a camera in a
/// UIManager's list of cameras to use
/// when casting rays for the GUI.
/// </summary>
[System.Serializable]
public class EZCameraSettings
{
	/// <summary>
	/// A camera, through which input events will be
	/// cast into the scene.
	/// </summary>
	public Camera camera;

	/// <summary>
	/// Layer mask to use for input through this camera.
	/// </summary>
	public LayerMask mask = -1;

	/// <summary>
	/// The depth into the scene to cast input events
	/// into the scene through this camera.  Only modify 
	/// this value if you wish to limit the player's 
	/// "reach" into a 3D scene when using the mouse or 
	/// touchpad.
	/// </summary>
	public float rayDepth = Mathf.Infinity;
}


/// <remarks>
/// UIManager polls for input, and dispatches
/// input events to controls in the scene.
/// Exactly one UIManager object should be in
/// each scene that contains EZ GUI controls.
/// </remarks>
[AddComponentMenu("EZ GUI/Management/UI Manager")]
public class UIManager : MonoBehaviour
{
	//----------------------------------------------------------------
	// Singleton code
	//----------------------------------------------------------------
	// s_Instance is used to cache the instance found in the scene so we don't have to look it up every time.
	private static UIManager s_Instance = null;

	// This defines a static instance property that attempts to find the manager object in the scene and
	// returns it to the caller.
	public static UIManager instance
	{
		get
		{
			if (s_Instance == null)
			{
				s_Instance = FindObjectOfType(typeof(UIManager)) as UIManager;

				if (s_Instance == null)
					Debug.LogError("Could not locate a UIManager object. You have to have exactly one UIManager in the scene.");
			}

			return s_Instance;
		}
	}

	public static bool Exists()
	{
		return s_Instance != null;
	}
	//----------------------------------------------------------------
	// End Singleton code
	//----------------------------------------------------------------

	//----------------------------------------------------------------
	// Local enums
	//----------------------------------------------------------------
	/// <remarks>
	/// Identifiers for different types of pointing devices.
	/// </remarks>
	public enum POINTER_TYPE
	{
		/// <remarks>
		/// A mouse pointer
		/// </remarks>
		MOUSE,

		/// <remarks>
		/// Multi-touch touchpad (iPhone/iPod/iPad finger touches)
		/// </remarks>
		TOUCHPAD,

		/// <summary>
		/// Same as using TOUCHPAD when on-device, but supports
		/// both mouse and touchpad input in-editor.
		/// </summary>
		AUTO_TOUCHPAD,

		/// <remarks>
		/// A ray cast in world space
		/// </remarks>
		RAY,

		/// <remarks>
		/// Mouse pointer and ray supported simultaneously.
		/// </remarks>
		MOUSE_AND_RAY,

		/// <remarks>
		/// Touchpad and ray supported simultaneously.
		/// </remarks>
		TOUCHPAD_AND_RAY
	}

	/// <remarks>
	/// Type of ray active state
	/// </remarks>
	public enum RAY_ACTIVE_STATE
	{
		/// <summary>
		/// The ray is not active
		/// </summary>
		Inactive,

		/// <summary>
		/// The ray is active for a single frame only.
		/// </summary>
		Momentary,

		/// <summary>
		/// The ray is active until released.
		/// </summary>
		Constant
	}

	// Delegate definition for pointer poller methods.
	public delegate void PointerPollerDelegate();

	// Delegate definition for receiving pointer info
	public delegate void PointerInfoDelegate(POINTER_INFO ptr);


	/// <summary>
	/// The type of pointing device to be used
	/// </summary>
	public POINTER_TYPE pointerType = POINTER_TYPE.MOUSE;

	/// <summary>
	/// The number of pixels a device must be dragged from its
	/// original click/touch location to cause the click/touch
	/// event not to be considered to be a "tap" (click) but
	/// rather just a drag action.
	/// </summary>
	public float dragThreshold = 5f;

	/// <summary>
	/// This is similar to dragThreshold except instead of a
	/// distance in pixels, it is the world-distance 
	/// between the endpoint of the ray (based on rayDepth)
	/// when the ray went active to the endpoint of the ray
	/// in successive frames.
	/// </summary>
	public float rayDragThreshold = 2f;

	/// <summary>
	/// Depth into the scene from the camera that a ray
	/// pointer should extend.
	/// </summary>
	public float rayDepth = Mathf.Infinity;

	/// <summary>
	/// Layer mask to use for casting the Ray pointer
	/// type into the scene.
	/// </summary>
	public LayerMask rayMask = -1;

	/// <summary>
	/// Sets whether the ray pointer (if any) should be
	/// used to set the keyboard focus, at the exclusion
	/// of the mouse/touchpad pointer(s).
	/// Set this to false if you want to use the touchpad
	/// or mouse to set the keyboard focus, and set it to
	/// true if you instead want to set the focus using
	/// the ray pointer.
	/// </summary>
	public bool focusWithRay = false;

	/// <summary>
	/// Name of the virtual axis that is used when the
	/// input ray is to be active.
	/// NOTE: See the "Input" section of the Unity
	/// User Guide. Set this value to an empty string
	/// ("") if you will not be using an axis setup in
	/// Player Settings->Input and instead will use
	/// a UIActionBtn or call RayActive(). Failure to
	/// do so will result in an exception being raised.
	/// </summary>
	public string actionAxis = "Fire1";

	/// <summary>
	/// When set to true, a warning will be logged to
	/// the console whenever a non-UI object is hit by
	/// an EZ GUI input raycast (either from mouse,
	/// touchpad, or ray pointer types).
	/// </summary>
	public bool warnOnNonUiHits = true;

	/*
		/// <summary>
		/// Object whose transform's "forward" vector will
		/// be used as the source of the raycast.
		/// If this is unassigned, it will default to the
		/// UI camera.
		/// </summary>
		public GameObject raycastingObject;
	*/
	protected Transform raycastingTransform; // What we actually use at runtime

	/// <summary>
	/// The cameras to use for raycasts and the like
	/// for mouse and touchpad input.
	/// </summary>
	public EZCameraSettings[] uiCameras = new EZCameraSettings[1];

	/// <summary>
	/// The camera to use for casting the Ray
	/// pointer type.
	/// </summary>
	public Camera rayCamera;

	/// <summary>
	/// Input will not be processed while true.
	/// </summary>
	public bool blockInput = false;

	/// <summary>
	/// The default font definition to use for control text.
	/// </summary>
	public TextAsset defaultFont;

	/// <summary>
	/// The material to use for the default font.
	/// </summary>
	public Material defaultFontMaterial;

#if UNITY_IPHONE || UNITY_ANDROID
	/// <summary>
	/// Auto rotate the keyboard to portrait orientation?
	/// </summary>
	public bool autoRotateKeyboardPortrait = true;

	/// <summary>
	/// Auto rotate the keyboard to portrait upside-down orientation?
	/// </summary>
	public bool autoRotateKeyboardPortraitUpsideDown = true;

	/// <summary>
	/// Auto rotate the keyboard to landscape left orientation?
	/// </summary>
	public bool autoRotateKeyboardLandscapeLeft = true;

	/// <summary>
	/// Auto rotate the keyboard to landscape right orientation?
	/// </summary>
	public bool autoRotateKeyboardLandscapeRight = true;
#endif

	// Holds whether our ray is active or not.
	protected bool rayActive = false;
	protected RAY_ACTIVE_STATE rayState;

	// Pointer-related stuff:
	protected POINTER_INFO[,] pointers;			// Used to track the status of pointer devices (mouse pointer, finger touches, etc) (one array for each mouse/touchpad camera in use)
	protected bool[] usedPointers;				// Used to track which pointers have been used by a camera already (have already hit a control)
	protected int numPointers;					// The number of pointers we have elements for in the "pointers" arrays (one for each camera).
	protected int[] activePointers;				// Indices of active pointers (same for all cameras)
	protected int numActivePointers;			// Holds the number of active pointers (this minus 1 indicates the index of the last valid index in activePointers)
	protected POINTER_INFO rayPtr;				// The ray pointer
	protected PointerPollerDelegate pointerPoller; // Delegate we'll call to poll the pointing device(s).
	protected PointerInfoDelegate informNonUIHit; // Delegate to call when we have a non-UI raycast hit
	protected EZLinkedList<EZLinkedListNode<PointerInfoDelegate>> mouseTouchListeners = new EZLinkedList<EZLinkedListNode<PointerInfoDelegate>>(); // Delegate to call with all mouse/touchpad input
	protected EZLinkedList<EZLinkedListNode<PointerInfoDelegate>> rayListeners = new EZLinkedList<EZLinkedListNode<PointerInfoDelegate>>();	// Delegate to call with all ray input
	protected EZLinkedList<EZLinkedListNode<PointerInfoDelegate>> listenerPool = new EZLinkedList<EZLinkedListNode<PointerInfoDelegate>>(); // Pool of unused listener delegate list notes.

	// Keyboard stuff:
	protected IUIObject focusObj;				// The object that has the keyboard focus
	protected string controlText;				// Text of the focusObj.
	protected int insert;						// The index of the insertion point for the focused control.
	KEYBOARD_INFO kbInfo = default(KEYBOARD_INFO);// Used to retrieve information on the keyboard.

	// Misc.:
	protected int inputLockCount;				// Keeps track of how many objects have locked input.  This is used to prevent input during panel transitions.

	// Working vars:
	int curActionID = 0;
	int numTouches;
	protected RaycastHit hit;					// Used to perform raycasts
	protected Vector3 tempVec;
	bool down;
	IUIObject tempObj;
	POINTER_INFO tempPtr;
	System.Text.StringBuilder sb = new System.Text.StringBuilder();
#if UNITY_IPHONE //|| UNITY_ANDROID
	iPhoneKeyboard iKeyboard;
#endif


	void Awake()
	{
		// See if we are a superfluous instance:
		if (s_Instance != null)
		{
			Debug.LogError("You can only have one instance of this singleton object in existence.");
		}
		else
			s_Instance = this;

		// See if we're supposed to auto-switch to TOUCHPAD:
		if (pointerType == POINTER_TYPE.AUTO_TOUCHPAD)
		{
			if (!Application.isEditor)
				pointerType = POINTER_TYPE.TOUCHPAD;
		}

		// Determine how many touches should be supported:
		if (pointerType == POINTER_TYPE.TOUCHPAD || pointerType == POINTER_TYPE.TOUCHPAD_AND_RAY)
		{
#if UNITY_IPHONE //|| UNITY_ANDROID
			iPhoneKeyboard.autorotateToPortrait = autoRotateKeyboardPortrait;
			iPhoneKeyboard.autorotateToPortraitUpsideDown = autoRotateKeyboardPortraitUpsideDown;
			iPhoneKeyboard.autorotateToLandscapeLeft = autoRotateKeyboardLandscapeLeft;
			iPhoneKeyboard.autorotateToLandscapeRight = autoRotateKeyboardLandscapeRight;

			if (iPhoneSettings.model == "iPad")
			{
#endif
			//				if (pointerType == POINTER_TYPE.TOUCHPAD_AND_RAY)
			//					numTouches = 12;
			//				else
			numTouches = 11;
#if UNITY_IPHONE //|| UNITY_ANDROID
			}
			else
			{
// 				if (pointerType == POINTER_TYPE.TOUCHPAD_AND_RAY)
// 					numTouches = 6;
// 				else
					numTouches = 5;
			}
#endif
		}
		else if (pointerType == POINTER_TYPE.AUTO_TOUCHPAD)
			numTouches = 12;
		else if (pointerType == POINTER_TYPE.MOUSE_AND_RAY)
			numTouches = 1;
		else
			numTouches = 1;

		// Get a reference to the camera:
		if (uiCameras.Length < 1)
		{
			uiCameras = new EZCameraSettings[1];
			uiCameras[0].camera = Camera.main;
		}
		else
		{
			for (int i = 0; i < uiCameras.Length; ++i)
				if (uiCameras[i].camera == null)
					uiCameras[i].camera = Camera.main;
		}

		if (rayCamera == null)
			rayCamera = uiCameras[0].camera;
	}

	void Start()
	{
		// Allocate our pointers, etc:
		pointers = new POINTER_INFO[uiCameras.Length, numTouches];
		numPointers = numTouches;
		activePointers = new int[numTouches];
		usedPointers = new bool[numPointers];

		// Get our raycasting object:
		/*
				if (raycastingObject != null)
					raycastingTransform = raycastingObject.transform;
				else
		*/
		raycastingTransform = rayCamera.gameObject.transform;


		switch (pointerType)
		{
			case POINTER_TYPE.MOUSE:
				pointerPoller = PollMouse;
				activePointers[0] = 0;
				numActivePointers = 1;
				for (int i = 0; i < uiCameras.Length; ++i)
				{
					pointers[i, 0].id = 0;
					pointers[i, 0].rayDepth = uiCameras[i].rayDepth;
					pointers[i, 0].layerMask = uiCameras[i].mask;
					pointers[i, 0].type = POINTER_INFO.POINTER_TYPE.MOUSE;
				}
				break;
			case POINTER_TYPE.TOUCHPAD:
				pointerPoller = PollTouchpad;
				for (int i = 0; i < uiCameras.Length; ++i)
					for (int j = 0; j < numPointers; ++j)
					{
						pointers[i, j].id = j;
						pointers[i, j].rayDepth = uiCameras[i].rayDepth;
						pointers[i, j].layerMask = uiCameras[i].mask;
						pointers[i, j].type = POINTER_INFO.POINTER_TYPE.TOUCHPAD;
					}
				break;
			case POINTER_TYPE.AUTO_TOUCHPAD:
				pointerPoller = PollMouseAndTouchpad;
				for (int i = 0; i < uiCameras.Length; ++i)
				{
					for (int j = 0; j < numPointers; ++j)
					{
						pointers[i, j].id = j;
						pointers[i, j].rayDepth = uiCameras[i].rayDepth;
						pointers[i, j].layerMask = uiCameras[i].mask;
						pointers[i, j].type = POINTER_INFO.POINTER_TYPE.TOUCHPAD;
					}
					pointers[i, numPointers-1].type = POINTER_INFO.POINTER_TYPE.MOUSE;
				}
				break;
			case POINTER_TYPE.RAY:
				pointerPoller = PollRay;
				numActivePointers = 0;
				rayPtr.type = POINTER_INFO.POINTER_TYPE.RAY;
				rayPtr.id = 0;
				rayPtr.rayDepth = rayDepth;
				rayPtr.layerMask = rayMask;
				break;
			case POINTER_TYPE.MOUSE_AND_RAY:
				pointerPoller = PollMouseRay;
				activePointers[0] = 0;
				numActivePointers = 1;
				// 0 is the mouse:
				for (int i = 0; i < uiCameras.Length; ++i)
				{
					pointers[i, 0].id = 0;
					pointers[i, 0].rayDepth = uiCameras[i].rayDepth;
					pointers[i, 0].layerMask = uiCameras[i].mask;
					pointers[i, 0].type = POINTER_INFO.POINTER_TYPE.MOUSE;
				}

				rayPtr.type = POINTER_INFO.POINTER_TYPE.RAY;
				rayPtr.rayDepth = rayDepth;
				rayPtr.layerMask = rayMask;
				break;
			case POINTER_TYPE.TOUCHPAD_AND_RAY:
				pointerPoller = PollTouchpadRay;
				for (int i = 0; i < uiCameras.Length; ++i)
					for (int j = 0; j < numPointers; ++j)
					{
						pointers[i, j].id = j;
						pointers[i, j].rayDepth = uiCameras[i].rayDepth;
						pointers[i, j].layerMask = uiCameras[i].mask;
						pointers[i, j].type = POINTER_INFO.POINTER_TYPE.TOUCHPAD;
					}

				rayPtr.type = POINTER_INFO.POINTER_TYPE.RAY;
				rayPtr.rayDepth = rayDepth;
				rayPtr.layerMask = rayMask;
				break;
			default:
				Debug.LogError("ERROR: Invalid pointer type selected!");
				break;
		}
	}


	/// <summary>
	/// Registers a delegate to be called when a raycast is
	/// performed on an input event which hits a non-UI
	/// object.  Use this when your game uses raycasts for
	/// non-UI game purposes but you don't want to waste 
	/// performance with a redundant set of raycasts.
	/// </summary>
	/// <param name="del">Delegate to be called.</param>
	/// <returns>The previous delegate. It is recommended to save 
	/// this value and call it in your own delegate to preserve 
	/// the delegate chain.</returns>
	public PointerInfoDelegate SetNonUIHitDelegate(PointerInfoDelegate del)
	{
		PointerInfoDelegate oldDel = informNonUIHit;
		informNonUIHit = del;
		return oldDel;
	}


	protected EZLinkedListNode<PointerInfoDelegate> GetPtrListenNode(PointerInfoDelegate del)
	{
		EZLinkedListNode<PointerInfoDelegate> delNode;

		if (listenerPool.Count > 0)
		{
			delNode = listenerPool.Head;
			listenerPool.Remove(delNode);
			delNode.val = del;
		}
		else
			delNode = new EZLinkedListNode<PointerInfoDelegate>(del);

		return delNode;
	}


	/// <summary>
	/// Registers a delegate to be called with all mouse 
	/// and touchpad pointer input.  Use this when you 
	/// want to "listen" to all mouse or touchpad input.
	/// </summary>
	/// <param name="del">Delegate to be called.</param>
	public void AddMouseTouchPtrListener(PointerInfoDelegate del)
	{
		mouseTouchListeners.Add(GetPtrListenNode(del));
	}


	/// <summary>
	/// Registers a delegate to be called with all ray 
	/// pointer input.  Use this when you want to "listen" 
	/// to all ray input.
	/// </summary>
	/// <param name="del">Delegate to be called.</param>
	public void AddRayPtrListener(PointerInfoDelegate del)
	{
		rayListeners.Add(GetPtrListenNode(del));
	}


	// Helper method that removes a pointer listener from a list:
	protected void RemovePtrListener(EZLinkedList<EZLinkedListNode<PointerInfoDelegate>> list, PointerInfoDelegate del)
	{
		for (EZLinkedListIterator<EZLinkedListNode<PointerInfoDelegate>> i = list.Begin(); !i.Done; i.Next())
		{
			if (i.Current.val == del)
			{
				// Save it:
				EZLinkedListNode<PointerInfoDelegate> node = i.Current;

				// Remove it from the active list:
				list.Remove(i.Current);

				// Add it to our free pool:
				listenerPool.Add(node);

				// We're done.
				i.End();
				break;
			}
		}
	}


	/// <summary>
	/// Removes a mouse/touchpad pointer listener.
	/// </summary>
	/// <param name="del">Delegate to be removed.</param>
	public void RemoveMouseTouchPtrListener(PointerInfoDelegate del)
	{
		RemovePtrListener(mouseTouchListeners, del);
	}


	/// <summary>
	/// Removes a mouse/touchpad pointer listener.
	/// </summary>
	/// <param name="del">Delegate to be removed.</param>
	public void RemoveRayPtrListener(PointerInfoDelegate del)
	{
		RemovePtrListener(rayListeners, del);
	}


	// Calls all the mouse/touch pointer listeners.
	protected void CallMouseTouchListeners(POINTER_INFO ptr)
	{
		for (EZLinkedListIterator<EZLinkedListNode<PointerInfoDelegate>> i = mouseTouchListeners.Begin(); !i.Done; i.Next())
		{
			i.Current.val(ptr);
		}
	}

	// Calls all the ray pointer listeners.
	protected void CallRayListeners()
	{
		for (EZLinkedListIterator<EZLinkedListNode<PointerInfoDelegate>> i = rayListeners.Begin(); !i.Done; i.Next())
		{
			i.Current.val(rayPtr);
		}
	}


	public void OnLevelWasLoaded(int level)
	{
		// Check cameras:
		for (int i = 0; i < uiCameras.Length; ++i)
			if (uiCameras[i].camera == null)
				uiCameras[i].camera = Camera.main;

		if (rayCamera == null)
			rayCamera = Camera.main;

		// Check the focus object:
		if (focusObj == null)
			FocusObject = null;
	}


	// Update is called once per frame
	public virtual void Update()
	{
		// Poll for input:
		pointerPoller();

		// Poll the keyboard:
		if (focusObj != null)
			PollKeyboard();

		DispatchInput();
		/*
				if (Input.GetMouseButton(0))
				{
					if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, layerMask))
					{
						((UIView)views[curView]).OnTouch(hit.collider.gameObject, touchActive);
						touchActive = true;
					}
					else
					{
						// See if we have a focus object to send the event:
						if(focusObj != null)
						{

						}
						else
							touchActive = false;
					}
				}
				else
				{
					// Obviously no touch is active:
					touchActive = false;
				}
		*/
	}

	// Dispatches detected input to any hit UIObject
	protected void DispatchInput()
	{
		// Loop through each active pointer once for each camera
		// and dispatch input events:
		if (mouseTouchListeners.Count > 0)
		{
			for (int i = 0; i < uiCameras.Length; ++i)
				for (int j = 0; j < numActivePointers; ++j)
				{
					// Only handle a pointer in a successive camera if
					// it didn't already get used by a previous camera:
					if (!usedPointers[activePointers[j]])
					{
						DispatchHelper(ref pointers[i, activePointers[j]]);
						CallMouseTouchListeners(pointers[i, activePointers[j]]);
					}
				}
		}
		else
		{
			for (int i = 0; i < uiCameras.Length; ++i)
				for (int j = 0; j < numActivePointers; ++j)
				{
					// Only handle a pointer in a successive camera if
					// it didn't already get used by a previous camera:
					if (!usedPointers[activePointers[j]])
						DispatchHelper(ref pointers[i, activePointers[j]]);
				}
		}

		// Dispatch for the ray:
		if (pointerType == POINTER_TYPE.RAY ||
			pointerType == POINTER_TYPE.MOUSE_AND_RAY ||
			pointerType == POINTER_TYPE.TOUCHPAD_AND_RAY)
		{
			DispatchHelper(ref rayPtr);
			if (rayListeners.Count > 0)
				CallRayListeners();
		}

		// Unset our used flags:
		for (int i = 0; i < usedPointers.Length; ++i)
			usedPointers[i] = false;
	}

	protected void DispatchHelper(ref POINTER_INFO curPtr)
	{
		switch (curPtr.evt)
		{
			case POINTER_INFO.INPUT_EVENT.DRAG:
			case POINTER_INFO.INPUT_EVENT.RELEASE:
			case POINTER_INFO.INPUT_EVENT.TAP:
				// See if a tap or release took place outside of
				// the targetObj:
				if (curPtr.evt == POINTER_INFO.INPUT_EVENT.RELEASE ||
					curPtr.evt == POINTER_INFO.INPUT_EVENT.TAP)
				{
					tempObj = null;

					if (Physics.Raycast(curPtr.ray, out hit, curPtr.rayDepth, curPtr.layerMask))
					{
#if INTOLERANT_GET_COMPONENT
						tempObj = (IUIObject)hit.collider.gameObject.GetComponent(typeof(IUIObject));
#else
						// Else, get the component in a tolerant way:
						tempObj = (IUIObject)hit.collider.gameObject.GetComponent("IUIObject");
#endif
						curPtr.hitInfo = hit;
					}
					else
						curPtr.hitInfo = default(RaycastHit);

					// If the hit object (or lack thereof) is not
					// the same as the current target:
					if (tempObj != curPtr.targetObj)
					{
						// If we have someone to notify of a release-off:
						if (curPtr.targetObj != null && !blockInput) // Don't change the targetObj if input is blocked
						{
							// Notify the targetObj of the event:
							tempPtr.Copy(curPtr);
							// Pass on a RELEASE_OFF if it was a RELEASE event,
							// else, pass on the TAP:
							if (curPtr.evt == POINTER_INFO.INPUT_EVENT.RELEASE)
								tempPtr.evt = POINTER_INFO.INPUT_EVENT.RELEASE_OFF;
							else
								tempPtr.evt = POINTER_INFO.INPUT_EVENT.TAP;

							curPtr.targetObj.OnInput(tempPtr);

							curPtr.targetObj = tempObj;
						}

						// See if we need to notify our new target:
						if (tempObj != null)
						{
							// Only pass on a non-tap event
							// to a different target from the
							// one the pointer already had:
							if (curPtr.evt != POINTER_INFO.INPUT_EVENT.TAP && !blockInput)
								tempObj.OnInput(curPtr);
						}
					}
					else if (curPtr.targetObj != null)
					{
						// Tell our target about the event:
						if (!blockInput)
							curPtr.targetObj.OnInput(curPtr);
					}

					// See if we hit a non-UI object:
					if (tempObj == null)
					{
						if (informNonUIHit != null)
							informNonUIHit(curPtr);
					}

				}// Notify focus object:
				else
				{
					if (Physics.Raycast(curPtr.ray, out hit, curPtr.rayDepth, curPtr.layerMask))
						curPtr.hitInfo = hit;

					if (curPtr.targetObj == null)
					{
						if (informNonUIHit != null)
							informNonUIHit(curPtr);
					}
					else if (!blockInput)
					{
						curPtr.targetObj.OnInput(curPtr);
					}
				}
				break;
			case POINTER_INFO.INPUT_EVENT.NO_CHANGE:
			case POINTER_INFO.INPUT_EVENT.MOVE:
				tempObj = null;
				// See if we need to notify anyone:
				if (Physics.Raycast(curPtr.ray, out hit, curPtr.rayDepth, curPtr.layerMask))
				{
#if INTOLERANT_GET_COMPONENT
					tempObj = (IUIObject)hit.collider.gameObject.GetComponent(typeof(IUIObject));
#else
					// Else, get the component in a tolerant way:
					tempObj = (IUIObject)hit.collider.gameObject.GetComponent("IUIObject");
#endif

					curPtr.hitInfo = hit;

					if (tempObj != null)
					{
						if (!blockInput)
							tempObj.OnInput(curPtr);
					}
					else
					{
						// Let any listener know the UI was not hit
						if (informNonUIHit != null)
							informNonUIHit(curPtr);
						if (warnOnNonUiHits)
							LogNonUIObjErr(hit.collider.gameObject);
					}
				} // Let any listener know the UI was not hit
				else if (curPtr.targetObj != null && curPtr.active)
				{
					if (!blockInput)
						curPtr.targetObj.OnInput(curPtr);
				}
				else if (informNonUIHit != null)
					informNonUIHit(curPtr);


				// If the mouse/touch isn't being held, then
				// see if we're over a new object since even
				// if this was a NO_CHANGE, the control could
				// have moved out from under a stationary
				// pointer.
				if (!curPtr.active)
				{
					if (curPtr.targetObj != tempObj)
						if (curPtr.targetObj != null)
						{
							tempPtr.Copy(curPtr);
							tempPtr.evt = POINTER_INFO.INPUT_EVENT.MOVE_OFF;
							if (!blockInput)
								curPtr.targetObj.OnInput(tempPtr);
						}

					// Only update the pointer's targetObj
					// if the pointer wasn't held down and
					// if input isn't blocked:
					if (!blockInput)
						curPtr.targetObj = tempObj;
				}
				break;
			case POINTER_INFO.INPUT_EVENT.PRESS:
				if (Physics.Raycast(curPtr.ray, out hit, curPtr.rayDepth, curPtr.layerMask))
				{
#if INTOLERANT_GET_COMPONENT
					tempObj = (IUIObject)hit.collider.gameObject.GetComponent(typeof(IUIObject));
#else
					// Else, get the component in a tolerant way:
					tempObj = (IUIObject)hit.collider.gameObject.GetComponent("IUIObject");
#endif
					curPtr.hitInfo = hit;

					// If this is different than the target obj:
					if (tempObj != curPtr.targetObj && curPtr.targetObj != null)
					{
						// Tell the target obj it's a move-off:
						tempPtr.Copy(curPtr);
						tempPtr.evt = POINTER_INFO.INPUT_EVENT.MOVE_OFF;
						if (!blockInput)
							curPtr.targetObj.OnInput(tempPtr);
					}

					// Don't change the targetObj if input is blocked
					if (!blockInput)
						curPtr.targetObj = tempObj;

					if (curPtr.targetObj != null)
					{
						// If this object doesn't already have focus
						// and this is a pointer capable of setting
						// the keyboard focus:
						if (curPtr.targetObj != focusObj &&
							((curPtr.type == POINTER_INFO.POINTER_TYPE.RAY) == focusWithRay))
						{
							// See if the object will accept the
							// keyboard focus:
							if (curPtr.targetObj.GotFocus())
								FocusObject = curPtr.targetObj;
							else
								FocusObject = null;
						}

						if (!blockInput)
							curPtr.targetObj.OnInput(curPtr);
						break;
					}
					else
					{
						// Let any listener know the UI was not hit
						if (informNonUIHit != null)
							informNonUIHit(curPtr);
						if (warnOnNonUiHits)
							LogNonUIObjErr(hit.collider.gameObject);
					}

					// Make sure this pointer is allowed to unset the focus:
					if ((curPtr.type == POINTER_INFO.POINTER_TYPE.RAY) == focusWithRay)
					{
						// If we got here, the user clicked/tapped
						// somewhere other than on the focus obj:
						FocusObject = null;
					}
				}
				else
				{
					// Nothing was hit, so unset the focus object, if allowed:
					// Don't change the targetObj if input is blocked
					if (!blockInput)
						curPtr.targetObj = null;
					// Make sure this pointer is allowed to unset the focus:
					if ((curPtr.type == POINTER_INFO.POINTER_TYPE.RAY) == focusWithRay)
					{
						FocusObject = null;
					}

					// Let any listener know the UI was not hit
					if (informNonUIHit != null)
						informNonUIHit(curPtr);
				}
				break;
		}

		// See if this pointer has been used:
		if (curPtr.targetObj != null)
			usedPointers[curPtr.id] = true;
	}

	// Polls the mouse pointing device
	protected void PollMouse()
	{
		PollMouse(ref pointers[0, 0]);
		// Make a copy of the pointer for
		// each camera:
		for (int i = 1; i < uiCameras.Length; ++i)
		{
			pointers[i, 0].Reuse(pointers[0, 0]);
			pointers[i, 0].prevRay = pointers[i, 0].ray;
			pointers[i, 0].ray = uiCameras[i].camera.ScreenPointToRay(pointers[i, 0].devicePos);
		}
	}

	// Polls the mouse AND touchpad (Unity remote):
	protected void PollMouseAndTouchpad()
	{
		PollTouchpad();

		int mouseIndex = numTouches - 1;

		numActivePointers += 1; // Add one for the mouse that is always active
		activePointers[numActivePointers - 1] = mouseIndex;

		// Our mouse is the last pointer in the list:
		PollMouse(ref pointers[0, mouseIndex]);
		// Make a copy of the pointer for
		// each camera:
		for (int i = 1; i < uiCameras.Length; ++i)
		{
			pointers[i, mouseIndex].Reuse(pointers[0, mouseIndex]);
			pointers[i, mouseIndex].prevRay = pointers[i, mouseIndex].ray;
			pointers[i, mouseIndex].ray = uiCameras[i].camera.ScreenPointToRay(pointers[i, mouseIndex].devicePos);
		}
	}

	// Polls the mouse pointing device
	protected void PollMouse(ref POINTER_INFO curPtr)
	{
		down = Input.GetMouseButton(0);

		// If the device is pressed and it 
		// was already pressed, it's a drag
		// or a repeat, depending on if there
		// was movement:
		if (down && curPtr.active)
		{
			// Drag:
			if (Input.mousePosition != curPtr.devicePos)
			{
				curPtr.evt = POINTER_INFO.INPUT_EVENT.DRAG;
				curPtr.inputDelta = Input.mousePosition - curPtr.devicePos;
				curPtr.devicePos = Input.mousePosition;

				// See if we have exceeded the drag threshold:
				if (curPtr.isTap)
				{
					tempVec = curPtr.origPos - curPtr.devicePos;
					if (Mathf.Abs(tempVec.x) > dragThreshold || Mathf.Abs(tempVec.y) > dragThreshold)
						curPtr.isTap = false;
				}
			}
			else
			{
				// Nothing to see here:
				curPtr.evt = POINTER_INFO.INPUT_EVENT.NO_CHANGE;
				curPtr.inputDelta = Vector3.zero;
			}
		}
		else if (down && !curPtr.active) // Else if it's a new press, it's a press
		{
			curPtr.Reset(curActionID++);
			curPtr.evt = POINTER_INFO.INPUT_EVENT.PRESS;
			curPtr.active = true;
			curPtr.inputDelta = Input.mousePosition - curPtr.devicePos;
			curPtr.origPos = Input.mousePosition;
			curPtr.isTap = true; // True for now, until it moves too much
		}
		else if (!down && curPtr.active) // A release
		{
			curPtr.inputDelta = Input.mousePosition - curPtr.devicePos;
			curPtr.devicePos = Input.mousePosition;

			// See if we have exceeded the drag threshold:
			if (curPtr.isTap)
			{
				tempVec = curPtr.origPos - curPtr.devicePos;
				if (Mathf.Abs(tempVec.x) > dragThreshold || Mathf.Abs(tempVec.y) > dragThreshold)
					curPtr.isTap = false;
			}

			if (curPtr.isTap)
				curPtr.evt = POINTER_INFO.INPUT_EVENT.TAP;
			else
				curPtr.evt = POINTER_INFO.INPUT_EVENT.RELEASE;

			curPtr.active = false;
		}
		else if (!down && Input.mousePosition != curPtr.devicePos) // Mouse was moved
		{
			curPtr.evt = POINTER_INFO.INPUT_EVENT.MOVE;
			curPtr.inputDelta = Input.mousePosition - curPtr.devicePos;
			curPtr.devicePos = Input.mousePosition;
		}
		else
		{
			curPtr.evt = POINTER_INFO.INPUT_EVENT.NO_CHANGE;
			curPtr.inputDelta = Vector3.zero;
		}

		curPtr.devicePos = Input.mousePosition;
		curPtr.prevRay = curPtr.ray;
		curPtr.ray = uiCameras[0].camera.ScreenPointToRay(curPtr.devicePos);
	}


	// Polls the touchpad
	protected void PollTouchpad()
	{
#if UNITY_IPHONE || UNITY_ANDROID

#if UNITY_3_0
		Touch touch;
#else
		iPhoneTouch touch;
#endif
		int id;

#if UNITY_3_0
		numActivePointers = Input.touchCount;
#else
		numActivePointers = iPhoneInput.touchCount;
#endif

		// Process our touches:
		for(int i=0; i<numActivePointers; ++i)
		{
#if UNITY_3_0
			touch = Input.GetTouch(i);
#else
			touch = iPhoneInput.GetTouch(i);
#endif
			id = touch.fingerId;

			// Assign which pointer in our pointer 
			// array is active here:
			activePointers[i] = id;
			
			switch(touch.phase)
			{
				// Drag:
#if UNITY_3_0
				case TouchPhase.Moved:
#else
				case iPhoneTouchPhase.Moved:
#endif
					pointers[0, id].evt = POINTER_INFO.INPUT_EVENT.DRAG;
					pointers[0, id].inputDelta = touch.deltaPosition;
					pointers[0, id].devicePos = touch.position;

					// See if we have exceeded the drag threshold:
					if (pointers[0, id].isTap)
					{
						tempVec = pointers[0, id].origPos - pointers[0, id].devicePos;
						if (Mathf.Abs(tempVec.x) > dragThreshold || Mathf.Abs(tempVec.y) > dragThreshold)
							pointers[0, id].isTap = false;
					}
					break;

				// Press:
#if UNITY_3_0
				case TouchPhase.Began:
#else
				case iPhoneTouchPhase.Began:
#endif
					pointers[0, id].Reset(curActionID++);
					pointers[0, id].evt = POINTER_INFO.INPUT_EVENT.PRESS;
					pointers[0, id].active = true;
					pointers[0, id].inputDelta = Vector3.zero;
					pointers[0, id].origPos = touch.position;
					pointers[0, id].isTap = true; // True for now, until it moves too much
					break;

				// Release
#if UNITY_3_0
				case TouchPhase.Ended:
				case TouchPhase.Canceled:
#else
				case iPhoneTouchPhase.Ended:
				case iPhoneTouchPhase.Canceled:
#endif
					if (pointers[0, id].isTap)
						pointers[0, id].evt = POINTER_INFO.INPUT_EVENT.TAP;
					else
						pointers[0, id].evt = POINTER_INFO.INPUT_EVENT.RELEASE;

					pointers[0, id].inputDelta = touch.deltaPosition;
					pointers[0, id].active = false;
					break;

				// No change:
#if UNITY_3_0
				case TouchPhase.Stationary:
#else
				case iPhoneTouchPhase.Stationary:
#endif
					pointers[0, id].evt = POINTER_INFO.INPUT_EVENT.NO_CHANGE;
					pointers[0, id].inputDelta = Vector3.zero;
					break;
			}

			pointers[0, id].devicePos = touch.position;
			pointers[0, id].prevRay = pointers[0, id].ray;
			pointers[0, id].ray = uiCameras[0].camera.ScreenPointToRay(pointers[0, id].devicePos);
		}

		// Make a copy of the pointers for
		// each camera:
		for (int i = 1; i < uiCameras.Length; ++i)
			for (int j = 0; j < numPointers; ++j)
			{
				pointers[i, j].Reuse(pointers[0, j]);
				pointers[i, j].prevRay = pointers[i, j].ray;
				pointers[i, j].ray = uiCameras[i].camera.ScreenPointToRay(pointers[i, j].devicePos);
			}
#endif
	}


	protected void PollRay()
	{
		if (actionAxis.Length != 0)
			rayActive = Input.GetButton(actionAxis);
		else
		{
			rayActive = rayState != RAY_ACTIVE_STATE.Inactive;

			// Deactivate a momentary action:
			if (rayState == RAY_ACTIVE_STATE.Momentary)
				rayState = RAY_ACTIVE_STATE.Inactive;
		}

		// If the device is pressed and it 
		// was already pressed, it's a drag
		// or a repeat, depending on if there
		// was movement:
		if (rayActive && rayPtr.active)
		{
			// Drag:
			if (raycastingTransform.forward != rayPtr.ray.direction ||
				raycastingTransform.position != rayPtr.ray.origin)
			{
				rayPtr.evt = POINTER_INFO.INPUT_EVENT.DRAG;
				tempVec = raycastingTransform.position + raycastingTransform.forward * rayDepth;
				rayPtr.inputDelta = tempVec - rayPtr.devicePos;
				rayPtr.devicePos = tempVec;

				// See if we have exceeded the drag threshold:
				if (rayPtr.isTap)
				{
					tempVec = rayPtr.origPos - rayPtr.devicePos;
					if (tempVec.sqrMagnitude > (rayDragThreshold * rayDragThreshold))
						rayPtr.isTap = false;
				}
			}
			else
			{
				// Nothing to see here:
				rayPtr.evt = POINTER_INFO.INPUT_EVENT.NO_CHANGE;
				rayPtr.inputDelta = Vector3.zero;
			}
		}
		else if (rayActive && !rayPtr.active) // Else if it's a new press, it's a press
		{
			rayPtr.Reset(curActionID++);
			rayPtr.evt = POINTER_INFO.INPUT_EVENT.PRESS;
			rayPtr.active = true;
			rayPtr.origPos = raycastingTransform.position + raycastingTransform.forward * rayDepth;
			rayPtr.inputDelta = rayPtr.origPos - rayPtr.devicePos;
			rayPtr.devicePos = rayPtr.origPos;
			rayPtr.isTap = true; // True for now, until it moves too much
		}
		else if (!rayActive && rayPtr.active) // A release
		{
			if (rayPtr.isTap)
				rayPtr.evt = POINTER_INFO.INPUT_EVENT.TAP;
			else
				rayPtr.evt = POINTER_INFO.INPUT_EVENT.RELEASE;

			tempVec = raycastingTransform.position + raycastingTransform.forward * rayDepth;
			rayPtr.inputDelta = tempVec - rayPtr.devicePos;
			rayPtr.devicePos = tempVec;
			rayPtr.active = false;
		}
		else if (!rayActive && Input.mousePosition != rayPtr.devicePos) // Mouse was moved
		{
			rayPtr.evt = POINTER_INFO.INPUT_EVENT.MOVE;
			tempVec = raycastingTransform.position + raycastingTransform.forward * rayDepth;
			rayPtr.inputDelta = tempVec - rayPtr.devicePos;
			rayPtr.devicePos = tempVec;
		}
		else
		{
			rayPtr.evt = POINTER_INFO.INPUT_EVENT.NO_CHANGE;
			rayPtr.inputDelta = Vector3.zero;
		}

		rayPtr.prevRay = rayPtr.ray;
		rayPtr.ray = new Ray(raycastingTransform.position, raycastingTransform.forward);
	}

	protected void PollMouseRay()
	{
		PollMouse();
		PollRay();
	}

	protected void PollTouchpadRay()
	{
		PollTouchpad();
		PollRay();
	}

	protected void PollKeyboard()
	{
#if UNITY_IPHONE //|| UNITY_ANDROID
		if(!Application.isEditor)
		{
			if(iKeyboard == null)
				return;

			if(controlText == iKeyboard.text)
				return; // Nothing to do

			controlText = iKeyboard.text;
			
			if(iKeyboard.done)
			{
				controlText = focusObj.SetInputText(controlText, ref insert);
				FocusObject = null;
				return;
			}
			
			iKeyboard.text = focusObj.SetInputText(controlText, ref insert);
		}
		else
			ProcessKeyboard();
#else
		ProcessKeyboard();

		// Look for arrow keys:
		if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			insert = Mathf.Min(controlText.Length, insert + 1);
			// Don't stop on a newline:
			while(insert < controlText.Length && controlText[insert] == '\n')
				++insert;
			focusObj.SetInputText(controlText, ref insert);
		}
		else if(Input.GetKeyDown(KeyCode.LeftArrow))
		{
			insert = Mathf.Max(0, insert - 1);
			focusObj.SetInputText(controlText, ref insert);
			// Don't stop on a newline:
			while (insert > 0 && controlText[insert] == '\n')
				--insert;
		}
		else if(Input.GetKeyDown(KeyCode.Home))
		{
			insert = 0;
			focusObj.SetInputText(controlText, ref insert);
		}
		else if(Input.GetKeyDown(KeyCode.End))
		{
			insert = controlText.Length;
			focusObj.SetInputText(controlText, ref insert);
		}
#endif
	}

	protected void ProcessKeyboard()
	{
		if (Input.inputString.Length == 0)
			return;

		insert = Mathf.Clamp(insert, 0, controlText.Length);

		if (sb.Length > 0)
			sb.Replace(sb.ToString(), controlText);
		else
			sb.Append(controlText);

		foreach (char c in Input.inputString)
		{
			// Backspace
			if (c == '\b')
			{
				insert = Mathf.Max(0, insert - 1);
				if (insert < sb.Length)
					sb.Remove(insert, 1);
				continue;
			}

			sb.Insert(insert, c);
			++insert;
		}

		controlText = sb.ToString();
		controlText = focusObj.SetInputText(controlText, ref insert);
	}

	/// <summary>
	/// Used to set the active status of the ray.
	/// When set to Momentary or Constant, this is 
	/// analogous to having the mouse button down.
	/// </summary>
	public RAY_ACTIVE_STATE RayActive
	{
		get { return rayState; }
		set { rayState = value; }
	}

	/// <summary>
	/// Adds a lock to the input.
	/// As long as there is at least
	/// one lock on input, input will
	/// not be processed.
	/// Use UnlockInput() to release
	/// a lock.  It is important to
	/// call UnlockInput() as many
	/// times as LockInput() for
	/// input processing to resume.
	/// </summary>
	public void LockInput()
	{
		blockInput = true;
		++inputLockCount;
	}

	/// <summary>
	/// Removes a lock on input.
	/// Input will not be processed
	/// until all locks placed by
	/// LockInput() have been removed.
	/// There must be as many calls to
	/// UnlockInput() as there were
	/// LockInput() for input processing
	/// to resume.
	/// </summary>
	public void UnlockInput()
	{
		--inputLockCount;
		if (inputLockCount < 1)
		{
			inputLockCount = 0; // Clamp to 0
			blockInput = false;
		}
	}

	/// <summary>
	/// Accessor for the object which has the
	/// keyboard focus.
	/// </summary>
	public IUIObject FocusObject
	{
		get { return focusObj; }
		set
		{
			// See if another object is losing focus:
			if (focusObj != null)
				focusObj.LostFocus();

			focusObj = value;

			if (focusObj != null)
			{
				controlText = focusObj.GetInputText(ref kbInfo);
				if (controlText == null)
					controlText = "";	// To be safe

#if UNITY_IPHONE //|| UNITY_ANDROID
				if(!Application.isEditor)
				{
					iKeyboard = iPhoneKeyboard.Open(controlText, kbInfo.type, kbInfo.autoCorrect, kbInfo.multiline, kbInfo.secure, kbInfo.alert, controlText);
					iKeyboard.text = controlText;
				}
#endif
				insert = kbInfo.insert;

				if (sb.Length > 0)
					sb.Replace(sb.ToString(), controlText);
				else
					sb.Append(controlText);
			}
#if UNITY_IPHONE //|| UNITY_ANDROID
			else
			{
				if(iKeyboard != null)
				{
					iKeyboard.active = false;
					iKeyboard = null;
				}
			}
#endif
		}
	}

	/// <summary>
	/// Accessor for the text input insertion point.
	/// </summary>
	public int InsertionPoint
	{
		get { return insert; }
		set { insert = value; }
	}


	/////////////////////////////////////////////////////
	//	Error logging routines
	/////////////////////////////////////////////////////

	protected void LogNonUIObjErr(GameObject obj)
	{
		Debug.LogWarning("The UIManager encountered a collider on object \"" + obj.name + "\" that does not not contain a UIObject or derivative component.  Please double-check that this object has the correct layer and components assigned.");
	}
}
