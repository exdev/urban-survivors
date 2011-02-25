// ======================================================================================
// File         : AudioManager.cs
// Author       : Wu Jie 
// Last Change  : 02/25/2011 | 21:57:09 PM | Friday,February
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

///////////////////////////////////////////////////////////////////////////////
// defines
///////////////////////////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////////////
// class AudioManager
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

public class AudioManager : MonoBehaviour {

    protected static AudioManager instance = null;
    protected GameObject[] audioPlayers;

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public int numAudioPlayers = 16;

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Awake () {
        if( instance == null ) {
            instance = this;

            audioPlayers = new GameObject[numAudioPlayers];
            for ( int i = 0; i < numAudioPlayers; ++i ) {
                GameObject go = new GameObject();
                AudioSource au = go.AddComponent<AudioSource>();
                au.loop = false;
                au.playOnAwake = false;
                audioPlayers[i] = go;
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public static void PlayAt ( AudioClip _clip, Vector3 _worldPos ) {
        bool played = false;
        foreach ( GameObject go in instance.audioPlayers ) {
            if ( go.audio.isPlaying == false ) {
                go.transform.position = _worldPos;
                go.audio.clip = _clip; 
                go.audio.Play();
                played = true;
                break;
            } 
        }

        //
        if ( played == false ) {
            Debug.LogError("no enough player to play!!!");
        }
    }
}

