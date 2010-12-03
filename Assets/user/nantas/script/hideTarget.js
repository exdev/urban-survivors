var tweenTarget : GameObject;

function OnTriggerEnter () {
	
	Debug.Log("trigger entered!");
	iTween.FadeTo(tweenTarget, {"alpha":0,"time":2});
	
}