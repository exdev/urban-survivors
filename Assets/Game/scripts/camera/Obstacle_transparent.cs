// ======================================================================================
// File         : Obstacle_transparent.cs
// Author       : Wu Jie 
// Last Change  : 11/07/2010 | 14:14:56 PM | Sunday,November
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

[AddComponentMenu("Camera-Control/Transparent Obstacle Object")]
public class Obstacle_transparent: MonoBehaviour {

    Shader alpha_blend;
    Shader orignal;
    List<GameObject> obstacle_list = new List<GameObject>();

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public float transparency = 0.5f;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Awake () {
        alpha_blend = Shader.Find("Transparent/VertexLit");
        orignal = Shader.Find("VertexLit"); // TODO: you can't pre-define orignal shader.
    }
 
    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public int CompareHits( RaycastHit _x, RaycastHit _y )
    {
        return _x.distance.CompareTo(_y.distance);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Update () {
        GameObject girl = Game.GetPlayerGirl().gameObject;
        Vector3 dir = girl.transform.position - transform.position;

        //
        RaycastHit[] hits = Physics.RaycastAll(transform.position, dir);
        if ( hits.Length < 2 )
            return;

        // sort the list by distance
        System.Array.Sort(hits,CompareHits);

        // get building before those obstacle player
        bool is_obstacle = false;
        List<GameObject> hitGOs = new List<GameObject>();
        foreach ( RaycastHit hit in hits ) {
            GameObject go = hit.transform.gameObject;
            if ( go.layer == Layer.player ) {
                is_obstacle = true;
                break;
            }

            if ( go.layer == Layer.building )
                hitGOs.Add(go);
        }

        //
        if ( is_obstacle == false )
            return;

        //
        foreach ( GameObject go in hitGOs ) {
            for ( int i = 0; i < obstacle_list.Count; ++i ) {
                if ( obstacle_list[i] == go ) {
                    obstacle_list.RemoveAt(i);
                    break;
                }
            }

            Renderer[] r_list = go.GetComponentsInChildren<Renderer>();
            foreach ( Renderer r in r_list ) {
                if ( r.materials.Length != 0 ) {
                    foreach ( Material m in r.materials ) {
                        if ( m.shader != alpha_blend ) {
                            m.shader = alpha_blend;
                            m.color = new Color ( m.color.r, m.color.g, m.color.b, transparency );
                        }
                    }
                }
            }
        }

        //
        foreach ( GameObject go in obstacle_list ) {
            Renderer[] r_list = go.GetComponentsInChildren<Renderer>();
            foreach ( Renderer r in r_list ) {
                if ( r.materials.Length != 0 ) {
                    foreach ( Material m in r.materials ) {
                        m.shader = orignal;
                    }
                }
            }
        }
        obstacle_list = hitGOs;
    }
}
