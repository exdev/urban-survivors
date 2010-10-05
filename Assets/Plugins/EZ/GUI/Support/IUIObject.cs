//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------



using UnityEngine;
using System.Collections;


/// <remarks>
/// Definition of a delegate type that is used as a pattern for methods that can be called when input is received.
/// </remarks>
/// <param name="ptr">Reference to the POINTER_INFO object that contains informaiton about the input event.</param>
public delegate void EZInputDelegate(ref POINTER_INFO ptr);

/// <remarks>
/// Definition of a delegate type that is used as a pattern for methods that can be called when the value of a control is changed.
/// </remarks>
/// <param name="obj">Reference to the control whose value changed.</param>
public delegate void EZValueChangedDelegate(IUIObject obj);

/// <remarks>
/// A generic interface for a UI element.
/// </remarks>
public interface IUIObject
{
	/// <summary>
	/// Controls whether this control is in an enabled
	/// state or not. If it is not, input is not processed.
	/// This can also be used to cause a control to take on
	/// a "grayed out" appearance when disabled.
	/// </summary>
	bool controlIsEnabled
	{
		get;
		set;
	}

	// Accessor for getting/setting a reference to the
	// object that contains this one.
	IUIContainer Container
	{
		get;
		set;
	}

	// Requests that the IUIObject accept the specified
	// container as its container.  It should search
	// up the hierarchy of parent objects first to see
	// if a more immediate parent is a container, and
	// if so, reject the request.
	// <param name="cont">A reference to the object that is requesting containership.</param>
	// <returns>True if succeeded, false if a closer container was found.</returns>
	bool RequestContainership(IUIContainer cont);

	// Is called when a control receives the keyboard focus.
	// <returns>The object should return true if it can process 
	// keyboard input (will result on displaying an on-screen 
	// keyboard on iPhone OS devices), or false if it cannot.</returns>
	bool GotFocus();

	// Is called to inform a control that it has lost the
	// keyboard focus.
	void LostFocus();

	// Gets the input text of the control (if any)
	// and returns the insertion point in the
	// reference variable "insert".
	// <param name="info">Will contain information about how the keyboard should be displayed (if iPhone) as well as the index of the insertion point.</param>
	// <returns>Returns the input text of the control.</returns>
	string GetInputText(ref KEYBOARD_INFO info);

	// Sets the input text of the control as well as
	// the insertion point.
	// <param name="text">The input text of the control.</param>
	// <param name="insert">The index of the insertion point.</param>
	// <returns>Returns the text accepted which may be different from the text sent in the "text" argument.</returns>
	string SetInputText(string text, ref int insert);

	/// <summary>
	/// This is where input handling code should go in any derived class.
	/// </summary>
	/// <param name="ptr">POINTER_INFO struct that contains information on the pointer that caused the event, as well as the event that occurred.</param>
	void OnInput(POINTER_INFO ptr);

	/// <summary>
	/// Register a method to be called when input occurs (input is forwarded from OnInput()).
	/// NOTE: It is recommended to save the return value of SetInputDelegate() and call it,
	/// if not null, when your delegate gets called so as to preserve the delegate chain.
	/// </summary>
	/// <param name="del">A method that conforms to the EZInputDelegate pattern.</param>
	/// <returns>Returns a reference to the previously registered delegate.</returns>
	EZInputDelegate SetInputDelegate(EZInputDelegate del);

	/// <summary>
	/// Register a method to be called when the value of a control changes (such as a checkbox changing from false to true, or a slider being moved).
	/// NOTE: It is recommended to save the return value of SetValueChangedDelegate() and call it,
	/// if not null, when your delegate gets called so as to preserve the delegate chain.
	/// </summary>
	/// <param name="del">A method that conforms to the EZValueChangedDelegate pattern.</param>
	/// <returns>Returns a reference to the previously registered delegate.</returns>
	EZValueChangedDelegate SetValueChangedDelegate(EZValueChangedDelegate del);
}
