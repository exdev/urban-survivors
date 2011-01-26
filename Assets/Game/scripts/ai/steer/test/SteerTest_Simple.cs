// ======================================================================================
// File         : SteerTest_Simple.cs
// Author       : Wu Jie 
// Last Change  : 11/28/2010 | 12:18:34 PM | Sunday,November
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

public class SteerTest_Simple : Steer {

    Vector3 DestPos = Vector3.zero;

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected new void Start () {
        base.Start();
        DestPos = new Vector3 ( 
                                Random.Range(-10.0f,10.0f) 
                              , 0.0f 
                              , Random.Range(-10.0f,10.0f)
                              );
        // DestPos = transform.position + Vector3.forward * 2.0f;

        base.curSpeed = 2.0f;
        base.controller.Move ( transform.forward * base.curSpeed * Time.deltaTime );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected new void Update () {
        Vector3 force = Vector3.zero;
        Vector3 force_seek = GetSteering_Seek_LimitByMaxSpeed ( DestPos );

        float distance = (transform.position - DestPos).magnitude;
        if ( distance < 5.0f ) {
            ApplyBrakingForce();
        }
        else {
            force = force_seek;
            force.y = 0.0f;
            ApplySteeringForce(force);
        }

        if ( base.curSpeed == 0.0f ) {
            DestPos = new Vector3 ( 
                                   Random.Range(-10.0f,10.0f) 
                                   , 0.0f 
                                   , Random.Range(-10.0f,10.0f)
                                  );

        }

        // DEBUG { 
        // draw destination
        DebugHelper.DrawDestination ( this.DestPos );
        // draw velocity
        Vector3 vel = base.Velocity(); 
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



