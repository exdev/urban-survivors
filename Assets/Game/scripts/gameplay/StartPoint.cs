// ======================================================================================
// File         : StartPoint.cs
// Author       : Wu Jie 
// Last Change  : 11/08/2010 | 00:20:15 AM | Monday,November
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////
// defines
///////////////////////////////////////////////////////////////////////////////

public class StartPoint : MonoBehaviour {
    // DISABLE: we should only setup in Game { 
    // void Awake () {
    //     StartCoroutine( PlacePlayer() );
    // }

    // IEnumerator PlacePlayer () {
    //     yield return new WaitForEndOfFrame();

    //     GameObject boy = Game.PlayerBoy();
    //     if (boy) {
    //         boy.transform.position = transform.position;
    //         boy.transform.rotation = transform.rotation;
    //     }

    //     GameObject girl = Game.PlayerGirl();
    //     if (girl) {
    //         girl.transform.position = transform.position - boy.transform.forward * 2.0f;
    //         girl.transform.rotation = transform.rotation;
    //     }
    //     Camera.main.transform.position = new Vector3(transform.position.x, 20.0f, transform.position.z); 
    // }
    // } DISABLE end 
}
