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
    public Transform anchor = null;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Start () {
        DebugHelper.Assert( anchor, "weapon's anchor not set" );
        LineRenderer lr = this.anchor.gameObject.GetComponent<LineRenderer>();
        if ( lr == null ) {
            lr = this.anchor.gameObject.AddComponent<LineRenderer>();
            lr.useWorldSpace = true;
            lr.SetWidth(lineSize, lineSize);
            lr.material = material;
            lr.SetPosition(0, Vector3.zero );
            lr.SetPosition(1, Vector3.forward * 0.0f );
            lr.SetColors(lineColor,lineColor);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Update () {
        LineRenderer lr = this.anchor.gameObject.GetComponent<LineRenderer>();
        if ( lr ) {
            // This would cast rays only against colliders in layer x.
            // ignore layer: bullet_player, player, trigger
            int layerMask = 1 << Layer.bullet_player 
                | 1 << Layer.melee_player
                | 1 << Layer.player 
                | 1 << Layer.trigger;

            // But instead we want to collide against everything except layer x. 
            // The ~ operator does this, it inverts a bitmask.
            layerMask = ~layerMask;

            float dist = 1000.0f;
            RaycastHit hit;
            Vector3 fwd = this.anchor.forward;
            fwd.y = 0.0f;
            fwd.Normalize();
            if ( Physics.Raycast ( this.anchor.position, fwd, out hit, dist, layerMask ) ) {
                dist = hit.distance;
            }
            lr.SetPosition( 0, this.anchor.position );
            lr.SetPosition( 1, this.anchor.position + fwd * dist );
        }
    }
}
