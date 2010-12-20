var moveSpeed : float = 2; // The speed in game units that the bouncer moves per second
var rotateSpeed : float = 90; // The rotation speed in degrees per second

var forwardBackward : boolean = false; // Whether we're moving forward or backward
var goingNowhere : boolean = false; // Whether we're going nowhere or not
var jumping : boolean = true; // Whether we're jumping or not

function Update() // This function is run once every frame
{
	if (Input.GetKey(KeyCode.W)) { // If the user presses W
		rigidbody.velocity = transform.forward * moveSpeed + Vector3(0, rigidbody.velocity.y, 0); // Set the velocity to move forward while keeping gravity
		if (!forwardBackward || goingNowhere) { // If we used to be going backward or not moving
			forwardBackward = true; // Set it so that we're going forward
			goingNowhere = false; // Set it so that we're going somewhere
			animation.CrossFade("Run", 1); // CrossFade the Run animation over one second
		}
	}
	else { // Otherwise
		if (Input.GetKey(KeyCode.S)) { // If the user presses S
			rigidbody.velocity = transform.forward * -moveSpeed + Vector3(0, rigidbody.velocity.y, 0); // Set the velocity to move backward while keeping gravity
			if (forwardBackward || goingNowhere) { // If we used to be going forward or not moving
				forwardBackward = false; // Set it so that we're going backward
				goingNowhere = false; // Set it so that we're going somewhere
				animation.CrossFade("Belly Slide", 1); // CrossFade the Belly Slide animation over one second
			}
		}
		else { // Otherwise
			if (goingNowhere == false) { // If we used to be going somewhere
				goingNowhere = true; // Set it so that we're going nowhere
				animation.CrossFade("Dance", 1); // CrossFade the Dance animation over one second
			}
			rigidbody.velocity = Vector3.zero + Vector3(0, rigidbody.velocity.y, 0); // Set the velocity to move nowhere while keeping gravity
		}
	}
	if (Input.GetKey(KeyCode.A)) { // If the user presses A
		transform.Rotate(0, -rotateSpeed * Time.deltaTime, 0); // Rotate left at rotateSpeed degrees per second
	}
	if (Input.GetKey(KeyCode.D)) { // If the user presses D
		transform.Rotate(0, rotateSpeed * Time.deltaTime, 0); // Rotate right at rotateSpeed degrees per second
	}
	
	if (Input.GetKey(KeyCode.Space) && jumping == false) { // If the user presses space and we aren't jumping already
		rigidbody.velocity.y += 10; // Jump upwards
		animation.Blend("Jump", 1, .5); // Blend the jump animation towards one blend weight over .5 seconds
		jumping = true; // Toggle the jumping boolean on
	}
}

function OnCollisionEnter() // When the bouncer hits something (probably the ground)
{
	jumping = false; // Toggle the jumping boolean off
	// Note: Changed from Stop to CrossFade to avoid the jerking during collision with ground
	if (goingNowhere) { // If we aren't moving forward or backward
		animation.CrossFade("Dance", 1); // Eliminate the blended Jump animation in one second with Dance
	}
	else { // Otherwise
		if (forwardBackward) { // If we're moving forward
			animation.CrossFade("Run", 1); // Eliminate the blended Jump animation in one second with Run
		}
		else { // If we're moving backward
			animation.CrossFade("Belly Slide", 1); // Eliminate the blended Jump animation in one second with Belly Slide
		}
	}
}