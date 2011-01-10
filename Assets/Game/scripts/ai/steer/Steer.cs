// ======================================================================================
// File         : Steer.cs
// Author       : Wu Jie 
// Last Change  : 11/24/2010 | 23:26:24 PM | Wednesday,November
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////
// class defines
///////////////////////////////////////////////////////////////////////////////

[RequireComponent(typeof(CharacterController))]
public class Steer : MonoBehaviour {

    protected CharacterController controller = null;
    protected float curSpeed = 0.0f;
    protected Vector3 smoothedAcceleration = Vector3.zero;
    float wanderSide = 0.0f;

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public float maxSpeed = 1.0f;
    public float maxForce = 0.1f;
    public float brakingRate = 10.0f;
    public float mass = 1.0f;

    ///////////////////////////////////////////////////////////////////////////////
    // private functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	protected void Start () {
        this.controller = GetComponent<CharacterController>();
	}

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	protected void Update () {
        // ShowDebugInfo();
	}

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    Vector3 AdjustRawSteeringForce ( Vector3 _force ) {
        float maxAdjustedSpeed = 0.2f * this.maxSpeed;

        if ( (this.curSpeed > maxAdjustedSpeed) || (_force == Vector3.zero) ) {
            return _force;
        }
        else {
            float range = this.curSpeed/maxAdjustedSpeed;
            // float cosine = Mathf.Lerp ( 1.0f, -1.0f, Mathf.Pow (range, 6) );
            // float cosine = Mathf.Lerp ( 1.0f, -1.0f, Mathf.Pow (range, 10) );
            // float cosine = Mathf.Lerp ( 1.0f, -1.0f, Mathf.Pow (range, 20) );
            // float cosine = Mathf.Lerp ( 1.0f, -1.0f, Mathf.Pow (range, 100) );
            // float cosine = Mathf.Lerp ( 1.0f, -1.0f, Mathf.Pow (range, 50) );
            float cosine = Mathf.Lerp( 1.0f, -1.0f, Mathf.Pow (range, 20) );
            return MathHelper.InsideDeviationAngle ( _force, transform.forward, cosine );
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void AdjustOrientation ( Vector3 _newVelocity ) {
        if ( this.curSpeed > 0.001f ) {
            transform.forward = _newVelocity.normalized;
        }
    }

    ///////////////////////////////////////////////////////////////////////////////
    // public functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    // NOTE: bullet hit controller will change its velocity { 
    // public Vector3 Velocity () { return this.controller.velocity; }
    // } NOTE end 
    public Vector3 Velocity () { return transform.forward * this.curSpeed; }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public float CurSpeed () { return this.curSpeed; }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public bool IsAhead ( Vector3 _targetPos, float _cosThreshold = 0.707f ) {
        Vector3 targetDir = (_targetPos - transform.position).normalized;
        return Vector3.Dot ( transform.forward, targetDir ) > _cosThreshold;
    } 

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public bool IsAside ( Vector3 _targetPos, float _cosThreshold = 0.707f ) {
        Vector3 targetDir = (_targetPos - transform.position).normalized;
        float dp = Vector3.Dot ( transform.forward, targetDir );
        return (dp < _cosThreshold) && (dp > -_cosThreshold);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public bool IsBehind ( Vector3 _targetPos, float _cosThreshold = -0.707f ) {
        Vector3 targetDir = (_targetPos - transform.position).normalized;
        return Vector3.Dot ( transform.forward, targetDir ) < _cosThreshold;
    }

    ///////////////////////////////////////////////////////////////////////////////
    // steering functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public Vector3 GetSteering_Seek_LimitByMaxSpeed ( Vector3 _pos ) {
        Vector3 offset = _pos - transform.position;
        Vector3 desiredVelocity = Vector3.ClampMagnitude ( offset, this.maxSpeed );
        return desiredVelocity - this.Velocity();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public Vector3 GetSteering_Seek ( Vector3 _pos ) {
        Vector3 desiredVelocity = _pos - transform.position;
        return desiredVelocity - this.Velocity();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public Vector3 GetSteering_Seek_MaxForces ( Vector3 _pos ) {
        Vector3 dir = (_pos - transform.position).normalized;
        return dir * this.maxForce;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public Vector3 GetSteering_Flee_LimitByMaxSpeed ( Vector3 _pos ) {
        Vector3 offset = transform.position - _pos;
        Vector3 desiredVelocity = Vector3.ClampMagnitude ( offset, this.maxSpeed );
        return desiredVelocity - this.Velocity();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public Vector3 GetSteering_Flee ( Vector3 _pos ) {
        Vector3 desiredVelocity = transform.position - _pos;
        return desiredVelocity - this.Velocity();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public Vector3 GetSteering_Wander () {
        float speed = 12.0f * Time.deltaTime;
        this.wanderSide = this.wanderSide + speed * Random.Range(-1.0f,1.0f); 
        this.wanderSide = Mathf.Clamp ( this.wanderSide, -1.0f, 1.0f );
        return transform.right * this.wanderSide;
    }

    ///////////////////////////////////////////////////////////////////////////////
    // movement functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void ApplySteeringForce ( Vector3 _force ) {
        Vector3 adjustedForce = AdjustRawSteeringForce (_force);

        // enforce limit on magnitude of steering force
        Vector3 clippedForce = Vector3.ClampMagnitude(adjustedForce,this.maxForce);

        // compute acceleration and velocity
        Vector3 newAcceleration = (clippedForce / this.mass);
        Vector3 newVelocity = this.Velocity();

        // jwu DISABLE { 
        // // damp out abrupt changes and oscillations in steering acceleration
        // // (rate is proportional to time step, then clipped into useful range)
        // if ( Time.deltaTime > 0.0f ) {
        //     float smoothRate = Mathf.Clamp (9.0f * Time.deltaTime, 0.15f, 0.4f);
        //     smoothRate = Mathf.Clamp01(smoothRate);
        //     this.smoothedAcceleration = Vector3.Lerp ( this.smoothedAcceleration, 
        //                                                newAcceleration, 
        //                                                smoothRate );
        // }
        this.smoothedAcceleration = newAcceleration;
        // } jwu DISABLE end 

        // Euler integrate (per frame) acceleration into velocity
        newVelocity += this.smoothedAcceleration * Time.deltaTime;
        newVelocity.y = 0.0f;

        // enforce speed limit
        newVelocity = Vector3.ClampMagnitude (newVelocity,this.maxSpeed);

        // update Speed
        this.curSpeed = newVelocity.magnitude;

        // TODO: add properties useGravity ? { 
        // apply gravity
        Vector3 gravity = Vector3.zero;
        if ( controller.isGrounded == false ) {
            gravity.y = -10.0f;
        }
        // } TODO end 

        // Euler integrate (per frame) velocity into position
        this.controller.Move ( (gravity + newVelocity) * Time.deltaTime);

        // regenerate local space (by default: align vehicle's forward axis with
        // new velocity, but this behavior may be overridden by derived classes.)
        AdjustOrientation ( newVelocity );

        // TODO { 
        // // maintain path curvature information
        // measurePathCurvature (Time.deltaTime); // TODO: draw this!!!

        // // running average of recent positions
        // blendIntoAccumulator (Time.deltaTime * 0.06f, // QQQ
        //                       position (),
        //                       _smoothedPosition);
        // } TODO end 
    }

    // KEEPME { 
    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void ApplyBrakingForce () {
        float rawBraking = this.curSpeed * this.brakingRate;
        float clipBraking = Mathf.Clamp( rawBraking, 0.0f, this.maxForce );
        this.curSpeed -= clipBraking * Time.deltaTime;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void BrakeImmediately () {
        this.curSpeed = 0.0f;
    }

    // } KEEPME end 

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected void ShowDebugInfo () {
        // draw velocity
        Vector3 vel = this.Velocity(); 
        DebugHelper.DrawLine ( transform.position, 
                               transform.position + vel * 3.0f, 
                               new Color(0.0f,1.0f,0.0f) );
        // draw smoothed acceleration
        Vector3 acc = this.smoothedAcceleration;
        DebugHelper.DrawLine ( transform.position, 
                               transform.position + acc * 3.0f, 
                               new Color(1.0f,0.0f,1.0f) );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    // public void ApplyBrakingForce ( float _brakingRate = 1.0f ) {
    //     // if we already break the target, skip process this.
    //     if ( this.curSpeed == 0.0f )
    //         return;

    //     // compute acceleration and velocity
    //     float brakingForce = this.maxBrakingForce * _brakingRate;
    //     Vector3 curVelocity = this.Velocity();
    //     Vector3 newAcceleration = ( -curVelocity.normalized * brakingForce / this.mass);

    //     // damp out abrupt changes and oscillations in steering acceleration
    //     // (rate is proportional to time step, then clipped into useful range)
    //     if ( Time.deltaTime > 0.0f ) {
    //         float smoothRate = Mathf.Clamp (9.0f * Time.deltaTime, 0.15f, 0.4f);
    //         smoothRate = Mathf.Clamp01(smoothRate);
    //         this.smoothedAcceleration = Vector3.Lerp ( this.smoothedAcceleration, 
    //                                                    newAcceleration, 
    //                                                    smoothRate );
    //     }

    //     // Euler integrate (per frame) acceleration into velocity
    //     Vector3 newVelocity = curVelocity;
    //     newVelocity += this.smoothedAcceleration * Time.deltaTime;
    //     newVelocity.y = 0.0f;

    //     // NOTE: this will prevent character move fast when braking (could be happend when using maxForce more bigger than maxBrakingForce)
    //     newVelocity = Vector3.ClampMagnitude (newVelocity,this.maxSpeed);

    //     float cosTheta = Vector3.Dot( curVelocity.normalized, newVelocity.normalized );
    //     // if newVelocity and curVelocity are not in same direction.
    //     if ( cosTheta < -0.707f ) {
    //         newVelocity = Vector3.zero;
    //         this.smoothedAcceleration = Vector3.zero;
    //     }

    //     // update Speed
    //     this.curSpeed = newVelocity.magnitude;

    //     // apply gravity
    //     Vector3 gravity = Vector3.zero;
    //     if ( controller.isGrounded == false ) {
    //         gravity.y = -10.0f;
    //     }

    //     //
    //     this.controller.Move ( (gravity + newVelocity) * Time.deltaTime);
    // }
}
