// ======================================================================================
// File         : ItemBulletPackage.cs
// Author       : Wu Jie 
// Last Change  : 01/06/2011 | 22:20:09 PM | Thursday,January
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

///////////////////////////////////////////////////////////////////////////////
// class ItemBulletPackage
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

public class ItemBulletPackage : MonoBehaviour {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public int bullets = 10;
    public float lifeTime = 10.0f;
    public GameObject FX_onTrigger = null;
    public AudioClip snd_pickup = null;

    ///////////////////////////////////////////////////////////////////////////////
    // functions defines
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	IEnumerator Start () {
        // DISABLE { 
        // this.mesh = this.transform.Find("mesh").gameObject;
        // Hashtable args = iTween.Hash( "path", new Vector3[] { 
        //                               this.mesh.transform.position,
        //                               this.mesh.transform.position + new Vector3(0.0f, this.animHeight, 0.0f), 
        //                               this.mesh.transform.position,
        //                               },
        //                               "time", 2.0f,
        //                               "easetype", iTween.EaseType.easeInOutSine, 
        //                               "looptype", iTween.LoopType.loop 
        //                             );
        // iTween.MoveTo ( this.mesh, args );

        // //
        // Hashtable args2 = iTween.Hash( "amount", new Vector3(0.0f, 1.0f, 0.0f),
        //                                "time", 2.0f,
        //                                "easetype", iTween.EaseType.linear, 
        //                                "looptype", iTween.LoopType.loop 
        //                              );
        // iTween.RotateBy ( this.mesh, args2 );
        // } DISABLE end 

        if ( this.lifeTime != -1 ) {
            yield return new WaitForSeconds(this.lifeTime);
            GameObject.Destroy(this.gameObject);
        }
	}
	
    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	void Update () {
	}

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnTriggerEnter ( Collider _other ) {
        // TODO:
        Game.PickupBullets(this.bullets);

        // remove hp package and play trigger effect
        AudioManager.PlayAt( transform.position, snd_pickup );
        GameObject.Destroy(this.gameObject);
        if ( this.FX_onTrigger != null ) {
            GameObject fxOnTrigger = (GameObject)Instantiate( this.FX_onTrigger, this.transform.position + new Vector3(0.0f,0.1f,0.0f), this.transform.rotation );
            fxOnTrigger.particleEmitter.Emit();
        }
    }
}
