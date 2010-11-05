// Draws the Light bulb icon at position of the object.
// Because we draw it inside OnDrawGizmos the icon is also pickable 
// in the scene view.

function OnDrawGizmos () {
    Gizmos.DrawIcon (transform.position, "Adobe.ico");
}