// ======================================================================================
// File         : PlayerBoy.cs
// Author       : Wu Jie 
// Last Change  : 10/08/2010 | 23:24:12 PM | Friday,October
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;


///////////////////////////////////////////////////////////////////////////////
// class PlayerBoy
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

public class PlayerBoy : PlayerBase {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public Vector3 screenDir = Vector3.zero;
    public Transform upperBody;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	protected new void Start () {
        base.Start();

        //
        this.InitAnim();

        // idle in
        this.anim.CrossFade("idle");
        States[0] = UpdateIdle;
	}
	
    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	protected new void Update () {
        HandleInput();
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
        // reset the internal state.
        this.moveDir = Vector3.zero; 
        this.lastHit.stunType = HitInfo.StunType.none;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected new void ShowDebugInfo () {
        DebugHelper.ScreenPrint("== player boy debug info ==");
        base.ShowDebugInfo();
        DebugHelper.ScreenPrint("curHP = " + this.playerInfo.curHP);
        DebugHelper.ScreenPrint("maxHP = " + this.playerInfo.maxHP);
    }


    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void InitAnim () {
        // setup the animation state
        AnimationState state;
        string[] anim_keys0 = { 
            "moveforward", 
            "idle",
            "downIdle"
        };
        foreach (string key in anim_keys0) {
            state = this.anim[key];
            state.layer = 0;
            state.wrapMode = WrapMode.Loop;
            state.weight = 1.0f;
            state.enabled = false;
        }

        //
        string[] anim_keys0_once = { 
            "fallDown",
            "getUp"
        };
        foreach (string key in anim_keys0_once) {
            state = this.anim[key];
            state.layer = 0;
            state.wrapMode = WrapMode.Once;
            state.weight = 1.0f;
            state.enabled = false;
        }

        //
        string[] melee_anims = { 
            "melee1_copy", 
            "melee2_copy", 
        };
        foreach (string key in melee_anims) {
            state = this.anim[key];
            state.layer = 1;
            state.wrapMode = WrapMode.Once;
            state.weight = 5.0f;
            state.enabled = false;
            state.AddMixingTransform(this.upperBody);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void UpdateIdle () {
        // go to falldown
        if ( this.playerInfo.curHP <= 0.0f ) {
            StartCoroutine(FallDown());
            return;
        }

        // go to movement
        if ( this.IsMoving() ) {
            this.anim.CrossFade("moveforward");
            States[0] = UpdateMove;
            return;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void UpdateMove () {
        // go to falldown
        if ( this.playerInfo.curHP <= 0.0f ) {
            StartCoroutine(FallDown());
            return;
        }

        // go to idle
        if ( this.IsMoving() == false ) {
            this.Stop();
            this.anim.CrossFade("idle");
            States[0] = UpdateIdle;
            return;
        }

        //
        Transform mainCamera = Camera.main.transform;
        Vector3 dir = mainCamera.TransformDirection(this.screenDir.normalized); 
        dir.y = 0.0f;
        dir.Normalize();
        this.Move(dir);
        ActMovement();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void UpdateCombo () {
        AttackInfo atk_info = this.GetAttackInfo();
        if ( atk_info.curCombo == null ) {
            States[1] = null;
            return;
        }

        if ( this.anim.IsPlaying(atk_info.curCombo.animName) == false ) {
            States[1] = null;
            return;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void WaitForNextCombo () {
        AttackInfo atk_info = this.GetAttackInfo();

        AnimationState curAnim = this.anim[atk_info.curCombo.animName];
        if ( curAnim.time >= atk_info.curCombo.validInputTime.y ) {
            ComboInfo nextCombo = atk_info.curCombo.next;
            this.anim[nextCombo.animName].normalizedSpeed = atk_info.speed;
            this.anim.Rewind(nextCombo.animName); // NOTE: without rewind, you can't play one animation continuesly
            this.anim.CrossFade(nextCombo.animName,0.1f);
            atk_info.curCombo = nextCombo;
            // adjust the orientation
            // AdjustOrientation();

            States[1] = UpdateCombo;
            return;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void UpdateStun () {
        if ( this.anim.IsPlaying("hit1") == false &&
             this.anim.IsPlaying("hit2") == false )
        {
            this.anim.CrossFade("idle");
            States[0] = UpdateIdle;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void StartCombo () {
        AttackInfo atk_info = this.GetAttackInfo();
        ComboInfo first_combo = atk_info.combo_entry;
        atk_info.curCombo = first_combo;
        this.anim[first_combo.animName].normalizedSpeed = atk_info.speed;
        this.anim.Rewind(first_combo.animName); // NOTE: without rewind, you can't play one animation continuesly
        this.anim.CrossFade(first_combo.animName,0.1f);

        // adjust the orientation
        // AdjustOrientation();
        States[1] = UpdateCombo;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    IEnumerator FallDown () {
        States[0] = null;
        States[1] = null;

        this.Stop();

        this.anim.Play("fallDown", PlayMode.StopAll);
        yield return new WaitForSeconds( this.anim["fallDown"].length );

        this.anim.CrossFade("downIdle");
        yield return StartCoroutine( "WaitForRecover" );

        this.anim.CrossFade("getUp");
        yield return new WaitForSeconds( this.anim["getUp"].length );

        // go back to idle
        this.anim.CrossFade("idle");
        States[0] = UpdateIdle;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void HandleInput() {
        this.screenDir = screenPad ? screenPad.GetMoveDirection() : Vector2.zero;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void ProcessMovement () {
        // handle steering
        Vector3 force = Vector3.zero;
        if ( this.steeringState == SteeringState.moving ) {
            force = this.moveDir * base.maxForce;
            force.y = 0.0f;
        }
        else if ( this.steeringState == SteeringState.braking || 
                  this.steeringState == SteeringState.disable ) {
            ApplyBrakingForce();
        }
        ApplySteeringForce( force );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public bool IsMoving () { return this.screenDir.sqrMagnitude > 0.0f; }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void StartMeleeAttack () {
        AttackInfo atk_info = this.GetAttackInfo();
        atk_info.curCombo.attack_shape.active = true;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void EndMeleeAttack () {
        AttackInfo atk_info = this.GetAttackInfo();
        atk_info.curCombo.attack_shape.active = false;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void ActMovement () {
        // adjust move animation speed
        this.anim["moveforward"].normalizedSpeed = Mathf.Max(this.StepSpeed * this.CurSpeed(),1.0f);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void AdjustOrientation () {
        if ( this.screenDir == Vector3.zero )
            return;

        // adjust the orientation
        Transform mainCamera = Camera.main.transform;
        Vector3 dir = mainCamera.TransformDirection(this.screenDir.normalized); 
        dir.y = 0.0f;
        dir.Normalize();
        this.transform.forward = dir;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnTriggerEnter ( Collider _other ) {
        if ( base.ApplyDamage(_other) ) {
            screenPad.SendMessage ( "OnBoyHit" );
        }

        if ( this.lastHit.stunType != HitInfo.StunType.none ) {
            OnStun();
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnMelee () {
        // never act when player noHP or OnStun
        if ( this.noHP() || States[0] == UpdateStun ) {
            return;
        }

        if ( States[1] == null ) {
            StartCombo();
        }
        else if ( States[1] == UpdateCombo ) {
            AttackInfo atk_info = this.GetAttackInfo();
            AnimationState curAnim = this.anim[atk_info.curCombo.animName];
            ComboInfo nextCombo = atk_info.curCombo.next;

            if ( nextCombo == null )
                return;

            // if we are in the valid input range
            if ( curAnim.time >= atk_info.curCombo.validInputTime.x 
                 && curAnim.time <= atk_info.curCombo.validInputTime.y )
            {
                States[1] = WaitForNextCombo;
            }
        }
    }


    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected void OnStun () {
        States[1] = null;
        this.anim.Stop();
        Stop();

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

        States[0] = UpdateStun;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    IEnumerator OnRecover ( float _hp ) { 
        if ( this.noHP() ) {
            StopCoroutine( "WaitForRecover" );
            this.anim.CrossFade("getUp");
            yield return new WaitForSeconds( this.anim["getUp"].length );

            // go back to idle
            this.anim.CrossFade("idle");
            States[0] = UpdateIdle;
        }
        this.playerInfo.curHP = Mathf.Min( this.playerInfo.curHP + _hp, this.playerInfo.maxHP );
    } 
}
