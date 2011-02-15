// ======================================================================================
// File         : PlayerGirl.cs
// Author       : Wu Jie 
// Last Change  : 10/12/2010 | 00:44:29 AM | Tuesday,October
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

///////////////////////////////////////////////////////////////////////////////
// class 
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

[RequireComponent (typeof (Animation))]
public class PlayerGirl : PlayerBase {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public float degreePlayMoveLeftRight = 60.0f;

    // Q: why don't we just use UpperBody in this game?
    // A: this method will make sure the 'upper-body' is specific by user regardless the name of the entity, 
    //    so it can be flexiable enough for different game.
    public Transform upperBody;
    public Transform lowerBody;

    public GameObject followTarget;
    public float followDistance = 1.5f;

    // protected
    protected Vector3 aimDir = Vector3.forward;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	protected new void Start () {
        base.Start();

        DebugHelper.Assert( degreePlayMoveLeftRight <= 90.0f, "the degree can't larger than 90.0" );
        InitAnim ();

        // idle in
        this.anim.CrossFade("idle");
        States[0] = UpdateIdle;
	}
	
    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	protected new void Update () {
        HandleInput ();
        foreach ( StateUpdate state in States ) {
            if ( state != null )
                state();
        }
        ProcessMovement (); // handle steering
        // ShowDebugInfo(); // DEBUG
	}

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void LateUpdate () {
        if ( this.noHP() == false ) {
            // NOTE: upper-body rotation must be calculate after lower-body.
            this.lowerBody.forward = this.aimDir;
            this.upperBody.forward = this.aimDir;
        }

        // reset the internal state.
        this.lastHit.stunType = HitInfo.StunType.none;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected new void ShowDebugInfo () {
        DebugHelper.ScreenPrint("== player girl debug info ==");
        base.ShowDebugInfo();
        DebugHelper.ScreenPrint("curHP = " + this.playerInfo.curHP);
        DebugHelper.ScreenPrint("maxHP = " + this.playerInfo.maxHP);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    private void InitAnim () {
        // animation
        AnimationState state;
        string[] anim_keys0 = { 
            "idle",
            "downIdle",
            "moveForward", 
            "moveBackward", 
            "moveRight", 
            "moveLeft" 
        };
        foreach (string key in anim_keys0) {
            state = this.anim[key];
            state.layer = 0;
            state.wrapMode = WrapMode.Loop;
            state.weight = 1.0f;
            state.enabled = false;
        }

        string[] anim_keys0_once = { 
            "fallDown",
            // "getUp", 
            // "turnRight", 
            // "turnLeft", 
        };
        foreach (string key in anim_keys0_once) {
            state = this.anim[key];
            state.layer = 0;
            state.wrapMode = WrapMode.Once;
            state.weight = 1.0f;
            state.enabled = false;
        }

        string[] anim_keys1 = { 
            "shootSMG",
            "reload_smg",
            "hit1",
            "hit2"
        };
        foreach (string key in anim_keys1) {
            state = this.anim[key];
            state.wrapMode = WrapMode.Once;
            state.layer = 1;
            state.weight = 1.0f;
            state.AddMixingTransform(this.upperBody);
            state.enabled = false;
        }
        this.anim.SyncLayer(0);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void UpdateIdle () {
        // shooting
        if ( States[1] == null && Game.ScreenPad().CanShoot() ) {
            States[1] = UpdateShoot;
        }

        // go to falldown
        if ( this.playerInfo.curHP <= 0.0f ) {
            StartCoroutine(FallDown());
            return;
        }

        // go to follow
        if ( IsFarAwayFollowTarget () ) {
            States[0] = UpdateFollow;
            return;
        }

        // go to avoid
        if ( NeedAvoid() ) {
            States[0] = UpdateAvoid;
            return;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void UpdateFollow () {
        // shooting
        if ( States[1] == null && Game.ScreenPad().CanShoot() ) {
            States[1] = UpdateShoot;
        }

        // if we in target distance
        if ( IsCloseFollowTarget () ) {
            this.Stop();
            this.anim.CrossFade("idle");
            States[0] = UpdateIdle;
            return;
        }

        //
        if ( NeedAvoid() ) {
            States[0] = UpdateAvoid;
            return;
        }

        // moving
        CharacterController fo = this.followTarget.collider as CharacterController;
        Vector3 dir = (this.transform.position - fo.transform.position).normalized;
        if ( Vector3.Dot ( fo.transform.forward, dir ) > -0.707f )
            this.Seek(this.followTarget.transform.position + this.followTarget.transform.forward * -this.followDistance );
        else
            this.Seek(this.followTarget.transform.position);
        ActMovement();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void UpdateAvoid () {
        // shooting
        if ( States[1] == null && Game.ScreenPad().CanShoot() ) {
            States[1] = UpdateShoot;
        }

        if ( NeedAvoid() == false ) {
            // follow
            if ( IsFarAwayFollowTarget () ) {
                States[0] = UpdateFollow;
                return;
            }
            // idle
            else {
                this.anim.CrossFade("idle");
                States[0] = UpdateIdle;
                return;
            }
        }

        //
        this.Avoid();
        ActMovement();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    IEnumerator FallDown () {
        States[0] = null;
        States[1] = null;

        this.Stop();
        this.transform.forward = this.upperBody.forward;

        this.anim.Play("fallDown", PlayMode.StopAll);
        yield return new WaitForSeconds( this.anim["fallDown"].length );
        this.anim.CrossFade("downIdle");

        yield return StartCoroutine( "WaitForRecover" );

        // DELME: do it in OnRecover { 
        // // go back to idle
        // this.anim.CrossFade("idle");
        // States[0] = UpdateIdle;
        // } DELME end 
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void UpdateReload () {
        if ( this.IsReloading() == false ) {
            States[1] = null;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void UpdateShoot () {
        ShootInfo shootInfo = this.GetShootInfo();

        // stop shoot
        if ( Game.ScreenPad().CanShoot() == false ) {
            States[1] = null;
            return;
        }

        // we don't have a gun
        if ( shootInfo == null ) {
            States[1] = null;
            return;
        }

        // go to reload
        if ( shootInfo.OutOfAmmo() && shootInfo.RemainBullets() > 0 ) {
            Game.ScreenPad().SendMessage ( "OnReload" );
            return;
        }

        //
        if ( this.anim.IsPlaying(shootInfo.shootAnim) == false ) {
            shootInfo.AdjustAnim(this.anim);
            shootInfo.Fire();
            // TODO: to use this way, you must rewrite the shoot system { 
            // this.anim.Rewind(shootInfo.shootAnim);
            // this.anim.CrossFade(shootInfo.shootAnim, 0.05f);
            // } TODO end 
            this.anim.Play(shootInfo.shootAnim);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void UpdateStun () {
        if ( this.anim.IsPlaying("hit1") == false &&
             this.anim.IsPlaying("hit2") == false )
        {
            States[1] = null;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public bool NeedAvoid () {
        CharacterController fo = this.followTarget.collider as CharacterController;
        Vector3 dir = (this.transform.position - fo.transform.position).normalized;
        bool needAvoid = false;
        if ( Vector3.Dot ( fo.transform.forward, dir ) > -0.707f ) {

            Vector3 newPos = this.transform.position + this.controller.velocity * Time.deltaTime;
            Vector3 newFoPos = fo.transform.position + fo.velocity * Time.deltaTime;
            float next_distance = (newPos - newFoPos).magnitude;
            float minCollideDistance = fo.radius + this.controller.radius;

            if ( next_distance <= minCollideDistance ) {
                needAvoid = true;
            }
        }
        return needAvoid;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public bool IsFarAwayFollowTarget () {
        float cur_distance = (this.transform.position - this.followTarget.transform.position).magnitude;
        if ( cur_distance >= this.followDistance * 1.2f ) {
            return true;
        }
        return false;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public bool IsCloseFollowTarget () {
        float cur_distance = (this.transform.position - this.followTarget.transform.position).magnitude;
        if ( cur_distance <= this.followDistance ) {
            return true;
        }
        return false;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public bool IsReloading () {
        ShootInfo shootInfo = this.GetShootInfo();
        return this.IsPlayingAnim( shootInfo.reloadAnim );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    private void HandleInput() {
        // DISABLE { 
        // // get move direction
        // if ( this.followTarget == null ) {
        //     Vector2 screen_dir = Game.ScreenPad().GetMoveDirection();
        //     if ( screen_dir.sqrMagnitude >= 0.0f ) {
        //         this.moveDir.x = screen_dir.x;
        //         this.moveDir.y = screen_dir.y;
        //         Transform mainCamera = Camera.main.transform;
        //         this.moveDir = mainCamera.TransformDirection(this.moveDir.normalized); 
        //         this.moveDir.y = 0.0f;
        //         this.moveDir = this.moveDir.normalized;
        //     }
        // }
        // } DISABLE end 

        // get direction by Game.ScreenPad()
        Vector2 aimDir2D = Game.ScreenPad().GetAimingDirection();
        this.aimDir = Vector3.zero; 
        this.aimDir.x = aimDir2D.x; 
        this.aimDir.y = aimDir2D.y; 
        this.aimDir = Camera.main.transform.TransformDirection(this.aimDir);
        this.aimDir.y = 0.0f;
        this.aimDir = this.aimDir.normalized;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void ProcessMovement () {
        // handle steering
        Vector3 force = Vector3.zero;
        if ( this.steeringState == SteeringState.seeking ) {
            force = GetSteering_Seek_MaxForces ( this.targetPos );
            force.y = 0.0f;
        }
        else if ( this.steeringState == SteeringState.avoiding ) {
            force = GetSteering_Flee_MaxForces( this.followTarget.transform.position ); 
            force.y = 0.0f;
        }
        else if ( this.steeringState == SteeringState.braking ) {
            ApplyBrakingForce();
        }
        ApplySteeringForce( force );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public Vector2 GetAutoLockDir ( Vector2 _dir ) {
        // TODO { 
        // Vector2 my_pos = new Vector2 ( this.transform.position.x, 
        //                                this.transform.position.z );
        // List<GameObject> enemies = Game.GetEnemies();
        // foreach ( GameObject enemy in enemies ) {
        //     CharacterController ctrl = enemy.collider as CharacterController;
        //     if ( ctrl ) {
        //         float radius = ctrl.radius; 

        //         Vector2 min = new Vector2 ( enemy.collider.bounds.min.x,
        //                                     enemy.collider.bounds.min.z );
        //         Vector2 max = new Vector2 ( enemy.collider.bounds.max.x,
        //                                     enemy.collider.bounds.max.z );
        //         Vector2 dmin = min - my_pos; 
        //         Vector2 dmax = max - my_pos; 
        //         float deg1 = Vector2.Angle(_dir,dmin);
        //         float deg2 = Vector2.Angle(_dir,dmax);

        //         // DebugHelper.ScreenPrint("deg1: " + deg1);
        //         // DebugHelper.ScreenPrint("deg2: " + deg2);
        //     }
        // }
        // } TODO end 
        // TODO:
        return _dir;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void ActMovement () {
        Vector3 dir = this.controller.velocity.normalized;

        // process lower-body rotation
        float angle = Vector3.Angle ( dir, this.aimDir );
        // DebugHelper.ScreenPrint("angle = " + angle); // DEBUG
        string animName = "";
        if ( angle > 180.0f - degreePlayMoveLeftRight ) {
            animName = "moveBackward";
        } else if ( angle < degreePlayMoveLeftRight ) {
            animName = "moveForward";
        } else {
            Vector3 up = Vector3.Cross(dir,aimDir);
            if ( up.y > 0.0f )
                animName = "moveLeft";
            else
                animName = "moveRight";
        }
        this.anim[animName].normalizedSpeed = this.StepSpeed * this.controller.velocity.magnitude;
        if ( this.anim.IsPlaying(animName) == false )
            this.anim.CrossFade(animName,0.3f);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnTriggerEnter ( Collider _other ) {
        if ( base.ApplyDamage(_other) ) {
            Game.ScreenPad().SendMessage ( "OnGirlHit" );
        }

        if ( this.lastHit.stunType != HitInfo.StunType.none ) {
            this.OnStun();
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnStun () {
        // NOTE: it could be possible we interupt to hit when boy is attacking.
        AttackInfo atk_info = this.GetAttackInfo();
        if ( atk_info != null && atk_info.curCombo != null )
            atk_info.curCombo.attack_shape.active = false;

        // HACK: simple random choose animation { 
        // string[] names = {"hit1", "hit2"};
        // string animName = names[Mathf.FloorToInt(Random.Range(0.0f,2.0f))];
        // } HACK end 
        string animName = "hit1";
        if ( IsBehind( this.lastHit.position ) )
            animName = "hit2";

        this.anim.Rewind(animName);
        this.anim.CrossFade(animName);

        States[1] = UpdateStun;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnReload () {
        ShootInfo shootInfo = this.GetShootInfo();
        if ( shootInfo ) {
            shootInfo.ActiveReload(false);
            shootInfo.AdjustAnim(this.anim);
            this.anim.CrossFade(shootInfo.reloadAnim);
            shootInfo.Reload();

            States[1] = UpdateReload;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnActiveReload () {
        ShootInfo shootInfo = this.GetShootInfo();
        if ( shootInfo ) {
            shootInfo.ActiveReload(true);
            shootInfo.AdjustAnim(this.anim);
            this.anim.CrossFade(shootInfo.reloadAnim);
            shootInfo.Reload();
            Game.ScreenPad().SendMessage ( "GoActiveReloadForSeconds", shootInfo.activeReloadTime );
            States[1] = UpdateReload;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public float ReloadTime () {
        ShootInfo shootInfo = this.GetShootInfo();
        if ( shootInfo ) {
            return this.anim[shootInfo.reloadAnim].length;
        }
        return 0.0f;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnRecover ( float _hp ) { 
        if ( this.noHP() ) {
            StopCoroutine( "WaitForRecover" );
            // go back to idle
            this.anim.CrossFade("idle");
            States[0] = UpdateIdle;
        }
        this.playerInfo.curHP = Mathf.Min( this.playerInfo.curHP + _hp, this.playerInfo.maxHP );
    } 
}
