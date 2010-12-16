// ======================================================================================
// File         : BulletInfo.cs
// Author       : Wu Jie 
// Last Change  : 12/13/2010 | 16:28:07 PM | Monday,December
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

public class BulletInfo : MonoBehaviour {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public float speed = 20.0f;
    public float lifeTime = 1.0f;
    public DamageInfo ownerDamageInfo = null; // NOTE: if we don't use public, Instantiate will not copy this. 

    protected float counter = 0.0f;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

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

    void OnTriggerEnter ( Collider _other ) {
        // TODO: play dead particle ??? { 
        // // play particles
        // if ( parBulletHit_inst != null ) {
        //     parBulletHit_inst.transform.position = transform.position;
        //     parBulletHit_inst.transform.rotation = transform.rotation;
        //     parBulletHit_inst.particleEmitter.Emit();
        // }
        // else {
        //     Debug.Log( "warning: the particle instance not instantiate!" );
        // }
        // } TODO end 

        // TODO: bullet type, hit what? should get through or not. { 
        // destroy bullet
        // DISABLE: SpawnManager.Instance().Destroy(gameObject);
        GameObject.Destroy(gameObject);
        // } TODO end 
    }
}
