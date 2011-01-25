var loadingAnimTar : PackedSprite;

function onTapStart () {
	Debug.Log("loading level");
	loadMainLevel(1);
}

function loadMainLevel(levelIndex: int) {


	Application.LoadLevel(levelIndex);


}

