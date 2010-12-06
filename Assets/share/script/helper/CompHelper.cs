// ======================================================================================
// File         : CompHelper.cs
// Author       : Wu Jie 
// Last Change  : 11/03/2010 | 23:16:04 PM | Wednesday,November
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Reflection;

///////////////////////////////////////////////////////////////////////////////
// defines
///////////////////////////////////////////////////////////////////////////////

public class CompHelper {

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public bool Copy ( Component _src, Component _dest ) {
        if ( _src.GetType() != _dest.GetType() ) {
            Debug.LogWarning ( "the type of src component and dest component are different" );
            return false;
        }

        // Animation
        if ( _src.GetType() == typeof(Animation) ) {
            return CopyAnimation ( _src as Animation, _dest as Animation );
        } 
        // Rigidbody
        else if ( _src.GetType() == typeof(Rigidbody) ) {
            return CopyRigidbody ( _src as Rigidbody, _dest as Rigidbody );
        }
        // BoxCollider
        else if ( _src.GetType() == typeof(BoxCollider) ) {
            return CopyBoxCollider ( _src as BoxCollider, _dest as BoxCollider );
        }
        // CapsuleCollider
        else if ( _src.GetType() == typeof(CapsuleCollider) ) {
            return CopyCapsuleCollider ( _src as CapsuleCollider, _dest as CapsuleCollider );
        }
        // CharacterController
        else if ( _src.GetType() == typeof(CharacterController) ) {
            return CopyCharacterController ( _src as CharacterController, _dest as CharacterController );
        }
        // CharacterJoint
        else if ( _src.GetType() == typeof(CharacterJoint) ) {
            return CopyCharacterJoint ( _src as CharacterJoint, _dest as CharacterJoint );
        }
        // MeshCollider
        else if ( _src.GetType() == typeof(MeshCollider) ) {
            return CopyMeshCollider ( _src as MeshCollider, _dest as MeshCollider );
        }
        // SphereCollider
        else if ( _src.GetType() == typeof(SphereCollider) ) {
            return CopySphereCollider ( _src as SphereCollider, _dest as SphereCollider );
        }
        // SkinnedMeshRenderer
        else if ( _src.GetType() == typeof(SkinnedMeshRenderer) ) {
            return CopySkinnedMeshRenderer ( _src as SkinnedMeshRenderer, _dest as SkinnedMeshRenderer );
        }
        // Transform NOTE: I don't think we need to copy this in most of the time.
        else if ( _src.GetType() == typeof(Transform) ) {
            // return CopyTransform ( _src as Transform, _dest as Transform );
            return true;
        }
        // WheelCollider
        else if ( _src.GetType() == typeof(WheelCollider) ) {
            return CopyWheelCollider ( _src as WheelCollider, _dest as WheelCollider );
        }

        // if the component is not the type above.
        CopyCommon ( _src, _dest );
        return true;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static bool CopyCommon ( Component _src, Component _dest ) {
        // get src and dest root
        Transform src_root = _src.transform;
        while ( src_root.parent != null ) {
            src_root = src_root.parent;
        }
        Transform dest_root = _dest.transform;
        while ( dest_root.parent != null ) {
            dest_root = dest_root.parent;
        }

        //
        foreach ( FieldInfo f in _src.GetType().GetFields() ) {
            //if the value in the script is a gameObject, and something is assigned to it...
            if ( f.GetValue(_src) != null ) {
                if ( f.GetValue(_src).GetType() == typeof(GameObject) ) {

                    //if the value is a child of the original source, so in the same hierarcy,
                    //then we should find the same object in the destination, and set that one
                    //because if the value points to my head, i dont want my clone's value to point to my head too
                    //i want it to point to its own head.
                    GameObject go = f.GetValue(_src) as GameObject;
                    if ( go && go.transform.IsChildOf(src_root) )
                    {
                        string path = "";
                        while( go != src_root.gameObject ) {
                            if (path=="") path = go.name;
                            else path = go.name+"/"+path;
                            go = go.transform.parent.gameObject;
                        }
                        Transform relativeTrans = dest_root.Find(path);
                        if (relativeTrans)
                            f.SetValue(_dest, relativeTrans.gameObject);
                        else if ( f.GetValue(_src) == src_root.gameObject ){
                            f.SetValue(_dest, dest_root.gameObject);
                        }
                        else{
                            f.SetValue(_dest, f.GetValue(_src));
                        }
                    }
                    else
                        f.SetValue(_dest, f.GetValue(_src));
                }
                else if ( f.GetValue(_src).GetType() == typeof(Transform) ){
                    //same as at the gameobject
                    Transform trans = f.GetValue(_src) as Transform;
                    if ( trans && trans.IsChildOf(src_root) )
                    {
                        string path = "";
                        while( trans != src_root ) {
                            if (path=="") path = trans.name;
                            else path = trans.name+"/"+path;
                            trans = trans.parent;
                        }
                        Transform relativeTrans = dest_root.Find(path);
                        if (relativeTrans)
                            f.SetValue(_dest, relativeTrans);
                        else if ( f.GetValue(_src) == src_root ) {
                            f.SetValue(_dest, dest_root);
                        }
                        else {
                            f.SetValue(_dest, f.GetValue(_src));
                        }
                    }
                    else
                        f.SetValue(_dest, f.GetValue(_src));
                }
                else if ( f.GetValue(_src).GetType() == typeof(Rigidbody) ){
                    //if the value is a child of the original source, so in the same hierarcy,
                    //then we should find the same object in the destination, and set that one
                    //because if the value points to my head, i dont want my clone's value to point to my head too
                    //i want it to point to its own head.
                    Rigidbody rg = f.GetValue(_src) as Rigidbody; 
                    if ( rg && rg.transform.IsChildOf(src_root) )
                    {
                        string path = "";
                        Transform trans = rg.transform;
                        while( rg != src_root.rigidbody ) {
                            if (path=="") path = trans.name;
                            else path = trans.name+"/"+path;
                            trans = trans.parent;
                        }
                        Transform relativeTrans = dest_root.Find(path);
                        if (relativeTrans)
                            f.SetValue(_dest, relativeTrans.rigidbody);
                        else if (f.GetValue(_src)==src_root.rigidbody){
                            f.SetValue(_dest, dest_root.rigidbody);
                        }
                        else{
                            f.SetValue(_dest, f.GetValue(_src));
                        }
                    }
                    else
                        f.SetValue(_dest, f.GetValue(_src));
                }
                else {
                    f.SetValue(_dest, f.GetValue(_src));
                }
            }
            else {
                f.SetValue(_dest, f.GetValue(_src));
            }
        }

        return true;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static bool CopyAnimation ( Animation _src, Animation _dest ) {
        foreach ( AnimationState state in _src ) {
            _dest.AddClip(_src[state.name].clip, state.name);
        }
        _dest.clip=_src.clip;
        _dest.playAutomatically = _src.playAutomatically;
        _dest.animatePhysics = _src.animatePhysics;
        _dest.animateOnlyIfVisible = _src.animateOnlyIfVisible;
        return true;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static bool CopyRigidbody ( Rigidbody _src, Rigidbody _dest ) {
        _dest.mass=_src.mass;
        _dest.drag=_src.drag;
        _dest.angularDrag=_src.angularDrag;
        _dest.useGravity=_src.useGravity;
        _dest.isKinematic=_src.isKinematic;
        _dest.interpolation=_src.interpolation;
        _dest.freezeRotation=_src.freezeRotation;
        _dest.collisionDetectionMode=_src.collisionDetectionMode;
        return true;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static bool CopyBoxCollider ( BoxCollider _src, BoxCollider _dest ) {
        _dest.material=_src.material;
        _dest.isTrigger=_src.isTrigger;
        _dest.size=_src.size;
        _dest.center=_src.center;
        return true;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static bool CopyCapsuleCollider ( CapsuleCollider _src, CapsuleCollider _dest ) {
        _dest.material=_src.material;
        _dest.isTrigger=_src.isTrigger;
        _dest.radius=_src.radius;
        _dest.height=_src.height;
        _dest.direction=_src.direction;
        _dest.center=_src.center;
        return true;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static bool CopyCharacterController ( CharacterController _src, CharacterController _dest ) {
        _dest.height=_src.height;
        _dest.radius=_src.radius;
        _dest.slopeLimit=_src.slopeLimit;
        _dest.stepOffset=_src.stepOffset;
        // _dest.skinWidth=_src.skinWidth;
        // _dest.minMoveDistance=_src.minMoveDistance;
        _dest.center=_src.center;
        return true;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static bool CopyMeshCollider ( MeshCollider _src, MeshCollider _dest ) {
        _dest.material=_src.material;
        _dest.isTrigger=_src.isTrigger;
        _dest.smoothSphereCollisions=_src.smoothSphereCollisions;
        _dest.convex=_src.convex;
        _dest.sharedMesh=_src.sharedMesh;
        return true;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static bool CopySphereCollider ( SphereCollider _src, SphereCollider _dest ) {
        _dest.material=_src.material;
        _dest.isTrigger=_src.isTrigger;
        _dest.radius=_src.radius;
        _dest.center=_src.center;
        return true;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static bool CopySkinnedMeshRenderer ( SkinnedMeshRenderer _src, SkinnedMeshRenderer _dest ) {
        _dest.castShadows =_src.castShadows;
        _dest.receiveShadows =_src.receiveShadows;
        _dest.sharedMaterials = _src.sharedMaterials;
        _dest.quality = _src.quality;
        _dest.updateWhenOffscreen = _src.updateWhenOffscreen;
        _dest.skinNormals = _src.skinNormals;
        _dest.sharedMesh = _src.sharedMesh;
        return true;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static bool CopyWheelCollider ( WheelCollider _src, WheelCollider _dest ) {
        _dest.center=_src.center;
        _dest.radius=_src.radius;
        _dest.suspensionDistance=_src.suspensionDistance;
        _dest.suspensionSpring=_src.suspensionSpring;
        _dest.mass=_src.mass;
        _dest.forwardFriction=_src.forwardFriction;
        _dest.sidewaysFriction=_src.sidewaysFriction;
        return true;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static bool CopyTransform ( Transform _src, Transform _dest ) {
        _dest.localPosition =_src.localPosition;
        _dest.localRotation =_src.localRotation;
        _dest.localScale =_src.localScale;
        return true;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static bool CopyCharacterJoint ( CharacterJoint _src, CharacterJoint _dest ) {
        // get src and dest root
        Transform src_root = _src.transform;
        while ( src_root.parent != null ) {
            src_root = src_root.parent;
        }
        Transform dest_root = _dest.transform;
        while ( dest_root.parent != null ) {
            dest_root = dest_root.parent;
        }

        //
        if ( _src.connectedBody != null ) {
            if ( _src.connectedBody.transform.IsChildOf(src_root) )
            {
                string path = "";
                Transform trans = _src.connectedBody.transform;
                while(trans.rigidbody != src_root.rigidbody) {
                    if ( path == "" ) path = trans.name;
                    else path = trans.name + "/" + path;
                    trans = trans.parent;
                }
                Transform relativeTrans = dest_root.Find(path);
                if (relativeTrans)
                    _dest.connectedBody = relativeTrans.rigidbody;
                else if (_src.connectedBody == src_root.rigidbody) {
                    _dest.connectedBody = dest_root.rigidbody;
                }
                else{
                    _dest.connectedBody=_src.connectedBody;
                }
            }
            else
                _dest.connectedBody=_src.connectedBody;
        }
        else
            _dest.connectedBody=_src.connectedBody;

        _dest.anchor=_src.anchor;
        _dest.axis=_src.axis;
        _dest.swingAxis=_src.swingAxis;
        _dest.lowTwistLimit=_src.lowTwistLimit;
        _dest.highTwistLimit=_src.highTwistLimit;
        _dest.swing1Limit=_src.swing1Limit;
        _dest.swing2Limit=_src.swing2Limit;
        _dest.breakForce=_src.breakForce;
        _dest.breakTorque=_src.breakTorque;
        return true;
    }
}
