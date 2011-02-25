// ======================================================================================
// File         : ItemHealthPackage.cs
// Author       : Wu Jie 
// Last Change  : 12/20/2010 | 11:54:34 AM | Monday,December
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
// class ItemHealthPackage
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

public class ItemHealthPackage : MonoBehaviour {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public float hpAmount = 10.0f; 
    public float animHeight = 0.2f;
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
        // 
        PlayerInfo girlInfo = Game.PlayerGirl().playerInfo;
        float hpLoseGirl = girlInfo.maxHP - girlInfo.curHP;
        float hpLeft = this.hpAmount - hpLoseGirl;

        // recover girl first
        if ( hpLoseGirl > 0.0f ) {
            Game.PlayerGirl().SendMessage("OnRecover", this.hpAmount);
            hpLeft = this.hpAmount - hpLoseGirl;
        }
        // poor brother, ony get hp if sister left some...
        if ( hpLeft > 0.0f ) {
            Game.PlayerBoy().SendMessage("OnRecover", hpLeft);
        }

        // remove hp package and play trigger effect
        AudioManager.PlayAt( snd_pickup, transform.position );
        GameObject.Destroy ( this.gameObject );
        if ( this.FX_onTrigger != null ) {
            GameObject fxOnTrigger = (GameObject)Instantiate( this.FX_onTrigger, this.transform.position + new Vector3(0.0f,0.1f,0.0f), this.transform.rotation );
            fxOnTrigger.particleEmitter.Emit();
        }
    }
}
