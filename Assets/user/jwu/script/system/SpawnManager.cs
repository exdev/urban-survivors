// ======================================================================================
// File         : SpawnManager.cs
// Author       : Wu Jie 
// Last Change  : 09/23/2010 | 08:58:12 AM | Thursday,September
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

///////////////////////////////////////////////////////////////////////////////
// defines
///////////////////////////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////////////
// class GameObjectPool 
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

[System.Serializable]
public class GameObjectPool
{
    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public int size;
    public GameObject prefab;  

    private GameObject[] all;
    private Stack available;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void init () {
        available = new Stack(size);
        all = new GameObject[size]; 
        for ( int i = 0; i < size; ++i ) {
            GameObject obj = (GameObject)GameObject.Instantiate(prefab,Vector3.zero, Quaternion.identity);
            this.SetActive( obj, false );
            available.Push(obj);
            all[i] = obj;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void SetActive ( GameObject _gameObj, bool _val ) {
        _gameObj.SetActiveRecursively(_val);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public GameObject Request ( Vector3 _pos, Quaternion _rot ) {
        // try to reclaim possible objects.
        if ( available.Count == 0 ) {
            Reclaim();
        }

        //
        if ( available.Count == 0 ) {
            Debug.Log("warnning, out of item!");
            return null;
        }
        else {
            GameObject result = (GameObject)available.Pop();
            result.transform.position = _pos;
            result.transform.rotation = _rot;
            this.SetActive( result, true );
            result.SendMessage("Start");
            return result;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public bool Return ( GameObject _obj ) {
        if ( available.Contains(_obj) ) {
            available.Push(_obj);
            this.SetActive(_obj, false);
            return true;
        }
        return false;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void ReturnAll () {
        foreach ( GameObject obj in all ) {
            if ( obj.active ) {
                Return (obj);
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void Clear () {
        for ( int i = 0; i < size; ++i ) {
            GameObject.Destroy(all[i]);
            all[i] = null;
        }
        available.Clear();
        size = 0;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void Reclaim () {
        foreach ( GameObject obj in all ) {
            if ( obj.active == false && available.Contains(obj) == false ) {
                available.Push(obj);
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    int GetActiveCount() { return all.Length - available.Count; }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    int GetAvailableCount() { return available.Count; }
}

///////////////////////////////////////////////////////////////////////////////
// class SpawnManager
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

public class SpawnManager : MonoBehaviour {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public GameObjectPool[] pools;

    protected static SpawnManager spawnManager = null;
    private Dictionary<GameObject,GameObjectPool> gameobjLookupTable = new Dictionary<GameObject,GameObjectPool>(); 

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public static SpawnManager Instance() {
        return spawnManager;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public GameObject Spawn ( GameObject _prefab, Vector3 _pos, Quaternion _rot ) {
        if ( gameobjLookupTable.ContainsKey(_prefab) )
            return gameobjLookupTable[_prefab].Request(_pos,_rot);
        else
            return null;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void Destroy ( GameObject _obj ) {
        _obj.SetActiveRecursively(false);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Awake () {
        if( spawnManager == null ) {
            spawnManager = this;

            // init spawn manager.
            for ( int i = 0; i < pools.Length; ++i ) {
                pools[i].init();
                gameobjLookupTable[pools[i].prefab] = pools[i];
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	void Start () {
	
	}
	
    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	void Update () {
	
	}
}
