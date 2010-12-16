var animStrings : String[]; // The different names for the animations

var animTarget : Animation; // The object's animation component

var blendSpeed : float = 1; // The time in seconds to fully blend over
var blendWeight : float = .5; // The power of the blend. 1 is fully blended, .5 is half blended, etc...
var blendWeight2 : float = .5; // The power of the blend for the second animation menu
var curAnim : int = 0; // The primary animation selected
var curAnim2 : int = 1; // The secondary animation selected

var textStyle : GUIStyle; // The white text holder

function OnGUI() // This function handles all GUI and is run multiple times per frame
{
	blendSpeed = GUI.HorizontalSlider(Rect(Screen.width * .5 - 100, Screen.height * .05, 200, 20), blendSpeed, .25, 5); // Set the blend speed based on the horizontal slider
	GUI.Label(Rect(Screen.width * .5 - 100, Screen.height * .05 - 25, 200, 20), "Blend Speed: " + (Mathf.Round(blendSpeed * 100) / 100).ToString() + " second(s)", textStyle); // Display the current blend speed
	
	GUI.Label(Rect(Screen.width * .5 - 100, Screen.height * .05 + 25, 200, 20), "Wrap Mode", textStyle); // The function call for the text "Wrap Mode"
	if (GUI.Button(Rect(Screen.width * .5 - 60, Screen.height * .05 + 50, 120, 20), "Once")) { // If the user wants the animations to play once
		animTarget.wrapMode = WrapMode.Once; // Set them to do so
	}
	if (GUI.Button(Rect(Screen.width * .5 - 60, Screen.height * .05 + 80, 120, 20), "Loop")) { // If the user wants the animations to loop forever
		animTarget.wrapMode = WrapMode.Loop; // Set them to do so
	}
	if (GUI.Button(Rect(Screen.width * .5 - 60, Screen.height * .05 + 110, 120, 20), "PingPong")) { // If the user wants the animations to go forward and backward forever
		animTarget.wrapMode = WrapMode.PingPong; // Set them to do so
	}
	if (GUI.Button(Rect(Screen.width * .5 - 60, Screen.height * .05 + 140, 120, 20), "ClampForever")) { // If the user wants the animations to freeze on the last frame
		animTarget.wrapMode = WrapMode.ClampForever; // Set them to do so
	}
	
	blendWeight = GUI.HorizontalSlider(Rect(Screen.width * .25 - 150, Screen.height * .05 + 300, 200, 20), blendWeight, .01, 1); // The function call for setting the first blend weight
	GUI.Label(Rect(Screen.width * .25 - 150, Screen.height * .05 + 275, 200, 20), "Blend Weight: " + (Mathf.Round(blendWeight * 100) / 100).ToString(), textStyle); // The function call for displaying the first blend weight
	GUI.Label(Rect(0, Screen.height * .05 + 50, Screen.width * .5 - 100, 20), "Animation: " + animStrings[curAnim], textStyle); // The first animation's string
	if (GUI.Button(Rect(Screen.width * .25 - 140, Screen.height * .05 + 75, 80, 20), "Next")) { // If the user wants to move one animation forward
		curAnim += 1; // Increase the current position by one
		if (curAnim >= animStrings.length) { // If it falls outside the animation array
			curAnim = 0; // Reset it to the first slot
		}
	}
	if (GUI.Button(Rect(Screen.width * .25 - 40, Screen.height * .05 + 75, 80, 20), "Last")) { // If the user wants to move one animation backward
		curAnim -= 1; // Decrease the current position by one
		if (curAnim < 0) { // If it falls outside the animation array
			curAnim = animStrings.length - 1; // Reset it to the last slot
		}
	}
	if (GUI.Button(Rect(Screen.width * .25 - 120, Screen.height * .05 + 115, 140, 20), "Play Animation")) { // If the user presses the Play button
		animTarget.Play(animStrings[curAnim]); // Play the current animation
	}
	if (GUI.Button(Rect(Screen.width * .25 - 120, Screen.height * .05 + 155, 140, 20), "Stop Animation")) { // If the user presses the Stop button
		animTarget.Stop(animStrings[curAnim]); // Stop the current animation
	}
	if (GUI.Button(Rect(Screen.width * .25 - 120, Screen.height * .05 + 195, 140, 20), "Crossfade Animation")) { // If the user presses the CrossFade button
		animTarget.CrossFade(animStrings[curAnim], blendSpeed); // CrossFade the current animation for blendSpeed in seconds
	}
	if (GUI.Button(Rect(Screen.width * .25 - 120, Screen.height * .05 + 235, 140, 20), "Blend Animation")) { // If the user presses the Blend button
		animTarget.Blend(animStrings[curAnim], blendWeight, blendSpeed); // Blend the current animation towards blendWeight for blendSpeed in seconds
	}
	
	blendWeight2 = GUI.HorizontalSlider(Rect(Screen.width * .75 - 50, Screen.height * .05 + 300, 200, 20), blendWeight2, .01, 1); // Set the second blend weight with the horizontal slider
	GUI.Label(Rect(Screen.width * .75 - 50, Screen.height * .05 + 275, 200, 20), "Blend Weight: " + (Mathf.Round(blendWeight2 * 100) / 100).ToString(), textStyle); // Display the second blend weight setting
	GUI.Label(Rect(Screen.width * .5 + 100, Screen.height * .05 + 50, Screen.width * .5 - 100, 20), "Animation: " + animStrings[curAnim2], textStyle); // Display the secondary animation's name
	if (GUI.Button(Rect(Screen.width * .75 - 40, Screen.height * .05 + 75, 80, 20), "Next")) { // If the user wants to move one animation forward
		curAnim2 += 1; // Increase the current position by one
		if (curAnim2 >= animStrings.length) { // If it falls outside the animation array
			curAnim2 = 0; // Reset it to the first slot
		}
	}
	if (GUI.Button(Rect(Screen.width * .75 + 60, Screen.height * .05 + 75, 80, 20), "Last")) { // If the user wants to move one animation backward
		curAnim2 -= 1; // Decrease the current position by one
		if (curAnim2 < 0) { // If it falls outside the animation array
			curAnim2 = animStrings.length - 1; // Reset it to the last slot
		}
	}
	if (GUI.Button(Rect(Screen.width * .75 - 20, Screen.height * .05 + 115, 140, 20), "Play Animation")) { // If the user presses the Play button
		animTarget.Play(animStrings[curAnim2]); // Play the current animation
	}
	if (GUI.Button(Rect(Screen.width * .75 - 20, Screen.height * .05 + 155, 140, 20), "Stop Animation")) { // If the user presses the Stop button
		animTarget.Stop(animStrings[curAnim2]); // Stop the current animation
	}
	if (GUI.Button(Rect(Screen.width * .75 - 20, Screen.height * .05 + 195, 140, 20), "Crossfade Animation")) { // If the user presses the CrossFade button
		animTarget.CrossFade(animStrings[curAnim2], blendSpeed); // CrossFade the current animation for blendSpeed in seconds
	}
	if (GUI.Button(Rect(Screen.width * .75 - 20, Screen.height * .05 + 235, 140, 20), "Blend Animation")) { // If the user presses the Blend button
		animTarget.Blend(animStrings[curAnim2], blendWeight2, blendSpeed); // Blend the current animation towards blendWeight for blendSpeed in seconds
	}
}