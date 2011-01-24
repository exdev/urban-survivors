//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


// When defined, uses the parent GameObject
// as the grouping method
#define RADIOBTN_USE_PARENT


using UnityEngine;
using System.Collections;


public class RadioBtnGroup
{
#if RADIOBTN_USE_PARENT
	public Transform groupID;
#else
	public int groupID;
#endif
	public ArrayList buttons = new ArrayList();
}


/// <remarks>
/// Button class that allows you to allow selection
/// among of set of mutually exclusive options.
/// </remarks>
[AddComponentMenu("EZ GUI/Controls/Radio Button")]
public class UIRadioBtn : AutoSpriteControlBase
{
	protected enum CONTROL_STATE
	{
		True,
		False,
		Disabled
	}

	// Keeps track of the control's state
	CONTROL_STATE state;


	//---------------------------------------------
	// Static members - used to synchronize buttons
	//---------------------------------------------

	// Array of arrays of buttons. Each element is
	// a RadioBtnGroup, which holds the group number
	// and its list of buttons.
	static ArrayList buttonGroups = new ArrayList();

	//---------------------------------------------
	// End Static members
	//---------------------------------------------

	public override bool controlIsEnabled
	{
		get { return m_controlIsEnabled; }
		set
		{
			m_controlIsEnabled = value;
			if (!value)
				DisableMe();
			else
				SetButtonState();
		}
	}


	// The current value of the button
	protected bool btnValue;

	/// <summary>
	/// Provides access to the boolean value of the button.
	/// </summary>
	public bool Value
	{
		get { return btnValue; }
		set
		{
			btnValue = value;

			// Pop out the other buttons in the group:
			if (btnValue)
				PopOtherButtonsInGroup();

			// Update the button's visual state:
			SetButtonState();
		}
	}



	/// <summary>
	/// The numbered group to which this radio button 
	/// belongs.  Buttons that share a group will be 
	/// mutually exclusive to one another.
	/// This value is only available if RADIOBTN_USE_PARENT
	/// is not defined.  Otherwise, by default, radio buttons
	/// group themselves according to a common parent GameObject.
	/// </summary>
#if !RADIOBTN_USE_PARENT
	public int radioGroup;
#endif

	// Reference to the group that contains this
	// radio button.
	protected RadioBtnGroup group;

	/// <summary>
	/// The default value of the button
	/// </summary>
	public bool defaultValue;

	// State info to use to draw the appearance
	// of the control.
	[HideInInspector]
	public TextureAnim[] states = new TextureAnim[]
		{
			new TextureAnim("True"),
			new TextureAnim("False"),
			new TextureAnim("Disabled")
		};

	public override TextureAnim[] States
	{
		get { return states; }
		set { states = value; }
	}

	// Strings to display for each state.
	[HideInInspector]
	public string[] stateLabels = new string[] { AutoSpriteControlBase.DittoString, AutoSpriteControlBase.DittoString, AutoSpriteControlBase.DittoString };

	public override string GetStateLabel(int index)
	{
		return stateLabels[index];
	}

	public override void SetStateLabel(int index, string label)
	{
		stateLabels[index] = label;
	}


