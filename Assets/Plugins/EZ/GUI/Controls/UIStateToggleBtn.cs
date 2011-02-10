//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <remarks>
/// Button class that allows you to toggle sequentially
/// through an arbitrary number of states.
/// </remarks>
[AddComponentMenu("EZ GUI/Controls/Toggle Button")]
public class UIStateToggleBtn : AutoSpriteControlBase
{
	public override bool controlIsEnabled
	{
		get { return m_controlIsEnabled; }
		set
		{
			m_controlIsEnabled = value;
			if (!value)
				DisableMe();
			else
				SetToggleState(curStateIndex);
		}
	}

	// The zero-based index of the current state
	protected int curStateIndex;

	/// <summary>
	/// Returns the zero-based number/index
	/// of the current state.
	/// </summary>
	public int StateNum
	{
		get { return curStateIndex; }
	}

	/// <summary>
	/// Returns the name of the current state.
	/// </summary>
	public string StateName
	{
		get { return states[curStateIndex].name; }
	}

	/// <summary>
	/// Zero-based index of the state that 
	/// should be the default, initial state.
	/// </summary>
	public int defaultState;

	/// Array of states that this button can have.
	[HideInInspector]
	public TextureAnim[] states = new TextureAnim[]
		{
			new TextureAnim("Unnamed"),
			new TextureAnim("Disabled")
		};

	public override TextureAnim[] States
	{
		get { return states; }
		set { states = value; }
	}

