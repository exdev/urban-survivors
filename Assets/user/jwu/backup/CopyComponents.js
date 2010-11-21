import System.Reflection;

class CopyComponents extends EditorWindow {
private var fromTrans:Transform;
private var toTrans:Transform;
var anims = false;
var physics = true;
var scripts = true;

private var errorMsg:String;

@MenuItem ("Component/Custom/Copy components")
static function Init () {
	var window : CopyComponents = EditorWindow.GetWindow (CopyComponents);
	window.Show ();
}

function OnGUI () {
	EditorGUILayout.Space();
	EditorGUILayout.Space();
	fromTrans = EditorGUILayout.ObjectField("From: ",fromTrans,Transform);
	toTrans = EditorGUILayout.ObjectField("To: ",toTrans,Transform);
	EditorGUILayout.Space();
	EditorGUILayout.BeginHorizontal ();
	GUILayout.Label ("With animations");
	anims = EditorGUILayout.Toggle (anims);
	EditorGUILayout.EndHorizontal ();
	EditorGUILayout.BeginHorizontal ();
	GUILayout.Label ("With physics+joints");
	physics = EditorGUILayout.Toggle (physics);
	EditorGUILayout.EndHorizontal ();
	EditorGUILayout.BeginHorizontal ();
	GUILayout.Label ("With scripts+controller");
	scripts = EditorGUILayout.Toggle (scripts);
	EditorGUILayout.EndHorizontal ();
	EditorGUILayout.Space();
	EditorGUILayout.Space();
	errorMsg="";
	if (!fromTrans)
		errorMsg+="\"From\" not yet set!";
	if (!toTrans)
		errorMsg+=" \n\"To\" not yet set!";
	if (fromTrans&&toTrans){
		if (!checkChildrenRecurse(fromTrans,toTrans,"")){
			GUILayout.Label ("Warning: ", EditorStyles.boldLabel);
		}
		if (errorMsg!=""){
			GUILayout.Label(errorMsg);
		}
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.Space();
		if (GUILayout.Button ("Copy!")){
			copyComponentsRecurse(fromTrans,toTrans,fromTrans,toTrans);
		}
		EditorGUILayout.Space();
		EditorGUILayout.EndHorizontal ();
	}
	else
	{
		if (errorMsg!=""){
			GUILayout.Label ("Warning: ", EditorStyles.boldLabel);
			GUILayout.Label(errorMsg);
		}
	}
}
}