	// Transitions - one set for each state
	[HideInInspector]
	public EZTransitionList[] transitions = new EZTransitionList[]
		{
			new EZTransitionList( new EZTransition[]	// True
			{
				new EZTransition("From False"),
				new EZTransition("From Disabled")
			}),
			new EZTransitionList( new EZTransition[]	// False
			{
				new EZTransition("From True"),
				new EZTransition("From Disabled")
			}),
			new EZTransitionList( new EZTransition[]	// Disabled
			{
				new EZTransition("From True"),
				new EZTransition("From False")
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


	// Helps us keep track of the previous transition
	EZTransition prevTransition;

	public override CSpriteFrame DefaultFrame
	{
		get
		{
			int stateNum = btnValue ? 0 : 1;

			if (States[stateNum].spriteFrames.Length != 0)
				return States[stateNum].spriteFrames[0];
			else
				return null;
		}
	}

	public override TextureAnim DefaultState
	{
		get
		{
			int stateNum = btnValue ? 0 : 1;

			return States[stateNum];
		}
	}


	/// <summary>
	/// An array of references to sprites which will
	/// visually represent this control.  Each element
	/// (layer) represents another layer to be drawn.
	/// This allows you to use multiple sprites to draw
	/// a single control, achieving a sort of layered
	/// effect. Ex: You can use a second layer to overlay 
	/// a button with a highlight effect.
	/// </summary>
	public SpriteRoot[] layers = new SpriteRoot[0];


	/// <summary>
	/// Reference to the script component with the method
	/// you wish to invoke when the button changes states.
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
	/// Sound that will be played when the button is tapped.
	/// </summary>
	public AudioSource soundToPlay;


	//---------------------------------------------------
	// State tracking:
	//---------------------------------------------------
	protected int[,] stateIndices;


	//---------------------------------------------------
	// Input handling:
	//---------------------------------------------------
	public override void OnInput(ref POINTER_INFO ptr)
	{
		if (!m_controlIsEnabled || IsHidden())
		{
			base.OnInput(ref ptr);
			return;
		}

		if (inputDelegate != null)
			inputDelegate(ref ptr);

		// Toggle if the required event occurred:
		if (ptr.evt == whenToInvoke)
		{
			Value = true;
			if (soundToPlay != null)
				soundToPlay.PlayOneShot(soundToPlay.clip);

			if (scriptWithMethodToInvoke != null)
				scriptWithMethodToInvoke.Invoke(methodToInvoke, delay);
		}

		base.OnInput(ref ptr);
	}

	public override void Copy(SpriteRoot s)
	{
		Copy(s, ControlCopyFlags.All);
	}

	public override void Copy(SpriteRoot s, ControlCopyFlags flags)
	{
		base.Copy(s, flags);

		if (!(s is UIRadioBtn))
			return;

		UIRadioBtn b = (UIRadioBtn)s;

		if ((flags & ControlCopyFlags.State) == ControlCopyFlags.State)
		{
			state = b.state;
			prevTransition = b.prevTransition;
			if (Application.isPlaying)
				Value = b.Value;
		}

		if ((flags & ControlCopyFlags.Settings) == ControlCopyFlags.Settings)
		{
			group = b.group;
			defaultValue = b.defaultValue;
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
			soundToPlay = b.soundToPlay;
		}
	}


	//---------------------------------------------------
	// Misc
	//---------------------------------------------------
	protected override void OnEnable()
	{
		base.OnEnable();

#if RADIOBTN_USE_PARENT
		SetGroup(transform.parent);
#else
		SetGroup(radioGroup);
#endif
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		if (group == null)
			return;

		// Remove self from the group
		group.buttons.Remove(this);
		group = null;
	}


	/// <summary>
	/// Makes the radio button a part of the specified group
	/// and it will thenceforth be mutually exclusive to all
	/// other radio buttons in the same group.
	/// </summary>
	/// <param name="parent">The object that will be made the parent of the radio button.</param>
#if RADIOBTN_USE_PARENT
	public void SetGroup(Transform parent)
#else
	public void SetGroup(int groupID)
#endif
	{
		// Remove from any existing group first:
		if (group != null)
		{
			group.buttons.Remove(this);
			group = null;
		}

#if RADIOBTN_USE_PARENT
		// This line makes Unity iPhone crash:
		//transform.parent = parent;
#else
		radioGroup = groupID;
#endif


		// Add self to a button group:
		for (int i = 0; i < buttonGroups.Count; ++i)
		{
#if RADIOBTN_USE_PARENT
			if (((RadioBtnGroup)buttonGroups[i]).groupID == transform.parent)
#else
			if(((RadioBtnGroup)buttonGroups[i]).groupID == radioGroup)
#endif
			{
				group = ((RadioBtnGroup)buttonGroups[i]);
				group.buttons.Add(this);
				if (btnValue)
					PopOtherButtonsInGroup();
			}
		}
		// If we didn't find a matching group, add a new one:
		if (group == null)
		{
			group = new RadioBtnGroup();
#if RADIOBTN_USE_PARENT
			group.groupID = transform.parent;
#else
			group.groupID = radioGroup;
#endif
			group.buttons.Add(this);
			buttonGroups.Add(group);
		}
	}


	protected override void Awake()
	{
		base.Awake();

		btnValue = defaultValue;
	}

	protected override void Start()
	{
		base.Start();

		// Assign our aggregate layers:
		aggregateLayers = new SpriteRoot[1][];
		aggregateLayers[0] = layers;

		state = controlIsEnabled ? (btnValue ? CONTROL_STATE.True : CONTROL_STATE.False) : CONTROL_STATE.Disabled;
/*
		if (btnValue)
			PopOtherButtonsInGroup();
*/

		// Runtime init stuff:
		if (Application.isPlaying)
		{
			// Setup our transitions:
			transitions[0].list[0].MainSubject = this.gameObject;
			transitions[1].list[0].MainSubject = this.gameObject;
			transitions[2].list[0].MainSubject = this.gameObject;
			transitions[2].list[1].MainSubject = this.gameObject;

			stateIndices = new int[layers.Length, 3];

			// We'll use this to setup our state:
			int stateIdx = btnValue ? 0 : 1;
			stateIdx = m_controlIsEnabled ? stateIdx : 2;

			// Populate our state indices based on if we
			// find any valid states/animations in each 
			// sprite layer:
			for (int i = 0; i < layers.Length; ++i)
			{
				if (layers[i] == null)
				{
					Debug.LogError("A null layer sprite was encountered on control \"" + name + "\". Please fill in the layer reference, or remove the empty element.");
					continue;
				}

				stateIndices[i, 0] = layers[i].GetStateIndex("true");
				stateIndices[i, 1] = layers[i].GetStateIndex("false");
				stateIndices[i, 2] = layers[i].GetStateIndex("disabled");

				// Add this as a subject of our transition for 
				// each state, as appropriate:
				if (stateIndices[i, 0] != -1)
					transitions[0].list[0].AddSubSubject(layers[i].gameObject);
				if (stateIndices[i, 1] != -1)
					transitions[1].list[0].AddSubSubject(layers[i].gameObject);
				if (stateIndices[i, 2] != -1)
				{
					transitions[2].list[0].AddSubSubject(layers[i].gameObject);
					transitions[2].list[1].AddSubSubject(layers[i].gameObject);
				}

				// Set the layer's state:
				if (stateIndices[i, stateIdx] != -1)
					layers[i].SetState(stateIndices[i, stateIdx]);
				else
					layers[i].Hide(true);
			}

			// Create a default collider if none exists:
			if (collider == null)
				AddCollider();

			Value = btnValue;
		}

		// Since hiding while managed depends on
		// setting our mesh extents to 0, and the
		// foregoing code causes us to not be set
		// to 0, re-hide ourselves:
		if (managed && m_hidden)
			Hide(true);
	}

	// Sets all other buttons in the group to false.
	protected void PopOtherButtonsInGroup()
	{
		if (group == null)
			return;

		for (int i = 0; i < group.buttons.Count; ++i)
		{
			if (((UIRadioBtn)group.buttons[i]) != this)
				((UIRadioBtn)group.buttons[i]).Value = false;
		}
	}

	// Sets the button's visual state to match its value.
	protected void SetButtonState()
	{
		// Make sure we have a mesh:
		if (spriteMesh == null)
			return;

		// Make sure we're initialized since
		// we might have been called as a result of
		// another button in our group settings its
		// value on Start() before we've had a cance
		// to Start() ourselves, meaning we may lack
		// important info like a valid screensize,
		// etc. which is necessary for sizing or else
		// we'll get vertices of "infinite" value
		// resulting in a !local.IsValid() error:
		if (!m_started)
			return;

		int prevState = (int)state;
		state = controlIsEnabled ? (btnValue ? CONTROL_STATE.True : CONTROL_STATE.False) : CONTROL_STATE.Disabled;

		int index = (int)state;

		this.SetState(index);

		this.UseStateLabel(index);

		// Recalculate our collider
		UpdateCollider();

		// Loop through each layer and set its state,
		// provided we have a valid index for that state:
		for (int i = 0; i < layers.Length; ++i)
		{
			if (-1 != stateIndices[i, index])
			{
				layers[i].Hide(false);
				layers[i].SetState(stateIndices[i, index]);
			}
			else
				layers[i].Hide(true);
		}

		// End any current transition:
		if (prevTransition != null)
			prevTransition.StopSafe();

		StartTransition(index, prevState);

		// Notify our change delegate:
		if (changeDelegate != null)
			changeDelegate(this);
	}

	// Starts the appropriate transition
	protected void StartTransition(int newState, int prevState)
	{
		int transIndex = 0;

		// What state are we now in?
		switch (newState)
		{
			case 0:	// True
				// Where did we come from?
				switch (prevState)
				{
					case 1: // False
						transIndex = 0;
						break;
					case 2:	// Disabled
						transIndex = 1;
						break;
				}
				break;
			case 1:	// False
				// Where did we come from?
				switch (prevState)
				{
					case 0: // True
						transIndex = 0;
						break;
					case 2:	// Disabled
						transIndex = 1;
						break;
				}
				break;
			case 2:	// Disabled
				// Where did we come from?
				switch (prevState)
				{
					case 0: // True
						transIndex = 0;
						break;
					case 1:	// False
						transIndex = 1;
						break;
				}
				break;
		}

		transitions[newState].list[transIndex].Start();
		prevTransition = transitions[newState].list[transIndex];
	}



	// Sets the control to its disabled appearance:
	protected void DisableMe()
	{
		// The disabled state is the last in the state list:
		SetState(states.Length - 1);

		// Set the layer states:
		for (int i = 0; i < layers.Length; ++i)
		{
			if (stateIndices[i, states.Length - 1] != -1)
				layers[i].SetState(stateIndices[i, states.Length - 1]);
		}

		// End any current transition:
		if (prevTransition != null)
			prevTransition.StopSafe();

		StartTransition((int)CONTROL_STATE.Disabled, (int)state);

		state = CONTROL_STATE.Disabled;
	}

	// Sets the default UVs:
	public override void InitUVs()
	{
		int index;

		if (!m_controlIsEnabled)
			index = states.Length - 1;
		else
			index = defaultValue ? 0 : 1;

		if (states[index].spriteFrames.Length != 0)
			frameInfo.Copy(states[index].spriteFrames[0]);

		base.InitUVs();
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
	static public UIRadioBtn Create(string name, Vector3 pos)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		return (UIRadioBtn)go.AddComponent(typeof(UIRadioBtn));
	}

	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <param name="rotation">Rotation of the object.</param>
	/// <returns>Returns a reference to the component.</returns>
	static public UIRadioBtn Create(string name, Vector3 pos, Quaternion rotation)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		go.transform.rotation = rotation;
		return (UIRadioBtn)go.AddComponent(typeof(UIRadioBtn));
	}
}
