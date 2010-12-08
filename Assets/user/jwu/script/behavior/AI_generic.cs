// ======================================================================================
// File         : AI_generic.cs
// Author       : Wu Jie 
// Last Change  : 09/27/2010 | 22:29:56 PM | Monday,September
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

public class AI_generic : MonoBehaviour {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    // DISABLE var btree : BehaveTree;

    public float HP = 100.0f;

    public float move_speed = 1.0f;
    public float rot_speed = 4.0f;

    public bool state_move = false;
    public bool state_attack = false;

    public Transform atkWrapper;
    public Transform atkAttachedBone;

    private Vector3 wanted_pos;
    private Quaternion wanted_rot;
    private Transform target;
    private CharacterController controller = null;

    ///////////////////////////////////////////////////////////////////////////////
    // defines
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Start () {
        // DISABLE { 
        // btree = new BehaveTree("MyBehave");
        // btree.init( 
        //     BTSeq( [ BT_move() ] ) 
        // );
        // } DISABLE end 

        controller = GetComponent<CharacterController>();

        DebugHelper.Assert(atkWrapper, "attack wrapper not assigned");
        DebugHelper.Assert(atkAttachedBone, "attack attached bone not assigned");
        atkWrapper.parent = atkAttachedBone;
        atkWrapper.gameObject.active = false;

        // init ai
        wanted_pos = transform.position;
        wanted_rot = transform.rotation;

        // StartCoroutine(GetRandomDest(2.0));
        StartCoroutine(GetNearestPlayerPos(2.0f));
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    IEnumerator GetNearestPlayerPos ( float _tickTime ) {
        while ( true ) {
            GameObject[] players = GameRules.Instance().GetPlayers();
            float nearest = 999.0f;
            foreach( GameObject player in players ) {
                float len = (player.transform.position - transform.position).magnitude;
                if ( len < nearest ) {
                    nearest = len;
                    wanted_pos = player.transform.position;
                    target = player.transform;
                }
            }
            yield return new WaitForSeconds (_tickTime);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    IEnumerator GetRandomDest( float _tickTime ) {
        while ( true ) {
            Vector3 delta = wanted_pos - transform.position;
            if ( delta.magnitude < 0.01f ) {
                wanted_pos = new Vector3( 
                                     Random.Range(-10.0f,10.0f), 
                                     0.0f,
                                     Random.Range(-10.0f,10.0f) 
                                    );
            }
            else {
                wanted_pos = new Vector3( 
                                     Random.Range(-10.0f,10.0f), 
                                     0.0f,
                                     Random.Range(-10.0f,10.0f) 
                                    );
                yield return new WaitForSeconds (_tickTime);
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Update () {
        // DISABLE { 
        // btree.tick();
        // } DISABLE end 

        // reset the state
        state_attack = false;
        state_move = false;

        // move ai
        Vector3 delta = target.position - transform.position;
        if ( delta.magnitude >= 1.5f ) {
            // transform.position += delta.normalized * move_speed * Time.deltaTime;
            // transform.position += transform.forward * move_speed * Time.deltaTime;
            state_move = true;
        }
        else {
            state_move = false;
            state_attack = true;
        }

        // apply gravity
        Vector3 vel = transform.forward * move_speed;
        if ( controller.isGrounded == false ) {
            vel.y = -10.0f;
        }
        controller.Move(vel * Time.deltaTime);

        // rotate ai
        wanted_rot = Quaternion.LookRotation( target.position - transform.position );
        wanted_rot.x = 0.0f; wanted_rot.z = 0.0f;
        transform.rotation = Quaternion.Slerp ( 
                                               transform.rotation, 
                                               wanted_rot, 
                                               rot_speed * Time.deltaTime
                                              );

        // material
        Renderer r = transform.Find("zombieGirl").renderer;
        r.material.color = new Color ( 1.0f, HP/100.0f, HP/100.0f );
        if ( HP <= 0.0f ) {
            Destroy(gameObject);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	void AttackOn (){
        atkWrapper.gameObject.active = true;
	}
	
    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	void AttackOff (){
        atkWrapper.gameObject.active = false;
	}
	
}


