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
    static void DrawPeriodicSource ( Source_periodic _c, GizmoType _gizmoType ) {
        if ( EditorApplication.isPlaying ) return;

        Vector3 position = _c.transform.position;
        Gizmos.DrawIcon (position, "Adobe.ico");
    }

    // ------------------------------------------------------------------ 
    // Desc: Source_collider
    // ------------------------------------------------------------------ 

    [DrawGizmo (GizmoType.NotSelected | GizmoType.Pickable)]
    static void DrawPeriodicSource ( Source_collider _c, GizmoType _gizmoType ) {
        if ( EditorApplication.isPlaying ) return;

        Vector3 position = _c.transform.position;
        Gizmos.DrawIcon (position, "Desktop.ico");

        // draw the collider
        Collider co = _c.gameObject.collider;
        if ( co.GetType() == typeof(BoxCollider) ) {
            BoxCollider box = co as BoxCollider;
            Gizmos.color = new Color( 1.0f, 0.0f, 0.0f, 0.2f );
            Gizmos.DrawCube (box.center + position, box.size);
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube (box.center + position, box.size);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: Spawner_point
    // ------------------------------------------------------------------ 

    [DrawGizmo (GizmoType.NotSelected | GizmoType.Pickable)]
    static void DrawPeriodicSource ( Spawner_point _c, GizmoType _gizmoType ) {
        if ( EditorApplication.isPlaying ) return;

        Vector3 position = _c.transform.position;
        Gizmos.DrawIcon (position, "Blender.ico");
    }

    // ------------------------------------------------------------------ 
    // Desc: Response_listSpawn
    // ------------------------------------------------------------------ 

    [DrawGizmo (GizmoType.NotSelected | GizmoType.Pickable)]
    static void DrawPeriodicSource ( Response_listSpawn _c, GizmoType _gizmoType ) {
        if ( EditorApplication.isPlaying ) return;

        Gizmos.color = Color.yellow;
        foreach ( GameObject go in _c.Spawners ) {
            if ( go == null )
                continue;
            Vector3 position = _c.transform.position;
            Gizmos.DrawLine (position, go.transform.position);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: StartPoint
    // ------------------------------------------------------------------ 

    [DrawGizmo (GizmoType.NotSelected | GizmoType.Pickable)]
    static void DrawPeriodicSource ( StartPoint _c, GizmoType _gizmoType ) {
        if ( EditorApplication.isPlaying ) return;

        Vector3 position = _c.transform.position;
        Gizmos.DrawIcon (position, "Napster.ico");
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
