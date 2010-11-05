// ======================================================================================
// File         : DrawGizmos.cs
// Author       : Wu Jie 
// Last Change  : 10/30/2010 | 11:43:39 AM | Saturday,October
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEditor;
using UnityEngine;

///////////////////////////////////////////////////////////////////////////////
// 
///////////////////////////////////////////////////////////////////////////////

class DrawGizmos {

    // ------------------------------------------------------------------ 
    // Desc: periodic trigger 
    // ------------------------------------------------------------------ 

    [DrawGizmo (GizmoType.NotSelected | GizmoType.Pickable)]
    static void DrawPeriodicSource ( Source_periodic _triggerSource, GizmoType _gizmoType ) {
        Vector3 position = _triggerSource.transform.position;
        // Draw the icon (A bit above the one drawn)
        Gizmos.DrawIcon (position, "Napster.ico");

        // KEEPME { 
        // // Are we selected? Draw a solid sphere surrounding the light
        // if ( (_gizmoType & GizmoType.SelectedOrChild) != 0 ) {
        //     // Indicate that this is the active object by using a brighter color.
        //     if ( (_gizmoType & GizmoType.Active) != 0 )
        //         Gizmos.color = Color.red;
        //     else
        //         Gizmos.color = Color.red * 0.5F;
        //     Gizmos.DrawSphere (position, 10.0f);
        // }
        // } KEEPME end 
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [DrawGizmo (GizmoType.NotSelected | GizmoType.Pickable)]
    static void DrawPeriodicSource ( Source_collider _triggerSource, GizmoType _gizmoType ) {
        Vector3 position = _triggerSource.transform.position;
        // Draw the icon (A bit above the one drawn)
        Gizmos.DrawIcon (position, "Desktop.ico");
    }
}

// KEEPME { 
/*
// Draw the gizmo if it is selected or a child of the selection.
// This is the most common way to render a gizmo
[DrawGizmo (GizmoType.SelectedOrChild)]

// Draw the gizmo only if it is the active object.
[DrawGizmo (GizmoType.Active)]
*/
// } KEEPME end 
