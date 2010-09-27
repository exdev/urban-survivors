// ======================================================================================
// File         : Bullet.cs
// Author       : Wu Jie 
// Last Change  : 09/27/2010 | 21:50:25 PM | Monday,September
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

public class Bullet : MonoBehaviour {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    static GameObject parBulletHit_inst = null;

    public float speed = 20.0f;
    public float lifeTime = 1.0f;
    public GameObject parBulletHit = null;

    private float counter = 0.0f;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Awake () {
        if ( parBulletHit_inst == null ) {
            parBulletHit_inst = (GameObject)Instantiate(parBulletHit, transform.position, transform.rotation );
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Start () {
        counter = 0.0f;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Update () {
        counter += Time.deltaTime;
        if ( counter >= lifeTime ) {
            SpawnManager.Instance().Destroy(gameObject);
        }
        transform.position = transform.position + speed * Time.deltaTime * transform.forward;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnCollisionEnter ( Collision _other ) {
        // DEBUG { 
        // Debug.Log("touch: " + _other.gameObject.name );
        // } DEBUG end 

        // play particles
        if ( parBulletHit_inst != null ) {
            parBulletHit_inst.transform.position = transform.position;
            parBulletHit_inst.transform.rotation = transform.rotation;
            parBulletHit_inst.particleEmitter.Emit();
        }
        else {
            Debug.Log( "warning: the particle instance not instantiate!" );
        }

        // HARDCODE { 
        // it is possible the collider already destroied 
        if ( _other.collider != null ) {
            AI_generic ai = _other.collider.GetComponent( typeof(AI_generic) ) as AI_generic;
            if ( ai != null ) {
                ai.HP -= 10.0f;
            }
        }
        // } HARDCODE end 

        // destroy bullet
        SpawnManager.Instance().Destroy(gameObject);
    }

}
