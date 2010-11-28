// ======================================================================================
// File         : SteerTest_WanderZombie.cs
// Author       : Wu Jie 
// Last Change  : 11/28/2010 | 12:20:00 PM | Sunday,November
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

public class SteerTest_WanderZombie : Steer {

    Vector3 DestPos = Vector3.zero;

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected override void Start () {
        base.Start();
        DestPos = transform.position;
        // DestPos = new Vector3 ( 
        //                         Random.Range(-10.0f,10.0f) 
        //                       , 0.0f 
        //                       , Random.Range(-10.0f,10.0f)
        //                       );

        // base.curSpeed = 1.5f;
        // base.controller.Move ( transform.forward * base.curSpeed * Time.deltaTime );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Update () {
        Vector3 force = GetSteering_Seek_LimitByMaxSpeed ( DestPos );
        // Vector3 force = GetSteering_Seek ( DestPos );
        force.y = 0.0f;

        force = Vector3.Lerp ( force, 10.0f * GetSteering_Wander(), 0.2f );

        ApplySteeringForce(force);

        // DEBUG { 
        // draw destination
        DebugHelper.DrawDestination ( this.DestPos );
        // draw velocity
        Vector3 vel = base.controller.velocity; 
        DebugHelper.DrawLine ( transform.position, 
                               transform.position + vel * 3.0f, 
                               new Color(0.0f,1.0f,0.0f) );
        // draw smoothed acceleration
        Vector3 acc = base.smoothedAcceleration;
        DebugHelper.DrawLine ( transform.position, 
                               transform.position + acc * 3.0f, 
                               new Color(1.0f,0.0f,1.0f) );
        // draw steering force
        // DebugHelper.DrawLine ( transform.position, 
        //                        transform.position + force, 
        //                        new Color(0.0f,1.0f,0.0f) );
        // } DEBUG end 
    }
}



