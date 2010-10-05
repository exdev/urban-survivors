//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


using UnityEngine;
using System.Collections;


/// <remarks>
/// Button class that allows you to invoke a specified method
/// on a specified component script.
/// This differs from UIButton in that it has no sprite
/// graphics and instead is intended to be used in conjuction
/// with an existing 3D object in the scene.
/// </remarks>
[AddComponentMenu("EZ GUI/Controls/3D Button")]
public class UIButton3D : ControlBase
{
	/// <summary>
	/// Indicates the state of the button
	/// </summary>
	public enum CONTROL_STATE
	{
		/// <summary>
		/// The button is "normal", awaiting input
		/// </summary>
		NORMAL,

		/// <summary>
		/// The button has an input device hovering over it.
		/// </summary>
		OVER,

		/// <summary>
		/// The button is being pressed
		/// </summary>
		ACTIVE,

		/// <summary>
		/// The button is disabled
		/// </summary>
		DISABLED
	};


	protected CONTROL_STATE m_ctrlState;

	/// <summary>
	/// Gets the current state of the button.
	/// </summary>
	public CONTROL_STATE controlState
	{
		get { return m_ctrlState; }
	}

	public override bool controlIsEnabled
	{
		get { return m_controlIsEnabled; }
		set
		{
			m_controlIsEnabled = value;
			if (!value)
				SetControlState(CONTROL_STATE.DISABLED);
			else
				SetControlState(CONTROL_STATE.NORMAL);
		}
	}

	protected string[] states = { "Normal", "Over", "Active", "Disabled" };

	public override string[] States
	{
		get { return states; }
	}

	// Transitions - one set for each state
	[HideInInspector]
	public EZTransitionList[] transitions = new EZTransitionList[]
		{
			new EZTransitionList( new EZTransition[]	// Normal
			{
				new EZTransition("From Over"),
				new EZTransition("From Active"),
				new EZTransition("From Disabled")
			}),
			new EZTransitionList( new EZTransition[]	// Over
			{
				new EZTransition("From Normal"),
				new EZTransition("From Active")
			}),
			new EZTransitionList( new EZTransition[]	// Active
			{
				new EZTransition("From Normal"),
				new EZTransition("From Over")
			}),
			new EZTransitionList( new EZTransition[]	// Disabled
			{
				new EZTransition("From Normal"),
				new EZTransition("From Over"),
				new EZTransition("From Active")
			})
		};

	public override EZTransitionList GetTransitions(int index)
	{
		if (index >= transitions.Length)
			return null;
		return transitions[index];
	}

	public override EZTransitionList[] Transitions
	{
		get { return transitions; }
		set { transitions = value; }
	}

	// Helps us keep track of the previous transition:
	EZTransition prevTransition;


	/// <summary>
	/// Reference to the script component with the method
	/// you wish to invoke when the button is tapped.
	/// </summary>
	public MonoBehaviour scriptWithMethodToInvoke;

	/// <summary>
	/// A string containing the name of the method to be invoked.
	/// </summary>
	public string methodToInvoke;

	/// <summary>
	/// Sets what event should have occurred to 
	/// invoke the associated MonoBehaviour method.
	/// Defaults to TAP.
	/// </summary>
	public POINTER_INFO.INPUT_EVENT whenToInvoke = POINTER_INFO.INPUT_EVENT.TAP;

	/// <summary>
	/// Delay, in seconds, between the time the control is tapped
	/// and the time the method is executed.
	/// </summary>
	public float delay;

	/// <summary>
	/// Sound that will be played when the button is is in an "over" state (mouse over)
	/// </summary>
	public AudioSource soundOnOver;

	/// <summary>
	/// Sound that will be played when the button is activated (pressed)
	/// </summary>
	public AudioSource soundOnClick;

