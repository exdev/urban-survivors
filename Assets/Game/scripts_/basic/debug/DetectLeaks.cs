// ======================================================================================
// File         : DetectLeaks.cs
// Author       : 
// Last Change  : 08/09/2010 | 22:15:25 PM | Monday,August
// Description  : 
// ======================================================================================

// Usage: add to an object in your game, choose show debug info to true and check the result.

///////////////////////////////////////////////////////////////////////////////
// using
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////
// defines
///////////////////////////////////////////////////////////////////////////////

public class DetectLeaks : MonoBehaviour {

    public bool showDebugInfo = false; 

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnGUI () {
        if ( showDebugInfo ) {
            GUILayout.Label("All " + Resources.FindObjectsOfTypeAll(typeof(UnityEngine.Object)).Length);
            GUILayout.Label("Textures " + Resources.FindObjectsOfTypeAll(typeof(Texture)).Length);
            GUILayout.Label("AudioClips " + Resources.FindObjectsOfTypeAll(typeof(AudioClip)).Length);
            GUILayout.Label("Meshes " + Resources.FindObjectsOfTypeAll(typeof(Mesh)).Length);
            GUILayout.Label("Materials " + Resources.FindObjectsOfTypeAll(typeof(Material)).Length);
            GUILayout.Label("GameObjects " + Resources.FindObjectsOfTypeAll(typeof(GameObject)).Length);
            GUILayout.Label("Components " + Resources.FindObjectsOfTypeAll(typeof(Component)).Length);
        }
    }
}
