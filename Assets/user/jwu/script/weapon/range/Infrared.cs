// ======================================================================================
// File         : Infrared.cs
// Author       : Wu Jie 
// Last Change  : 09/27/2010 | 21:53:05 PM | Monday,September
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////
// defines
///////////////////////////////////////////////////////////////////////////////

public class Infrared : MonoBehaviour {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public Material material;
    public float lineSize = 0.2f;
    public Color lineColor;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Start () {
        LineRenderer lr = GetComponent(typeof(LineRenderer)) as LineRenderer;
        if ( lr == null ) {
            lr = gameObject.AddComponent(typeof(LineRenderer)) as LineRenderer;
            lr.useWorldSpace = false;
            lr.SetWidth(lineSize, lineSize);
            lr.material = material;
            lr.SetPosition(0, Vector3.zero );
            lr.SetPosition(1, Vector3.forward * 10.0f );
            lr.SetColors(lineColor,lineColor);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Update () {
        LineRenderer lr = GetComponent(typeof(LineRenderer)) as LineRenderer;
        if ( lr ) {
            // This would cast rays only against colliders in layer x.
            // ignore layer: bullet_player, player, trigger
            int layerMask = 1 << Layer.bullet_player 
                | 1 << Layer.player 
                | 1 << Layer.trigger;

            // But instead we want to collide against everything except layer x. 
            // The ~ operator does this, it inverts a bitmask.
            layerMask = ~layerMask;

            float dist = 100.0f;
            RaycastHit hit;
            Vector3 fwd = transform.TransformDirection (Vector3.forward);
            if ( Physics.Raycast (transform.position, fwd, out hit, 100.0f, layerMask ) ) {
                dist = hit.distance;
            }
            lr.SetPosition( 1, Vector3.forward * dist );
        }
    }
}
