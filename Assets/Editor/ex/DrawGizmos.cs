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

    // [DrawGizmo (GizmoType.SelectedOrChild | GizmoType.Pickable)]
    [DrawGizmo (GizmoType.NotSelected | GizmoType.Pickable)]
    static void Draw ( Source_periodic _c, GizmoType _gizmoType ) {
        if ( EditorApplication.isPlaying ) return;

        Vector3 position = _c.transform.position;
        Gizmos.DrawIcon (position, "Desktop.ico");
    }

    // ------------------------------------------------------------------ 
    // Desc: Source_collider
    // ------------------------------------------------------------------ 

    // [DrawGizmo (GizmoType.SelectedOrChild | GizmoType.Pickable)]
    [DrawGizmo (GizmoType.NotSelected | GizmoType.Pickable)]
    static void Draw ( Source_collider _c, GizmoType _gizmoType ) {
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
    // Desc: 
    // ------------------------------------------------------------------ 

    [DrawGizmo (GizmoType.SelectedOrChild | GizmoType.Pickable)]
    // [DrawGizmo (GizmoType.NotSelected | GizmoType.Pickable)]
    static void Draw ( Spawner_base _c, GizmoType _gizmoType ) {
        if ( _c.showSpawns ) {
            foreach ( Object obj in _c.existObjects ) {
                if ( obj == null )
                    continue;
                Vector3 position = _c.transform.position;
                Gizmos.color = new Color( 0.0f, 0.0f, 1.0f, 1.0f );
                Gizmos.DrawLine (position, (obj as GameObject).transform.position);
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: Spawner_point
    // ------------------------------------------------------------------ 

    // [DrawGizmo (GizmoType.SelectedOrChild | GizmoType.Pickable)]
    [DrawGizmo (GizmoType.NotSelected | GizmoType.Pickable)]
    static void Draw ( Spawner_point _c, GizmoType _gizmoType ) {
        if ( EditorApplication.isPlaying ) return;

        Vector3 position = _c.transform.position;
        Gizmos.DrawIcon (position, "Blender.ico");
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    // [DrawGizmo (GizmoType.SelectedOrChild | GizmoType.Pickable)]
    [DrawGizmo (GizmoType.NotSelected | GizmoType.Pickable)]
    static void Draw ( Spawner_zone _c, GizmoType _gizmoType ) {
        if ( EditorApplication.isPlaying ) return;

        Vector3 position = _c.transform.position;
        Gizmos.color = new Color( 0.0f, 0.5f, 0.0f, 0.2f );
        Gizmos.DrawCube (position, _c.size);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube (position, _c.size);
    }

    // ------------------------------------------------------------------ 
    // Desc: Response_listSpawn
    // ------------------------------------------------------------------ 

    [DrawGizmo (GizmoType.SelectedOrChild | GizmoType.Pickable)]
    // [DrawGizmo (GizmoType.NotSelected | GizmoType.Pickable)]
    static void Draw ( Response_listSpawn _c, GizmoType _gizmoType ) {
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
    // Desc: Response_listDespawn
    // ------------------------------------------------------------------ 

    [DrawGizmo (GizmoType.SelectedOrChild | GizmoType.Pickable)]
    // [DrawGizmo (GizmoType.NotSelected | GizmoType.Pickable)]
    static void Draw ( Response_listDespawn _c, GizmoType _gizmoType ) {
        if ( EditorApplication.isPlaying ) return;

        Gizmos.color = Color.blue;
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

    // [DrawGizmo (GizmoType.SelectedOrChild | GizmoType.Pickable)]
    [DrawGizmo (GizmoType.NotSelected | GizmoType.Pickable)]
    static void Draw ( StartPoint _c, GizmoType _gizmoType ) {
        if ( EditorApplication.isPlaying ) return;

        Vector3 position = _c.transform.position;
        Gizmos.DrawIcon (position, "Foobar.ico");
    }

    // ------------------------------------------------------------------ 
    // Desc: StartPoint
    // ------------------------------------------------------------------ 

    [DrawGizmo (GizmoType.SelectedOrChild | GizmoType.Pickable)]
    static void Draw ( LevelUp _c, GizmoType _gizmoType ) {
        if ( EditorApplication.isPlaying ) return;

        Gizmos.color = Color.green;
        foreach ( GameObject go in _c.targets ) {
            if ( go == null )
                continue;
            Vector3 position = _c.transform.position;
            Gizmos.DrawLine (position, go.transform.position);
        }
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
