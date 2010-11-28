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
    public float mass = 1.0f;

    ///////////////////////////////////////////////////////////////////////////////
    // private functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	protected virtual void Start () {
        this.controller = GetComponent<CharacterController>();
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
        if ( this.curSpeed > 0 ) {
            transform.forward = _newVelocity.normalized;
        }
    }

    ///////////////////////////////////////////////////////////////////////////////
    // public functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public Vector3 GetSteering_Seek_LimitByMaxSpeed ( Vector3 _pos ) {
        Vector3 offset = _pos - transform.position;
        Vector3 desiredVelocity = Vector3.ClampMagnitude ( offset, this.maxSpeed );
        return desiredVelocity - this.controller.velocity;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public Vector3 GetSteering_Seek ( Vector3 _pos ) {
        Vector3 desiredVelocity = _pos - transform.position;
        return desiredVelocity - this.controller.velocity;
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

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void ApplySteeringForce ( Vector3 _force ) {
        Vector3 adjustedForce = AdjustRawSteeringForce (_force);

        // enforce limit on magnitude of steering force
        Vector3 clippedForce = Vector3.ClampMagnitude(adjustedForce,this.maxForce);

        // compute acceleration and velocity
        Vector3 newAcceleration = (clippedForce / this.mass);
        Vector3 newVelocity = this.controller.velocity;

        // damp out abrupt changes and oscillations in steering acceleration
        // (rate is proportional to time step, then clipped into useful range)
        if ( Time.deltaTime > 0.0f ) {
            float smoothRate = Mathf.Clamp (9.0f * Time.deltaTime, 0.15f, 0.4f);
            smoothRate = Mathf.Clamp01(smoothRate);
            this.smoothedAcceleration = Vector3.Lerp ( this.smoothedAcceleration, 
                                                       newAcceleration, 
                                                       smoothRate );
        }

        // Euler integrate (per frame) acceleration into velocity
        newVelocity += this.smoothedAcceleration * Time.deltaTime;
        newVelocity.y = 0.0f;

        // enforce speed limit
        newVelocity = Vector3.ClampMagnitude (newVelocity,this.maxSpeed);

        // update Speed
        this.curSpeed = newVelocity.magnitude;

        // Euler integrate (per frame) velocity into position
        // DELME: setPosition (position() + (newVelocity * Time.deltaTime));
        this.controller.Move (newVelocity * Time.deltaTime);

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
}
