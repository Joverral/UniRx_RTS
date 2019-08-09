using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifetimePoolable : MonoBehaviour {

    [Header("Life time settings")]
    [Tooltip("Life time of this gameobject. (Sec)")]
    public float lifeTime = 2.0f;

    void Start()
    {
        GameObjectPooler.Current.PoolObject(this.gameObject, lifeTime);
    }
    
	
	
}