function copyComponentsRecurse (src : Transform,  dst : Transform, originalSrc : Transform,  originalDst : Transform)
{
	//Delete stuff from destination
	
	dst.tag=src.tag;
	dst.gameObject.layer=src.gameObject.layer;
	
	var toComps:Component[] = dst.GetComponents(Component);
	for (var dstComponent:Component in toComps){
		if (dstComponent.GetType()==Animation&&anims)
			DestroyImmediate(dstComponent);
		else if (dstComponent.GetType()==Rigidbody&&physics){
			if (!dst.GetComponent(CharacterJoint))
				DestroyImmediate(dstComponent);
		}
		else if (dstComponent.GetType()==BoxCollider&&physics)
			DestroyImmediate(dstComponent);
		else if (dstComponent.GetType()==SphereCollider&&physics)
			DestroyImmediate(dstComponent);
		else if (dstComponent.GetType()==CapsuleCollider&&physics)
			DestroyImmediate(dstComponent);
		else if (dstComponent.GetType()==MeshCollider&&physics)
			DestroyImmediate(dstComponent);
		else if (dstComponent.GetType()==CharacterController&&scripts)
			DestroyImmediate(dstComponent);
		else if (dstComponent.GetType()==CharacterJoint&&physics)
			DestroyImmediate(dstComponent);
		else if (dstComponent.GetType().BaseType==MonoBehaviour&&scripts)
			DestroyImmediate(dstComponent);
	}
	
	//Copy stuff from source to destination
	var copyComps:Component[] = src.GetComponents(Component);
	for (var srcComponent:Component in copyComps){
		if (srcComponent.GetType()==Animation&&anims){
			var new_animation:Animation= dst.gameObject.AddComponent(Animation);
			var helpsource:Animation=srcComponent;
			for (var state : AnimationState in helpsource)
			{
				new_animation.AddClip(helpsource[state.name].clip, state.name);
			}
			new_animation.clip=helpsource.clip;
			new_animation.playAutomatically = helpsource.playAutomatically;
			new_animation.animatePhysics = helpsource.animatePhysics;
			new_animation.animateOnlyIfVisible = helpsource.animateOnlyIfVisible;
		}
		else if (srcComponent.GetType()==Rigidbody&&physics){
			//var ialreadyhavethis:Rigidbody=dst.GetComponent(Rigidbody);
			var new_rigid:Rigidbody=dst.GetComponent(Rigidbody);
			if (!new_rigid)
				new_rigid= dst.gameObject.AddComponent(Rigidbody);
			var helpsource2:Rigidbody=srcComponent;
			new_rigid.mass=helpsource2.mass;
			new_rigid.drag=helpsource2.drag;
			new_rigid.angularDrag=helpsource2.angularDrag;
			new_rigid.useGravity=helpsource2.useGravity;
			new_rigid.isKinematic=helpsource2.isKinematic;
			new_rigid.interpolation=helpsource2.interpolation;
			new_rigid.freezeRotation=helpsource2.freezeRotation;
		}
		else if (srcComponent.GetType()==BoxCollider&&physics){
			var new_box:BoxCollider= dst.gameObject.AddComponent(BoxCollider);
			var helpsource3:BoxCollider=srcComponent;
			new_box.material=helpsource3.material;
			new_box.isTrigger=helpsource3.isTrigger;
			new_box.size=helpsource3.size;
			new_box.center=helpsource3.center;
		}
		else if (srcComponent.GetType()==SphereCollider&&physics){
			var new_sphere:SphereCollider= dst.gameObject.AddComponent(SphereCollider);
			var helpsource4:SphereCollider=srcComponent;
			new_sphere.material=helpsource4.material;
			new_sphere.isTrigger=helpsource4.isTrigger;
			new_sphere.radius=helpsource4.radius;
			new_sphere.center=helpsource4.center;
		}
		else if (srcComponent.GetType()==CapsuleCollider&&physics){
			var new_capsule:CapsuleCollider= dst.gameObject.AddComponent(CapsuleCollider);
			var helpsource5:CapsuleCollider=srcComponent;
			new_capsule.material=helpsource5.material;
			new_capsule.isTrigger=helpsource5.isTrigger;
			new_capsule.radius=helpsource5.radius;
			new_capsule.height=helpsource5.height;
			new_capsule.direction=helpsource5.direction;
			new_capsule.center=helpsource5.center;
		}
		else if (srcComponent.GetType()==CharacterController&&scripts){
			var new_characterC:CharacterController= dst.GetComponent(CharacterController);
			if (!new_characterC)
				new_characterC=dst.gameObject.AddComponent(CharacterController);
			var helpsource6:CharacterController=srcComponent;
			new_characterC.height=helpsource6.height;
			new_characterC.radius=helpsource6.radius;
			new_characterC.slopeLimit=helpsource6.slopeLimit;
			new_characterC.stepOffset=helpsource6.stepOffset;
			new_characterC.center=helpsource6.center;
		}
		else if (srcComponent.GetType()==CharacterJoint&&physics){
			var new_joint:CharacterJoint= dst.gameObject.AddComponent(CharacterJoint);
			var helpsource7:CharacterJoint=srcComponent;
			if (helpsource7.connectedBody!=null){
				if (helpsource7.connectedBody.transform.IsChildOf(originalSrc))
				{
					var myPath4:String="";
					var thisIsMe4:Transform=helpsource7.connectedBody.transform;
					while(thisIsMe4.rigidbody!=originalSrc.rigidbody) {
						if (myPath4=="") myPath4=thisIsMe4.name;
						else myPath4=thisIsMe4.name+"/"+myPath4;
						thisIsMe4=thisIsMe4.parent;
					}
					var relativeValue4:Transform=originalDst.Find(myPath4);
					if (relativeValue4)
						new_joint.connectedBody=relativeValue4.rigidbody;
					else if (helpsource7.connectedBody==originalSrc.rigidbody){
						new_joint.connectedBody=originalDst.rigidbody;
					}
					else{
						new_joint.connectedBody=helpsource7.connectedBody;
					}
				}
				else
					new_joint.connectedBody=helpsource7.connectedBody;
			}
			else
				new_joint.connectedBody=helpsource7.connectedBody;
				
			new_joint.anchor=helpsource7.anchor;
			new_joint.axis=helpsource7.axis;
			new_joint.swingAxis=helpsource7.swingAxis;
			new_joint.lowTwistLimit=helpsource7.lowTwistLimit;
			new_joint.highTwistLimit=helpsource7.highTwistLimit;
			new_joint.swing1Limit=helpsource7.swing1Limit;
			new_joint.swing2Limit=helpsource7.swing2Limit;
			new_joint.breakForce=helpsource7.breakForce;
			new_joint.breakTorque=helpsource7.breakTorque;
		}
		//if its a script
		else if (srcComponent.GetType().BaseType==MonoBehaviour&&scripts){
			var new_script = dst.gameObject.AddComponent(srcComponent.GetType());
			for(var f: FieldInfo in srcComponent.GetType().GetFields())
			{
				//if the value in the script is a gameObject, and something is assigned to it...
				if (f.GetValue(srcComponent)!=null){
					if (f.GetValue(srcComponent).GetType()==GameObject){
						//if the value is a child of the original source, so in the same hierarcy,
						//then we should find the same object in the destination, and set that one
						//because if the value points to my head, i dont want my clone's value to point to my head too
						//i want it to point to its own head.
						if (f.GetValue(srcComponent).transform.IsChildOf(originalSrc))
						{
							var myPath:String="";
							var thisIsMe:GameObject=f.GetValue(srcComponent);
							while(thisIsMe!=originalSrc.gameObject) {
								if (myPath=="") myPath=thisIsMe.name;
								else myPath=thisIsMe.name+"/"+myPath;
								thisIsMe=thisIsMe.transform.parent.gameObject;
							}
							var relativeValue:Transform=originalDst.Find(myPath);
							if (relativeValue)
								f.SetValue(new_script, relativeValue.gameObject);
							else if (f.GetValue(srcComponent)==originalSrc.gameObject){
								f.SetValue(new_script, originalDst.gameObject);
							}
							else{
								f.SetValue(new_script, f.GetValue(srcComponent));
							}
						}
						else
							f.SetValue(new_script, f.GetValue(srcComponent));
					}
					else if (f.GetValue(srcComponent).GetType()==Transform){
						//same as at the gameobject
						if (f.GetValue(srcComponent).IsChildOf(originalSrc))
						{
							var myPath2:String="";
							var thisIsMe2:Transform=f.GetValue(srcComponent);
							while(thisIsMe2!=originalSrc) {
								if (myPath2=="") myPath2=thisIsMe2.name;
								else myPath2=thisIsMe2.name+"/"+myPath2;
								thisIsMe2=thisIsMe2.parent;
							}
							var relativeValue2:Transform=originalDst.Find(myPath2);
							if (relativeValue2)
								f.SetValue(new_script, relativeValue2);
							else if (f.GetValue(srcComponent)==originalSrc){
								f.SetValue(new_script, originalDst);
							}
							else{
								f.SetValue(new_script, f.GetValue(srcComponent));
							}
						}
						else
							f.SetValue(new_script, f.GetValue(srcComponent));
					}
					else if (f.GetValue(srcComponent).GetType()==Rigidbody){
						//if the value is a child of the original source, so in the same hierarcy,
						//then we should find the same object in the destination, and set that one
						//because if the value points to my head, i dont want my clone's value to point to my head too
						//i want it to point to its own head.
						if (f.GetValue(srcComponent).transform.IsChildOf(originalSrc))
						{
							var myPath3:String="";
							var thisIsMe3:Transform=f.GetValue(srcComponent).transform;
							while(thisIsMe3.rigidbody!=originalSrc.rigidbody) {
								if (myPath3=="") myPath3=thisIsMe3.name;
								else myPath3=thisIsMe3.name+"/"+myPath3;
								thisIsMe3=thisIsMe3.parent;
							}
							var relativeValue3:Transform=originalDst.Find(myPath3);
							if (relativeValue3)
								f.SetValue(new_script, relativeValue3.rigidbody);
							else if (f.GetValue(srcComponent)==originalSrc.rigidbody){
								f.SetValue(new_script, originalDst.rigidbody);
							}
							else{
								f.SetValue(new_script, f.GetValue(srcComponent));
							}
						}
						else
							f.SetValue(new_script, f.GetValue(srcComponent));
					}
					else {
						f.SetValue(new_script, f.GetValue(srcComponent));
					}
				}
				else {
					f.SetValue(new_script, f.GetValue(srcComponent));
				}
			}
		}
	}
	
	//make the recurse thingy
	for (var child : Transform in src) {
		var curDst : Transform = dst.Find(child.name);
		if (curDst){
			copyComponentsRecurse(child, curDst, originalSrc, originalDst);
		}
	}
}

function checkChildrenRecurse (src : Transform,  dst : Transform, srcPath:String) : boolean
{
	if (src.childCount!=dst.childCount){
		errorMsg="Child count of "+srcPath+src.gameObject.name+" in \"From\" is not equal to "+dst.gameObject.name+" in \"To\".\n";
		return false;
	}
	else if (src.childCount>0)
	{
		var okay:boolean=true;
		for (var child : Transform in src) {
			var curDst : Transform = dst.Find(child.name);
			if (curDst){
				if (!checkChildrenRecurse(child, curDst, srcPath+child.gameObject.name+"\\"))
				{
					okay=false;
				}
			}
			else{
				errorMsg="Can't find "+src.gameObject.name+"\\"+srcPath+child.gameObject.name+" in \"To\"\n";
				return false;
			}
		}
		return okay;
	}
	else{
		return true;
	}
}
