// ======================================================================================
// File         : CollisionIgnoreManager.cs
// Author       : Wu Jie 
// Last Change  : 09/01/2010 | 23:10:20 PM | Wednesday,September
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

// Anything provided to this manager will have its collisions with anything else registered

public class CollisionIgnoreManager : MonoBehaviour {

    public static CollisionIgnoreManager collisionIgnoreManager = null;

    ArrayList ignoreObjects = new ArrayList();
    ArrayList ignoreMasks = new ArrayList();

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public static CollisionIgnoreManager Instance() {
        return collisionIgnoreManager;
    }

    // ------------------------------------------------------------------ 
    // Desc: Use this for initialization
    // ------------------------------------------------------------------ 

    void Awake () {
        if( collisionIgnoreManager == null )
            collisionIgnoreManager = this;
    }
    
    // ------------------------------------------------------------------ 
    // Desc: Update is called once per frame
    // ------------------------------------------------------------------ 

    void Update () {
        // clean up any dead objects
        for( int i = ignoreObjects.Count - 1; i >= 0; i-- ) {
            Collider collider = ignoreObjects[i] as Collider;
            if( collider == null || collider.gameObject.active == false ) {
                ignoreObjects.RemoveAt( i );
                ignoreMasks.RemoveAt( i );
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void AddIgnore( Collider newCollider ) {
        AddIgnore( newCollider, 0xffff, 0xffff );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void AddIgnore( Collider newCollider, int thisMask, int mask ) {
        for( int i = 0; i < ignoreObjects.Count; i++ ) {
            Collider collider = ignoreObjects[i] as Collider;

            if( collider == null || collider.gameObject.active == false )
                continue;

            if( ( mask & ( (int) ignoreMasks[ i ]) ) != 0 )
                Physics.IgnoreCollision( newCollider, collider, true );
        }

        ignoreObjects.Add( newCollider );
        ignoreMasks.Add( thisMask );
    }
}
