// ======================================================================================
// File         : Infrared.js
// Author       : Wu Jie 
// Last Change  : 08/25/2010 | 22:42:29 PM | Wednesday,August
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// properties
///////////////////////////////////////////////////////////////////////////////

var material: Material;
var lineSize = 0.2;
var lineColor : Color;

///////////////////////////////////////////////////////////////////////////////
// function define
///////////////////////////////////////////////////////////////////////////////

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

function Start () {
    var lr = gameObject.GetComponent(LineRenderer);
    if ( lr == null ) {
        lr = gameObject.AddComponent(LineRenderer);
        lr.useWorldSpace = false;
        lr.SetWidth(lineSize, lineSize);
        lr.material = material;
        lr.SetPosition(0, Vector3.zero );
        lr.SetPosition(1, Vector3.forward * 10.0 );
        lr.SetColors(lineColor,lineColor);
    }
}

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

function Update () {
    var lr = GetComponent(LineRenderer);
    if ( lr ) {

        var layerMask = 1 << 2;
        // This would cast rays only against colliders in layer 2.
        // But instead we want to collide against everything except layer 2. 
        // The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;

        var dist = 100.0;
        var hit : RaycastHit;
        var fwd = transform.TransformDirection (Vector3.forward);
        if ( Physics.Raycast (transform.position, fwd, hit, 100.0, layerMask ) ) {
            dist = hit.distance;
        }
        lr.SetPosition( 1, Vector3.forward * dist );
    }
}
