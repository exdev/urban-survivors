// ======================================================================================
// File         : Emitter_S.cs
// Author       : Wu Jie 
// Last Change  : 09/27/2010 | 21:47:49 PM | Monday,September
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

public class Emitter_S : Emitter {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public float angle = 30.0f;
    public int max_bullet = 5;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public override int Emit ( Transform _anchor, GameObject _bullet, bool _activeReload ) {
        // create a bullet, and rotate it based on the vector inputRotation
        float half_ang = this.angle * 0.5f;

        for ( int i = 0; i < this.max_bullet; ++i ) {
            Quaternion rot = _anchor.rotation;
            rot.eulerAngles = new Vector3( rot.eulerAngles.x, 
                                           rot.eulerAngles.y + Random.Range(-half_ang, half_ang),  
                                           rot.eulerAngles.z 
                                           );

            // GameObject spawnBullet = Instantiate(_bullet, _anchor.position, rot ) as GameObject;
            GameObject spawnBullet = SpawnManager.Instance().Spawn(_bullet, _anchor.position, rot );

            // TODO { 
            // if ( activeReloadCounter > 0.0f ) {
            //     this.bullet.renderer.material = this.matActiveReload;
            //     // this.bullet.transform.lossyScale = new Vector3( 2.0f, 2.0f, 1.0f );
            // }
            // else {
            //     this.bullet.renderer.material = this.matNormal;
            //     // this.bullet.transform.lossyScale = Vector3.one;
            // }
            // spawnBullet.renderer.material = _bullet.renderer.material;
            // // spawnBullet.transform.lossyScale = _bullet.transform.lossyScale;
            // } TODO end 

            BulletInfo bi = spawnBullet.GetComponent<BulletInfo>();
            if ( _activeReload ) {
                spawnBullet.transform.localScale = 2.0f * _bullet.transform.localScale;
                spawnBullet.renderer.material = bi.matActiveReload;
            }
            else {
                spawnBullet.transform.localScale = _bullet.transform.localScale;
                spawnBullet.renderer.material = bi.matNormal;
            }

            bi.ownerDamageInfo = this.GetComponent<DamageInfo>();

            DebugHelper.Assert( spawnBullet, "failed to spawn bullet" );
            // spawnBullet.transform.position += Random.Range(0.0f,3.0f) * spawnBullet.transform.forward;

            if ( fireEffect) {
                fireEffect.transform.position = _anchor.position;
                fireEffect.transform.rotation = _anchor.rotation;
                fireEffect.particleEmitter.Emit();
            }
        }
        return this.max_bullet;
    }
}
