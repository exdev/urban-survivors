var interval : float = 30.0f ;
var Spawners:GameObject[]; 
private var spawn_script:Spawner_point;

function Start () {
	
	//start function ZombieOutbreak after interval
	yield StartCoroutine(ZombieOutbreak());

	
}

function ZombieOutbreak () {
	
	yield WaitForSeconds (interval);
	//add 1 to maxAlive property of each spawner in the list
	for (var sp_point : GameObject in Spawners)
	{
		
		spawn_script = sp_point.GetComponent("Spawner_point");
		spawn_script.maxAlive += 1;
	}
	yield StartCoroutine(ZombieOutbreak());
	print("Time up "+Time.time);
}
