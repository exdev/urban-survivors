//Easy movement on the x-z plane.
var speed = 10.0;
var rotationSpeed = 100.0;

function Update () {
	// Get the horizontal and vertical axis.
	var translation = Input.GetAxis ("Vertical") * speed;
	var rotation = Input.GetAxis ("Horizontal") * rotationSpeed;

	// Make it move 10 meters per second instead of 10 meters per frame...
	translation *= Time.deltaTime;
	rotation *= Time.deltaTime;

	// Move translation along the objects z-axis
	transform.Translate (0, 0, translation);
	// Rotate around our y-axis
	transform.Rotate (0, rotation, 0);
}