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
    protected GameObject mesh = null;

    ///////////////////////////////////////////////////////////////////////////////
    // functions defines
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	IEnumerator Start () {
        this.mesh = this.transform.Find("mesh").gameObject;

        //
        Hashtable args = iTween.Hash( "path", new Vector3[] { 
                                      this.mesh.transform.position,
                                      this.mesh.transform.position + new Vector3(0.0f, this.animHeight, 0.0f), 
                                      this.mesh.transform.position,
                                      },
                                      "time", 2.0f,
                                      "easetype", iTween.EaseType.easeInOutSine, 
                                      "looptype", iTween.LoopType.loop 
                                    );
        iTween.MoveTo ( this.mesh, args );

        //
        Hashtable args2 = iTween.Hash( "amount", new Vector3(0.0f, 1.0f, 0.0f),
                                       "time", 2.0f,
                                       "easetype", iTween.EaseType.linear, 
                                       "looptype", iTween.LoopType.loop 
                                     );
        iTween.RotateBy ( this.mesh, args2 );

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
        Player_base boy = GameRules.Instance().GetPlayerBoy().GetComponent<Player_base>();
        PlayerInfo boyInfo = boy.playerInfo;
        Player_base girl = GameRules.Instance().GetPlayerGirl().GetComponent<Player_base>();
        PlayerInfo girlInfo = girl.playerInfo;

        float hpLoseBoy = boyInfo.maxHP - boyInfo.curHP;
        float hpLoseGirl = girlInfo.maxHP - girlInfo.curHP;

        if ( girl.IsDown() || hpLoseGirl > hpLoseBoy ) {
            girl.Recover(this.hpAmount);
            float hpLeft = hpLoseGirl - this.hpAmount;
            if ( hpLeft > 0.0f )
                boy.Recover(hpLeft);
        }
        else {
            boy.Recover(this.hpAmount);
            float hpLeft = hpLoseBoy - this.hpAmount;
            if ( hpLeft > 0.0f )
                girl.Recover(hpLeft);
        }

        // remove hp package and play trigger effect
        GameObject.Destroy(this.gameObject);
        if ( this.FX_onTrigger != null ) {
            GameObject fxOnTrigger = (GameObject)Instantiate( this.FX_onTrigger, this.transform.position + new Vector3(0.0f,0.1f,0.0f), this.transform.rotation );
            fxOnTrigger.particleEmitter.Emit();
        }
    }
}
