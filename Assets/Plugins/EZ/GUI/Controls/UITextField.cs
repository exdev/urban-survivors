//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


using UnityEngine;
using System.Collections;


/// <remarks>
/// Class which allows single-line text input.
/// </remarks>
[AddComponentMenu("EZ GUI/Controls/Text Field")]
public class UITextField : AutoSpriteControlBase
{
	public override bool controlIsEnabled
	{
		get { return m_controlIsEnabled; }
		set
		{
			m_controlIsEnabled = value;
		}
	}

	// State info to use to draw the appearance
	// of the control.
	[HideInInspector]
	public TextureAnim[] states = new TextureAnim[]
		{
			new TextureAnim("Field graphic"),
			new TextureAnim("Caret")
		};

	public override TextureAnim[] States
	{
		get { return states; }
		set { states = value; }
	}

	// Transitions - Controls caret flash
	[HideInInspector]
	public EZTransitionList[] transitions = new EZTransitionList[]
		{
			null,
			new EZTransitionList( new EZTransition[]	// Caret flash
			{
				new EZTransition("Caret Flash")
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

	/// <summary>
	/// The distance, in local units, inward from the edges of the
	/// control that the text should be clipped.
	/// </summary>
	public Vector2 margins;
	protected Rect3D clientClippingRect; // The clipping rect against which the text will be clipped
	// Corners of the margins
	protected Vector2 marginTopLeft;
	protected Vector2 marginBottomRight;

	/// <summary>
	/// The maximum number of characters the field can hold.
	/// A value of 0 is unlimited.
	/// </summary>
	public int maxLength;

	/// <summary>
	/// Whether or not this field will accept multi-line input.
	/// </summary>
	public bool multiline = false;

	/// <summary>
	/// The size, in local units, of the caret sprite.
	/// This can be left at default if using pixel-perfect.
	/// </summary>
	public Vector2 caretSize;

	/// <summary>
	/// The anchor method to be used by the caret.
	/// </summary>
	public SpriteRoot.ANCHOR_METHOD caretAnchor = ANCHOR_METHOD.BOTTOM_LEFT;

	/// <summary>
	/// The distance, in local units, that the caret will be
	/// offset from the text, along the Z-axis.
	/// </summary>
	public float caretZOffset = -0.1f;

#if UNITY_IPHONE || UNITY_ANDROID
	/// <summary>
	/// The type of keyboard to display. (iPhone OS only)
	/// </summary>
	public iPhoneKeyboardType type;

	/// <summary>
	/// Whether to use auto correction. (iPhone OS only)
	/// </summary>
	public bool autoCorrect;

	/// <summary>
	/// Whether the keyboard should be shown in secure mode. (iPhone OS only)
	/// </summary>
	public bool secure;

	/// <summary>
	/// Whether the keyboard should be shown in alert mode. (iPhone OS only)
	/// </summary>
	public bool alert;
#endif

	/// <summary>
	/// Sound that will be played when the field receives keyboard input
	/// </summary>
	public AudioSource typingSoundEffect;

	/// <summary>
	/// Sound to play if typing has exceeded the
	/// field's length.
	/// </summary>
	public AudioSource fieldFullSound;

	// Sprite that will represent the caret.
	protected AutoSprite caret;


	// The text insertion point
	protected int insert;

	// Lets us keep track of whether we've moved
	protected Vector3 cachedPos;
	protected Quaternion cachedRot;
	protected Vector3 cachedScale;

	// state tracking:
	protected bool hasFocus = false;

	// Misc.
	protected Vector3 origTextPos;


	//---------------------------------------------------
	// State tracking:
	//---------------------------------------------------
	protected int[,] stateIndices;


	//---------------------------------------------------
	// Input handling:
	//---------------------------------------------------
	public override void OnInput(POINTER_INFO ptr)
	{
		if (!m_controlIsEnabled || IsHidden())
		{
			base.OnInput(ptr);
			return;
		}

		if (inputDelegate != null)
			inputDelegate(ref ptr);

		// Change the state if necessary:
		switch (ptr.evt)
		{
			case POINTER_INFO.INPUT_EVENT.TAP:
				// Find our insertion position:
				break;
		}

		base.OnInput(ptr);
	}

	public override void Copy(SpriteRoot s)
	{
		Copy(s, ControlCopyFlags.All);
	}

	public override void Copy(SpriteRoot s, ControlCopyFlags flags)
	{
		base.Copy(s, flags);

		if (!(s is UITextField))
			return;

		UITextField b = (UITextField)s;


		if ((flags & ControlCopyFlags.Settings) == ControlCopyFlags.Settings)
		{
			maxLength = b.maxLength;

#if UNITY_IPHONE || UNITY_ANDROID
			type = b.type;
			autoCorrect = b.autoCorrect;
			secure = b.secure;
			alert = b.alert;
#endif
			typingSoundEffect = b.typingSoundEffect;
			fieldFullSound = b.fieldFullSound;
		}

		if ((flags & ControlCopyFlags.Appearance) == ControlCopyFlags.Appearance)
		{
			caret.Copy(b.caret);
		}

		if ((flags & ControlCopyFlags.State) == ControlCopyFlags.State)
		{
			insert = b.insert;
			Text = b.Text;
		}
	}


	public override bool GotFocus()
	{
		hasFocus = m_controlIsEnabled;

		return m_controlIsEnabled;
	}

	public override string GetInputText(ref KEYBOARD_INFO info)
	{
		info.insert = insert;
#if UNITY_IPHONE
		info.type = type;
		info.autoCorrect = autoCorrect;
		info.multiline = false;
		info.secure = secure;
		info.alert = alert;
#endif
		// Show our caret
		ShowCaret();

		return text;
	}

	public override string SetInputText(string inputText, ref int insertPt)
	{
		// Validate our input:
		if(!multiline)
		{
			int idx;
			// Check for Enter:
			if ((idx = inputText.IndexOf('\n')) != -1)
			{
				inputText = inputText.Substring(0, idx);
				UIManager.instance.FocusObject = null;
			}
			if ((idx = inputText.IndexOf('\r')) != -1)
			{
				inputText = inputText.Substring(0, idx);
				UIManager.instance.FocusObject = null;
			}
		}

		if (inputText.Length > maxLength && maxLength > 0)
		{
			Text = inputText.Substring(0, maxLength);
			insert = Mathf.Clamp(insertPt, 0, maxLength);

			if (fieldFullSound != null)
				fieldFullSound.PlayOneShot(fieldFullSound.clip);
		}
		else
		{
			Text = inputText;
			insert = insertPt;

			if (typingSoundEffect != null)
				typingSoundEffect.PlayOneShot(typingSoundEffect.clip);

			if (changeDelegate != null)
				changeDelegate(this);
		}

/*
		if (text.Length > 0)
		{
			if (caret != null)
				if (caret.IsHidden())
					ShowCaret();
		}
		else
			HideCaret();
*/

		if(caret != null)
		{
			if (caret.IsHidden() && hasFocus)
				caret.Hide(false);
			PositionCaret();
		}

		return text;
	}

	public override void LostFocus()
	{
		hasFocus = false;

		// Hide our caret
		HideCaret();
	}

	protected void ShowCaret()
	{
		if (caret == null)
			return;

		caret.Hide(false);
		PositionCaret();

		// Make sure the caret is still showing:
		if (!caret.IsHidden())
			transitions[1].list[0].Start();
	}

	public override void Hide(bool tf)
	{
		base.Hide(tf);

		if (caret != null)
		{
			if (!tf && hasFocus)
				caret.Hide(tf);
			else
				caret.Hide(true);
		}
	}

	protected void HideCaret()
	{
		if (caret == null)
			return;

		transitions[1].list[0].StopSafe();
		caret.Hide(true);
	}

	protected void PositionCaret()
	{
		if (caret == null || spriteText == null)
			return;

		//insert = Mathf.Min(insert, spriteText.DisplayString.Length);

		Vector3 pos = transform.InverseTransformPoint(spriteText.GetInsertionPointPos(spriteText.PlainIndexToDisplayIndex(insert)));

		// See if the current character is in our viewable area:
		Vector3 top = pos + Vector3.up * spriteText.BaseHeight;
		if(top.y > marginTopLeft.y)
		{
			spriteText.transform.localPosition -= Vector3.up * spriteText.LineSpan;
			PositionCaret();
			// Re-clip our text:
			spriteText.ClippingRect = clientClippingRect;
			return;
		}
		else if(pos.y < marginBottomRight.y)
		{
			spriteText.transform.localPosition += Vector3.up * spriteText.LineSpan;
			PositionCaret();
			// Re-clip our text:
			spriteText.ClippingRect = clientClippingRect;
			return;
		}
		else if(!multiline) // Only check left/right if we're not multiline
		{
			if (pos.x < marginTopLeft.x)
			{
				Vector3 center = GetCenterPoint();
				// Move it so that the current character is in the middle:
				Vector3 newTxtPos = spriteText.transform.localPosition + Vector3.right * Mathf.Abs(center.x - pos.x);
				// Don't move right of its starting position:
				newTxtPos.x = Mathf.Min(newTxtPos.x, origTextPos.x);
				spriteText.transform.localPosition = newTxtPos;
				PositionCaret();
				// Re-clip our text:
				spriteText.ClippingRect = clientClippingRect;
				return;
			}
			else if (pos.x > marginBottomRight.x)
			{
				Vector3 center = GetCenterPoint();
				// Move it so that the current character is in the middle:
				Vector3 newTxtPos = spriteText.transform.localPosition - Vector3.right * Mathf.Abs(center.x - pos.x);
				spriteText.transform.localPosition = newTxtPos;
				PositionCaret();
				// Re-clip our text:
				spriteText.ClippingRect = clientClippingRect;
				return;
			}
		}

		transitions[1].list[0].StopSafe();

		pos.z += caretZOffset;
		caret.transform.localPosition = pos;

		transitions[1].list[0].Start();
	}

	// Accepts a point in world space and finds which
	// text character is nearest to it
	protected void PositionInsertionPoint(Vector3 pt)
	{
		if (caret == null || spriteText == null)
			return;

		/*
				pt = spriteText.transform.InverseTransformPoint(pt);

				Vector3[] verts = txtMesh.vertices;

				// Find the nearest vertex to the right of our point:
				float dist;
				float minDist = 99999f;
				int nearest;

				for(int i=0; i<txtMesh.vertexCount; ++i)
				{
					dist = verts[i].x - pt.x;
					if (dist < minDist)
					{
						minDist = dist;
						nearest = i;
					}
				}

				int[] tris = txtMesh.triangles;

				// Now find the triangle that contains this vertex:
				for(int i=0; i<tris.Length; ++i)
				{

				}
		*/
	}


	//---------------------------------------------------
	// Misc
	//---------------------------------------------------
	protected override void Awake()
	{
		base.Awake();

		defaultTextAlignment = SpriteText.Alignment_Type.Left;
		defaultTextAnchor = SpriteText.Anchor_Pos.Upper_Left;
	}

	protected override void Start()
	{
		base.Start();

		// Create a TextMesh object if none exists:
		if (spriteText == null)
		{
			Text = " ";
			Text = "";
		}

		if(spriteText != null)
		{
			origTextPos = spriteText.transform.localPosition;
			SetMargins(margins);
		}

		// Set the insertion point to the end by default:
		insert = Text.Length;

		// Runtime init stuff:
		if (Application.isPlaying)
		{
			// Create a default collider if none exists:
			if (collider == null)
				AddCollider();

			// Create our caret and hide it by default:
			GameObject go = new GameObject();
			go.name = name + " - caret";
			go.transform.parent = transform;
			go.transform.localPosition = Vector3.zero;
			go.transform.localRotation = Quaternion.identity;
			go.transform.localScale = Vector3.one;
			go.layer = gameObject.layer;
			caret = (AutoSprite)go.AddComponent(typeof(AutoSprite));
			caret.plane = plane;
			caret.SetAnchor(caretAnchor);
			caret.persistent = persistent;
			if (!managed)
			{
				if (caret.spriteMesh != null)
					((SpriteMesh)caret.spriteMesh).material = renderer.sharedMaterial;
			}
			else
			{
				if (manager != null)
				{
					caret.Managed = managed;
					manager.AddSprite(caret);
					caret.SetDrawLayer(drawLayer + 1);	// Caret should be drawn in front of the field graphic
				}
				else
					Debug.LogError("Sprite on object \"" + name + "\" not assigned to a SpriteManager!");
			}
			caret.autoResize = autoResize;
			if (pixelPerfect)
				caret.pixelPerfect = pixelPerfect;
			else
				caret.SetSize(caretSize.x, caretSize.y);

			if (states[1].spriteFrames.Length != 0)
			{
				caret.animations = new UVAnimation[1];
				caret.animations[0] = new UVAnimation();
				caret.animations[0].SetAnim(states[1], 0);
				caret.PlayAnim(0, 0);
			}
			caret.SetCamera(renderCamera);
			caret.Hide(true);
			transitions[1].list[0].MainSubject = caret.gameObject;

			PositionCaret();

			if (container != null)
				container.AddChild(caret.gameObject);
		}

		cachedPos = transform.position;
		cachedRot = transform.rotation;
		cachedScale = transform.lossyScale;
		CalcClippingRect();
	}

	// Calculates the clipping rect for the text
	public void CalcClippingRect()
	{
		if (spriteText == null)
			return;

		Vector3 tl = marginTopLeft;
		Vector3 br = marginBottomRight;

		// Clamp the client rect to any clipping rect we may have:
		if(clipped)
		{
			Vector3 origTL = tl;
			Vector3 origBR = br;
			tl.x = Mathf.Clamp(localClipRect.x, origTL.x, origBR.x);
			br.x = Mathf.Clamp(localClipRect.xMax, origTL.x, origBR.x);
			tl.y = Mathf.Clamp(localClipRect.yMax, origBR.y, origTL.y);
			br.y = Mathf.Clamp(localClipRect.y, origBR.y, origTL.y);
		}

		clientClippingRect.FromRect(Rect.MinMaxRect(tl.x, br.y, br.x, tl.y));
		clientClippingRect.MultFast(transform.localToWorldMatrix);

		spriteText.ClippingRect = clientClippingRect;
		if (caret != null)
			caret.ClippingRect = clientClippingRect;
	}

	/// <summary>
	/// Sets the margins to the specified values.
	/// </summary>
	/// <param name="marg">The distance, in local units, in from the edges of
	/// the control where the text within should be clipped.</param>
	public void SetMargins(Vector2 marg)
	{
		margins = marg;
		Vector3 center = GetCenterPoint();
		marginTopLeft = new Vector3(center.x + margins.x - width * 0.5f, center.y - margins.y + height * 0.5f);
		marginBottomRight = new Vector3(center.x - margins.x + width * 0.5f, center.y + margins.y - height * 0.5f);

		if (multiline)
		{
			float distanceToMargin = 0;

			switch (spriteText.anchor)
			{
				case SpriteText.Anchor_Pos.Upper_Left:
				case SpriteText.Anchor_Pos.Middle_Left:
				case SpriteText.Anchor_Pos.Lower_Left:
					distanceToMargin = marginBottomRight.x - origTextPos.x;
					break;
				case SpriteText.Anchor_Pos.Upper_Center:
				case SpriteText.Anchor_Pos.Middle_Center:
				case SpriteText.Anchor_Pos.Lower_Center:
					distanceToMargin = ((marginBottomRight.x - marginTopLeft.x) * 2f) - 2f * Mathf.Abs(origTextPos.x);
					break;
				case SpriteText.Anchor_Pos.Upper_Right:
				case SpriteText.Anchor_Pos.Middle_Right:
				case SpriteText.Anchor_Pos.Lower_Right:
					distanceToMargin = origTextPos.x - marginTopLeft.x;
					break;
			}

			// Adjust for scale:
			spriteText.maxWidth = (1f / spriteText.transform.localScale.x) * distanceToMargin;
		}
		else
			spriteText.maxWidth = 0;
	}


	// Sets the default UVs:
	public override void InitUVs()
	{
		if (states[0].spriteFrames.Length != 0)
			frameInfo.Copy(states[0].spriteFrames[0]);

		base.InitUVs();
	}


	public override IUIContainer Container
	{
		get
		{
			return base.Container;
		}
		set
		{
			if (value != container)
			{
				if (container != null && caret != null)
					container.RemoveChild(caret.gameObject);

				if (value != null && caret != null)
					value.AddChild(caret.gameObject);
			}

			base.Container = value;
		}
	}


	public override string Text
	{
		get
		{
			return base.Text;
		}
		set
		{
			bool newText = (spriteText == null);

			base.Text = value;

			if (newText && spriteText != null)
			{
				spriteText.transform.localPosition = new Vector4(width * -0.5f + margins.x, height * 0.5f + margins.y);
				spriteText.removeUnsupportedCharacters = true;
				spriteText.parseColorTags = false;
			}

			if (cachedPos != transform.position ||
				cachedRot != transform.rotation ||
				cachedScale != transform.lossyScale)
			{
				cachedPos = transform.position;
				cachedRot = transform.rotation;
				cachedScale = transform.lossyScale;
				CalcClippingRect();
			}
		}
	}


	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <returns>Returns a reference to the component.</returns>
	static public UITextField Create(string name, Vector3 pos)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		return (UITextField)go.AddComponent(typeof(UITextField));
	}

	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <param name="rotation">Rotation of the object.</param>
	/// <returns>Returns a reference to the component.</returns>
	static public UITextField Create(string name, Vector3 pos, Quaternion rotation)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		go.transform.rotation = rotation;
		return (UITextField)go.AddComponent(typeof(UITextField));
	}



	public override void Unclip()
	{
		base.Unclip();
		CalcClippingRect();
	}

	public override Rect3D ClippingRect
	{
		get { return base.ClippingRect; }
		set
		{
			base.ClippingRect = value;
			CalcClippingRect();
		}
	}


	public override bool Clipped
	{
		get { return base.Clipped; }
		set
		{
			base.Clipped = value;
			CalcClippingRect();
		}
	}



	public override void OnDrawGizmosSelected()
	{
 		base.OnDrawGizmosSelected();

		Vector3 ul, ll, lr, ur;

		ul = (transform.position - transform.TransformDirection(Vector3.right * clientClippingRect.width * 0.5f * transform.lossyScale.x) + transform.TransformDirection(Vector3.up * clientClippingRect.height * 0.5f * transform.lossyScale.y));
		ll = (transform.position - transform.TransformDirection(Vector3.right * clientClippingRect.width * 0.5f * transform.lossyScale.x) - transform.TransformDirection(Vector3.up * clientClippingRect.height * 0.5f * transform.lossyScale.y));
		lr = (transform.position + transform.TransformDirection(Vector3.right * clientClippingRect.width * 0.5f * transform.lossyScale.x) - transform.TransformDirection(Vector3.up * clientClippingRect.height * 0.5f * transform.lossyScale.y));
		ur = (transform.position + transform.TransformDirection(Vector3.right * clientClippingRect.width * 0.5f * transform.lossyScale.x) + transform.TransformDirection(Vector3.up * clientClippingRect.height * 0.5f * transform.lossyScale.y));

		Gizmos.color = new Color(1f, 0, 0.5f, 1f);
		Gizmos.DrawLine(ul, ll);	// Left
		Gizmos.DrawLine(ll, lr);	// Bottom
		Gizmos.DrawLine(lr, ur);	// Right
		Gizmos.DrawLine(ur, ul);	// Top
	}


	// Ensures that the object is updated in the scene view
	// while editing:
	public override void OnDrawGizmos()
	{
		// Only run if we're not playing:
		if (Application.isPlaying)
			return;

		// This means Awake() was recently called, meaning
		// we couldn't reliably get valid camera viewport
		// sizes, so we zeroed them out so we'd know to
		// get good values later on (when OnDrawGizmos()
		// is called):
		if (screenSize.x == 0 || screenSize.y == 0)
			Start();

		if (mirror == null)
		{
			mirror = new UITextFieldMirror();
			mirror.Mirror(this);
		}

		mirror.Validate(this);

		// Compare our mirrored settings to the current settings
		// to see if something was changed:
		if (mirror.DidChange(this))
		{
			Init();
			mirror.Mirror(this);	// Update the mirror
		}
	}
}


public class UITextFieldMirror : AutoSpriteControlBaseMirror
{
	public Vector2 margins;
	public bool multiline;

	// Mirrors the specified SpriteText's settings
	public override void Mirror(SpriteRoot s)
	{
		base.Mirror(s);
		UITextField tf = (UITextField)s;
		margins = tf.margins;
		multiline = tf.multiline;
	}

	// Validates certain settings:
	public override bool Validate(SpriteRoot s)
	{
		return base.Validate(s);
	}

	// Returns true if any of the settings do not match:
	public override bool DidChange(SpriteRoot s)
	{
		UITextField tf = (UITextField)s;
		if (margins.x != tf.margins.x ||
			margins.y != tf.margins.y)
		{
			tf.CalcClippingRect();
			margins = tf.margins;
			// Keep it to ourselves since we handled it
			//return true;
		}
		if (multiline != tf.multiline)
			return true;

		return base.DidChange(s);
	}
}