// ======================================================================================
// File         : Steer_SimpleTest.cs
// Author       : Wu Jie 
// Last Change  : 11/28/2010 | 09:52:51 AM | Sunday,November
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

public class Steer_SimpleTest : Steer {

    Vector3 DestPos = Vector3.zero;

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected override void Start () {
        base.Start();
        DestPos = new Vector3 ( -10.0f, 0.0f, -10.0f );

        base.curSpeed = 0.0f;
        base.controller.Move ( transform.forward * base.curSpeed * Time.deltaTime );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Update () {
        Vector3 force = GetSteering_Seek_LimitByMaxSpeed ( DestPos );
        // Vector3 force = GetSteering_Seek ( DestPos );
        force.y = 0.0f;
        ApplySteeringForce(force);

        // DEBUG { 
        // draw destination
        DebugHelper.DrawCircleY ( this.DestPos, 1.0f, new Color(1.0f,1.0f,0.0f) );
        // draw velocity
        Vector3 vel = base.controller.velocity; 
        DebugHelper.DrawLine ( transform.position, 
                               transform.position + vel, 
                               new Color(0.0f,1.0f,0.0f) );
        // draw smoothed acceleration
        Vector3 acc = base.smoothedAcceleration;
        DebugHelper.DrawLine ( transform.position, 
                               transform.position + acc, 
                               new Color(1.0f,0.0f,1.0f) );
        // draw steering force
        // DebugHelper.DrawLine ( transform.position, 
        //                        transform.position + force, 
        //                        new Color(0.0f,1.0f,0.0f) );
        // } DEBUG end 
    }
}



