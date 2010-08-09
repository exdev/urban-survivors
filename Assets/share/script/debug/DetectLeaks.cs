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
            GUILayout.Label("All " + FindObjectsOfTypeAll(typeof(UnityEngine.Object)).Length);
            GUILayout.Label("Textures " + FindObjectsOfTypeAll(typeof(Texture)).Length);
            GUILayout.Label("AudioClips " + FindObjectsOfTypeAll(typeof(AudioClip)).Length);
            GUILayout.Label("Meshes " + FindObjectsOfTypeAll(typeof(Mesh)).Length);
            GUILayout.Label("Materials " + FindObjectsOfTypeAll(typeof(Material)).Length);
            GUILayout.Label("GameObjects " + FindObjectsOfTypeAll(typeof(GameObject)).Length);
            GUILayout.Label("Components " + FindObjectsOfTypeAll(typeof(Component)).Length);
        }
    }
}