	// Strings to display for each state.
	[HideInInspector]
	public string[] stateLabels = new string[] { AutoSpriteControlBase.DittoString, AutoSpriteControlBase.DittoString, AutoSpriteControlBase.DittoString, AutoSpriteControlBase.DittoString };

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
			new EZTransitionList( new EZTransition[]	// First State
			{
				new EZTransition("From Prev")
			}),
			new EZTransitionList( new EZTransition[]	// Disabled
			{
				new EZTransition("From State")
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



	public override CSpriteFrame DefaultFrame
	{
		get
		{
			if (States[defaultState].spriteFrames.Length != 0)
				return States[defaultState].spriteFrames[0];
			else
				return null;
		}
	}

	public override TextureAnim DefaultState
	{
		get
		{
			return States[defaultState];
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
		if (deleted)
			return;

		if (!m_controlIsEnabled || IsHidden())
		{
			base.OnInput(ref ptr);
			return;
		}

		if (inputDelegate != null)
			inputDelegate(ref ptr);

		if(ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			ToggleState();
			if (soundToPlay != null)
				soundToPlay.PlayOneShot(soundToPlay.clip);
		}

		// Toggle if the required event occurred:
		if (ptr.evt == whenToInvoke)
		{
			if (scriptWithMethodToInvoke != null)
				scriptWithMethodToInvoke.Invoke(methodToInvoke, delay);
		}

		base.OnInput(ref ptr);
	}

	
	//---------------------------------------------------
	// Misc
	//---------------------------------------------------
	protected override void Awake()
	{
		base.Awake();

		curStateIndex = defaultState;
	}

	public override void Start()
	{
		base.Start();

		// Assign our aggregate layers:
		aggregateLayers = new SpriteRoot[1][];
		aggregateLayers[0] = layers;

		// Runtime init stuff:
		if (Application.isPlaying)
		{
			//---
			stateIndices = new int[layers.Length, states.Length+1];

			// Populate our state indices based on if we
			// find any valid states/animations in each 
			// sprite layer:
			int j, i;
			for (j = 0; j < states.Length; ++j)				
			{
				// Setup the transition:
				transitions[j].list[0].MainSubject = this.gameObject;

				for (i = 0; i < layers.Length; ++i)
				{
					if (layers[i] == null)
					{
						Debug.LogError("A null layer sprite was encountered on control \"" + name + "\". Please fill in the layer reference, or remove the empty element.");
						continue;
					}

					stateIndices[i, j] = layers[i].GetStateIndex(states[j].name);

					// Set the layer's state:
					if (stateIndices[i, curStateIndex] != -1)
						layers[i].SetState(stateIndices[i, curStateIndex]);
					else
						layers[i].Hide(true);

					// Add this as a subject of our transition for 
					// each state, as appropriate:
					if (stateIndices[i, j] != -1)
						transitions[j].list[0].AddSubSubject(layers[i].gameObject);
				}
			}

			// Create a default collider if none exists:
			if (collider == null)
				AddCollider();

			SetToggleState(curStateIndex);
		}

		// Since hiding while managed depends on
		// setting our mesh extents to 0, and the
		// foregoing code causes us to not be set
		// to 0, re-hide ourselves:
		if (managed && m_hidden)
			Hide(true);
	}

	public override void Copy(SpriteRoot s)
	{
		Copy(s, ControlCopyFlags.All);
	}

	public override void Copy(SpriteRoot s, ControlCopyFlags flags)
	{
		base.Copy(s, flags);

		if (!(s is UIStateToggleBtn))
			return;

		UIStateToggleBtn b = (UIStateToggleBtn)s;

		if ((flags & ControlCopyFlags.Settings) == ControlCopyFlags.Settings)
		{
			defaultState = b.defaultState;
		}

		if ((flags & ControlCopyFlags.State) == ControlCopyFlags.State)
		{
			prevTransition = b.prevTransition;
	
			if (Application.isPlaying)
				SetToggleState(b.StateNum);
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


	/// <summary>
	/// Toggles the button's state to the next in the
	/// sequence and returns the resulting state number.
	/// </summary>
	public int ToggleState()
	{
		SetToggleState(curStateIndex + 1);

		// Call our changed delegate:
		if (changeDelegate != null)
			changeDelegate(this);

		return curStateIndex;
	}

	/// <summary>
	/// Sets the button's toggle state to the specified state.
	/// </summary>
	/// <param name="s">The zero-based state number/index.</param>
	public void SetToggleState(int s)
	{
		curStateIndex = s % (states.Length-1);

		this.SetState(curStateIndex);

		this.UseStateLabel(curStateIndex);

		// Recalculate our collider
		UpdateCollider();

		// Loop through each layer and set its state,
		// provided we have a valid index for that state:
		for (int i = 0; i < layers.Length; ++i)
		{
			if (-1 != stateIndices[i, curStateIndex])
			{
				layers[i].Hide(false);
				layers[i].SetState(stateIndices[i, curStateIndex]);
			}
			else
				layers[i].Hide(true);
		}

		// End any current transition:
		if (prevTransition != null)
			prevTransition.StopSafe();

		transitions[curStateIndex].list[0].Start();
		prevTransition = transitions[curStateIndex].list[0];
	}

	/// <summary>
	/// Sets the button's toggle state to the specified state.
	/// Does nothing if the specified state is not found.
	/// </summary>
	/// <param name="s">The name of the desired state.</param>
	public void SetToggleState(string stateName)
	{
		for(int i=0; i<states.Length; ++i)
		{
			if (states[i].name == stateName)
			{
				SetToggleState(i);
				return;
			}
		}
	}

	// Sets the control to its disabled appearance:
	protected void DisableMe()
	{
		// The disabled state is the last in the states list:
		SetState(states.Length-1);

		// Set the layer states:
		for(int i=0; i<layers.Length; ++i)
		{
			if (stateIndices[i, states.Length - 1] != -1)
				layers[i].SetState(stateIndices[i, states.Length - 1]);
		}

		// End any current transition:
		if (prevTransition != null)
			prevTransition.StopSafe();

		transitions[states.Length - 1].list[0].Start();
		prevTransition = transitions[states.Length - 1].list[0];
	}

	// Sets the default UVs:
	public override void InitUVs()
	{
		if(states != null)
			if(defaultState <= states.Length-1)
				if (states[defaultState].spriteFrames.Length != 0)
					frameInfo.Copy(states[defaultState].spriteFrames[0]);

		base.InitUVs();
	}

	// Draw our state creation/deletion controls in the GUI:
	public override int DrawPreStateSelectGUI(int selState, bool inspector)
	{
		GUILayout.BeginHorizontal(GUILayout.MaxWidth(50f));

		// Add a new state
		if(GUILayout.Button(inspector?"+":"Add State", inspector?"ToolbarButton":"Button"))
		{
			// Insert the new state before the "disabled" state:
			List<TextureAnim> tempList = new List<TextureAnim>();
			tempList.AddRange(states);
			tempList.Insert(states.Length - 1, new TextureAnim("State " + (states.Length-1)));
			states = tempList.ToArray();

			// Add a transition to match:
			List<EZTransitionList> tempTrans = new List<EZTransitionList>();
			tempTrans.AddRange(transitions);
			tempTrans.Insert(transitions.Length - 1, new EZTransitionList( new EZTransition[] {new EZTransition("From Prev")} ) );
			transitions = tempTrans.ToArray();

			// Add a state label to match:
			List<string> tempLabels = new List<string>();
			tempLabels.AddRange(stateLabels);
			tempLabels.Insert(stateLabels.Length - 1, AutoSpriteControlBase.DittoString);
			stateLabels = tempLabels.ToArray();
		}

		// Only allow removing a state if it isn't
		// our last one or our "disabled" state
		// which is always our last state:
		if(states.Length > 2 && selState != states.Length-1)
		{
			// Delete a state
			if (GUILayout.Button(inspector ? "-" : "Delete State", inspector ? "ToolbarButton" : "Button"))
			{
				// Remove teh selected state:
				List<TextureAnim> tempList = new List<TextureAnim>();
				tempList.AddRange(states);
				tempList.RemoveAt(selState);
				states = tempList.ToArray();

				// Remove the associated transition:
				List<EZTransitionList> tempTrans = new List<EZTransitionList>();
				tempTrans.AddRange(transitions);
				tempTrans.RemoveAt(selState);
				transitions = tempTrans.ToArray();

				// Remove the associated label:
				List<string> tempLabels = new List<string>();
				tempLabels.AddRange(stateLabels);
				tempLabels.RemoveAt(selState);
				stateLabels = tempLabels.ToArray();
			}

			// Make sure the default state is
			// within a valid range:
			defaultState = defaultState % states.Length;
		}
		
		if (inspector)
		{
			GUILayout.FlexibleSpace();
		}


		GUILayout.EndHorizontal();

		return 14;
	}

	// Draw our state naming controls in the GUI:
	public override int DrawPostStateSelectGUI(int selState)
	{
		GUILayout.BeginHorizontal(GUILayout.MaxWidth(50f));

		GUILayout.Space(20f);
		GUILayout.Label("State Name:");

		// Only allow editing if this is not the disabled state:
		if (selState < states.Length - 1)
		{
			states[selState].name = GUILayout.TextField(states[selState].name);
		}
		else
			GUILayout.TextField(states[selState].name);

		GUILayout.EndHorizontal();

		return 28;
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
	static public UIStateToggleBtn Create(string name, Vector3 pos)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		return (UIStateToggleBtn)go.AddComponent(typeof(UIStateToggleBtn));
	}

	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <param name="rotation">Rotation of the object.</param>
	/// <returns>Returns a reference to the component.</returns>
	static public UIStateToggleBtn Create(string name, Vector3 pos, Quaternion rotation)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		go.transform.rotation = rotation;
		return (UIStateToggleBtn)go.AddComponent(typeof(UIStateToggleBtn));
	}
}