	/// <summary>
	/// When repeat is true, the button will call the various
	/// delegates and invokes as long as the button is held
	/// down.
	/// NOTE: If repeat is true, it overrides any setting of
	/// "whenToInvoke"/"When To Invoke". One exception to this
	/// is that "soundToPlay" is still played based upon
	/// "whenToInvoke".
	/// </summary>
	public bool repeat;


	
	//---------------------------------------------------
	// Input handling:
	//---------------------------------------------------
	public override void OnInput(POINTER_INFO ptr)
	{
		if (!m_controlIsEnabled)
		{
			base.OnInput(ptr);
			return;
		}

		if (inputDelegate != null)
			inputDelegate(ref ptr);

		// Change the state if necessary:
		switch(ptr.evt)
		{
			case POINTER_INFO.INPUT_EVENT.MOVE:
				if (m_ctrlState != CONTROL_STATE.OVER)
				{
					SetControlState(CONTROL_STATE.OVER);
					if (soundOnOver != null)
						soundOnOver.PlayOneShot(soundOnOver.clip);
				}
				break;
			case POINTER_INFO.INPUT_EVENT.DRAG:
			case POINTER_INFO.INPUT_EVENT.PRESS:
				SetControlState(CONTROL_STATE.ACTIVE);
				break;
			case POINTER_INFO.INPUT_EVENT.RELEASE:
			case POINTER_INFO.INPUT_EVENT.TAP:
				// Only go to the OVER state if we have
				// have frame info for that or if we aren't
				// in touchpad mode, or if the collider hit
				// by the touch was actually us, indicating
				// that we're still under the pointer:
				if (ptr.type != POINTER_INFO.POINTER_TYPE.TOUCHPAD &&
					ptr.hitInfo.collider == collider)
					SetControlState(CONTROL_STATE.OVER);
				else
					SetControlState(CONTROL_STATE.NORMAL);
				break;
			case POINTER_INFO.INPUT_EVENT.MOVE_OFF:
			case POINTER_INFO.INPUT_EVENT.RELEASE_OFF:
				SetControlState(CONTROL_STATE.NORMAL);
				break;
		}

		base.OnInput(ptr);

		if (repeat)
		{
			if (m_ctrlState == CONTROL_STATE.ACTIVE)
				goto Invoke;
		}
		else if (ptr.evt == whenToInvoke)
			goto Invoke;

		return;

		Invoke:
		if (ptr.evt == whenToInvoke)
		{
			if (soundOnClick != null)
				soundOnClick.PlayOneShot(soundOnClick.clip);
		}
		if (scriptWithMethodToInvoke != null)
			scriptWithMethodToInvoke.Invoke(methodToInvoke, delay);
		if (changeDelegate != null)
			changeDelegate(this);
	}

	
	//---------------------------------------------------
	// Misc
	//---------------------------------------------------
	protected virtual void Start()
	{
		// Runtime init stuff:
		if(Application.isPlaying)
		{
			// Setup our transitions:
			transitions[0].list[0].MainSubject = this.gameObject;
			transitions[0].list[1].MainSubject = this.gameObject;
			transitions[0].list[2].MainSubject = this.gameObject;
			transitions[1].list[0].MainSubject = this.gameObject;
			transitions[1].list[1].MainSubject = this.gameObject;
			transitions[2].list[0].MainSubject = this.gameObject;
			transitions[2].list[1].MainSubject = this.gameObject;
			transitions[3].list[0].MainSubject = this.gameObject;
			transitions[3].list[1].MainSubject = this.gameObject;
			transitions[3].list[2].MainSubject = this.gameObject;


			// Create a default collider if none exists:
			if (collider == null)
			{
				AddCollider();
			}

//			SetState((int)m_ctrlState);
		}
	}

	public override void Copy(IControl c)
	{
		Copy(c, ControlCopyFlags.All);
	}

	public override void Copy(IControl c, ControlCopyFlags flags)
	{
		base.Copy(c, flags);

		if (!(c is UIButton3D))
			return;

		UIButton3D b = (UIButton3D)c;

		if ((flags & ControlCopyFlags.State) == ControlCopyFlags.State)
		{
			prevTransition = b.prevTransition;

			if (Application.isPlaying)
				SetControlState(b.controlState);
		}

		if ((flags & ControlCopyFlags.Invocation) == ControlCopyFlags.Invocation)
		{
			scriptWithMethodToInvoke = b.scriptWithMethodToInvoke;
			methodToInvoke = b.methodToInvoke;
			whenToInvoke = b.whenToInvoke;
			delay = b.delay;
		}

		if ((flags & ControlCopyFlags.Sound) == ControlCopyFlags.Sound)
		{
			soundOnOver = b.soundOnOver;
			soundOnClick = b.soundOnClick;
		}

		if ((flags & ControlCopyFlags.Settings) == ControlCopyFlags.Settings)
		{
			repeat = b.repeat;
		}
	}

	// Switches the displayed sprite(s) to match the current state:
	protected void SetControlState(CONTROL_STATE s)
	{
		// If this is the same as the current state, ignore:
		if (m_ctrlState == s)
			return;

		int prevState = (int)m_ctrlState;

		m_ctrlState = s;

		// End any current transition:
		if (prevTransition != null)
			prevTransition.StopSafe();

		// Start a new transition:
		StartTransition((int)s, prevState);
	}

	// Starts the appropriate transition
	protected void StartTransition(int newState, int prevState)
	{
		int transIndex = 0;

		// What state are we now in?
		switch(newState)
		{
			case 0:	// Normal
				// Where did we come from?
				switch(prevState)
				{
					case 1: // Over
						transIndex = 0;
						break;
					case 2:	// Active
						transIndex = 1;
						break;
					case 3:	// Disabled
						transIndex = 2;
						break;
				}
				break;
			case 1:	// Over
				// Where did we come from?
				switch (prevState)
				{
					case 0: // Normal
						transIndex = 0;
						break;
					case 2:	// Active
						transIndex = 1;
						break;
				}
				break;
			case 2:	// Active
				// Where did we come from?
				switch (prevState)
				{
					case 0: // Normal
						transIndex = 0;
						break;
					case 1:	// Over
						transIndex = 1;
						break;
				}
				break;
			case 3:	// Disabled
				// Where did we come from?
				switch (prevState)
				{
					case 0: // Normal
						transIndex = 0;
						break;
					case 1:	// Over
						transIndex = 1;
						break;
					case 2:	// Active
						transIndex = 2;
						break;
				}
				break;
		}

		prevTransition = transitions[newState].list[transIndex];
		prevTransition.Start();
	}


	public override void DrawPreTransitionUI(int selState, IGUIScriptSelector gui)
	{
		scriptWithMethodToInvoke = gui.DrawScriptSelection(scriptWithMethodToInvoke, ref methodToInvoke);
	}


	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <returns>Returns a reference to the component.</returns>
	static public UIButton3D Create(string name, Vector3 pos)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		return (UIButton3D)go.AddComponent(typeof(UIButton3D));
	}

	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <param name="rotation">Rotation of the object.</param>
	/// <returns>Returns a reference to the component.</returns>
	static public UIButton3D Create(string name, Vector3 pos, Quaternion rotation)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		go.transform.rotation = rotation;
		return (UIButton3D)go.AddComponent(typeof(UIButton3D));
	}
}
