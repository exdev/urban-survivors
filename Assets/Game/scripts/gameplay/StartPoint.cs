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
    // DISABLE: we should only setup in GameRules { 
    // void Awake () {
    //     StartCoroutine( PlacePlayer() );
    // }

    // IEnumerator PlacePlayer () {
    //     yield return new WaitForEndOfFrame();

    //     GameObject boy = GameRules.Instance().GetPlayerBoy();
    //     if (boy) {
    //         boy.transform.position = transform.position;
    //         boy.transform.rotation = transform.rotation;
    //     }

    //     GameObject girl = GameRules.Instance().GetPlayerGirl();
    //     if (girl) {
    //         girl.transform.position = transform.position - boy.transform.forward * 2.0f;
    //         girl.transform.rotation = transform.rotation;
    //     }
    //     Camera.main.transform.position = new Vector3(transform.position.x, 20.0f, transform.position.z); 
    // }
    // } DISABLE end 
}
